using CMS.Membership;

using HotChocolate.Authorization;

using Kentico.Membership;
using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.Authentication.Internal;
using Kentico.Xperience.Aira.Admin;
using Kentico.Xperience.Aira.Assets;
using Kentico.Xperience.Aira.AssetUploader.Models;
using Kentico.Xperience.Aira.Authentication;
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
    private readonly AdminSignInManager signInManager;
    private readonly AdminUserManager adminUserManager;
    private readonly IAiraConfigurationService airaConfigurationService;
    private readonly IAiraAssetService airaAssetService;
    private readonly INavBarService airaUIService;

    public AiraCompanionAppController(AdminSignInManager signInManager,
        AdminUserManager adminUserManager,
        IAiraConfigurationService airaConfigurationService,
        IAiraAssetService airaAssetService,
        INavBarService airaUIService)
    {
        this.adminUserManager = adminUserManager;
        this.airaConfigurationService = airaConfigurationService;
        this.signInManager = signInManager;
        this.airaAssetService = airaAssetService;
        this.airaUIService = airaUIService;
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
            AIIconImagePath = $"/{AiraCompanionAppConstants.RCLUrlPrefix}/{AiraCompanionAppConstants.PictureHatImgPath}",
            NavBarViewModel = await airaUIService.GetNavBarViewModel(AiraCompanionAppConstants.ChatRelativeUrl)
        };

        if (chatModel.History.Count == 0)
        {
            chatModel.History.AddRange(
                AiraCompanionAppConstants.AiraChatInitialAiraMessages.Select(x => new AiraChatMessage
                {
                    Message = x,
                    Role = AiraCompanionAppConstants.AiraChatRoleName
                })
            );
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

        string? message = null;
        if (request.TryGetValue("message", out var messages))
        {
            message = messages.ToString().Replace("\"", "");
        }

        var response = new AiraChatMessage
        {
            Role = AiraCompanionAppConstants.AiraChatRoleName,
            Message = "Ok",
            QuickPrompts = message == "Prompts" ?
                ["Prompts", "Just Message"] : []
        };

        return Ok(response);
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
            NavBarViewModel = await airaUIService.GetNavBarViewModel(AiraCompanionAppConstants.SmartUploadRelativeUrl),
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

    public class CheckAuthenticationModel
    {
        public bool IsAuthenticated { get; set; }

        public CheckAuthenticationModel(bool isAuthenticated)
            => IsAuthenticated = isAuthenticated;
    }

    /// <summary>
    /// Endpoint retrieving the signin page.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Signin()
    {
        var airaPathBase = await GetAiraPathBase();

        var model = new SignInViewModel
        {
            PathBase = airaPathBase,
            ChatRelativeUrl = AiraCompanionAppConstants.ChatRelativeUrl
        };

        return View("~/Authentication/SignIn.cshtml", model);
    }
}
