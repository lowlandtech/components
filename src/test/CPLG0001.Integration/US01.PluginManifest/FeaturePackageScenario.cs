using System.Diagnostics;
using System.Text.Json;

namespace LowlandTech.Components.Tests.CPLG0001.Integration.US01.PluginManifest;

/// <summary>
/// One CPLG0001 packaging scenario run: the packages this repository produced
/// into a private feed.
///
/// <remarks>
/// The mechanical work lives in
/// <c>src/test/CPLG0001.Integration/US01.PluginManifest/feature-scenario.ps1</c>.
/// This type only starts that harness and reads back the manifest it writes, so
/// the acceptance always inspects packages that were packed from the current
/// source.
/// </remarks>
public sealed class FeaturePackageScenario : IDisposable
{
    private readonly string _scenarioRoot;

    private FeaturePackageScenario(JsonElement manifest, string scenarioRoot)
    {
        _scenarioRoot = scenarioRoot;

        ScenarioRoot = scenarioRoot;
        FeedDirectory = Read(manifest, "feedDirectory");
        CoreVersion = Read(manifest, "coreVersion");
        PackageVersion = Read(manifest, "packageVersion");
        Log = Read(manifest, "log");

        var packages = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var property in manifest.GetProperty("packages").EnumerateObject())
        {
            packages[property.Name] = property.Value.GetString() ?? string.Empty;
        }

        Packages = packages;
    }

    /// <summary>Root of the scenario: the private feed and its transcript.</summary>
    public string ScenarioRoot { get; }

    /// <summary>Private package feed that carries the packed packages.</summary>
    public string FeedDirectory { get; }

    /// <summary>Upstream Core package version the packages were built against.</summary>
    public string CoreVersion { get; }

    /// <summary>Version the Components packages were packed under.</summary>
    public string PackageVersion { get; }

    /// <summary>The packed packages, by package id.</summary>
    public IReadOnlyDictionary<string, string> Packages { get; }

    /// <summary>Transcript of every command the harness ran.</summary>
    public string Log { get; }

    /// <summary>
    /// Runs the packaging scenario and returns the produced packages.
    /// </summary>
    /// <param name="repositoryRoot">Repository the packages are packed from.</param>
    public static FeaturePackageScenario Run(string repositoryRoot)
    {
        var scenarioRoot = Path.Combine(
            Path.GetTempPath(),
            $"cplg0001-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}");
        Directory.CreateDirectory(scenarioRoot);

        var script = Path.Combine(
            repositoryRoot, "src", "test", "CPLG0001.Integration", "US01.PluginManifest",
            "feature-scenario.ps1");

        if (!File.Exists(script))
        {
            throw new FileNotFoundException(
                $"The CPLG0001 packaging harness is missing. Repository root: '{repositoryRoot}'. " +
                $"Expected harness: '{script}'. Base directory: '{AppContext.BaseDirectory}'.",
                script);
        }

        var arguments = new List<string>
        {
            "-NoProfile",
            "-ExecutionPolicy", "Bypass",
            "-File", script,
            "-RepositoryRoot", repositoryRoot,
            "-ScenarioRoot", scenarioRoot,
        };

        var (exitCode, output) = RunProcess(ResolveShell(), arguments, repositoryRoot);

        var manifestPath = Path.Combine(scenarioRoot, "scenario.json");
        if (exitCode != 0 || !File.Exists(manifestPath))
        {
            throw new InvalidOperationException(
                $"The CPLG0001 packaging scenario failed with exit code {exitCode}.{Environment.NewLine}" +
                $"{output}{Environment.NewLine}{ReadLog(scenarioRoot)}");
        }

        using var document = JsonDocument.Parse(File.ReadAllText(manifestPath));
        return new FeaturePackageScenario(document.RootElement, scenarioRoot);
    }

    /// <summary>
    /// Locates the repository root by walking up from the test binaries until
    /// the packaging harness is found.
    /// </summary>
    public static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            var harness = Path.Combine(
                directory.FullName, "src", "test", "CPLG0001.Integration", "US01.PluginManifest",
                "feature-scenario.ps1");

            if (File.Exists(harness))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            $"The CPLG0001 packaging harness was not found above '{AppContext.BaseDirectory}'.");
    }

    /// <summary>Reads the harness transcript, when it exists.</summary>
    public string ReadLog() =>
        File.Exists(Log) ? File.ReadAllText(Log) : string.Empty;

    /// <summary>Removes the temporary scenario directory.</summary>
    public void Dispose()
    {
        try
        {
            if (Directory.Exists(_scenarioRoot))
            {
                Directory.Delete(_scenarioRoot, recursive: true);
            }
        }
        catch (IOException)
        {
            // A leftover temporary directory must never fail an acceptance run.
        }
        catch (UnauthorizedAccessException)
        {
            // Same as above: cleanup is best effort.
        }
    }

    private static string Read(JsonElement manifest, string name) =>
        manifest.GetProperty(name).GetString()
        ?? throw new InvalidOperationException($"The scenario manifest has no '{name}'.");

    private static string ResolveShell()
    {
        if (OperatingSystem.IsWindows())
        {
            var windowsShell = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.System),
                "WindowsPowerShell",
                "v1.0",
                "powershell.exe");

            if (File.Exists(windowsShell))
            {
                return windowsShell;
            }
        }

        return "pwsh";
    }

    private static (int ExitCode, string Output) RunProcess(
        string filePath,
        IEnumerable<string> arguments,
        string workingDirectory)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = filePath,
            WorkingDirectory = workingDirectory,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };

        foreach (var argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        using var process = Process.Start(startInfo)
                            ?? throw new InvalidOperationException($"Could not start '{filePath}'.");

        var standardOutput = process.StandardOutput.ReadToEndAsync();
        var standardError = process.StandardError.ReadToEndAsync();
        process.WaitForExit();

        return (process.ExitCode, standardOutput.Result + standardError.Result);
    }

    private static string ReadLog(string scenarioRoot)
    {
        var log = Path.Combine(scenarioRoot, "scenario.log");
        return File.Exists(log) ? File.ReadAllText(log) : string.Empty;
    }
}
