namespace Gamelistify.Helpers;

public static class AppPaths
{
    public static string? AppDataDirectoryOverride { get; set; }

    public static string AppDataDirectory => AppDataDirectoryOverride ?? Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        ProjectInfo.ProjectName);

    public static string SettingsPath => Path.Combine(AppDataDirectory, "settings.json");
}
