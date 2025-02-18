using CMS.Membership;

using HotChocolate.Authorization;

using Kentico.Membership;
using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Aira.Admin;
using Kentico.Xperience.Aira.Assets;
using Kentico.Xperience.Aira.AssetUploader.Models;
using Kentico.Xperience.Aira.Authentication;
using Kentico.Xperience.Aira.Chat;
using Kentico.Xperience.Aira.Chat.Models;
using Kentico.Xperience.Aira.NavBar;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kentico.Xperience.Aira;

/// <summary>
/// The main controller exposing the PWA.
/// </summary>
[ApiController]
[Route("[controller]/[action]")]
public sealed class AiraCompanionAppController : Controller
{
    private readonly AdminUserManager adminUserManager;
    private readonly IAiraConfigurationService airaConfigurationService;
    private readonly IAiraChatService airaChatService;
    private readonly IAiraAssetService airaAssetService;
    private readonly INavBarService navBarService;

    public AiraCompanionAppController(
        AdminUserManager adminUserManager,
        IAiraConfigurationService airaConfigurationService,
        IAiraAssetService airaAssetService,
        INavBarService navBarService,
        IAiraChatService airaChatService)
    {
        this.adminUserManager = adminUserManager;
        this.airaConfigurationService = airaConfigurationService;
        this.airaAssetService = airaAssetService;
        this.airaChatService = airaChatService;
        this.navBarService = navBarService;
    }

    /// <summary>
    /// Endpoint exposing access to the Chat page.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var airaPathBase = await GetAiraPathBase();

        var user = await adminUserManager.GetUserAsync(User);

        var signinRedirectUrl = GetRedirectUrl(AiraCompanionAppConstants.SigninRelativeUrl, airaPathBase);

        if (user is null)
        {
            return Redirect(signinRedirectUrl);
        }

        var hasAiraViewPermission = await airaAssetService.DoesUserHaveAiraCompanionAppPermission(SystemPermissions.VIEW, user.UserID);

        if (!hasAiraViewPermission)
        {
            return Redirect(signinRedirectUrl);
        }

        var chatModel = new ChatViewModel
        {
            PathBase = airaPathBase,
            History = await airaChatService.GetUserChatHistory(user.UserID),
            AIIconImagePath = $"/{AiraCompanionAppConstants.RCLUrlPrefix}/{AiraCompanionAppConstants.PictureStarImgPath}",
            NavBarViewModel = await navBarService.GetNavBarViewModel(AiraCompanionAppConstants.ChatRelativeUrl),
            RemovePromptUrl = AiraCompanionAppConstants.RemoveUsedPromptGroupRelativeUrl,
            ServicePageViewModel = new ServicePageViewModel()
            {
                ChatAiraIconUrl = $"/{AiraCompanionAppConstants.RCLUrlPrefix}/{AiraCompanionAppConstants.PictureStarImgPath}",
                ChatUnavailableIconUrl = $"/{AiraCompanionAppConstants.RCLUrlPrefix}/{AiraCompanionAppConstants.PictureMessageImgPath}",
                ChatUnvailableMainMessage = Resource.ServicePageChatUnavailable,
                ChatUnavailableTryAgainMessage = Resource.ServicePageChatTryAgainLater
            }
        };

        if (chatModel.History.Count == 0)
        {
            chatModel.History = [
                new AiraChatMessageViewModel
                {
                    Message = Resource.InitialAiraMessage1,
                    Role = AiraCompanionAppConstants.AiraChatRoleName
                },
                new AiraChatMessageViewModel
                {
                    Message = Resource.InitialAiraMessage2,
                    Role = AiraCompanionAppConstants.AiraChatRoleName
                },
            ];
        }
        else
        {
            chatModel.History.Add(new AiraChatMessageViewModel
            {
                Message = Resource.WelcomeBackAiraMessage,
                Role = AiraCompanionAppConstants.AiraChatRoleName
            });
        }

