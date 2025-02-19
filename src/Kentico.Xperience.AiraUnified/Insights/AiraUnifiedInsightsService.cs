using CMS.ContactManagement;
using CMS.ContentEngine;
using CMS.ContentEngine.Internal;
using CMS.ContentWorkflowEngine;
using CMS.DataEngine;
using CMS.EmailLibrary;
using CMS.Websites;

namespace Kentico.Xperience.AiraUnified.Insights;

internal class AiraUnifiedInsightsService : IAiraUnifiedInsightsService
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

    private string[]? reusableTypes;
    private string[]? emailTypes;
    private string[]? pageTypes;
    private string[] ReusableTypes => reusableTypes ??= GetReusableTypes();
    private string[] PageTypes => pageTypes ??= GetPageTypes();
    private string[] EmailTypes => emailTypes ??= GetEmailTypes();

    private const string CONTENT_ITEM_EMAIL_TYPE = "Email";
    private const string CONTENT_ITEM_WEBSITE_TYPE = "Website";
    private const string CONTENT_ITEM_REUSABLE_TYPE = "Reusable";

    private const string SCHEDULED_IDENTIFIER = "Scheduled";
    private const string DRAFT_IDENTIFIER = "Draft";

    public AiraUnifiedInsightsService(
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

    private async Task<IEnumerable<ContentItemModel>> GetContent(int userId, string classType = CONTENT_ITEM_REUSABLE_TYPE, string? status = null)
    {
        var builder = classType switch
        {
            CONTENT_ITEM_EMAIL_TYPE => GetContentItemBuilder(EmailTypes),
            CONTENT_ITEM_WEBSITE_TYPE => GetContentItemBuilder(PageTypes),
            _ => GetContentItemBuilder(ReusableTypes),
        };

        var options = new ContentQueryExecutionOptions
        {
            ForPreview = true,
            IncludeSecuredItems = true
        };

        if (builder is null)
        {
            return [];
        }

        if (status == DRAFT_IDENTIFIER)
        {
            builder.Parameters(q => q.Where(w => w
                .WhereEquals(nameof(ContentItemCommonDataInfo.ContentItemCommonDataVersionStatus), VersionStatus.Draft)
                .Or()
                .WhereEquals(nameof(ContentItemCommonDataInfo.ContentItemCommonDataVersionStatus), VersionStatus.InitialDraft)));

            return await contentQueryExecutor.GetResult(builder, ContentItemBinder, options);
        }

        var items = await contentQueryExecutor.GetResult(builder, ContentItemBinder, options);

        if (string.IsNullOrEmpty(status))
        {
            return items;
        }

        return status switch
        {
            DRAFT_IDENTIFIER => FilterDrafts(items),
            SCHEDULED_IDENTIFIER => await FilterScheduled(userId, items),
            _ => await FilterCustomWorkflowStep(items, status),
        };
    }

    private IEnumerable<ContactGroupInfo> GetContactGroups(string[] names)
    {
        List<ContactGroupInfo> result = [];

        if (names is not null && names.Length > 0)
        {
            foreach (var name in names)
            {
                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }
                var group = contactGroupInfoProvider.Get().Where(g => g.ContactGroupDisplayName == name).FirstOrDefault();
                if (group is not null)
                {
                    result.Add(group);
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

        if (step is not null)
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
            .Where(nameof(DataClassInfo.ClassContentTypeType), QueryOperator.Equals, CONTENT_ITEM_REUSABLE_TYPE)
            .Select(c => c.ClassName)
            .ToArray();

    private static string[] GetPageTypes() =>
        DataClassInfoProvider.GetClasses()
            .Where(nameof(DataClassInfo.ClassContentTypeType), QueryOperator.Equals, CONTENT_ITEM_WEBSITE_TYPE)
            .Select(c => c.ClassName)
            .ToArray();



    private static string[] GetEmailTypes() =>
        DataClassInfoProvider.GetClasses()
            .Where(nameof(DataClassInfo.ClassContentTypeType), QueryOperator.Equals, CONTENT_ITEM_EMAIL_TYPE)
            .Select(c => c.ClassName)
            .ToArray();
}
