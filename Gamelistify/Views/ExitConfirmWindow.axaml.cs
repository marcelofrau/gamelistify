using Avalonia.Controls;
using Avalonia.Interactivity;
using Gamelistify.Services;
using Gamelistify.ViewModels;

namespace Gamelistify.Views;

public partial class ExitConfirmWindow : Window
{
    public ExitConfirmWindow()
    {
        InitializeComponent();
        Opened += (_, _) => Logger.Information("ExitConfirmWindow opened");
        Closed += (_, _) => Logger.Information("ExitConfirmWindow closed");
    }

    private void OnSaveAndExitClick(object? sender, RoutedEventArgs e)
    {
        Logger.Debug("Exit decision: save and exit");
        Close(ExitDecision.SaveAndExit);
    }

    private void OnDiscardClick(object? sender, RoutedEventArgs e)
    {
        Logger.Debug("Exit decision: discard");
        Close(ExitDecision.Discard);
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        Logger.Debug("Exit decision: cancel");
        Close(ExitDecision.Cancel);
    }
}
