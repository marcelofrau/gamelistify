using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Gamelistify.ViewModels;
using Gamelistify.Services;

namespace Gamelistify.Views;

public partial class MainWindow : Window
{
    private bool _allowClose;

    public MainWindow()
    {
        InitializeComponent();
        Opened += (_, _) => Logger.Information("MainWindow opened at {Time}", DateTime.Now);
        Closing += (_, _) => Logger.Information("MainWindow closing at {Time}", DateTime.Now);
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        base.OnClosing(e);
        if (_allowClose)
            return;

        e.Cancel = true;
        _ = ConfirmExitAndCloseAsync();
    }

    private async Task ConfirmExitAndCloseAsync()
    {
        if (DataContext is not MainViewModel viewModel)
        {
            _allowClose = true;
            Close();
            return;
        }

        var decision = await viewModel.ConfirmExitAsync();
        switch (decision)
        {
            case ExitDecision.SaveAndExit:
                if (!await viewModel.TrySaveAsync())
                    return;
                break;
            case ExitDecision.Discard:
                break;
            default:
                return;
        }

        Logger.Information("Exit confirmed, closing main window");
        _allowClose = true;
        Close();
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
