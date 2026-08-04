using CommunityToolkit.Mvvm.Input;
using Gamelistify.Models;
using Gamelistify.Services;

namespace Gamelistify.ViewModels;

public partial class HygienePlanViewModel : ViewModelBase
{
    public HygienePlanViewModel(HygienePlan plan)
    {
        Plan = plan;
        KeepVisible = [.. plan.KeepVisible.Select(static e => e.Name)];
        ToHide = [.. plan.ToHide.Select(static e => e.Name)];
    }

    public HygienePlan Plan { get; }

    public string Title => Plan.Title;

    public string Summary =>
        $"This will hide {Plan.ToHide.Count} entr{(Plan.ToHide.Count == 1 ? "y" : "ies")} and keep {Plan.KeepVisible.Count} entr{(Plan.KeepVisible.Count == 1 ? "y" : "ies")} visible.";

    public IReadOnlyList<string> KeepVisible { get; }

    public IReadOnlyList<string> ToHide { get; }

    public bool Accepted { get; private set; }

    [RelayCommand]
    private void Accept()
    {
        Logger.Information("Hygiene plan accepted: {Title}", Title);
        Accepted = true;
    }
}
