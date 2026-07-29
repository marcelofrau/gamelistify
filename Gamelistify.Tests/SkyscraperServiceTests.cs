using Gamelistify.Models;
using Gamelistify.Services;

namespace Gamelistify.Tests;

public sealed class SkyscraperServiceTests
{
    [Fact]
    public void BuildCommand_creates_single_rom_command()
    {
        var request = new ScrapeRequest
        {
            Platform = "megadrive",
            RomsDirectory = "/roms/megadrive",
            SelectedRomPath = "./Alien Soldier.zip",
            MediaDirectory = "/roms/megadrive/images",
            ExtraArguments = ["--flags", "unattend"],
        };

        var command = SkyscraperService.BuildCommand("/usr/bin/Skyscraper", request);

        Assert.Equal("/usr/bin/Skyscraper", command[0]);
        Assert.Contains("./Alien Soldier.zip", command);
        Assert.Contains("--flags", command);
    }

    [Fact]
    public async Task WriteCredentialsAsync_writes_config_format()
    {
        var tempDirectory = CreateTempDirectory();
        try
        {
            var configPath = Path.Combine(tempDirectory, "config.ini");
            await SkyscraperService.WriteCredentialsAsync(configPath, "user", "pass");

            var content = await File.ReadAllTextAsync(configPath);
            Assert.Contains("[screenscraper]", content, StringComparison.Ordinal);
            Assert.Contains("userCreds=user:pass", content, StringComparison.Ordinal);
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
