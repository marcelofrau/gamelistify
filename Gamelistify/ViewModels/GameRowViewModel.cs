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
    private static readonly IBrush NormalForegroundBrush = new SolidColorBrush(Color.Parse("#F5EEE7"));
    private static readonly IBrush FavoriteStarBrush = new SolidColorBrush(Color.Parse("#E4B44A"));

    private static readonly Geometry FolderGeometry = Geometry.Parse("M2,3.5 L7,3.5 L8.5,5.5 L14,5.5 L14,12.5 L2,12.5 Z");
    private static readonly Geometry GameGeometry = Geometry.Parse("M2,5.5 A1.5,1.5 0 0,1 3.5,4 L12.5,4 A1.5,1.5 0 0,1 14,5.5 L14,10 A1.5,1.5 0 0,1 12.5,11.5 L3.5,11.5 A1.5,1.5 0 0,1 2,10 Z M5,6.5 L5,9.5 M3.5,8 L6.5,8 M10.5,7 A0.75,0.75 0 1,0 10.5,7.01 M12.8,8.8 A0.75,0.75 0 1,0 12.8,8.81");
    private static readonly Geometry EyeGeometry = Geometry.Parse("M8,3.5 C4.5,3.5 2.2,7.6 2,8 C2.2,8.4 4.5,12.5 8,12.5 C11.5,12.5 13.8,8.4 14,8 C13.8,7.6 11.5,3.5 8,3.5 Z M8,5.8 A2.2,2.2 0 1,0 8,10.2 A2.2,2.2 0 1,0 8,5.8 Z");
    private static readonly Geometry EyeOffGeometry = Geometry.Parse("M8,3.5 C4.5,3.5 2.2,7.6 2,8 C2.2,8.4 4.5,12.5 8,12.5 C11.5,12.5 13.8,8.4 14,8 C13.8,7.6 11.5,3.5 8,3.5 Z M2,2 L14,14");

    public GameRowViewModel(GamelistEntry entry)
    {
        Logger.Debug("GameRowViewModel created for {EntryPath}", entry.Path);
        Entry = entry;
    }

    public GamelistEntry Entry { get; }

    public string ItemType => Entry.Kind == GamelistEntryKind.Folder ? "folder" : "game";

    public Geometry TypeIcon => Entry.Kind == GamelistEntryKind.Folder ? FolderGeometry : GameGeometry;

    public Geometry HiddenIcon => Hidden ? EyeOffGeometry : EyeGeometry;

    public IBrush FavoriteIconFill => Favorite ? FavoriteStarBrush : Brushes.Transparent;

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

    public IBrush RowForeground => Hidden ? HiddenForegroundBrush : NormalForegroundBrush;

    public void Refresh()
    {
        OnPropertyChanged(nameof(ItemType));
        OnPropertyChanged(nameof(TypeIcon));
        OnPropertyChanged(nameof(HiddenIcon));
        OnPropertyChanged(nameof(FavoriteIconFill));
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
