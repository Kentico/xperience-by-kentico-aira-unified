namespace Kentico.Xperience.AiraUnified.Admin;

/// <summary>
/// Provides application constant strings.
/// </summary>
public static class AiraUnifiedConstants
{
    /// <summary>
    /// Relative path to the chat page in the PWA.
    /// </summary>
    public const string ChatRelativeUrl = "chat";

    /// <summary>
    /// Relative path for sending chat messages to aira unified chat.
    /// </summary>
    public const string ChatMessageUrl = "message";

    /// <summary>
    /// Relative path for retrieval of user's chat history.
    /// </summary>
    public const string ChatHistoryUrl = "history";

    /// <summary>
    /// Relative path for retrieval of the navigation model <see cref="NavBar.NavBarViewModel"/> data.
    /// </summary>
    public const string NavigationUrl = "nav";

    /// <summary>
    /// Relative path to the smart upload page in the PWA.
    /// </summary>
    public const string SmartUploadRelativeUrl = "assets";

    /// <summary>
    /// Relative path for retrieval of allowed file extensions in the Smart upload Aira unified page.
    /// </summary>
    public const string SmartUploadAllowedFileExtensionsUrl = "allowedFileExtensions";

    /// <summary>
    /// Relative path for uploading assets to Aira unified.
    /// </summary>
    public const string SmartUploadUploadUrl = "upload";

    /// <summary>
    /// Relative path to the PWA signIn page.
    /// </summary>
    public const string SigninRelativeUrl = "signin";

    /// <summary>
    /// Name of the optional query parameter of the signIn endpoint.
    /// </summary>
    public const string SigninMissingPermissionParameterName = "missingPermission";

    /// <summary>
    /// Relative path to the used prompt group removal endpoint.
    /// </summary>
    public const string RemoveUsedPromptGroupRelativeUrl = "prompt/use";

    /// <summary>
    /// The name of the ai role used within chat page.
    /// </summary>
    public const string AiraUnifiedChatRoleName = "ai";

    /// <summary>
    /// The name of the ai role used for ai endpoint identifier.
    /// </summary>
    public const string AIRequestAssistantRoleName = "assistant";

    /// <summary>
    /// The name of the user role used for ai endpoint identifier.
    /// </summary>
    public const string AIRequestUserRoleName = "user";

    /// <summary>
    /// The ai endpoint.
    /// </summary>
    public const string AiraUnifiedAIEndpoint = "https://kenticoairaapimanager.azure-api.net/aira/rag";

    /// <summary>
    /// The identifier of the ai role used to identify the role in the database.
    /// </summary>
    public const int AiraUnifiedChatRoleIdentifier = 0;

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
    public const string RCLUrlPrefix = "_content/Kentico.Xperience.AiraUnified";

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
    /// Path where the chat bot smile bubble orange icon is located.
    /// </summary>
    public const string PictureChatBotSmileBubbleOrangeImgPath = "img/icons/chatbot-smile-bubble-orange.svg";

    /// <summary>
    /// Name of the Xperience Admin User authentication scheme.
    /// </summary>
    public const string XperienceAdminSchemeName = "Xperience.Application";

    /// <summary>
    /// Resource Display Name of this module.
    /// </summary>
    public const string ResourceDisplayName = "Kentico Integration - Aira Unified";

    /// <summary>
    /// Resource Name of this module.
    /// </summary>
    public const string ResourceName = "CMS.Integration.AiraUnified";

    /// <summary>
    /// Resource Description of this module.
    /// </summary>
    public const string ResourceDescription = "Kentico Aira Unified custom data";

    /// <summary>
    /// Configuration Key of the mass asset upload.
    /// </summary>
    public const string MassAssetUploadConfigurationKey = "CMSMassAssetUploadConfiguration";

    /// <summary>
    /// Resource is in development attribute of this module.
    /// </summary>
    public const bool ResourceIsInDevelopment = false;

    /// <summary>
    /// Insights email identifier.
    /// </summary>
    public const string InsightsEmailIdentifier = "email";

    /// <summary>
    /// Insights content identifier.
    /// </summary>
    public const string InsightsContentIdentifier = "content";

    /// <summary>
    /// Insights in draft identifier.
    /// </summary>
    public const string InsightsInDraftIdentifier = "inDraft";

    /// <summary>
    /// Insights draft identifier.
    /// </summary>
    public const string InsightsDraftIdentifier = "Draft";

    /// <summary>
    /// Insights scheduled identifier.
    /// </summary>
    public const string InsightsScheduledIdentifier = "Scheduled";

    /// <summary>
    /// Insights in scheduled identifier.
    /// </summary>
    public const string InsightsInScheduledIdentifier = "inScheduled";

    /// <summary>
    /// Insights reusable identifier.
    /// </summary>
    public const string InsightsReusableIdentifier = "reusable";

    /// <summary>
    /// Insights website identifier.
    /// </summary>
    public const string InsightsWebsiteIdentifier = "website";

    /// <summary>
    /// Insights all accounts identifier.
    /// </summary>
    public const string InsightsAllAccountsIdentifier = "allAccounts";

    /// <summary>
    /// Insights contact group identifier.
    /// </summary>
    public const string InsightsContactGroupIdentifier = "contactGroup";

    /// <summary>
    /// Insights count identifier.
    /// </summary>
    public const string InsightsCountIdentifier = "count";

    /// <summary>
    /// Insights ratio of contacts to other contacts identifier.
    /// </summary>
    public const string InsightsRatioToAllContactsIdentifier = "ratioOfContactsInGroupToOtherContacts";
}
