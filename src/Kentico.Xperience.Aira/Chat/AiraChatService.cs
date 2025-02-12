using CMS.DataEngine;
using CMS.DataEngine.Query;

using Kentico.Xperience.Aira.Admin;
using Kentico.Xperience.Aira.Admin.InfoModels;
using Kentico.Xperience.Aira.Chat.Models;

namespace Kentico.Xperience.Aira.Chat;

internal class AiraChatService : IAiraChatService
{
    private readonly IInfoProvider<AiraChatPromptGroupInfo> airaChatPromptGroupProvider;
    private readonly IInfoProvider<AiraChatPromptInfo> airaChatPromptProvider;
    private readonly IInfoProvider<AiraChatMessageInfo> airaChatMessageProvider;
    private readonly IInfoProvider<AiraChatThreadInfo> airaChatThreadProvider;

    public AiraChatService(IInfoProvider<AiraChatPromptGroupInfo> airaChatPromptGroupProvider,
        IInfoProvider<AiraChatPromptInfo> airaChatPromptProvider,
        IInfoProvider<AiraChatMessageInfo> airaChatMessageProvider,
        IInfoProvider<AiraChatThreadInfo> airaChatThreadProvider)
    {
        this.airaChatPromptGroupProvider = airaChatPromptGroupProvider;
        this.airaChatPromptProvider = airaChatPromptProvider;
        this.airaChatMessageProvider = airaChatMessageProvider;
        this.airaChatThreadProvider = airaChatThreadProvider;
    }

    public async Task<AiraChatThreadModel> GetAiraChatThreadModel(int userId, int? threadId = null)
    {
        if (threadId is null)
        {
            var numberOfThreads = await airaChatThreadProvider.Get().GetCountAsync();

            if (numberOfThreads == 0)
            {
                return await CreateNewChatThread(userId);
            }

            var latestUsedThread = airaChatThreadProvider
                .Get()
                .WhereEquals(nameof(AiraChatThreadInfo.AiraChatThreadUserId), userId)
                .WhereTrue(nameof(AiraChatThreadInfo.AiraChatThreadIsLatest))
                .SingleOrDefault() ?? throw new InvalidOperationException($"No thread exists for the user with id {userId}.");

            return new AiraChatThreadModel
            {
                ThreadName = latestUsedThread.AiraChatThreadName,
                ThreadId = latestUsedThread.AiraChatThreadId
            };
        }

        var chatThread = airaChatThreadProvider
            .Get()
            .WhereEquals(nameof(AiraChatThreadInfo.AiraChatThreadUserId), userId)
            .WhereEquals(nameof(AiraChatThreadInfo.AiraChatThreadId), threadId.Value)
            .FirstOrDefault() ?? throw new InvalidOperationException($"The specified thread with id {threadId} for the specified user with id {userId} does not exist.");

        return new AiraChatThreadModel
        {
            ThreadName = chatThread.AiraChatThreadName,
            ThreadId = chatThread.AiraChatThreadId
        };
    }

    public async Task<AiraChatThreadModel> CreateNewChatThread(int userId)
    {
        var countOfThreads = await airaChatThreadProvider.Get().GetCountAsync();

        var newChatThread = new AiraChatThreadInfo
        {
            AiraChatThreadUserId = userId,
            AiraChatThreadIsLatest = true,
            AiraChatThreadName = $"{countOfThreads + 1}"
        };

        airaChatThreadProvider.Set(newChatThread);

        return new AiraChatThreadModel
        {
            ThreadId = newChatThread.AiraChatThreadId,
            ThreadName = newChatThread.AiraChatThreadName
        };
    }

