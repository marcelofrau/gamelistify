using System.Text;
using System.Diagnostics;
using Gamelistify.Models;

namespace Gamelistify.Services;

public sealed class SkyscraperService
{
    public sealed record RunResult(int ExitCode, bool WasCancelled);

    public static string? FindBinary(string? configuredPath = null)
    {
        Logger.Debug("Resolving Skyscraper binary. ConfiguredPath={ConfiguredPath}", configuredPath ?? "<none>");
        if (!string.IsNullOrWhiteSpace(configuredPath) && File.Exists(configuredPath))
        {
            Logger.Information("Using configured Skyscraper binary {BinaryPath}", configuredPath);
            return configuredPath;
        }

        foreach (var candidate in MetadataDefinitions.SkyscraperCandidates)
        {
            var resolved = Environment.ExpandEnvironmentVariables(candidate);
            if (File.Exists(resolved))
            {
                Logger.Information("Resolved Skyscraper binary from candidate {BinaryPath}", resolved);
                return resolved;
            }

            var fromPath = TryResolveFromPath(resolved);
            if (fromPath is not null)
            {
                Logger.Information("Resolved Skyscraper binary from PATH {BinaryPath}", fromPath);
                return fromPath;
            }
        }

        Logger.Warning("Skyscraper binary could not be resolved");
        return null;
    }

    public static IReadOnlyList<string> BuildCommand(string binaryPath, ScrapeRequest request)
    {
        if (string.IsNullOrWhiteSpace(binaryPath))
            throw new ArgumentException("Skyscraper binary path is required.", nameof(binaryPath));

        var command = new List<string>
        {
            binaryPath,
            "-p", request.Platform,
            "-s", "screenscraper",
            "-i", request.RomsDirectory,
        };

        if (!string.IsNullOrWhiteSpace(request.MediaDirectory))
            command.AddRange(["-a", request.MediaDirectory]);

        if (request.ExtraArguments.Count > 0)
            command.AddRange(request.ExtraArguments);

        if (!request.IsBulk)
            command.Add(request.SelectedRomPath!);

        Logger.Debug("Built Skyscraper command for platform {Platform}: {Command}", request.Platform, string.Join(" ", command));
        return command;
    }

    public static async Task WriteCredentialsAsync(string configPath, string user, string password, CancellationToken cancellationToken = default)
    {
        var directory = Path.GetDirectoryName(configPath);
        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);

        var content = new StringBuilder()
            .AppendLine("[screenscraper]")
            .Append("userCreds=")
            .Append(user)
            .Append(':')
            .Append(password)
            .AppendLine()
            .ToString();

        await File.WriteAllTextAsync(configPath, content, cancellationToken);
        Logger.Information("Wrote Skyscraper credentials to {ConfigPath}", configPath);
    }

    public static async Task<RunResult> RunAsync(
        IReadOnlyList<string> command,
        Action<string>? onOutput,
        CancellationToken cancellationToken)
    {
        Logger.Information("Starting Skyscraper process {Binary}", command[0]);
        var startInfo = new ProcessStartInfo
        {
            FileName = command[0],
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        foreach (var argument in command.Skip(1))
            startInfo.ArgumentList.Add(argument);

        using var process = new Process { StartInfo = startInfo, EnableRaisingEvents = true };
        process.Start();

        await using var _ = cancellationToken.Register(() =>
        {
            try
            {
                if (!process.HasExited)
                    process.Kill(entireProcessTree: true);
            }
            catch
            {
            }
        });

        var outputTask = PumpReaderAsync(process.StandardOutput, onOutput, cancellationToken);
        var errorTask = PumpReaderAsync(process.StandardError, onOutput, cancellationToken);

        try
        {
            await Task.WhenAll(outputTask, errorTask, process.WaitForExitAsync(cancellationToken));
            Logger.Information("Skyscraper process exited with code {ExitCode}", process.ExitCode);
            return new RunResult(process.ExitCode, false);
        }
        catch (OperationCanceledException)
        {
            Logger.Warning("Skyscraper process cancelled");
            return new RunResult(process.HasExited ? process.ExitCode : -1, true);
        }
    }

    private static async Task PumpReaderAsync(StreamReader reader, Action<string>? onOutput, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationToken);
            if (line is null)
                break;

            onOutput?.Invoke(line);
        }
    }

    private static string? TryResolveFromPath(string executableName)
    {
        if (Path.IsPathRooted(executableName) || executableName.Contains(Path.DirectorySeparatorChar) || executableName.Contains(Path.AltDirectorySeparatorChar))
            return null;

        var pathValue = Environment.GetEnvironmentVariable("PATH");
        if (string.IsNullOrWhiteSpace(pathValue))
            return null;

        foreach (var pathSegment in pathValue.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var candidate = Path.Combine(pathSegment, executableName);
            if (File.Exists(candidate))
                return candidate;

            if (OperatingSystem.IsWindows())
            {
                var candidateExe = candidate + ".exe";
                if (File.Exists(candidateExe))
                    return candidateExe;
            }
        }

        return null;
    }
}
