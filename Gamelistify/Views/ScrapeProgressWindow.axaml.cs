using Avalonia.Controls;
using Avalonia.Interactivity;
using Gamelistify.Services;

namespace Gamelistify.Views;

public partial class ScrapeProgressWindow : Window
{
    public ScrapeProgressWindow()
    {
        InitializeComponent();
        Opened += (_, _) => Logger.Information("ScrapeProgressWindow opened");
        Closed += (_, _) => Logger.Information("ScrapeProgressWindow closed");
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        Logger.Information("ScrapeProgressWindow close requested");
        Close();
    }
}
