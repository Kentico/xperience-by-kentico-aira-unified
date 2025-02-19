namespace Kentico.Xperience.AiraUnified.Admin;

/// <summary>
/// Installs Aira unified data classes(Info objects) to Database.
/// </summary>
internal interface IAiraUnifiedModuleInstaller
{
    /// <summary>
    /// Performs the Database installation of the tables defined by data classes(Info objects).
    /// </summary>
    void Install();
}
