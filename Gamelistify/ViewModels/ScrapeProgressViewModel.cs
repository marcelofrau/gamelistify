using System.Collections.ObjectModel;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gamelistify.Services;

namespace Gamelistify.ViewModels;

public partial class ScrapeProgressViewModel : ViewModelBase
{
    private CancellationTokenSource? _cts;

    public ObservableCollection<string> LogLines { get; } = [];

    [ObservableProperty]
    private string _title = "Scrape Progress";

    [ObservableProperty]
    private string _status = "Preparing scrape...";

    [ObservableProperty]
    private bool _canClose;

    [ObservableProperty]
    private bool _canCancel = true;

    public void AttachCancellation(CancellationTokenSource cts)
    {
        _cts = cts;
    }

    public void AppendLog(string line)
    {
        Logger.Debug("ScrapeProgress: {Line}", line);
        Dispatcher.UIThread.Post(() => LogLines.Add(line));
    }

    [RelayCommand(CanExecute = nameof(CanCancel))]
    private void Cancel()
    {
        Logger.Debug("ScrapeProgress cancel requested");
        _cts?.Cancel();
        Status = "Cancellation requested...";
        CanCancel = false;
    }
}
