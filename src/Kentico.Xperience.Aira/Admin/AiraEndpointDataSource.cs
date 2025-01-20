using System.Reflection;
using System.Text.Json;

using CMS.DataEngine;

using Kentico.Xperience.Aira.Admin.InfoModels;
using Kentico.Xperience.Aira.Authentication;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

namespace Kentico.Xperience.Aira.Admin;

internal class AiraEndpointDataSource : MutableEndpointDataSource
{
    private readonly IInfoProvider<AiraConfigurationItemInfo> airaConfigurationProvider;

    public AiraEndpointDataSource(IInfoProvider<AiraConfigurationItemInfo> airaConfigurationProvider)
        : base(new CancellationTokenSource(), new CancellationChangeToken(new CancellationToken()))
        => this.airaConfigurationProvider = airaConfigurationProvider;

    public void UpdateEndpoints()
        => SetEndpoints(MakeEndpoints());

    private IReadOnlyList<Endpoint> MakeEndpoints()
    {
        var configuration = airaConfigurationProvider.Get().GetEnumerableTypedResult().SingleOrDefault();

        if (configuration is null)
        {
            return [];
        }

        if (string.IsNullOrEmpty(configuration.AiraConfigurationItemAiraPathBase))
        {
            return [];
        }

        return
        [
            CreateAiraEndpoint(configuration,
                AiraCompanionAppConstants.ChatRelativeUrl,
                nameof(AiraCompanionAppController.Index),
                controller => controller.Index()
            ),
            CreateAiraIFormCollectionEndpoint(configuration,
                $"{AiraCompanionAppConstants.ChatRelativeUrl}/{AiraCompanionAppConstants.ChatMessageUrl}",
                nameof(AiraCompanionAppController.PostChatMessage),
                (controller, request) => controller.PostChatMessage(request)
            ),
            CreateAiraIFormCollectionEndpoint(configuration,
                 $"{AiraCompanionAppConstants.SmartUploadRelativeUrl}/{AiraCompanionAppConstants.SmartUploadUploadUrl}",
                nameof(AiraCompanionAppController.PostImages),
                (controller, request) => controller.PostImages(request)),
            CreateAiraEndpoint(configuration,
                AiraCompanionAppConstants.SmartUploadRelativeUrl,
                nameof(AiraCompanionAppController.Assets),
                controller => controller.Assets()
            ),
            CreateAiraEndpoint<SignInViewModel>(configuration,
                AiraCompanionAppConstants.SigninRelativeUrl,
                nameof(AiraCompanionAppController.SignIn),
                controller => controller.Signin(),
                (controller, request) => controller.SignIn(request)
            )
        ];
    }
    private static Endpoint CreateAiraEndpoint<T>(AiraConfigurationItemInfo configurationInfo,
        string subPath,
        string actionName,
        Func<AiraCompanionAppController, Task<IActionResult>> paramlessAction,
        Func<AiraCompanionAppController, T, Task<IActionResult>> actionWithForm) where T : class, new()
        => CreateEndpoint($"{configurationInfo.AiraConfigurationItemAiraPathBase}/{subPath}", async context =>
        {
            var airaController = await GetAiraCompanionAppControllerInContext(context, actionName);

            if (context.Request.ContentType is not null &&
                context.Request.ContentType.Contains("application/x-www-form-urlencoded"))
            {
                var form = await context.Request.ReadFormAsync();
                var requestObject = new T();
                foreach (var key in form.Keys)
                {
                    var property = typeof(T).GetProperty(key, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                    var value = form[key].ToString();

                    property?.SetValue(requestObject, Convert.ChangeType(value, property.PropertyType));
                }

                var result = await actionWithForm.Invoke(airaController, requestObject);

                await result.ExecuteResultAsync(airaController.ControllerContext);
            }
            else if (context.Request.ContentLength > 0
                && string.Equals(context.Request.ContentType, "application/json"))
            {
                var requestObject = new T();
                using var reader = new StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();
                requestObject = JsonSerializer.Deserialize<T>(body, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })!;

                var result = await actionWithForm.Invoke(airaController, requestObject);

                await result.ExecuteResultAsync(airaController.ControllerContext);
            }
            else
            {
                var result = await paramlessAction.Invoke(airaController);

                await result.ExecuteResultAsync(airaController.ControllerContext);
            }
        });

