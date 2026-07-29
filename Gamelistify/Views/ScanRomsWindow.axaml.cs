using Avalonia.Controls;
using Avalonia.Interactivity;
using Gamelistify.Services;
using Gamelistify.ViewModels;

namespace Gamelistify.Views;

public partial class ScanRomsWindow : Window
{
    public ScanRomsWindow()
    {
        InitializeComponent();
        Opened += (_, _) => Logger.Information("ScanRomsWindow opened");
        Closed += (_, _) => Logger.Information("ScanRomsWindow closed");
    }

    private void OnAddClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is ScanRomsViewModel viewModel && viewModel.Accepted)
        {
            Logger.Information("ScanRomsWindow accepted with selected item count {Count}", viewModel.SelectedItems.Count);
            Close(true);
        }
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        Logger.Information("ScanRomsWindow cancelled");
        Close(false);
    }
}
