using Avalonia;
using System;
using Avalonia.Media.Fonts;
using Gamelistify.Services;

namespace Gamelistify;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        Logger.Init();
        Logger.Information("Program.Main starting");

        try
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
            Logger.Information("Application lifetime exited normally");
        }
        catch (Exception ex)
        {
            Logger.Fatal(ex, "Fatal unhandled exception in Program.Main");
            throw;
        }
        finally
        {
            Logger.Information("Program.Main shutting down");
            Logger.ShutdownAsync().AsTask().GetAwaiter().GetResult();
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
#if DEBUG
            .WithDeveloperTools()
#endif
            .WithInterFont()
            .ConfigureFonts(fontManager => fontManager.AddFontCollection(
                new EmbeddedFontCollection(
                    new Uri("fonts:SpaceGrotesk", UriKind.Absolute),
                    new Uri("avares://Gamelistify/Assets/Fonts", UriKind.Absolute))))
            .LogToTrace();
}
