using System.Xml;
using System.Xml.Linq;
using Gamelistify.Models;

namespace Gamelistify.Services;

public sealed class GamelistService
{
    public GamelistService(BackupService backupService)
    {
    }

    public static async Task<GamelistDocument> LoadAsync(string xmlPath, CancellationToken cancellationToken = default)
    {
        Logger.Information("Loading gamelist XML from {XmlPath}", xmlPath);
        await using var stream = File.OpenRead(xmlPath);
        var document = await XDocument.LoadAsync(stream, LoadOptions.None, cancellationToken);
        var root = document.Root ?? throw new InvalidDataException("gamelist.xml root element is missing.");

        var gamelist = new GamelistDocument
        {
            SourcePath = xmlPath,
            RootElementName = root.Name.LocalName,
        };

        foreach (var element in root.Elements())
        {
            if (!element.Name.LocalName.Equals("game", StringComparison.OrdinalIgnoreCase)
                && !element.Name.LocalName.Equals("folder", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var kind = element.Name.LocalName.Equals("folder", StringComparison.OrdinalIgnoreCase)
                ? GamelistEntryKind.Folder
                : GamelistEntryKind.Game;

            var entry = new GamelistEntry(kind);
            var seenKnownFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var child in element.Elements())
            {
                var fieldName = child.Name.LocalName;
                if (MetadataDefinitions.KnownFields.Contains(fieldName) && seenKnownFields.Add(fieldName))
                {
                    var value = child.Value?.Trim() ?? string.Empty;
                    if (fieldName.Equals("path", StringComparison.OrdinalIgnoreCase))
                        value = GamelistPathHelper.NormalizeStoredPath(value);
                    entry.SetField(fieldName, value);
                    continue;
                }

                entry.UnknownElements.Add(new XElement(child));
            }

            gamelist.Entries.Add(entry);
        }

        Logger.Information("Loaded {EntryCount} entries from {XmlPath}", gamelist.Entries.Count, xmlPath);
        return gamelist;
    }

    public static List<GamelistEntry> GetInvalidEntries(GamelistDocument document)
    {
        var invalid = new List<GamelistEntry>();
        foreach (var entry in document.Entries)
        {
            if (entry.Kind == GamelistEntryKind.Folder)
                continue;

            var storedPath = entry.GetField("path");
            if (string.IsNullOrWhiteSpace(storedPath))
            {
                invalid.Add(entry);
                continue;
            }

            var absolutePath = GamelistPathHelper.ResolveToAbsolutePath(storedPath, document.BaseDirectory);
            if (absolutePath is null || !File.Exists(absolutePath))
                invalid.Add(entry);
        }

        return invalid;
    }

    public static async Task<string?> SaveAsync(GamelistDocument document, string? destinationPath = null, bool createBackup = true, bool compact = false, CancellationToken cancellationToken = default)
    {
        var outputPath = destinationPath ?? document.SourcePath ?? throw new InvalidOperationException("Destination path is required.");
        Logger.Verbose("SaveAsync: {EntryCount} entries to save", document.Entries.Count);
        Logger.Information("Saving gamelist XML to {OutputPath}. Backup enabled={BackupEnabled}", outputPath, createBackup);

        string? backupPath = null;
        if (createBackup && File.Exists(outputPath))
            backupPath = await BackupService.CreateBackupAsync(outputPath, cancellationToken: cancellationToken);

        var root = new XElement(document.RootElementName);
        foreach (var entry in document.Entries)
        {
            var entryElement = new XElement(entry.Kind == GamelistEntryKind.Folder ? "folder" : "game");

            foreach (var fieldName in MetadataDefinitions.KnownFieldOrder)
            {
                if (!entry.HasField(fieldName))
                    continue;

                var value = entry.GetField(fieldName) ?? string.Empty;
                if (fieldName.Equals("path", StringComparison.OrdinalIgnoreCase))
                    value = GamelistPathHelper.NormalizeStoredPath(value);

                entryElement.Add(new XElement(fieldName, value));
            }

            foreach (var unknownElement in entry.UnknownElements)
                entryElement.Add(new XElement(unknownElement));

            root.Add(entryElement);
        }

        var xDocument = new XDocument(new XDeclaration("1.0", "UTF-8", null), root);
        var settings = new XmlWriterSettings
        {
            Async = true,
            Encoding = new System.Text.UTF8Encoding(false),
            Indent = !compact,
            NewLineChars = Environment.NewLine,
        };

        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? throw new InvalidOperationException("Output directory missing."));
        await using var stream = File.Create(outputPath);
        await using var writer = XmlWriter.Create(stream, settings);
        xDocument.Save(writer);
        await writer.FlushAsync();
        Logger.Information("Saved {EntryCount} entries to {OutputPath}", document.Entries.Count, outputPath);
        return backupPath;
    }
}
