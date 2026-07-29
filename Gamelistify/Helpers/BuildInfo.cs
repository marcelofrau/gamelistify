using System.Reflection;

namespace Gamelistify.Helpers;

public static class BuildInfo
{
    public static string InformationalVersion { get; } =
        Assembly.GetEntryAssembly()?
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion
        ?? "0.0.0+build.0";

    public static string DisplayVersion
    {
        get
        {
            var plusIndex = InformationalVersion.IndexOf('+');
            var semver = plusIndex >= 0 ? InformationalVersion[..plusIndex] : InformationalVersion;
            return $"v{semver}";
        }
    }

    public static string BuildLabel
    {
        get
        {
            var plusIndex = InformationalVersion.IndexOf('+');
            return plusIndex >= 0 ? InformationalVersion[(plusIndex + 1)..] : "build.0";
        }
    }
}
