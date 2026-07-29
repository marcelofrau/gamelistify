using Gamelistify.Services;

namespace Gamelistify.Tests;

public sealed class BackupServiceTests
{
    [Fact]
    public async Task CreateBackupAsync_creates_timestamped_backup_for_xml_file()
    {
        var tempDirectory = CreateTempDirectory();
        try
        {
            var sourcePath = Path.Combine(tempDirectory, "gamelist.xml");
            await File.WriteAllTextAsync(sourcePath, "<gameList />");

            var backupPath = await BackupService.CreateBackupAsync(sourcePath, new DateTimeOffset(2026, 7, 22, 15, 30, 10, TimeSpan.Zero));

            Assert.True(File.Exists(backupPath));
            Assert.EndsWith("gamelist.2026-07-22T15-30-10.000.xml.bak", backupPath, StringComparison.Ordinal);
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
