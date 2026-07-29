namespace Gamelistify.Helpers;

public static class AppPaths
{
    public static string AppDataDirectory => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        ProjectInfo.ProjectName);

    public static string SettingsPath => Path.Combine(AppDataDirectory, "settings.json");
}
