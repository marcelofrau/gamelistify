using Gamelistify.Models;

namespace Gamelistify.Services;

public sealed class RomScannerService
{
    public static IReadOnlyList<ScannedRom> Scan(string directory, IReadOnlyCollection<string>? extensions = null)
    {
        Logger.Information("Scanning ROM directory {Directory}", directory);
        var normalizedExtensions = (extensions ?? MetadataDefinitions.DefaultRomExtensions)
            .Select(static extension => extension.ToLowerInvariant())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var results = new List<ScannedRom>();
        ScanDirectory(directory, directory, normalizedExtensions, results);
        Logger.Information("ROM scan found {Count} ROM candidates", results.Count);
        return results.OrderBy(static entry => entry.RelativePath, StringComparer.OrdinalIgnoreCase).ToList();
    }

    public static IReadOnlyList<ScannedRom> FindMissingEntries(IEnumerable<ScannedRom> scannedRoms, GamelistDocument gamelist)
    {
        var existingPaths = gamelist.Entries
            .Select(static entry => entry.Path)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var missing = scannedRoms.Where(rom => !existingPaths.Contains(rom.RelativePath)).ToList();
        Logger.Information("ROM diff completed. Existing entries: {ExistingCount}, Missing entries: {MissingCount}", existingPaths.Count, missing.Count);
        return missing;
    }

    private static void ScanDirectory(string rootDirectory, string currentDirectory, HashSet<string> normalizedExtensions, List<ScannedRom> results)
    {
        foreach (var directory in Directory.GetDirectories(currentDirectory))
        {
            var folderName = Path.GetFileName(directory);
            if (folderName.StartsWith('.'))
                continue;

            if (MetadataDefinitions.KnownMediaSubfolders.Any(subfolder => folderName.Equals(Path.GetFileName(subfolder), StringComparison.OrdinalIgnoreCase)))
                continue;

            ScanDirectory(rootDirectory, directory, normalizedExtensions, results);
        }

        foreach (var file in Directory.GetFiles(currentDirectory))
        {
            var extension = Path.GetExtension(file).ToLowerInvariant();
            if (!normalizedExtensions.Contains(extension))
                continue;

            var relativePath = Path.GetRelativePath(rootDirectory, file).Replace('\\', '/');
            results.Add(new ScannedRom($"./{relativePath}", Path.GetFileNameWithoutExtension(file), Path.GetFullPath(file)));
        }
    }
}
