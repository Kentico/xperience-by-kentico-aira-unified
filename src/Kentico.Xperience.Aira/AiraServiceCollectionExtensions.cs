using CMS.EmailEngine;

using Kentico.Xperience.Admin.Base.Forms;
using Kentico.Xperience.Aira.Admin;
using Kentico.Xperience.Aira.Assets;
using Kentico.Xperience.Aira.Services;

using Microsoft.AspNetCore.Routing;

namespace Microsoft.Extensions.DependencyInjection;

public static class AiraServiceCollectionExtensions
{
    public static IServiceCollection AddKenticoAira(this IServiceCollection services)
        => services.AddKenticoAiraInternal();

    private static IServiceCollection AddKenticoAiraInternal(this IServiceCollection services)
    {
        services.AddControllersWithViews();

        services
            .AddSingleton<IAiraModuleInstaller, AiraModuleInstaller>()
            .AddSingleton<AiraEndpointDataSource>()
            .AddScoped<ContentItemAssetUploaderComponent>()
            .AddScoped<AiraConfigurationService>()
            .AddScoped<IAiraAiraAssetService, AiraAssetService>()
            .AddScoped<AiraUIService>()
            .AddScoped<SystemEmailOptions>();

        return services;
    }

    public static void UseAiraEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var dataSource = endpoints.ServiceProvider.GetService<AiraEndpointDataSource>()
            ?? throw new InvalidOperationException("Did you forget to call Services.AddKenticoAira()?");
        endpoints.DataSources.Add(dataSource);
    }
}
