using Avalonia;
using System;
using System.IO;
using System.Runtime.InteropServices;
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
        var consoleAttached = TryAttachParentConsole();
        Logger.Init(consoleEnabled: consoleAttached);
        if (consoleAttached)
            Logger.Information("Console log output enabled. Run from a terminal to see logs here.");
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

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool AttachConsole(uint dwProcessId);

    // ATTACH_PARENT_PROCESS: attach to the console that launched this app.
    // Works because the app is a WinExe (no console of its own). Fails (returns
    // false) when launched by double-click with no parent console.
    private static bool TryAttachParentConsole()
    {
        if (!OperatingSystem.IsWindows())
            return false;

        try
        {
            if (!AttachConsole(0xFFFFFFFF))
                return false;

            var stdout = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true };
            Console.SetOut(stdout);
            var stderr = new StreamWriter(Console.OpenStandardError()) { AutoFlush = true };
            Console.SetError(stderr);
            return true;
        }
        catch
        {
            return false;
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>()
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
