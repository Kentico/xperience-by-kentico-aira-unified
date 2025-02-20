using System.Text.Json;

using CMS.ContactManagement;
using CMS.DataEngine;

using Kentico.Xperience.AiraUnified.Admin;
using Kentico.Xperience.AiraUnified.Admin.InfoModels;
using Kentico.Xperience.AiraUnified.Chat.Models;
using Kentico.Xperience.AiraUnified.Insights;

using Microsoft.Extensions.Options;

namespace Kentico.Xperience.AiraUnified.Chat;

internal class AiraUnifiedChatService : IAiraUnifiedChatService
{
    private readonly IInfoProvider<AiraUnifiedChatPromptGroupInfo> airaUnifiedChatPromptGroupProvider;
    private readonly IInfoProvider<AiraUnifiedChatPromptInfo> airaUnifiedChatPromptProvider;
    private readonly IInfoProvider<AiraUnifiedChatMessageInfo> airaUnifiedChatMessageProvider;
    private readonly IInfoProvider<AiraUnifiedChatSummaryInfo> airaUnifiedChatSummaryProvider;
    private readonly IInfoProvider<ContactGroupInfo> contactGroupProvider;
    private readonly IAiraUnifiedInsightsService airaUnifiedInsightsService;
    private readonly AiraUnifiedOptions airaUnifiedOptions;
    private readonly HttpClient httpClient;

    public AiraUnifiedChatService(IInfoProvider<AiraUnifiedChatPromptGroupInfo> airaUnifiedChatPromptGroupProvider,
        IInfoProvider<AiraUnifiedChatPromptInfo> airaUnifiedChatPromptProvider,
        IInfoProvider<ContactGroupInfo> contactGroupProvider,
        IInfoProvider<AiraUnifiedChatMessageInfo> airaUnifiedChatMessageProvider,
        IInfoProvider<AiraUnifiedChatSummaryInfo> airaUnifiedChatSummaryProvider,
        IAiraUnifiedInsightsService airaUnifiedInsightsService,
        IOptions<AiraUnifiedOptions> airaUnifiedOptions,
        HttpClient httpClient)
    {
        this.airaUnifiedChatPromptGroupProvider = airaUnifiedChatPromptGroupProvider;
        this.airaUnifiedChatPromptProvider = airaUnifiedChatPromptProvider;
        this.contactGroupProvider = contactGroupProvider;
        this.airaUnifiedChatMessageProvider = airaUnifiedChatMessageProvider;
        this.airaUnifiedInsightsService = airaUnifiedInsightsService;
        this.airaUnifiedChatSummaryProvider = airaUnifiedChatSummaryProvider;
        this.httpClient = httpClient;
        this.airaUnifiedOptions = airaUnifiedOptions.Value;
    }

    public async Task<List<AiraUnifiedChatMessageViewModel>> GetUserChatHistory(int userId)
    {
        var chatPrompts = (await airaUnifiedChatPromptProvider
            .Get()
            .Source(x => x.InnerJoin<AiraUnifiedChatPromptGroupInfo>(
                nameof(AiraUnifiedChatPromptInfo.AiraUnifiedChatPromptChatPromptGroupId),
                nameof(AiraUnifiedChatPromptGroupInfo.AiraUnifiedChatPromptGroupId)
            ))
            .WhereEquals(nameof(AiraUnifiedChatPromptGroupInfo.AiraUnifiedChatPromptUserId), userId)
            .Columns(nameof(AiraUnifiedChatPromptGroupInfo.AiraUnifiedChatPromptGroupCreatedWhen),
                nameof(AiraUnifiedChatPromptGroupInfo.AiraUnifiedChatPromptGroupId),
                nameof(AiraUnifiedChatPromptInfo.AiraUnifiedChatPromptText))
            .OrderBy(nameof(AiraUnifiedChatPromptGroupInfo.AiraUnifiedChatPromptGroupCreatedWhen))
            .GetDataContainerResultAsync())
            .GroupBy(x =>
                new
                {
                    PromptGroupId = x[nameof(AiraUnifiedChatPromptGroupInfo.AiraUnifiedChatPromptGroupId)] as int?
                }
            );

        var textMessages = (await airaUnifiedChatMessageProvider.Get()
            .WhereEquals(nameof(AiraUnifiedChatMessageInfo.AiraUnifiedChatMessageUserId), userId)
            .GetEnumerableTypedResultAsync())
            .Select(x => new AiraUnifiedChatMessageViewModel
            {
                Role = x.AiraUnifiedChatMessageRole == AiraUnifiedConstants.AiraUnifiedChatRoleIdentifier ?
                    AiraUnifiedConstants.AiraUnifiedChatRoleName :
                    AiraUnifiedConstants.UserChatRoleName,
                CreatedWhen = x.AiraUnifiedChatMessageCreatedWhen,
                Message = x.AiraUnifiedChatMessageText
            });

        return chatPrompts.Select(x =>
        {
            var prompts = x.AsEnumerable();

            return new AiraUnifiedChatMessageViewModel
            {
                QuickPrompts = prompts.Select(x => (string)x[nameof(AiraUnifiedChatPromptInfo.AiraUnifiedChatPromptText)]).ToList(),
                Role = AiraUnifiedConstants.AiraUnifiedChatRoleName,
                QuickPromptsGroupId = x.Key.PromptGroupId!.ToString()!,
                CreatedWhen = (DateTime)prompts.First()[nameof(AiraUnifiedChatPromptGroupInfo.AiraUnifiedChatPromptGroupCreatedWhen)]
            };
        })
        .Union(textMessages)
        .OrderBy(x => x.CreatedWhen)
        .ToList();
    }

