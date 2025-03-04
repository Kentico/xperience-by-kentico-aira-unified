using Kentico.Xperience.Admin.Base.Forms;
using Kentico.Xperience.AiraUnified.Admin;
using Kentico.Xperience.AiraUnified.Assets;
using Kentico.Xperience.AiraUnified.Chat;
using Kentico.Xperience.AiraUnified.Chat.Models;
using Kentico.Xperience.AiraUnified.Insights;
using Kentico.Xperience.AiraUnified.NavBar;

using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Application startup extension methods.
/// </summary>
public static class AiraUnifiedServiceCollectionExtensions
{
    /// <summary>
    /// Adds Aira Unified services and custom module to application.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/>The application services.</param>
    /// <param name="configuration">The application <see cref="IConfiguration"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> application services.</returns>
    public static IServiceCollection AddKenticoAiraUnified(this IServiceCollection services, IConfiguration configuration)
        => services.AddKenticoAiraUnifiedInternal(configuration);

    private static IServiceCollection AddKenticoAiraUnifiedInternal(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllersWithViews();

        services
            .AddSingleton<IAiraUnifiedModuleInstaller, AiraUnifiedModuleInstaller>()
            .AddSingleton<AiraUnifiedEndpointDataSource>()
            .AddScoped<ContentItemAssetUploaderComponent>()
            .AddScoped<AiraUnifiedConfigurationService>()
            .AddScoped<IAiraUnifiedConfigurationService, AiraUnifiedConfigurationService>()
            .AddScoped<IAiraUnifiedInsightsService, AiraUnifiedInsightsService>()
            .AddScoped<IAiraUnifiedAssetService, AiraUnifiedAssetService>()
            .AddScoped<IAiraUnifiedChatService, AiraUnifiedChatService>()
            .AddScoped<INavigationService, NavigationService>()
            .Configure<AiraUnifiedOptions>(configuration.GetSection(nameof(AiraUnifiedOptions)));

        return services;
    }

    /// <summary>
    /// Allows using the Aira Unified dynamic endpoint creation.
    /// </summary>
    /// <param name="endpoints"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public static void UseAiraUnifiedEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var dataSource = endpoints.ServiceProvider.GetService<AiraUnifiedEndpointDataSource>()
            ?? throw new InvalidOperationException("Did you forget to call Services.AddKenticoAiraUnified()?");
        endpoints.DataSources.Add(dataSource);
    }
}
