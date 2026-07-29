using CommunityToolkit.Mvvm.ComponentModel;
using Gamelistify.Models;

namespace Gamelistify.ViewModels;

public partial class ScannedRomItemViewModel : ViewModelBase
{
    public ScannedRomItemViewModel(ScannedRom rom)
    {
        Rom = rom;
    }

    public ScannedRom Rom { get; }

    public string Name => Rom.DisplayName;

    public string RelativePath => Rom.RelativePath;

    [ObservableProperty]
    private bool _isSelected = true;
}
