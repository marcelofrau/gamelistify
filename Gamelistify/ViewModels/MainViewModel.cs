using System.Collections.ObjectModel;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gamelistify.Helpers;
using Gamelistify.Models;
using Gamelistify.Services;

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

    [ObservableProperty]
    private string _statusText = "Ready. Open a gamelist.xml file to begin.";

    [ObservableProperty]
    private string _windowTitle = $"{ProjectInfo.ProjectName} {BuildInfo.DisplayVersion}";

    [ObservableProperty]
    private string _detailTitle = "No selection";

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
    private string? _selectedRecentFile;

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
    private bool _isDirty;

    [ObservableProperty]
    private Bitmap? _detailImage;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasDetailImage))]
    [NotifyPropertyChangedFor(nameof(ShowPreviewPlaceholder))]
    private GameRowViewModel? _selectedEntry;

    public ObservableCollection<GameRowViewModel> VisibleEntries { get; } = [];

    public ObservableCollection<string> RecentFiles { get; } = [];

    public Func<Task<string?>>? PickGamelistFileAsync { get; set; }

    public Func<string?, Task<string?>>? PickFolderAsync { get; set; }

    public Func<Task>? ShowAboutAsync { get; set; }

    public Func<AppSettings, Task<AppSettings?>>? EditSettingsAsync { get; set; }

    public Func<IReadOnlyList<ScannedRomItemViewModel>, string, Task<IReadOnlyList<ScannedRomItemViewModel>?>>? ReviewScannedRomsAsync { get; set; }

    public Func<string, Task<(string Platform, string ExtraArguments)?>>? ChooseScrapeOptionsAsync { get; set; }

    public Func<ScrapeProgressViewModel, Task>? ShowScrapeProgressAsync { get; set; }

    public bool HasLoadedDocument => _loadedDocument is not null;

    public bool HasDetailImage => DetailImage is not null;

    public bool ShowPreviewPlaceholder => !HasDetailImage;

    public bool HasRecentFiles => RecentFiles.Count > 0;

    public bool HasMultiSelection => _selectedEntries.Count > 1;

    partial void OnSearchTextChanged(string value)
    {
        ApplySearchFilter();
    }

    partial void OnSelectedEntryChanged(GameRowViewModel? value)
    {
        UpdateDetailPane(value);
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

    partial void OnShowNameColumnChanged(bool value) => PersistColumnVisibility(columns => columns.Name = value);

    partial void OnShowGenreColumnChanged(bool value) => PersistColumnVisibility(columns => columns.Genre = value);

    partial void OnShowDeveloperColumnChanged(bool value) => PersistColumnVisibility(columns => columns.Developer = value);

    partial void OnShowHiddenColumnChanged(bool value) => PersistColumnVisibility(columns => columns.Hidden = value);

    partial void OnShowFavoriteColumnChanged(bool value) => PersistColumnVisibility(columns => columns.Favorite = value);

    public async Task InitializeAsync()
    {
        Logger.Verbose("MainViewModel.InitializeAsync start");
        _settings = await SettingsService.LoadAsync(AppPaths.SettingsPath);
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

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAsync()
    {
        if (_loadedDocument is null || string.IsNullOrWhiteSpace(_loadedDocument.SourcePath))
            return;

        Logger.Information("Saving current gamelist to {Path}", _loadedDocument.SourcePath);
        await GamelistService.SaveAsync(_loadedDocument);
        IsDirty = false;
        UpdateWindowTitle();
        StatusText = $"Saved {Path.GetFileName(_loadedDocument.SourcePath)} with timestamped backup.";
    }

    private bool CanSave() => HasLoadedDocument;

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

    [RelayCommand]
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

    [RelayCommand]
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
    private async Task OpenRecentAsync()
    {
        if (string.IsNullOrWhiteSpace(SelectedRecentFile))
            return;

        if (!File.Exists(SelectedRecentFile))
        {
            Logger.Warning("Recent file missing: {RecentFile}", SelectedRecentFile);
            StatusText = "Recent file no longer exists.";
            _settings.RecentFiles.RemoveAll(path => path.Equals(SelectedRecentFile, StringComparison.OrdinalIgnoreCase));
            SyncRecentFiles();
            await SettingsService.SaveAsync(AppPaths.SettingsPath, _settings);
            return;
        }

        Logger.Information("Opening recent file {RecentFile}", SelectedRecentFile);
        await LoadFileAsync(SelectedRecentFile);
    }

    private bool CanOpenRecent() => !string.IsNullOrWhiteSpace(SelectedRecentFile);

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

    private bool CanRunBulkAction() => _selectedEntries.Count > 0;

    private void ApplySearchFilter()
    {
        var search = SearchText.Trim();
        IEnumerable<GameRowViewModel> filtered = _allEntries;

        if (!string.IsNullOrWhiteSpace(search))
        {
            filtered = filtered.Where(entry =>
                entry.Name.Contains(search, StringComparison.OrdinalIgnoreCase)
                || entry.Genre.Contains(search, StringComparison.OrdinalIgnoreCase)
                || entry.Developer.Contains(search, StringComparison.OrdinalIgnoreCase)
                || entry.Path.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        VisibleEntries.Clear();
        foreach (var entry in filtered)
            VisibleEntries.Add(entry);

        EntryCountText = $"{VisibleEntries.Count} visible / {_allEntries.Count} total";
    }

    private void UpdateDetailPane(GameRowViewModel? row)
    {
        DetailImage?.Dispose();
        DetailImage = null;
        _suppressEditorApply = true;

        if (row is null)
        {
            DetailTitle = "No selection";
            DetailPath = string.Empty;
            DetailSummary = "Select an entry to inspect metadata and preview image assets.";
            DetailImageStatus = "No image loaded.";
            DetailNameEdit = string.Empty;
            DetailGenreEdit = string.Empty;
            DetailDeveloperEdit = string.Empty;
            DetailDescriptionEdit = string.Empty;
            DetailFavoriteEdit = false;
            DetailHiddenEdit = false;
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
        _suppressEditorApply = false;

        if (_loadedDocument is null)
        {
            DetailImageStatus = "No document loaded.";
            return;
        }

        var resolvedImagePath = row.ImagePath is null
            ? null
            : MediaResolverService.ResolveMediaPath(_loadedDocument.BaseDirectory, row.ImagePath);

        if (resolvedImagePath is null)
        {
            DetailImageStatus = "No mapped image found for current selection.";
            return;
        }

        using var stream = File.OpenRead(resolvedImagePath);
        DetailImage = new Bitmap(stream);
        DetailImageStatus = $"Preview source: {Path.GetFileName(resolvedImagePath)}";
    }

    private void ApplyEditorChanges()
    {
        if (_suppressEditorApply)
            return;

        if (_loadedDocument is null || SelectedEntry?.Entry is null)
            return;

        SelectedEntry.Entry.SetField("name", DetailNameEdit.Trim());
        SelectedEntry.Entry.SetField("genre", DetailGenreEdit.Trim());
        SelectedEntry.Entry.SetField("developer", DetailDeveloperEdit.Trim());
        SelectedEntry.Entry.SetField("desc", DetailDescriptionEdit.Trim());
        SelectedEntry.Entry.SetBooleanField("favorite", DetailFavoriteEdit);
        SelectedEntry.Entry.SetBooleanField("hidden", DetailHiddenEdit);

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
        ApplySearchFilter();
        SelectedEntry = VisibleEntries.FirstOrDefault();
        EntryCountText = $"{_allEntries.Count} entries";
        UpdateWindowTitle();
        StatusText = $"Loaded {_allEntries.Count} entries from {Path.GetFileName(filePath)}.";
        Logger.Information("Loaded {Count} entries from {FilePath}", _allEntries.Count, filePath);
        OnPropertyChanged(nameof(HasLoadedDocument));
        SaveCommand.NotifyCanExecuteChanged();
        OpenRecentCommand.NotifyCanExecuteChanged();
        HideSelectedCommand.NotifyCanExecuteChanged();
        UnhideSelectedCommand.NotifyCanExecuteChanged();
        FavoriteSelectedCommand.NotifyCanExecuteChanged();
        UnfavoriteSelectedCommand.NotifyCanExecuteChanged();
    }

    private void SyncRecentFiles()
    {
        RecentFiles.Clear();
        foreach (var recentFile in _settings.RecentFiles)
            RecentFiles.Add(recentFile);

        SelectedRecentFile = RecentFiles.FirstOrDefault();
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

        return await ChooseScrapeOptionsAsync(title);
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

        foreach (var row in _selectedEntries)
            row.Entry.SetBooleanField(fieldName, value);

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
