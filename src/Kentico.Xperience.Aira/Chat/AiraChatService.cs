using System.Text.Json;

using CMS.ContactManagement;
using CMS.DataEngine;

using Kentico.Xperience.Aira.Admin;
using Kentico.Xperience.Aira.Admin.InfoModels;
using Kentico.Xperience.Aira.Chat.Models;
using Kentico.Xperience.Aira.Insights;

using Microsoft.Extensions.Options;

namespace Kentico.Xperience.Aira.Chat;

internal class AiraChatService : IAiraChatService
{
    private readonly IInfoProvider<AiraChatPromptGroupInfo> airaChatPromptGroupProvider;
    private readonly IInfoProvider<AiraChatPromptInfo> airaChatPromptProvider;
    private readonly IInfoProvider<AiraChatMessageInfo> airaChatMessageProvider;
    private readonly IInfoProvider<AiraChatSummaryInfo> airaChatSummaryProvider;
    private readonly IInfoProvider<ContactGroupInfo> contactGroupProvider;
    private readonly IAiraInsightsService airaInsightsService;
    private readonly AiraCompanionAppOptions airaCompanionAppOptions;
    private readonly HttpClient httpClient;

    public AiraChatService(IInfoProvider<AiraChatPromptGroupInfo> airaChatPromptGroupProvider,
        IInfoProvider<AiraChatPromptInfo> airaChatPromptProvider,
        IInfoProvider<ContactGroupInfo> contactGroupProvider,
        IInfoProvider<AiraChatMessageInfo> airaChatMessageProvider,
        IInfoProvider<AiraChatSummaryInfo> airaChatSummaryProvider,
        IAiraInsightsService airaInsightsService,
        IOptions<AiraCompanionAppOptions> airaCompanionAppOptions,
        HttpClient httpClient)
    {
        this.airaChatPromptGroupProvider = airaChatPromptGroupProvider;
        this.airaChatPromptProvider = airaChatPromptProvider;
        this.contactGroupProvider = contactGroupProvider;
        this.airaChatMessageProvider = airaChatMessageProvider;
        this.airaInsightsService = airaInsightsService;
        this.airaChatSummaryProvider = airaChatSummaryProvider;
        this.httpClient = httpClient;
        this.airaCompanionAppOptions = airaCompanionAppOptions.Value;
    }

