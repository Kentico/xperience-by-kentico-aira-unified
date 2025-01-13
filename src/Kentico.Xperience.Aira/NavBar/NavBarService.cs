using CMS.DataEngine;
using CMS.MediaLibrary;

using Kentico.Content.Web.Mvc;
using Kentico.Xperience.Aira.Admin;

namespace Kentico.Xperience.Aira.NavBar;

internal class NavBarService : INavBarService
{
    private readonly IMediaFileUrlRetriever mediaFileUrlRetriever;
    private readonly IInfoProvider<MediaFileInfo> mediaFileInfoProvider;
    private readonly IAiraConfigurationService airaConfigurationService;

    public NavBarService(
        IMediaFileUrlRetriever mediaFileUrlRetriever,
        IInfoProvider<MediaFileInfo> mediaFileInfoProvider,
        IAiraConfigurationService airaConfigurationService)
    {
        this.mediaFileUrlRetriever = mediaFileUrlRetriever;
        this.mediaFileInfoProvider = mediaFileInfoProvider;
        this.airaConfigurationService = airaConfigurationService;
    }

    public async Task<NavBarViewModel> GetNavBarViewModel(string activePage)
    {
        string defaultImageUrl = "path-to-not-found/image.jpg";

        var airaConfiguration = await airaConfigurationService.GetAiraConfiguration();

        string logoUrl = GetMediaFileUrl(airaConfiguration.AiraConfigurationItemAiraRelativeLogoId)?.RelativePath ?? defaultImageUrl;
        string chatImageUrl = GetMediaFileUrl(airaConfiguration.AiraConfigurationItemAiraRelativeChatImgId)?.RelativePath ?? defaultImageUrl;
        string smartUploadImageUrl = GetMediaFileUrl(airaConfiguration.AiraConfigurationItemAiraSmartUploadImgId)?.RelativePath ?? defaultImageUrl;

        return new NavBarViewModel
        {
            LogoImgRelativePath = logoUrl,
            TitleImagePath = activePage == AiraCompanionAppConstants.ChatRelativeUrl ? chatImageUrl : smartUploadImageUrl,
            TitleText = activePage == AiraCompanionAppConstants.ChatRelativeUrl ? airaConfiguration.AiraConfigurationItemAiraChatTitle : airaConfiguration.AiraConfigurationItemAiraSmartUploadTitle,
            ChatItem = new MenuItemModel
            {
                Title = airaConfiguration.AiraConfigurationItemAiraChatTitle,
                ImagePath = chatImageUrl,
                MenuImage = $"/{AiraCompanionAppConstants.RCLUrlPrefix}/{AiraCompanionAppConstants.PictureNetworkGraphImgPath}",
                Url = AiraCompanionAppConstants.ChatRelativeUrl
            },
            SmartUploadItem = new MenuItemModel
            {
                Title = airaConfiguration.AiraConfigurationItemAiraSmartUploadTitle,
                MenuImage = $"/{AiraCompanionAppConstants.RCLUrlPrefix}/{AiraCompanionAppConstants.PicturePlaceholderImgPath}",
                ImagePath = smartUploadImageUrl,
                Url = AiraCompanionAppConstants.SmartUploadRelativeUrl
            }
        };
    }

    public IMediaFileUrl? GetMediaFileUrl(string identifier)
    {

        if (Guid.TryParse(identifier, out var identifierGuid))
        {
            IEnumerable<MediaFileInfo> mediaLibraryFiles = mediaFileInfoProvider.Get()
                .WhereEquals(nameof(MediaFileInfo.FileGUID), identifierGuid);
            if (mediaLibraryFiles.Any())
            {
                var media = mediaFileUrlRetriever.Retrieve(mediaLibraryFiles.First());
                return media;
            }
        }

        return default;
    }
}
