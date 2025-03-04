using CMS.MediaLibrary;

using Kentico.Xperience.Admin.Base.FormAnnotations;
using Kentico.Xperience.AiraUnified.Admin.InfoModels;
using Kentico.Xperience.AiraUnified.Admin.Providers;

namespace Kentico.Xperience.AiraUnified.Admin;

public class AiraUnifiedConfigurationModel
{
    [RequiredValidationRule]
    [TextInputComponent(Label = "Relative Path Base", Order = 0,
        ExplanationText = "Relative path where the Aira unified is available. " +
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
    [TextInputComponent(Label = "Smart Upload Title", Order = 3, ExplanationText = "Title of the smart upload.")]
    public string SmartUploadTitle { get; set; } = string.Empty;

    [RequiredValidationRule]
    [DropDownComponent(Label = "Workspace", DataProviderType = typeof(WorkspaceProvider), Order = 4, ExplanationText = "The workspace used by the smart uploader.")]
    public string Workspace { get; set; } = string.Empty;

    public AiraUnifiedConfigurationModel() { }

    public AiraUnifiedConfigurationModel(
        AiraUnifiedConfigurationItemInfo airaUnifiedConfigurationInfo) => MapFromAiraUnifiedConfigurationInfo(airaUnifiedConfigurationInfo);

    public void MapFromAiraUnifiedConfigurationInfo(AiraUnifiedConfigurationItemInfo info)
    {
        RelativePathBase = info.AiraUnifiedConfigurationItemAiraPathBase;

        if (Guid.TryParse(info.AiraUnifiedConfigurationItemAiraRelativeLogoId, out var relativeLogoUrlGuid))
        {
            var relativeLogoUrlAsset = new AssetRelatedItem
            {
                Identifier = relativeLogoUrlGuid
            };
            Logo = [relativeLogoUrlAsset];
        }

        ChatTitle = info.AiraUnifiedConfigurationItemAiraUnifiedChatTitle;
        Workspace = info.AiraUnifiedConfigurationWorkspaceName;
        SmartUploadTitle = info.AiraUnifiedConfigurationItemAiraSmartUploadTitle;
    }

    public AiraUnifiedConfigurationItemInfo MapToAiraUnifiedConfigurationInfo(
        AiraUnifiedConfigurationItemInfo? info = null
    )
    {
        info ??= new AiraUnifiedConfigurationItemInfo();
        info.AiraUnifiedConfigurationItemAiraPathBase = RelativePathBase;
        info.AiraUnifiedConfigurationItemAiraRelativeLogoId = GetImageIdentifier(Logo);
        info.AiraUnifiedConfigurationItemAiraUnifiedChatTitle = ChatTitle;
        info.AiraUnifiedConfigurationItemAiraSmartUploadTitle = SmartUploadTitle;
        info.AiraUnifiedConfigurationWorkspaceName = Workspace;

        return info;
    }

    private static string GetImageIdentifier(IEnumerable<AssetRelatedItem>? asset) => asset?.FirstOrDefault()?.Identifier.ToString() ?? "";

}
