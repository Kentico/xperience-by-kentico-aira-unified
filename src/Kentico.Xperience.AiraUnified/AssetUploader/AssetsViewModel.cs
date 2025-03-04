namespace Kentico.Xperience.AiraUnified.AssetUploader.Models;

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

    /// <summary>
    /// Relative path of the endpoint which retrieves the navigation model <see cref="NavBar.NavBarViewModel"/>.
    /// </summary>
    public string NavigationUrl { get; set; } = string.Empty;

    /// <summary>
    /// Identifier of the smart upload page recognised by the navigation <see cref="AiraUnifiedController.Navigation(NavBar.NavBarRequestModel)"/> endpoint.
    /// </summary>
    public string NavigationPageIdentifier { get; set; } = string.Empty;

    /// <summary>
    /// Smart upload upload url.
    /// </summary>
    public string UploadUrl { get; set; } = string.Empty;
}
