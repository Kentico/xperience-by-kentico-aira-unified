using Kentico.Xperience.Aira.Chat.Models;
using Kentico.Xperience.Aira.NavBar;

namespace Kentico.Xperience.Aira.AssetUploader.Models;

public class AssetsViewModel
{
    public AiraPathsModel PathsModel { get; set; } = new AiraPathsModel();
    public NavBarViewModel NavBarViewModel { get; set; } = new NavBarViewModel();
}
