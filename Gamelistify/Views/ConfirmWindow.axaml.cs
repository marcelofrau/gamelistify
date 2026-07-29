using Avalonia.Controls;
using Avalonia.Interactivity;
using Gamelistify.Services;
using Gamelistify.ViewModels;

namespace Gamelistify.Views;

public partial class ConfirmWindow : Window
{
    public ConfirmWindow()
    {
        InitializeComponent();
        Opened += (_, _) => Logger.Information("ConfirmWindow opened: {Title}", Title ?? "<none>");
        Closed += (_, _) => Logger.Information("ConfirmWindow closed");
    }

    private void OnYesClick(object? sender, RoutedEventArgs e)
    {
        Logger.Debug("ConfirmWindow accepted");
        Close(true);
    }

    private void OnNoClick(object? sender, RoutedEventArgs e)
    {
        Logger.Debug("ConfirmWindow rejected");
        Close(false);
    }
}
