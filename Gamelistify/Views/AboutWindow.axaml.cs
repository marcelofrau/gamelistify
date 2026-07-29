using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Gamelistify.Helpers;
using Gamelistify.Services;
using Gamelistify.ViewModels;

namespace Gamelistify.Views;

public partial class AboutWindow : Window
{
    public AboutWindow()
    {
        InitializeComponent();
        DataContext ??= new AboutViewModel();
        Opened += (_, _) => Logger.Information("AboutWindow opened");
        Closed += (_, _) => Logger.Information("AboutWindow closed");
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        Logger.Debug("AboutWindow close button clicked");
        Close();
    }

    private static void OpenUrl(string url)
    {
        Logger.Information("Opening external URL {Url}", url);
        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
    }

    private void OnRepositoryClick(object? sender, RoutedEventArgs e)
    {
        OpenUrl(ProjectInfo.RepositoryUrl);
    }

    private void OnIssuesClick(object? sender, RoutedEventArgs e)
    {
        OpenUrl(ProjectInfo.IssueTrackerUrl);
    }
}
