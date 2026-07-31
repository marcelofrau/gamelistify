using Gamelistify.ViewModels;

namespace Gamelistify.Tests;

public sealed class ScrapeProgressViewModelTests
{
    [Theory]
    [InlineData("[INFO] Progress: 42%", 42)]
    [InlineData("[INFO] 12/30 games scraped", 40)]
    [InlineData("[INFO] 0% done", 0)]
    [InlineData("[INFO] 100% done", 100)]
    [InlineData("[INFO] Processing 7 of 10", 70)]
    public void ParseLine_updates_progress_percent(string line, int expected)
    {
        var vm = new ScrapeProgressViewModel();

        vm.ParseLine(line);

        Assert.Equal(expected, vm.ProgressPercent);
    }

    [Theory]
    [InlineData("1000% overflow", 100)]
    [InlineData("[INFO] folder/a/b/c no numbers", 0)]
    [InlineData("media/images/cover.png", 0)]
    public void ParseLine_clamps_and_ignores_bad_input(string line, int expected)
    {
        var vm = new ScrapeProgressViewModel();

        vm.ParseLine(line);

        Assert.Equal(expected, vm.ProgressPercent);
    }

    [Theory]
    [InlineData("Now scraping \"Sonic the Hedgehog (USA).md\"", "Sonic the Hedgehog (USA).md")]
    [InlineData("Scraping: Super Mario Bros", "Super Mario Bros")]
    [InlineData("[INFO] Scraping game Zelda II", "Zelda II")]
    [InlineData("no game here", null)]
    public void ParseLine_detects_current_game(string line, string? expected)
    {
        var vm = new ScrapeProgressViewModel();

        vm.ParseLine(line);

        Assert.Equal(expected, vm.CurrentGame);
        if (expected is not null)
            Assert.Equal($"Scraping: {expected}", vm.Status);
    }

    [Fact]
    public void ParseLine_prefers_quoted_game_over_ratio_pattern()
    {
        var vm = new ScrapeProgressViewModel();

        vm.ParseLine("Progress: 3/10 \"Donkey Kong\" scraped");

        Assert.Equal("Donkey Kong", vm.CurrentGame);
        Assert.Equal(30, vm.ProgressPercent);
    }
}
