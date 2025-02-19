using Kentico.Xperience.Aira.NavBar;

namespace Kentico.Xperience.Aira.AssetUploader.Models;

/// <summary>
/// View model for the Smart upload page.
/// </summary>
public class AssetsViewModel
{
    /// <summary>
    /// Path base for the Smart upload page.
    /// </summary>
    public string PathBase { get; set; } = string.Empty;

    /// <summary>
    /// View model for the navigation.
    /// </summary>
    public NavBarViewModel NavBarViewModel { get; set; } = new NavBarViewModel();

    /// <summary>
    /// Relative url to access the allowed file extensions.
    /// </summary>
    public string AllowedFileExtensionsUrl { get; set; } = string.Empty;

    /// <summary>
    /// The button triggering the file selection.
    /// </summary>
    public string SelectFilesButton { get; set; } = string.Empty;

    /// <summary>
    /// The message displayed on successful asset upload.
    /// </summary>
    public string FilesUploadedMessage { get; set; } = string.Empty;
}
