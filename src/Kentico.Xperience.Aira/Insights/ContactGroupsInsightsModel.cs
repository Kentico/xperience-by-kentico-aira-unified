namespace Kentico.Xperience.Aira.Insights;

/// <summary>
/// Contact groups insights model.
/// </summary>
public class ContactGroupsInsightsModel
{
    /// <summary>
    /// Contact group insights model.
    /// </summary>
    public List<ContactGroupInsightsModel> Groups { get; set; } = [];

    /// <summary>
    /// Contact groups dynamic count.
    /// </summary>
    public int AllCount { get; set; }
}
