using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;

namespace Gamelistify.ViewModels;

public partial class ScanRomsViewModel : ViewModelBase
{
    public ScanRomsViewModel(IEnumerable<ScannedRomItemViewModel> items, string directory)
    {
        foreach (var item in items)
            Items.Add(item);

        Directory = directory;
    }

    public ObservableCollection<ScannedRomItemViewModel> Items { get; } = [];

    public string Directory { get; }

    public string Summary => $"{Items.Count} missing ROMs found";

    public bool Accepted { get; private set; }

    public IReadOnlyList<ScannedRomItemViewModel> SelectedItems => Items.Where(static item => item.IsSelected).ToList();

    [RelayCommand]
    private void SelectAll()
    {
        foreach (var item in Items)
            item.IsSelected = true;
    }

    [RelayCommand]
    private void ClearSelection()
    {
        foreach (var item in Items)
            item.IsSelected = false;
    }

    [RelayCommand]
    private void Accept()
    {
        Accepted = true;
    }
}
