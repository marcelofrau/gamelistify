using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Gamelistify.Models;
using Gamelistify.Services;
using Gamelistify.ViewModels;
using Gamelistify.Views;

namespace Gamelistify;

public partial class App : Application
{
    private const int SplashDelayMs = 1200;

    public override void Initialize()
    {
        Logger.Verbose("App.Initialize start");
        AvaloniaXamlLoader.Load(this);
        Logger.Verbose("App.Initialize end");
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Logger.Information("App framework initialization completed");
        SetupGlobalExceptionHandling();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            Logger.Debug("Creating startup windows and view models");
            var mainViewModel = new MainViewModel();
            var mainWindow = new MainWindow
            {
                DataContext = mainViewModel,
            };

            var splashWindow = new SplashWindow();
            splashWindow.Show();
            Logger.Information("Splash window shown");

            mainViewModel.PickGamelistFileAsync = async () =>
            {
                Logger.Debug("Open file dialog requested");
                var files = await mainWindow.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                {
                    Title = "Open gamelist.xml",
                    AllowMultiple = false,
                    FileTypeFilter =
                    [
                        new FilePickerFileType("Gamelist XML")
                        {
                            Patterns = ["*.xml"],
                            MimeTypes = ["application/xml", "text/xml"],
                        },
                    ],
                });

                var path = files.Count > 0 ? files[0].TryGetLocalPath() : null;
                Logger.Debug("Open file dialog result: {Path}", path ?? "<none>");
                return path;
            };

            mainViewModel.PickFolderAsync = async suggestedPath =>
            {
                Logger.Debug("Open folder dialog requested. Suggested: {Path}", suggestedPath ?? "<none>");
                IStorageFolder? suggestedStartLocation = null;
                if (!string.IsNullOrWhiteSpace(suggestedPath))
                    suggestedStartLocation = await mainWindow.StorageProvider.TryGetFolderFromPathAsync(suggestedPath);

                var folders = await mainWindow.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
                {
                    Title = "Select ROM directory",
                    AllowMultiple = false,
                    SuggestedStartLocation = suggestedStartLocation,
                });

