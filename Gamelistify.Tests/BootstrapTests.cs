namespace Gamelistify.Tests;

public sealed class BootstrapTests
{
    [Fact]
    public void Solution_bootstrap_is_available()
    {
        Assert.Equal("Gamelistify", typeof(Gamelistify.App).Namespace);
    }
}
