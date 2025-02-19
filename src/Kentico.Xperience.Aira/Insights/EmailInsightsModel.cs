namespace Kentico.Xperience.Aira.Insights;

/// <summary>
/// Email insights model.
/// </summary>
public class EmailInsightsModel
{
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

    /// <summary>
    /// The name of the email configuration.
    /// </summary>
    public string EmailConfigurationName { get; set; } = string.Empty;
}
