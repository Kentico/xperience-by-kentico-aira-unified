﻿using System.Reflection;
using System.Text.Json;

using CMS.DataEngine;
using CMS.Membership;

using Kentico.Membership;
using Kentico.Xperience.AiraUnified.Admin.InfoModels;
using Kentico.Xperience.AiraUnified.Assets;
using Kentico.Xperience.AiraUnified.Chat.Models;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

namespace Kentico.Xperience.AiraUnified.Admin;

internal class AiraUnifiedEndpointDataSource : MutableEndpointDataSource
{
    private readonly IInfoProvider<AiraUnifiedConfigurationItemInfo> airaUnifiedConfigurationProvider;

    public AiraUnifiedEndpointDataSource(IInfoProvider<AiraUnifiedConfigurationItemInfo> airaUnifiedConfigurationProvider)
        : base(new CancellationTokenSource(), new CancellationChangeToken(new CancellationToken()))
        => this.airaUnifiedConfigurationProvider = airaUnifiedConfigurationProvider;

    public void UpdateEndpoints()
        => SetEndpoints(MakeEndpoints());

    private IReadOnlyList<Endpoint> MakeEndpoints()
    {
        var configuration = airaUnifiedConfigurationProvider.Get().GetEnumerableTypedResult().SingleOrDefault();

        if (configuration is null)
        {
            return [];
        }

        if (string.IsNullOrEmpty(configuration.AiraUnifiedConfigurationItemAiraPathBase))
        {
            return [];
        }

        return
        [
            CreateAiraEndpoint(configuration,
                AiraUnifiedConstants.ChatRelativeUrl,
                nameof(AiraUnifiedController.Index),
                controller => controller.Index(),
                requiredPermission: SystemPermissions.VIEW
            ),
            CreateAiraIFormCollectionEndpoint(configuration,
                $"{AiraUnifiedConstants.ChatRelativeUrl}/{AiraUnifiedConstants.ChatMessageUrl}",
                nameof(AiraUnifiedController.PostChatMessage),
                (controller, request) => controller.PostChatMessage(request),
                requiredPermission: SystemPermissions.VIEW
            ),
            CreateAiraIFormCollectionEndpoint(configuration,
                 $"{AiraUnifiedConstants.SmartUploadRelativeUrl}/{AiraUnifiedConstants.SmartUploadUploadUrl}",
                nameof(AiraUnifiedController.PostImages),
                (controller, request) => controller.PostImages(request),
                requiredPermission: SystemPermissions.CREATE
            ),
            CreateAiraEndpoint(configuration,
                AiraUnifiedConstants.SmartUploadRelativeUrl,
                nameof(AiraUnifiedController.Assets),
                controller => controller.Assets(),
                requiredPermission: SystemPermissions.CREATE
            ),
            CreateAiraEndpoint(configuration,
                AiraUnifiedConstants.SigninRelativeUrl,
                nameof(AiraUnifiedController.SignIn),
                (controller) => controller.SignIn()
            ),
            CreateAiraEndpoint(configuration,
                subPath: string.Empty,
                nameof(AiraUnifiedController.SignIn),
                controller => controller.SignIn()
            ),
            CreateAiraEndpointFromBody<AiraUnifiedUsedPromptGroupModel>(configuration,
                AiraUnifiedConstants.RemoveUsedPromptGroupRelativeUrl,
                nameof(AiraUnifiedController.RemoveUsedPromptGroup),
                (controller, model) => controller.RemoveUsedPromptGroup(model),
                requiredPermission: SystemPermissions.VIEW
            ),
            CreateAiraEndpoint(configuration,
                $"{AiraUnifiedConstants.SmartUploadRelativeUrl}/{AiraUnifiedConstants.SmartUploadAllowedFileExtensionsUrl}",
                nameof(AiraUnifiedController.GetAllowedFileExtensions),
                controller => controller.GetAllowedFileExtensions(),
                requiredPermission: SystemPermissions.CREATE
            )
        ];
    }
    private static Endpoint CreateAiraEndpointFromBody<T>(
        AiraUnifiedConfigurationItemInfo configurationInfo,
        string subPath,
        string actionName,
        Func<AiraUnifiedController, T, IActionResult> actionWithModel,
        string? requiredPermission = null
    ) where T : class, new()
    => CreateEndpoint($"{configurationInfo.AiraUnifiedConfigurationItemAiraPathBase}/{subPath}", async context =>
    {
        var airaUnifiedController = await GetAiraUnifiedControllerInContext(context, actionName);

        if (!await CheckHttps(context))
        {
            return;
        }

        if (requiredPermission is not null && !await CheckAuthorizationOrSetRedirectToSignIn(context, configurationInfo.AiraUnifiedConfigurationItemAiraPathBase, requiredPermission))
        {
            return;
        }

        if (context.Request.ContentType is not null &&
            string.Equals(context.Request.ContentType, "application/json", StringComparison.OrdinalIgnoreCase))
        {
            using var reader = new StreamReader(context.Request.Body);
            var body = await reader.ReadToEndAsync();

            try
            {
                var requestObject = JsonSerializer.Deserialize<T>(body, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (requestObject is not null)
                {
                    var result = actionWithModel.Invoke(airaUnifiedController, requestObject);
                    await result.ExecuteResultAsync(airaUnifiedController.ControllerContext);
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("Invalid or missing request body.");
                }
            }
            catch (JsonException ex)
            {
                // Handle JSON deserialization errors
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync($"Invalid JSON format: {ex.Message}");
            }
        }
        else
        {
            // Handle unsupported content types
            context.Response.StatusCode = StatusCodes.Status415UnsupportedMediaType;
            await context.Response.WriteAsync("Unsupported content type. Expected 'application/json'.");
        }
    });

    private static Endpoint CreateAiraEndpoint(AiraUnifiedConfigurationItemInfo configurationInfo,
        string subPath,
        string actionName,
        Func<AiraUnifiedController, Task<IActionResult>> action,
        string? requiredPermission = null)
    {
        var path = $"{configurationInfo.AiraUnifiedConfigurationItemAiraPathBase}";

        if (!string.Equals(subPath, string.Empty))
        {
            path = $"{configurationInfo.AiraUnifiedConfigurationItemAiraPathBase}/{subPath}";
        }

        return CreateEndpoint(path, async context =>
        {
            var airaUnifiedController = await GetAiraUnifiedControllerInContext(context, actionName);

            if (!await CheckHttps(context))
            {
                return;
            }

            if (requiredPermission is not null && !await CheckAuthorizationOrSetRedirectToSignIn(context, configurationInfo.AiraUnifiedConfigurationItemAiraPathBase, requiredPermission))
            {
                return;
            }

            var result = await action.Invoke(airaUnifiedController);
            await result.ExecuteResultAsync(airaUnifiedController.ControllerContext);
        });
    }

    private static Endpoint CreateAiraIFormCollectionEndpoint(AiraUnifiedConfigurationItemInfo configurationItemInfo,
        string subPath,
        string actionName,
        Func<AiraUnifiedController, IFormCollection, Task<IActionResult>> action,
        string? requiredPermission = null)
    => CreateEndpoint($"{configurationItemInfo.AiraUnifiedConfigurationItemAiraPathBase}/{subPath}", async context =>
    {
        if (!await CheckHttps(context))
        {
            return;
        }

        var airaUnifiedController = await GetAiraUnifiedControllerInContext(context, actionName);

        if (requiredPermission is not null && !await CheckAuthorizationOrSetRedirectToSignIn(context, configurationItemInfo.AiraUnifiedConfigurationItemAiraPathBase, requiredPermission))
        {
            return;
        }

        if (context.Request.ContentType is null)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("Missing Content-Type header.");
            return;
        }

        if (context.Request.ContentType.Contains("multipart/form-data"))
        {
            var requestObject = await context.Request.ReadFormAsync();
            var result = await action.Invoke(airaUnifiedController, requestObject);
            await result.ExecuteResultAsync(airaUnifiedController.ControllerContext);
        }
        else if (string.Equals(context.Request.ContentType, "application/json"))
        {
            using var reader = new StreamReader(context.Request.Body);
            var body = await reader.ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(body))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Empty request body.");
                return;
            }

            var formCollection = new FormCollection(new Dictionary<string, StringValues>
            {
                { "message", body }
            });

            var result = await action.Invoke(airaUnifiedController, formCollection);
            await result.ExecuteResultAsync(airaUnifiedController.ControllerContext);
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status415UnsupportedMediaType;
            await context.Response.WriteAsync("Unsupported content type. Expected 'application/json' or 'multipart/form-data'.");
        }
    });

    private static async Task<AiraUnifiedController> GetAiraUnifiedControllerInContext(HttpContext context, string actionName)
    {
        var controllerShortName = nameof(AiraUnifiedController).Replace("Controller", string.Empty);

        var routeData = new RouteData();
        routeData.Values["controller"] = controllerShortName;
        routeData.Values["action"] = actionName;

        var actionDescriptor = new ControllerActionDescriptor
        {
            ControllerName = controllerShortName,
            ActionName = actionName,
            ControllerTypeInfo = typeof(AiraUnifiedController).GetTypeInfo()
        };

        await AuthenticateAiraEndpoint(context);

        var actionContext = new ActionContext(context, routeData, actionDescriptor);
        var controllerContext = new ControllerContext(actionContext);
        var controllerFactory = context.RequestServices.GetRequiredService<IControllerFactory>();
        var airaUnifiedController = (AiraUnifiedController)controllerFactory.CreateController(controllerContext);

        airaUnifiedController.ControllerContext = controllerContext;
        airaUnifiedController.ControllerContext.HttpContext = context;

        return airaUnifiedController;
    }

    private static async Task AuthenticateAiraEndpoint(HttpContext context)
    {
        if (context.User?.Identity is null || !context.User.Identity.IsAuthenticated)
        {
            var authenticateResult = await context.RequestServices
                .GetRequiredService<IAuthenticationService>()
                .AuthenticateAsync(context, AiraUnifiedConstants.XperienceAdminSchemeName);

            if (authenticateResult.Succeeded && authenticateResult.Principal is not null)
            {
                context.User = authenticateResult.Principal;
            }
        }
    }

    private static async Task<bool> CheckHttps(HttpContext context)
    {
        if (!context.Request.IsHttps)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("HTTPS is required.");
            return false;
        }

        context.Response.Headers.XFrameOptions = "SAMEORIGIN";
        context.Response.Headers.ContentSecurityPolicy = "frame-ancestors 'self'";
        context.Response.Headers.StrictTransportSecurity = "max-age=31536000; includeSubDomains; preload";

        return true;
    }

    private static Endpoint CreateEndpoint(string pattern, RequestDelegate requestDelegate) =>
        new RouteEndpointBuilder(
            requestDelegate: requestDelegate,
            routePattern: RoutePatternFactory.Parse(pattern),
            order: 0)
        .Build();

    private static async Task<bool> CheckAuthorizationOrSetRedirectToSignIn(HttpContext context, string airaUnifiedPathBase, string permission)
    {
        var adminUserManager = context.RequestServices.GetRequiredService<AdminUserManager>();
        var airaUnifiedAssetService = context.RequestServices.GetRequiredService<IAiraUnifiedAssetService>();
        var userProvider = context.RequestServices.GetRequiredService<IInfoProvider<UserInfo>>();

        var user = await adminUserManager.GetUserAsync(context.User);
        var signInRedirectUrl = $"{airaUnifiedPathBase}/{AiraUnifiedConstants.SigninRelativeUrl}";

        if (user is null || !userProvider.Get().WhereEquals(nameof(UserInfo.UserGUID), user.UserGUID).Any())
        {
            context.Response.Redirect($"{signInRedirectUrl}?{AiraUnifiedConstants.SigninMissingPermissionParameterName}={permission}");
            return false;
        }

        var hasAiraViewPermission = await airaUnifiedAssetService.DoesUserHaveAiraUnifiedPermission(permission, user.UserID);
        if (!hasAiraViewPermission)
        {
            context.Response.Redirect($"{signInRedirectUrl}?{AiraUnifiedConstants.SigninMissingPermissionParameterName}={permission}");
            return false;
        }

        return true;
    }
}
