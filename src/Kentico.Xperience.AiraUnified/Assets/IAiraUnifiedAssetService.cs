using Kentico.Content.Web.Mvc;
using Kentico.Xperience.AiraUnified.Admin.InfoModels;

using Microsoft.AspNetCore.Http;

namespace Kentico.Xperience.AiraUnified.Assets;

/// <summary>
/// Service responsible for handling files and accessing content item asset management.
/// </summary>
public interface IAiraUnifiedAssetService
{
    /// <summary>
    /// Creates content item assets for a content type configured for mass asset upload.
    /// </summary>
    /// <param name="files">Files which will be added to the corresponding content item asset.</param>
    /// <param name="userId">The admin application user id.</param>
    Task<bool> HandleFileUpload(IFormFileCollection files, int userId);

    /// <summary>
    /// Checks whether the specified user has a role which contains the specified permission to the Aira unified. 
    /// </summary>
    /// <param name="permission">System permission name.</param>
    /// <param name="userId">The admin application user id.</param>
    /// <returns>A Task indicating whether the user has the specified permission.</returns>
    Task<bool> DoesUserHaveAiraUnifiedPermission(string permission, int userId);

    /// <summary>
    /// Retrieves the file extensions which are configured for mass asset upload.
    /// </summary>
    /// <returns>Allowed file extensions.</returns>
    Task<string> GetAllowedFileExtensions();

    /// <summary>
    /// Retrieves url for a file displayed in the PWA.
    /// </summary>
    /// <param name="identifier">Identifier of the file.</param>
    /// <returns><see cref="IMediaFileUrl"/>The file reference.</returns>
    IMediaFileUrl? GetMediaFileUrl(string identifier);

    /// <summary>
    /// Retrieves url of an image with configured url or logs warning if it doesn't exist.
    /// </summary>
    /// <param name="configuredUrl"></param>
    /// <param name="defaultUrl"></param>
    /// <param name="imagePurpose"></param>
    string GetSanitizedImageUrl(string? configuredUrl, string defaultUrl, string imagePurpose);

    /// <summary>
    /// Retrieves url of the aira unified logo specified in the configuration.
    /// </summary>
    /// <param name="configuration"></param>
    string GetSanitizedLogoUrl(AiraUnifiedConfigurationItemInfo configuration);

    /// <summary>
    /// Retrieves url of the aira unified logo specified in the configuration.
    /// </summary>
    Task<string> GetSanitizedLogoUrl();
}
