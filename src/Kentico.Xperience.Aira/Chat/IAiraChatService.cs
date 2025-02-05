using Kentico.Xperience.Aira.Chat.Models;

namespace Kentico.Xperience.Aira.Chat;

public interface IAiraChatService
{
    Task<List<AiraChatMessage>> GetUserChatHistory(int userId);
    Task<AiraChatMessage> GenerateAiraPrompts(int userId);
    void RemoveUsedPrompts(string promptGroupId);
    void SaveMessage(string text, int userId, string role);
}
