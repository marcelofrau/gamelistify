namespace Gamelistify.Services;

public static class GamelistPathHelper
{
    public static string NormalizeStoredPath(string rawPath)
    {
        if (string.IsNullOrWhiteSpace(rawPath))
            return string.Empty;

        var normalized = rawPath.Replace('\\', '/').Trim();
        if (Path.IsPathRooted(normalized) || normalized.StartsWith("~/", StringComparison.Ordinal))
            return normalized;

        if (normalized.StartsWith("./", StringComparison.Ordinal) || normalized.StartsWith("../", StringComparison.Ordinal))
            return normalized;

        return $"./{normalized.TrimStart('/')}";
    }

    public static string GetEntryStem(string entryPath)
    {
        if (string.IsNullOrWhiteSpace(entryPath))
            return string.Empty;

        var normalized = entryPath.Replace('\\', '/');
        var lastSegment = normalized.Split('/').Last();
        return Path.GetFileNameWithoutExtension(lastSegment);
    }
}
