using System.Collections.ObjectModel;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gamelistify.Helpers;
using Gamelistify.Models;
using Gamelistify.Services;
using Serilog.Events;

namespace Gamelistify.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private GamelistDocument? _loadedDocument;
    private List<GameRowViewModel> _allEntries = [];
    private AppSettings _settings = new();
    private bool _suppressEditorApply;
    private List<GameRowViewModel> _selectedEntries = [];
    private readonly RomScannerService _romScannerService = new();

    [ObservableProperty]
    private string _searchText = string.Empty;

    public string[] FilterOptions { get; } = ["All Entries", "Games", "Folders"];

    [ObservableProperty]
    private int _selectedFilterIndex;

    [ObservableProperty]
    private bool _showHiddenEntries = true;

    [ObservableProperty]
    private string _statusText = "Ready. Open a gamelist.xml file to begin.";

    [ObservableProperty]
    private string _windowTitle = $"{ProjectInfo.ProjectName} {BuildInfo.DisplayVersion}";

    [ObservableProperty]
    private string _detailTitle = string.Empty;

    [ObservableProperty]
    private string _detailPath = string.Empty;

    [ObservableProperty]
    private string _detailSummary = "Open a gamelist.xml file to inspect metadata and preview images.";

    [ObservableProperty]
    private string _detailImageStatus = "No image loaded.";

    [ObservableProperty]
    private string _currentFilePath = "No file loaded";

    [ObservableProperty]
    private string _entryCountText = "0 entries";

    [ObservableProperty]
    private string _detailNameEdit = string.Empty;

    [ObservableProperty]
    private string _detailGenreEdit = string.Empty;

    [ObservableProperty]
    private string _detailDeveloperEdit = string.Empty;

    [ObservableProperty]
    private string _detailDescriptionEdit = string.Empty;

    [ObservableProperty]
    private bool _showNameColumn = true;

    [ObservableProperty]
    private bool _showGenreColumn = true;

    [ObservableProperty]
    private bool _showDeveloperColumn = true;

    [ObservableProperty]
    private bool _showHiddenColumn = true;

    [ObservableProperty]
    private bool _showFavoriteColumn = true;

    [ObservableProperty]
    private bool _detailFavoriteEdit;

    [ObservableProperty]
    private bool _detailHiddenEdit;

    [ObservableProperty]
    private bool _detailKidgameEdit;

    [ObservableProperty]
    private string _detailImageEdit = string.Empty;

    [ObservableProperty]
    private string _detailVideoEdit = string.Empty;

    [ObservableProperty]
    private string _detailMarqueeEdit = string.Empty;

    [ObservableProperty]
    private string _detailWheelEdit = string.Empty;

    [ObservableProperty]
    private string _detailFanartEdit = string.Empty;

    [ObservableProperty]
    private string _detailThumbnailEdit = string.Empty;

    [ObservableProperty]
    private string _detailScreenshotEdit = string.Empty;

    [ObservableProperty]
    private string _detailPublisher = string.Empty;

    [ObservableProperty]
    private string _detailPlayers = string.Empty;

    [ObservableProperty]
    private string _detailRating = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Star1Filled))]
    [NotifyPropertyChangedFor(nameof(Star2Filled))]
    [NotifyPropertyChangedFor(nameof(Star3Filled))]
    [NotifyPropertyChangedFor(nameof(Star4Filled))]
    [NotifyPropertyChangedFor(nameof(Star5Filled))]
    private double _detailRatingValue;

    public bool Star1Filled => Math.Round(DetailRatingValue * 5, MidpointRounding.AwayFromZero) >= 1;
    public bool Star2Filled => Math.Round(DetailRatingValue * 5, MidpointRounding.AwayFromZero) >= 2;
    public bool Star3Filled => Math.Round(DetailRatingValue * 5, MidpointRounding.AwayFromZero) >= 3;
    public bool Star4Filled => Math.Round(DetailRatingValue * 5, MidpointRounding.AwayFromZero) >= 4;
    public bool Star5Filled => Math.Round(DetailRatingValue * 5, MidpointRounding.AwayFromZero) >= 5;

    [ObservableProperty]
    private string _detailVotes = string.Empty;

    [ObservableProperty]
    private string _detailReleaseDate = string.Empty;

    [ObservableProperty]
    private string _detailLastPlayed = string.Empty;

    [ObservableProperty]
    private string _detailPlayCount = string.Empty;

    [ObservableProperty]
    private bool _isDirty;

    partial void OnIsDirtyChanged(bool value)
    {
        ReloadCommand.NotifyCanExecuteChanged();
    }

    [ObservableProperty]
    private Bitmap? _detailImage;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasDetailImage))]
    [NotifyPropertyChangedFor(nameof(ShowPreviewPlaceholder))]
    [NotifyPropertyChangedFor(nameof(HasSelection))]
    private GameRowViewModel? _selectedEntry;

    public ObservableCollection<GameRowViewModel> VisibleEntries { get; } = [];

    public ObservableCollection<string> OrphanMediaItems { get; } = [];

    [ObservableProperty]
    private string _orphanScanStatus = "Run a scan to find unlinked media for the selected entry.";

    public ObservableCollection<RecentFileViewModel> RecentFiles { get; } = [];

    public ObservableCollection<OpenFlyoutItem> OpenFlyoutItems { get; } = [];

    public Func<Task<string?>>? PickGamelistFileAsync { get; set; }

    public Func<Task<string?>>? PickRomFileAsync { get; set; }

    public Func<Task<string?>>? PickSavePathAsync { get; set; }

    public Func<string?, Task<string?>>? PickFolderAsync { get; set; }

    public Func<Task>? ShowAboutAsync { get; set; }

    public Func<AppSettings, Task<AppSettings?>>? EditSettingsAsync { get; set; }

    public Func<IReadOnlyList<ScannedRomItemViewModel>, string, Task<IReadOnlyList<ScannedRomItemViewModel>?>>? ReviewScannedRomsAsync { get; set; }

    public Func<string, string?, Task<(string Platform, string ExtraArguments)?>>? ChooseScrapeOptionsAsync { get; set; }

    public Func<ScrapeProgressViewModel, Task>? ShowScrapeProgressAsync { get; set; }

    public Func<string, string?, Task<string?>>? PickMediaFileAsync { get; set; }

    public Func<string, string, Task<bool>>? ConfirmAsync { get; set; }

    public Func<Task<IReadOnlyList<string>?>>? ShowBatchFavoriteAsync { get; set; }

    public Func<HygienePlan, Task<bool>>? ShowHygienePlanAsync { get; set; }

    public Func<Task<ExitDecision>>? ShowExitConfirmAsync { get; set; }

    public Func<IReadOnlyList<BackupInfo>, Task<string?>>? ShowRestoreBackupAsync { get; set; }

    public async Task<ExitDecision> ConfirmExitAsync()
    {
        if (!IsDirty)
        {
            if (ConfirmAsync is not null && !await ConfirmAsync("Exit Gamelistify", "Close Gamelistify?"))
                return ExitDecision.Cancel;
            return ExitDecision.Discard;
        }

        if (ShowExitConfirmAsync is not null)
            return await ShowExitConfirmAsync();

        Logger.Warning("Exit confirmation requested before delegate wiring");
        return ExitDecision.Cancel;
    }

    public bool HasLoadedDocument => _loadedDocument is not null;

    public bool HasSelection => SelectedEntry is not null;

    public string[] QuickTipsList { get; } =
    [
        "Use Ctrl+O to open a gamelist.xml file.",
        "Use Ctrl+S to save changes back to the gamelist.xml.",
        "Select entries in the grid to edit metadata in the right panel.",
        "Use the toolbar buttons to hide, favorite, or bulk-scrape entries.",
        "Image preview shows the first available matching media file.",
        "Filter entries using the text search bar above the grid.",
        "Column visibility checkboxes let you customize the grid view.",
        "The Info tab shows raw metadata from the gamelist.xml entry.",
        "Changes are tracked — the save icon highlights when dirty.",
        "Backups are created automatically before each save."
    ];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CurrentTip))]
    [NotifyPropertyChangedFor(nameof(TipCounter))]
    private int _currentTipIndex;

    public string CurrentTip => QuickTipsList[CurrentTipIndex];

    public string TipCounter => $"{CurrentTipIndex + 1} / {QuickTipsList.Length}";

    [RelayCommand]
    private void PreviousTip()
    {
        CurrentTipIndex = CurrentTipIndex > 0 ? CurrentTipIndex - 1 : QuickTipsList.Length - 1;
    }

    [RelayCommand]
    private void NextTip()
    {
        CurrentTipIndex = CurrentTipIndex < QuickTipsList.Length - 1 ? CurrentTipIndex + 1 : 0;
    }

    public bool HasDetailImage => DetailImage is not null;

    public bool ShowPreviewPlaceholder => !HasDetailImage;

    public bool HasRecentFiles => RecentFiles.Count > 0;

    public bool HasMultiSelection => _selectedEntries.Count > 1;

    partial void OnSearchTextChanged(string value)
    {
        ApplySearchFilter();
    }

    partial void OnSelectedFilterIndexChanged(int value)
    {
        ApplySearchFilter();
    }

    partial void OnShowHiddenEntriesChanged(bool value)
    {
        ApplySearchFilter();
    }

    partial void OnSelectedEntryChanged(GameRowViewModel? value)
    {
        UpdateDetailPane(value);
        RemoveEntryCommand.NotifyCanExecuteChanged();
    }

    partial void OnDetailImageChanged(Bitmap? value)
    {
        OnPropertyChanged(nameof(HasDetailImage));
        OnPropertyChanged(nameof(ShowPreviewPlaceholder));
    }

    partial void OnDetailNameEditChanged(string value) => ApplyEditorChanges();

    partial void OnDetailGenreEditChanged(string value) => ApplyEditorChanges();

    partial void OnDetailDeveloperEditChanged(string value) => ApplyEditorChanges();

    partial void OnDetailDescriptionEditChanged(string value) => ApplyEditorChanges();

    partial void OnDetailFavoriteEditChanged(bool value) => ApplyEditorChanges();

    partial void OnDetailHiddenEditChanged(bool value) => ApplyEditorChanges();

    partial void OnDetailKidgameEditChanged(bool value) => ApplyEditorChanges();

    partial void OnDetailImageEditChanged(string value)
    {
        ApplyEditorChanges();
        RefreshImagePreview();
    }

    partial void OnDetailVideoEditChanged(string value) => ApplyEditorChanges();

    partial void OnDetailMarqueeEditChanged(string value) => ApplyEditorChanges();

    partial void OnDetailWheelEditChanged(string value) => ApplyEditorChanges();

    partial void OnDetailFanartEditChanged(string value) => ApplyEditorChanges();

    partial void OnDetailThumbnailEditChanged(string value) => ApplyEditorChanges();

    partial void OnDetailScreenshotEditChanged(string value) => ApplyEditorChanges();

    partial void OnDetailPublisherChanged(string value) => ApplyEditorChanges();

    partial void OnDetailPlayersChanged(string value) => ApplyEditorChanges();

    partial void OnDetailRatingChanged(string value)
    {
        if (double.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var d))
            DetailRatingValue = double.Clamp(d, 0, 1);
        else
            DetailRatingValue = 0;
        ApplyEditorChanges();
    }

    partial void OnDetailRatingValueChanged(double value)
    {
        var formatted = value.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture);
        if (DetailRating != formatted)
            DetailRating = formatted;
    }

    [RelayCommand]
    private void SetRating(string? value)
    {
        if (double.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var d))
        {
            Logger.Debug("Rating set to {Rating}/5 for {Entry}", d, SelectedEntry?.Name ?? "(none)");
            DetailRatingValue = double.Clamp(d / 5.0, 0, 1);
        }
    }

    [RelayCommand]
    private async Task BrowseMediaAsync(string? fieldName)
    {
        if (string.IsNullOrWhiteSpace(fieldName) || PickMediaFileAsync is null || SelectedEntry is null)
            return;

        Logger.Debug("Browsing media file for field {Field}", fieldName);
        var baseDirectory = _loadedDocument?.BaseDirectory ?? string.Empty;
        var path = await PickMediaFileAsync(fieldName, baseDirectory);
        if (string.IsNullOrWhiteSpace(path))
        {
            Logger.Debug("Media browse cancelled for {Field}", fieldName);
            return;
        }

        Logger.Information("Setting media field {Field} to {Path}", fieldName, path);
        SetMediaField(fieldName, path);
    }

    private void SetMediaField(string fieldName, string path)
    {
        switch (fieldName)
        {
            case "image":
                DetailImageEdit = path;
                break;
            case "video":
                DetailVideoEdit = path;
                break;
            case "marquee":
                DetailMarqueeEdit = path;
                break;
            case "wheel":
                DetailWheelEdit = path;
                break;
            case "fanart":
                DetailFanartEdit = path;
                break;
            case "thumbnail":
                DetailThumbnailEdit = path;
                break;
            case "screenshot":
                DetailScreenshotEdit = path;
                break;
        }
    }

    [RelayCommand]
    private void FindOrphanMedia()
    {
        OrphanMediaItems.Clear();
        if (SelectedEntry?.Entry is null || _loadedDocument is null)
        {
            OrphanScanStatus = "Select an entry to scan for orphan media.";
            return;
        }

        Logger.Information("Scanning orphan media for {Entry}", SelectedEntry.Name);
        var matches = MediaResolverService.FindOrphanImages(_loadedDocument.BaseDirectory, SelectedEntry.Entry);
        foreach (var match in matches)
            OrphanMediaItems.Add(match.MediaKey);

        OrphanScanStatus = matches.Count == 0
            ? "No orphan media found."
            : $"Found {matches.Count} orphan media file{(matches.Count == 1 ? string.Empty : "s")}.";
    }

    partial void OnDetailVotesChanged(string value) => ApplyEditorChanges();

    partial void OnDetailReleaseDateChanged(string value) => ApplyEditorChanges();

    partial void OnDetailLastPlayedChanged(string value) => ApplyEditorChanges();

    partial void OnDetailPlayCountChanged(string value) => ApplyEditorChanges();

    partial void OnShowNameColumnChanged(bool value) => PersistColumnVisibility(columns => columns.Name = value);

    partial void OnShowGenreColumnChanged(bool value) => PersistColumnVisibility(columns => columns.Genre = value);

    partial void OnShowDeveloperColumnChanged(bool value) => PersistColumnVisibility(columns => columns.Developer = value);

    partial void OnShowHiddenColumnChanged(bool value) => PersistColumnVisibility(columns => columns.Hidden = value);

    partial void OnShowFavoriteColumnChanged(bool value) => PersistColumnVisibility(columns => columns.Favorite = value);

    public async Task InitializeAsync()
    {
        Logger.Verbose("MainViewModel.InitializeAsync start");
        _settings = await SettingsService.LoadAsync(AppPaths.SettingsPath);
        if (Enum.TryParse<LogEventLevel>(_settings.LogLevel, true, out var persistedLevel))
            Logger.SetMinimumLevel(persistedLevel);
        ApplyColumnSettings();
        SyncRecentFiles();
        StatusText = "Ready. Open a gamelist.xml file to begin.";
        Logger.Information("MainViewModel initialized. Recent files count: {Count}", RecentFiles.Count);
    }

    [RelayCommand]
    private async Task OpenAsync()
    {
        if (PickGamelistFileAsync is null)
        {
            Logger.Warning("Open command invoked before delegate wiring");
            StatusText = "Open action is not wired yet.";
            return;
        }

        var filePath = await PickGamelistFileAsync();
        if (string.IsNullOrWhiteSpace(filePath))
        {
            Logger.Debug("Open command cancelled");
            StatusText = "Open cancelled.";
            return;
        }

        Logger.Information("Opening gamelist file {FilePath}", filePath);
        await LoadFileAsync(filePath);
    }

    [RelayCommand(CanExecute = nameof(CanDiscard))]
    private async Task ReloadAsync()
    {
        Logger.Information("Discard requested — reloading current document");
        if (_loadedDocument?.SourcePath is null)
            return;
        if (ConfirmAsync is not null && !await ConfirmAsync("Discard changes", "Reload the file from disk? All unsaved changes will be lost."))
            return;
        await LoadFileAsync(_loadedDocument.SourcePath);
        StatusText = "Changes discarded. Reloaded from disk.";
    }

    [RelayCommand(CanExecute = nameof(CanRestoreBackup))]
    private async Task RestoreBackupAsync()
    {
        if (_loadedDocument?.SourcePath is null)
            return;

        var backups = BackupService.GetBackups(_loadedDocument.SourcePath);
        if (backups.Count == 0)
        {
            StatusText = "No backups found for this gamelist.";
            return;
        }

        if (ShowRestoreBackupAsync is null)
        {
            Logger.Warning("Restore backup requested before delegate wiring");
            StatusText = "Restore backup dialog is not wired yet.";
            return;
        }

        var backupPath = await ShowRestoreBackupAsync(backups);
        if (string.IsNullOrEmpty(backupPath))
        {
            Logger.Debug("Restore backup cancelled");
            StatusText = "Restore backup cancelled.";
            return;
        }

        var confirmed = ConfirmAsync is null
            || await ConfirmAsync("Restore backup",
                $"Restore the gamelist from \"{Path.GetFileName(backupPath)}\"? The current gamelist is backed up first.");
        if (!confirmed)
        {
            StatusText = "Restore backup cancelled.";
            return;
        }

        try
        {
            Logger.Information("Restoring gamelist from backup {BackupPath}", backupPath);
            await BackupService.RestoreBackupAsync(_loadedDocument.SourcePath, backupPath);
            await LoadFileAsync(_loadedDocument.SourcePath);
            StatusText = $"Restored from {Path.GetFileName(backupPath)}.";
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Restore failed from {BackupPath}", backupPath);
            StatusText = "Restore failed. Check the log for details.";
        }
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAsync()
    {
        await TrySaveAsync();
    }

    public async Task<bool> TrySaveAsync()
    {
        if (_loadedDocument is null || string.IsNullOrWhiteSpace(_loadedDocument.SourcePath))
            return false;

        try
        {
            Logger.Information("Saving current gamelist to {Path}", _loadedDocument.SourcePath);
            var backupPath = await GamelistService.SaveAsync(_loadedDocument);
            IsDirty = false;
            UpdateWindowTitle();
            StatusText = backupPath is null
                ? $"Saved {Path.GetFileName(_loadedDocument.SourcePath)}. No backup created (first save)."
                : $"Saved {Path.GetFileName(_loadedDocument.SourcePath)}. Backup: {Path.GetFileName(backupPath)}";
            return true;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Save failed for {Path}", _loadedDocument.SourcePath);
            StatusText = "Save failed. Check the log for details.";
            return false;
        }
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAsAsync()
    {
        if (_loadedDocument is null)
            return;

        if (PickSavePathAsync is null)
        {
            Logger.Warning("Save As requested before delegate wiring");
            StatusText = "Save As action is not wired yet.";
            return;
        }

        var destinationPath = await PickSavePathAsync();
        if (string.IsNullOrWhiteSpace(destinationPath))
        {
            Logger.Debug("Save As cancelled");
            StatusText = "Save As cancelled.";
            return;
        }

        try
        {
            Logger.Information("Saving current gamelist as {Path}", destinationPath);
            await GamelistService.SaveAsync(_loadedDocument, destinationPath, createBackup: false);
            _loadedDocument.SourcePath = destinationPath;
            CurrentFilePath = destinationPath;
            IsDirty = false;
            _settings.LastGamelistDirectory = Path.GetDirectoryName(destinationPath) ?? string.Empty;
            RegisterRecentFile(destinationPath);
            await SettingsService.SaveAsync(AppPaths.SettingsPath, _settings);
            UpdateWindowTitle();
            StatusText = $"Saved as {Path.GetFileName(destinationPath)}.";
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Save As failed for {Path}", destinationPath);
            StatusText = "Save As failed. Check the log for details.";
        }
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task OptimizeAsync()
    {
        if (_loadedDocument is null || string.IsNullOrWhiteSpace(_loadedDocument.SourcePath))
            return;

        Logger.Information("Optimizing gamelist to {Path}", _loadedDocument.SourcePath);
        var backupPath = await GamelistService.SaveAsync(_loadedDocument, compact: true);
        IsDirty = false;
        UpdateWindowTitle();
        StatusText = backupPath is null
            ? $"Optimized {Path.GetFileName(_loadedDocument.SourcePath)} (minified XML, no backup on first save)."
            : $"Optimized {Path.GetFileName(_loadedDocument.SourcePath)} (minified XML). Backup: {Path.GetFileName(backupPath)}";
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task CleanupAsync()
    {
        if (_loadedDocument is null)
            return;

        var invalid = GamelistService.GetInvalidEntries(_loadedDocument);
        if (invalid.Count == 0)
        {
            Logger.Information("Cleanup found no invalid entries");
            StatusText = "No invalid entries found. All game paths resolve to existing files.";
            return;
        }

        Logger.Information("Cleanup found {Count} invalid entries", invalid.Count);
        var names = string.Join(", ", invalid.Take(5).Select(static entry => entry.Name));
        if (invalid.Count > 5)
            names += $" … and {invalid.Count - 5} more";

        var confirmed = ConfirmAsync is null
            || await ConfirmAsync("Remove invalid entries",
                $"{invalid.Count} entr{(invalid.Count == 1 ? "y" : "ies")} reference missing files and will be removed: {names}");
        if (!confirmed)
        {
            StatusText = "Cleanup cancelled.";
            return;
        }

        foreach (var entry in invalid)
        {
            _loadedDocument.RemoveEntry(entry);
            _allEntries.RemoveAll(row => ReferenceEquals(row.Entry, entry));
        }

        ApplySearchFilter();
        SelectedEntry = null;
        IsDirty = true;
        UpdateWindowTitle();
        StatusText = $"Removed {invalid.Count} invalid entr{(invalid.Count == 1 ? "y" : "ies")}.";
    }

    [RelayCommand(CanExecute = nameof(CanAddEntry))]
    private async Task AddGameAsync()
    {
        if (PickRomFileAsync is null)
        {
            Logger.Warning("Add game requested before delegate wiring");
            StatusText = "Add game action is not wired yet.";
            return;
        }

        var filePath = await PickRomFileAsync();
        if (string.IsNullOrWhiteSpace(filePath))
        {
            Logger.Debug("Add game cancelled");
            StatusText = "Add game cancelled.";
            return;
        }

        if (!File.Exists(filePath))
        {
            StatusText = "Selected game file no longer exists.";
            return;
        }

        var entry = new GamelistEntry(GamelistEntryKind.Game);
        entry.SetField("path", ToStoredPath(filePath));
        entry.SetField("name", Path.GetFileNameWithoutExtension(filePath));
        Logger.Information("Adding game entry {Name} ({Path})", entry.Name, entry.Path);
        AddEntryToList(entry);
    }

    [RelayCommand(CanExecute = nameof(CanAddEntry))]
    private async Task AddFolderAsync()
    {
        if (PickFolderAsync is null)
        {
            Logger.Warning("Add folder requested before delegate wiring");
            StatusText = "Add folder action is not wired yet.";
            return;
        }

        var baseDirectory = _loadedDocument?.BaseDirectory ?? string.Empty;
        var folderPath = await PickFolderAsync(baseDirectory);
        if (string.IsNullOrWhiteSpace(folderPath))
        {
            Logger.Debug("Add folder cancelled");
            StatusText = "Add folder cancelled.";
            return;
        }

        var entry = new GamelistEntry(GamelistEntryKind.Folder);
        entry.SetField("path", ToStoredPath(folderPath));
        entry.SetField("name", Path.GetFileName(Path.TrimEndingDirectorySeparator(folderPath)));
        Logger.Information("Adding folder entry {Name} ({Path})", entry.Name, entry.Path);
        AddEntryToList(entry);
    }

    private bool CanAddEntry() => _loadedDocument is not null;

    private void AddEntryToList(GamelistEntry entry)
    {
        if (_loadedDocument is null)
            return;

        _loadedDocument.Entries.Add(entry);
        var row = new GameRowViewModel(entry);
        _allEntries.Add(row);

        SelectedFilterIndex = 0;
        SearchText = string.Empty;
        ApplySearchFilter();
        SelectedEntry = row;

        IsDirty = true;
        UpdateWindowTitle();
        StatusText = $"Added \"{entry.Name}\" to the gamelist.";
    }

    private string ToStoredPath(string filePath)
    {
        var baseDirectory = _loadedDocument?.BaseDirectory ?? string.Empty;
        if (string.IsNullOrWhiteSpace(baseDirectory))
            return GamelistPathHelper.NormalizeStoredPath(filePath);

        var relative = Path.GetRelativePath(baseDirectory, filePath).Replace('\\', '/');
        return relative.StartsWith("..", StringComparison.Ordinal)
            ? GamelistPathHelper.NormalizeStoredPath(filePath)
            : $"./{relative}";
    }

    private bool CanSave() => HasLoadedDocument;

    private bool CanDiscard() => IsDirty;

    private bool CanRestoreBackup() => HasLoadedDocument;

    private bool CanRemove() => HasSelection;

    [RelayCommand(CanExecute = nameof(CanRemove))]
    private async Task RemoveEntryAsync()
    {
        if (SelectedEntry is null) return;
        if (ConfirmAsync is not null && !await ConfirmAsync("Remove entry", $"Remove \"{SelectedEntry.Entry.Name}\" from the gamelist?"))
            return;

        Logger.Information("Removing entry {Name} from gamelist", SelectedEntry.Entry.Name);
        _allEntries.Remove(SelectedEntry);
        _loadedDocument?.RemoveEntry(SelectedEntry.Entry);
        SelectedEntry = null;
        IsDirty = true;
        UpdateWindowTitle();
        StatusText = "Entry removed.";
    }

    [RelayCommand(CanExecute = nameof(CanRunBulkAction))]
    private async Task RemoveSelectedEntriesAsync()
    {
        if (_selectedEntries.Count == 0) return;
        if (ConfirmAsync is not null && !await ConfirmAsync("Remove selected", $"Remove {_selectedEntries.Count} selected entr{( _selectedEntries.Count == 1 ? "y" : "ies")} from the gamelist?"))
            return;

        var count = _selectedEntries.Count;
        Logger.Information("Removing {Count} selected entries from gamelist", count);
        foreach (var row in _selectedEntries.ToList())
        {
            _allEntries.Remove(row);
            _loadedDocument?.RemoveEntry(row.Entry);
        }
        _selectedEntries.Clear();
        ApplySearchFilter();
        SelectedEntry = null;
        IsDirty = true;
        UpdateWindowTitle();
        StatusText = $"{count} entr{(count == 1 ? "y" : "ies")} removed.";
    }

    [RelayCommand]
    private async Task ScanRomsAsync()
    {
        if (_loadedDocument is null)
        {
            Logger.Warning("Scan ROMs requested without loaded document");
            StatusText = "Load a gamelist.xml file before scanning ROMs.";
            return;
        }

        if (PickFolderAsync is null || ReviewScannedRomsAsync is null)
        {
            Logger.Warning("Scan ROMs workflow not fully wired");
            StatusText = "Scan workflow is not wired yet.";
            return;
        }

        var selectedDirectory = await PickFolderAsync(_loadedDocument.BaseDirectory);
        if (string.IsNullOrWhiteSpace(selectedDirectory))
        {
            Logger.Debug("ROM scan cancelled after folder picker");
            StatusText = "ROM scan cancelled.";
            return;
        }

        Logger.Information("Scanning directory for missing ROMs: {Directory}", selectedDirectory);
        var scanned = RomScannerService.Scan(selectedDirectory);
        var missing = RomScannerService.FindMissingEntries(scanned, _loadedDocument)
            .Select(static rom => new ScannedRomItemViewModel(rom))
            .ToList();

        if (missing.Count == 0)
        {
            Logger.Information("ROM scan found no missing entries");
            StatusText = "No missing ROM entries were found.";
            return;
        }

        var approved = await ReviewScannedRomsAsync(missing, selectedDirectory);
        if (approved is null || approved.Count == 0)
        {
            Logger.Debug("ROM scan review closed without additions");
            StatusText = "ROM scan review closed without adding entries.";
            return;
        }

        Logger.Information("Applying {Count} ROM entries from scan", approved.Count);
        foreach (var item in approved)
        {
            var entry = new GamelistEntry(GamelistEntryKind.Game);
            entry.SetField("path", item.RelativePath);
            entry.SetField("name", item.Name);
            _loadedDocument.Entries.Add(entry);
        }

        _allEntries = _loadedDocument.Entries.Select(static entry => new GameRowViewModel(entry)).ToList();
        IsDirty = true;
        ApplySearchFilter();
        SelectedEntry = VisibleEntries.FirstOrDefault(entry => approved.Any(added => added.RelativePath.Equals(entry.Path, StringComparison.OrdinalIgnoreCase)))
            ?? VisibleEntries.FirstOrDefault();
        UpdateWindowTitle();
        StatusText = $"Added {approved.Count} ROM entries from scan.";
    }

    [RelayCommand(CanExecute = nameof(CanRunBulkAction))]
    private async Task ScrapeSelectedAsync()
    {
        if (_loadedDocument is null)
        {
            Logger.Warning("Scrape selected requested without loaded document");
            StatusText = "Load a gamelist.xml file before scraping.";
            return;
        }

        var selectedGames = _selectedEntries.Where(static entry => entry.Entry.Kind == GamelistEntryKind.Game).ToList();
        if (selectedGames.Count == 0)
        {
            Logger.Warning("Scrape selected requested without game selection");
            StatusText = "Select at least one game entry to scrape.";
            return;
        }

        var options = await PromptForScrapeOptionsAsync("Scrape Selected");
        if (options is null)
            return;

        foreach (var game in selectedGames)
        {
            Logger.Information("Starting scrape for selected game {Game}", game.Name);
            var request = new ScrapeRequest
            {
                Platform = options.Value.Platform,
                RomsDirectory = _loadedDocument.BaseDirectory,
                SelectedRomPath = game.Path,
                MediaDirectory = Path.Combine(_loadedDocument.BaseDirectory, "images"),
                ExtraArguments = ParseArguments(options.Value.ExtraArguments),
            };

            var result = await RunScrapeAsync($"Scrape Selected: {game.Name}", request);
            if (result is null || result.WasCancelled || result.ExitCode != 0)
                return;
        }

        await RefreshAfterScrapeAsync();
        Logger.Information("Scrape selected completed for {Count} entries", selectedGames.Count);
        StatusText = $"Completed scraping for {selectedGames.Count} selected entries.";
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task ScrapeAllAsync()
    {
        if (_loadedDocument is null)
        {
            Logger.Warning("Scrape all requested without loaded document");
            StatusText = "Load a gamelist.xml file before scraping.";
            return;
        }

        var options = await PromptForScrapeOptionsAsync("Scrape All");
        if (options is null)
            return;

        var request = new ScrapeRequest
        {
            Platform = options.Value.Platform,
            RomsDirectory = _loadedDocument.BaseDirectory,
            MediaDirectory = Path.Combine(_loadedDocument.BaseDirectory, "images"),
            ExtraArguments = ParseArguments(options.Value.ExtraArguments),
        };

        var result = await RunScrapeAsync("Scrape All", request);
        if (result is null)
            return;

        if (result.WasCancelled)
        {
            StatusText = "Bulk scrape cancelled.";
            return;
        }

        if (result.ExitCode != 0)
        {
            StatusText = $"Bulk scrape failed with exit code {result.ExitCode}.";
            return;
        }

        await RefreshAfterScrapeAsync();
        Logger.Information("Bulk scrape completed successfully");
        StatusText = "Bulk scrape completed and document reloaded.";
    }

    [RelayCommand]
    private async Task OpenAboutAsync()
    {
        if (ShowAboutAsync is not null)
            await ShowAboutAsync();
    }

    [RelayCommand]
    private async Task OpenSettingsAsync()
    {
        if (EditSettingsAsync is null)
            return;

        Logger.Information("Opening settings from main window");
        var edited = await EditSettingsAsync(_settings);
        if (edited is null)
            return;

        _settings = edited;
        await SettingsService.SaveAsync(AppPaths.SettingsPath, _settings);
        Logger.Information("Settings saved from main window");
        StatusText = "Settings saved.";
        if (SelectedEntry is not null)
            UpdateDetailPane(SelectedEntry);
    }

    [RelayCommand(CanExecute = nameof(CanOpenRecent))]
    private async Task OpenRecentAsync(string? filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return;

        if (!File.Exists(filePath))
        {
            Logger.Warning("Recent file missing: {RecentFile}", filePath);
            StatusText = "Recent file no longer exists.";
            _settings.RecentFiles.RemoveAll(path => path.Equals(filePath, StringComparison.OrdinalIgnoreCase));
            SyncRecentFiles();
            await SettingsService.SaveAsync(AppPaths.SettingsPath, _settings);
            return;
        }

        Logger.Information("Opening recent file {RecentFile}", filePath);
        await LoadFileAsync(filePath);
    }

    private static bool CanOpenRecent(string? filePath) => !string.IsNullOrWhiteSpace(filePath);

    [RelayCommand(CanExecute = nameof(CanRunBulkAction))]
    private void HideSelected()
    {
        ApplyBulkBooleanField("hidden", true, "Selected entries hidden.");
    }

    [RelayCommand(CanExecute = nameof(CanRunBulkAction))]
    private void UnhideSelected()
    {
        ApplyBulkBooleanField("hidden", false, "Selected entries unhidden.");
    }

    [RelayCommand(CanExecute = nameof(CanRunBulkAction))]
    private void FavoriteSelected()
    {
        ApplyBulkBooleanField("favorite", true, "Selected entries favorited.");
    }

    [RelayCommand(CanExecute = nameof(CanRunBulkAction))]
    private void UnfavoriteSelected()
    {
        ApplyBulkBooleanField("favorite", false, "Selected entries unfavorited.");
    }

    [RelayCommand(CanExecute = nameof(CanRunBulkAction))]
    private void ToggleHiddenSelected()
    {
        if (_selectedEntries.Count == 0)
            return;

        var anyVisible = _selectedEntries.Any(row => !row.Hidden);
        ApplyBulkBooleanField("hidden", anyVisible, anyVisible ? "Selected entries hidden." : "Selected entries unhidden.");
    }

    [RelayCommand(CanExecute = nameof(CanRunBulkAction))]
    private void ToggleFavoriteSelected()
    {
        if (_selectedEntries.Count == 0)
            return;

        var anyUnfavorite = _selectedEntries.Any(row => !row.Favorite);
        ApplyBulkBooleanField("favorite", anyUnfavorite, anyUnfavorite ? "Selected entries favorited." : "Selected entries unfavorited.");
    }

    [RelayCommand(CanExecute = nameof(CanRunBulkAction))]
    private void SetNameFromFilename()
    {
        if (_selectedEntries.Count == 0)
            return;

        var updated = 0;
        Logger.Information("Setting name from filename for {Count} selected entries", _selectedEntries.Count);
        foreach (var row in _selectedEntries)
        {
            var fileName = Path.GetFileName(row.Path);
            var name = string.IsNullOrWhiteSpace(fileName) ? null : Path.GetFileNameWithoutExtension(fileName);
            if (string.IsNullOrWhiteSpace(name))
                continue;

            row.Entry.SetField("name", name);
            row.Refresh();
            updated++;
        }

        if (updated == 0)
        {
            StatusText = "No selected entry has a usable file path.";
            return;
        }

        IsDirty = true;
        UpdateWindowTitle();
        StatusText = $"Set name from filename for {updated} entr{(updated == 1 ? "y" : "ies")}.";
        if (SelectedEntry is not null)
            UpdateDetailPane(SelectedEntry);
        RefreshVisibleRows();
    }

    private bool CanRunBulkAction() => _selectedEntries.Count > 0;

    [RelayCommand(CanExecute = nameof(CanRunLibraryAction))]
    private async Task BatchFavoriteAsync()
    {
        if (_loadedDocument is null || ShowBatchFavoriteAsync is null)
            return;

        Logger.Information("Batch favorite by names requested");
        var names = await ShowBatchFavoriteAsync();
        if (names is null || names.Count == 0)
        {
            Logger.Debug("Batch favorite dialog cancelled or empty");
            return;
        }

        var targetNames = names.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var matched = new List<GameRowViewModel>();
        foreach (var row in _allEntries)
        {
            if (targetNames.Contains(row.Name) && !row.Favorite)
            {
                row.Entry.SetBooleanField("favorite", true);
                row.Refresh();
                matched.Add(row);
            }
        }

        if (matched.Count == 0)
        {
            StatusText = "No entries matched the given names.";
            return;
        }

        IsDirty = true;
        UpdateWindowTitle();
        StatusText = $"Favorited {matched.Count} entr{(matched.Count == 1 ? "y" : "ies")}.";
        RefreshVisibleRows();
    }

    [RelayCommand(CanExecute = nameof(CanRunLibraryAction))]
    private async Task DetectDuplicatesAsync()
    {
        if (_loadedDocument is null || ShowHygienePlanAsync is null)
            return;

        Logger.Information("Detect & hide duplicates requested");
        var plan = LibraryHygieneService.BuildDuplicatesPlan(_allEntries.Select(static r => r.Entry));
        if (plan.ToHide.Count == 0)
        {
            StatusText = "No duplicates found.";
            return;
        }

        if (!await ShowHygienePlanAsync(plan))
        {
            Logger.Debug("Duplicates plan cancelled");
            return;
        }

        var hidden = plan.Apply();
        ApplyHygieneResult($"Hid {hidden} duplicate entr{(hidden == 1 ? "y" : "ies")}.");
    }

    [RelayCommand(CanExecute = nameof(CanRunLibraryAction))]
    private async Task DetectBadVersionsAsync()
    {
        if (_loadedDocument is null || ShowHygienePlanAsync is null)
            return;

        Logger.Information("Detect & hide bad versions requested");
        var plan = LibraryHygieneService.BuildBadVersionsPlan(_allEntries.Select(static r => r.Entry));
        if (plan.ToHide.Count == 0)
        {
            StatusText = "No bad versions found.";
            return;
        }

        if (!await ShowHygienePlanAsync(plan))
        {
            Logger.Debug("Bad versions plan cancelled");
            return;
        }

        var hidden = plan.Apply();
        ApplyHygieneResult($"Hid {hidden} bad version entr{(hidden == 1 ? "y" : "ies")}.");
    }

    [RelayCommand(CanExecute = nameof(CanRunLibraryAction))]
    private async Task ReviewHiddenFavoritesAsync()
    {
        if (_loadedDocument is null)
            return;

        Logger.Information("Review hidden & favorites requested");
        var toReveal = LibraryHygieneService.FindEntriesToReveal(_allEntries.Select(static r => r.Entry));
        if (toReveal.Count == 0)
        {
            StatusText = "No hidden entries need review.";
            return;
        }

        var names = string.Join("\n", toReveal.Select(static e => e.Name));
        if (ConfirmAsync is not null
            && !await ConfirmAsync("Review Hidden & Favorites",
                $"The following entries are hidden but should probably stay visible:\n\n{names}\n\nUnhide them?"))
        {
            Logger.Debug("Review reveal cancelled");
            return;
        }

        foreach (var entry in toReveal)
            entry.SetBooleanField("hidden", false);

        ApplyHygieneResult($"Unhid {toReveal.Count} entr{(toReveal.Count == 1 ? "y" : "ies")}.");
    }

    private void ApplyHygieneResult(string statusMessage)
    {
        IsDirty = true;
        UpdateWindowTitle();
        StatusText = statusMessage;
        foreach (var row in _allEntries)
            row.Refresh();
        RefreshVisibleRows();
        if (SelectedEntry is not null)
            UpdateDetailPane(SelectedEntry);
    }

    private bool CanRunLibraryAction() => _loadedDocument is not null;

    private void ApplySearchFilter()
    {
        var matching = _allEntries.Where(MatchesFilter).ToList();
        if (VisibleEntries.SequenceEqual(matching))
        {
            EntryCountText = $"{VisibleEntries.Count} visible / {_allEntries.Count} total";
            return;
        }

        VisibleEntries.Clear();
        foreach (var entry in matching)
            VisibleEntries.Add(entry);

        EntryCountText = $"{VisibleEntries.Count} visible / {_allEntries.Count} total";
    }

    private bool MatchesFilter(GameRowViewModel row)
    {
        if (SelectedFilterIndex == 1 && row.Entry.Kind != GamelistEntryKind.Game)
            return false;
        if (SelectedFilterIndex == 2 && row.Entry.Kind != GamelistEntryKind.Folder)
            return false;
        if (!ShowHiddenEntries && row.Hidden)
            return false;

        var search = SearchText.Trim();
        if (string.IsNullOrWhiteSpace(search))
            return true;

        return row.Name.Contains(search, StringComparison.OrdinalIgnoreCase)
            || row.Genre.Contains(search, StringComparison.OrdinalIgnoreCase)
            || row.Developer.Contains(search, StringComparison.OrdinalIgnoreCase)
            || row.Path.Contains(search, StringComparison.OrdinalIgnoreCase);
    }

    private void UpdateDetailPane(GameRowViewModel? row)
    {
        Logger.Verbose("UpdateDetailPane for {Row}", row?.Name ?? "(null)");
        DetailImage?.Dispose();
        DetailImage = null;
        _suppressEditorApply = true;

        if (row is null)
        {
            DetailTitle = string.Empty;
            DetailPath = string.Empty;
            DetailSummary = string.Empty;
            DetailNameEdit = string.Empty;
            DetailGenreEdit = string.Empty;
            DetailDeveloperEdit = string.Empty;
            DetailDescriptionEdit = string.Empty;
            DetailFavoriteEdit = false;
            DetailHiddenEdit = false;
            DetailKidgameEdit = false;
            DetailImageEdit = string.Empty;
            DetailVideoEdit = string.Empty;
            DetailMarqueeEdit = string.Empty;
            DetailWheelEdit = string.Empty;
            DetailFanartEdit = string.Empty;
            DetailThumbnailEdit = string.Empty;
            DetailScreenshotEdit = string.Empty;
            DetailPublisher = string.Empty;
            DetailPlayers = string.Empty;
            DetailRating = string.Empty;
            DetailVotes = string.Empty;
            DetailReleaseDate = string.Empty;
            DetailLastPlayed = string.Empty;
            DetailPlayCount = string.Empty;
            OrphanMediaItems.Clear();
            OrphanScanStatus = "Run a scan to find unlinked media for the selected entry.";
            _suppressEditorApply = false;
            return;
        }

        DetailTitle = row.Name;
        DetailPath = row.Path;
        DetailSummary = BuildSummary(row);
        DetailNameEdit = row.Name;
        DetailGenreEdit = row.Genre;
        DetailDeveloperEdit = row.Developer;
        DetailDescriptionEdit = row.Description;
        DetailFavoriteEdit = row.Favorite;
        DetailHiddenEdit = row.Hidden;
        DetailKidgameEdit = row.Entry.GetBooleanField("kidgame");
        DetailImageEdit = row.Entry.GetField("image") ?? string.Empty;
        DetailVideoEdit = row.Entry.GetField("video") ?? string.Empty;
        DetailMarqueeEdit = row.Entry.GetField("marquee") ?? string.Empty;
        DetailWheelEdit = row.Entry.GetField("wheel") ?? string.Empty;
        DetailFanartEdit = row.Entry.GetField("fanart") ?? string.Empty;
        DetailThumbnailEdit = row.Entry.GetField("thumbnail") ?? string.Empty;
        DetailScreenshotEdit = row.Entry.GetField("screenshot") ?? string.Empty;
        DetailPublisher = row.Entry.GetField("publisher") ?? string.Empty;
        DetailPlayers = row.Entry.GetField("players") ?? string.Empty;
        DetailRating = row.Entry.GetField("rating") ?? string.Empty;
        DetailVotes = row.Entry.GetField("votes") ?? string.Empty;
        DetailReleaseDate = row.Entry.GetField("releasedate") ?? string.Empty;
        DetailLastPlayed = row.Entry.GetField("lastplayed") ?? string.Empty;
        DetailPlayCount = row.Entry.GetField("playcount") ?? string.Empty;
        OrphanMediaItems.Clear();
        OrphanScanStatus = "Run a scan to find unlinked media for the selected entry.";
        _suppressEditorApply = false;

        if (_loadedDocument is null)
            return;

        LoadImagePreview();
    }

    private void LoadImagePreview()
    {
        if (SelectedEntry is null || _loadedDocument is null)
        {
            DetailImage?.Dispose();
            DetailImage = null;
            DetailImageStatus = "No image loaded.";
            return;
        }

        var row = SelectedEntry;
        var resolvedImagePath = row.ImagePath is null
            ? null
            : MediaResolverService.ResolveMediaPath(_loadedDocument.BaseDirectory, row.ImagePath);

        if (resolvedImagePath is null)
        {
            Logger.Verbose("No image for {Entry} (no mapped path)", row.Name);
            DetailImageStatus = "No mapped image found for current selection.";
            return;
        }

        Logger.Verbose("Loading preview image {Image}", resolvedImagePath);
        using var stream = File.OpenRead(resolvedImagePath);
        DetailImage = new Bitmap(stream);
        DetailImageStatus = $"Preview source: {Path.GetFileName(resolvedImagePath)}";
    }

    private void RefreshImagePreview()
    {
        if (_suppressEditorApply || SelectedEntry is null)
            return;

        DetailImage?.Dispose();
        DetailImage = null;
        LoadImagePreview();
    }

    private void ApplyEditorChanges()
    {
        if (_suppressEditorApply)
            return;

        if (_loadedDocument is null || SelectedEntry?.Entry is null)
            return;

        Logger.Verbose("Editor changes applied to {Entry}", SelectedEntry.Name);

        SelectedEntry.Entry.SetField("name", DetailNameEdit.Trim());
        SelectedEntry.Entry.SetField("genre", DetailGenreEdit.Trim());
        SelectedEntry.Entry.SetField("developer", DetailDeveloperEdit.Trim());
        SelectedEntry.Entry.SetField("desc", DetailDescriptionEdit.Trim());
        SelectedEntry.Entry.SetField("publisher", DetailPublisher.Trim());
        SelectedEntry.Entry.SetField("players", DetailPlayers.Trim());
        SelectedEntry.Entry.SetField("rating", DetailRating.Trim());
        SelectedEntry.Entry.SetField("votes", DetailVotes.Trim());
        SelectedEntry.Entry.SetField("releasedate", DetailReleaseDate.Trim());
        SelectedEntry.Entry.SetField("lastplayed", DetailLastPlayed.Trim());
        SelectedEntry.Entry.SetField("playcount", DetailPlayCount.Trim());
        SelectedEntry.Entry.SetField("image", DetailImageEdit.Trim());
        SelectedEntry.Entry.SetField("video", DetailVideoEdit.Trim());
        SelectedEntry.Entry.SetField("marquee", DetailMarqueeEdit.Trim());
        SelectedEntry.Entry.SetField("wheel", DetailWheelEdit.Trim());
        SelectedEntry.Entry.SetField("fanart", DetailFanartEdit.Trim());
        SelectedEntry.Entry.SetField("thumbnail", DetailThumbnailEdit.Trim());
        SelectedEntry.Entry.SetField("screenshot", DetailScreenshotEdit.Trim());
        SelectedEntry.Entry.SetBooleanField("favorite", DetailFavoriteEdit);
        SelectedEntry.Entry.SetBooleanField("hidden", DetailHiddenEdit);
        SelectedEntry.Entry.SetBooleanField("kidgame", DetailKidgameEdit);
        SelectedEntry.Refresh();

        IsDirty = true;
        UpdateWindowTitle();
        DetailTitle = DetailNameEdit.Trim();
        DetailSummary = BuildSummary(SelectedEntry);
        RefreshVisibleRows();
    }

    private void RefreshVisibleRows()
    {
        if (SelectedEntry is null)
            return;

        var selected = SelectedEntry;
        ApplySearchFilter();
        SelectedEntry = VisibleEntries.FirstOrDefault(entry => ReferenceEquals(entry.Entry, selected.Entry));
    }

    public void UpdateSelectedEntries(IReadOnlyList<GameRowViewModel> entries)
    {
        _selectedEntries = entries.ToList();
        Logger.Debug("Grid selection changed. Selected entries: {Count}", _selectedEntries.Count);
        OnPropertyChanged(nameof(HasMultiSelection));
        HideSelectedCommand.NotifyCanExecuteChanged();
        UnhideSelectedCommand.NotifyCanExecuteChanged();
        FavoriteSelectedCommand.NotifyCanExecuteChanged();
        UnfavoriteSelectedCommand.NotifyCanExecuteChanged();
        ToggleHiddenSelectedCommand.NotifyCanExecuteChanged();
        ToggleFavoriteSelectedCommand.NotifyCanExecuteChanged();
        SetNameFromFilenameCommand.NotifyCanExecuteChanged();
        RemoveSelectedEntriesCommand.NotifyCanExecuteChanged();
        ScrapeSelectedCommand.NotifyCanExecuteChanged();
    }

    private void RegisterRecentFile(string filePath)
    {
        _settings.RecentFiles.RemoveAll(path => path.Equals(filePath, StringComparison.OrdinalIgnoreCase));
        _settings.RecentFiles.Insert(0, filePath);
        if (_settings.RecentFiles.Count > 10)
            _settings.RecentFiles = _settings.RecentFiles.Take(10).ToList();
        SyncRecentFiles();
    }

    private void ApplyColumnSettings()
    {
        _suppressEditorApply = true;
        ShowNameColumn = _settings.ColumnsVisible.Name;
        ShowGenreColumn = _settings.ColumnsVisible.Genre;
        ShowDeveloperColumn = _settings.ColumnsVisible.Developer;
        ShowHiddenColumn = _settings.ColumnsVisible.Hidden;
        ShowFavoriteColumn = _settings.ColumnsVisible.Favorite;
        _suppressEditorApply = false;
    }

    private async Task LoadFileAsync(string filePath)
    {
        Logger.Debug("LoadFileAsync start for {FilePath}", filePath);
        _loadedDocument = await GamelistService.LoadAsync(filePath);
        _allEntries = _loadedDocument.Entries.Select(static entry => new GameRowViewModel(entry)).ToList();
        _selectedEntries.Clear();
        CurrentFilePath = filePath;
        IsDirty = false;
        _settings.LastGamelistDirectory = Path.GetDirectoryName(filePath) ?? string.Empty;
        RegisterRecentFile(filePath);
        await SettingsService.SaveAsync(AppPaths.SettingsPath, _settings);
        SelectedFilterIndex = 0;
        ApplySearchFilter();
        SelectedEntry = VisibleEntries.FirstOrDefault();
        EntryCountText = $"{_allEntries.Count} entries";
        UpdateWindowTitle();
        StatusText = $"Loaded {_allEntries.Count} entries from {Path.GetFileName(filePath)}.";
        Logger.Information("Loaded {Count} entries from {FilePath}", _allEntries.Count, filePath);
        OnPropertyChanged(nameof(HasLoadedDocument));
        SaveCommand.NotifyCanExecuteChanged();
        SaveAsCommand.NotifyCanExecuteChanged();
        RemoveEntryCommand.NotifyCanExecuteChanged();
        ReloadCommand.NotifyCanExecuteChanged();
        RestoreBackupCommand.NotifyCanExecuteChanged();
        AddGameCommand.NotifyCanExecuteChanged();
        AddFolderCommand.NotifyCanExecuteChanged();
        OptimizeCommand.NotifyCanExecuteChanged();
        CleanupCommand.NotifyCanExecuteChanged();
        OpenRecentCommand.NotifyCanExecuteChanged();
        HideSelectedCommand.NotifyCanExecuteChanged();
        UnhideSelectedCommand.NotifyCanExecuteChanged();
        FavoriteSelectedCommand.NotifyCanExecuteChanged();
        UnfavoriteSelectedCommand.NotifyCanExecuteChanged();
        ToggleHiddenSelectedCommand.NotifyCanExecuteChanged();
        ToggleFavoriteSelectedCommand.NotifyCanExecuteChanged();
        SetNameFromFilenameCommand.NotifyCanExecuteChanged();
        RemoveSelectedEntriesCommand.NotifyCanExecuteChanged();
        ScrapeSelectedCommand.NotifyCanExecuteChanged();
        ScrapeAllCommand.NotifyCanExecuteChanged();
        BatchFavoriteCommand.NotifyCanExecuteChanged();
        DetectDuplicatesCommand.NotifyCanExecuteChanged();
        DetectBadVersionsCommand.NotifyCanExecuteChanged();
        ReviewHiddenFavoritesCommand.NotifyCanExecuteChanged();
    }

    private void SyncRecentFiles()
    {
        RecentFiles.Clear();
        foreach (var recentFile in _settings.RecentFiles)
            RecentFiles.Add(new RecentFileViewModel(recentFile));

        OpenFlyoutItems.Clear();
        OpenFlyoutItems.Add(new OpenFlyoutItem("Open File...", OpenCommand));
        OpenFlyoutItems.Add(new OpenFlyoutItem("-"));
        foreach (var recent in RecentFiles)
            OpenFlyoutItems.Add(new OpenFlyoutItem(recent.DisplayName, OpenRecentCommand, recent.FullPath, recent.FullPath));

        OnPropertyChanged(nameof(HasRecentFiles));
        OpenRecentCommand.NotifyCanExecuteChanged();
    }

    private void PersistColumnVisibility(Action<ColumnVisibilitySettings> apply)
    {
        if (_suppressEditorApply)
            return;

        apply(_settings.ColumnsVisible);
        _ = SettingsService.SaveAsync(AppPaths.SettingsPath, _settings);
    }

    private async Task<(string Platform, string ExtraArguments)?> PromptForScrapeOptionsAsync(string title)
    {
        if (ChooseScrapeOptionsAsync is null)
        {
            Logger.Warning("Scrape options requested before delegate wiring");
            StatusText = "Scrape options dialog is not wired yet.";
            return null;
        }

        if (string.IsNullOrWhiteSpace(_settings.ScreenScraperUser)
            || string.IsNullOrWhiteSpace(_settings.ScreenScraperPassword))
        {
            Logger.Warning("Scrape attempted without ScreenScraper credentials configured");
            StatusText = "Configure ScreenScraper credentials in Settings first.";
            return null;
        }

        var binary = SkyscraperService.FindBinary(_settings.SkyscraperBinaryPath);
        if (binary is null)
        {
            Logger.Warning("Scrape attempted without valid Skyscraper binary");
            StatusText = "Skyscraper binary not found. Configure it in Settings.";
            return null;
        }

        var suggestedPlatform = MetadataDefinitions.InferPlatform(
            _loadedDocument is not null ? Path.GetFileName(Path.TrimEndingDirectorySeparator(_loadedDocument.BaseDirectory)) : null);
        if (suggestedPlatform is not null)
            Logger.Debug("Inferred platform {Platform} from base directory", suggestedPlatform);

        return await ChooseScrapeOptionsAsync(title, suggestedPlatform);
    }

    private async Task<SkyscraperService.RunResult?> RunScrapeAsync(string title, ScrapeRequest request)
    {
        if (_loadedDocument is null || ShowScrapeProgressAsync is null)
            return null;

        var binary = SkyscraperService.FindBinary(_settings.SkyscraperBinaryPath);
        if (binary is null)
        {
            Logger.Warning("RunScrapeAsync aborted because binary was not found");
            StatusText = "Skyscraper binary not found. Configure it in Settings.";
            return null;
        }

        Logger.Information("Preparing scrape run {Title} with binary {Binary}", title, binary);
        var credentialsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".skyscraper", "config.ini");
        await SkyscraperService.WriteCredentialsAsync(credentialsPath, _settings.ScreenScraperUser, _settings.ScreenScraperPassword);
        Logger.Debug("Skyscraper credentials written to {Path}", credentialsPath);

        var command = SkyscraperService.BuildCommand(binary, request);
        Logger.Debug("Scrape command: {Command}", string.Join(" ", command));
        var progressViewModel = new ScrapeProgressViewModel { Title = title, Status = "Starting Skyscraper..." };
        using var cts = new CancellationTokenSource();
        progressViewModel.AttachCancellation(cts);

        var showTask = ShowScrapeProgressAsync(progressViewModel);
        var result = await SkyscraperService.RunAsync(command, line => progressViewModel.AppendLog(line), cts.Token);
        Logger.Information("Scrape result for {Title}: ExitCode={ExitCode}, Cancelled={Cancelled}", title, result.ExitCode, result.WasCancelled);

        progressViewModel.Status = result.WasCancelled
            ? "Scrape cancelled."
            : result.ExitCode == 0
                ? "Scrape completed successfully."
                : $"Scrape failed with exit code {result.ExitCode}.";
        progressViewModel.CanCancel = false;
        progressViewModel.CanClose = true;
        await showTask;
        return result;
    }

    private async Task RefreshAfterScrapeAsync()
    {
        if (_loadedDocument?.SourcePath is null)
            return;

        Logger.Information("Refreshing loaded document after scrape from {Path}", _loadedDocument.SourcePath);
        await LoadFileAsync(_loadedDocument.SourcePath);
    }

    private static List<string> ParseArguments(string rawArguments)
    {
        return rawArguments
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();
    }

    private void ApplyBulkBooleanField(string fieldName, bool value, string statusMessage)
    {
        if (_selectedEntries.Count == 0)
            return;

        Logger.Information("Bulk action: {Field}={Value} on {Count} entries", fieldName, value, _selectedEntries.Count);
        foreach (var row in _selectedEntries)
        {
            row.Entry.SetBooleanField(fieldName, value);
            row.Refresh();
        }

        IsDirty = true;
        UpdateWindowTitle();
        StatusText = statusMessage;
        if (SelectedEntry is not null)
            UpdateDetailPane(SelectedEntry);
        RefreshVisibleRows();
    }

    private static string BuildSummary(GameRowViewModel row)
    {
        var parts = new List<string>
        {
            $"Type: {row.ItemType}",
            $"Favorite: {(row.Favorite ? "Yes" : "No")}",
            $"Hidden: {(row.Hidden ? "Yes" : "No")}",
        };

        if (!string.IsNullOrWhiteSpace(row.Genre))
            parts.Add($"Genre: {row.Genre}");

        if (!string.IsNullOrWhiteSpace(row.Developer))
            parts.Add($"Developer: {row.Developer}");

        if (!string.IsNullOrWhiteSpace(row.Description))
            parts.Add($"Description: {row.Description}");

        return string.Join(Environment.NewLine, parts);
    }

    private void UpdateWindowTitle()
    {
        var fileName = _loadedDocument?.SourcePath is null ? null : Path.GetFileName(_loadedDocument.SourcePath);
        var dirtyMarker = IsDirty ? "*" : string.Empty;
        WindowTitle = fileName is null
            ? $"{ProjectInfo.ProjectName} {BuildInfo.DisplayVersion}"
            : $"{ProjectInfo.ProjectName} {BuildInfo.DisplayVersion} - {fileName}{dirtyMarker}";
    }
}
