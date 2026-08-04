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

    public void SetDialogDim(bool dimmed)
    {
        DialogDimOverlay.IsVisible = dimmed;
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
            return;
        }

        if (!EntriesGrid.IsFocused || DataContext is not MainViewModel viewModel)
            return;

        if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            switch (e.Key)
            {
                case Key.A:
                    EntriesGrid.SelectAll();
                    e.Handled = true;
                    break;
                case Key.I:
                    InvertGridSelection(viewModel);
                    e.Handled = true;
                    break;
            }
            return;
        }

        switch (e.Key)
        {
            case Key.H when viewModel.ToggleHiddenSelectedCommand.CanExecute(null):
                viewModel.ToggleHiddenSelectedCommand.Execute(null);
                e.Handled = true;
                break;
            case Key.F when viewModel.ToggleFavoriteSelectedCommand.CanExecute(null):
                viewModel.ToggleFavoriteSelectedCommand.Execute(null);
                e.Handled = true;
                break;
            case Key.U when viewModel.UnhideSelectedCommand.CanExecute(null):
                viewModel.UnhideSelectedCommand.Execute(null);
                e.Handled = true;
                break;
            case Key.G when viewModel.UnfavoriteSelectedCommand.CanExecute(null):
                viewModel.UnfavoriteSelectedCommand.Execute(null);
                e.Handled = true;
                break;
        }
    }

    private void InvertGridSelection(MainViewModel viewModel)
    {
        var selected = new HashSet<object>(EntriesGrid.SelectedItems.Cast<object>());
        EntriesGrid.SelectedItems.Clear();
        foreach (var row in viewModel.VisibleEntries)
        {
            if (!selected.Contains(row))
                EntriesGrid.SelectedItems.Add(row);
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
