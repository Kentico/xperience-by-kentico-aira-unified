namespace Kentico.Xperience.AiraUnified.Chat.Models;

/// <summary>
/// State of the chat.
/// </summary>
public enum ChatStateType
{
    /// <summary>
    /// First time opening a new chat thread.
    /// </summary>
    initial,

    /// <summary>
    /// Reopening a chat thread.
    /// </summary>
    returning,

    /// <summary>
    /// Standard message.
    /// </summary>
    ongoing
}
