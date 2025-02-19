namespace Kentico.Xperience.AiraUnified.Insights;

/// <summary>
/// Email configuration insights model.
/// </summary>
public class EmailConfigurationInsightsModel
{
    /// <summary>
    /// Email id.
    /// </summary>
    public int EmailId { get; set; }

    /// <summary>
    /// Email name.
    /// </summary>
    public string EmailName { get; set; } = string.Empty;

    /// <summary>
    /// Channel id.
    /// </summary>
    public int ChannelId { get; set; }

    /// <summary>
    /// Channel name.
    /// </summary>
    public string ChannelName { get; set; } = string.Empty;

    /// <summary>
    /// Content type id.
    /// </summary>
    public int ContentTypeId { get; set; }

    /// <summary>
    /// Content type name.
    /// </summary>
    public string ContentTypeName { get; set; } = string.Empty;

}
