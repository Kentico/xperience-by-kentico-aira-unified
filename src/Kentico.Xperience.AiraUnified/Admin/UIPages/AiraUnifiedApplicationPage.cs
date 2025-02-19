using CMS.Membership;

using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.UIPages;

using Kentico.Xperience.AiraUnified.Admin.UIPages;

[assembly: UIApplication(
    identifier: AiraUnifiedApplicationPage.IDENTIFIER,
    type: typeof(AiraUnifiedApplicationPage),
    slug: "aira-unified",
    name: "AIRA Unified",
    category: BaseApplicationCategories.CONFIGURATION,
    icon: Icons.RectangleAInverted,
    templateName: TemplateNames.SECTION_LAYOUT
    )]

namespace Kentico.Xperience.AiraUnified.Admin.UIPages;

[UIPermission(SystemPermissions.VIEW)]
[UIPermission(SystemPermissions.CREATE)]
[UIPermission(SystemPermissions.UPDATE)]
[UIPermission(SystemPermissions.DELETE)]
internal class AiraUnifiedApplicationPage : ApplicationPage
{
    public const string IDENTIFIER = "Kentico.Xperience.Integrations.AiraUnified";
}
