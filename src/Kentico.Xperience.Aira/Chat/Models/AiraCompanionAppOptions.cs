namespace Kentico.Xperience.Aira.Chat.Models;

/// <summary>
/// The options used to setup this integration stored in the appsettings.json file.
/// </summary>
public sealed class AiraCompanionAppOptions
{
    /// <summary>
    /// The aira service subscription key.
    /// </summary>
    public string AiraApiSubscriptionKey { get; set; } = string.Empty;
}
