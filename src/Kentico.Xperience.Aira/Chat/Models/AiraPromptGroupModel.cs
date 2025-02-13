namespace Kentico.Xperience.Aira.Chat.Models;

public class AiraPromptGroupModel
{
    public int QuickPromptsGroupId { get; set; }
    public IEnumerable<string> QuickPrompts { get; set; } = [];
}
