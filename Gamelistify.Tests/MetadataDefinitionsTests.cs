using Gamelistify.Services;

namespace Gamelistify.Tests;

public sealed class MetadataDefinitionsTests
{
    [Theory]
    [InlineData("nes", "nes")]
    [InlineData("snes", "snes")]
    [InlineData("SNES", "snes")]
    [InlineData("n64", "n64")]
    [InlineData("gameboy", "gb")]
    [InlineData("gameboy color", "gbc")]
    [InlineData("gameboy advance", "gba")]
    [InlineData("gba", "gba")]
    [InlineData("megadrive", "megadrive")]
    [InlineData("genesis", "genesis")]
    [InlineData("mega cd", "segacd")]
    [InlineData("playstation", "psx")]
    [InlineData("ps1", "psx")]
    [InlineData("ps2", "ps2")]
    [InlineData("psp", "psp")]
    [InlineData("turbografx16", "pcengine")]
    [InlineData("tg16", "pcengine")]
    [InlineData("mame", "mame-libretro")]
    [InlineData("arcade", "arcade")]
    [InlineData("commodore64", "c64")]
    [InlineData("atari2600", "atari2600")]
    [InlineData("a2600", "atari2600")]
    [InlineData("neogeo", "neogeo")]
    [InlineData("neogeo pocket", "ngp")]
    [InlineData("wonderswan color", "wonderswancolor")]
    [InlineData("spectrum", "zxspectrum")]
    [InlineData("super nintendo", "snes")]
    [InlineData("nintendods", "nds")]
    [InlineData("nintendowii", "wii")]
    public void InferPlatform_maps_directory_name(string directoryName, string expected)
    {
        Assert.Equal(expected, MetadataDefinitions.InferPlatform(directoryName));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("roms")]
    [InlineData("not-a-platform")]
    public void InferPlatform_returns_null_for_unknown(string? directoryName)
    {
        Assert.Null(MetadataDefinitions.InferPlatform(directoryName));
    }

    [Fact]
    public void InferPlatform_ignores_file_name_instead_of_directory()
    {
        Assert.Equal("snes", MetadataDefinitions.InferPlatform("Super Nintendo.xml"));
    }
}
