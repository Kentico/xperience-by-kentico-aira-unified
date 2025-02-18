﻿using CMS.ContactManagement;
using CMS.ContentEngine;
using CMS.ContentEngine.Internal;
using CMS.ContentWorkflowEngine;
using CMS.DataEngine;
using CMS.EmailLibrary;
using CMS.Websites;

using Kentico.Membership;

namespace Kentico.Xperience.Aira.Insights;

internal class AiraInsightsService : IAiraInsightsService
{
    private readonly IContentItemManagerFactory contentItemManagerFactory;
    private readonly IContentQueryExecutor contentQueryExecutor;
    private readonly IInfoProvider<ChannelInfo> channelInfoProvider;
    private readonly IInfoProvider<ContentLanguageInfo> contentLanguageInfoProvider;
    private readonly IInfoProvider<ContactGroupInfo> contactGroupInfoProvider;
    private readonly IInfoProvider<ContactGroupMemberInfo> contactGroupMemberInfoProvider;
    private readonly IInfoProvider<ContentItemLanguageMetadataInfo> contentItemLanguageMetadataInfoProvider;
    private readonly IInfoProvider<ContentWorkflowStepInfo> contentWorkflowStepInfoProvider;
    private readonly IInfoProvider<EmailStatisticsInfo> emailStatisticsInfoProvider;
    private readonly IInfoProvider<EmailConfigurationInfo> emailConfigurationInfoProvider;
    private readonly IInfoProvider<ContactInfo> contactInfoProvider;

    private string[]? reusableTypes;
    private string[]? emailTypes;
    private string[]? pageTypes;
    private string[] ReusableTypes => reusableTypes ??= GetReusableTypes();
    private string[] PageTypes => pageTypes ??= GetPageTypes();
    private string[] EmailTypes => emailTypes ??= GetEmailTypes();

    public AiraInsightsService(
        IContentItemManagerFactory contentItemManagerFactory,
        IContentQueryExecutor contentQueryExecutor,
        IInfoProvider<ChannelInfo> channelInfoProvider,
        IInfoProvider<ContentLanguageInfo> contentLanguageInfoProvider,
        IInfoProvider<ContactGroupInfo> contactGroupInfoProvider,
        IInfoProvider<ContactGroupMemberInfo> contactGroupMemberInfoProvider,
        IInfoProvider<ContentItemLanguageMetadataInfo> contentItemLanguageMetadataInfoProvider,
        IInfoProvider<ContentWorkflowStepInfo> contentWorkflowStepInfoProvider,
        IInfoProvider<EmailStatisticsInfo> emailStatisticsInfoProvider,
        IInfoProvider<EmailConfigurationInfo> emailConfigurationInfoProvider,
        IInfoProvider<ContactInfo> contactInfoProvider)
    {
        this.contentItemManagerFactory = contentItemManagerFactory;
        this.contentQueryExecutor = contentQueryExecutor;
        this.channelInfoProvider = channelInfoProvider;
        this.contentLanguageInfoProvider = contentLanguageInfoProvider;
        this.contactGroupInfoProvider = contactGroupInfoProvider;
        this.contactGroupMemberInfoProvider = contactGroupMemberInfoProvider;
        this.contentItemLanguageMetadataInfoProvider = contentItemLanguageMetadataInfoProvider;
        this.contentWorkflowStepInfoProvider = contentWorkflowStepInfoProvider;
        this.emailStatisticsInfoProvider = emailStatisticsInfoProvider;
        this.emailConfigurationInfoProvider = emailConfigurationInfoProvider;
        this.contactInfoProvider = contactInfoProvider;
    }

    public async Task<ContentInsightsModel> GetContentInsights(ContentType contentType, AdminApplicationUser user, string? status = null)
    {
        var content = await GetContent(user, contentType.ToString(), status);

        var items = new List<ContentItemInsightsModel>();

        foreach (var contentItem in content)
        {
            items.Add(new ContentItemInsightsModel
            {
                Id = contentItem.Id,
                DisplayName = contentItem.Name
            });
        }

        return new ContentInsightsModel
        {
            Items = items
        };
    }

