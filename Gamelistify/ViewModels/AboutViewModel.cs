using Gamelistify.Helpers;

namespace Gamelistify.ViewModels;

public sealed class AboutViewModel : ViewModelBase
{
    public static string ProjectName => ProjectInfo.ProjectName;

    public static string Description => ProjectInfo.Description;

    public static string Version => BuildInfo.DisplayVersion;

    public static string Build => BuildInfo.BuildLabel;

    public static string License => ProjectInfo.LicenseName;

    public static string Author => ProjectInfo.Author;

    public static string RepositoryUrl => ProjectInfo.RepositoryUrl;

    public static string IssueTrackerUrl => ProjectInfo.IssueTrackerUrl;
}
