namespace Kentico.Xperience.AiraUnified.NavBar;

/// <summary>
/// View model for the Navigation.
/// </summary>
public class NavBarViewModel
{
    /// <summary>
    /// Path of the logo displayed in the PWA.
    /// </summary>
    public string LogoImgRelativePath { get; set; } = string.Empty;

    /// <summary>
    /// Currently displayed title.
    /// </summary>
    public string TitleText { get; set; } = string.Empty;

    /// <summary>
    /// Currently displayed image.
    /// </summary>
    public string TitleImagePath { get; set; } = string.Empty;

    /// <summary>
    /// Chat menu item model.
    /// </summary>
    public MenuItemModel ChatItem { get; set; } = new MenuItemModel();

    /// <summary>
    /// Smart upload menu item model.
    /// </summary>
    public MenuItemModel SmartUploadItem { get; set; } = new MenuItemModel();

    /// <summary>
    /// The message displayed bellow the navigation items in the menu.
    /// </summary>
    public string MenuMessage { get; set; } = string.Empty;
}
