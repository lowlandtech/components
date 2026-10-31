using System.Diagnostics;

namespace LowlandTech.Components.Tests.CPKG0004.Packing.US01.Invocation;

/// <summary>
/// The mechanical half of the project-selection diagnosis: it runs the native
/// packing tool in the three shapes a caller can produce and records the exit and
/// the output. The Given and the When stay in the scenario.
/// </summary>
internal sealed class ProjectSelectionHarness
{
    private const string LibProject = "src/lib/LowlandTech.Components.csproj";
    private const string BlocksProject = "src/blocks/LowlandTech.Components.Blocks.csproj";
    private const string ContractsVersion = "2026.10.3-20261029112040813496109";
    private const string CoreVersion = "2026.10.3-20261029175917";

    private ProjectSelectionHarness(string repositoryRoot, string feedPath, string version)
    {
        RepositoryRoot = repositoryRoot;
        FeedPath = feedPath;
        Version = version;
    }

    public string RepositoryRoot { get; }

    public string FeedPath { get; }

    public string Version { get; }

    public int JoinedExit { get; private set; }

    public string JoinedOutput { get; private set; } = string.Empty;

    public int StrayExit { get; private set; }

    public string StrayOutput { get; private set; } = string.Empty;

    public int SupportedExit { get; private set; }

    public string SupportedOutput { get; private set; } = string.Empty;

    public IReadOnlyList<int> ConcurrentExits { get; private set; } = [];

    public IReadOnlyList<string> ConcurrentOutputs { get; private set; } = [];

    public static ProjectSelectionHarness Prepare()
    {
        var repositoryRoot = FindRepositoryRoot();
        var feed = Path.Combine(Path.GetTempPath(), "cpkg0004-invocation", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(feed);

        var contracts = Path.Combine("D:", "lowlandtech", "packages",
            "LowlandTech.Components.Core." + ContractsVersion + ".nupkg");
        if (!File.Exists(contracts))
        {
            throw new FileNotFoundException(
                "The contracts generation of the packages is not published in the workspace feed.", contracts);
        }

        File.Copy(contracts, Path.Combine(feed, Path.GetFileName(contracts)), overwrite: true);

        var version = "2026.10.3-" + DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        return new ProjectSelectionHarness(repositoryRoot, feed, version);
    }

    /// <summary>A selection that arrives as one comma-joined string.</summary>
    public void RunJoinedSelection()
    {
        (JoinedExit, JoinedOutput) = RunFile(["-FeedPath", FeedPath, "-Version", Version + "1",
            "-ContractsVersion", ContractsVersion, "-Projects", LibProject + "," + BlocksProject]);
    }

    /// <summary>A selection whose second project is left as a positional argument.</summary>
    public void RunStrayPositional()
    {
        (StrayExit, StrayOutput) = RunFile(["-FeedPath", FeedPath, "-Version", Version + "2",
            "-ContractsVersion", ContractsVersion, "-Projects", LibProject, BlocksProject]);
    }

    /// <summary>The supported invocation: an array through -Command.</summary>
    public void RunSupportedSelection()
    {
        (SupportedExit, SupportedOutput) = Collect(StartSupported(Version + "3"));
    }

    /// <summary>
    /// Two batches of different versions started at the same time: they build the
    /// same projects, so the pair proves the tool serializes that work.
    /// </summary>
    public void RunTwoBatchesConcurrently()
    {
        using var first = StartSupported(Version + "4");
        using var second = StartSupported(Version + "5");

        var firstOutput = first.StandardOutput.ReadToEnd() + first.StandardError.ReadToEnd();
        var secondOutput = second.StandardOutput.ReadToEnd() + second.StandardError.ReadToEnd();
        first.WaitForExit();
        second.WaitForExit();

        ConcurrentExits = [first.ExitCode, second.ExitCode];
        ConcurrentOutputs = [Normalise(firstOutput), Normalise(secondOutput)];
    }

    private Process StartSupported(string version)
    {
        var command =
            "& '" + ToolPath + "' -FeedPath '" + FeedPath + "' -Version '" + version + "'" +
            " -ContractsVersion '" + ContractsVersion + "'" +
            " -Projects @('" + LibProject + "','" + BlocksProject + "')";

        var start = NewStart();
        start.ArgumentList.Add("-NoProfile");
        start.ArgumentList.Add("-Command");
        start.ArgumentList.Add(command);
        return Process.Start(start)
            ?? throw new InvalidOperationException("The packing tool did not start.");
    }

    /// <summary>The packages the supported invocation published.</summary>
    public IReadOnlyList<string> PublishedPackages() =>
        Directory.Exists(FeedPath)
            ? Directory.GetFiles(FeedPath, "LowlandTech.Components.*.nupkg")
                .Select(Path.GetFileName)
                .Where(name => name is not null && !name!.StartsWith("LowlandTech.Components.Core.", StringComparison.Ordinal))
                .Select(name => name!)
                .ToArray()
            : [];

    private string ToolPath => Path.Combine(RepositoryRoot, "eng", "build-local.ps1");

    private (int Exit, string Output) RunFile(IReadOnlyList<string> arguments)
    {
        var start = NewStart();
        start.ArgumentList.Add("-NoProfile");
        start.ArgumentList.Add("-File");
        start.ArgumentList.Add(ToolPath);
        foreach (var argument in arguments)
        {
            start.ArgumentList.Add(argument);
        }

        return Run(start);
    }

    private ProcessStartInfo NewStart()
    {
        var start = new ProcessStartInfo("pwsh")
        {
            WorkingDirectory = RepositoryRoot,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };
        start.Environment["LowlandTechCoreVersion"] = CoreVersion;
        return start;
    }

    private static (int Exit, string Output) Run(ProcessStartInfo start)
    {
        using var process = Process.Start(start)
            ?? throw new InvalidOperationException("The packing tool did not start.");
        return Collect(process);
    }

    private static (int Exit, string Output) Collect(Process process)
    {
        var output = process.StandardOutput.ReadToEnd() + process.StandardError.ReadToEnd();
        process.WaitForExit();
        return (process.ExitCode, Normalise(output));
    }

    /// <summary>
    /// The PowerShell host wraps a diagnostic and prefixes the wrapped lines with its
    /// own frame markers, so those markers are removed and the whitespace collapsed
    /// before a scenario asserts a phrase of the message.
    /// </summary>
    private static string Normalise(string output)
    {
        var normalised = System.Text.RegularExpressions.Regex.Replace(output, @"\s*\|\s*", " ");
        normalised = System.Text.RegularExpressions.Regex.Replace(normalised, @"~{2,}", " ");
        return System.Text.RegularExpressions.Regex.Replace(normalised, @"\s+", " ");
    }

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
