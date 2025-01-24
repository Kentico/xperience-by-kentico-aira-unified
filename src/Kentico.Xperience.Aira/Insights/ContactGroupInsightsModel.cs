namespace Kentico.Xperience.Aira.Insights
{
    public class ContactGroupInsightsModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Conditions { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
