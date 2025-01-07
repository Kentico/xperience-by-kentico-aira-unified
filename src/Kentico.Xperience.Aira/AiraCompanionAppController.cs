using CMS.DataEngine;

using HotChocolate.Authorization;
using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Aira.Admin.InfoModels;
using Kentico.Xperience.Aira.Chat.Models;
using Htmx;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using Microsoft.AspNetCore.Http;
using Kentico.Xperience.Aira.Authentication;
using Kentico.Xperience.Aira.Assets;
using Kentico.Xperience.Aira.Registration;
using Kentico.Xperience.Aira.Membership;
using Kentico.Xperience.Aira.Services;
using Kentico.Xperience.Aira.AssetUploader.Models;

namespace Kentico.Xperience.Aira;

[ApiController]
[Route("[controller]/[action]")]
public sealed class AiraCompanionAppController(
    SignInManager<Member> signInManager,
    UserManager<Member> userManager,
    IInfoProvider<AiraConfigurationItemInfo> airaConfigurationInfoProvider,
    IAiraAiraAssetService airaAssetService,
    AiraUIService airaUIService
) : Controller
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var configuration = await airaConfigurationInfoProvider.Get().GetEnumerableTypedResultAsync();

        var assets = await airaAssetService.GetUsersUploadedAssetUrls(53);

        var chatModel = new ChatViewModel
        {
            PathBase = configuration.First().AiraConfigurationItemAiraPathBase,
            NavBarViewModel = airaUIService.GetNavBarViewModel("chat"),
            History = assets.Select(x => new AiraChatMessage
            {
                Url = x,
                Role = "user"
            }).ToList(),
        };

        chatModel.InitialAiraMessage = chatModel.History.Count == 0
            ? "This is initial Aira message"
            : "What can I help you with ?";

        return View("~/Chat/Chat.cshtml", chatModel);
    }

    [HttpPost]
    public async Task<IActionResult> PostChatMessage(IFormCollection request)
    {
        await airaAssetService.HandleFileUpload(request.Files, 53);

        return Ok(new AiraChatMessage { Role = "ai", Message = "Ok" });
    }

    [HttpPost]
    public async Task<IActionResult> PostImages(IFormCollection request)
    {
        await airaAssetService.HandleFileUpload(request.Files, 53);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> Assets()
    {
        //var member = await userManager.GetUserAsync(User);

        //if (member is null)
        //{
        //    return Redirect($"{Request.PathBase}/aira/signin");
        //}

        var configuration = await airaConfigurationInfoProvider.Get().GetEnumerableTypedResultAsync();

        var model = new AssetsViewModel
        {
            NavBarViewModel = airaUIService.GetNavBarViewModel("smart-upload"),
            PathBase = configuration.First().AiraConfigurationItemAiraPathBase
        };

        return View("~/AssetUploader/Assets.cshtml", model);
    }

    [HttpGet("/_content/Kentico.Xperience.Aira/manifest.json")]
    [Produces("application/json")]
    public async Task<IActionResult> GetPwaManifest()
    {
        var configuration = (await airaConfigurationInfoProvider.Get().GetEnumerableTypedResultAsync()).First();

        string libraryBasePath = "/_content/Kentico.Xperience.Aira";

        var manifest = new
        {
            name = "Aira",
            short_name = "Aira",
            start_url = $"{configuration.AiraConfigurationItemAiraPathBase}/chat",
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
            var member = await GetMember();

            if (member is null)
            {
                signInResult = SignInResult.Failed;
            }
            else if (!member.Enabled)
            {
                var emailConfirmationModel = new EmailConfirmationViewModel()
                {
                    State = EmailConfirmationState.Failure_NotYetConfirmed,
                    Message = "This Email Is Not Verified Yet",
                    SendButtonText = "Send Verification Email",
                    Username = member.UserName!
                };

                return PartialView("~/Registration/_EmailConfirmation.cshtml", emailConfirmationModel);
            }
            else
            {
                signInResult = await signInManager.PasswordSignInAsync(member.UserName!, model.Password, isPersistent: true, lockoutOnFailure: false);
            }
        }
        catch (Exception ex)
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
        string redirectUrl = $"{baseUrl}{airaPathBase}/chat";

        Response.Htmx(h => h.Redirect(redirectUrl));

        return Request.IsHtmx()
        ? Ok()
        : Redirect(redirectUrl);

        async Task<Member?> GetMember()
        {
            var member = await userManager.FindByNameAsync(model.UserNameOrEmail);

            if (member is not null)
            {
                return member;
            }

            return await userManager.FindByEmailAsync(model.UserNameOrEmail);
        }
    }
}
