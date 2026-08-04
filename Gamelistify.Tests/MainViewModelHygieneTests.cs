using Gamelistify.Helpers;
using Gamelistify.Services;
using Gamelistify.ViewModels;

namespace Gamelistify.Tests;

public sealed class MainViewModelHygieneTests : IDisposable
{
    private const string GameListXml = """
        <?xml version="1.0" encoding="UTF-8"?>
        <gameList>
          <game>
            <path>./Sonic the Hedgehog (USA).zip</path>
            <name>Sonic the Hedgehog (USA)</name>
            <favorite>true</favorite>
          </game>
          <game>
            <path>./Sonic the Hedgehog (Japan).zip</path>
            <name>Sonic the Hedgehog (Japan)</name>
          </game>
          <game>
            <path>./Sonic the Hedgehog (Europe).zip</path>
            <name>Sonic the Hedgehog (Europe)</name>
            <hidden>true</hidden>
            <favorite>true</favorite>
          </game>
        </gameList>
        """;

    private readonly string _tempDirectory;
    private readonly MainViewModel _viewModel;
    private readonly string _gamelistPath;

    public MainViewModelHygieneTests()
    {
        _tempDirectory = Path.Combine(Path.GetTempPath(), "gamelistify-tests-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDirectory);
        AppPaths.AppDataDirectoryOverride = _tempDirectory;
        _gamelistPath = Path.Combine(_tempDirectory, "gamelist.xml");
        _viewModel = new MainViewModel();
    }

    public void Dispose()
    {
        AppPaths.AppDataDirectoryOverride = null;
        try
        {
            Directory.Delete(_tempDirectory, recursive: true);
        }
        catch (IOException)
        {
        }
    }

    private async Task LoadAsync(string xml = GameListXml)
    {
        await File.WriteAllTextAsync(_gamelistPath, xml);
        _viewModel.PickGamelistFileAsync = () => Task.FromResult<string?>(_gamelistPath);
        await _viewModel.OpenCommand.ExecuteAsync(null);
    }

    [Fact]
    public async Task DetectDuplicates_hides_extra_regions_and_keeps_usa()
    {
        await LoadAsync();
        var shownPlan = false;
        _viewModel.ShowHygienePlanAsync = plan =>
        {
            shownPlan = true;
            Assert.Equal("Detect & Hide Duplicates", plan.Title);
            Assert.Equal("Sonic the Hedgehog (USA)", plan.KeepVisible.Single().Name);
            Assert.Equal(2, plan.ToHide.Count);
            return Task.FromResult(true);
        };

        await _viewModel.DetectDuplicatesCommand.ExecuteAsync(null);

        Assert.True(shownPlan);
        var rows = _viewModel.VisibleEntries.ToDictionary(static row => row.Name);
        Assert.False(rows["Sonic the Hedgehog (USA)"].Hidden);
        Assert.True(rows["Sonic the Hedgehog (Japan)"].Hidden);
        Assert.True(rows["Sonic the Hedgehog (Europe)"].Hidden);
        Assert.True(_viewModel.IsDirty);
    }

    [Fact]
    public async Task DetectDuplicates_does_not_apply_when_cancelled()
    {
        await LoadAsync();
        _viewModel.ShowHygienePlanAsync = _ => Task.FromResult(false);

        await _viewModel.DetectDuplicatesCommand.ExecuteAsync(null);

        Assert.False(_viewModel.VisibleEntries.Single(static row => row.Name == "Sonic the Hedgehog (Japan)").Hidden);
        Assert.False(_viewModel.IsDirty);
    }

    [Fact]
    public async Task DetectBadVersions_hides_beta_and_keeps_rev2()
    {
        const string xml = """
            <?xml version="1.0" encoding="UTF-8"?>
            <gameList>
              <game><path>./Sonic.zip</path><name>Sonic (USA) (Rev 1)</name></game>
              <game><path>./Sonic2.zip</path><name>Sonic (USA) (Rev 2)</name></game>
              <game><path>./SonicBeta.zip</path><name>Sonic (USA) (Beta)</name></game>
            </gameList>
            """;
        await LoadAsync(xml);
        var shownPlan = false;
        _viewModel.ShowHygienePlanAsync = plan =>
        {
            shownPlan = true;
            Assert.Equal("Detect & Hide Bad Versions", plan.Title);
            Assert.Equal("Sonic (USA) (Rev 2)", plan.KeepVisible.Single().Name);
            Assert.Equal(2, plan.ToHide.Count);
            return Task.FromResult(true);
        };

        await _viewModel.DetectBadVersionsCommand.ExecuteAsync(null);

        Assert.True(shownPlan);
        var rows = _viewModel.VisibleEntries.ToDictionary(static row => row.Name);
        Assert.True(rows["Sonic (USA) (Rev 1)"].Hidden);
        Assert.True(rows["Sonic (USA) (Beta)"].Hidden);
        Assert.False(rows["Sonic (USA) (Rev 2)"].Hidden);
    }

    [Fact]
    public async Task BatchFavorite_favorites_matching_entries_by_name()
    {
        await LoadAsync();
        _viewModel.ShowBatchFavoriteAsync = () => Task.FromResult<IReadOnlyList<string>?>(
            ["sonic the hedgehog (japan)"]);

        await _viewModel.BatchFavoriteCommand.ExecuteAsync(null);

        var rows = _viewModel.VisibleEntries.ToDictionary(static row => row.Name);
        Assert.True(rows["Sonic the Hedgehog (Japan)"].Favorite);
        Assert.True(rows["Sonic the Hedgehog (USA)"].Favorite);
        Assert.True(_viewModel.IsDirty);
    }

    [Fact]
    public async Task BatchFavorite_no_match_does_not_dirty()
    {
        await LoadAsync();
        _viewModel.ShowBatchFavoriteAsync = () => Task.FromResult<IReadOnlyList<string>?>(["No Such Game"]);

        await _viewModel.BatchFavoriteCommand.ExecuteAsync(null);

        Assert.False(_viewModel.IsDirty);
    }

    [Fact]
    public async Task ReviewHiddenFavorites_unhides_hidden_entries_with_confirm()
    {
        await LoadAsync();
        var confirmed = false;
        _viewModel.ConfirmAsync = (_, _) =>
        {
            confirmed = true;
            return Task.FromResult(true);
        };

        await _viewModel.ReviewHiddenFavoritesCommand.ExecuteAsync(null);

        Assert.True(confirmed);
        Assert.False(_viewModel.VisibleEntries.Single(static row => row.Name == "Sonic the Hedgehog (Europe)").Hidden);
        Assert.True(_viewModel.IsDirty);
    }

    [Fact]
    public async Task ReviewHiddenFavorites_skips_when_nothing_to_reveal()
    {
        const string xml = """
            <?xml version="1.0" encoding="UTF-8"?>
            <gameList>
              <game><path>./Sonic.zip</path><name>Sonic the Hedgehog (USA)</name></game>
            </gameList>
            """;
        await LoadAsync(xml);
        var confirmed = false;
        _viewModel.ConfirmAsync = (_, _) =>
        {
            confirmed = true;
            return Task.FromResult(true);
        };

        await _viewModel.ReviewHiddenFavoritesCommand.ExecuteAsync(null);

        Assert.False(confirmed);
        Assert.False(_viewModel.IsDirty);
    }

    [Fact]
    public async Task SetNameFromFilename_updates_selected_entries()
    {
        await LoadAsync();
        var row = _viewModel.VisibleEntries.Single(static r => r.Name == "Sonic the Hedgehog (Japan)");
        _viewModel.UpdateSelectedEntries([row]);

        _viewModel.SetNameFromFilenameCommand.Execute(null);

        Assert.Equal("Sonic the Hedgehog (Japan)", row.Entry.GetField("name"));
        Assert.True(_viewModel.IsDirty);
    }

    [Fact]
    public async Task SaveAs_retargets_document_to_new_path()
    {
        await LoadAsync();
        var destination = Path.Combine(_tempDirectory, "renamed-gamelist.xml");
        _viewModel.PickSavePathAsync = () => Task.FromResult<string?>(destination);

        await _viewModel.SaveAsCommand.ExecuteAsync(null);

        Assert.Equal(destination, _viewModel.CurrentFilePath);
        Assert.False(_viewModel.IsDirty);
        Assert.True(File.Exists(destination));

        var saved = await GamelistService.LoadAsync(destination);
        Assert.Equal(3, saved.Entries.Count);
    }

    [Fact]
    public async Task ShowHiddenEntries_filters_hidden_rows()
    {
        await LoadAsync();

        Assert.Equal(3, _viewModel.VisibleEntries.Count);

        _viewModel.ShowHiddenEntries = false;

        Assert.Equal(2, _viewModel.VisibleEntries.Count);
        Assert.DoesNotContain(_viewModel.VisibleEntries, static row => row.Hidden);

        _viewModel.ShowHiddenEntries = true;

        Assert.Equal(3, _viewModel.VisibleEntries.Count);
    }
}
