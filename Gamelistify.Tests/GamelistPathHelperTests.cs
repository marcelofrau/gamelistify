using Gamelistify.Services;

namespace Gamelistify.Tests;

public sealed class GamelistPathHelperTests
{
    [Theory]
    [InlineData("Alien Soldier.zip", "./Alien Soldier.zip")]
    [InlineData("./roms/game.zip", "./roms/game.zip")]
    [InlineData("roms\\game.zip", "./roms/game.zip")]
    [InlineData("~/roms/game.zip", "~/roms/game.zip")]
    public void NormalizeStoredPath_returns_expected_format(string input, string expected)
    {
        Assert.Equal(expected, GamelistPathHelper.NormalizeStoredPath(input));
    }
}
