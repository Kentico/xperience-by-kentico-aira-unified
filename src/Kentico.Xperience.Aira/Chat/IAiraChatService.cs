using Kentico.Xperience.Aira.Chat.Models;

namespace Kentico.Xperience.Aira.Chat;

/// <summary>
/// Service responsible for managing chat history of a user.
/// </summary>
public interface IAiraChatService
{
    /// <summary>
    /// Returns the chat history of a user.
    /// </summary>
    /// <param name="userId">Admin application user id.</param>
    /// <returns>A Task returning a List of <see cref="AiraChatMessageViewModel"/> in User's history.</returns>
    Task<List<AiraChatMessageViewModel>> GetUserChatHistory(int userId);

    /// <summary>
    /// Generates new suggested prompts for a user and saves them in the history.
    /// </summary>
    /// <param name="userId">Admin application user id.</param>
    /// <returns>A task returning a <see cref="AiraChatMessageViewModel"/> with the generated prompts.</returns>
    AiraPromptGroupModel SaveAiraPrompts(int userId, List<string> suggestions);

    /// <summary>
    /// Removes used prompt group.
    /// </summary>
    /// <param name="promptGroupId">Prompt group id.</param>
    void RemoveUsedPrompts(string promptGroupId);

    /// <summary>
    /// Saves a text message in the history.
    /// </summary>
    /// <param name="text">Text of the message.</param>
    /// <param name="userId">Admin application user id.</param>
    /// <param name="role">Role of the chat member.</param>
    void SaveMessage(string text, int userId, string role);

    Task<AiraAIResponse?> GetAIResponseOrNull(string message, int numberOfIncludedHistoryMessages, int userId);

    void UpdateChatSummary(int userId, string summary);
}
