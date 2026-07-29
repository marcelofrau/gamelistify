using Gamelistify.Models;
using Gamelistify.Services;

namespace Gamelistify.Tests;

public sealed class MediaResolverServiceTests
{
    [Fact]
    public void ResolveMediaPath_finds_relative_asset()
    {
        var tempDirectory = CreateTempDirectory();
        try
        {
            var imagesDirectory = Path.Combine(tempDirectory, "images");
            Directory.CreateDirectory(imagesDirectory);
            var imagePath = Path.Combine(imagesDirectory, "Alien Soldier.png");
            File.WriteAllText(imagePath, "image");

            var resolved = MediaResolverService.ResolveMediaPath(tempDirectory, "./images/Alien Soldier.png");

            Assert.Equal(Path.GetFullPath(imagePath), resolved);
        }
        finally
        {
            Directory.Delete(tempDirectory, recursive: true);
        }
    }

    [Fact]
    public void FindOrphanImages_returns_unmapped_matching_images()
    {
        var tempDirectory = CreateTempDirectory();
        try
        {
            var imagesDirectory = Path.Combine(tempDirectory, "images");
            Directory.CreateDirectory(imagesDirectory);
            var orphanPath = Path.Combine(imagesDirectory, "Alien Soldier.png");
            File.WriteAllText(orphanPath, "image");

            var entry = new GamelistEntry(GamelistEntryKind.Game);
            entry.SetField("path", "./Alien Soldier.zip");

            var matches = MediaResolverService.FindOrphanImages(tempDirectory, entry);

            Assert.Single(matches);
            Assert.Equal(Path.GetFullPath(orphanPath), matches[0].AbsolutePath);
        }
        finally
        {
            Directory.Delete(tempDirectory, recursive: true);
        }
    }

    private static string CreateTempDirectory()
    {
        var path = Path.Combine(Path.GetTempPath(), "Gamelistify.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path);
        return path;
    }
}
