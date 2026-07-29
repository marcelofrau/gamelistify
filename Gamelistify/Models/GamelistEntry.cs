using System.Xml.Linq;

namespace Gamelistify.Models;

public sealed class GamelistEntry
{
    private readonly Dictionary<string, string> _fields = new(StringComparer.OrdinalIgnoreCase);

    public GamelistEntry(GamelistEntryKind kind)
    {
        Kind = kind;
    }

    public GamelistEntryKind Kind { get; }

    public IReadOnlyDictionary<string, string> Fields => _fields;

    public List<XElement> UnknownElements { get; } = [];

    public string Name => GetField("name") ?? GetField("path") ?? string.Empty;

    public string Path => GetField("path") ?? string.Empty;

    public bool HasField(string fieldName)
    {
        return _fields.ContainsKey(fieldName);
    }

    public string? GetField(string fieldName)
    {
        return _fields.TryGetValue(fieldName, out var value) ? value : null;
    }

    public void SetField(string fieldName, string? value)
    {
        if (value is null)
        {
            _fields.Remove(fieldName);
            return;
        }

        _fields[fieldName] = value;
    }

    public bool GetBooleanField(string fieldName)
    {
        var value = GetField(fieldName);
        return value is not null && (value.Equals("true", StringComparison.OrdinalIgnoreCase)
            || value.Equals("1", StringComparison.OrdinalIgnoreCase)
            || value.Equals("yes", StringComparison.OrdinalIgnoreCase));
    }

    public void SetBooleanField(string fieldName, bool value)
    {
        SetField(fieldName, value ? "true" : "false");
    }
}
