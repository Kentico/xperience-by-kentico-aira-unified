﻿using Kentico.Xperience.Aira.NavBar;

namespace Kentico.Xperience.Aira.AssetUploader.Models;

/// <summary>
/// View model for the Smart upload page.
/// </summary>
public class AssetsViewModel
{
    /// <summary>
    /// Path base for the Smart upload page.
    /// </summary>
    public string PathBase { get; set; } = string.Empty;

    /// <summary>
    /// View model for the navigation.
    /// </summary>
    public NavBarViewModel NavBarViewModel { get; set; } = new NavBarViewModel();

    /// <summary>
    /// Allowed file extensions.
    /// </summary>
    public List<string> AllowedFileExtensions { get; set; } = [];
}
