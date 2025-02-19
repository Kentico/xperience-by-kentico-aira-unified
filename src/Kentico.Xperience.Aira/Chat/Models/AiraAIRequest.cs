using System.Text.Json.Serialization;

namespace Kentico.Xperience.Aira.Chat.Models;

internal class AiraAIRequest
{
    [JsonPropertyName("conversation_history")]
    public List<AiraChatMessageModel> ConversationHistory { get; set; } = [];

    [JsonPropertyName("chat_message")]
    public string ChatMessage { get; set; } = string.Empty;

    [JsonPropertyName("app_insights")]
    public Dictionary<string, string> AppInsights { get; set; } = [];
}
