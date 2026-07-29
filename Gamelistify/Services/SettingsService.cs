using System.Text.Json;
using Gamelistify.Models;

namespace Gamelistify.Services;

public sealed class SettingsService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
    };

    public static async Task<AppSettings> LoadAsync(string settingsPath, CancellationToken cancellationToken = default)
    {
        Logger.Debug("Loading settings from {SettingsPath}", settingsPath);
        if (!File.Exists(settingsPath))
        {
            Logger.Information("Settings file not found. Defaults will be used.");
            return new AppSettings();
        }

        await using var stream = File.OpenRead(settingsPath);
        var settings = await JsonSerializer.DeserializeAsync<AppSettings>(stream, JsonOptions, cancellationToken)
            ?? new AppSettings();
        Logger.Information("Settings loaded from {SettingsPath}", settingsPath);
        return settings;
    }

    public static async Task SaveAsync(string settingsPath, AppSettings settings, CancellationToken cancellationToken = default)
    {
        Logger.Debug("Saving settings to {SettingsPath}", settingsPath);
        var directory = Path.GetDirectoryName(settingsPath);
        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);

        await using var stream = File.Create(settingsPath);
        await JsonSerializer.SerializeAsync(stream, settings, JsonOptions, cancellationToken);
        Logger.Information("Settings saved to {SettingsPath}", settingsPath);
    }
}
