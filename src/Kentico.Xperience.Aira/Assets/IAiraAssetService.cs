using Microsoft.AspNetCore.Http;

namespace Kentico.Xperience.Aira.Assets;

/// <summary>
/// Service reponsible for handling files and accessing content item asset management.
/// </summary>
public interface IAiraAssetService
{
    Task HandleFileUpload(IFormFileCollection files, int userId);
    Task<bool> DoesUserHaveAiraCompanionAppPermission(string permission, int userId);
}
