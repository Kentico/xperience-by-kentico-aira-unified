using Kentico.Membership;

namespace Kentico.Xperience.Aira.Insights;

/// <summary>
/// Service providing aira insights.
/// </summary>
public interface IAiraInsightsService
{
    /// <summary>
    /// Gets content items insights.
    /// </summary>
    /// <param name="contentType"><see cref="ContentType"/>Reusable or website content type.</param>
    /// <param name="user">Admin application user.</param>
    /// <param name="status">Status of the content type.</param>
    /// <returns><see cref="ContentInsightsModel"/></returns>
    Task<ContentInsightsModel> GetContentInsights(ContentType contentType, AdminApplicationUser user, string? status = null);

    /// <summary>
    /// Gets email insights.
    /// </summary>
    /// <param name="user">Admin application user.</param>
    /// <returns><see cref="EmailInsightsModel"/></returns>
    Task<EmailInsightsModel> GetEmailInsights(AdminApplicationUser user);

    /// <summary>
    /// Gets contact groups insights.
    /// </summary>
    /// <param name="names">Names of the contact groups.</param>
    /// <returns><see cref="ContactGroupsInsightsModel"/></returns>
    ContactGroupsInsightsModel GetContactGroupInsights(string[] names);
}
