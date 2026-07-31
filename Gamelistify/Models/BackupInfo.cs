using System.Globalization;

namespace Gamelistify.Models;

public sealed record BackupInfo(string FilePath, DateTimeOffset Timestamp, long SizeBytes)
{
    public string FileName => Path.GetFileName(FilePath);

    public string DisplayTime => Timestamp.LocalDateTime.ToString("MMM d, yyyy HH:mm:ss", CultureInfo.InvariantCulture);

    public string SizeDisplay => SizeBytes switch
    {
        >= 1024 * 1024 => $"{SizeBytes / (1024.0 * 1024.0):0.0} MB",
        >= 1024 => $"{SizeBytes / 1024.0:0.0} KB",
        _ => $"{SizeBytes} B",
    };
}
