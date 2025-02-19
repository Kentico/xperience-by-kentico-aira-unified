using CMS.MediaLibrary;

using Kentico.Xperience.Admin.Base.FormAnnotations;
using Kentico.Xperience.Aira.Admin.InfoModels;

namespace Kentico.Xperience.Aira.Admin;

public class AiraConfigurationModel
{
    [RequiredValidationRule]
    [TextInputComponent(Label = "Relative Path Base", Order = 0,
        ExplanationText = "Relative path where the ACA is available. " +
        "The path is relative to the base url of your application. " +
        "It must start with exactly one '/' and must not end with '/' . " +
        "It can contain only letters, numbers, dashes and underscores. " +
        "The path will prefix all other paths created by this integration.")]
    public string RelativePathBase { get; set; } = string.Empty;

    [RequiredValidationRule]
    [AssetSelectorComponent(Label = "Logo", Order = 1, ExplanationText = "Logo Image from a library.", AllowedExtensions = "jpg;jpeg;png", MaximumAssets = 1)]
    public IEnumerable<AssetRelatedItem>? Logo { get; set; }

    [RequiredValidationRule]
    [TextInputComponent(Label = "Chat Title", Order = 2, ExplanationText = "Title of the chat.")]
    public string ChatTitle { get; set; } = string.Empty;

    [RequiredValidationRule]
    [TextInputComponent(Label = "Smart Upload Title", Order = 5, ExplanationText = "Title of the smart upload.")]
    public string SmartUploadTitle { get; set; } = string.Empty;

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

        SmartUploadTitle = info.AiraConfigurationItemAiraSmartUploadTitle;
    }

    public AiraConfigurationItemInfo MapToAiraConfigurationInfo(AiraConfigurationItemInfo? info = null)
    {
        info ??= new AiraConfigurationItemInfo();
        info.AiraConfigurationItemAiraPathBase = RelativePathBase;
        info.AiraConfigurationItemAiraRelativeLogoId = GetImageIdentifier(Logo);

        info.AiraConfigurationItemAiraChatTitle = ChatTitle;

        info.AiraConfigurationItemAiraSmartUploadTitle = SmartUploadTitle;

        return info;
    }

    private static string GetImageIdentifier(IEnumerable<AssetRelatedItem>? asset) => asset?.FirstOrDefault()?.Identifier.ToString() ?? "";

}