    public async Task<EmailInsightsModel> GetEmailInsights(AdminApplicationUser user)
    {
        var channels = channelInfoProvider.Get().ToList();
        var statistics = emailStatisticsInfoProvider.Get().ToList();
        var items = await GetContent(user, "Email");

        var regularEmails = emailConfigurationInfoProvider.Get().Where(c => c.WhereEquals("EmailConfigurationPurpose", "Regular")).ToList();

        var sent = 0;
        var delivered = 0;
        var opened = 0;
        var clicked = 0;
        var unsubscribed = 0;
        var spam = 0;

        var emails = new List<EmailConfigurationInsightsModel>();

        foreach (var email in regularEmails)
        {
            var channel = channels.FirstOrDefault(ch => ch.ChannelID == email.EmailConfigurationEmailChannelID);
            var item = items.FirstOrDefault(i => i.Id == email.EmailConfigurationContentItemID);

            emails.Add(new EmailConfigurationInsightsModel
            {
                EmailId = email.EmailConfigurationID,
                EmailName = email.EmailConfigurationName,
                ChannelId = email.EmailConfigurationEmailChannelID,
                ChannelName = channel?.ChannelName ?? "",
                ContentTypeId = item?.ContentTypeId ?? 0,
                ContentTypeName = item?.ContentTypeName ?? ""
            });

            var stats = statistics.FirstOrDefault(s => s.EmailStatisticsEmailConfigurationID == email.EmailConfigurationID);

            if (stats != null)
            {
                sent += stats.EmailStatisticsTotalSent;
                delivered += stats.EmailStatisticsEmailsDelivered;
                opened += stats.EmailStatisticsEmailOpens;
                clicked += stats.EmailStatisticsEmailUniqueClicks;
                unsubscribed += stats.EmailStatisticsUniqueUnsubscribes;
                spam += stats.EmailStatisticsSpamReports ?? 0;
            }

        }

        return new EmailInsightsModel
        {
            Emails = emails,
            EmailsSent = sent,
            EmailsDelivered = delivered,
            EmailsOpened = opened,
            LinksClicked = clicked,
            UnsubscribeRate = unsubscribed,
            SpamReports = spam
        };
    }

    public ContactGroupsInsightsModel GetContactGroupInsights(string[] names)
    {
        var allCount = contactInfoProvider.Get().ToList().Count;
        var contactGroups = GetContactGroups(names);
        var groups = new List<ContactGroupInsightsModel>();

        foreach (var contactGroup in contactGroups)
        {
            groups.Add(new ContactGroupInsightsModel
            {
                Id = contactGroup.ContactGroupID,
                Name = contactGroup.ContactGroupDisplayName,
                Conditions = contactGroup.ContactGroupDynamicCondition,
                Count = contactGroupMemberInfoProvider.GetContactsInContactGroupCount(contactGroup.ContactGroupID)
            });
        }

        return new ContactGroupsInsightsModel
        {
            AllCount = allCount,
            Groups = groups
        };
    }

    private async Task<IEnumerable<ContentItemModel>> GetContent(AdminApplicationUser user, string classType = "Reusable", string? status = null)
    {
        var builder = classType switch
        {
            "Email" => GetContentItemBuilder(EmailTypes),
            "Website" => GetContentItemBuilder(PageTypes),
            _ => GetContentItemBuilder(ReusableTypes),
        };

        var options = new ContentQueryExecutionOptions
        {
            ForPreview = true,
            IncludeSecuredItems = true
        };

        if (builder != null)
        {
            if (status == "Draft")
            {
                builder.Parameters(q => q.Where(w => w
                    .WhereEquals(nameof(ContentItemCommonDataInfo.ContentItemCommonDataVersionStatus), VersionStatus.Draft)
                    .Or()
                    .WhereEquals(nameof(ContentItemCommonDataInfo.ContentItemCommonDataVersionStatus), VersionStatus.InitialDraft)));

                var items = await contentQueryExecutor.GetResult(builder, ContentItemBinder, options);
                return items;
            }
            else
            {
                var items = await contentQueryExecutor.GetResult(builder, ContentItemBinder, options);
                if (!string.IsNullOrEmpty(status))
                {
                    return status switch
                    {
                        "Draft" => await FilterDrafts(items),
                        "Scheduled" => await FilterScheduled(user, items),
                        _ => await FilterCustomWorkflowStep(items, status),
                    };
                }
                else
                {
                    return items;
                }
            }
        }

        return [];
    }

