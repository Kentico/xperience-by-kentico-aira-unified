using System.Text.Json.Serialization;

namespace Kentico.Xperience.Aira.Chat.Models;

public class AiraAIResponse
{
    [JsonPropertyName("summary")]
    public string Summary { get; set; } = string.Empty;

    [JsonPropertyName("suggested_questions")]
    public List<string> SuggestedQuestions { get; set; } = [];

    [JsonPropertyName("response")]
    public string Response { get; set; } = string.Empty;
}
