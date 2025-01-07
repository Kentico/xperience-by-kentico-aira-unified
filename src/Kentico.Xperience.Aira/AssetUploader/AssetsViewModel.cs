using Kentico.Xperience.Aira.Chat.Models;
using Kentico.Xperience.Aira.NavBar;

namespace Kentico.Xperience.Aira.AssetUploader.Models;

public class AssetsViewModel
{
    public string PathBase { get; set; } = string.Empty;
    public NavBarViewModel NavBarViewModel { get; set; } = new NavBarViewModel();
}
