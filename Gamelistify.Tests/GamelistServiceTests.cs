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

            var backupPath = await GamelistService.SaveAsync(document);

            var savedXml = await File.ReadAllTextAsync(sourcePath);
            Assert.Contains("Alien Soldier (USA)", savedXml, StringComparison.Ordinal);
            Assert.Contains("<customTag>keep me</customTag>", savedXml, StringComparison.Ordinal);
            Assert.NotNull(backupPath);
            Assert.True(File.Exists(backupPath));
            var backupDirectory = Path.Combine(tempDirectory, "gamelists_backup");
            Assert.Single(Directory.GetFiles(backupDirectory, "*.bak"));
            Assert.Equal(Path.GetFileName(backupPath), Path.GetFileName(Directory.GetFiles(backupDirectory, "*.bak").Single()));
        }
        finally
        {
            Directory.Delete(tempDirectory, recursive: true);
        }
    }

    [Fact]
    public async Task SaveAsync_compact_writes_minified_single_line_xml()
    {
        var tempDirectory = CreateTempDirectory();
        try
        {
            var sourcePath = Path.Combine(tempDirectory, "gamelist.xml");
            File.Copy(GetFixturePath("sample-gamelist.xml"), sourcePath);

            var document = await GamelistService.LoadAsync(sourcePath);

            await GamelistService.SaveAsync(document, compact: true);

            var savedXml = await File.ReadAllTextAsync(sourcePath);
            Assert.DoesNotContain(Environment.NewLine, savedXml);
            Assert.Contains("<game>", savedXml, StringComparison.Ordinal);
        }
        finally
        {
            Directory.Delete(tempDirectory, recursive: true);
        }
    }

    [Fact]
    public async Task GetInvalidEntries_detects_missing_files_and_skips_folders()
    {
        var tempDirectory = CreateTempDirectory();
        try
        {
            var sourcePath = Path.Combine(tempDirectory, "gamelist.xml");
            await File.WriteAllTextAsync(sourcePath, """
                <?xml version="1.0" encoding="UTF-8"?>
                <gameList>
                  <game>
                    <path>./missing.zip</path>
                    <name>Missing</name>
                  </game>
                  <game>
                    <path>./existing.zip</path>
                    <name>Existing</name>
                  </game>
                  <game>
                    <name>No Path</name>
                  </game>
                  <folder>
                    <path>./Shmups</path>
                    <name>Shmups</name>
                  </folder>
                </gameList>
                """);

            File.WriteAllBytes(Path.Combine(tempDirectory, "existing.zip"), [1, 2, 3]);
            var document = await GamelistService.LoadAsync(sourcePath);

            var invalid = GamelistService.GetInvalidEntries(document);

            Assert.Equal(2, invalid.Count);
            Assert.Contains(document.Entries[0], invalid);
            Assert.DoesNotContain(document.Entries[1], invalid);
            Assert.Contains(document.Entries[2], invalid);
            Assert.DoesNotContain(document.Entries[3], invalid);
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
