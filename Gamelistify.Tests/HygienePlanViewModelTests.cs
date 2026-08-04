using Gamelistify.Models;
using Gamelistify.ViewModels;

namespace Gamelistify.Tests;

public sealed class HygienePlanViewModelTests
{
    private static GamelistEntry Game(string name)
    {
        var entry = new GamelistEntry(GamelistEntryKind.Game);
        entry.SetField("name", name);
        return entry;
    }

    [Fact]
    public void Exposes_plan_lists_and_summary()
    {
        var plan = new HygienePlan(
            "Detect & Hide Duplicates",
            [Game("Sonic (USA)")],
            [Game("Sonic (Japan)"), Game("Sonic (Europe)")],
            favoriteTransfers: []);

        var viewModel = new HygienePlanViewModel(plan);

        Assert.Equal("Detect & Hide Duplicates", viewModel.Title);
        Assert.Equal("This will hide 2 entries and keep 1 entry visible.", viewModel.Summary);
        Assert.Equal(["Sonic (USA)"], viewModel.KeepVisible);
        Assert.Equal(["Sonic (Japan)", "Sonic (Europe)"], viewModel.ToHide);
    }

    [Fact]
    public void Accept_marks_plan_as_accepted()
    {
        var plan = new HygienePlan("Test", [], [], favoriteTransfers: []);
        var viewModel = new HygienePlanViewModel(plan);

        Assert.False(viewModel.Accepted);
        viewModel.AcceptCommand.Execute(null);

        Assert.True(viewModel.Accepted);
    }
}
