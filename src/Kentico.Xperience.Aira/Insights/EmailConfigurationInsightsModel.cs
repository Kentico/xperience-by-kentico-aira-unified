namespace Kentico.Xperience.Aira.Insights
{
    public class EmailConfigurationInsightsModel
    {
        public int EmailId { get; set; }
        public string EmailName { get; set; } = string.Empty;
        public int ChannelId { get; set; }
        public string ChannelName { get; set; } = string.Empty;
    }
}
