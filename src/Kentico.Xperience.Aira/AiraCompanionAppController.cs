using CMS.DataEngine;

using HotChocolate.Authorization;
using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Aira.Admin.InfoModels;
using Kentico.Xperience.Aira.Chat.Models;
using Kentico.Xperience.Aira.NavBar;
using Htmx;

using Kentico.Membership;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using Microsoft.AspNetCore.Http;
using Kentico.Xperience.Aira.Authentication;
using Kentico.Xperience.Aira.Assets;

namespace Kentico.Xperience.Aira;

[ApiController]
[Route("[controller]/[action]")]
public sealed class AiraCompanionAppController(
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    IInfoProvider<AiraConfigurationItemInfo> airaConfigurationInfoProvider,
    IAiraAiraAssetService airaAssetService
) : Controller
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var configuration = await airaConfigurationInfoProvider.Get().GetEnumerableTypedResultAsync();

        var chatModel = new ChatViewModel
        {
            PathsModel = new AiraPathsModel
            {
                PathBase = configuration.First().AiraConfigurationItemAiraPathBase,
                ChatMessagePath = "chat/message",
            },
            NavBarViewModel = new NavBarViewModel
            {
                LogoImgRelativePath = AiraCompanionAppConstants.RelativeLogoUrl,
                TitleImagePath = AiraCompanionAppConstants.RelativeChatImgUrl,
                TitleText = AiraCompanionAppConstants.ChatTitle,
                ChatItem = new MenuItemModel
                {
                    Title = AiraCompanionAppConstants.ChatTitle,
                    ImagePath = AiraCompanionAppConstants.RelativeChatImgUrl,
                    Url = AiraCompanionAppConstants.ChatRelativeUrl
                },
                SmartUploadItem = new MenuItemModel
                {
                    Title = AiraCompanionAppConstants.SmartUploadTitle,
                    ImagePath = AiraCompanionAppConstants.RelativeSmartUploadUrl,
                    Url = AiraCompanionAppConstants.ChatRelativeUrl
                }
            }
        };

        return View("~/Chat/Chat.cshtml", chatModel);
    }

    [HttpPost]
    public async Task<IActionResult> PostChatMessage(IFormCollection request)
    {
        await airaAssetService.HandleFileUpload(request.Files);

        return Ok(new AiraChatMessageModel { Role = "ai", Text = "Ok" });
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
                return PartialView("~/Authentication/_SignIn.cshtml", model);
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

        async Task<ApplicationUser?> GetMember()
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
