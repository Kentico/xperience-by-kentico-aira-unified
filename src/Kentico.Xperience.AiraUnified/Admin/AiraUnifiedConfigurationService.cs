using CMS.DataEngine;
using CMS.DataEngine.Query;

using Kentico.Xperience.AiraUnified.Admin.InfoModels;

namespace Kentico.Xperience.AiraUnified.Admin;
internal class AiraUnifiedConfigurationService : IAiraUnifiedConfigurationService
{
    private readonly IInfoProvider<AiraUnifiedConfigurationItemInfo> airaUnifiedConfigurationProvider;
    private readonly AiraUnifiedEndpointDataSource airaUnifiedEndpointDataSource;

    public AiraUnifiedConfigurationService(IInfoProvider<AiraUnifiedConfigurationItemInfo> airaUnifiedConfigurationProvider, AiraUnifiedEndpointDataSource airaUnifiedEndpointDataSource)
    {
        this.airaUnifiedConfigurationProvider = airaUnifiedConfigurationProvider;
        this.airaUnifiedEndpointDataSource = airaUnifiedEndpointDataSource;
    }

    public async Task<bool> TrySaveOrUpdateConfiguration(AiraUnifiedConfigurationModel configurationModel)
    {
        var existingConfiguration = (await airaUnifiedConfigurationProvider.Get().GetEnumerableTypedResultAsync()).SingleOrDefault();

        if (existingConfiguration is null)
        {
            if (string.IsNullOrWhiteSpace(configurationModel.RelativePathBase))
            {
                return false;
            }
            var configurationCount = await airaUnifiedConfigurationProvider.Get().GetCountAsync();

            if (configurationCount > 0)
            {
                return false;
            }

            var newConfigurationInfo = configurationModel.MapToAiraUnifiedConfigurationInfo();
            airaUnifiedConfigurationProvider.Set(newConfigurationInfo);

            return true;
        }

        existingConfiguration = configurationModel.MapToAiraUnifiedConfigurationInfo(existingConfiguration);

        airaUnifiedConfigurationProvider.Set(existingConfiguration);

        airaUnifiedEndpointDataSource.UpdateEndpoints();

        return true;
    }

    public async Task<AiraUnifiedConfigurationItemInfo> GetAiraUnifiedConfiguration()
    {
        var airaUnifiedConfigurationItemList = await airaUnifiedConfigurationProvider.Get().GetEnumerableTypedResultAsync();

        return airaUnifiedConfigurationItemList.SingleOrDefault()
            ?? throw new InvalidOperationException("Aira Unified has not been configured yet.");
    }
}
