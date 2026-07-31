using System.Windows.Input;

namespace Gamelistify.ViewModels;

public sealed class OpenFlyoutItem
{
    public OpenFlyoutItem(string header, ICommand? command = null, object? commandParameter = null, string? toolTip = null)
    {
        Header = header;
        Command = command;
        CommandParameter = commandParameter;
        ToolTip = toolTip;
    }

    public string Header { get; }

    public ICommand? Command { get; }

    public object? CommandParameter { get; }

    public string? ToolTip { get; }
}
