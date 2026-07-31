using Serilog;
using Serilog.Core;
using Serilog.Events;
using System.Globalization;

namespace Gamelistify.Services;

public static class Logger
{
    private static bool _initialized;
    private static LoggingLevelSwitch? _levelSwitch;

    public static void Init(LogEventLevel minimumLevel = LogEventLevel.Verbose, bool consoleEnabled = false)
    {
        if (_initialized)
            return;

        _levelSwitch = new LoggingLevelSwitch(minimumLevel);

        var logDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Gamelistify",
            "logs");

        Directory.CreateDirectory(logDirectory);

        var configuration = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(_levelSwitch);

        if (consoleEnabled)
        {
            configuration = configuration.WriteTo.Console(
                formatProvider: CultureInfo.InvariantCulture,
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
        }

        Log.Logger = configuration
            .WriteTo.File(
                Path.Combine(logDirectory, "gamelistify-.log"),
                formatProvider: CultureInfo.InvariantCulture,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 10,
                retainedFileTimeLimit: TimeSpan.FromDays(5),
                shared: true,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        _initialized = true;
        Log.Verbose("Logger initialized. Directory: {Directory}. Rolling: daily, retained 5 days (count cap 10).", logDirectory);
    }

    public static void SetMinimumLevel(LogEventLevel level)
    {
        if (_levelSwitch is null)
            return;
        _levelSwitch.MinimumLevel = level;
        Log.Information("Log level changed to {Level}", level);
    }

    public static string CurrentLevelName => _levelSwitch?.MinimumLevel.ToString() ?? "Verbose";

    public static LogEventLevel CurrentLevel => _levelSwitch?.MinimumLevel ?? LogEventLevel.Verbose;

    public static void Verbose(string template, params object[] args) => Log.Verbose(template, args);
    public static void Debug(string template, params object[] args) => Log.Debug(template, args);
    public static void Information(string template, params object[] args) => Log.Information(template, args);
    public static void Warning(string template, params object[] args) => Log.Warning(template, args);
    public static void Error(string template, params object[] args) => Log.Error(template, args);
    public static void Error(Exception exception, string template, params object[] args) => Log.Error(exception, template, args);
    public static void Fatal(Exception exception, string template, params object[] args) => Log.Fatal(exception, template, args);

    public static ValueTask ShutdownAsync() => Log.CloseAndFlushAsync();
}