    public async Task<List<AiraChatMessageViewModel>> GetUserChatHistory(int userId)
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
            .Select(x => new AiraChatMessageViewModel
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

            return new AiraChatMessageViewModel
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

    public async Task<AiraAIResponse?> GetAIResponseOrNull(string message, int numberOfIncludedHistoryMessages, int userId)
    {
        var textMessageHistory = (await airaChatMessageProvider.Get()
            .WhereEquals(nameof(AiraChatMessageInfo.AiraChatMessageUserId), userId)
            .OrderByDescending(nameof(AiraChatMessageInfo.AiraChatMessageCreatedWhen))
            .TopN(numberOfIncludedHistoryMessages)
            .GetEnumerableTypedResultAsync())
            .Select(x => new AiraChatMessageModel
            {
                Role = x.AiraChatMessageRole == AiraCompanionAppConstants.AiraChatRoleIdentifier ?
                    AiraCompanionAppConstants.AIRequestAssistantRoleName :
                    AiraCompanionAppConstants.AIRequestUserRoleName,
                Content = x.AiraChatMessageText
            })
            .ToList();

        var conversationSummary = airaChatSummaryProvider.Get()
            .WhereEquals(nameof(AiraChatSummaryInfo.AiraChatSummaryUserId), userId)
            .FirstOrDefault();

        if (conversationSummary is not null)
        {
            textMessageHistory.Add(new AiraChatMessageModel
            {
                Role = AiraCompanionAppConstants.AIRequestAssistantRoleName,
                Content = conversationSummary.AiraChatSummaryContent
            });
        }

        var request = new AiraAIRequest
        {
            ChatMessage = message,
            ConversationHistory = textMessageHistory,
            AppInsights = await GenerateInsights(userId)
        };

        var jsonRequest = JsonSerializer.Serialize(request);
        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        content.Headers.Add("Ocp-Apim-Subscription-Key", airaCompanionAppOptions.AiraApiSubscriptionKey);

        var response = await httpClient.PostAsync(AiraCompanionAppConstants.AiraAIEndpoint, content);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<AiraAIResponse>(jsonResponse);
    }

    private async Task<Dictionary<string, string>> GenerateInsights(int userId)
    {
        var emailInsights = await airaInsightsService.GetEmailInsights();

        var draft = "Draft";
        var scheduled = "Scheduled";

        var reusableDraftContentInsights = await airaInsightsService.GetContentInsights(ContentType.Reusable, userId, draft);
        var reusableScheduledContentInsights = await airaInsightsService.GetContentInsights(ContentType.Reusable, userId, scheduled);
        var websiteDraftContentInsights = await airaInsightsService.GetContentInsights(ContentType.Website, userId, draft);
        var websiteScheduledContentInsights = await airaInsightsService.GetContentInsights(ContentType.Website, userId, scheduled);

        var contactGroups = await contactGroupProvider.Get().GetEnumerableTypedResultAsync();
        var contactGroupNames = contactGroups.Select(x => x.ContactGroupDisplayName).ToArray();

        var contactGroupInsights = airaInsightsService.GetContactGroupInsights(contactGroupNames);

        var emailPrefix = "email";
        var contentPrefix = "content";
        var inDraftPrefix = "inDraft";
        var scheduledPrefix = "inScheduled";
        var reusableContentPrefix = "reusable";
        var websiteContentPrefix = "website";
        var allAccountsPrefix = "allAccounts";
        var contactGroupPrefix = "contactGroup";
        var countIdentifier = "count";
        var ratioToAllContactsIdentifier = "ratioOfContactsInGroupToOtherContacts";
        var separator = '_';

        var resultInsights = new Dictionary<string, string>
        {
            { string.Join(separator, contentPrefix, reusableContentPrefix, inDraftPrefix, countIdentifier), reusableDraftContentInsights.Items.Count.ToString()},
            { string.Join(separator, contentPrefix, reusableContentPrefix, scheduledPrefix, countIdentifier), reusableScheduledContentInsights.Items.Count.ToString()},
            { string.Join(separator, contentPrefix, websiteContentPrefix, inDraftPrefix, countIdentifier), websiteDraftContentInsights.Items.Count.ToString()},
            { string.Join(separator, contentPrefix, websiteContentPrefix, scheduledPrefix, countIdentifier), websiteScheduledContentInsights.Items.Count.ToString()},

            { string.Join(separator, allAccountsPrefix, countIdentifier), contactGroupInsights.AllCount.ToString() }
        };

        foreach (var contactGroup in contactGroupInsights.Groups)
        {
            resultInsights.Add(
                string.Join(separator, contactGroupPrefix, contactGroup.Name, countIdentifier), contactGroup.Count.ToString()
            );

            resultInsights.Add(
                string.Join(separator, contactGroupPrefix, contactGroup.Name, ratioToAllContactsIdentifier), ((decimal)contactGroup.Count / contactGroupInsights.AllCount).ToString()
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

        AddContentItemsToInsights(reusableDraftContentInsights.Items, reusableContentPrefix, inDraftPrefix);
        AddContentItemsToInsights(reusableScheduledContentInsights.Items, reusableContentPrefix, scheduledPrefix);
        AddContentItemsToInsights(websiteDraftContentInsights.Items, websiteContentPrefix, inDraftPrefix);
        AddContentItemsToInsights(websiteScheduledContentInsights.Items, websiteContentPrefix, scheduledPrefix);

        void AddEmailInsight(string statisticsParameter, string emailConfigurationName, string value) =>
            resultInsights.Add(
                string.Join(separator, emailPrefix, statisticsParameter, emailConfigurationName),
                value
            );

        void AddContentItemsToInsights(List<ContentItemInsightsModel> items, string contentTypePrefix, string releaseStatusPrefix)
        {
            foreach (var contentItem in items)
            {
                resultInsights.Add(
                    string.Join(separator, contentPrefix, contentTypePrefix, releaseStatusPrefix, contentItem.DisplayName),
                    ""
                );
            }
        }

        return resultInsights;
    }

    public AiraPromptGroupModel SaveAiraPrompts(int userId, List<string> suggestions)
    {
        var chatPromptGroup = new AiraChatPromptGroupInfo
        {
            AiraChatPromptGroupCreatedWhen = DateTime.Now,
            AiraChatPromptUserId = userId,
        };

        airaChatPromptGroupProvider.Set(chatPromptGroup);

        var messages = new List<AiraChatPromptInfo>();

        foreach (var suggestion in suggestions)
        {
            var prompt = new AiraChatPromptInfo
            {
                AiraChatPromptText = suggestion,
                AiraChatPromptChatPromptGroupId = chatPromptGroup.AiraChatPromptGroupId
            };

            airaChatPromptProvider.Set(prompt);

            messages.Add(prompt);
        }

        return new AiraPromptGroupModel
        {
            QuickPromptsGroupId = chatPromptGroup.AiraChatPromptGroupId,
            QuickPrompts = messages.Select(x => x.AiraChatPromptText)
        };
    }

    public void UpdateChatSummary(int userId, string summary)
    {
        var summaryInfo = airaChatSummaryProvider
            .Get()
            .WhereEquals(nameof(AiraChatSummaryInfo.AiraChatSummaryUserId), userId)
            .FirstOrDefault()
            ??
            new AiraChatSummaryInfo
            {
                AiraChatSummaryUserId = userId
            };

        summaryInfo.AiraChatSummaryContent = summary;

        airaChatSummaryProvider.Set(summaryInfo);
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
