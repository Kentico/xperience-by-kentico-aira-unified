using Kentico.Xperience.AiraUnified.Chat.Models;

namespace Kentico.Xperience.AiraUnified.Chat;

/// <summary>
/// Service responsible for managing chat history of a user.
/// </summary>
public interface IAiraUnifiedChatService
{
    /// <summary>
    /// Returns the chat history of a user.
    /// </summary>
    /// <param name="userId">Admin application user id.</param>
    /// <returns>A Task returning a List of <see cref="AiraUnifiedChatMessageViewModel"/> in User's history.</returns>
    Task<List<AiraUnifiedChatMessageViewModel>> GetUserChatHistory(int userId);

    /// <summary>
    /// Generates new suggested prompts for a user and saves them in the history.
    /// </summary>
    /// <param name="userId">Admin application user id.</param>
    /// <param name="suggestions">The prompt suggestions.</param>
    /// <returns>A task returning a <see cref="AiraUnifiedChatMessageViewModel"/> with the generated prompts.</returns>
    AiraUnifiedPromptGroupModel SaveAiraPrompts(int userId, List<string> suggestions);

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

    /// <summary>
    /// Calls the ai endpoint with a message from the user.
    /// </summary>
    /// <param name="message">The user message.</param>
    /// <param name="numberOfIncludedHistoryMessages">Number of history messages added to the context.</param>
    /// <param name="userId">The user id.</param>
    /// <returns>A task returning the <see cref="AiraUnifiedAIResponse"/> with the ai response.</returns>
    Task<AiraUnifiedAIResponse?> GetAIResponseOrNull(string message, int numberOfIncludedHistoryMessages, int userId);

    /// <summary>
    /// Updates the chat summary of a user.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="summary">New summary.</param>
    void UpdateChatSummary(int userId, string summary);
}
