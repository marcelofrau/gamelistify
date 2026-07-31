using System.Globalization;
using Gamelistify.Services;

namespace Gamelistify.Tests;

public sealed class BackupServiceTests
{
    [Fact]
    public async Task CreateBackupAsync_creates_timestamped_backup_in_hidden_folder()
    {
        var tempDirectory = CreateTempDirectory();
        try
        {
            var sourcePath = Path.Combine(tempDirectory, "gamelist.xml");
            await File.WriteAllTextAsync(sourcePath, "<gameList />");

            var backupPath = await BackupService.CreateBackupAsync(sourcePath, new DateTimeOffset(2026, 7, 22, 15, 30, 10, TimeSpan.Zero));

            Assert.True(File.Exists(backupPath));
            Assert.Equal(Path.Combine(tempDirectory, "gamelists_backup", "gamelist.2026-07-22T15-30-10.000.xml.bak"), backupPath);
            Assert.True(Directory.Exists(Path.Combine(tempDirectory, "gamelists_backup")));
            if (OperatingSystem.IsWindows())
                Assert.True((File.GetAttributes(Path.Combine(tempDirectory, "gamelists_backup")) & FileAttributes.Hidden) != 0);
        }
        finally
        {
            Directory.Delete(tempDirectory, recursive: true);
        }
    }

    [Fact]
    public async Task GetBackups_returns_backups_sorted_newest_first()
    {
        var tempDirectory = CreateTempDirectory();
        try
        {
            var sourcePath = Path.Combine(tempDirectory, "gamelist.xml");
            await File.WriteAllTextAsync(sourcePath, "<gameList />");

            var oldPath = await BackupService.CreateBackupAsync(sourcePath, new DateTimeOffset(2026, 7, 22, 15, 30, 10, TimeSpan.Zero));
            var newPath = await BackupService.CreateBackupAsync(sourcePath, new DateTimeOffset(2026, 7, 23, 9, 5, 0, TimeSpan.Zero));

            var backups = BackupService.GetBackups(sourcePath);

            Assert.Equal(2, backups.Count);
            Assert.Equal(newPath, backups[0].FilePath);
            Assert.Equal(oldPath, backups[1].FilePath);
            Assert.Equal("Jul 23, 2026 09:05:00", backups[0].DisplayTime);
            Assert.Equal("2026-07-23 09:05:00", backups[0].Timestamp.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
            Assert.Equal("2026-07-22 15:30:10", backups[1].Timestamp.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
        }
        finally
        {
            Directory.Delete(tempDirectory, recursive: true);
        }
    }

    [Fact]
    public async Task RestoreBackupAsync_overwrites_source_and_creates_safety_backup()
    {
        var tempDirectory = CreateTempDirectory();
        try
        {
            var sourcePath = Path.Combine(tempDirectory, "gamelist.xml");
            await File.WriteAllTextAsync(sourcePath, "<gameList><game><name>Before</name></game></gameList>");

            var backupPath = await BackupService.CreateBackupAsync(sourcePath);
            await File.WriteAllTextAsync(sourcePath, "<gameList><game><name>After</name></game></gameList>");

            await BackupService.RestoreBackupAsync(sourcePath, backupPath);

            var restored = await File.ReadAllTextAsync(sourcePath);
            Assert.Contains("<name>Before</name>", restored, StringComparison.Ordinal);
            Assert.Equal(2, BackupService.GetBackups(sourcePath).Count);
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
