namespace Kentico.Xperience.Aira.Admin;

/// <summary>
/// Installs Aira's data classes(Info objects) to Database.
/// </summary>
internal interface IAiraModuleInstaller
{
    /// <summary>
    /// Performs the Database installation of the tables defined by data classes(Info objects).
    /// </summary>
    void Install();
}
