namespace Kentico.Xperience.Aira.NavBar;

public class NavBarViewModel
{
    public string LogoImgRelativePath { get; set; } = string.Empty;
    public string TitleText { get; set; } = string.Empty;
    public string TitleImagePath { get; set; } = string.Empty;
    public MenuItemModel ChatItem { get; set; } = new MenuItemModel();
    public MenuItemModel SmartUploadItem { get; set; } = new MenuItemModel();
}