    private static Endpoint CreateAiraEndpoint(AiraConfigurationItemInfo configurationInfo, string subPath, string actionName, Func<AiraCompanionAppController, Task<IActionResult>> action) =>
        CreateEndpoint($"{configurationInfo.AiraConfigurationItemAiraPathBase}/{subPath}", async context =>
        {
            var airaController = await GetAiraCompanionAppControllerInContext(context, actionName);

            var result = await action.Invoke(airaController);

            await result.ExecuteResultAsync(airaController.ControllerContext);
        });

    private static Endpoint CreateAiraIFormCollectionEndpoint(AiraConfigurationItemInfo configurationItemInfo, string subPath, string actionName, Func<AiraCompanionAppController, IFormCollection, Task<IActionResult>> action)
    => CreateEndpoint($"{configurationItemInfo.AiraConfigurationItemAiraPathBase}/{subPath}", async context =>
    {
        var airaController = await GetAiraCompanionAppControllerInContext(context, actionName);

        if (context.Request.ContentType is null)
        {
            return;
        }
        if (context.Request.ContentType.Contains("multipart/form-data"))
        {
            var requestObject = await context.Request.ReadFormAsync();
            var result = await action.Invoke(airaController, requestObject);
            await result.ExecuteResultAsync(airaController.ControllerContext);
        }
        else if (string.Equals(context.Request.ContentType, "application/json"))
        {
            using var reader = new StreamReader(context.Request.Body);
            var body = await reader.ReadToEndAsync();

            var formCollection = new FormCollection(new Dictionary<string, StringValues>
            {
                { "message", body }
            });

            var result = await action.Invoke(airaController, formCollection);
            await result.ExecuteResultAsync(airaController.ControllerContext);
        }
    });

    private static async Task<AiraCompanionAppController> GetAiraCompanionAppControllerInContext(HttpContext context, string actionName)
    {
        var controllerShortName = nameof(AiraCompanionAppController).Replace("Controller", string.Empty);

        var routeData = new RouteData();
        routeData.Values["controller"] = controllerShortName;
        routeData.Values["action"] = actionName;

        var actionDescriptor = new ControllerActionDescriptor
        {
            ControllerName = controllerShortName,
            ActionName = actionName,
            ControllerTypeInfo = typeof(AiraCompanionAppController).GetTypeInfo()
        };

        await AuthenticateAiraEndpoint(context);

        var actionContext = new ActionContext(context, routeData, actionDescriptor);
        var controllerContext = new ControllerContext(actionContext);
        var controllerFactory = context.RequestServices.GetRequiredService<IControllerFactory>();
        var airaController = (AiraCompanionAppController)controllerFactory.CreateController(controllerContext);

        airaController.ControllerContext = controllerContext;
        airaController.ControllerContext.HttpContext = context;

        return airaController;
    }

    private static async Task AuthenticateAiraEndpoint(HttpContext context)
    {
        if (context.User?.Identity is null || !context.User.Identity.IsAuthenticated)
        {
            var authenticateResult = await context.RequestServices
                .GetRequiredService<IAuthenticationService>()
                .AuthenticateAsync(context, AiraCompanionAppConstants.XperienceAdminSchemeName);

            if (authenticateResult.Succeeded && authenticateResult.Principal is not null)
            {
                context.User = authenticateResult.Principal;
            }
        }
    }

    private static Endpoint CreateEndpoint(string pattern, RequestDelegate requestDelegate) =>
        new RouteEndpointBuilder(
            requestDelegate: requestDelegate,
            routePattern: RoutePatternFactory.Parse(pattern),
            order: 0)
        .Build();
}
