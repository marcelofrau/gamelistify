namespace Gamelistify.ViewModels;

public enum ExitDecision
{
    SaveAndExit,
    Discard,
    Cancel,
}

public sealed class ExitConfirmViewModel
{
    public string Title { get; init; } = "Exit Gamelistify";

    public string Message { get; init; } = string.Empty;
}
