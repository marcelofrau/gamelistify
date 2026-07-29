namespace Gamelistify.Models;

public sealed class ScrapeRequest
{
    public required string Platform { get; init; }

    public required string RomsDirectory { get; init; }

    public string? SelectedRomPath { get; init; }

    public string? MediaDirectory { get; init; }

    public List<string> ExtraArguments { get; init; } = [];

    public bool IsBulk => string.IsNullOrWhiteSpace(SelectedRomPath);
}
