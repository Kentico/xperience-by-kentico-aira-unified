namespace Kentico.Xperience.Aira.Chat.Models;

public class AiraChatMessage
{
    public string? Message { get; set; }
    public string? Url { get; set; }
    public string Role { get; set; } = string.Empty;
}
