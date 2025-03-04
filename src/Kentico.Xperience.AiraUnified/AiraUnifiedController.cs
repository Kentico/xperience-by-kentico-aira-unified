using CMS.Core;

using Kentico.Membership;
using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.AiraUnified.Admin;
using Kentico.Xperience.AiraUnified.Assets;
using Kentico.Xperience.AiraUnified.AssetUploader.Models;
using Kentico.Xperience.AiraUnified.Authentication;
using Kentico.Xperience.AiraUnified.Chat;
using Kentico.Xperience.AiraUnified.Chat.Models;
using Kentico.Xperience.AiraUnified.NavBar;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kentico.Xperience.AiraUnified;

/// <summary>
/// The main controller exposing the PWA.
/// </summary>
[ApiController]
[Route("[controller]/[action]")]
public sealed class AiraUnifiedController : Controller
{
    private readonly AdminUserManager adminUserManager;
    private readonly IAiraUnifiedConfigurationService airaUnifiedConfigurationService;
    private readonly IAiraUnifiedChatService airaUnifiedChatService;
    private readonly IAiraUnifiedAssetService airaUnifiedAssetService;
    private readonly INavigationService navigationService;
    private readonly IEventLogService eventLogService;

    public AiraUnifiedController(
        AdminUserManager adminUserManager,
        IAiraUnifiedConfigurationService airaUnifiedConfigurationService,
        IAiraUnifiedAssetService airaUnifiedAssetService,
        INavigationService navigationService,
        IAiraUnifiedChatService airaUnifiedChatService,
        IEventLogService eventLogService)
    {
        this.adminUserManager = adminUserManager;
        this.airaUnifiedConfigurationService = airaUnifiedConfigurationService;
        this.airaUnifiedAssetService = airaUnifiedAssetService;
        this.airaUnifiedChatService = airaUnifiedChatService;
        this.navigationService = navigationService;
        this.eventLogService = eventLogService;
    }

    private const string InvalidPathBaseErrorMessage = "Invalid aira unified path base.";

    /// <summary>
    /// Endpoint exposing access to the Chat page.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var configuration = await GetConfiguration();
        var logoUrl = await airaUnifiedAssetService.GetSanitizedLogoUrl();

        // Only the URLs which origin from the input of the Admin user in aira unified need verification.
        var removePromptUrl = navigationService.BuildUriOrNull(configuration.BaseUrl, configuration.AiraUnifiedPathBase, AiraUnifiedConstants.RemoveUsedPromptGroupRelativeUrl);
        var navigationUrl = navigationService.BuildUriOrNull(configuration.BaseUrl, configuration.AiraUnifiedPathBase, AiraUnifiedConstants.NavigationUrl);
        var historyUrl = navigationService.BuildUriOrNull(configuration.BaseUrl, configuration.AiraUnifiedPathBase, AiraUnifiedConstants.ChatRelativeUrl, AiraUnifiedConstants.ChatHistoryUrl);
        var chatUrl = navigationService.BuildUriOrNull(configuration.BaseUrl, configuration.AiraUnifiedPathBase, AiraUnifiedConstants.ChatRelativeUrl, AiraUnifiedConstants.ChatMessageUrl);

        if (removePromptUrl is null
            || navigationUrl is null
            || historyUrl is null
            || chatUrl is null)
        {
            eventLogService.LogError(nameof(AiraUnifiedController), nameof(Index), InvalidPathBaseErrorMessage);

            return BadRequest(InvalidPathBaseErrorMessage);
        }

