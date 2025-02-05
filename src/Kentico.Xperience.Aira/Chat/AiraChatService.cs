using CMS.DataEngine;

using Kentico.Xperience.Aira.Admin;
using Kentico.Xperience.Aira.Admin.InfoModels;
using Kentico.Xperience.Aira.Chat.Models;

namespace Kentico.Xperience.Aira.Chat;

internal class AiraChatService : IAiraChatService
{
    private readonly IInfoProvider<AiraChatPromptGroupInfo> airaChatPromptGroupProvider;
    private readonly IInfoProvider<AiraChatPromptInfo> airaChatPromptProvider;
    private readonly IInfoProvider<AiraChatMessageInfo> airaChatMessageProvider;

    public AiraChatService(IInfoProvider<AiraChatPromptGroupInfo> airaChatPromptGroupProvider,
        IInfoProvider<AiraChatPromptInfo> airaChatPromptProvider,
        IInfoProvider<AiraChatMessageInfo> airaChatMessageProvider)
    {
        this.airaChatPromptGroupProvider = airaChatPromptGroupProvider;
        this.airaChatPromptProvider = airaChatPromptProvider;
        this.airaChatMessageProvider = airaChatMessageProvider;
    }

    public async Task<List<AiraChatMessage>> GetUserChatHistory(int userId)
    {
        var chatPrompts = (await airaChatPromptProvider
            .Get()
            .Source(x => x.InnerJoin<AiraChatPromptGroupInfo>(
                nameof(AiraChatPromptInfo.AiraChatPromptChatPromptGroupId),
                nameof(AiraChatPromptGroupInfo.AiraChatPromptGroupId)
            ))
            .WhereEquals(nameof(AiraChatPromptGroupInfo.AiraChatPromptUserId), userId)
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

    public void SaveMessage(string text, int userId, string role)
    {
        var message = new AiraChatMessageInfo
        {
            AiraChatMessageCreatedWhen = DateTime.Now,
            AiraChatMessageText = text,
            AiraChatMessageUserId = userId,
            AiraChatMessageRole = role == AiraCompanionAppConstants.AiraChatRoleName ?
                AiraCompanionAppConstants.AiraChatRoleIdentifier :
                AiraCompanionAppConstants.UserChatRoleIdentifier
        };

        airaChatMessageProvider.Set(message);
    }

    public async Task<AiraChatMessage> GenerateAiraPrompts(int userId)
    {
        var chatPromptGroup = new AiraChatPromptGroupInfo
        {
            AiraChatPromptGroupCreatedWhen = DateTime.Now,
            AiraChatPromptUserId = userId,
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
