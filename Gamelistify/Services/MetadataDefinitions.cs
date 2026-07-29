namespace Gamelistify.Services;

public static class MetadataDefinitions
{
    public static IReadOnlyList<string> KnownFieldOrder { get; } =
    [
        "path", "name", "desc", "rating", "releasedate",
        "developer", "publisher", "genre", "players",
        "image", "video", "marquee", "wheel", "fanart",
        "thumbnail", "screenshot",
        "hidden", "kidgame", "favorite",
        "playcount", "lastplayed",
        "hash", "genreid",
    ];

    public static ISet<string> KnownFields { get; } = new HashSet<string>(KnownFieldOrder, StringComparer.OrdinalIgnoreCase);

    public static ISet<string> BooleanFields { get; } = new HashSet<string>(["hidden", "kidgame", "favorite"], StringComparer.OrdinalIgnoreCase);

    public static ISet<string> MediaFields { get; } = new HashSet<string>(["image", "video", "marquee", "wheel", "fanart", "thumbnail", "screenshot"], StringComparer.OrdinalIgnoreCase);

    public static IReadOnlyList<string> KnownMediaSubfolders { get; } =
    [
        "images", "videos", "marquees", "wheels", "fanart",
        "screenshots", "boxart", "box2dfront", "box2dback",
        "box3d", "support", "steam", "snap", "titles",
        "media/images", "media/videos", "media/marquees",
        "media/wheels", "media/fanart", "media/screenshots",
        "media/boxart", "media/box2dfront",
    ];

    public static IReadOnlyList<string> DefaultRomExtensions { get; } =
    [
        ".zip", ".7z", ".chd", ".cue", ".iso", ".img", ".bin",
        ".rom", ".n64", ".z64", ".v64", ".nes", ".sfc", ".smc",
        ".gba", ".gbc", ".gb", ".nds", ".pce", ".md", ".gen",
        ".32x", ".gg", ".sms", ".col", ".a26", ".a78", ".lnx",
        ".ngp", ".ngc", ".ws", ".wsc", ".psx", ".pbp",
    ];

    public static IReadOnlyList<string> ImageExtensions { get; } = [".png", ".jpg", ".jpeg", ".webp", ".gif", ".bmp"];

    public static IReadOnlyList<string> SkyscraperCandidates { get; } =
    [
        "Skyscraper",
        "skyscraper",
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Skyscraper", "Skyscraper"),
        "/usr/local/bin/Skyscraper",
        "/usr/bin/Skyscraper",
    ];

    public static IReadOnlyList<string> ScraperPlatforms { get; } =
    [
        "amiga", "amstradcpc", "arcade", "atari2600", "atari5200", "atari7800",
        "atarilynx", "atarist", "c64", "colecovision", "dreamcast", "fba",
        "fds", "gameandwatch", "gamegear", "gb", "gba", "gbc", "genesis",
        "intellivision", "mame-libretro", "mastersystem", "megadrive", "msx",
        "n64", "nds", "neogeo", "nes", "ngp", "ngpc", "pcengine", "pcfx",
        "ps2", "psp", "psx", "saturn", "scummvm", "sega32x", "segacd",
        "sg-1000", "snes", "vectrex", "virtualboy", "wii", "wonderswan",
        "wonderswancolor", "x68000", "zxspectrum",
    ];
}
