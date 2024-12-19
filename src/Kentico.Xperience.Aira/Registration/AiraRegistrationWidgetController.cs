using CMS.Base;
using CMS.Membership;
using Kentico.Membership.Internal;
using Kentico.Membership;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CMS.DataEngine;
using Kentico.Xperience.Aira.Admin;
using Microsoft.AspNetCore.Authorization;
using CMS.EmailEngine;
using Kentico.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Content.Web.Mvc;
using Kentico.Xperience.Aira.Membership;

namespace Kentico.Xperience.Aira.Registration;

[Route("[controller]/[action]")]
public class AiraRegistrationWidgetController(
    AdminUserManager adminUserManager,
    IUserInfoProvider userInfoProvider,
    UserManager<Member> userManager,
    IInfoProvider<RoleInfo> roleInfoProvider,
    IUserManagementService userManagementService,
    SystemEmailOptions systemEmailOptions,
    IEmailService emailService
) : Controller
{
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegistrationViewModel model)
    {
        var member = new Member
        {
            UserName = model.UserName,
            Email = model.Email,
            Enabled = false
        };

        var adminUser = new AdminApplicationUser
        {
            UserName = model.UserName,
            Email = model.Email,
            UserEnabled = false,
            UserAdministrationAccess = true
        };

        var result = IdentityResult.Failed();
        try
        {
            var existingMember = await userManager.FindByEmailAsync(member.Email);

            if (existingMember is not null)
            {
                result = IdentityResult.Failed([new() { Code = "Failure", Description = "Member with this email already exists." }]);
            }
            else
            {
                result = await userManager.CreateAsync(member, model.Password);

                var airaRoleInfo = roleInfoProvider.Get().WhereEquals(nameof(RoleInfo.RoleName), AiraConstants.AiraRoleName).First();

                using (new CMSActionContext
                {
                    UseGlobalAdminContext = true
                })
                {
                    userManagementService.RegisterUser(new RegisterUserParameters(adminUser, new[]
                    {
                        airaRoleInfo.RoleID
                    }));
                }

                var adminConfirmationResult = await userManagementService.ConfirmUserAsync(
                    new ConfirmUserParameters(adminUser, adminUser!.UserName, model.Password, adminUserManager)
                );

                var adminUserInfo = userInfoProvider.Get().WhereEquals(nameof(UserInfo.Email), adminUser.Email).First();

                using (new CMSActionContext
                {
                    UseGlobalAdminContext = true,
                })
                {
                    adminUserInfo.UserEnabled = false;
                    adminUserInfo.Update();
                }
            }
        }
        catch (Exception ex)
        {
            result = IdentityResult.Failed([new() { Code = "Failure", Description = "Your registration was not successful." }]);
        }

        if (result.Succeeded)
        {
            await SendVerificationEmail(member);

            return Ok("<h3>Verify your email to finish signing up</h3><p>A confirmation email has been sent to your inbox. To complete your registration, please click the verification link in the email.</p>");
        }
        else
        {
            foreach (string error in result.Errors.Select(e => e.Description))
            {
                ModelState.AddModelError(string.Empty, error);
            }

            var properties = new RegistrationWidgetProperties
            {
                Model = model
            };

            return PartialView("~/Registration/_RegistrationWidget.cshtml", properties);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Confirm([FromQuery] string memberEmail, [FromQuery] string confirmToken)
    {
        string username = "johnDoe";
        if (!(HttpContext.Kentico().PageBuilder().EditMode || HttpContext.Kentico().Preview().Enabled))
        {
            IdentityResult confirmResult;

            var member = await userManager.FindByEmailAsync(memberEmail);

            if (member is null)
            {
                return View("~/Registration/_EmailConfirmation.csthml", new EmailConfirmationViewModel
                {
                    State = EmailConfirmationState.Failure_ConfirmationFailed,
                    Message = "Email Confirmation failed",
                    SendButtonText = "Send Again",
                    Username = ""
                });
            }

            if (member.Enabled)
            {
                return View("~/Registration/_EmailConfirmation.cshtml", new EmailConfirmationViewModel
                {
                    State = EmailConfirmationState.Success_AlreadyConfirmed,
                    Message = "Your email is already confirmed"
                });
            }

            try
            {
                confirmResult = await userManager.ConfirmEmailAsync(member, confirmToken);
            }
            catch
            {
                confirmResult = IdentityResult.Failed(new IdentityError() { Description = "User not found." });
            }

            if (confirmResult.Succeeded)
            {
                var adminUserInfo = userInfoProvider.Get().WhereEquals(nameof(UserInfo.Email), member.Email).First();

                using (new CMSActionContext
                {
                    UseGlobalAdminContext = true
                })
                {
                    adminUserInfo.UserEnabled = true;
                    adminUserInfo.Update();
                }

                return View("~/Registration/EmailConfirmation.cshtml", new EmailConfirmationViewModel
                {
                    State = EmailConfirmationState.Success_Confirmed,
                    Message = "<h3>Success!Your Email is Verified</h3><p>Your email has been successfully verified. Thank you! You can now enjoy full access to your account.</p>"
                });
            }

            if (member.UserName != null)
            {
                username = member.UserName;
            }
        }

        return View("~/Registration/_EmailConfirmation.csthml", new EmailConfirmationViewModel()
        {
            State = EmailConfirmationState.Failure_ConfirmationFailed,
            Message = "Email Confirmation failed",
            SendButtonText = "Send Again",
            Username = username
        });
    }

    [HttpPost]
    public async Task<IActionResult> ResendVerificationEmail([FromQuery] string username)
    {
        var member = await userManager.FindByNameAsync(username);

        if (member is not null)
        {
            await SendVerificationEmail(member);
        }

        return View("~/Registration/_VerifyEmail.cshtml");
    }

    private async Task SendVerificationEmail(Member member)
    {
        try
        {
            string confirmToken = await userManager.GenerateEmailConfirmationTokenAsync(member);
            string confirmationURL = Url.Action(nameof(Confirm), "AiraRegistrationWidget",
                new
                {
                    memberEmail = member.Email,
                    confirmToken
                },
                Request.Scheme) ?? "";

            await emailService.SendEmail(new EmailMessage()
            {
                From = $"test@{systemEmailOptions.SendingDomain}",
                Recipients = member.Email,
                Subject = $"Confirm your email here",
                Body = $@"Click this link to confirm your email: {confirmationURL}"
            });
        }
        catch (Exception ex)
        {
            string aa = "aa";
        }
    }
}
