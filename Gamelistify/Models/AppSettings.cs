namespace Gamelistify.Models;

public sealed class AppSettings
{
    public string SkyscraperBinaryPath { get; set; } = string.Empty;

    public string ScreenScraperUser { get; set; } = string.Empty;

    public string ScreenScraperPassword { get; set; } = string.Empty;

    public string LastGamelistDirectory { get; set; } = string.Empty;

    public string Theme { get; set; } = "dark";

    public int ImagePreviewSize { get; set; } = 220;

    public string LogLevel { get; set; } = "Verbose";

    public List<string> RecentFiles { get; set; } = [];

    public ColumnVisibilitySettings ColumnsVisible { get; set; } = new();
}
