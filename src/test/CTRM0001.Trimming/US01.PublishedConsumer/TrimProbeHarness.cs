using System.Diagnostics;
using System.Text.Json;

namespace LowlandTech.Components.Tests.CTRM0001.Trimming.US01.PublishedConsumer;

/// <summary>
/// The mechanical half of the CTRM0001 trim preservation probe: it prepares an
/// isolated scenario root, runs <c>probe-scenario.ps1</c> and reads the result the
/// script writes. The Given and When of the scenario stay in the scenario source;
/// this type only owns the process plumbing.
/// </summary>
internal sealed class TrimProbeHarness
{
    private TrimProbeHarness(string scenarioRoot, string scriptPath)
    {
        ScenarioRoot = scenarioRoot;
        ScriptPath = scriptPath;
    }

    public string ScenarioRoot { get; }

    public string ScriptPath { get; }

    public TrimProbeResult? Result { get; private set; }

    public string LogPath => Path.Combine(ScenarioRoot, "probe.log");

    /// <summary>Creates a fresh scenario root for one probe run.</summary>
    public static TrimProbeHarness Prepare()
    {
        var repositoryRoot = FindRepositoryRoot();
        var script = Path.Combine(
            repositoryRoot,
            "src", "test", "CTRM0001.Trimming", "US01.PublishedConsumer", "probe-scenario.ps1");
        if (!File.Exists(script))
        {
            throw new FileNotFoundException("The trim preservation probe script is missing.", script);
        }

        var root = Path.Combine(Path.GetTempPath(), "ctrm0001-trim-probe", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        return new TrimProbeHarness(root, script);
    }

    /// <summary>Runs the probe and reads the result it wrote.</summary>
    public void Run()
    {
        var start = new ProcessStartInfo(ResolveShell())
        {
            WorkingDirectory = FindRepositoryRoot(),
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };
        start.ArgumentList.Add("-NoProfile");
        start.ArgumentList.Add("-File");
        start.ArgumentList.Add(ScriptPath);
        start.ArgumentList.Add("-ScenarioRoot");
        start.ArgumentList.Add(ScenarioRoot);

        using var process = Process.Start(start)
            ?? throw new InvalidOperationException("The trim preservation probe did not start.");
        var stdout = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();
        process.WaitForExit();

        var resultPath = Path.Combine(ScenarioRoot, "probe-result.json");
        if (!File.Exists(resultPath))
        {
            throw new InvalidOperationException(
                $"The trim preservation probe wrote no result (exit {process.ExitCode}).{Environment.NewLine}" +
                $"{stdout}{Environment.NewLine}{stderr}{Environment.NewLine}{ReadLog()}");
        }

        Result = TrimProbeResult.Parse(File.ReadAllText(resultPath));
    }

    /// <summary>Reads the probe log, for a failure message.</summary>
    public string ReadLog() => File.Exists(LogPath) ? File.ReadAllText(LogPath) : string.Empty;

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var marker = Path.Combine(directory.FullName, "eng", "build-local.ps1");
            if (File.Exists(marker))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            $"Could not locate the component repository above '{AppContext.BaseDirectory}'.");
    }

    private static string ResolveShell() => "pwsh";
}

/// <summary>The result the probe script writes.</summary>
internal sealed record TrimProbeResult(
    string BatchVersion,
    string Contracts,
    string Core,
    string? CoreDependency,
    string? ContractsDependency,
    int PublishExit,
    int? RunExit,
    int WarningCount,
    IReadOnlyList<string> OwnedWarnings,
    IReadOnlyDictionary<string, JsonElement> Probe)
{
    public static TrimProbeResult Parse(string json)
    {
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;
        return new TrimProbeResult(
            root.GetProperty("batchVersion").GetString() ?? string.Empty,
            root.GetProperty("contracts").GetString() ?? string.Empty,
            root.GetProperty("core").GetString() ?? string.Empty,
            root.GetProperty("coreDependency").ValueKind == JsonValueKind.Null
                ? null
                : root.GetProperty("coreDependency").GetString(),
            root.GetProperty("contractsDependency").ValueKind == JsonValueKind.Null
                ? null
                : root.GetProperty("contractsDependency").GetString(),
            root.GetProperty("publishExit").GetInt32(),
            root.GetProperty("runExit").ValueKind == JsonValueKind.Null
                ? null
                : root.GetProperty("runExit").GetInt32(),
            root.GetProperty("warningCount").GetInt32(),
            root.GetProperty("ownedWarnings").EnumerateArray().Select(item => item.GetString() ?? string.Empty).ToArray(),
            ReadProbe(root));
    }

    private static IReadOnlyDictionary<string, JsonElement> ReadProbe(JsonElement root)
    {
        var probe = root.GetProperty("probe");
        if (probe.ValueKind != JsonValueKind.String)
        {
            return new Dictionary<string, JsonElement>(StringComparer.Ordinal);
        }

        using var document = JsonDocument.Parse(probe.GetString() ?? "{}");
        return document.RootElement.EnumerateObject()
            .ToDictionary(property => property.Name, property => property.Value.Clone(), StringComparer.Ordinal);
    }
}
