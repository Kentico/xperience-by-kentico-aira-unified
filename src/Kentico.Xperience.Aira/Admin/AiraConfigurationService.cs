using CMS.DataEngine;
using CMS.DataEngine.Query;

using Kentico.Xperience.Aira.Admin.InfoModels;

namespace Kentico.Xperience.Aira.Admin;
internal class AiraConfigurationService
{
    private readonly IInfoProvider<AiraConfigurationItemInfo> airaConfigurationProvider;
    private readonly AiraEndpointDataSource airaEndpointDataSource;

    public AiraConfigurationService(IInfoProvider<AiraConfigurationItemInfo> airaConfigurationProvider, AiraEndpointDataSource airaEndpointDataSource)
    {
        this.airaConfigurationProvider = airaConfigurationProvider;
        this.airaEndpointDataSource = airaEndpointDataSource;
    }

    public async Task<bool> TrySaveOrUpdateConfiguration(AiraConfigurationModel configurationModel)
    {
        var existingConfiguration = (await airaConfigurationProvider.Get().GetEnumerableTypedResultAsync()).SingleOrDefault();

        if (existingConfiguration is null)
        {
            if (!string.IsNullOrWhiteSpace(configurationModel.RelativePathBase))
            {
                int configurationCount = await airaConfigurationProvider.Get().GetCountAsync();

                if (configurationCount > 0)
                {
                    return false;
                }

                var newConfigurationInfo = configurationModel.MapToAiraConfigurationInfo();
                airaConfigurationProvider.Set(newConfigurationInfo);

                return true;
            }
            return false;
        }

        existingConfiguration = configurationModel.MapToAiraConfigurationInfo(existingConfiguration);

        airaConfigurationProvider.Set(existingConfiguration);

        airaEndpointDataSource.UpdateEndpoints();

        return true;
    }
}
