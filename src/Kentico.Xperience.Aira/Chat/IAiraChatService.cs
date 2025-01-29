using Kentico.Xperience.Aira.Chat.Models;

namespace Kentico.Xperience.Aira.Chat;

public interface IAiraChatService
{
    Task<List<AiraChatMessage>> GetUserChatHistory(int userID);
    Task<AiraChatMessage> GenerateAiraPrompts(int userID);
    void RemoveUsedPrompts(string promptGroupId);
}
