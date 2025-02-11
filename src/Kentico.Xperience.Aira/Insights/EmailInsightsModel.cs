namespace Kentico.Xperience.Aira.Insights;

/// <summary>
/// Email insights model.
/// </summary>
public class EmailInsightsModel
{
    /// <summary>
    /// Email configuration insights models.
    /// </summary>
    public List<EmailConfigurationInsightsModel> Emails { get; set; } = [];

    /// <summary>
    /// Number of emails sent.
    /// </summary>
    public int EmailsSent { get; set; }

    /// <summary>
    /// Number of emails delivered.
    /// </summary>
    public int EmailsDelivered { get; set; }

    /// <summary>
    /// Number of emails opened.
    /// </summary>
    public int EmailsOpened { get; set; }

    /// <summary>
    /// Number of links clicked.
    /// </summary>
    public int LinksClicked { get; set; }

    /// <summary>
    /// Unsubscribe rate.
    /// </summary>
    public decimal UnsubscribeRate { get; set; }

    /// <summary>
    /// Number of spam reports.
    /// </summary>
    public int SpamReports { get; set; }
}
