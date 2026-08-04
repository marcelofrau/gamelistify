using Gamelistify.ViewModels;

namespace Gamelistify.Tests;

public sealed class BatchFavoriteViewModelTests
{
    [Fact]
    public void Names_trims_lines_and_drops_empty()
    {
        var viewModel = new BatchFavoriteViewModel
        {
            NamesText = "  Sonic the Hedgehog  \n\n \nMario Bros.\n   ",
        };

        Assert.Equal(["Sonic the Hedgehog", "Mario Bros."], viewModel.Names);
    }

    [Fact]
    public void Names_deduplicates_case_insensitively()
    {
        var viewModel = new BatchFavoriteViewModel
        {
            NamesText = "Sonic\nSONIC\nsonic\nMario",
        };

        Assert.Equal(2, viewModel.Names.Count);
        Assert.Contains("Sonic", viewModel.Names);
        Assert.Contains("Mario", viewModel.Names);
    }

    [Fact]
    public void Names_returns_empty_when_blank()
    {
        var viewModel = new BatchFavoriteViewModel { NamesText = "   \n  \n" };

        Assert.Empty(viewModel.Names);
    }
}