    public async Task<List<AiraChatMessage>> GetUserChatHistory(int userId, int threadId)
    {
        var chatPrompts = (await airaChatPromptProvider
            .Get()
            .Source(x => x.InnerJoin<AiraChatPromptGroupInfo>(
                nameof(AiraChatPromptInfo.AiraChatPromptChatPromptGroupId),
                nameof(AiraChatPromptGroupInfo.AiraChatPromptGroupId)
            ))
            .WhereEquals(nameof(AiraChatPromptGroupInfo.AiraChatPromptGroupUserId), userId)
            .WhereEquals(nameof(AiraChatPromptGroupInfo.AiraChatPromptGroupThreadId), threadId)
            .Columns(nameof(AiraChatPromptGroupInfo.AiraChatPromptGroupCreatedWhen),
                nameof(AiraChatPromptGroupInfo.AiraChatPromptGroupId),
                nameof(AiraChatPromptInfo.AiraChatPromptText))
            .OrderBy(nameof(AiraChatPromptGroupInfo.AiraChatPromptGroupCreatedWhen))
            .GetDataContainerResultAsync())
            .GroupBy(x =>
                new
                {
                    PromptGroupId = x[nameof(AiraChatPromptGroupInfo.AiraChatPromptGroupId)] as int?
                }
            );

        var textMessages = (await airaChatMessageProvider.Get()
            .WhereEquals(nameof(AiraChatMessageInfo.AiraChatMessageUserId), userId)
            .WhereEquals(nameof(AiraChatMessageInfo.AiraChatMessageThreadId), threadId)
            .GetEnumerableTypedResultAsync())
            .Select(x => new AiraChatMessage
            {
                Role = x.AiraChatMessageRole == AiraCompanionAppConstants.AiraChatRoleIdentifier ?
                    AiraCompanionAppConstants.AiraChatRoleName :
                    AiraCompanionAppConstants.UserChatRoleName,
                CreatedWhen = x.AiraChatMessageCreatedWhen,
                Message = x.AiraChatMessageText
            });

        return chatPrompts.Select(x =>
        {
            var prompts = x.AsEnumerable();

            return new AiraChatMessage
            {
                QuickPrompts = prompts.Select(x => (string)x[nameof(AiraChatPromptInfo.AiraChatPromptText)]).ToList(),
                Role = AiraCompanionAppConstants.AiraChatRoleName,
                QuickPromptsGroupId = x.Key.PromptGroupId!.ToString()!,
                CreatedWhen = (DateTime)prompts.First()[nameof(AiraChatPromptGroupInfo.AiraChatPromptGroupCreatedWhen)]
            };
        })
        .Union(textMessages)
        .OrderBy(x => x.CreatedWhen)
        .ToList();
    }

    public void SaveMessage(string text, int userId, string role, int threadId)
    {
        var message = new AiraChatMessageInfo
        {
            AiraChatMessageCreatedWhen = DateTime.Now,
            AiraChatMessageText = text,
            AiraChatMessageUserId = userId,
            AiraChatMessageThreadId = threadId,
            AiraChatMessageRole = role == AiraCompanionAppConstants.AiraChatRoleName ?
                AiraCompanionAppConstants.AiraChatRoleIdentifier :
                AiraCompanionAppConstants.UserChatRoleIdentifier
        };

        airaChatMessageProvider.Set(message);
    }

    public async Task<AiraChatMessage> GenerateAiraPrompts(int userId, int threadId)
    {
        var chatPromptGroup = new AiraChatPromptGroupInfo
        {
            AiraChatPromptGroupCreatedWhen = DateTime.Now,
            AiraChatPromptGroupUserId = userId,
            AiraChatPromptGroupThreadId = threadId
        };

        airaChatPromptGroupProvider.Set(chatPromptGroup);

        var messages = new List<AiraChatPromptInfo>();

        var drafts = new AiraChatPromptInfo
        {
            AiraChatPromptText = "Reusable Drafts",
            AiraChatPromptChatPromptGroupId = chatPromptGroup.AiraChatPromptGroupId
        };

        var scheduled = new AiraChatPromptInfo
        {
            AiraChatPromptText = "Website Scheduled",
            AiraChatPromptChatPromptGroupId = chatPromptGroup.AiraChatPromptGroupId
        };

        var emails = new AiraChatPromptInfo
        {
            AiraChatPromptText = "Emails",
            AiraChatPromptChatPromptGroupId = chatPromptGroup.AiraChatPromptGroupId
        };

        var contactGroups = new AiraChatPromptInfo
        {
            AiraChatPromptText = "Contact Groups",
            AiraChatPromptChatPromptGroupId = chatPromptGroup.AiraChatPromptGroupId
        };

        airaChatPromptProvider.Set(drafts);
        airaChatPromptProvider.Set(scheduled);
        airaChatPromptProvider.Set(emails);
        airaChatPromptProvider.Set(contactGroups);

        messages.Add(drafts);
        messages.Add(scheduled);
        messages.Add(emails);
        messages.Add(contactGroups);

        return await Task.FromResult(new AiraChatMessage
        {
            QuickPromptsGroupId = chatPromptGroup.AiraChatPromptGroupId.ToString(),
            CreatedWhen = chatPromptGroup.AiraChatPromptGroupCreatedWhen,
            QuickPrompts = messages.Select(x => x.AiraChatPromptText)
        });
    }

    public void RemoveUsedPrompts(string promptGroupId)
    {
        if (int.TryParse(promptGroupId, out var id))
        {
            airaChatPromptGroupProvider.BulkDelete(new WhereCondition($"{nameof(AiraChatPromptGroupInfo.AiraChatPromptGroupId)} = {id}"));
            airaChatPromptProvider.BulkDelete(new WhereCondition($"{nameof(AiraChatPromptInfo.AiraChatPromptChatPromptGroupId)} = {id}"));
        }
    }
}
