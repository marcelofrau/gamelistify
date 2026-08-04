using Avalonia.Controls;
using Avalonia.Interactivity;
using Gamelistify.Services;
using Gamelistify.ViewModels;

namespace Gamelistify.Views;

public partial class BatchFavoriteWindow : Window
{
    public BatchFavoriteWindow()
    {
        InitializeComponent();
        Opened += (_, _) => Logger.Information("BatchFavoriteWindow opened");
        Closed += (_, _) => Logger.Information("BatchFavoriteWindow closed");
    }

    private void OnOkClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is BatchFavoriteViewModel viewModel && viewModel.Names.Count > 0)
        {
            Logger.Information("BatchFavoriteWindow accepted with {Count} names", viewModel.Names.Count);
            Close(true);
        }
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        Logger.Information("BatchFavoriteWindow cancelled");
        Close(false);
    }
}
