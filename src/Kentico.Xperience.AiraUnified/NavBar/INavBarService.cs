using Kentico.Content.Web.Mvc;

namespace Kentico.Xperience.AiraUnified.NavBar;

/// <summary>
/// Service managing the Navigation.
/// </summary>
public interface INavBarService
{
    /// <summary>
    /// Generates the Navigation View Model.
    /// </summary>
    /// <param name="activePage">The page active in the PWA.</param>
    /// <returns><see cref="NavBarViewModel"/>Navigation View Model.</returns>
    Task<NavBarViewModel> GetNavBarViewModel(string activePage);

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
    /// <returns></returns>
    string GetSanitizedImageUrl(string? configuredUrl, string defaultUrl, string imagePurpose);
}
