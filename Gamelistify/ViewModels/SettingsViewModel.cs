using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gamelistify.Models;
using Gamelistify.Services;
using Serilog.Events;

namespace Gamelistify.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _skyscraperBinaryPath = string.Empty;

    [ObservableProperty]
    private string _screenScraperUser = string.Empty;

    [ObservableProperty]
    private string _screenScraperPassword = string.Empty;

    [ObservableProperty]
    private int _imagePreviewSize = 220;

    [ObservableProperty]
    private int _selectedLogLevelIndex;

    public string[] LogLevelOptions { get; } =
    [
        "Verbose",
        "Debug",
        "Information",
        "Warning",
        "Error"
    ];

    public bool Saved { get; private set; }

    public void LoadFrom(AppSettings settings)
    {
        SkyscraperBinaryPath = settings.SkyscraperBinaryPath;
        ScreenScraperUser = settings.ScreenScraperUser;
        ScreenScraperPassword = settings.ScreenScraperPassword;
        ImagePreviewSize = settings.ImagePreviewSize;

        var current = Logger.CurrentLevelName;
        for (int i = 0; i < LogLevelOptions.Length; i++)
        {
            if (LogLevelOptions[i].Equals(current, StringComparison.OrdinalIgnoreCase))
            {
                SelectedLogLevelIndex = i;
                break;
            }
        }
    }

    public void ApplyTo(AppSettings settings)
    {
        settings.SkyscraperBinaryPath = SkyscraperBinaryPath.Trim();
        settings.ScreenScraperUser = ScreenScraperUser.Trim();
        settings.ScreenScraperPassword = ScreenScraperPassword;
        settings.ImagePreviewSize = ImagePreviewSize;
        settings.LogLevel = LogLevelOptions[SelectedLogLevelIndex];
    }

    [RelayCommand]
    private void Save()
    {
        Logger.Information("Settings save requested. LogLevel={Level}, SkyscraperBinary={Binary}, ScreenScraperUser={User}, ImagePreviewSize={Size}",
            LogLevelOptions[SelectedLogLevelIndex],
            string.IsNullOrWhiteSpace(SkyscraperBinaryPath) ? "(none)" : SkyscraperBinaryPath,
            string.IsNullOrWhiteSpace(ScreenScraperUser) ? "(none)" : ScreenScraperUser,
            ImagePreviewSize);
        var levelName = LogLevelOptions[SelectedLogLevelIndex];
        if (Enum.TryParse<LogEventLevel>(levelName, out var level))
            Logger.SetMinimumLevel(level);
        Saved = true;
    }
}
