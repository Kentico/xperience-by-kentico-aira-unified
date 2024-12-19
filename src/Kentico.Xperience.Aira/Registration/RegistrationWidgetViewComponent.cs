using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using CMS.DataEngine;
using CMS.Membership;
using Kentico.Membership;
using Kentico.Membership.Internal;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Aira.Registration;
using Kentico.Xperience.Aira.Membership;

[assembly: RegisterWidget(
    identifier: RegistrationWidgetViewComponent.IDENTIFIER,
    viewComponentType: typeof(RegistrationWidgetViewComponent),
    name: "Registration Widget",
    propertiesType: typeof(RegistrationWidgetProperties))]

namespace Kentico.Xperience.Aira.Registration;
public class RegistrationWidgetViewComponent(
        AdminUserManager adminUserManager,
        IUserInfoProvider userInfoProvider,
        UserManager<Member> userManager,
        IInfoProvider<RoleInfo> roleInfoProvider,
        IUserManagementService userManagementService
        ) : ViewComponent
{
    private readonly AdminUserManager adminUserManager = adminUserManager;
    private readonly IUserInfoProvider userInfoProvider = userInfoProvider;
    private readonly UserManager<Member> userManager = userManager;
    private readonly IInfoProvider<RoleInfo> roleInfoProvider = roleInfoProvider;
    private readonly IUserManagementService userManagementService = userManagementService;

    public const string IDENTIFIER = "Kentico.Xperience.Aira.RegistrationWidget";

    public IViewComponentResult Invoke(RegistrationWidgetProperties properties)
    {
        properties.Model ??= new RegistrationViewModel();
        return View("~/Registration/_RegistrationWidget.cshtml", properties);
    }
}
