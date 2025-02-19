namespace Kentico.Xperience.AiraUnified.Chat.Models;

/// <summary>
/// The options used to setup this integration stored in the appsettings.json file.
/// </summary>
public sealed class AiraUnifiedOptions
{
    /// <summary>
    /// The aira unified service subscription key.
    /// </summary>
    public string AiraUnifiedApiSubscriptionKey { get; set; } = string.Empty;
}
