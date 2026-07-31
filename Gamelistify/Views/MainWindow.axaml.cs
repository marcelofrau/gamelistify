using Avalonia.Controls;
using Avalonia.Input;
using Gamelistify.ViewModels;
using Gamelistify.Services;

namespace Gamelistify.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Opened += (_, _) => Logger.Information("MainWindow opened at {Time}", DateTime.Now);
        Closing += (_, _) => Logger.Information("MainWindow closing at {Time}", DateTime.Now);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Key == Key.F && e.KeyModifiers.HasFlag(KeyModifiers.Control) && SearchBox is not null)
        {
            SearchBox.Focus();
            SearchBox.SelectAll();
            e.Handled = true;
        }
    }

    private void OnEntriesDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is MainViewModel { HasSelection: true } && DetailNameBox is not null)
        {
            DetailNameBox.Focus();
            DetailNameBox.SelectAll();
        }
    }

    private void OnEntriesSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is not DataGrid dataGrid || DataContext is not MainViewModel viewModel)
            return;

        Logger.Debug("MainWindow DataGrid selection changed");
        viewModel.UpdateSelectedEntries(dataGrid.SelectedItems.OfType<GameRowViewModel>().ToList());
    }
}
