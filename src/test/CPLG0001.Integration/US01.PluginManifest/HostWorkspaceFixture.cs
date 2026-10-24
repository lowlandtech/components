using System.Diagnostics;

namespace LowlandTech.Components.Tests.CPLG0001.Integration.US01.PluginManifest;

/// <summary>
/// A real Git workspace that hosts two explicit capability profiles: a backend
/// only host and a Blazor Server host.
///
/// <para>
/// The capabilities of the two hosts are never declared by this fixture. They
/// are derived by the shared Plugins capability contract from the literal
/// project evidence of each host, so the acceptance exercises the same
/// capability vocabulary a real host receives instead of a local copy of its
/// rules.
/// </para>
/// <para>
/// The workspace is read only for the acceptance: the host projects carry an
/// MSBuild target that writes a marker, and the marker must never appear,
/// because validating a package set never builds the host.
/// </para>
/// </summary>
public sealed class HostWorkspaceFixture : IDisposable
{
    /// <summary>The profile name of the backend only host.</summary>
    public const string BackendProfile = "backend";

    /// <summary>The profile name of the Blazor Server host.</summary>
    public const string BlazorProfile = "blazor-server";

    /// <summary>Relative path of the backend only host project.</summary>
    public const string BackendProjectPath = "targets/Backend.csproj";

    /// <summary>Relative path of the Blazor Server host project.</summary>
    public const string BlazorProjectPath = "targets/Server.csproj";

    private readonly string _root;
    private readonly string _marker;

    /// <summary>
    /// Creates the workspace and its two host projects.
    /// </summary>
    public HostWorkspaceFixture()
    {
        _root = Path.Combine(
            Path.GetTempPath(),
            "LowlandTech.Components.Hosts",
            Guid.CreateVersion7().ToString("N"));

        _marker = Path.Combine(_root, "must-not-execute.txt");

        Directory.CreateDirectory(_root);

        InitializeRepository();
        WriteProject(BackendProjectPath, BackendProject);
        WriteProject(BlazorProjectPath, BlazorProject);
    }

    /// <summary>The Git workspace that hosts both profiles.</summary>
    public string Root => _root;

    /// <summary>The marker an MSBuild target of a host project would write.</summary>
    public string Marker => _marker;

    /// <summary>Whether the validation built a host project instead of only reading it.</summary>
    public bool HostProjectWasBuilt => File.Exists(_marker);

    /// <summary>
    /// The registrations of both hosts. A registration is host owned: a
    /// package never declares the profile of the host that carries it.
    /// </summary>
    public static IReadOnlyList<WorkspaceProfileRegistration> Registrations =>
    [
        new WorkspaceProfileRegistration { Profile = BackendProfile, ProjectPath = BackendProjectPath },
        new WorkspaceProfileRegistration { Profile = BlazorProfile, ProjectPath = BlazorProjectPath },
    ];

    /// <summary>
    /// Validates a resolved package closure against one of the registered host
    /// profiles through the shared Plugins capability contract.
    /// </summary>
    /// <param name="profile">The profile of the host to validate against.</param>
    /// <param name="projectPath">The relative path of the host project.</param>
    /// <param name="resolvedPackagePaths">The resolved package closure.</param>
    /// <param name="rootPackageIds">The packages the closure was resolved for.</param>
    public Task<WorkspaceProfilePackageValidation> ValidateAsync(
        string profile,
        string projectPath,
        IEnumerable<string> resolvedPackagePaths,
        IReadOnlyList<string>? rootPackageIds = null) =>
        _root.ValidateWorkspaceProfilePackagesAsync(
            Registrations,
            profile,
            projectPath,
            resolvedPackagePaths,
            rootPackageIds);

    /// <summary>
    /// The literal bytes of a host project, as the validation must leave them.
    /// </summary>
    /// <param name="relativePath">The relative path of the host project.</param>
    public byte[] ReadProjectBytes(string relativePath) =>
        File.ReadAllBytes(Path.Combine(_root, relativePath));

    /// <summary>Removes the temporary workspace.</summary>
    public void Dispose()
    {
        try
        {
            if (Directory.Exists(_root))
            {
                Directory.Delete(_root, recursive: true);
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

    /// <summary>
    /// A backend only host: a web project that never declares Blazor.
    /// </summary>
    private string BackendProject =>
        $"<Project Sdk=\"Microsoft.NET.Sdk.Web\"><PropertyGroup><TargetFramework>net10.0</TargetFramework></PropertyGroup>{ProbeTarget}</Project>";

    /// <summary>
    /// A Blazor Server host: the same web project with the explicit Blazor
    /// declaration the capability contract reads.
    /// </summary>
    private string BlazorProject =>
        $"<Project Sdk=\"Microsoft.NET.Sdk.Web\"><PropertyGroup><TargetFramework>net10.0</TargetFramework><LowlandTechBlazor>true</LowlandTechBlazor></PropertyGroup>{ProbeTarget}</Project>";

    /// <summary>
    /// A target that writes the marker when the project is built. The
    /// validation never builds the host, so the marker never appears.
    /// </summary>
    private string ProbeTarget =>
        $"<Target Name=\"Probe\" BeforeTargets=\"Build\"><WriteLinesToFile File=\"{_marker}\" Lines=\"executed\" /></Target>";

    private void InitializeRepository()
    {
        var start = new ProcessStartInfo("git")
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };

        foreach (var key in start.Environment.Keys
                     .Where(key => key.StartsWith("GIT_", StringComparison.OrdinalIgnoreCase))
                     .ToArray())
        {
            start.Environment.Remove(key);
        }

        foreach (var argument in new[] { "init", "--quiet", "--initial-branch=main", _root })
        {
            start.ArgumentList.Add(argument);
        }

        using var process = Process.Start(start)
                            ?? throw new InvalidOperationException("Git could not start.");

        var output = process.StandardOutput.ReadToEnd();
        var errors = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"The host workspace could not become a Git root: {errors}{output}");
        }
    }

    private void WriteProject(string relativePath, string content)
    {
        var path = Path.Combine(_root, relativePath);

        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, content);
    }
}
