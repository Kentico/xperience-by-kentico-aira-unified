using CMS.DataEngine;
using CMS.MediaLibrary;

using Kentico.Content.Web.Mvc;
using Kentico.Xperience.Aira.Admin.InfoModels;
using Kentico.Xperience.Aira.NavBar;

namespace Kentico.Xperience.Aira.Services
{
    public class AiraUIService
    {
        private readonly IInfoProvider<AiraConfigurationItemInfo>? airaConfigurationProvider;
        private readonly AiraConfigurationItemInfo airaConfiguration;
        private readonly IMediaFileUrlRetriever mediaFileUrlRetriever;
        private readonly IInfoProvider<MediaFileInfo> mediaFileInfoProvider;

        public AiraUIService(
            IInfoProvider<AiraConfigurationItemInfo> airaConfigurationProvider,
            IMediaFileUrlRetriever mediaFileUrlRetriever,
            IInfoProvider<MediaFileInfo> mediaFileInfoProvider)
        {
            var airaConfigurationList = airaConfigurationProvider.Get().GetEnumerableTypedResult();

            airaConfiguration = airaConfigurationList != null && airaConfigurationList.Any()
                ? airaConfigurationList.First()
                : throw new Exception("AIRA Configuration didn't load");

            this.mediaFileUrlRetriever = mediaFileUrlRetriever;
            this.mediaFileInfoProvider = mediaFileInfoProvider;
        }

        public NavBarViewModel GetNavBarViewModel(string activePage)
        {
            string defaultImageUrl = "path-to-not-found/image.jpg";

            string logoUrl = GetMediaFileUrl(airaConfiguration.AiraConfigurationItemAiraRelativeLogoId)?.RelativePath ?? defaultImageUrl;
            string chatImageUrl = GetMediaFileUrl(airaConfiguration.AiraConfigurationItemAiraRelativeChatImgId)?.RelativePath ?? defaultImageUrl;
            string smartUploadImageUrl = GetMediaFileUrl(airaConfiguration.AiraConfigurationItemAiraSmartUploadImgId)?.RelativePath ?? defaultImageUrl;

            return new NavBarViewModel
            {
                LogoImgRelativePath = logoUrl,
                TitleImagePath = activePage == "chat" ? chatImageUrl : smartUploadImageUrl,
                TitleText = activePage == "chat" ? airaConfiguration.AiraConfigurationItemAiraChatTitle : airaConfiguration.AiraConfigurationItemAiraSmartUploadTitle,
                ChatItem = new MenuItemModel
                {
                    Title = airaConfiguration.AiraConfigurationItemAiraChatTitle,
                    ImagePath = chatImageUrl,
                    Url = airaConfiguration.AiraConfigurationItemAiraRelativeChatUrl
                },
                SmartUploadItem = new MenuItemModel
                {
                    Title = airaConfiguration.AiraConfigurationItemAiraSmartUploadTitle,
                    ImagePath = smartUploadImageUrl,
                    Url = airaConfiguration.AiraConfigurationItemAiraRelativeSmartUploadUrl
                }
            };
        }

        public IMediaFileUrl? GetMediaFileUrl(string identifier)
        {

            if (Guid.TryParse(identifier, out var identifierGuid))
            {
                IEnumerable<MediaFileInfo> mediaLibraryFiles = mediaFileInfoProvider.Get()
                    .WhereEquals("FileGUID", identifierGuid);
                if (mediaLibraryFiles.Any())
                {
                    var media = mediaFileUrlRetriever.Retrieve(mediaLibraryFiles.First());
                    return media;
                }
            }

            return default;
        }
    }
}
