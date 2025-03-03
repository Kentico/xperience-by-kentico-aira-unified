namespace Kentico.Xperience.AiraUnified.Chat.Models;

public class ChatPromptLibraryViewModel
{
    public List<ChatPromptCategoryModel> PromptCategories { get; set; } = [];
}

public class ChatPromptCategoryModel
{
    public string CategoryName { get; set; } = string.Empty;
    public List<string> Prompts { get; set; } = [];
}
