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
    /// <param name="threadId">The chat thread id.</param>
    /// <returns>A Task returning a List of <see cref="AiraChatMessage"/> in User's history.</returns>
    Task<List<AiraChatMessage>> GetUserChatHistory(int userId, int threadId);

    /// <summary>
    /// Generates new suggested prompts for a user and saves them in the history.
    /// </summary>
    /// <param name="userId">Admin application user id.</param>
    /// <param name="threadId">The chat thread id.</param>
    /// <returns>A task returning a <see cref="AiraChatMessage"/> with the generated prompts.</returns>
    Task<AiraChatMessage> GenerateAiraPrompts(int userId, int threadId);

    /// <summary>
    /// Gets a chat thread model of the specified id. If the id is null the latest used thread will be returned. If no thread for the user exists, a new thread for the user will be created.
    /// </summary>
    /// <param name="userId">Admin application user id.</param>
    /// <param name="threadId">The desired thread id or null.</param>
    /// <returns>The task containing the desired <see cref="AiraChatThreadModel"/>.</returns>
    Task<AiraChatThreadModel> GetAiraChatThreadModel(int userId, int? threadId = null);

    /// <summary>
    /// Creates new chat thread for the specified user.
    /// </summary>
    /// <param name="userId">Admin application user id.</param>
    /// <returns></returns>
    Task<AiraChatThreadModel> CreateNewChatThread(int userId);

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
    /// <param name="threadId">The chat thread id.</param>
    void SaveMessage(string text, int userId, string role, int threadId);
}
