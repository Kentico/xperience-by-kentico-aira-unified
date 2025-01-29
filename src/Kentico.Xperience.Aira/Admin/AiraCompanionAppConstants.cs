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
    /// Relative path for uploading assets to ACA.
    /// </summary>
    public const string SmartUploadUploadUrl = "upload";

    /// <summary>
    /// Relative path to the PWA signin page.
    /// </summary>
    public const string SigninRelativeUrl = "signin";

    /// <summary>
    /// The name of the ai role used within chat page.
    /// </summary>
    public const string AiraChatRoleName = "ai";

    /// <summary>
    /// The name of the user role used within chat page.
    /// </summary>
    public const string UserChatRoleName = "user";

    public static readonly List<string> AiraChatInitialAiraMessages = [
        "Hi, I'm AIRA your AI powered coworker. I can answer questions about your Kentico Xperience data and even carry out tasks for you. Let me show you how I work....",
        "Every time you open the chat dialog you can ask me directly or you can use some pre-made requests called prompts",
        "We can try it right now, choose one of the prompts below or you can use our prompt library below the chat."
    ];

    public const string AiraChatAIWelcomeBackMessage = "What can I help you with ?";

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
    /// Path where the hat icon is located.
    /// </summary>
    public const string PictureHatImgPath = "img/icons/magic-hat.svg";

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
