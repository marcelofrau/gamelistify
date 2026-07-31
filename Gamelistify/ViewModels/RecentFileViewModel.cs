using System.IO;

namespace Gamelistify.ViewModels;

public sealed class RecentFileViewModel
{
    public RecentFileViewModel(string fullPath)
    {
        FullPath = fullPath;
    }

    public string FullPath { get; }

    public string DisplayName
    {
        get
        {
            var directory = Path.GetFileName(Path.TrimEndingDirectorySeparator(Path.GetDirectoryName(FullPath) ?? string.Empty));
            var file = Path.GetFileName(FullPath);
            return string.IsNullOrEmpty(directory) ? file : $"{directory}/{file}";
        }
    }
}
