using Gamelistify.ViewModels;

namespace Gamelistify.Tests;

public sealed class RecentFileViewModelTests
{
    [Theory]
    [InlineData(@"C:\roms\psx\gamelist.xml", "psx/gamelist.xml")]
    [InlineData(@"/home/pi/RetroPie/roms/snes/gamelist.xml", "snes/gamelist.xml")]
    [InlineData("gamelist.xml", "gamelist.xml")]
    [InlineData(@"C:\roms\gamelist.xml", "roms/gamelist.xml")]
    public void DisplayName_shows_parent_folder_and_file(string fullPath, string expected)
    {
        var vm = new RecentFileViewModel(fullPath);

        Assert.Equal(expected, vm.DisplayName);
    }

    [Fact]
    public void FullPath_keeps_original_path_for_tooltip()
    {
        const string fullPath = @"C:\roms\psx\gamelist.xml";
        var vm = new RecentFileViewModel(fullPath);

        Assert.Equal(fullPath, vm.FullPath);
    }
}