    public void SaveMessage(string text, int userId, string role)
    {
        var message = new AiraUnifiedChatMessageInfo
        {
            AiraUnifiedChatMessageCreatedWhen = DateTime.Now,
            AiraUnifiedChatMessageText = text,
            AiraUnifiedChatMessageUserId = userId,
            AiraUnifiedChatMessageRole = role == AiraUnifiedConstants.AiraUnifiedChatRoleName ?
                AiraUnifiedConstants.AiraUnifiedChatRoleIdentifier :
                AiraUnifiedConstants.UserChatRoleIdentifier
        };

        airaUnifiedChatMessageProvider.Set(message);
    }

    public async Task<AiraUnifiedAIResponse?> GetAIResponseOrNull(string message, int numberOfIncludedHistoryMessages, int userId)
    {
        var textMessageHistory = (await airaUnifiedChatMessageProvider.Get()
            .WhereEquals(nameof(AiraUnifiedChatMessageInfo.AiraUnifiedChatMessageUserId), userId)
            .OrderByDescending(nameof(AiraUnifiedChatMessageInfo.AiraUnifiedChatMessageCreatedWhen))
            .TopN(numberOfIncludedHistoryMessages)
            .GetEnumerableTypedResultAsync())
            .Select(x => new AiraUnifiedChatMessageModel
            {
                Role = x.AiraUnifiedChatMessageRole == AiraUnifiedConstants.AiraUnifiedChatRoleIdentifier ?
                    AiraUnifiedConstants.AIRequestAssistantRoleName :
                    AiraUnifiedConstants.AIRequestUserRoleName,
                Content = x.AiraUnifiedChatMessageText
            })
            .ToList();

        var conversationSummary = airaUnifiedChatSummaryProvider.Get()
            .WhereEquals(nameof(AiraUnifiedChatSummaryInfo.AiraUnifiedChatSummaryUserId), userId)
            .FirstOrDefault();

        if (conversationSummary is not null)
        {
            textMessageHistory.Add(new AiraUnifiedChatMessageModel
            {
                Role = AiraUnifiedConstants.AIRequestAssistantRoleName,
                Content = conversationSummary.AiraUnifiedChatSummaryContent
            });
        }

        var request = new AiraUnifiedAIRequest
        {
            ChatMessage = message,
            ConversationHistory = textMessageHistory,
            AppInsights = await GenerateInsights(userId)
        };

        var jsonRequest = JsonSerializer.Serialize(request);
        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        content.Headers.Add("Ocp-Apim-Subscription-Key", airaUnifiedOptions.AiraUnifiedApiSubscriptionKey);

        var response = await httpClient.PostAsync(AiraUnifiedConstants.AiraUnifiedAIEndpoint, content);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<AiraUnifiedAIResponse>(jsonResponse);
    }

