using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Gamelistify.Models;

namespace Gamelistify.ViewModels;

public sealed partial class RestoreBackupViewModel : ObservableObject
{
    public RestoreBackupViewModel(IReadOnlyList<BackupInfo> backups)
    {
        foreach (var backup in backups)
            Backups.Add(backup);
        SelectedBackup = Backups.FirstOrDefault();
    }

    public ObservableCollection<BackupInfo> Backups { get; } = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelection))]
    private BackupInfo? _selectedBackup;

    public bool HasBackups => Backups.Count > 0;

    public bool ShowEmptyMessage => !HasBackups;

    public bool HasSelection => SelectedBackup is not null;
}
