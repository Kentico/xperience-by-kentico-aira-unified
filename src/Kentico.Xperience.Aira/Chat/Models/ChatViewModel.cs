﻿using Kentico.Xperience.Aira.NavBar;

namespace Kentico.Xperience.Aira.Chat.Models;

public class ChatViewModel
{
    public AiraPathsModel PathsModel { get; set; } = new AiraPathsModel();
    public NavBarViewModel NavBarViewModel { get; set; } = new NavBarViewModel();
    public int HistoryMessageCount { get; set; }
    public List<AiraChatMessage> History { get; set; } = [];
    public string InitialAiraMessage { get; set; } = string.Empty;
}
