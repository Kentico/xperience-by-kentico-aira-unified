using Kentico.Xperience.AiraUnified.Admin;
using Kentico.Xperience.AiraUnified.Assets;

namespace Kentico.Xperience.AiraUnified.NavBar;

internal class NavigationService : INavigationService
{
    private readonly IAiraUnifiedConfigurationService airaUnifiedConfigurationService;
    private readonly IAiraUnifiedAssetService airaUnifiedAssetService;

    public NavigationService(IAiraUnifiedConfigurationService airaUnifiedConfigurationService,
        IAiraUnifiedAssetService airaUnifiedAssetService)
    {
        this.airaUnifiedConfigurationService = airaUnifiedConfigurationService;
        this.airaUnifiedAssetService = airaUnifiedAssetService;
    }

    public async Task<NavBarViewModel> GetNavBarViewModel(string activePage, string baseUrl)
    {
        var airaUnifiedConfiguration = await airaUnifiedConfigurationService.GetAiraUnifiedConfiguration();
        var sanitizedLogoUrl = airaUnifiedAssetService.GetSanitizedLogoUrl(airaUnifiedConfiguration);

        var chatItemUrl = BuildUriOrNull(baseUrl, airaUnifiedConfiguration.AiraUnifiedConfigurationItemAiraPathBase, AiraUnifiedConstants.ChatRelativeUrl);

        var smartUploadUrl = BuildUriOrNull(baseUrl, airaUnifiedConfiguration.AiraUnifiedConfigurationItemAiraPathBase, AiraUnifiedConstants.SmartUploadRelativeUrl);

        return new NavBarViewModel
        {
            LogoImgRelativePath = sanitizedLogoUrl,
            TitleImagePath = activePage == AiraUnifiedConstants.ChatRelativeUrl ?
             $"/{AiraUnifiedConstants.RCLUrlPrefix}/{AiraUnifiedConstants.PictureNetworkGraphImgPath}"
             : $"/{AiraUnifiedConstants.RCLUrlPrefix}/{AiraUnifiedConstants.PicturePlaceholderImgPath}",

            TitleText = activePage == AiraUnifiedConstants.ChatRelativeUrl ?
                airaUnifiedConfiguration.AiraUnifiedConfigurationItemAiraUnifiedChatTitle
                : airaUnifiedConfiguration.AiraUnifiedConfigurationItemAiraSmartUploadTitle,

            MenuMessage = Resource.NavigationMenuMessage,

            ChatItem = new MenuItemModel
            {
                Title = airaUnifiedConfiguration.AiraUnifiedConfigurationItemAiraUnifiedChatTitle,
                MenuImage = $"/{AiraUnifiedConstants.RCLUrlPrefix}/{AiraUnifiedConstants.PictureNetworkGraphImgPath}",
                Url = chatItemUrl?.ToString()
            },
            SmartUploadItem = new MenuItemModel
            {
                Title = airaUnifiedConfiguration.AiraUnifiedConfigurationItemAiraSmartUploadTitle,
                MenuImage = $"/{AiraUnifiedConstants.RCLUrlPrefix}/{AiraUnifiedConstants.PicturePlaceholderImgPath}",
                Url = smartUploadUrl?.ToString()
            }
        };
    }

    public Uri? BuildUriOrNull(string baseUrl, string airaPathBase, params string[] relativePathParts)
        => Uri.TryCreate($"{baseUrl}{airaPathBase}/{string.Join('/', relativePathParts)}", UriKind.Absolute, out var uri) ? uri : null;
}
