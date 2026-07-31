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

    [Theory]
    [InlineData("./roms/game.zip")]
    [InlineData("roms/game.zip")]
    [InlineData("game.zip")]
    [InlineData("sub\\nested.zip")]
    public void ResolveToAbsolutePath_resolves_relative_to_base(string input)
    {
        const string baseDirectory = @"C:\base";

        var expected = Path.GetFullPath(Path.Combine(baseDirectory, input.Replace('\\', '/')));
        var result = GamelistPathHelper.ResolveToAbsolutePath(input, baseDirectory);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void ResolveToAbsolutePath_expands_tilde_to_user_profile()
    {
        var result = GamelistPathHelper.ResolveToAbsolutePath("~/roms/game.zip", @"C:\base");

        var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        Assert.Equal(Path.GetFullPath(Path.Combine(userProfile, "roms/game.zip")), result);
    }

    [Fact]
    public void ResolveToAbsolutePath_returns_null_for_blank_path()
    {
        Assert.Null(GamelistPathHelper.ResolveToAbsolutePath(string.Empty, @"C:\base"));
        Assert.Null(GamelistPathHelper.ResolveToAbsolutePath("   ", @"C:\base"));
    }
}