    private IEnumerable<ContactGroupInfo> GetContactGroups(string[] names)
    {
        List<ContactGroupInfo> result = [];

        if (names != null && names.Length > 0)
        {
            foreach (var name in names)
            {
                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }
                else
                {
                    var group = contactGroupInfoProvider.Get().Where(g => g.ContactGroupDisplayName == name).FirstOrDefault();
                    if (group != null)
                    {
                        result.Add(group);
                    }
                }
            }
        }

        return result;
    }

    private static ContentItemQueryBuilder? GetContentItemBuilder(string[] contentTypes)
    {
        var builder = new ContentItemQueryBuilder();

        return contentTypes.Length switch
        {
            0 => null,
            1 => builder.ForContentType(contentTypes[0]),
            _ => builder.ForContentTypes(q => q.OfContentType(contentTypes)),
        };
    }

    private async Task<IEnumerable<ContentItemModel>> FilterScheduled(AdminApplicationUser user, IEnumerable<ContentItemModel> items)
    {
        List<ContentItemModel> result = [];

        var contentItemManager = contentItemManagerFactory.Create(user.UserID);
        foreach (var item in items)
        {
            var language = await contentLanguageInfoProvider.GetAsync(item.LanguageId);
            var isScheduled = await contentItemManager.IsPublishScheduled(item.Id, language.ContentLanguageName);
            if (isScheduled)
            {
                result.Add(item);
            }
        }

        return result;
    }

    private static async Task<IEnumerable<ContentItemModel>> FilterDrafts(IEnumerable<ContentItemModel> items)
    {
        List<ContentItemModel> result = [];

        foreach (var item in items)
        {
            if (item.VersionStatus == VersionStatus.Draft)
            {
                result.Add(item);
            }
        }

        return result;
    }

    private async Task<IEnumerable<ContentItemModel>> FilterCustomWorkflowStep(IEnumerable<ContentItemModel> items, string? status)
    {
        List<ContentItemModel> result = [];

        var step = contentWorkflowStepInfoProvider.Get().WhereEquals(nameof(ContentWorkflowStepInfo.ContentWorkflowStepDisplayName), status).FirstOrDefault();

        if (step != null)
        {
            var languageMetadata = await contentItemLanguageMetadataInfoProvider
                .Get()
                .WhereEquals(nameof(ContentItemLanguageMetadataInfo.ContentItemLanguageMetadataContentWorkflowStepID), step.ContentWorkflowStepID)
                .GetEnumerableTypedResultAsync();

            result.AddRange(
                items.Where(item => languageMetadata
                    .Any(m =>
                        m.ContentItemLanguageMetadataContentItemID == item.Id &&
                        m.ContentItemLanguageMetadataContentLanguageID == item.LanguageId
                    )
                )
            );
        }
        return result;
    }

    private ContentItemModel ContentItemBinder(IContentQueryDataContainer container) => new()
    {
        Id = container.ContentItemID,
        Name = container.ContentItemName,
        DisplayName = container.ContentItemName,
        ContentTypeId = container.ContentItemContentTypeID,
        ContentTypeName = container.ContentTypeName,
        VersionStatus = container.ContentItemCommonDataVersionStatus,
        LanguageId = container.ContentItemCommonDataContentLanguageID,

    };

    private static string[] GetReusableTypes() =>
        DataClassInfoProvider.GetClasses()
            .Where(nameof(DataClassInfo.ClassContentTypeType), QueryOperator.Equals, "Reusable")
            .Select(c => c.ClassName)
            .ToArray();

    private static string[] GetPageTypes() =>
        DataClassInfoProvider.GetClasses()
            .Where(nameof(DataClassInfo.ClassContentTypeType), QueryOperator.Equals, "Website")
            .Select(c => c.ClassName)
            .ToArray();



    private static string[] GetEmailTypes() =>
        DataClassInfoProvider.GetClasses()
            .Where(nameof(DataClassInfo.ClassContentTypeType), QueryOperator.Equals, "Email")
            .Select(c => c.ClassName)
            .ToArray();
}
