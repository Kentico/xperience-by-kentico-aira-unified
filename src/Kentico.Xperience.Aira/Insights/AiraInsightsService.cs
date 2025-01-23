using CMS.ContactManagement;
using CMS.ContentEngine;
using CMS.ContentEngine.Internal;
using CMS.ContentWorkflowEngine;
using CMS.DataEngine;
using CMS.EmailLibrary;
using CMS.Websites;

using Kentico.Membership;

namespace Kentico.Xperience.Aira.Insights
{
    public class AiraInsightsService : IAiraInsightsService
    {
        private readonly IContentItemManagerFactory contentItemManagerFactory;
        private readonly IContentQueryExecutor contentQueryExecutor;
        private readonly IInfoProvider<ChannelInfo> channelInfoProvider;
        private readonly IInfoProvider<ContentLanguageInfo> contentLanguageInfoProvider;
        private readonly IInfoProvider<ContactGroupInfo> contactGroupInfoProvider;
        private readonly IInfoProvider<ContentItemLanguageMetadataInfo> contentItemLanguageMetadataInfoProvider;
        private readonly IInfoProvider<ContentWorkflowStepInfo> contentWorkflowStepInfoProvider;
        private readonly IInfoProvider<EmailStatisticsInfo> emailStatisticsInfoProvider;
        private readonly IInfoProvider<EmailConfigurationInfo> emailConfigurationInfoProvider;

        private string[]? reusableTypes = null;
        private string[]? pageTypes = null;
        private string[]? emailTypes = null;

        public AiraInsightsService(
            IContentItemManagerFactory contentItemManagerFactory,
            IContentQueryExecutor contentQueryExecutor,
            IInfoProvider<ChannelInfo> channelInfoProvider,
            IInfoProvider<ContentLanguageInfo> contentLanguageInfoProvider,
            IInfoProvider<ContactGroupInfo> contactGroupInfoProvider,
            IInfoProvider<ContentItemLanguageMetadataInfo> contentItemLanguageMetadataInfoProvider,
            IInfoProvider<ContentWorkflowStepInfo> contentWorkflowStepInfoProvider,
            IInfoProvider<EmailStatisticsInfo> emailStatisticsInfoProvider,
            IInfoProvider<EmailConfigurationInfo> emailConfigurationInfoProvider)
        {
            this.contentItemManagerFactory = contentItemManagerFactory;
            this.contentQueryExecutor = contentQueryExecutor;
            this.channelInfoProvider = channelInfoProvider;
            this.contentLanguageInfoProvider = contentLanguageInfoProvider;
            this.contactGroupInfoProvider = contactGroupInfoProvider;
            this.contentItemLanguageMetadataInfoProvider = contentItemLanguageMetadataInfoProvider;
            this.contentWorkflowStepInfoProvider = contentWorkflowStepInfoProvider;
            this.emailStatisticsInfoProvider = emailStatisticsInfoProvider;
            this.emailConfigurationInfoProvider = emailConfigurationInfoProvider;
        }

        public async Task<ContentInsightsModel> GetContentInsights(ContentType contentType, AdminApplicationUser user, string? status = null)
        {
            var content = await GetContent(user, contentType.ToString(), status);

            var items = new List<ContentItemInsightsModel>();

            foreach (var contentItem in content)
            {
                items.Add(new ContentItemInsightsModel
                {
                    Id = contentItem.SystemFields.ContentItemID,
                    DisplayName = contentItem.SystemFields.ContentItemName
                });
            }

            return new ContentInsightsModel
            {
                Items = items
            };
        }

