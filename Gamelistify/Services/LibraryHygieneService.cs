using System.Text.RegularExpressions;
using Gamelistify.Models;

namespace Gamelistify.Services;

public static class LibraryHygieneService
{
    private static readonly string[] BadVersionTokens =
    [
        "!",
        "beta",
        "demo",
        "prototype",
        "preview",
        "sample",
        "unlicensed",
        "hack",
        "homebrew",
        "kiosk",
        "unknown",
        "bios",
    ];

    private static readonly Dictionary<string, int> RegionPriorities = new(StringComparer.OrdinalIgnoreCase)
    {
        ["usa"] = 1,
        ["ntsc-u"] = 1,
        ["us"] = 1,
        ["japan"] = 2,
        ["ntsc-j"] = 2,
        ["jp"] = 2,
        ["brazil"] = 3,
        ["br"] = 3,
        ["europe"] = 4,
        ["pal"] = 4,
        ["eur"] = 4,
    };

    public static string GetBaseName(string name)
    {
        return Regex.Replace(name.Trim(), @"\s*\((?:[^()]*)\)\s*", "").Trim();
    }

    public static int GetRegionPriority(string name)
    {
        var match = Regex.Match(name, @"\(([^()]*)\)");
        if (!match.Success)
            return int.MaxValue;

        var tags = match.Groups[1].Value
            .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        foreach (var tag in tags)
        {
            if (RegionPriorities.TryGetValue(tag, out var priority))
                return priority;
        }

        return int.MaxValue;
    }

    public static bool IsBadVersion(string name)
    {
        var match = Regex.Match(name, @"\(([^()]*)\)");
        if (!match.Success)
            return false;

        return match.Groups[1].Value
            .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Any(tag => BadVersionTokens.Contains(tag, StringComparer.OrdinalIgnoreCase));
    }

    public static int GetRevision(string name)
    {
        var match = Regex.Match(name, @"\(rev\s+(\d+)\)", RegexOptions.IgnoreCase);
        return match.Success ? int.Parse(match.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture) : 0;
    }

    public static HygienePlan BuildDuplicatesPlan(IEnumerable<GamelistEntry> entries)
    {
        var games = entries.Where(e => e.Kind == GamelistEntryKind.Game).ToList();
        var groups = games
            .Where(g => !string.IsNullOrWhiteSpace(GetBaseName(g.Name)))
            .GroupBy(g => GetBaseName(g.Name), StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1);

        var keepVisible = new List<GamelistEntry>();
        var toHide = new List<GamelistEntry>();
        var transfers = new List<(GamelistEntry Keeper, GamelistEntry Hidden)>();

        foreach (var group in groups)
        {
            var ordered = group
                .OrderBy(g => GetRegionPriority(g.Name))
                .ThenBy(g => g.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();

            var keeper = ordered[0];
            var hides = ordered.Skip(1).ToList();

            keepVisible.Add(keeper);
            toHide.AddRange(hides);
            foreach (var hidden in hides)
            {
                if (hidden.GetBooleanField("favorite"))
                    transfers.Add((keeper, hidden));
            }
        }

        return new HygienePlan("Detect & Hide Duplicates", keepVisible, toHide, transfers);
    }

    public static HygienePlan BuildBadVersionsPlan(IEnumerable<GamelistEntry> entries)
    {
        var games = entries.Where(e => e.Kind == GamelistEntryKind.Game).ToList();
        var groups = games
            .Where(g => !string.IsNullOrWhiteSpace(GetBaseName(g.Name)))
            .GroupBy(g => GetBaseName(g.Name), StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1);

        var keepVisible = new List<GamelistEntry>();
        var toHide = new List<GamelistEntry>();
        var transfers = new List<(GamelistEntry Keeper, GamelistEntry Hidden)>();

        foreach (var group in groups)
        {
            var good = group.Where(g => !IsBadVersion(g.Name)).ToList();
            var bad = group.Where(g => IsBadVersion(g.Name)).ToList();

            GamelistEntry keeper;
            List<GamelistEntry> hides;

            if (good.Count > 1)
            {
                var orderedGood = good
                    .OrderByDescending(g => GetRevision(g.Name))
                    .ThenBy(g => g.Name, StringComparer.OrdinalIgnoreCase)
                    .ToList();
                keeper = orderedGood[0];
                hides = orderedGood.Skip(1).Concat(bad).ToList();
            }
            else if (good.Count == 1)
            {
                keeper = good[0];
                hides = bad;
            }
            else
            {
                var orderedBad = bad
                    .OrderByDescending(g => GetRevision(g.Name))
                    .ThenBy(g => g.Name, StringComparer.OrdinalIgnoreCase)
                    .ToList();
                keeper = orderedBad[0];
                hides = orderedBad.Skip(1).ToList();
            }

            if (hides.Count == 0)
                continue;

            keepVisible.Add(keeper);
            toHide.AddRange(hides);
            foreach (var hidden in hides)
            {
                if (hidden.GetBooleanField("favorite"))
                    transfers.Add((keeper, hidden));
            }
        }

        return new HygienePlan("Detect & Hide Bad Versions", keepVisible, toHide, transfers);
    }

    public static IReadOnlyList<GamelistEntry> FindEntriesToReveal(IEnumerable<GamelistEntry> entries)
    {
        var games = entries.Where(e => e.Kind == GamelistEntryKind.Game).ToList();
        var groups = games
            .Where(g => !string.IsNullOrWhiteSpace(GetBaseName(g.Name)))
            .GroupBy(g => GetBaseName(g.Name), StringComparer.OrdinalIgnoreCase);

        var toReveal = new HashSet<GamelistEntry>();

        foreach (var group in groups)
        {
            var groupList = group.ToList();

            var allHidden = groupList.All(g => g.GetBooleanField("hidden"));
            if (allHidden)
            {
                toReveal.Add(groupList
                    .OrderBy(g => GetRegionPriority(g.Name))
                    .ThenBy(g => g.Name, StringComparer.OrdinalIgnoreCase)
                    .First());
                continue;
            }

            var hiddenFavorites = groupList
                .Where(g => g.GetBooleanField("hidden") && g.GetBooleanField("favorite"))
                .OrderBy(g => GetRegionPriority(g.Name))
                .ThenBy(g => g.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();
            if (hiddenFavorites.Count > 0)
                toReveal.Add(hiddenFavorites[0]);
        }

        return toReveal.ToList();
    }
}
