using CMS.Base;
using CMS.Membership;
using Kentico.Membership.Internal;
using Kentico.Membership;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CMS.DataEngine;
using Kentico.Xperience.Aira.Admin;
using Microsoft.AspNetCore.Authorization;

namespace Kentico.Xperience.Aira.Registration;

[Route("[controller]/[action]")]
public class AiraRegistrationWidgetController(
    AdminUserManager adminUserManager,
    IUserInfoProvider userInfoProvider,
    UserManager<ApplicationUser> userManager,
    IInfoProvider<RoleInfo> roleInfoProvider,
    IUserManagementService userManagementService
) : Controller
{
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegistrationViewModel model)
    {
        var member = new ApplicationUser
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
            return Ok("Ok");
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
}