        public EmailInsightsModel GetEmailInsights()
        {
            var channels = channelInfoProvider.Get().ToList();
            var statistics = emailStatisticsInfoProvider.Get().ToList();
            var regularEmails = emailConfigurationInfoProvider.Get().Where(c => c.WhereEquals("EmailConfigurationPurpose", "Regular"));

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

                emails.Add(new EmailConfigurationInsightsModel
                {
                    EmailId = email.EmailConfigurationID,
                    EmailName = email.EmailConfigurationName,
                    ChannelId = email.EmailConfigurationEmailChannelID,
                    ChannelName = channel?.ChannelName ?? ""
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

        public ContactGroupInsightsModel GetContactGroupInsights()
        {
            throw new NotImplementedException();
        }

        private async Task<IEnumerable<IContentItemFieldsSource>> GetContent(AdminApplicationUser user, string classType = "Reusable", string? status = null)
        {
            var builder = classType switch
            {
                "Email" => GetContentItemBuilder(EmailTypes),
                "Website" => GetContentItemBuilder(PageTypes),
                _ => GetContentItemBuilder(ReusableTypes),
            };

            if (status == "Draft")
            {
                builder.Parameters(q => q.Where(w => w
                    .WhereEquals("ContentItemCommonDataVersionStatus", VersionStatus.Draft)
                    .Or()
                    .WhereEquals("ContentItemCommonDataVersionStatus", VersionStatus.InitialDraft)));

                var items = await contentQueryExecutor.GetMappedResult<IContentItemFieldsSource>(builder);
                return items;
            }
            else
            {
                var items = await contentQueryExecutor.GetMappedResult<IContentItemFieldsSource>(builder);
                var result = status switch
                {
                    "Draft" => await FilterDrafts(items),
                    "Scheduled" => await FilterScheduled(user, items),
                    _ => await FilterCustomWorkflowStep(items, status),
                };
                return result;
            }
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
                        var group = contactGroupInfoProvider.Get(name);
                        if (group != null)
                        {
                            result.Add(group);
                        }
                    }
                }
            }

            return result;
        }

        private ContentItemQueryBuilder GetContentItemBuilder(string[] contentTypes) => new ContentItemQueryBuilder()
                .ForContentTypes(q => q.OfContentType(contentTypes).ForWebsite());

        private async Task<IEnumerable<IContentItemFieldsSource>> FilterScheduled(AdminApplicationUser user, IEnumerable<IContentItemFieldsSource> items)
        {
            List<IContentItemFieldsSource> result = [];

            var contentItemManager = contentItemManagerFactory.Create(user.UserID);
            foreach (var item in items)
            {
                var language = await contentLanguageInfoProvider.GetAsync(item.SystemFields.ContentItemCommonDataContentLanguageID);
                var isScheduled = await contentItemManager.IsPublishScheduled(item.SystemFields.ContentItemID, language.ContentLanguageName);
                if (isScheduled)
                {
                    result.Add(item);
                }
            }

            return result;
        }

        private async Task<IEnumerable<IContentItemFieldsSource>> FilterDrafts(IEnumerable<IContentItemFieldsSource> items)
        {
            List<IContentItemFieldsSource> result = [];

            foreach (var item in items)
            {
                if (item.SystemFields.ContentItemCommonDataVersionStatus == VersionStatus.Draft)
                {
                    result.Add(item);
                }
            }

            return result;
        }

        private async Task<IEnumerable<IContentItemFieldsSource>> FilterCustomWorkflowStep(IEnumerable<IContentItemFieldsSource> items, string? status)
        {
            List<IContentItemFieldsSource> result = [];

            var step = contentWorkflowStepInfoProvider.Get().WhereEquals("ContentWorkflowStepDisplayName", status).FirstOrDefault();

            if (step != null)
            {
                var languageMetadata = contentItemLanguageMetadataInfoProvider.Get().WhereEquals("ContentItemLanguageMetadataContentWorkflowStepID", step.ContentWorkflowStepID).ToList();

                foreach (var item in items)
                {
                    if (languageMetadata.Any(m =>
                        m.ContentItemLanguageMetadataContentItemID == item.SystemFields.ContentItemID &&
                        m.ContentItemLanguageMetadataContentLanguageID == item.SystemFields.ContentItemCommonDataContentLanguageID))
                    {
                        result.Add(item);
                    }
                }
            }
            return result;
        }

        private string[] ReusableTypes
        {
            get
            {
                reusableTypes ??= DataClassInfoProvider.GetClasses().Where("ClassContentTypeType", QueryOperator.Equals, "Reusable").Select(c => c.ClassName).ToArray();
                return reusableTypes;
            }
        }

        private string[] PageTypes
        {
            get
            {
                pageTypes ??= DataClassInfoProvider.GetClasses().Where("ClassContentTypeType", QueryOperator.Equals, "Website").Select(c => c.ClassName).ToArray();
                return pageTypes;
            }
        }

        private string[] EmailTypes
        {
            get
            {
                emailTypes ??= DataClassInfoProvider.GetClasses().Where("ClassContentTypeType", QueryOperator.Equals, "Email").Select(c => c.ClassName).ToArray();
                return emailTypes;
            }
        }
    }
}
