namespace Kentico.Xperience.Aira.Insights
{
    public class EmailInsightsModel
    {
        public List<EmailConfigurationInsightsModel> Emails { get; set; } = [];
        public int EmailsSent { get; set; }
        public int EmailsDelivered { get; set; }
        public int EmailsOpened { get; set; }
        public int LinksClicked { get; set; }
        public decimal UnsubscribeRate { get; set; }
        public int SpamReports { get; set; }
    }
}
