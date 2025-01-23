using Kentico.Membership;

namespace Kentico.Xperience.Aira.Insights
{
    public interface IAiraInsightsService
    {
        Task<ContentInsightsModel> GetContentInsights(ContentType contentType, AdminApplicationUser user, string? status = null);
        EmailInsightsModel GetEmailInsights();
        ContactGroupInsightsModel GetContactGroupInsights();
    }
}
