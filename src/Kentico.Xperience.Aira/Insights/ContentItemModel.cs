using CMS.ContentEngine;

namespace Kentico.Xperience.Aira.Insights
{
    public class ContentItemModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public int ContentTypeId { get; set; }
        public string ContentTypeName { get; set; } = string.Empty;
        public VersionStatus VersionStatus { get; set; } = VersionStatus.InitialDraft;
        public int LanguageId { get; set; }
    }
}
