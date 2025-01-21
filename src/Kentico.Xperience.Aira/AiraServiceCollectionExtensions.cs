using Kentico.Xperience.Admin.Base.Forms;
using Kentico.Xperience.Aira.Admin;
using Kentico.Xperience.Aira.Assets;
using Kentico.Xperience.Aira.NavBar;

using Microsoft.AspNetCore.Routing;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Application startup extension methods.
/// </summary>
public static class AiraServiceCollectionExtensions
{
    /// <summary>
    /// Adds Aira services and custom module to application."/>
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/>The application services.</param>
    /// <returns><see cref="IServiceCollection"/>The application services.</returns>
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
            .AddScoped<IAiraConfigurationService, AiraConfigurationService>()
            .AddScoped<IAiraAssetService, AiraAssetService>()
            .AddScoped<INavBarService, NavBarService>();

        return services;
    }

    /// <summary>
    /// Allows using the Aira dynamic endpoint creation.
    /// </summary>
    /// <param name="endpoints"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public static void UseAiraEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var dataSource = endpoints.ServiceProvider.GetService<AiraEndpointDataSource>()
            ?? throw new InvalidOperationException("Did you forget to call Services.AddKenticoAira()?");
        endpoints.DataSources.Add(dataSource);
    }
}
