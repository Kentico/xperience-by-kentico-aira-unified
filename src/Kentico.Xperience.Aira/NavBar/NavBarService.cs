using CMS.Core;
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
    private readonly IEventLogService eventLogService;

    public NavBarService(
        IEventLogService eventLogService,
        IMediaFileUrlRetriever mediaFileUrlRetriever,
        IInfoProvider<MediaFileInfo> mediaFileInfoProvider,
        IAiraConfigurationService airaConfigurationService)
    {
        this.mediaFileUrlRetriever = mediaFileUrlRetriever;
        this.mediaFileInfoProvider = mediaFileInfoProvider;
        this.airaConfigurationService = airaConfigurationService;
        this.eventLogService = eventLogService;
    }

    public async Task<NavBarViewModel> GetNavBarViewModel(string activePage)
    {
        var defaultImageUrl = "path-to-not-found/image.jpg";
        var airaConfiguration = await airaConfigurationService.GetAiraConfiguration();
        var logoUrl = GetMediaFileUrl(airaConfiguration.AiraConfigurationItemAiraRelativeLogoId)?.RelativePath;
        logoUrl = GetSanitizedImageUrl(logoUrl, defaultImageUrl, "AIRA Logo");

        return new NavBarViewModel
        {
            LogoImgRelativePath = logoUrl,
            TitleImagePath = activePage == AiraCompanionAppConstants.ChatRelativeUrl ?
             $"/{AiraCompanionAppConstants.RCLUrlPrefix}/{AiraCompanionAppConstants.PictureNetworkGraphImgPath}"
             : $"/{AiraCompanionAppConstants.RCLUrlPrefix}/{AiraCompanionAppConstants.PicturePlaceholderImgPath}",

            TitleText = activePage == AiraCompanionAppConstants.ChatRelativeUrl ? airaConfiguration.AiraConfigurationItemAiraChatTitle : airaConfiguration.AiraConfigurationItemAiraSmartUploadTitle,

            ChatItem = new MenuItemModel
            {
                Title = airaConfiguration.AiraConfigurationItemAiraChatTitle,
                MenuImage = $"/{AiraCompanionAppConstants.RCLUrlPrefix}/{AiraCompanionAppConstants.PictureNetworkGraphImgPath}",
                Url = AiraCompanionAppConstants.ChatRelativeUrl
            },
            SmartUploadItem = new MenuItemModel
            {
                Title = airaConfiguration.AiraConfigurationItemAiraSmartUploadTitle,
                MenuImage = $"/{AiraCompanionAppConstants.RCLUrlPrefix}/{AiraCompanionAppConstants.PicturePlaceholderImgPath}",
                Url = AiraCompanionAppConstants.SmartUploadRelativeUrl
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
        if (string.IsNullOrEmpty(configuredUrl))
        {
            eventLogService.LogWarning(nameof(INavBarService), imagePurpose, "Configured URL is empty, using default");
            return defaultUrl;
        }

        return configuredUrl;
    }
}
