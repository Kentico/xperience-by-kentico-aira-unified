namespace Kentico.Xperience.Aira.Admin;

/// <summary>
/// Provides application constant strings.
/// </summary>
public static class AiraCompanionAppConstants
{
    /// <summary>
    /// Relative path to the chat page in the PWA.
    /// </summary>
    public const string ChatRelativeUrl = "chat";

    /// <summary>
    /// Relative path for sending chat messages to AIRA.
    /// </summary>
    public const string ChatMessageUrl = "message";

    /// <summary>
    /// Relative path to the smart upload page in the PWA.
    /// </summary>
    public const string SmartUploadRelativeUrl = "assets";

    /// <summary>
    /// Relative path for retrieval of allowed file extensions in the Smart upload ACA page.
    /// </summary>
    public const string SmartUploadAllowedFileExtensionsUrl = "allowedFileExtensions";

    /// <summary>
    /// Relative path for uploading assets to ACA.
    /// </summary>
    public const string SmartUploadUploadUrl = "upload";

    /// <summary>
    /// Relative path to the PWA signin page.
    /// </summary>
    public const string SigninRelativeUrl = "signin";

    /// <summary>
    /// Relative path to the used prompt group removal endpoint.
    /// </summary>
    public const string RemoveUsedPromptGroupRelativeUrl = "prompt/use";

    /// <summary>
    /// The name of the ai role used within chat page.
    /// </summary>
    public const string AiraChatRoleName = "ai";

    public const string AIRequestAssistantRoleName = "assistant";

    public const string AIRequestUserRoleName = "user";

    public const string AiraAIEndpoint = "https://kenticoairaapimanager.azure-api.net/aira/rag";

    /// <summary>
    /// The identifier of the ai role used to identify the role in the database.
    /// </summary>
    public const int AiraChatRoleIdentifier = 0;

    /// <summary>
    /// The name of the user role used within chat page.
    /// </summary>
    public const string UserChatRoleName = "user";

    /// <summary>
    /// The identifier of the user role used to identify the role in the database.
    /// </summary>
    public const int UserChatRoleIdentifier = 1;

    /// <summary>
    /// Url prefix where static assets of this package are accessible.
    /// </summary>
    public const string RCLUrlPrefix = "_content/Kentico.Xperience.Aira";

    /// <summary>
    /// Path where the placeholder icon is located.
    /// </summary>
    public const string PicturePlaceholderImgPath = "img/icons/picture-placeholder-purple.svg";

    /// <summary>
    /// Path where the network icon is located.
    /// </summary>
    public const string PictureNetworkGraphImgPath = "img/icons/network-purple.svg";

    /// <summary>
    /// Path where the star icon is located.
    /// </summary>
    public const string PictureStarImgPath = "img/icons/stars-icon.svg";

    /// <summary>
    /// 
    /// </summary>
    public const string PictureMessageImgPath = "img/icons/chatbot-smile-bubble-orange.svg";

    /// <summary>
    /// Name of the Xperience Admin User authentication scheme.
    /// </summary>
    public const string XperienceAdminSchemeName = "Xperience.Application";

    /// <summary>
    /// Resource Display Name of this module.
    /// </summary>
    public const string ResourceDisplayName = "Kentico Integration - Aira";

    /// <summary>
    /// Resource Name of this module.
    /// </summary>
    public const string ResourceName = "CMS.Integration.Aira";

    /// <summary>
    /// Resource Description of this module.
    /// </summary>
    public const string ResourceDescription = "Kentico Aira custom data";

    /// <summary>
    /// Configuration Key of the mass asset upload.
    /// </summary>
    public const string MassAssetUploadConfigurationKey = "CMSMassAssetUploadConfiguration";

    /// <summary>
    /// Resource is in development attribute of this module.
    /// </summary>
    public const bool ResourceIsInDevelopment = false;
}
