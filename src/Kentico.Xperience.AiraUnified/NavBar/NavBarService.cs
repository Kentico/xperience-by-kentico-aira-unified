using CMS.Core;
using CMS.DataEngine;
using CMS.MediaLibrary;

using Kentico.Content.Web.Mvc;
using Kentico.Xperience.AiraUnified.Admin;

namespace Kentico.Xperience.AiraUnified.NavBar;

internal class NavBarService : INavBarService
{
    private readonly IMediaFileUrlRetriever mediaFileUrlRetriever;
    private readonly IInfoProvider<MediaFileInfo> mediaFileInfoProvider;
    private readonly IAiraUnifiedConfigurationService airaUnifiedConfigurationService;
    private readonly IEventLogService eventLogService;

    public NavBarService(
        IEventLogService eventLogService,
        IMediaFileUrlRetriever mediaFileUrlRetriever,
        IInfoProvider<MediaFileInfo> mediaFileInfoProvider,
        IAiraUnifiedConfigurationService airaUnifiedConfigurationService)
    {
        this.mediaFileUrlRetriever = mediaFileUrlRetriever;
        this.mediaFileInfoProvider = mediaFileInfoProvider;
        this.airaUnifiedConfigurationService = airaUnifiedConfigurationService;
        this.eventLogService = eventLogService;
    }

    public async Task<NavBarViewModel> GetNavBarViewModel(string activePage)
    {
        var defaultImageUrl = "";
        var airaUnifiedConfiguration = await airaUnifiedConfigurationService.GetAiraUnifiedConfiguration();
        var logoUrl = GetMediaFileUrl(airaUnifiedConfiguration.AiraUnifiedConfigurationItemAiraRelativeLogoId)?.RelativePath;
        var sanitizedLogoUrl = GetSanitizedImageUrl(logoUrl, defaultImageUrl, "AIRA unified Logo");

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
                Url = AiraUnifiedConstants.ChatRelativeUrl
            },
            SmartUploadItem = new MenuItemModel
            {
                Title = airaUnifiedConfiguration.AiraUnifiedConfigurationItemAiraSmartUploadTitle,
                MenuImage = $"/{AiraUnifiedConstants.RCLUrlPrefix}/{AiraUnifiedConstants.PicturePlaceholderImgPath}",
                Url = AiraUnifiedConstants.SmartUploadRelativeUrl
            }
        };
    }

    public IMediaFileUrl? GetMediaFileUrl(string identifier)
    {
        if (Guid.TryParse(identifier, out var identifierGuid))
        {
            var mediaLibraryFiles = mediaFileInfoProvider
                .Get()
                .WhereEquals(nameof(MediaFileInfo.FileGUID), identifierGuid);
            if (mediaLibraryFiles.Any())
            {
                var media = mediaFileUrlRetriever.Retrieve(mediaLibraryFiles.First());
                return media;
            }
        }

        return default;
    }

    public string GetSanitizedImageUrl(string? configuredUrl, string defaultUrl, string imagePurpose)
    {
        if (!string.IsNullOrEmpty(configuredUrl))
        {
            return configuredUrl;
        }

        eventLogService.LogWarning(nameof(INavBarService), imagePurpose, "Configured URL is empty, using default");
        return defaultUrl;
    }
}
