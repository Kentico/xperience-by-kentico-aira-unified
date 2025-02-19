using System.Text.Json.Serialization;

namespace Kentico.Xperience.Aira.Chat.Models;

/// <summary>
/// The ai response model.
/// </summary>
public class AiraAIResponse
{
    /// <summary>
    /// The chat summary retrieved from the ai response according to the user's conversation history.
    /// </summary>
    [JsonPropertyName("summary")]
    public string Summary { get; set; } = string.Empty;

    /// <summary>
    /// The suggested prompts.
    /// </summary>
    [JsonPropertyName("suggested_questions")]
    public List<string> SuggestedQuestions { get; set; } = [];

    /// <summary>
    /// The text response of the ai endpoint.
    /// </summary>
    [JsonPropertyName("response")]
    public string Response { get; set; } = string.Empty;
}
