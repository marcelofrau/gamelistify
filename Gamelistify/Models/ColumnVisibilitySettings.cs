namespace Gamelistify.Models;

public sealed class ColumnVisibilitySettings
{
    public bool Name { get; set; } = true;

    public bool Genre { get; set; } = true;

    public bool Developer { get; set; } = true;

    public bool ReleaseDate { get; set; } = true;

    public bool Rating { get; set; } = true;

    public bool Players { get; set; } = true;

    public bool Hidden { get; set; } = true;

    public bool Favorite { get; set; } = true;

    public bool KidGame { get; set; }

    public bool PlayCount { get; set; }
}
