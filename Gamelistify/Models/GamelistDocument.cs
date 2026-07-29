namespace Gamelistify.Models;

public sealed class GamelistDocument
{
    public string? SourcePath { get; init; }

    public string RootElementName { get; init; } = "gameList";

    public List<GamelistEntry> Entries { get; } = [];

    public string BaseDirectory => SourcePath is null
        ? string.Empty
        : Path.GetDirectoryName(SourcePath) ?? string.Empty;
}
