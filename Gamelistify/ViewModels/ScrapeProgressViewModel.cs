using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gamelistify.Services;

namespace Gamelistify.ViewModels;

public partial class ScrapeProgressViewModel : ViewModelBase
{
    private static readonly Regex PercentRegex = new(@"\b(\d+)\s*%", RegexOptions.Compiled);
    private static readonly Regex RatioRegex = new(@"\b(\d+)\s*(?:/|of)\s*(\d+)\b", RegexOptions.Compiled);
    private static readonly Regex QuotedGameRegex = new(@"""[^""]+""", RegexOptions.Compiled);
    private static readonly Regex ScrapingGameRegex = new(@"(?i)\bscraping\s*:?\s*(?:game\s+)?(.*)", RegexOptions.Compiled);

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

    [ObservableProperty]
    private int _progressPercent;

    [ObservableProperty]
    private string? _currentGame;

    public void AttachCancellation(CancellationTokenSource cts)
    {
        _cts = cts;
    }

    public void AppendLog(string line)
    {
        Logger.Debug("ScrapeProgress: {Line}", line);
        ParseLine(line);
        Dispatcher.UIThread.Post(() => LogLines.Add(line));
    }

    internal void ParseLine(string line)
    {
        var quoted = QuotedGameRegex.Match(line);
        if (quoted.Success)
        {
            CurrentGame = quoted.Value.Trim('"');
            Status = $"Scraping: {CurrentGame}";
        }
        else
        {
            var scraping = ScrapingGameRegex.Match(line);
            if (scraping.Success)
            {
                var game = scraping.Groups[1].Value.Trim();
                if (!string.IsNullOrWhiteSpace(game))
                {
                    CurrentGame = game;
                    Status = $"Scraping: {game}";
                }
            }
        }

        var percent = PercentRegex.Match(line);
        if (percent.Success && int.TryParse(percent.Groups[1].Value, out var pct))
        {
            ProgressPercent = Math.Clamp(pct, 0, 100);
            return;
        }

        var ratio = RatioRegex.Match(line);
        if (ratio.Success
            && int.TryParse(ratio.Groups[1].Value, out var done)
            && int.TryParse(ratio.Groups[2].Value, out var total)
            && total > 0
            && done <= total)
        {
            ProgressPercent = Math.Clamp((int)Math.Round(done * 100.0 / total), 0, 100);
        }
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
