namespace Kentico.Xperience.Aira.Chat.Models;

/// <summary>
/// View model for the service page displayed when the ai service is unavailable.
/// </summary>
public class ServicePageViewModel
{
    /// <summary>
    /// The chat unavailable icon url.
    /// </summary>
    public string ChatUnavailableIconUrl { get; set; } = string.Empty;

    /// <summary>
    /// The aira icon url.
    /// </summary>
    public string ChatAiraIconUrl { get; set; } = string.Empty;

    /// <summary>
    /// The message explaining that the chat service is unavailable.
    /// </summary>
    public string ChatUnavailableMainMessage { get; set; } = string.Empty;

    /// <summary>
    /// The message explaining to try again later.
    /// </summary>
    public string ChatUnavailableTryAgainMessage { get; set; } = string.Empty;
}
