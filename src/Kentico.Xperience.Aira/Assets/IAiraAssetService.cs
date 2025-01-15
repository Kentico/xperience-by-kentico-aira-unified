using Microsoft.AspNetCore.Http;

namespace Kentico.Xperience.Aira.Assets;

public interface IAiraAssetService
{
    Task HandleFileUpload(IFormFileCollection files, int userId);
    Task<bool> DoesUserHaveAiraCompanionAppPermission(string permission, int userId);
}
