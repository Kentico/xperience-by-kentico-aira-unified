using CMS.ContentEngine;

namespace Kentico.Xperience.AiraUnified.Insights;

/// <summary>
/// Content item model.
/// </summary>
public class ContentItemModel
{
    /// <summary>
    /// Content item id.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Content item name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Content item display name.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Content type id.
    /// </summary>
    public int ContentTypeId { get; set; }

    /// <summary>
    /// Content type name.
    /// </summary>
    public string ContentTypeName { get; set; } = string.Empty;

    /// <summary>
    /// Version status.
    /// </summary>
    public VersionStatus VersionStatus { get; set; } = VersionStatus.InitialDraft;

    /// <summary>
    /// Language id.
    /// </summary>
    public int LanguageId { get; set; }
}
