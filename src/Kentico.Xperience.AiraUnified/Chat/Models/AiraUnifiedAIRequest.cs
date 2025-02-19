using System.Text.Json.Serialization;

namespace Kentico.Xperience.AiraUnified.Chat.Models;

internal class AiraUnifiedAIRequest
{
    [JsonPropertyName("conversation_history")]
    public List<AiraUnifiedChatMessageModel> ConversationHistory { get; set; } = [];

    [JsonPropertyName("chat_message")]
    public string ChatMessage { get; set; } = string.Empty;

    [JsonPropertyName("app_insights")]
    public Dictionary<string, string> AppInsights { get; set; } = [];
}
