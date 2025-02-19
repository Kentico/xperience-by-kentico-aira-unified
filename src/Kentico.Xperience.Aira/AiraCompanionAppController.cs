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
    private readonly INavBarService airaUIService;

    public AiraCompanionAppController(
        AdminUserManager adminUserManager,
        IAiraConfigurationService airaConfigurationService,
        IAiraAssetService airaAssetService,
        INavBarService airaUIService,
        IAiraChatService airaChatService)
    {
        this.adminUserManager = adminUserManager;
        this.airaConfigurationService = airaConfigurationService;
        this.airaAssetService = airaAssetService;
        this.airaChatService = airaChatService;
        this.airaUIService = airaUIService;
    }

    /// <summary>
    /// Endpoint exposing access to the Chat page.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await adminUserManager.GetUserAsync(User);
        var airaPathBase = await GetAiraPathBase();

        var chatModel = new ChatViewModel
        {
            PathBase = airaPathBase,
            // User can not be null, because he is already checked in the AiraEndpointDataSource middleware
            History = await airaChatService.GetUserChatHistory(user!.UserID),
            AIIconImagePath = $"/{AiraCompanionAppConstants.RCLUrlPrefix}/{AiraCompanionAppConstants.PictureStarImgPath}",
            NavBarViewModel = await airaUIService.GetNavBarViewModel(AiraCompanionAppConstants.ChatRelativeUrl),
            RemovePromptUrl = AiraCompanionAppConstants.RemoveUsedPromptGroupRelativeUrl
        };

        if (chatModel.History.Count == 0)
        {
            chatModel.History = [
                new AiraChatMessage
                {
                    Message = Resource.InitialAiraMessageIntroduction,
                    Role = AiraCompanionAppConstants.AiraChatRoleName
                },
                new AiraChatMessage
                {
                    Message = Resource.InitialAiraMessagePromptExplanation,
                    Role = AiraCompanionAppConstants.AiraChatRoleName
                }
            ];
        }
        else
        {
            chatModel.History.Add(new AiraChatMessage
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
        var user = await adminUserManager.GetUserAsync(User);

        string? message = null;

#pragma warning disable IDE0079 // Kept for development. This will be restored in subsequent versions.
#pragma warning disable S6932 // Kept for development. This will be restored in subsequent versions.
        if (request.TryGetValue("message", out var messages))
        {
            message = messages.ToString().Replace("\"", "");
        }
#pragma warning restore S6932 //
#pragma warning restore IDE0079 //

        AiraChatMessage response;

        // User can not be null, because he is already checked in the AiraEndpointDataSource middleware
        airaChatService.SaveMessage(message ?? "", user!.UserID, AiraCompanionAppConstants.UserChatRoleName);

        response = new AiraChatMessage
        {
            Role = AiraCompanionAppConstants.AiraChatRoleName,
            Message = "Ok"
        };

        // User can not be null, because he is already checked in the AiraEndpointDataSource middleware
        airaChatService.SaveMessage(response.Message ?? "", user!.UserID, AiraCompanionAppConstants.AiraChatRoleName);

        return Ok(response);
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
        var user = await adminUserManager.GetUserAsync(User);

        // User can not be null, because he is already checked in the AiraEndpointDataSource middleware
        var uploadSuccessful = await airaAssetService.HandleFileUpload(request.Files, user!.UserID);

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
        var airaPathBase = await GetAiraPathBase();

        var model = new AssetsViewModel
        {
            NavBarViewModel = await airaUIService.GetNavBarViewModel(AiraCompanionAppConstants.SmartUploadRelativeUrl),
            PathBase = airaPathBase,
            AllowedFileExtensionsUrl = $"{AiraCompanionAppConstants.SmartUploadRelativeUrl}/{AiraCompanionAppConstants.SmartUploadAllowedFileExtensionsUrl}"
        };

        return View("~/AssetUploader/Assets.cshtml", model);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllowedFileExtensions()
    {
        var allowedExtensions = await airaAssetService.GetAllowedFileExtensions();

        return Ok(allowedExtensions);
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
        var airaPathBase = await GetAiraPathBase();

        var model = new SignInViewModel
        {
            PathBase = airaPathBase,
            ChatRelativeUrl = AiraCompanionAppConstants.ChatRelativeUrl
        };

        return View("~/Authentication/SignIn.cshtml", model);
    }

    private async Task<string> GetAiraPathBase()
    {
        var configuration = await airaConfigurationService.GetAiraConfiguration();

        return configuration.AiraConfigurationItemAiraPathBase;
    }
}
