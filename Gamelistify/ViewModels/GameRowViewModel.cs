using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Gamelistify.Models;
using Gamelistify.Services;

namespace Gamelistify.ViewModels;

public sealed partial class GameRowViewModel : ObservableObject
{
    private static readonly IBrush NormalRowBrush = new SolidColorBrush(Color.Parse("#151413"));
    private static readonly IBrush FavoriteRowBrush = new SolidColorBrush(Color.Parse("#2E2A1A"));
    private static readonly IBrush HiddenRowBrush = new SolidColorBrush(Color.Parse("#101010"));
    private static readonly IBrush HiddenFavoriteRowBrush = new SolidColorBrush(Color.Parse("#241F10"));
    private static readonly IBrush HiddenForegroundBrush = new SolidColorBrush(Color.Parse("#8A8A8A"));

    public GameRowViewModel(GamelistEntry entry)
    {
        Logger.Debug("GameRowViewModel created for {EntryPath}", entry.Path);
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

    public IBrush RowBackground => (Favorite, Hidden) switch
    {
        (true, true) => HiddenFavoriteRowBrush,
        (true, false) => FavoriteRowBrush,
        (false, true) => HiddenRowBrush,
        _ => NormalRowBrush,
    };

    public IBrush? RowForeground => Hidden ? HiddenForegroundBrush : null;

    public void Refresh()
    {
        OnPropertyChanged(nameof(ItemType));
        OnPropertyChanged(nameof(Name));
        OnPropertyChanged(nameof(Path));
        OnPropertyChanged(nameof(Hidden));
        OnPropertyChanged(nameof(Favorite));
        OnPropertyChanged(nameof(Genre));
        OnPropertyChanged(nameof(Developer));
        OnPropertyChanged(nameof(Description));
        OnPropertyChanged(nameof(ImagePath));
        OnPropertyChanged(nameof(RowBackground));
        OnPropertyChanged(nameof(RowForeground));
    }
}
