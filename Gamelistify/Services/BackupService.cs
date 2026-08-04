using System.Globalization;
using Gamelistify.Models;

namespace Gamelistify.Services;

public sealed class BackupService
{
    public const string BackupDirectoryName = "gamelists_backup";

    public static string GetBackupDirectory(string sourcePath)
    {
        var directory = Path.GetDirectoryName(sourcePath) ?? throw new InvalidOperationException("Source directory is missing.");
        return Path.Combine(directory, BackupDirectoryName);
    }

    public static async Task<string> CreateBackupAsync(string sourcePath, DateTimeOffset? timestamp = null, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(sourcePath))
            throw new FileNotFoundException("Source file not found for backup.", sourcePath);

        Logger.Debug("Creating backup for {SourcePath}", sourcePath);

        var backupDirectory = GetBackupDirectory(sourcePath);
        var created = !Directory.Exists(backupDirectory);
        Directory.CreateDirectory(backupDirectory);
        if (created)
            File.SetAttributes(backupDirectory, File.GetAttributes(backupDirectory) | FileAttributes.Hidden);

        var stamp = (timestamp ?? DateTimeOffset.Now).ToString("yyyy-MM-ddTHH-mm-ss.fff", CultureInfo.InvariantCulture);
        var fileName = Path.GetFileName(sourcePath);
        var backupName = fileName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)
            ? $"{fileName[..^4]}.{stamp}.xml.bak"
            : $"{fileName}.{stamp}.bak";
        var backupPath = Path.Combine(backupDirectory, backupName);

        await using var sourceStream = File.Open(sourcePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        await using var backupStream = File.Create(backupPath);
        await sourceStream.CopyToAsync(backupStream, cancellationToken);
        Logger.Information("Backup created at {BackupPath}", backupPath);
        return backupPath;
    }

    public static List<BackupInfo> GetBackups(string sourcePath)
    {
        var backupDirectory = GetBackupDirectory(sourcePath);
        if (!Directory.Exists(backupDirectory))
            return [];

        var fileName = Path.GetFileName(sourcePath);
        var prefix = fileName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)
            ? fileName[..^4]
            : fileName;

        var result = new List<BackupInfo>();
        foreach (var file in Directory.EnumerateFiles(backupDirectory, $"{prefix}.*.bak"))
        {
            var name = Path.GetFileName(file);
            var stampText = name[prefix.Length..];
            if (stampText.EndsWith(".xml.bak", StringComparison.OrdinalIgnoreCase))
                stampText = stampText[..^".xml.bak".Length];
            else if (stampText.EndsWith(".bak", StringComparison.OrdinalIgnoreCase))
                stampText = stampText[..^".bak".Length];
            stampText = stampText.TrimStart('.');

            if (DateTimeOffset.TryParseExact(
                    stampText,
                    "yyyy-MM-dd'T'HH-mm-ss.fff",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeLocal,
                    out var parsed))
            {
                result.Add(new BackupInfo(file, parsed, new FileInfo(file).Length));
            }
        }

        result.Sort(static (a, b) => b.Timestamp.CompareTo(a.Timestamp));
        return result;
    }

    public static async Task RestoreBackupAsync(string sourcePath, string backupPath, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(backupPath))
            throw new FileNotFoundException("Backup file not found.", backupPath);
        if (!File.Exists(sourcePath))
            throw new FileNotFoundException("Target gamelist file not found.", sourcePath);

        Logger.Information("Restoring backup {BackupPath} over {SourcePath}", backupPath, sourcePath);
        await CreateBackupAsync(sourcePath, cancellationToken: cancellationToken);

        File.Copy(backupPath, sourcePath, overwrite: true);
        Logger.Information("Restored backup {BackupPath} to {SourcePath}", backupPath, sourcePath);
    }
}
