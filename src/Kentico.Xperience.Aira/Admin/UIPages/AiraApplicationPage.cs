using CMS.Membership;

using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.UIPages;

using Kentico.Xperience.Aira.Admin.UIPages;

[assembly: UIApplication(
    identifier: AiraApplicationPage.IDENTIFIER,
    type: typeof(AiraApplicationPage),
    slug: "aira-companion-app",
    name: "AIRA Unified",
    category: BaseApplicationCategories.CONFIGURATION,
    icon: Icons.RectangleAInverted,
    templateName: TemplateNames.SECTION_LAYOUT
    )]

namespace Kentico.Xperience.Aira.Admin.UIPages;

[UIPermission(SystemPermissions.VIEW)]
[UIPermission(SystemPermissions.CREATE)]
[UIPermission(SystemPermissions.UPDATE)]
[UIPermission(SystemPermissions.DELETE)]
internal class AiraApplicationPage : ApplicationPage
{
    public const string IDENTIFIER = "Kentico.Xperiencec.Integrations.Aira";
}
