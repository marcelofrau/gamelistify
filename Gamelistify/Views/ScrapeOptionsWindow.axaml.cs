using Avalonia.Controls;
using Avalonia.Interactivity;
using Gamelistify.Services;
using Gamelistify.ViewModels;

namespace Gamelistify.Views;

public partial class ScrapeOptionsWindow : Window
{
    public ScrapeOptionsWindow()
    {
        InitializeComponent();
        Opened += (_, _) => Logger.Information("ScrapeOptionsWindow opened");
        Closed += (_, _) => Logger.Information("ScrapeOptionsWindow closed");
    }

    private void OnStartClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is ScrapeOptionsViewModel viewModel && viewModel.Accepted)
        {
            Logger.Information("ScrapeOptionsWindow accepted. Platform {Platform}", viewModel.SelectedPlatform);
            Close(true);
        }
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        Logger.Information("ScrapeOptionsWindow cancelled");
        Close(false);
    }
}
