using Avalonia.Controls;
using Avalonia.Interactivity;
using Gamelistify.Services;
using Gamelistify.ViewModels;

namespace Gamelistify.Views;

public partial class HygienePlanWindow : Window
{
    public HygienePlanWindow()
    {
        InitializeComponent();
        Opened += (_, _) => Logger.Information("HygienePlanWindow opened");
        Closed += (_, _) => Logger.Information("HygienePlanWindow closed");
    }

    private void OnApplyClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is HygienePlanViewModel viewModel && viewModel.Accepted)
        {
            Logger.Information("HygienePlanWindow accepted: {Title}", viewModel.Title);
            Close(true);
        }
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        Logger.Information("HygienePlanWindow cancelled");
        Close(false);
    }
}
