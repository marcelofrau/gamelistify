namespace Gamelistify.Models;

public sealed class HygienePlan
{
    private readonly IReadOnlyList<(GamelistEntry Keeper, GamelistEntry Hidden)> _favoriteTransfers;

    public HygienePlan(
        string title,
        IReadOnlyList<GamelistEntry> keepVisible,
        IReadOnlyList<GamelistEntry> toHide,
        IReadOnlyList<(GamelistEntry Keeper, GamelistEntry Hidden)> favoriteTransfers)
    {
        Title = title;
        KeepVisible = keepVisible;
        ToHide = toHide;
        _favoriteTransfers = favoriteTransfers;
    }

    public string Title { get; }

    public IReadOnlyList<GamelistEntry> KeepVisible { get; }

    public IReadOnlyList<GamelistEntry> ToHide { get; }

    public int Apply()
    {
        foreach (var entry in ToHide)
            entry.SetBooleanField("hidden", true);

        foreach (var (keeper, hidden) in _favoriteTransfers)
        {
            if (hidden.GetBooleanField("favorite") && !keeper.GetBooleanField("favorite"))
                keeper.SetBooleanField("favorite", true);
        }

        return ToHide.Count;
    }
}
