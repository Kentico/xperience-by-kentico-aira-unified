using CMS.ContactManagement;
using CMS.ContentEngine;
using CMS.ContentEngine.Internal;
using CMS.ContentWorkflowEngine;
using CMS.DataEngine;
using CMS.EmailLibrary;
using CMS.Websites;

namespace Kentico.Xperience.Aira.Insights;

internal class AiraInsightsService : IAiraInsightsService
{
    private readonly IContentItemManagerFactory contentItemManagerFactory;
    private readonly IContentQueryExecutor contentQueryExecutor;
    private readonly IInfoProvider<ContentLanguageInfo> contentLanguageInfoProvider;
    private readonly IInfoProvider<ContactGroupInfo> contactGroupInfoProvider;
    private readonly IInfoProvider<ContactGroupMemberInfo> contactGroupMemberInfoProvider;
    private readonly IInfoProvider<ContentItemLanguageMetadataInfo> contentItemLanguageMetadataInfoProvider;
    private readonly IInfoProvider<ContentWorkflowStepInfo> contentWorkflowStepInfoProvider;
    private readonly IInfoProvider<EmailStatisticsInfo> emailStatisticsInfoProvider;
    private readonly IInfoProvider<EmailConfigurationInfo> emailConfigurationInfoProvider;
    private readonly IInfoProvider<ContactInfo> contactInfoProvider;

    private string[]? reusableTypes = null;
    private string[]? pageTypes = null;
    private string[]? emailTypes = null;

    public AiraInsightsService(
        IContentItemManagerFactory contentItemManagerFactory,
        IContentQueryExecutor contentQueryExecutor,
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
        this.contentLanguageInfoProvider = contentLanguageInfoProvider;
        this.contactGroupInfoProvider = contactGroupInfoProvider;
        this.contactGroupMemberInfoProvider = contactGroupMemberInfoProvider;
        this.contentItemLanguageMetadataInfoProvider = contentItemLanguageMetadataInfoProvider;
        this.contentWorkflowStepInfoProvider = contentWorkflowStepInfoProvider;
        this.emailStatisticsInfoProvider = emailStatisticsInfoProvider;
        this.emailConfigurationInfoProvider = emailConfigurationInfoProvider;
        this.contactInfoProvider = contactInfoProvider;
    }

    public async Task<ContentInsightsModel> GetContentInsights(ContentType contentType, int userId, string? status = null)
    {
        var content = await GetContent(userId, contentType.ToString(), status);

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

    public async Task<List<EmailInsightsModel>> GetEmailInsights()
    {
        var statistics = await emailStatisticsInfoProvider.Get().GetEnumerableTypedResultAsync();

        var regularEmails = await emailConfigurationInfoProvider
            .Get()
            .WhereEquals(nameof(EmailConfigurationInfo.EmailConfigurationPurpose), "Regular")
            .GetEnumerableTypedResultAsync();

        var emailsInsights = new List<EmailInsightsModel>();

        foreach (var email in regularEmails)
        {
            var stats = statistics.FirstOrDefault(s => s.EmailStatisticsEmailConfigurationID == email.EmailConfigurationID);

            if (stats != null)
            {
                emailsInsights.Add(new EmailInsightsModel
                {
                    EmailsSent = stats.EmailStatisticsTotalSent,
                    EmailsDelivered = stats.EmailStatisticsEmailsDelivered,
                    EmailsOpened = stats.EmailStatisticsEmailOpens,
                    LinksClicked = stats.EmailStatisticsEmailUniqueClicks,
                    UnsubscribeRate = stats.EmailStatisticsUniqueUnsubscribes,
                    SpamReports = stats.EmailStatisticsSpamReports ?? 0,
                    EmailConfigurationName = email.EmailConfigurationName
                });
            }
        }

        return emailsInsights;
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

    private async Task<IEnumerable<ContentItemModel>> GetContent(int userId, string classType = "Reusable", string? status = null)
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
                        "Draft" => FilterDrafts(items),
                        "Scheduled" => await FilterScheduled(userId, items),
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

    private async Task<IEnumerable<ContentItemModel>> FilterScheduled(int userId, IEnumerable<ContentItemModel> items)
    {
        List<ContentItemModel> result = [];

        var contentItemManager = contentItemManagerFactory.Create(userId);
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

    private static IEnumerable<ContentItemModel> FilterDrafts(IEnumerable<ContentItemModel> items)
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

    private string[] ReusableTypes
    {
        get
        {
            reusableTypes ??= DataClassInfoProvider.GetClasses().Where(nameof(DataClassInfo.ClassContentTypeType), QueryOperator.Equals, "Reusable").Select(c => c.ClassName).ToArray();
            return reusableTypes;
        }
    }

    private string[] PageTypes
    {
        get
        {
            pageTypes ??= DataClassInfoProvider.GetClasses().Where(nameof(DataClassInfo.ClassContentTypeType), QueryOperator.Equals, "Website").Select(c => c.ClassName).ToArray();
            return pageTypes;
        }
    }

    private string[] EmailTypes
    {
        get
        {
            emailTypes ??= DataClassInfoProvider.GetClasses().Where(nameof(DataClassInfo.ClassContentTypeType), QueryOperator.Equals, "Email").Select(c => c.ClassName).ToArray();
            return emailTypes;
        }
    }
}
