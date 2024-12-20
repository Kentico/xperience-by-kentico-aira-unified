using CMS.DataEngine;

using Kentico.Xperience.Aira.Admin.InfoModels;

using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

using System.Reflection;
using System.Text.Json;
using Kentico.Xperience.Aira.Authentication;

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

        string controllerShortName = "AiraCompanionApp";

        return
        [
            CreateAiraEndpoint(configuration,
                "chat",
                controllerShortName,
                nameof(AiraCompanionAppController.Index),
                controller => controller.Index()
            ),
            CreateAiraIFormCollectionEndpoint(configuration,
                "chat/message",
                controllerShortName,
                nameof(AiraCompanionAppController.PostChatMessage),
                (controller, request) => controller.PostChatMessage(request)
            ),
            CreateAiraIFormCollectionEndpoint(configuration,
                "assets/upload",
                controllerShortName,
                nameof(AiraCompanionAppController.PostImages),
                (controller, request) => controller.PostImages(request)),
            CreateAiraEndpoint(configuration,
                "assets",
                controllerShortName,
                nameof(AiraCompanionAppController.Assets),
                controller => controller.Assets()
            ),
            CreateAiraEndpoint<SignInViewModel>(configuration,
                "signin",
                controllerShortName,
                nameof(AiraCompanionAppController.SignIn),
                controller => controller.Signin(),
                (controller, request) => controller.SignIn(request)
            )
            //),
            //CreateAiraEndpoint(configuration,
            //    "manifest.json",
            //    controllerShortName,
            //    nameof(AiraCompanionAppController.GetPwaManifest),
            //    controller => controller.GetPwaManifest()
            //)
        ];
    }
    private Endpoint CreateAiraEndpoint<T>(AiraConfigurationItemInfo configurationInfo,
        string subPath,
        string controllerName,
        string actionName,
        Func<AiraCompanionAppController, Task<IActionResult>> paramlessAction,
        Func<AiraCompanionAppController, T, Task<IActionResult>> actionWithForm) where T : class, new()
        => CreateEndpoint($"{configurationInfo.AiraConfigurationItemAiraPathBase}/{subPath}", async context =>
        {
            var routeData = new RouteData();
            routeData.Values["controller"] = controllerName;
            routeData.Values["action"] = actionName;

            var actionDescriptor = new ControllerActionDescriptor
            {
                ControllerName = controllerName,
                ActionName = actionName,
                ControllerTypeInfo = typeof(AiraCompanionAppController).GetTypeInfo()
            };

            var actionContext = new ActionContext(context, routeData, actionDescriptor);
            var controllerContext = new ControllerContext(actionContext);
            var controllerFactory = context.RequestServices.GetRequiredService<IControllerFactory>();
            object controller = controllerFactory.CreateController(controllerContext);

            if (controller is AiraCompanionAppController airaController)
            {
                airaController.ControllerContext = controllerContext;
                airaController.ControllerContext.HttpContext = context;

                if (context.Request.ContentType != null &&
                    context.Request.ContentType.Contains("application/x-www-form-urlencoded"))
                {
                    var form = await context.Request.ReadFormAsync(); // Parse form data
                    var requestObject = new T();
                    foreach (string key in form.Keys)
                    {
                        var property = typeof(T).GetProperty(key, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                        property?.SetValue(requestObject, Convert.ChangeType(form[key], property.PropertyType));
                    }

                    var result = await actionWithForm.Invoke(airaController, requestObject);

                    await result.ExecuteResultAsync(controllerContext);
                }
                else if (context.Request.ContentLength > 0 && context.Request.ContentType == "application/json")
                {
                    var requestObject = new T();
                    using var reader = new StreamReader(context.Request.Body);
                    string body = await reader.ReadToEndAsync();
                    requestObject = JsonSerializer.Deserialize<T>(body, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    })!;

                    var result = await actionWithForm.Invoke(airaController, requestObject);

                    await result.ExecuteResultAsync(controllerContext);
                }
                else
                {
                    var result = await paramlessAction.Invoke(airaController);

                    await result.ExecuteResultAsync(controllerContext);
                }
            }
        });
    private Endpoint CreateAiraEndpoint(AiraConfigurationItemInfo configurationInfo, string subPath, string controllerName, string actionName, Func<AiraCompanionAppController, Task<IActionResult>> action) =>
        CreateEndpoint($"{configurationInfo.AiraConfigurationItemAiraPathBase}/{subPath}", async context =>
        {
            var routeData = new RouteData();
            routeData.Values["controller"] = controllerName;
            routeData.Values["action"] = actionName;

            var actionDescriptor = new ControllerActionDescriptor
            {
                ControllerName = controllerName,
                ActionName = actionName,
                ControllerTypeInfo = typeof(AiraCompanionAppController).GetTypeInfo()
            };

            var actionContext = new ActionContext(context, routeData, actionDescriptor);
            var controllerContext = new ControllerContext(actionContext);
            var controllerFactory = context.RequestServices.GetRequiredService<IControllerFactory>();
            object controller = controllerFactory.CreateController(controllerContext);

            if (controller is AiraCompanionAppController airaController)
            {
                airaController.ControllerContext = controllerContext;
                airaController.ControllerContext.HttpContext = context;

                var result = await action.Invoke(airaController);

                await result.ExecuteResultAsync(controllerContext);
            }
        });
    private Endpoint CreateAiraIFormCollectionEndpoint(AiraConfigurationItemInfo configurationItemInfo, string subPath, string controllerName, string actionName, Func<AiraCompanionAppController, IFormCollection, Task<IActionResult>> action)
        => CreateEndpoint($"{configurationItemInfo.AiraConfigurationItemAiraPathBase}/{subPath}", async context =>
        {
            var routeData = new RouteData();
            routeData.Values["controller"] = controllerName;
            routeData.Values["action"] = actionName;

            var actionDescriptor = new ControllerActionDescriptor
            {
                ControllerName = controllerName,
                ActionName = actionName,
                ControllerTypeInfo = typeof(AiraCompanionAppController).GetTypeInfo()
            };

            var actionContext = new ActionContext(context, routeData, actionDescriptor);
            var controllerContext = new ControllerContext(actionContext);
            var controllerFactory = context.RequestServices.GetRequiredService<IControllerFactory>();
            object controller = controllerFactory.CreateController(controllerContext);

            if (controller is AiraCompanionAppController airaController)
            {
                airaController.ControllerContext = controllerContext;
                airaController.ControllerContext.HttpContext = context;

                if (context.Request.ContentType == null)
                {
                    return;
                }
                if (context.Request.ContentType.Contains("multipart/form-data"))
                {
                    var requestObject = await context.Request.ReadFormAsync();
                    var result = await action.Invoke(airaController, requestObject);
                    await result.ExecuteResultAsync(controllerContext);
                }
                else if (context.Request.ContentType == "application/json")
                {
                    using var reader = new StreamReader(context.Request.Body);
                    string body = await reader.ReadToEndAsync();

                    var formCollection = new FormCollection(new Dictionary<string, StringValues>
                    {
                        { "message", body }
                    });

                    var result = await action.Invoke(airaController, formCollection);
                    await result.ExecuteResultAsync(controllerContext);
                }
            }
        });
    private Endpoint CreateAiraEndpoint<T>(AiraConfigurationItemInfo configurationInfo, string subPath, string controllerName, string actionName, Func<AiraCompanionAppController, T, Task<IActionResult>> action) where T : class, new()
        => CreateEndpoint($"{configurationInfo.AiraConfigurationItemAiraPathBase}/{subPath}", async context =>
        {
            var routeData = new RouteData();
            routeData.Values["controller"] = controllerName;
            routeData.Values["action"] = actionName;

            var actionDescriptor = new ControllerActionDescriptor
            {
                ControllerName = controllerName,
                ActionName = actionName,
                ControllerTypeInfo = typeof(AiraCompanionAppController).GetTypeInfo()
            };

            var actionContext = new ActionContext(context, routeData, actionDescriptor);
            var controllerContext = new ControllerContext(actionContext);
            var controllerFactory = context.RequestServices.GetRequiredService<IControllerFactory>();
            object controller = controllerFactory.CreateController(controllerContext);

            if (controller is AiraCompanionAppController airaController)
            {
                airaController.ControllerContext = controllerContext;
                airaController.ControllerContext.HttpContext = context;
                var requestObject = new T();

                if (context.Request.ContentType == null)
                {
                    return;
                }

                if (context.Request.ContentType.Contains("application/x-www-form-urlencoded"))
                {
                    var form = await context.Request.ReadFormAsync();
                    foreach (string key in form.Keys)
                    {
                        var property = typeof(T).GetProperty(key, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                        property?.SetValue(requestObject, Convert.ChangeType(form[key], property.PropertyType));
                    }
                }
                else if (context.Request.ContentLength > 0 && (context.Request.ContentType == "application/json"))
                {
                    using var reader = new StreamReader(context.Request.Body);
                    string body = await reader.ReadToEndAsync();
                    requestObject = JsonSerializer.Deserialize<T>(body, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    })!;
                }
                else
                {
                    return;
                }

                var result = await action.Invoke(airaController, requestObject);

                await result.ExecuteResultAsync(controllerContext);
            }
        });
    private static Endpoint CreateEndpoint(string pattern, RequestDelegate requestDelegate) =>
        new RouteEndpointBuilder(
            requestDelegate: requestDelegate,
            routePattern: RoutePatternFactory.Parse(pattern),
            order: 0)
        .Build();
}

internal abstract class MutableEndpointDataSource : EndpointDataSource
{
    private readonly object endpointLock = new();
    private IReadOnlyList<Endpoint> endpoints = [];
    private CancellationTokenSource cancellationTokenSource;
    private IChangeToken changeToken;
    protected MutableEndpointDataSource(CancellationTokenSource cancellationTokenSource, IChangeToken changeToken)
    {
        SetEndpoints(endpoints);
        this.cancellationTokenSource = cancellationTokenSource;
        this.changeToken = changeToken;
    }
    public override IChangeToken GetChangeToken() => changeToken;
    public override IReadOnlyList<Endpoint> Endpoints => endpoints;
    protected void SetEndpoints(IReadOnlyList<Endpoint> endpoints)
    {
        lock (endpointLock)
        {
            var oldCancellationTokenSource = cancellationTokenSource;
            this.endpoints = endpoints;
            cancellationTokenSource = new CancellationTokenSource();
            changeToken = new CancellationChangeToken(cancellationTokenSource.Token);
            oldCancellationTokenSource?.Cancel();
        }
    }
}
