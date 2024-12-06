﻿using CMS.DataEngine;

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
                nameof(AiraCompanionAppController.Index).ToLowerInvariant(),
                controllerShortName,
                nameof(AiraCompanionAppController.Index),
                controller => controller.Index()
            ),
            CreateAiraEndpoint<List<AiraChatMessageModel>>(configuration,
                "chat/message",
                controllerShortName,
                nameof(AiraCompanionAppController.PostChatMessage),
                (controller, request) => controller.PostChatMessage(request)
            )
        ];
    }
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
                if (context.Request.ContentLength > 0 && context.Request.Body.CanRead)
                {
                    using var reader = new StreamReader(context.Request.Body);
                    string body = await reader.ReadToEndAsync();
                    requestObject = JsonSerializer.Deserialize<T>(body, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    })!;
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
