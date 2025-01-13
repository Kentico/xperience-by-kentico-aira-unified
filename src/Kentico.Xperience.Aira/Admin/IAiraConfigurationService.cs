using Kentico.Xperience.Aira.Admin.InfoModels;

namespace Kentico.Xperience.Aira.Admin;

public interface IAiraConfigurationService
{
    Task<AiraConfigurationItemInfo> GetAiraConfiguration();
    Task<bool> TrySaveOrUpdateConfiguration(AiraConfigurationModel configurationModel);
}
