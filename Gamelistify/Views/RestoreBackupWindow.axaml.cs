using Avalonia.Controls;
using Avalonia.Interactivity;
using Gamelistify.Services;
using Gamelistify.ViewModels;

namespace Gamelistify.Views;

public partial class RestoreBackupWindow : Window
{
    public RestoreBackupWindow()
    {
        InitializeComponent();
        Opened += (_, _) => Logger.Information("RestoreBackupWindow opened");
        Closed += (_, _) => Logger.Information("RestoreBackupWindow closed");
    }

    private void OnRestoreClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is RestoreBackupViewModel viewModel && viewModel.SelectedBackup is { } backup)
        {
            Logger.Information("RestoreBackupWindow restoring {BackupPath}", backup.FilePath);
            Close(backup.FilePath);
        }
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        Logger.Information("RestoreBackupWindow cancelled");
        Close();
    }
}
