using Gamelistify.Models;

namespace Gamelistify.ViewModels;

public sealed class GameRowViewModel
{
    public GameRowViewModel(GamelistEntry entry)
    {
        Entry = entry;
    }

    public GamelistEntry Entry { get; }

    public string ItemType => Entry.Kind == GamelistEntryKind.Folder ? "folder" : "game";

    public string Name => Entry.Name;

    public string Path => Entry.Path;

    public bool Hidden => Entry.GetBooleanField("hidden");

    public bool Favorite => Entry.GetBooleanField("favorite");

    public string Genre => Entry.GetField("genre") ?? string.Empty;

    public string Developer => Entry.GetField("developer") ?? string.Empty;

    public string Description => Entry.GetField("desc") ?? string.Empty;

    public string? ImagePath => Entry.GetField("image") ?? Entry.GetField("thumbnail") ?? Entry.GetField("screenshot");
}