        return View("~/Chat/Chat.cshtml", chatModel);
    }

    /// <summary>
    /// Endpoint allowing chat communication via the chat interface.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> PostChatMessage(IFormCollection request)
    {
        var airaPathBase = await GetAiraPathBase();

        var user = await adminUserManager.GetUserAsync(User);

        var signinRedirectUrl = GetRedirectUrl(AiraCompanionAppConstants.SigninRelativeUrl, airaPathBase);

        if (user is null)
        {
            return Redirect(signinRedirectUrl);
        }

        var hasAiraViewPermission = await airaAssetService.DoesUserHaveAiraCompanionAppPermission(SystemPermissions.VIEW, user.UserID);

        if (!hasAiraViewPermission)
        {
            return Redirect(signinRedirectUrl);
        }

        var message = string.Empty;

        if (request.TryGetValue("message", out var messages))
        {
            message = messages.ToString().Replace("\"", "");
        }
        else
        {
            return Ok();
        }
        if (string.IsNullOrEmpty(message))
        {
            return Ok();
        }

        AiraChatMessageViewModel result;

        airaChatService.SaveMessage(message, user.UserID, AiraCompanionAppConstants.UserChatRoleName);

        try
        {
            var aiResponse = await airaChatService.GetAIResponseOrNull(message, numberOfIncludedHistoryMessages: 5, user.UserID);

            if (aiResponse is null)
            {
                result = new AiraChatMessageViewModel
                {
                    ServiceUnavailable = true,
                };

                return Ok(result);
            }

            airaChatService.SaveMessage(aiResponse.Response, user.UserID, AiraCompanionAppConstants.AiraChatRoleName);

            airaChatService.UpdateChatSummary(user.UserID, message);

            result = new AiraChatMessageViewModel
            {
                Role = AiraCompanionAppConstants.AiraChatRoleName,
                Message = aiResponse.Response
            };

            if (aiResponse.SuggestedQuestions is not null)
            {
                var promptGroup = airaChatService.SaveAiraPrompts(user.UserID, aiResponse.SuggestedQuestions);
                result.QuickPrompts = promptGroup.QuickPrompts;
                result.QuickPromptsGroupId = promptGroup.QuickPromptsGroupId.ToString();
            }
        }
        catch (Exception ex)
        {
            result = new AiraChatMessageViewModel
            {
                Role = AiraCompanionAppConstants.AiraChatRoleName,
                ServiceUnavailable = true,
                Message = $"Error: {ex.Message}"
            };
        }

        return Ok(result);
    }

    /// <summary>
    /// Endpoint allowing removal of a used suggested prompt group.
    /// </summary>
    /// <param name="model">The <see cref="AiraUsedPromptGroupModel"/> with the information about the prompt group.</param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult RemoveUsedPromptGroup([FromBody] AiraUsedPromptGroupModel model)
    {
        airaChatService.RemoveUsedPrompts(model.GroupId);

        return Ok();
    }

    /// <summary>
    /// Endpoint allowing upload of the files via smart upload.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> PostImages(IFormCollection request)
    {
        var airaPathBase = await GetAiraPathBase();

        var user = await adminUserManager.GetUserAsync(User);

        var signinRedirectUrl = GetRedirectUrl(AiraCompanionAppConstants.SigninRelativeUrl, airaPathBase);

        if (user is null)
        {
            return Redirect(signinRedirectUrl);
        }

        var hasAiraViewPermission = await airaAssetService.DoesUserHaveAiraCompanionAppPermission(SystemPermissions.CREATE, user.UserID);

        if (!hasAiraViewPermission)
        {
            return Redirect(signinRedirectUrl);
        }

        await airaAssetService.HandleFileUpload(request.Files, user.UserID);
        return Ok();
    }

    /// <summary>
    /// Endpoint allowing accessing the smart upload page.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Assets()
    {
        var airaPathBase = await GetAiraPathBase();

        var user = await adminUserManager.GetUserAsync(User);

        var signinRedirectUrl = GetRedirectUrl(AiraCompanionAppConstants.SigninRelativeUrl, airaPathBase);

        if (user is null)
        {
            return Redirect(signinRedirectUrl);
        }

        var hasAiraCreatePermission = await airaAssetService.DoesUserHaveAiraCompanionAppPermission(SystemPermissions.CREATE, user.UserID);

        if (!hasAiraCreatePermission)
        {
            return Redirect(signinRedirectUrl);
        }

        var model = new AssetsViewModel
        {
            NavBarViewModel = await navBarService.GetNavBarViewModel(AiraCompanionAppConstants.SmartUploadRelativeUrl),
            PathBase = airaPathBase
        };

        return View("~/AssetUploader/Assets.cshtml", model);
    }

    /// <summary>
    /// Endpoint exposing the manifest file for the PWA.
    /// </summary>
    /// <returns></returns>
    [HttpGet($"/{AiraCompanionAppConstants.RCLUrlPrefix}/manifest.json")]
    [Produces("application/json")]
    public async Task<IActionResult> GetPwaManifest()
    {
        var configuration = await airaConfigurationService.GetAiraConfiguration();

        var libraryBasePath = '/' + AiraCompanionAppConstants.RCLUrlPrefix;

        var manifest = new
        {
            name = "Aira",
            short_name = "Aira",
            start_url = $"{configuration.AiraConfigurationItemAiraPathBase}/{AiraCompanionAppConstants.ChatRelativeUrl}",
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
        var airaConfiguration = await airaConfigurationService.GetAiraConfiguration();
        var logoUrl = navBarService.GetMediaFileUrl(airaConfiguration.AiraConfigurationItemAiraRelativeLogoId)?.RelativePath;
        logoUrl = navBarService.GetSanitizedImageUrl(logoUrl, $"/{AiraCompanionAppConstants.RCLUrlPrefix}/{AiraCompanionAppConstants.PictureStarImgPath}", "AIRA Logo");

        var model = new SignInViewModel
        {
            PathBase = airaConfiguration.AiraConfigurationItemAiraPathBase,
            ChatRelativeUrl = AiraCompanionAppConstants.ChatRelativeUrl,
            LogoImageRelativePath = logoUrl
        };

        return View("~/Authentication/SignIn.cshtml", model);
    }

    private async Task<string> GetAiraPathBase()
    {
        var configuration = await airaConfigurationService.GetAiraConfiguration();

        return configuration.AiraConfigurationItemAiraPathBase;
    }

    private string GetRedirectUrl(string relativeUrl, string airaPathBase)
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        return $"{baseUrl}{airaPathBase}/{relativeUrl}";
    }
}
