using CMS.MediaLibrary;

using Kentico.Xperience.Admin.Base.FormAnnotations;
using Kentico.Xperience.Aira.Admin.InfoModels;

namespace Kentico.Xperience.Aira.Admin;

internal class AiraConfigurationModel
{
    [RequiredValidationRule]
    [TextInputComponent(Label = "Relative Path Base", Order = 0, ExplanationText = "Relative path where the ACA is available.")]
    public string RelativePathBase { get; set; } = string.Empty;

    [RequiredValidationRule]
    [AssetSelectorComponent(Label = "Logo", Order = 1, ExplanationText = "Logo Image from a library.", AllowedExtensions = "jpg;jpeg;png", MaximumAssets = 1)]
    public IEnumerable<AssetRelatedItem>? Logo { get; set; }

    [RequiredValidationRule]
    [TextInputComponent(Label = "Chat Title", Order = 2, ExplanationText = "Title of the chat.")]
    public string ChatTitle { get; set; } = string.Empty;

    [RequiredValidationRule]
    [AssetSelectorComponent(Label = "Relative Chat Image Url", Order = 3, ExplanationText = "Chat image from a library.", AllowedExtensions = "jpg;jpeg;png", MaximumAssets = 1)]
    public IEnumerable<AssetRelatedItem>? ChatImage { get; set; }

    [RequiredValidationRule]
    [TextInputComponent(Label = "Relative Chat Url", Order = 4, ExplanationText = "Relative path to the chat.")]
    public string RelativeChatUrl { get; set; } = string.Empty;

    [RequiredValidationRule]
    [TextInputComponent(Label = "Smart Upload Title", Order = 5, ExplanationText = "Title of the smart upload.")]
    public string SmartUploadTitle { get; set; } = string.Empty;

    [RequiredValidationRule]
    [AssetSelectorComponent(Label = "Smart Upload Image", Order = 6, ExplanationText = "Smart Upload Image from a library.", AllowedExtensions = "jpg;jpeg;png", MaximumAssets = 1)]
    public IEnumerable<AssetRelatedItem>? SmartUploadImage { get; set; }

    [RequiredValidationRule]
    [TextInputComponent(Label = "Relative Smart Upload Url", Order = 7, ExplanationText = "Relative path to the smart upload.")]
    public string RelativeSmartUploadUrl { get; set; } = string.Empty;

    public AiraConfigurationModel() { }

    public AiraConfigurationModel(
        AiraConfigurationItemInfo airaConfigurationInfo) => MapFrommAiraConfigurationInfo(airaConfigurationInfo);

    public void MapFrommAiraConfigurationInfo(AiraConfigurationItemInfo info)
    {
        RelativePathBase = info.AiraConfigurationItemAiraPathBase;

        if (Guid.TryParse(info.AiraConfigurationItemAiraRelativeLogoId, out var relativeLogoUrlGuid))
        {
            var relativeLogoUrlAsset = new AssetRelatedItem
            {
                Identifier = relativeLogoUrlGuid
            };
            Logo = [relativeLogoUrlAsset];
        }

        ChatTitle = info.AiraConfigurationItemAiraChatTitle;

        if (Guid.TryParse(info.AiraConfigurationItemAiraRelativeChatImgId, out var relativeChatImageUrlGuid))
        {
            var relativeChatImageUrlAsset = new AssetRelatedItem
            {
                Identifier = relativeChatImageUrlGuid
            };
            ChatImage = [relativeChatImageUrlAsset];
        }

        RelativeChatUrl = info.AiraConfigurationItemAiraRelativeChatUrl;

        SmartUploadTitle = info.AiraConfigurationItemAiraSmartUploadTitle;

        if (Guid.TryParse(info.AiraConfigurationItemAiraSmartUploadImgId, out var relativeSmartUploadImgUrlGuid))
        {
            var relativeSmartUploadImgUrl = new AssetRelatedItem
            {
                Identifier = relativeSmartUploadImgUrlGuid
            };
            SmartUploadImage = [relativeSmartUploadImgUrl];
        }

        RelativeSmartUploadUrl = info.AiraConfigurationItemAiraRelativeSmartUploadUrl;
    }

    public AiraConfigurationItemInfo MapToAiraConfigurationInfo(AiraConfigurationItemInfo? info = null)
    {
        info ??= new AiraConfigurationItemInfo();
        info.AiraConfigurationItemAiraPathBase = RelativePathBase;
        info.AiraConfigurationItemAiraRelativeLogoId = GetImageIdentifier(Logo);

        info.AiraConfigurationItemAiraChatTitle = ChatTitle;
        info.AiraConfigurationItemAiraRelativeChatImgId = GetImageIdentifier(ChatImage);
        info.AiraConfigurationItemAiraRelativeChatUrl = RelativeChatUrl;

        info.AiraConfigurationItemAiraSmartUploadTitle = SmartUploadTitle;
        info.AiraConfigurationItemAiraSmartUploadImgId = GetImageIdentifier(SmartUploadImage);
        info.AiraConfigurationItemAiraRelativeSmartUploadUrl = RelativeSmartUploadUrl;

        return info;
    }

    private string GetImageIdentifier(IEnumerable<AssetRelatedItem>? asset)
    {
        return asset?.FirstOrDefault()?.Identifier.ToString() ?? "";
    }

}
