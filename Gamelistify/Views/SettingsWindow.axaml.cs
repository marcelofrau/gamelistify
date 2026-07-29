using Avalonia.Controls;
using Avalonia.Interactivity;
using Gamelistify.Services;
using Gamelistify.ViewModels;

namespace Gamelistify.Views;

public partial class SettingsWindow : Window
{
    public SettingsWindow()
    {
        InitializeComponent();
        Opened += (_, _) => Logger.Information("SettingsWindow opened");
        Closed += (_, _) => Logger.Information("SettingsWindow closed");
    }

    private void OnSaveClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is SettingsViewModel viewModel && viewModel.Saved)
        {
            Logger.Information("SettingsWindow confirmed save");
            Close(true);
        }
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        Logger.Information("SettingsWindow cancelled");
        Close(false);
    }
}
