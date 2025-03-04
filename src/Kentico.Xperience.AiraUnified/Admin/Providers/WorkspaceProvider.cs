using CMS.DataEngine;
using CMS.Workspaces;

using Kentico.Xperience.Admin.Base.FormAnnotations;

namespace Kentico.Xperience.AiraUnified.Admin.Providers;

internal class WorkspaceProvider : IDropDownOptionsProvider
{
    private readonly IInfoProvider<WorkspaceInfo> workspaceProvider;

    public WorkspaceProvider(IInfoProvider<WorkspaceInfo> workspaceProvider) => this.workspaceProvider = workspaceProvider;

    public async Task<IEnumerable<DropDownOptionItem>> GetOptionItems() =>
        (await workspaceProvider.Get()
        .GetEnumerableTypedResultAsync())
        .Select(x => new DropDownOptionItem
        {
            Value = x.WorkspaceName,
            Text = x.WorkspaceDisplayName
        });
}
