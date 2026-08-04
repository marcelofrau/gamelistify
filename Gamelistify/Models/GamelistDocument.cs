namespace Gamelistify.Models;

public sealed class GamelistDocument
{
    public string? SourcePath { get; set; }

    public string RootElementName { get; init; } = "gameList";

    public List<GamelistEntry> Entries { get; } = [];

    public string BaseDirectory => SourcePath is null
        ? string.Empty
        : Path.GetDirectoryName(SourcePath) ?? string.Empty;

    public bool RemoveEntry(GamelistEntry entry) => Entries.Remove(entry);
}
