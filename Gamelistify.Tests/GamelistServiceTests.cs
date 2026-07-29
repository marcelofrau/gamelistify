using Gamelistify.Models;
using Gamelistify.Services;

namespace Gamelistify.Tests;

public sealed class GamelistServiceTests
{
    [Fact]
    public async Task LoadAsync_reads_entries_and_preserves_unknown_elements()
    {
        var fixturePath = GetFixturePath("sample-gamelist.xml");
        var document = await GamelistService.LoadAsync(fixturePath);

        Assert.Equal(2, document.Entries.Count);
        Assert.Equal(GamelistEntryKind.Game, document.Entries[0].Kind);
        Assert.Equal("./Alien Soldier.zip", document.Entries[0].Path);
        Assert.Single(document.Entries[0].UnknownElements);
        Assert.Equal("customTag", document.Entries[0].UnknownElements[0].Name.LocalName);
        Assert.True(document.Entries[1].GetBooleanField("hidden"));
    }

    [Fact]
    public async Task SaveAsync_writes_normalized_xml_and_creates_backup()
    {
        var tempDirectory = CreateTempDirectory();
        try
        {
            var sourcePath = Path.Combine(tempDirectory, "gamelist.xml");
            File.Copy(GetFixturePath("sample-gamelist.xml"), sourcePath);

            var document = await GamelistService.LoadAsync(sourcePath);
            document.Entries[0].SetField("name", "Alien Soldier (USA)");

            await GamelistService.SaveAsync(document);

            var savedXml = await File.ReadAllTextAsync(sourcePath);
            Assert.Contains("Alien Soldier (USA)", savedXml, StringComparison.Ordinal);
            Assert.Contains("<customTag>keep me</customTag>", savedXml, StringComparison.Ordinal);
            Assert.Single(Directory.GetFiles(tempDirectory, "*.bak"));
        }
        finally
        {
            Directory.Delete(tempDirectory, recursive: true);
        }
    }

    private static string GetFixturePath(string fileName)
    {
        return Path.Combine(AppContext.BaseDirectory, "Fixtures", fileName);
    }

    private static string CreateTempDirectory()
    {
        var path = Path.Combine(Path.GetTempPath(), "Gamelistify.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path);
        return path;
    }
}
