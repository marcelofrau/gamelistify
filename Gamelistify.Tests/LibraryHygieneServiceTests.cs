using Gamelistify.Models;
using Gamelistify.Services;

namespace Gamelistify.Tests;

public sealed class LibraryHygieneServiceTests
{
    private static GamelistEntry Game(string name, bool hidden = false, bool favorite = false)
    {
        var entry = new GamelistEntry(GamelistEntryKind.Game);
        entry.SetField("name", name);
        entry.SetField("path", $"./{name}.zip");
        entry.SetBooleanField("hidden", hidden);
        entry.SetBooleanField("favorite", favorite);
        return entry;
    }

    private static GamelistEntry Folder(string name)
    {
        var entry = new GamelistEntry(GamelistEntryKind.Folder);
        entry.SetField("name", name);
        return entry;
    }

    [Theory]
    [InlineData("Sonic (USA)", "Sonic")]
    [InlineData("Sonic (USA) (Rev 1)", "Sonic")]
    [InlineData("Sonic (USA, Europe)", "Sonic")]
    [InlineData("Sonic", "Sonic")]
    [InlineData("  Sonic (USA)  ", "Sonic")]
    public void GetBaseName_strips_paren_tags(string name, string expected)
    {
        Assert.Equal(expected, LibraryHygieneService.GetBaseName(name));
    }

    [Theory]
    [InlineData("Sonic (USA)", 1)]
    [InlineData("Sonic (Japan)", 2)]
    [InlineData("Sonic (Brazil)", 3)]
    [InlineData("Sonic (Europe)", 4)]
    [InlineData("Sonic (Rev 1)", int.MaxValue)]
    [InlineData("Sonic", int.MaxValue)]
    public void GetRegionPriority_ranks_regions(string name, int expected)
    {
        Assert.Equal(expected, LibraryHygieneService.GetRegionPriority(name));
    }

    [Theory]
    [InlineData("Sonic (USA)", false)]
    [InlineData("Sonic (USA) (Rev 1)", false)]
    [InlineData("Sonic (Beta)", true)]
    [InlineData("Sonic (Demo)", true)]
    [InlineData("Sonic (Proto, USA)", false)]
    [InlineData("Sonic (Prototype)", true)]
    [InlineData("Sonic (Sample)", true)]
    [InlineData("Sonic (Homebrew)", true)]
    [InlineData("Sonic (Kiosk)", true)]
    [InlineData("Sonic (Unknown)", true)]
    [InlineData("Sonic (BIOS)", true)]
    public void IsBadVersion_detects_bad_tags(string name, bool expected)
    {
        Assert.Equal(expected, LibraryHygieneService.IsBadVersion(name));
    }

    [Theory]
    [InlineData("Sonic (USA) (Rev 1)", 1)]
    [InlineData("Sonic (USA) (Rev 2)", 2)]
    [InlineData("Sonic (USA)", 0)]
    public void GetRevision_parses_revision(string name, int expected)
    {
        Assert.Equal(expected, LibraryHygieneService.GetRevision(name));
    }

    [Fact]
    public void BuildDuplicatesPlan_keeps_usa_hides_others()
    {
        var entries = new List<GamelistEntry>
        {
            Game("Sonic (USA)"),
            Game("Sonic (Japan)"),
            Game("Sonic (Europe)"),
        };

        var plan = LibraryHygieneService.BuildDuplicatesPlan(entries);

        Assert.Single(plan.KeepVisible);
        Assert.Equal("Sonic (USA)", plan.KeepVisible[0].Name);
        Assert.Equal(2, plan.ToHide.Count);
    }

    [Fact]
    public void BuildDuplicatesPlan_ignores_folders()
    {
        var entries = new List<GamelistEntry>
        {
            Game("Sonic (USA)"),
            Game("Sonic (Japan)"),
            Folder("Sonic"),
        };

        var plan = LibraryHygieneService.BuildDuplicatesPlan(entries);

        Assert.Single(plan.ToHide);
    }

    [Fact]
    public void BuildDuplicatesPlan_transfers_favorite_to_keeper()
    {
        var hiddenFavorite = Game("Sonic (Japan)", favorite: true);
        var entries = new List<GamelistEntry>
        {
            Game("Sonic (USA)"),
            hiddenFavorite,
        };

        var plan = LibraryHygieneService.BuildDuplicatesPlan(entries);
        plan.Apply();

        var keeper = plan.KeepVisible[0];
        Assert.True(keeper.GetBooleanField("favorite"));
        Assert.True(plan.ToHide[0].GetBooleanField("hidden"));
    }

    [Fact]
    public void BuildBadVersionsPlan_keeps_best_revision_of_good_versions()
    {
        var entries = new List<GamelistEntry>
        {
            Game("Sonic (USA) (Rev 1)"),
            Game("Sonic (USA) (Rev 2)"),
            Game("Sonic (USA) (Beta)"),
        };

        var plan = LibraryHygieneService.BuildBadVersionsPlan(entries);

        Assert.Single(plan.KeepVisible);
        Assert.Equal("Sonic (USA) (Rev 2)", plan.KeepVisible[0].Name);
        Assert.Equal(2, plan.ToHide.Count);
    }

    [Fact]
    public void BuildBadVersionsPlan_keeps_best_revision_when_all_bad()
    {
        var entries = new List<GamelistEntry>
        {
            Game("Sonic (Beta)"),
            Game("Sonic (Demo)"),
        };

        var plan = LibraryHygieneService.BuildBadVersionsPlan(entries);

        Assert.Equal("Sonic (Beta)", plan.KeepVisible[0].Name);
        Assert.Single(plan.ToHide);
    }

    [Fact]
    public void Apply_hides_to_hide_and_preserves_others()
    {
        var usa = Game("Sonic (USA)");
        var japan = Game("Sonic (Japan)");
        var entries = new List<GamelistEntry> { usa, japan };

        var plan = LibraryHygieneService.BuildDuplicatesPlan(entries);
        var hidden = plan.Apply();

        Assert.Equal(1, hidden);
        Assert.True(japan.GetBooleanField("hidden"));
        Assert.False(usa.GetBooleanField("hidden"));
    }

    [Fact]
    public void FindEntriesToReveal_unhides_all_hidden_group()
    {
        var entries = new List<GamelistEntry>
        {
            Game("Sonic (USA)", hidden: true),
            Game("Sonic (Japan)", hidden: true),
        };

        var toReveal = LibraryHygieneService.FindEntriesToReveal(entries);

        Assert.Single(toReveal);
        Assert.Equal("Sonic (USA)", toReveal[0].Name);
    }

    [Fact]
    public void FindEntriesToReveal_unhides_hidden_favorite()
    {
        var entries = new List<GamelistEntry>
        {
            Game("Sonic (USA)"),
            Game("Sonic (Japan)", hidden: true, favorite: true),
            Game("Sonic (Europe)", hidden: true),
        };

        var toReveal = LibraryHygieneService.FindEntriesToReveal(entries);

        Assert.Single(toReveal);
        Assert.Equal("Sonic (Japan)", toReveal[0].Name);
    }
}
