namespace Kentico.Xperience.AiraUnified.NavBar;

/// <summary>
/// Service managing the Navigation.
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Generates the Navigation View Model.
    /// </summary>
    /// <param name="activePage">The page active in the PWA.</param>
    /// <param name="baseUrl">Application base url.</param>
    /// <returns><see cref="NavBarViewModel"/>Navigation View Model.</returns>
    Task<NavBarViewModel> GetNavBarViewModel(string activePage, string baseUrl);

    /// <summary>
    /// Safely builds an absolute aira URI by combining a base URL with a relative path validating the uri can be created from the aira path base.
    /// </summary>
    /// <param name="baseUrl">Application base url.</param>
    /// <param name="airaPathBase">Aira path base specified in the admin UI.</param>
    /// <param name="relativePathParts">Another relative path parts. (Not starting with "/")</param>
    /// <returns>The resulting <see cref="Uri"/> or null if the uri can not be created.</returns>
    Uri? BuildUriOrNull(string baseUrl, string airaPathBase, params string[] relativePathParts);
}
