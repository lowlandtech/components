using System.Diagnostics;
using System.Text.Json;

namespace LowlandTech.Components.Tests.CPKG0007.Trimming.US01.Trimmable;

/// <summary>
/// The mechanical half of the trimmed-assembly acceptance: it runs the scenario script
/// and reads the result it writes. The Given and the When stay in the scenario source.
/// </summary>
internal sealed class TrimmedAssembliesHarness
{
    private TrimmedAssembliesHarness(string scenarioRoot, string scriptPath)
    {
        ScenarioRoot = scenarioRoot;
        ScriptPath = scriptPath;
    }

    public string ScenarioRoot { get; }

    public string ScriptPath { get; }

    public string LogPath => Path.Combine(ScenarioRoot, "probe.log");

    public TrimmedAssembliesResult? Result { get; private set; }

    public static TrimmedAssembliesHarness Prepare()
    {
        var repositoryRoot = FindRepositoryRoot();
        var script = Path.Combine(repositoryRoot,
            "src", "test", "CPKG0007.Trimming", "US01.Trimmable", "trimmed-assemblies-scenario.ps1");
        if (!File.Exists(script))
        {
            throw new FileNotFoundException("The trimmed-assembly scenario script is missing.", script);
        }

        var root = Path.Combine(Path.GetTempPath(), "cpkg0007-trimming", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        return new TrimmedAssembliesHarness(root, script);
    }

    public void Run()
    {
        var start = new ProcessStartInfo("pwsh")
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
            ?? throw new InvalidOperationException("The trimmed-assembly scenario did not start.");
        var stdout = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();
        process.WaitForExit();

        var resultPath = Path.Combine(ScenarioRoot, "probe-result.json");
        if (!File.Exists(resultPath))
        {
            throw new InvalidOperationException(
                $"The trimmed-assembly scenario wrote no result (exit {process.ExitCode}).{Environment.NewLine}" +
                $"{stdout}{Environment.NewLine}{stderr}{Environment.NewLine}{ReadLog()}");
        }

        Result = TrimmedAssembliesResult.Parse(File.ReadAllText(resultPath));
    }

    public string ReadLog() => File.Exists(LogPath) ? File.ReadAllText(LogPath) : string.Empty;

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "eng", "build-local.ps1")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            $"Could not locate the component repository above '{AppContext.BaseDirectory}'.");
    }
}

/// <summary>The result the scenario script writes.</summary>
internal sealed record TrimmedAssembliesResult(
    string Version,
    int PackExit,
    int PackWarnings,
    int PublishExit,
    int? RunExit,
    string RunOutput,
    IReadOnlyDictionary<string, long> Shipped,
    IReadOnlyDictionary<string, long> Trimmed,
    IReadOnlyDictionary<string, string> Annotations,
    int RestoreExit,
    string RestoreText)
{
    public static TrimmedAssembliesResult Parse(string json)
    {
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;
        return new TrimmedAssembliesResult(
            root.GetProperty("version").GetString() ?? string.Empty,
            root.GetProperty("packExit").GetInt32(),
            root.GetProperty("packWarnings").GetInt32(),
            root.GetProperty("publishExit").GetInt32(),
            root.GetProperty("runExit").ValueKind == JsonValueKind.Null ? null : root.GetProperty("runExit").GetInt32(),
            root.GetProperty("runOutput").ValueKind == JsonValueKind.Null
                ? string.Empty
                : root.GetProperty("runOutput").GetString() ?? string.Empty,
            ReadSizes(root.GetProperty("shipped")),
            ReadSizes(root.GetProperty("trimmed")),
            ReadAnnotations(root.GetProperty("annotations")),
            root.GetProperty("restoreExit").GetInt32(),
            root.GetProperty("restoreText").ValueKind == JsonValueKind.Null
                ? string.Empty
                : root.GetProperty("restoreText").GetString() ?? string.Empty);
    }

    private static IReadOnlyDictionary<string, long> ReadSizes(JsonElement element) =>
        element.EnumerateObject().ToDictionary(property => property.Name, property => property.Value.GetInt64(), StringComparer.Ordinal);

    private static IReadOnlyDictionary<string, string> ReadAnnotations(JsonElement element) =>
        element.EnumerateObject().ToDictionary(
            property => property.Name,
            property => property.Value.ValueKind == JsonValueKind.True ? "true"
                : property.Value.ValueKind == JsonValueKind.False ? "false"
                : property.Value.GetString() ?? string.Empty,
            StringComparer.Ordinal);
}
