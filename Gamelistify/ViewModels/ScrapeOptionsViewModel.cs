using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gamelistify.Services;

namespace Gamelistify.ViewModels;

public partial class ScrapeOptionsViewModel : ViewModelBase
{
    public ScrapeOptionsViewModel(string title)
    {
        Title = title;
        foreach (var platform in MetadataDefinitions.ScraperPlatforms)
            Platforms.Add(platform);

        SelectedPlatform = Platforms.FirstOrDefault() ?? string.Empty;
    }

    public string Title { get; }

    public ObservableCollection<string> Platforms { get; } = [];

    [ObservableProperty]
    private string _selectedPlatform = string.Empty;

    partial void OnSelectedPlatformChanged(string value)
    {
        Logger.Debug("ScrapeOptions platform selected: {Platform}", value);
    }

    [ObservableProperty]
    private string _extraArguments = "--flags unattend";

    public bool Accepted { get; private set; }

    [RelayCommand]
    private void Accept()
    {
        Accepted = true;
    }
}