    private async Task<Dictionary<string, string>> GenerateInsights(int userId)
    {
        var emailInsights = await airaUnifiedInsightsService.GetEmailInsights();

        var reusableDraftContentInsights = await airaUnifiedInsightsService.GetContentInsights(ContentType.Reusable, userId, AiraUnifiedConstants.InsightsDraftIdentifier);
        var reusableScheduledContentInsights = await airaUnifiedInsightsService.GetContentInsights(ContentType.Reusable, userId, AiraUnifiedConstants.InsightsScheduledIdentifier);
        var websiteDraftContentInsights = await airaUnifiedInsightsService.GetContentInsights(ContentType.Website, userId, AiraUnifiedConstants.InsightsDraftIdentifier);
        var websiteScheduledContentInsights = await airaUnifiedInsightsService.GetContentInsights(ContentType.Website, userId, AiraUnifiedConstants.InsightsScheduledIdentifier);

        var contactGroups = await contactGroupProvider.Get().GetEnumerableTypedResultAsync();
        var contactGroupNames = contactGroups.Select(x => x.ContactGroupDisplayName).ToArray();

        var contactGroupInsights = airaUnifiedInsightsService.GetContactGroupInsights(contactGroupNames);

        var separator = '_';

        var resultInsights = new Dictionary<string, string>
        {
            { string.Join(separator, AiraUnifiedConstants.InsightsContentIdentifier, AiraUnifiedConstants.InsightsReusableIdentifier, AiraUnifiedConstants.InsightsInDraftIdentifier, AiraUnifiedConstants.InsightsCountIdentifier), reusableDraftContentInsights.Items.Count.ToString()},
            { string.Join(separator, AiraUnifiedConstants.InsightsContentIdentifier, AiraUnifiedConstants.InsightsReusableIdentifier, AiraUnifiedConstants.InsightsInScheduledIdentifier, AiraUnifiedConstants.InsightsCountIdentifier), reusableScheduledContentInsights.Items.Count.ToString()},
            { string.Join(separator, AiraUnifiedConstants.InsightsContentIdentifier, AiraUnifiedConstants.InsightsWebsiteIdentifier, AiraUnifiedConstants.InsightsInDraftIdentifier, AiraUnifiedConstants.InsightsCountIdentifier), websiteDraftContentInsights.Items.Count.ToString()},
            { string.Join(separator, AiraUnifiedConstants.InsightsContentIdentifier, AiraUnifiedConstants.InsightsWebsiteIdentifier, AiraUnifiedConstants.InsightsInScheduledIdentifier, AiraUnifiedConstants.InsightsCountIdentifier), websiteScheduledContentInsights.Items.Count.ToString()},

            { string.Join(separator, AiraUnifiedConstants.InsightsAllAccountsIdentifier, AiraUnifiedConstants.InsightsCountIdentifier), contactGroupInsights.AllCount.ToString() }
        };

        foreach (var contactGroup in contactGroupInsights.Groups)
        {
            resultInsights.Add(
                string.Join(separator, AiraUnifiedConstants.InsightsContactGroupIdentifier, contactGroup.Name, AiraUnifiedConstants.InsightsCountIdentifier), contactGroup.Count.ToString()
            );

            resultInsights.Add(
                string.Join(separator, AiraUnifiedConstants.InsightsContactGroupIdentifier, contactGroup.Name, AiraUnifiedConstants.InsightsRatioToAllContactsIdentifier), ((decimal)contactGroup.Count / contactGroupInsights.AllCount).ToString()
            );
        }

        foreach (var emailInsight in emailInsights)
        {
            AddEmailInsight(nameof(EmailInsightsModel.EmailsSent), emailInsight.EmailConfigurationName, emailInsight.EmailsSent.ToString());
            AddEmailInsight(nameof(EmailInsightsModel.EmailsDelivered), emailInsight.EmailConfigurationName, emailInsight.EmailsDelivered.ToString());
            AddEmailInsight(nameof(EmailInsightsModel.EmailsOpened), emailInsight.EmailConfigurationName, emailInsight.EmailsOpened.ToString());
            AddEmailInsight(nameof(EmailInsightsModel.LinksClicked), emailInsight.EmailConfigurationName, emailInsight.LinksClicked.ToString());
            AddEmailInsight(nameof(EmailInsightsModel.UnsubscribeRate), emailInsight.EmailConfigurationName, emailInsight.UnsubscribeRate.ToString());
            AddEmailInsight(nameof(EmailInsightsModel.SpamReports), emailInsight.EmailConfigurationName, emailInsight.SpamReports.ToString());
        }

        AddContentItemsToInsights(reusableDraftContentInsights.Items, AiraUnifiedConstants.InsightsReusableIdentifier, AiraUnifiedConstants.InsightsInDraftIdentifier);
        AddContentItemsToInsights(reusableScheduledContentInsights.Items, AiraUnifiedConstants.InsightsReusableIdentifier, AiraUnifiedConstants.InsightsInScheduledIdentifier);
        AddContentItemsToInsights(websiteDraftContentInsights.Items, AiraUnifiedConstants.InsightsWebsiteIdentifier, AiraUnifiedConstants.InsightsInDraftIdentifier);
        AddContentItemsToInsights(websiteScheduledContentInsights.Items, AiraUnifiedConstants.InsightsWebsiteIdentifier, AiraUnifiedConstants.InsightsInScheduledIdentifier);

        void AddEmailInsight(string statisticsParameter, string emailConfigurationName, string value) =>
            resultInsights.Add(
                string.Join(separator, AiraUnifiedConstants.InsightsEmailIdentifier, statisticsParameter, emailConfigurationName),
                value
            );

        void AddContentItemsToInsights(List<ContentItemInsightsModel> items, string contentTypePrefix, string releaseStatusPrefix)
        {
            foreach (var contentItem in items)
            {
                resultInsights.Add(
                    string.Join(separator, AiraUnifiedConstants.InsightsEmailIdentifier, contentTypePrefix, releaseStatusPrefix, contentItem.DisplayName),
                    ""
                );
            }
        }

        return resultInsights;
    }

