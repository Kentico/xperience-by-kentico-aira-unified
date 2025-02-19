namespace Kentico.Xperience.Aira.Authentication;

public class SignInViewModel
{
    /// <summary>
    /// Url of the chat page relative to the ACA base url.
    /// </summary>
    public string ChatRelativeUrl { get; set; } = string.Empty;

    /// <summary>
    /// ACA base url.
    /// </summary>
    public string PathBase { get; set; } = string.Empty;

    /// <summary>
    /// Path of the logo displayed in the PWA.
    /// </summary>
    public string LogoImageRelativePath { get; set; } = string.Empty;
}
