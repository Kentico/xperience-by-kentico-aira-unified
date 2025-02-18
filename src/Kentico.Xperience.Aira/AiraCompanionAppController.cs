﻿using System.Text;

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
using Kentico.Xperience.Aira.Insights;
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
    private readonly IAiraInsightsService airaInsightsService;
    private readonly IAiraChatService airaChatService;
    private readonly IAiraAssetService airaAssetService;
    private readonly INavBarService airaUIService;

    public AiraCompanionAppController(
        AdminUserManager adminUserManager,
        IAiraConfigurationService airaConfigurationService,
        IAiraInsightsService airaInsightsService,
        IAiraAssetService airaAssetService,
        INavBarService airaUIService,
        IAiraChatService airaChatService)
    {
        this.adminUserManager = adminUserManager;
        this.airaConfigurationService = airaConfigurationService;
        this.airaInsightsService = airaInsightsService;
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
            NavBarViewModel = await airaUIService.GetNavBarViewModel(AiraCompanionAppConstants.ChatRelativeUrl),
            RemovePromptUrl = AiraCompanionAppConstants.RemoveUsedPromptGroupRelativeUrl
        };

        if (chatModel.History.Count == 0)
        {
            chatModel.History = [
                new AiraChatMessage
                {
                    Message = Resource.InitialAiraMessage1,
                    Role = AiraCompanionAppConstants.AiraChatRoleName
                },
                new AiraChatMessage
                {
                    Message = Resource.InitialAiraMessage2,
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

#pragma warning disable IDE0079 // Kept for development. This will be restored in subsequent versions.
#pragma warning disable S6932 // Kept for development. This will be restored in subsequent versions.
        if (request.TryGetValue("message", out var messages))
        {
            message = messages.ToString().Replace("\"", "");
        }
#pragma warning restore S6932 //
#pragma warning restore IDE0079 //

        AiraChatMessage response;

        airaChatService.SaveMessage(message ?? "", user.UserID, AiraCompanionAppConstants.UserChatRoleName);

        if (message == "Prompts")
        {
            response = await airaChatService.GenerateAiraPrompts(user.UserID);
            response.Message = "OK";
        }
        else
        {
            try
            {
                switch (message)
                {
                    case "Reusable Drafts":
                        var reusableDraftResult = await airaInsightsService.GetContentInsights(ContentType.Reusable, user, "Draft");
                        response = BuildMessage(reusableDraftResult);
                        break;
                    case "Website Scheduled":
                        var websiteScheduledResult = await airaInsightsService.GetContentInsights(ContentType.Website, user, "Scheduled");
                        response = BuildMessage(websiteScheduledResult);
                        break;
                    case "Emails":
                        var emailsResult = await airaInsightsService.GetEmailInsights(user);
                        response = BuildMessage(emailsResult);
                        break;
                    case "Contact Groups":
                        var contactGroupsResult = airaInsightsService.GetContactGroupInsights(["Females", "Males"]);
                        response = BuildMessage(contactGroupsResult);
                        break;
                    default:
                        response = new AiraChatMessage
                        {
                            Role = AiraCompanionAppConstants.AiraChatRoleName,
                            Message = "Ok",
                            QuickPrompts = message == "Prompts" ?
                                ["Reusable Drafts", "Website Scheduled", "Emails", "Contact Groups"] : []
                        };
                        break;
                }
            }
            catch (Exception ex)
            {
                response = new AiraChatMessage
                {
                    Role = AiraCompanionAppConstants.AiraChatRoleName,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        airaChatService.SaveMessage(response.Message ?? "", user.UserID, AiraCompanionAppConstants.AiraChatRoleName);

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

        var uploadSuccessful = await airaAssetService.HandleFileUpload(request.Files, user.UserID);

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

    private string GetRedirectUrl(string relativeUrl, string airaPathBase)
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        return $"{baseUrl}{airaPathBase}/{relativeUrl}";
    }

    private AiraChatMessage BuildMessage(ContentInsightsModel content)
    {
        var message = new StringBuilder();

        foreach (var item in content.Items)
        {
            if (message.Length > 0)
            {
                message.Append(", ");
            }

            message.Append(item.DisplayName);
        }

        return new AiraChatMessage
        {
            Role = AiraCompanionAppConstants.AiraChatRoleName,
            Message = message.Length == 0 ? "No Content" : message.ToString()
        };
    }

    private AiraChatMessage BuildMessage(EmailInsightsModel emails)
    {
        var message = new StringBuilder();

        foreach (var item in emails.Emails)
        {
            if (message.Length > 0)
            {
                message.Append(", ");
            }

            message.AppendFormat("{0} ({1}, {2})", item.EmailName, item.ContentTypeName, item.ChannelName);
        }

        message.Append(" - ");
        message.AppendFormat("Sent: {0}, ", emails.EmailsSent);
        message.AppendFormat("Delivered: {0}, ", emails.EmailsDelivered);
        message.AppendFormat("Clicked: {0}, ", emails.LinksClicked);
        message.AppendFormat("Opened: {0}, ", emails.EmailsOpened);
        message.AppendFormat("Unsubscribed: {0}, ", emails.UnsubscribeRate);
        message.AppendFormat("Spam Reports: {0}", emails.SpamReports);

        message.Insert(0, "Email: ");

        return new AiraChatMessage
        {
            Role = AiraCompanionAppConstants.AiraChatRoleName,
            Message = message.Length == 0 ? "No Emails" : message.ToString()
        };
    }

    private AiraChatMessage BuildMessage(ContactGroupsInsightsModel contactGroups)
    {
        var message = new StringBuilder();

        foreach (var contactGroup in contactGroups.Groups)
        {
            var percentage = contactGroup.Count * (100M / contactGroups.AllCount);
            if (message.Length > 0)
            {
                message.Append(", ");
            }

            message.Append(contactGroup.Name);
            message.AppendFormat(" (Count: {0}, Percentage {1}%)", contactGroup.Count, Math.Round(percentage, 2));
        }

        message.Append(" - ");
        message.AppendFormat("Contacts: {0}", contactGroups.AllCount);

        return new AiraChatMessage
        {
            Role = AiraCompanionAppConstants.AiraChatRoleName,
            Message = message.Length == 0 ? "No Contact Groups" : message.ToString()
        };
    }
}
