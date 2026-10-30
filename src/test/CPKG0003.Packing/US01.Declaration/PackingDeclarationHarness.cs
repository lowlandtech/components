using System.Diagnostics;
using System.IO.Compression;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace LowlandTech.Components.Tests.CPKG0003.Packing.US01.Declaration;

/// <summary>
/// The mechanical half of the exact-declaration acceptance: it prepares a private
/// feed, runs the native packing tool over the published projects, reads the packed
/// nuspec and drives the refusals. The Given and the When stay in the scenario.
/// <para>
/// The tool is invoked through <c>pwsh -Command</c> with a PowerShell array, which is
/// the documented shape for a subset of projects; <c>pwsh -File</c> cannot bind an
/// array literal to a <c>[string[]]</c> parameter.
/// </para>
/// </summary>
internal sealed class PackingDeclarationHarness
{
    /// <summary>A version per run, so the staging directory of a previous run never collides.</summary>
    public string BatchVersion { get; } = "2026.10.3-" + DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    public const string ContractsVersion = "2026.10.3-20261029112040813496109";
    public const string CoreVersion = "2026.10.3-20261029175917";

    private const string BatchProjects =
        "@('src/lib/LowlandTech.Components.csproj','src/blocks/LowlandTech.Components.Blocks.csproj')";

    private PackingDeclarationHarness(string repositoryRoot, string feedPath)
    {
        RepositoryRoot = repositoryRoot;
        FeedPath = feedPath;
    }

    public string RepositoryRoot { get; }

    public string FeedPath { get; }

    public int PackExit { get; private set; }

    public string PackOutput { get; private set; } = string.Empty;

    public int RepackExit { get; private set; }

    public string RepackOutput { get; private set; } = string.Empty;

    public int EmptySelectionExit { get; private set; }

    public string EmptySelectionOutput { get; private set; } = string.Empty;

    public static PackingDeclarationHarness Prepare()
    {
        var repositoryRoot = FindRepositoryRoot();
        var feed = Path.Combine(Path.GetTempPath(), "cpkg0003-packing", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(feed);

        var contracts = Path.Combine(
            "D:", "lowlandtech", "packages",
            "LowlandTech.Components.Core." + ContractsVersion + ".nupkg");
        if (!File.Exists(contracts))
        {
            throw new FileNotFoundException(
                "The contracts generation the batch has to declare is not published in the workspace feed.",
                contracts);
        }

        File.Copy(contracts, Path.Combine(feed, Path.GetFileName(contracts)), overwrite: true);
        return new PackingDeclarationHarness(repositoryRoot, feed);
    }

    /// <summary>Packs the selected projects into the private feed.</summary>
    public void Pack()
    {
        (PackExit, PackOutput) = RunTool(BatchProjects, BatchVersion);
    }

    /// <summary>Asks the tool for the version the feed already holds.</summary>
    public void Repack()
    {
        (RepackExit, RepackOutput) = RunTool(BatchProjects, BatchVersion);
    }

    /// <summary>Asks the tool to pack an explicitly empty project selection.</summary>
    public void PackWithoutProjects()
    {
        (EmptySelectionExit, EmptySelectionOutput) = RunTool("@()", BatchVersion + "9");
    }

    /// <summary>The dependency declarations of the packed batch package.</summary>
    public IReadOnlyList<string> ReadDeclarations()
    {
        var package = Path.Combine(FeedPath, "LowlandTech.Components." + BatchVersion + ".nupkg");
        if (!File.Exists(package))
        {
            return [];
        }

        using var archive = ZipFile.OpenRead(package);
        var entry = archive.Entries.FirstOrDefault(item => item.FullName.EndsWith(".nuspec", StringComparison.OrdinalIgnoreCase));
        if (entry is null)
        {
            return [];
        }

        using var reader = new StreamReader(entry.Open());
        var nuspec = reader.ReadToEnd();
        return Regex.Matches(nuspec, "<dependency id=\"LowlandTech[^\"]*\" version=\"([^\"]+)\"")
            .Select(match => match.Groups[1].Value)
            .ToArray();
    }

    /// <summary>The version prefix the manifest declares.</summary>
    public string ReadVersionPrefix()
    {
        var manifest = Path.Combine(RepositoryRoot, "eng", "local-packages.json");
        using var document = JsonDocument.Parse(File.ReadAllText(manifest));
        return document.RootElement.GetProperty("versionPrefix").GetString() ?? string.Empty;
    }

    private string ToolPath => Path.Combine(RepositoryRoot, "eng", "build-local.ps1");

    private (int Exit, string Output) RunTool(string projectsExpression, string version)
    {
        var command =
            "& '" + ToolPath + "'" +
            " -FeedPath '" + FeedPath + "'" +
            " -Version '" + version + "'" +
            " -ContractsVersion '" + ContractsVersion + "'" +
            " -Projects " + projectsExpression;

        var start = new ProcessStartInfo("pwsh")
        {
            WorkingDirectory = RepositoryRoot,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };
        start.ArgumentList.Add("-NoProfile");
        start.ArgumentList.Add("-Command");
        start.ArgumentList.Add(command);
        start.Environment["LowlandTechCoreVersion"] = CoreVersion;

        using var process = Process.Start(start)
            ?? throw new InvalidOperationException("The packing tool did not start.");
        var output = process.StandardOutput.ReadToEnd() + process.StandardError.ReadToEnd();
        process.WaitForExit();
        return (process.ExitCode, output);
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
