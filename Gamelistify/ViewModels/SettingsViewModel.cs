using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gamelistify.Models;

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

    public bool Saved { get; private set; }

    public void LoadFrom(AppSettings settings)
    {
        SkyscraperBinaryPath = settings.SkyscraperBinaryPath;
        ScreenScraperUser = settings.ScreenScraperUser;
        ScreenScraperPassword = settings.ScreenScraperPassword;
        ImagePreviewSize = settings.ImagePreviewSize;
    }

    public void ApplyTo(AppSettings settings)
    {
        settings.SkyscraperBinaryPath = SkyscraperBinaryPath.Trim();
        settings.ScreenScraperUser = ScreenScraperUser.Trim();
        settings.ScreenScraperPassword = ScreenScraperPassword;
        settings.ImagePreviewSize = ImagePreviewSize;
    }

    [RelayCommand]
    private void Save()
    {
        Saved = true;
    }
}
