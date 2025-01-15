namespace Kentico.Xperience.Aira.Admin;

public static class AiraCompanionAppConstants
{
    public const string ChatRelativeUrl = "chat";
    public const string ChatMessageUrl = "message";
    public const string SmartUploadRelativeUrl = "assets";
    public const string SmartUploadUploadUrl = "upload";
    public const string SigninRelativeUrl = "signin";

    public const string AiraChatRoleName = "ai";
    public const string UserChatRoleName = "user";

    public static readonly List<string> AiraChatInitialAiraMessages = [
        "Hi, Im AIRA your AI powered coworker. I can answer questions about your Kentico Xperience data and even carry out tasks for you. Let me show you how I work....",
        "Every time you open the chat dialog you can ask me directly or you can use some pre-made requests called prompts",
        "We can try it right now, choose one of the prompts below or you can use our prompt library below the chat."
    ];

    public const string AiraChatAIWelcomeBackMessage = "What can I help you with ?";

    public const string RCLUrlPrefix = "_content/Kentico.Xperience.Aira";
    public const string PicturePlaceholderImgPath = "img/icons/picture-placeholder-purple.svg";
    public const string PictureNetworkGraphImgPath = "img/icons/network-purple.svg";
    public const string PictureHatImgPath = "img/icons/magic-hat.svg";

    public const string AiraRoleName = "AiraUser";
    public const string AiraRoleDisplayName = "Aira User";
    public const string AiraRoleDescription = "Role for Aira users";

    public const string XperienceAdminSchemeName = "Xperience.Application";

    public const string ResourceDisplayName = "Kentico Integration - Aira";
    public const string ResourceName = "CMS.Integration.Aira";
    public const string ResourceDescription = "Kentico Aira custom data";
    public const string MassAssetUploadConfigurationKey = "CMSMassAssetUploadConfiguration";
    public const bool ResourceIsInDevelopment = false;
}
