using CMS.DataEngine;

using HotChocolate.Authorization;

using Htmx;

using Kentico.Membership;
using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.Authentication.Internal;

using Kentico.Xperience.Aira.Admin.InfoModels;
using Kentico.Xperience.Aira.Chat.Models;
using Kentico.Xperience.Aira.Authentication;
using Kentico.Xperience.Aira.Assets;
using Kentico.Xperience.Aira.AssetUploader.Models;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using Kentico.Xperience.Aira.Admin;
using CMS.Membership;

namespace Kentico.Xperience.Aira;

[ApiController]
[Route("[controller]/[action]")]
public sealed class AiraCompanionAppController(
    AdminSignInManager signInManager,
    AdminUserManager userManager,
    IInfoProvider<AiraConfigurationItemInfo> airaConfigurationInfoProvider,
    IAiraAssetService airaAssetService,
    AiraUIService airaUIService
) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        string airaPathBase = await GetAiraPathBase();

        var user = await userManager.GetUserAsync(User);

        string signinRedirectUrl = GetRedirectUrl(AiraCompanionAppConstants.SigninRelativeUrl, airaPathBase);

        if (user is null)
        {
            return Redirect(signinRedirectUrl);
        }

        bool hasAiraViewPermission = await airaAssetService.DoesUserHaveAiraCompanionAppPermission(SystemPermissions.VIEW, user.UserID);

        if (!hasAiraViewPermission)
        {
            return Redirect(signinRedirectUrl);
        }

        var chatModel = new ChatViewModel
        {
            PathBase = airaPathBase,
            AIIconImagePath = $"/{AiraCompanionAppConstants.RCLUrlPrefix}/{AiraCompanionAppConstants.PictureHatImgPath}",
            NavBarViewModel = airaUIService.GetNavBarViewModel(AiraCompanionAppConstants.ChatRelativeUrl)
        };

        chatModel.InitialAiraMessage = chatModel.History.Count == 0
            ? AiraCompanionAppConstants.AiraChatInitialAIMessage
            : AiraCompanionAppConstants.AiraChatAIWelcomeBackMessage;

        return View("~/Chat/Chat.cshtml", chatModel);
    }

    [HttpPost]
    public async Task<IActionResult> PostChatMessage(IFormCollection request)
    {
        string airaPathBase = await GetAiraPathBase();

        var user = await userManager.GetUserAsync(User);

        string signinRedirectUrl = GetRedirectUrl(AiraCompanionAppConstants.SigninRelativeUrl, airaPathBase);

        if (user is null)
        {
            return Redirect(signinRedirectUrl);
        }

        bool hasAiraViewPermission = await airaAssetService.DoesUserHaveAiraCompanionAppPermission(SystemPermissions.VIEW, user.UserID);

        if (!hasAiraViewPermission)
        {
            return Redirect(signinRedirectUrl);
        }

        return Ok(new AiraChatMessage { Role = AiraCompanionAppConstants.AiraChatRoleName, Message = "Ok" });
    }

    [HttpPost]
    public async Task<IActionResult> PostImages(IFormCollection request)
    {
        string airaPathBase = await GetAiraPathBase();

        var user = await userManager.GetUserAsync(User);

        string signinRedirectUrl = GetRedirectUrl(AiraCompanionAppConstants.SigninRelativeUrl, airaPathBase);

        if (user is null)
        {
            return Redirect(signinRedirectUrl);
        }

        bool hasAiraViewPermission = await airaAssetService.DoesUserHaveAiraCompanionAppPermission(SystemPermissions.CREATE, user.UserID);

        if (!hasAiraViewPermission)
        {
            return Redirect(signinRedirectUrl);
        }

        await airaAssetService.HandleFileUpload(request.Files, user.UserID);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> Assets()
    {
        string airaPathBase = await GetAiraPathBase();

        var user = await userManager.GetUserAsync(User);

        string signinRedirectUrl = GetRedirectUrl(AiraCompanionAppConstants.SigninRelativeUrl, airaPathBase);

        if (user is null)
        {
            return Redirect(signinRedirectUrl);
        }

        bool hasAiraCreatePermission = await airaAssetService.DoesUserHaveAiraCompanionAppPermission(SystemPermissions.CREATE, user.UserID);

        if (!hasAiraCreatePermission)
        {
            return Redirect(signinRedirectUrl);
        }

        var model = new AssetsViewModel
        {
            NavBarViewModel = airaUIService.GetNavBarViewModel(AiraCompanionAppConstants.SmartUploadRelativeUrl),
            PathBase = airaPathBase
        };

        return View("~/AssetUploader/Assets.cshtml", model);
    }

    [HttpGet($"/{AiraCompanionAppConstants.RCLUrlPrefix}/manifest.json")]
    [Produces("application/json")]
    public async Task<IActionResult> GetPwaManifest()
    {
        var configuration = (await airaConfigurationInfoProvider.Get().GetEnumerableTypedResultAsync()).First();

        string libraryBasePath = '/' + AiraCompanionAppConstants.RCLUrlPrefix;

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
        var configuration = await airaConfigurationInfoProvider.Get().GetEnumerableTypedResultAsync();

        var configurationItem = configuration.SingleOrDefault() ?? throw new InvalidOperationException("Aira is not configured");

        return configurationItem.AiraConfigurationItemAiraPathBase;
    }

    private string GetRedirectUrl(string relativeUrl, string airaPathBase)
    {
        string baseUrl = $"{Request.Scheme}://{Request.Host}";

        return $"{baseUrl}{airaPathBase}/{relativeUrl}";
    }

    [HttpGet]
    [AllowAnonymous]
    public Task<IActionResult> Signin()
        => Task.FromResult((IActionResult)View("~/Authentication/SignIn.cshtml"));

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SignIn([FromForm] SignInViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return PartialView("~/Authentication/_SignIn.cshtml", model);
        }

        SignInResult signInResult;
        try
        {
            var user = await AdminApplicationUser();

            if (user is null)
            {
                signInResult = SignInResult.Failed;
            }
            else
            {
                signInResult = await signInManager.PasswordSignInAsync(user.UserName!, model.Password, isPersistent: true, lockoutOnFailure: false);
                if (signInResult.Succeeded)
                {
                    var claimsPrincipal = await signInManager.CreateUserPrincipalAsync(user);

                    await signInManager.SignInWithClaimsAsync(user, new AuthenticationProperties
                    {
                        IsPersistent = true
                    }, claimsPrincipal.Claims);

                    HttpContext.User = claimsPrincipal;
                }
            }
        }
        catch
        {
            signInResult = SignInResult.Failed;
        }

        if (!signInResult.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Your sign-in attempt was not successful. Please try again.");

            return PartialView("~/Authentication/_SignIn.cshtml", model);
        }

        var configuration = await airaConfigurationInfoProvider.Get().GetEnumerableTypedResultAsync();
        string airaPathBase = configuration.First().AiraConfigurationItemAiraPathBase;

        string baseUrl = $"{Request.Scheme}://{Request.Host}";
        string redirectUrl = $"{baseUrl}{airaPathBase}/{AiraCompanionAppConstants.ChatRelativeUrl}";

        Response.Htmx(h => h.Redirect(redirectUrl));

        return Request.IsHtmx()
        ? Ok()
        : Redirect(redirectUrl);

        async Task<AdminApplicationUser?> AdminApplicationUser()
        {
            var user = await userManager.FindByNameAsync(model.UserNameOrEmail);

            if (user is not null)
            {
                return user;
            }

            return await userManager.FindByEmailAsync(model.UserNameOrEmail);
        }
    }
}
