using CommunityToolkit.Mvvm.ComponentModel;

namespace Gamelistify.ViewModels;

public partial class BatchFavoriteViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _namesText = string.Empty;

    public IReadOnlyList<string> Names =>
        NamesText
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
}
