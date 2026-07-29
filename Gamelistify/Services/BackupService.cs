using System.Globalization;

namespace Gamelistify.Services;

public sealed class BackupService
{
    public static async Task<string> CreateBackupAsync(string sourcePath, DateTimeOffset? timestamp = null, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(sourcePath))
            throw new FileNotFoundException("Source file not found for backup.", sourcePath);

        Logger.Debug("Creating backup for {SourcePath}", sourcePath);

        var stamp = (timestamp ?? DateTimeOffset.Now).ToString("yyyy-MM-ddTHH-mm-ss.fff", CultureInfo.InvariantCulture);
        var directory = Path.GetDirectoryName(sourcePath) ?? throw new InvalidOperationException("Source directory is missing.");
        var fileName = Path.GetFileName(sourcePath);
        var backupName = fileName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)
            ? $"{fileName[..^4]}.{stamp}.xml.bak"
            : $"{fileName}.{stamp}.bak";
        var backupPath = Path.Combine(directory, backupName);

        await using var sourceStream = File.Open(sourcePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        await using var backupStream = File.Create(backupPath);
        await sourceStream.CopyToAsync(backupStream, cancellationToken);
        Logger.Information("Backup created at {BackupPath}", backupPath);
        return backupPath;
    }
}
