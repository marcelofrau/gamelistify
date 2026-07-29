using Gamelistify.Models;
using Gamelistify.Services;

namespace Gamelistify.Tests;

public sealed class SettingsServiceTests
{
    [Fact]
    public async Task SaveAsync_and_LoadAsync_round_trip_settings()
    {
        var tempDirectory = CreateTempDirectory();
        try
        {
            var settingsPath = Path.Combine(tempDirectory, "settings.json");
            var settings = new AppSettings
            {
                SkyscraperBinaryPath = "/usr/bin/Skyscraper",
                ScreenScraperUser = "user",
                RecentFiles = ["a.xml", "b.xml"],
            };

            await SettingsService.SaveAsync(settingsPath, settings);
            var loaded = await SettingsService.LoadAsync(settingsPath);

            Assert.Equal("/usr/bin/Skyscraper", loaded.SkyscraperBinaryPath);
            Assert.Equal("user", loaded.ScreenScraperUser);
            Assert.Equal(2, loaded.RecentFiles.Count);
        }
        finally
        {
            Directory.Delete(tempDirectory, recursive: true);
        }
    }

    private static string CreateTempDirectory()
    {
        var path = Path.Combine(Path.GetTempPath(), "Gamelistify.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path);
        return path;
    }
}
