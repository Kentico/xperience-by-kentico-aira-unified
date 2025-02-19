using Kentico.Xperience.AiraUnified.Admin.InfoModels;

namespace Kentico.Xperience.AiraUnified.Admin;

public interface IAiraUnifiedConfigurationService
{
    Task<AiraUnifiedConfigurationItemInfo> GetAiraUnifiedConfiguration();
    Task<bool> TrySaveOrUpdateConfiguration(AiraUnifiedConfigurationModel configurationModel);
}
