using Kentico.Xperience.Aira.NavBar;

namespace Kentico.Xperience.Aira.Chat.Models;

/// <summary>
/// View model for the Chat page.
/// </summary>
public class ChatViewModel
{
    /// <summary>
    /// View model for the navigation.
    /// </summary>
    public NavBarViewModel NavBarViewModel { get; set; } = new NavBarViewModel();

    /// <summary>
    /// Count of messages in User's chat history.
    /// </summary>
    public int HistoryMessageCount { get; set; }

    /// <summary>
    /// User's chat history messages.
    /// </summary>
    public List<AiraChatMessage> History { get; set; } = [];

    /// <summary>
    /// Path base for the Chat page.
    /// </summary>
    public string PathBase { get; set; } = string.Empty;
    public string InitialAiraMessage { get; set; } = string.Empty;
    public string AIIconImagePath { get; set; } = string.Empty;
}
