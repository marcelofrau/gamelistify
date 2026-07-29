using Gamelistify.Models;

namespace Gamelistify.Services;

public sealed class MediaResolverService
{
    public static string? ResolveMediaPath(string baseDirectory, string rawPath)
    {
        if (string.IsNullOrWhiteSpace(rawPath))
            return null;

        var trimmed = rawPath.Trim();
        var candidatePaths = new List<string>
        {
            trimmed,
        };

        var cleaned = trimmed.Replace("~/", string.Empty, StringComparison.Ordinal).TrimStart('/', '\\');
        candidatePaths.Add(Path.Combine(baseDirectory, cleaned));
        candidatePaths.Add(Path.Combine(baseDirectory, trimmed.TrimStart('.', '/', '\\')));

        var resolved = candidatePaths
            .Select(static path => Path.GetFullPath(path))
            .FirstOrDefault(File.Exists);
        Logger.Debug("ResolveMediaPath base={BaseDirectory} raw={RawPath} resolved={ResolvedPath}", baseDirectory, rawPath, resolved ?? "<none>");
        return resolved;
    }

    public static IReadOnlyList<MediaAssetMatch> FindOrphanImages(string baseDirectory, GamelistEntry entry)
    {
        var stem = GamelistPathHelper.GetEntryStem(entry.Path);
        if (string.IsNullOrWhiteSpace(stem))
            return [];

        var mappedPaths = MetadataDefinitions.MediaFields
            .Select(entry.GetField)
            .Where(static path => !string.IsNullOrWhiteSpace(path))
            .Select(static path => path!.Replace('\\', '/'))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var results = new List<MediaAssetMatch>();
        foreach (var subfolder in MetadataDefinitions.KnownMediaSubfolders)
        {
            var absoluteFolder = Path.Combine(baseDirectory, subfolder.Replace('/', Path.DirectorySeparatorChar));
            if (!Directory.Exists(absoluteFolder))
                continue;

            foreach (var file in Directory.GetFiles(absoluteFolder))
            {
                var extension = Path.GetExtension(file).ToLowerInvariant();
                if (!MetadataDefinitions.ImageExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
                    continue;

                if (!Path.GetFileNameWithoutExtension(file).Equals(stem, StringComparison.OrdinalIgnoreCase))
                    continue;

                var relativePath = Path.GetRelativePath(baseDirectory, file).Replace('\\', '/');
                if (mappedPaths.Contains(relativePath) || mappedPaths.Contains($"./{relativePath}"))
                    continue;

                results.Add(new MediaAssetMatch($"{subfolder}/{Path.GetFileName(file)}", Path.GetFullPath(file)));
            }
        }

        Logger.Information("Found {Count} orphan image candidates for entry {EntryPath}", results.Count, entry.Path);
        return results;
    }
}