                var path = folders.Count > 0 ? folders[0].TryGetLocalPath() : null;
                Logger.Debug("Open folder dialog result: {Path}", path ?? "<none>");
                return path;
            };

            mainViewModel.ShowAboutAsync = async () =>
            {
                Logger.Information("Showing About window");
                var aboutWindow = new AboutWindow();
                await aboutWindow.ShowDialog(mainWindow);
                Logger.Information("About window closed");
            };

            mainViewModel.EditSettingsAsync = async currentSettings =>
            {
                Logger.Information("Showing Settings window");
                var settingsViewModel = new SettingsViewModel();
                settingsViewModel.LoadFrom(currentSettings);
                var settingsWindow = new SettingsWindow
                {
                    DataContext = settingsViewModel,
                };

                var result = await settingsWindow.ShowDialog<bool?>(mainWindow);
                if (result != true)
                {
                    Logger.Debug("Settings window closed without save");
                    return null;
                }

                var updated = new AppSettings
                {
                    SkyscraperBinaryPath = currentSettings.SkyscraperBinaryPath,
                    ScreenScraperUser = currentSettings.ScreenScraperUser,
                    ScreenScraperPassword = currentSettings.ScreenScraperPassword,
                    LastGamelistDirectory = currentSettings.LastGamelistDirectory,
                    Theme = currentSettings.Theme,
                    ImagePreviewSize = currentSettings.ImagePreviewSize,
                    DebugLogging = currentSettings.DebugLogging,
                    RecentFiles = [.. currentSettings.RecentFiles],
                    ColumnsVisible = currentSettings.ColumnsVisible,
                };

                settingsViewModel.ApplyTo(updated);
                Logger.Information("Settings accepted and applied");
                return updated;
            };

            mainViewModel.ConfirmAsync = async (title, message) =>
            {
                Logger.Information("Showing confirmation dialog: {Title}", title);
                var confirmWindow = new ConfirmWindow
                {
                    DataContext = new ConfirmViewModel
                    {
                        Title = title,
                        Message = message,
                    },
                };
                var result = await confirmWindow.ShowDialog<bool>(mainWindow);
                Logger.Debug("Confirmation result: {Result}", result);
                return result;
            };

            mainViewModel.ReviewScannedRomsAsync = async (items, directory) =>
            {
                Logger.Information("Showing Scan ROMs review window with {Count} candidate entries", items.Count);
                var scanViewModel = new ScanRomsViewModel(items, directory);
                var scanWindow = new ScanRomsWindow
                {
                    DataContext = scanViewModel,
                };

                var result = await scanWindow.ShowDialog<bool?>(mainWindow);
                if (result != true)
                {
                    Logger.Debug("Scan ROMs review window cancelled");
                    return null;
                }

                Logger.Information("Scan ROMs review accepted with {Count} selected entries", scanViewModel.SelectedItems.Count);
                return scanViewModel.SelectedItems;
            };

            mainViewModel.ChooseScrapeOptionsAsync = async title =>
            {
                Logger.Information("Showing scrape options window: {Title}", title);
                var optionsViewModel = new ScrapeOptionsViewModel(title);
                var optionsWindow = new ScrapeOptionsWindow
                {
                    DataContext = optionsViewModel,
                };

                var result = await optionsWindow.ShowDialog<bool?>(mainWindow);
                if (result != true)
                {
                    Logger.Debug("Scrape options window cancelled");
                    return null;
                }

                Logger.Information("Scrape options selected. Platform: {Platform}, ExtraArgs: {Args}", optionsViewModel.SelectedPlatform, optionsViewModel.ExtraArguments);
                return (optionsViewModel.SelectedPlatform, optionsViewModel.ExtraArguments);
            };

            mainViewModel.ShowScrapeProgressAsync = async progressViewModel =>
            {
                Logger.Information("Showing scrape progress window: {Title}", progressViewModel.Title);
                var progressWindow = new ScrapeProgressWindow
                {
                    DataContext = progressViewModel,
                };

                await progressWindow.ShowDialog(mainWindow);
                Logger.Information("Scrape progress window closed: {Title}", progressViewModel.Title);
            };

            desktop.MainWindow = mainWindow;
            Logger.Information("Main window assigned to desktop lifetime");

            _ = InitializeMainWindowAsync(mainViewModel, mainWindow, splashWindow);
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static async Task InitializeMainWindowAsync(MainViewModel mainViewModel, Window mainWindow, Window splashWindow)
    {
        try
        {
            Logger.Information("Async startup initialization started");
            await mainViewModel.InitializeAsync();
            await Task.Delay(SplashDelayMs);

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (splashWindow.IsVisible)
                    splashWindow.Close();

                if (!mainWindow.IsVisible)
                    mainWindow.Show();

                Logger.Information("Splash closed and main window shown");
            });
        }
        catch (Exception ex)
        {
            Logger.Fatal(ex, "Fatal exception during async startup initialization");
            throw;
        }
    }

    private static void SetupGlobalExceptionHandling()
    {
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            if (e.ExceptionObject is Exception ex)
                Logger.Fatal(ex, "AppDomain unhandled exception");
            else
                Logger.Error("AppDomain unhandled payload: {Payload}", e.ExceptionObject?.ToString() ?? "<null>");
        };

        TaskScheduler.UnobservedTaskException += (_, e) =>
        {
            Logger.Error(e.Exception, "Unobserved task exception");
            e.SetObserved();
        };

        Dispatcher.UIThread.UnhandledException += (_, e) =>
        {
            Logger.Error(e.Exception, "UI thread unhandled exception");
            e.Handled = true;
        };
    }
}
