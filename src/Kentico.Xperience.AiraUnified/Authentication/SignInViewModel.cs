namespace Kentico.Xperience.AiraUnified.Authentication;

public class SignInViewModel
{
    /// <summary>
    /// Url of the chat page relative to the Aira unified base url.
    /// </summary>
    public string ChatUrl { get; set; } = string.Empty;

    /// <summary>
    /// Aira unified base url.
    /// </summary>
    public string PathBase { get; set; } = string.Empty;

    /// <summary>
    /// Path of the logo displayed in the PWA.
    /// </summary>
    public string LogoImageRelativePath { get; set; } = string.Empty;

    /// <summary>
    /// Error message displayed if the user misses a permission.
    /// </summary>
    public string? ErrorMessage { get; set; }
}
