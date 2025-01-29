﻿namespace Kentico.Xperience.Aira.Chat.Models;

/// <summary>
/// Model for the Aira chat message.
/// </summary>
public class AiraChatMessage
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
    public IEnumerable<string> QuickPrompts { get; set; } = [];
    public string QuickPromptsGroupId { get; set; } = string.Empty;
    public DateTime CreatedWhen { get; set; }
}