        var chatModel = new ChatViewModel
        {
            PathBase = configuration.AiraUnifiedPathBase,
            AIIconImagePath = $"/{AiraUnifiedConstants.RCLUrlPrefix}/{AiraUnifiedConstants.PictureStarImgPath}",
            RemovePromptUrl = removePromptUrl.ToString(),
            ServicePageViewModel = new ServicePageViewModel()
            {
                ChatAiraIconUrl = $"/{AiraUnifiedConstants.RCLUrlPrefix}/{AiraUnifiedConstants.PictureStarImgPath}",
                ChatUnavailableIconUrl = $"/{AiraUnifiedConstants.RCLUrlPrefix}/{AiraUnifiedConstants.PictureChatBotSmileBubbleOrangeImgPath}",
                ChatUnavailableMainMessage = Resource.ServicePageChatUnavailable,
                ChatUnavailableTryAgainMessage = Resource.ServicePageChatTryAgainLater
            },
            NavigationUrl = navigationUrl.ToString(),
            NavigationPageIdentifier = AiraUnifiedConstants.ChatRelativeUrl,
            HistoryUrl = historyUrl.ToString(),
            ChatUrl = chatUrl.ToString(),
            LogoImgRelativePath = logoUrl
        };

        return View("~/Chat/Chat.cshtml", chatModel);
    }

    /// <summary>
    /// Retrieves the navigation view model.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Navigation([FromBody] NavBarRequestModel request)
    {
        if (!string.Equals(request.PageIdentifier, AiraUnifiedConstants.ChatRelativeUrl) &&
            !string.Equals(request.PageIdentifier, AiraUnifiedConstants.SmartUploadRelativeUrl))
        {
            return BadRequest("The navigation menu for the specified page does not exist.");
        }

        var model = await navigationService.GetNavBarViewModel(request.PageIdentifier, baseUrl: HttpContext.Request.GetBaseUrl());

        // Only the URLs which origin from the input of the Admin user in aira unified need verification.
        if (model.ChatItem.Url is null
            || model.SmartUploadItem is null)
        {
            eventLogService.LogError(nameof(INavigationService), nameof(INavigationService.GetNavBarViewModel), InvalidPathBaseErrorMessage);

            return BadRequest(InvalidPathBaseErrorMessage);
        }

        return Ok(model);
    }

    /// <summary>
    /// Endpoint exposing the user's chat history.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetChatHistory()
    {
        var user = await adminUserManager.GetUserAsync(User);

        // User can not be null, because he is already checked in the AiraUnifiedEndpointDataSource middleware.
        var history = await airaUnifiedChatService.GetUserChatHistory(user!.UserID);

        if (history.Count == 0)
        {
            history = [
                new AiraUnifiedChatMessageViewModel
                {
                    Message = Resource.InitialAiraMessageIntroduction,
                    Role = AiraUnifiedConstants.AiraUnifiedChatRoleName
                },
                new AiraUnifiedChatMessageViewModel
                {
                    Message = Resource.InitialAiraMessagePromptExplanation,
                    Role = AiraUnifiedConstants.AiraUnifiedChatRoleName
                }
            ];

            return Ok(history);
        }

        history.Add(new AiraUnifiedChatMessageViewModel
        {
            Message = Resource.WelcomeBackAiraMessage,
            Role = AiraUnifiedConstants.AiraUnifiedChatRoleName
        });

        return Ok(history);
    }

    /// <summary>
    /// Endpoint allowing chat communication via the chat interface.
    /// </summary>
    /// <param name="request">The <see cref="IFormCollection"/> including the chat message.</param>
    [HttpPost]
    public async Task<IActionResult> PostChatMessage(IFormCollection request)
    {
        var user = await adminUserManager.GetUserAsync(User);

        string? message;

#pragma warning disable IDE0079 // Kept for development. This will be restored in subsequent versions.
#pragma warning disable S6932 // Kept for development. This will be restored in subsequent versions.
        if (request.TryGetValue("message", out var messages))
        {
            message = messages.ToString().Replace("\"", "");
        }
#pragma warning restore S6932 //
#pragma warning restore IDE0079 //
        else
        {
            return Ok();
        }
        if (string.IsNullOrEmpty(message))
        {
            return Ok();
        }

        AiraUnifiedChatMessageViewModel result;

        // User can not be null, because he is already checked in the AiraUnifiedEndpointDataSource middleware
        airaUnifiedChatService.SaveMessage(message, user!.UserID, AiraUnifiedConstants.UserChatRoleName);

        try
        {
            var aiResponse = await airaUnifiedChatService.GetAIResponseOrNull(message, numberOfIncludedHistoryMessages: 5, user.UserID);

            if (aiResponse is null)
            {
                result = new AiraUnifiedChatMessageViewModel
                {
                    ServiceUnavailable = true,
                };

                return Ok(result);
            }

            airaUnifiedChatService.SaveMessage(aiResponse.Response, user.UserID, AiraUnifiedConstants.AiraUnifiedChatRoleName);

            airaUnifiedChatService.UpdateChatSummary(user.UserID, message);

            result = new AiraUnifiedChatMessageViewModel
            {
                Role = AiraUnifiedConstants.AiraUnifiedChatRoleName,
                Message = aiResponse.Response
            };

            if (aiResponse.SuggestedQuestions is not null)
            {
                var promptGroup = airaUnifiedChatService.SaveAiraPrompts(user.UserID, aiResponse.SuggestedQuestions);
                result.QuickPrompts = promptGroup.QuickPrompts;
                result.QuickPromptsGroupId = promptGroup.QuickPromptsGroupId.ToString();
            }
        }
        catch (Exception ex)
        {
            result = new AiraUnifiedChatMessageViewModel
            {
                Role = AiraUnifiedConstants.AiraUnifiedChatRoleName,
                ServiceUnavailable = true,
                Message = $"Error: {ex.Message}"
            };
        }

        return Ok(result);
    }

    /// <summary>
    /// Endpoint allowing removal of a used suggested prompt group.
    /// </summary>
    /// <param name="model">The <see cref="AiraUnifiedUsedPromptGroupModel"/> with the information about the prompt group.</param>
    [HttpPost]
    public IActionResult RemoveUsedPromptGroup([FromBody] AiraUnifiedUsedPromptGroupModel model)
    {
        airaUnifiedChatService.RemoveUsedPrompts(model.GroupId);

        return Ok();
    }

    /// <summary>
    /// Endpoint allowing upload of the files via smart upload.
    /// </summary>
    /// <param name="request">The <see cref="IFormCollection"/> request containing the uploaded files.</param>
    [HttpPost]
    public async Task<IActionResult> PostImages(IFormCollection request)
    {
        var user = await adminUserManager.GetUserAsync(User);

        // User can not be null, because he is already checked in the AiraUnifiedEndpointDataSource middleware
        var uploadSuccessful = await airaUnifiedAssetService.HandleFileUpload(request.Files, user!.UserID);

        if (uploadSuccessful)
        {
            return Ok();
        }

        return BadRequest("Attempted to upload file with forbidden format.");
    }

    /// <summary>
    /// Endpoint allowing accessing the smart upload page.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Assets()
    {
        var configuration = await GetConfiguration();

        // Only the URLs which origin from the input of the Admin user in aira unified need verification.
        var allowedFileExtensionsUrl = navigationService.BuildUriOrNull(configuration.BaseUrl, configuration.AiraUnifiedPathBase, AiraUnifiedConstants.SmartUploadRelativeUrl, AiraUnifiedConstants.SmartUploadAllowedFileExtensionsUrl);
        var navigationUrl = navigationService.BuildUriOrNull(configuration.BaseUrl, configuration.AiraUnifiedPathBase, AiraUnifiedConstants.NavigationUrl);
        var uploadUrl = navigationService.BuildUriOrNull(configuration.BaseUrl, configuration.AiraUnifiedPathBase, AiraUnifiedConstants.SmartUploadRelativeUrl, AiraUnifiedConstants.SmartUploadUploadUrl);

        if (allowedFileExtensionsUrl is null
            || navigationUrl is null
            || uploadUrl is null)
        {
            eventLogService.LogError(nameof(AiraUnifiedController), nameof(Assets), InvalidPathBaseErrorMessage);

            return BadRequest(InvalidPathBaseErrorMessage);
        }

        var model = new AssetsViewModel
        {
            PathBase = configuration.AiraUnifiedPathBase,
            AllowedFileExtensionsUrl = allowedFileExtensionsUrl.ToString(),
            SelectFilesButton = Resource.SmartUploadSelectFilesButton,
            FilesUploadedMessage = Resource.SmartUploadFilesUploadedMessage,
            NavigationPageIdentifier = AiraUnifiedConstants.SmartUploadRelativeUrl,
            NavigationUrl = navigationUrl.ToString(),
            UploadUrl = uploadUrl.ToString()
        };

        return View("~/AssetUploader/Assets.cshtml", model);
    }

    /// <summary>
    /// Endpoint retrieving the allowed smart upload file extensions.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllowedFileExtensions()
    {
        var allowedExtensions = await airaUnifiedAssetService.GetAllowedFileExtensions();

        return Ok(allowedExtensions);
    }

    /// <summary>
    /// Endpoint exposing the manifest file for the PWA.
    /// </summary>
    [HttpGet($"/{AiraUnifiedConstants.RCLUrlPrefix}/manifest.json")]
    [Produces("application/json")]
    public async Task<IActionResult> GetPwaManifest()
    {
        var configuration = await airaUnifiedConfigurationService.GetAiraUnifiedConfiguration();

        var libraryBasePath = '/' + AiraUnifiedConstants.RCLUrlPrefix;

        var manifest = new
        {
            name = "AiraUnified",
            short_name = "AiraUnified",
            start_url = $"{configuration.AiraUnifiedConfigurationItemAiraPathBase}/{AiraUnifiedConstants.ChatRelativeUrl}",
            display = "standalone",
            background_color = "#ffffff",
            theme_color = "#ffffff",
            scope = "/",
            icons = new[]
            {
                new
                {
                    src = $"{libraryBasePath}/img/favicon/android-chrome-192x192.png",
                    sizes = "192x192",
                    type = "image/png"
                },
                new
                {
                    src = $"{libraryBasePath}/img/favicon/android-chrome-512x512.png",
                    sizes = "512x512",
                    type = "image/png"
                }
            }
        };

        return Json(manifest);
    }

    /// <summary>
    /// Endpoint retrieving the SignIn page.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> SignIn()
    {
        var airaUnifiedConfiguration = await airaUnifiedConfigurationService.GetAiraUnifiedConfiguration();

        var baseUrl = HttpContext.Request.GetBaseUrl();

        // Only the URLs which origin from the input of the Admin user in aira unified need verification.
        var chatUrl = navigationService.BuildUriOrNull(baseUrl, airaUnifiedConfiguration.AiraUnifiedConfigurationItemAiraPathBase, AiraUnifiedConstants.ChatRelativeUrl, AiraUnifiedConstants.ChatMessageUrl);

        if (chatUrl is null)
        {
            eventLogService.LogError(nameof(AiraUnifiedController), nameof(SignIn), InvalidPathBaseErrorMessage);

            return BadRequest(InvalidPathBaseErrorMessage);
        }

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable S6932 // Kept, we are using the aira unified middleware, where we want to specify this parameter, therefore asp.net's query parameter should not be used, because we also want to reference the samme value in that middleware and we would not be able to use a variable for the query parameter.
        var missingPermission = Request.Query[AiraUnifiedConstants.SigninMissingPermissionParameterName].ToString();
#pragma warning restore S6932 //
#pragma warning restore IDE0079 // Remove unnecessary suppression

        var errorMessage = string.IsNullOrEmpty(missingPermission) ? null : $"You do not have the Aira Unified {missingPermission} permission.";

        var logoUrl = await airaUnifiedAssetService.GetSanitizedLogoUrl();

        var model = new SignInViewModel
        {
            PathBase = airaUnifiedConfiguration.AiraUnifiedConfigurationItemAiraPathBase,
            ChatUrl = AiraUnifiedConstants.ChatRelativeUrl,
            LogoImageRelativePath = logoUrl,
            ErrorMessage = errorMessage
        };

        return View("~/Authentication/SignIn.cshtml", model);
    }

    private sealed record ConfigurationModel(string BaseUrl, string AiraUnifiedPathBase);

    private async Task<ConfigurationModel> GetConfiguration()
    {
        var airaUnifiedConfiguration = await airaUnifiedConfigurationService.GetAiraUnifiedConfiguration();
        var baseUrl = HttpContext.Request.GetBaseUrl();

        return new ConfigurationModel(baseUrl, airaUnifiedConfiguration.AiraUnifiedConfigurationItemAiraPathBase);
    }
}
