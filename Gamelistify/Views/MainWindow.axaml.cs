using Avalonia.Controls;
using Gamelistify.ViewModels;
using Gamelistify.Services;

namespace Gamelistify.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Opened += (_, _) => Logger.Information("MainWindow opened");
        Closing += (_, _) => Logger.Information("MainWindow closing");
    }

    private void OnEntriesSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is not DataGrid dataGrid || DataContext is not MainViewModel viewModel)
            return;

        Logger.Debug("MainWindow DataGrid selection changed");
        viewModel.UpdateSelectedEntries(dataGrid.SelectedItems.OfType<GameRowViewModel>().ToList());
    }
}
