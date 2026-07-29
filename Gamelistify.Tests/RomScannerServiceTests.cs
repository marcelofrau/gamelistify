using Gamelistify.Models;
using Gamelistify.Services;

namespace Gamelistify.Tests;

public sealed class RomScannerServiceTests
{
    [Fact]
    public void Scan_finds_roms_and_skips_media_directories()
    {
        var tempDirectory = CreateTempDirectory();
        try
        {
            File.WriteAllText(Path.Combine(tempDirectory, "Alien Soldier.zip"), string.Empty);
            Directory.CreateDirectory(Path.Combine(tempDirectory, "images"));
            File.WriteAllText(Path.Combine(tempDirectory, "images", "Ignored.zip"), string.Empty);

            var results = RomScannerService.Scan(tempDirectory);

            Assert.Single(results);
            Assert.Equal("./Alien Soldier.zip", results[0].RelativePath);
        }
        finally
        {
            Directory.Delete(tempDirectory, recursive: true);
        }
    }

    [Fact]
    public void FindMissingEntries_filters_existing_gamelist_paths()
    {
        var scanned = new[]
        {
            new ScannedRom("./Alien Soldier.zip", "Alien Soldier", "c:\\roms\\Alien Soldier.zip"),
            new ScannedRom("./Panzer Dragoon Saga.chd", "Panzer Dragoon Saga", "c:\\roms\\Panzer Dragoon Saga.chd"),
        };

        var document = new GamelistDocument();
        var entry = new GamelistEntry(GamelistEntryKind.Game);
        entry.SetField("path", "./Alien Soldier.zip");
        document.Entries.Add(entry);

        var missing = RomScannerService.FindMissingEntries(scanned, document);

        Assert.Single(missing);
        Assert.Equal("./Panzer Dragoon Saga.chd", missing[0].RelativePath);
    }

    private static string CreateTempDirectory()
    {
        var path = Path.Combine(Path.GetTempPath(), "Gamelistify.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path);
        return path;
    }
}
