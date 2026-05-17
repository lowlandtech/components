namespace LowlandTech.Components.E2E.Infrastructure;

/// <summary>
/// Starts the Blazor WASM Showcode app (<c>LowlandTech.Components.App</c>)
/// via <c>dotnet run</c> on an ephemeral loopback port and waits until
/// Kestrel is accepting HTTP requests.
///
/// Dispose kills the process. Intended as a one-per-test-session singleton
/// managed by xUnit's <see cref="IAsyncLifetime"/> / collection fixtures.
/// </summary>
public sealed class ShowcodeProcess : IAsyncDisposable
{
    private readonly Process _process;

    public string BaseUrl { get; }

    private ShowcodeProcess(Process process, string baseUrl)
    {
        _process = process;
        BaseUrl = baseUrl;
    }

    /// <summary>
    /// Boot the WASM dev-server and return when it is ready to serve
    /// HTTP requests (max ~60 s). The project file is located by walking
    /// up from the compile-time <c>[CallerFilePath]</c> source directory
    /// until a <c>.sln</c> is found, making discovery robust under NCrunch.
    /// </summary>
    public static async Task<ShowcodeProcess> StartAsync(CancellationToken ct = default)
    {
        var projectPath = FindProjectFile(
            ["src", "components", "app", "LowlandTech.Components.App.csproj"]);

        var port = FindFreePort();
        var baseUrl = $"http://127.0.0.1:{port}";

        var psi = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"run --project \"{projectPath}\" --urls {baseUrl} --no-launch-profile",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        var process = Process.Start(psi)
            ?? throw new InvalidOperationException("Failed to start dotnet run.");

        await WaitForReadyAsync(baseUrl, ct);

        return new ShowcodeProcess(process, baseUrl);
    }

    private static async Task WaitForReadyAsync(string baseUrl, CancellationToken ct)
    {
        using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(3) };
        var deadline = DateTime.UtcNow.AddSeconds(180);

        while (DateTime.UtcNow < deadline && !ct.IsCancellationRequested)
        {
            try
            {
                var response = await http.GetAsync(baseUrl, ct);
                if (response.StatusCode is HttpStatusCode.OK or HttpStatusCode.NotFound)
                    return; // server is responding
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or OperationCanceledException)
            {
                // not ready yet
            }

            await Task.Delay(1_000, ct);
        }

        throw new TimeoutException($"Showcode app did not start within 90 s at {baseUrl}.");
    }

    // [CallerFilePath] is substituted by the compiler with the real source path
    // (e.g. D:\lowlandtech\develop\src\components\e2e\Infrastructure\ShowcodeProcess.cs).
    // This survives NCrunch binary relocation because it is a compile-time constant.
    private static string FindProjectFile(
        string[] relativeParts,
        [CallerFilePath] string sourceFilePath = "")
    {
        var attempts = new List<string>();

        // NCrunch rewrites [CallerFilePath] to its own mirrored temp path, but
        // the PDB still contains the original on-disk source path. Reading a
        // StackFrame with file info extracts it from the PDB at runtime.
        var pdbSourcePath = new StackTrace(fNeedFileInfo: true)
            .GetFrames()
            .Select(f => f.GetFileName())
            .FirstOrDefault(f => f is not null && f.Contains("ShowcodeProcess"));

        foreach (var anchor in new[] { pdbSourcePath, sourceFilePath })
        {
            if (string.IsNullOrEmpty(anchor) || !File.Exists(anchor))
                continue;

            foreach (var dir in EnumerateSelfAndParents(new DirectoryInfo(Path.GetDirectoryName(anchor)!)))
            {
                var candidate = Path.Combine([dir.FullName, .. relativeParts]);
                attempts.Add(candidate);
                if (File.Exists(candidate))
                    return candidate;
            }
        }

        // Fallback: runtime roots (works for normal dotnet test runs).
        foreach (var root in GetRuntimeSearchRoots())
        {
            foreach (var dir in EnumerateSelfAndParents(root))
            {
                var candidate = Path.Combine([dir.FullName, .. relativeParts]);
                attempts.Add(candidate);
                if (File.Exists(candidate))
                    return candidate;
            }
        }

        var diagnostics = string.Join(Environment.NewLine, attempts.Select((p, i) => $"  [{i}] {p}"));
        throw new FileNotFoundException(
            $"Could not find project file: {string.Join("/", relativeParts)}{Environment.NewLine}" +
            $"AppContext.BaseDirectory: {AppContext.BaseDirectory}{Environment.NewLine}" +
            $"CurrentDirectory: {Directory.GetCurrentDirectory()}{Environment.NewLine}" +
            $"CallerFilePath: {sourceFilePath}{Environment.NewLine}" +
            $"PdbSourcePath: {pdbSourcePath}{Environment.NewLine}" +
            $"Paths checked:{Environment.NewLine}{diagnostics}");
    }

    private static IEnumerable<DirectoryInfo> GetRuntimeSearchRoots()
    {
        var roots = new[]
        {
            AppContext.BaseDirectory,
            Directory.GetCurrentDirectory(),
            Path.GetDirectoryName(typeof(ShowcodeProcess).Assembly.Location),
        };

        return roots
            .Where(root => !string.IsNullOrWhiteSpace(root))
            .Select(root => new DirectoryInfo(root!))
            .Where(root => root.Exists)
            .DistinctBy(root => root.FullName);
    }

    private static IEnumerable<DirectoryInfo> EnumerateSelfAndParents(DirectoryInfo start)
    {
        var dir = start;
        while (dir is not null)
        {
            yield return dir;
            dir = dir.Parent;
        }
    }

    private static int FindFreePort()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    public async ValueTask DisposeAsync()
    {
        if (!_process.HasExited)
        {
            _process.Kill(entireProcessTree: true);
            await _process.WaitForExitAsync();
        }

        _process.Dispose();
    }
}
