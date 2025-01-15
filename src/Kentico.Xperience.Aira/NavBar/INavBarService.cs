using Kentico.Content.Web.Mvc;

namespace Kentico.Xperience.Aira.NavBar;

public interface INavBarService
{
    Task<NavBarViewModel> GetNavBarViewModel(string activePage);
    IMediaFileUrl? GetMediaFileUrl(string identifier);
}
