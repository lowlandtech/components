using System.Diagnostics;
using System.Text.Json;

namespace LowlandTech.Components.Tests.CPKG0001.Packaging;

/// <summary>
/// One CPKG0001 packaging scenario run: the packages produced from this
/// repository, the temporary consumer that restored them from an offline feed
/// and the artifacts that consumer resolved.
/// </summary>
/// <remarks>
/// The mechanical work lives in
/// <c>src/test/CPKG0001.Packaging/consumer-scenario.ps1</c>, so the unit and
/// the browser acceptance project drive the same packaging path. This type only
/// starts that harness and reads back the manifest it writes.
/// </remarks>
public sealed class ConsumerScenario : IDisposable
{
    private readonly string _scenarioRoot;

    private ConsumerScenario(JsonElement manifest, string scenarioRoot)
    {
        _scenarioRoot = scenarioRoot;

        ScenarioRoot = scenarioRoot;
        FeedDirectory = Read(manifest, "feedDirectory");
        PackagesDirectory = Read(manifest, "packagesDirectory");
        ConsumerDirectory = Read(manifest, "consumerDirectory");
        PublishDirectory = Read(manifest, "publishDirectory");
        AssetsFile = Read(manifest, "assetsFile");
        CoreVersion = Read(manifest, "coreVersion");
        PackageVersion = Read(manifest, "packageVersion");
        PackageSource = Read(manifest, "packageSource");
        UsePublishedBatch = manifest.GetProperty("usePublishedBatch").GetBoolean();
        LocalFeed = Read(manifest, "localFeed");
        Log = Read(manifest, "log");
        RestoreExitCode = manifest.GetProperty("restoreExitCode").GetInt32();
        BuildExitCode = manifest.GetProperty("buildExitCode").GetInt32();
        PublishExitCode = manifest.GetProperty("publishExitCode").GetInt32();
    }

    /// <summary>Root of the scenario: feed, consumer and publish output.</summary>
    public string ScenarioRoot { get; }

    /// <summary>Private package feed that carries the packed packages.</summary>
    public string FeedDirectory { get; }

    /// <summary>Isolated global-packages folder the consumer restored into.</summary>
    public string PackagesDirectory { get; }

    /// <summary>The temporary consumer application.</summary>
    public string ConsumerDirectory { get; }

    /// <summary>Publish output of the consumer application.</summary>
    public string PublishDirectory { get; }

    /// <summary>NuGet assets file the consumer restore produced.</summary>
    public string AssetsFile { get; }

    /// <summary>Upstream Core package version the scenario selected.</summary>
    public string CoreVersion { get; }

    /// <summary>Version the three Components packages were packed under.</summary>
    public string PackageVersion { get; }

    /// <summary>
    /// Where the three base packages of the scenario came from: <c>packed</c>
    /// when the harness packed them from this repository, <c>published</c> when
    /// it copied the published identities of the shared feed.
    /// </summary>
    public string PackageSource { get; }

    /// <summary>Whether the scenario consumed the published batch of the shared feed.</summary>
    public bool UsePublishedBatch { get; }

    /// <summary>The shared feed the published identities are read from.</summary>
    public string LocalFeed { get; }

    /// <summary>Transcript of every command the harness ran.</summary>
    public string Log { get; }

    /// <summary>Exit code of the consumer restore.</summary>
    public int RestoreExitCode { get; }

    /// <summary>Exit code of the consumer build.</summary>
    public int BuildExitCode { get; }

    /// <summary>Exit code of the consumer publish.</summary>
    public int PublishExitCode { get; }

    /// <summary>
    /// Runs the packaging scenario and returns the produced artifacts.
    /// </summary>
    /// <param name="repositoryRoot">Repository the packages are packed from.</param>
    /// <param name="publish">Also publish the consumer application.</param>
    /// <param name="usePublishedBatch">
    /// Consume the published identities of the shared feed instead of packing
    /// the packages of this repository.
    /// </param>
    public static ConsumerScenario Run(
        string repositoryRoot,
        bool publish = false,
        bool usePublishedBatch = false)
    {
        var scenarioRoot = Path.Combine(
            Path.GetTempPath(),
            $"cpkg0001-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}");
        Directory.CreateDirectory(scenarioRoot);

        var script = Path.Combine(
            repositoryRoot, "src", "test", "CPKG0001.Packaging", "consumer-scenario.ps1");

        if (!File.Exists(script))
        {
            throw new FileNotFoundException(
                $"The CPKG0001 packaging harness is missing. Repository root: '{repositoryRoot}'. " +
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

        if (publish)
        {
            arguments.Add("-Publish");
        }

        if (usePublishedBatch)
        {
            arguments.Add("-UsePublishedBatch");
        }

        // The scenario packs the shared Components projects in the Release
        // configuration, so it must not run beside another packaging scenario
        // of this assembly.
        var (exitCode, output) = PackagingScenarioLock.Run(
            () => RunProcess(ResolveShell(), arguments, repositoryRoot));

        var manifestPath = Path.Combine(scenarioRoot, "scenario.json");
        if (exitCode != 0 || !File.Exists(manifestPath))
        {
            throw new InvalidOperationException(
                $"The CPKG0001 packaging scenario failed with exit code {exitCode}.{Environment.NewLine}" +
                $"{output}{Environment.NewLine}{ReadLog(scenarioRoot)}");
        }

        using var document = JsonDocument.Parse(File.ReadAllText(manifestPath));
        return new ConsumerScenario(document.RootElement, scenarioRoot);
    }

    /// <summary>
    /// Locates the repository root by walking up from the test binaries until
    /// the packaging harness is found. The harness is the anchor because the
    /// repository holds more than one solution file.
    /// </summary>
    public static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            var harness = Path.Combine(
                directory.FullName, "src", "test", "CPKG0001.Packaging", "consumer-scenario.ps1");

            if (File.Exists(harness))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            $"The CPKG0001 packaging harness was not found above '{AppContext.BaseDirectory}'.");
    }

    /// <summary>Reads the harness transcript, when it exists.</summary>
    public string ReadLog()
    {
        return File.Exists(Log) ? File.ReadAllText(Log) : string.Empty;
    }

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

    private static string Read(JsonElement manifest, string name)
    {
        return manifest.GetProperty(name).GetString()
               ?? throw new InvalidOperationException($"The scenario manifest has no '{name}'.");
    }

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
