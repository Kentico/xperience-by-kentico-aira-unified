using CMS;
using CMS.Core;

using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Aira.Admin;

[assembly: RegisterModule(typeof(AiraAdminModule))]

namespace Kentico.Xperience.Aira.Admin;

/// <summary>
/// Manages administration features and integration.
/// </summary>
internal class AiraAdminModule : AdminModule
{
    public AiraAdminModule() : base(nameof(AiraAdminModule)) { }

    protected override void OnInit(ModuleInitParameters parameters)
    {
        base.OnInit(parameters);

        RegisterClientModule("kentico", "xperience-integrations-aira");
    }
}
