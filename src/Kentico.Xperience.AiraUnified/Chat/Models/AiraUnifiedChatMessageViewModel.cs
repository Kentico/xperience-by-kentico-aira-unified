namespace Kentico.Xperience.AiraUnified.Chat.Models;

/// <summary>
/// Model for the Aira Unified chat message.
/// </summary>
<<<<<<<< HEAD:src/Kentico.Xperience.AiraUnified/Chat/Models/AiraUnifiedChatMessageViewModel.cs
public class AiraUnifiedChatMessageViewModel
========
public class AiraChatMessageViewModel
>>>>>>>> main:src/Kentico.Xperience.AiraUnified/Chat/Models/AiraChatMessageViewModel.cs
{
    /// <summary>
    /// The text message.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Url of an asset which can be displayed in the chat.
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Role of the author of the message in the chat.
    /// </summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// Suggested prompt texts.
    /// </summary>
    public IEnumerable<string> QuickPrompts { get; set; } = [];

    /// <summary>
    /// Id of the prompt group generated and shown together in the message.
    /// </summary>
    public string QuickPromptsGroupId { get; set; } = string.Empty;

    /// <summary>
    /// Chat message creation time.
    /// </summary>
    public DateTime CreatedWhen { get; set; }

    /// <summary>
    /// True if the ai service is unavailable.
    /// </summary>
    public bool ServiceUnavailable { get; set; } = false;
}
