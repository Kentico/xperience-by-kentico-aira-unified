using Microsoft.AspNetCore.Http;

namespace Kentico.Xperience.Aira.Assets;

/// <summary>
/// Service reponsible for handling files and accessing content item asset management.
/// </summary>
public interface IAiraAssetService
{
    /// <summary>
    /// Creates content item assets for a content type configured for mass asset upload.
    /// </summary>
    /// <param name="files">Files which will be added to the corresponding content item asset.</param>
    /// <param name="userId">The admin application user id.</param>
    /// <returns>Task</returns>
    Task HandleFileUpload(IFormFileCollection files, int userId);

    /// <summary>
    /// Checks whether the specified user has a role which contains the specified permission to the ACA. 
    /// </summary>
    /// <param name="permission">System permission name.</param>
    /// <param name="userId">The admin application user id.</param>
    /// <returns>A Task indicating whether the user has the specified permission.</returns>
    Task<bool> DoesUserHaveAiraCompanionAppPermission(string permission, int userId);
}
