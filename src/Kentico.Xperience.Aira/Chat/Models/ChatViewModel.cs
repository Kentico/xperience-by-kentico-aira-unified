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

    public ServicePageViewModel ServicePageViewModel { get; set; } = new ServicePageViewModel();

    /// <summary>
    /// Count of messages in User's chat history.
    /// </summary>
    public int HistoryMessageCount { get; set; }

    /// <summary>
    /// User's chat history messages.
    /// </summary>
    public List<AiraChatMessageViewModel> History { get; set; } = [];

    /// <summary>
    /// Path base for the Chat page.
    /// </summary>
    public string PathBase { get; set; } = string.Empty;

    /// <summary>
    /// Relative path to the AI icon displayed in the chat as the avatar of the AI.
    /// </summary>
    public string AIIconImagePath { get; set; } = string.Empty;

    /// <summary>
    /// Relative path of the endpoint responsible for removing used suggested prompt group.
    /// </summary>
    public string RemovePromptUrl { get; set; } = string.Empty;
}