    public AiraUnifiedPromptGroupModel SaveAiraPrompts(int userId, List<string> suggestions)
    {
        var chatPromptGroup = new AiraUnifiedChatPromptGroupInfo
        {
            AiraUnifiedChatPromptGroupCreatedWhen = DateTime.Now,
            AiraUnifiedChatPromptUserId = userId,
        };

        airaUnifiedChatPromptGroupProvider.Set(chatPromptGroup);

        var messages = new List<AiraUnifiedChatPromptInfo>();

        foreach (var suggestion in suggestions)
        {
            var prompt = new AiraUnifiedChatPromptInfo
            {
                AiraUnifiedChatPromptText = suggestion,
                AiraUnifiedChatPromptChatPromptGroupId = chatPromptGroup.AiraUnifiedChatPromptGroupId
            };

            airaUnifiedChatPromptProvider.Set(prompt);

            messages.Add(prompt);
        }

        return new AiraUnifiedPromptGroupModel
        {
            QuickPromptsGroupId = chatPromptGroup.AiraUnifiedChatPromptGroupId,
            QuickPrompts = messages.Select(x => x.AiraUnifiedChatPromptText)
        };
    }

    public void UpdateChatSummary(int userId, string summary)
    {
        var summaryInfo = airaUnifiedChatSummaryProvider
            .Get()
            .WhereEquals(nameof(AiraUnifiedChatSummaryInfo.AiraUnifiedChatSummaryUserId), userId)
            .FirstOrDefault()
            ??
            new AiraUnifiedChatSummaryInfo
            {
                AiraUnifiedChatSummaryUserId = userId
            };

        summaryInfo.AiraUnifiedChatSummaryContent = summary;

        airaUnifiedChatSummaryProvider.Set(summaryInfo);
    }

    public void RemoveUsedPrompts(string promptGroupId)
    {
        if (int.TryParse(promptGroupId, out var id))
        {
            airaUnifiedChatPromptGroupProvider.BulkDelete(new WhereCondition($"{nameof(AiraUnifiedChatPromptGroupInfo.AiraUnifiedChatPromptGroupId)} = {id}"));
            airaUnifiedChatPromptProvider.BulkDelete(new WhereCondition($"{nameof(AiraUnifiedChatPromptInfo.AiraUnifiedChatPromptChatPromptGroupId)} = {id}"));
        }
    }
}
