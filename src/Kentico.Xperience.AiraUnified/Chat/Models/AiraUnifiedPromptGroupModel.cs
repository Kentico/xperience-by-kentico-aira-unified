namespace Kentico.Xperience.AiraUnified.Chat.Models;

public class AiraUnifiedPromptGroupModel
{
    public int QuickPromptsGroupId { get; set; }
    public IEnumerable<string> QuickPrompts { get; set; } = [];
}
