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
    /// HTTP requests (max ~180 s). The project file is located by walking
    /// up from the compile-time <c>[CallerFilePath]</c> source directory
    /// until a <c>.sln</c> is found, making discovery robust under NCrunch.
    /// </summary>
    /// <remarks>
    /// Uses <c>--configuration Release</c> to avoid VS DLL-lock failures.
    /// VS holds exclusive locks on <c>bin/Debug/</c> DLLs while the solution
    /// is open; building in Release mode outputs to <c>bin/Release/</c> which
    /// VS does not load, so MSBuild copy steps succeed without contention.
    /// Using a dedicated configuration also guarantees that the static web
    /// assets endpoints and runtime manifests are generated together in a
    /// single consistent build pass, preventing WASM boot failures caused by
    /// fingerprint mismatches from incremental Debug builds.
    /// </remarks>
    public static async Task<ShowcodeProcess> StartAsync(CancellationToken ct = default)
    {
        var projectPath = FindProjectFile(
            ["src", "components", "app", "LowlandTech.Components.App.csproj"]);

        Exception? lastEx = null;
        for (var attempt = 1; attempt <= 3; attempt++)
        {
            var port = FindFreePort();
            var baseUrl = $"http://127.0.0.1:{port}";

            var stderrBuf = new StringBuilder();
            var psi = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run --project \"{projectPath}\" --configuration Release --urls {baseUrl} --no-launch-profile",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            var process = Process.Start(psi)
                ?? throw new InvalidOperationException("Failed to start dotnet run.");

            // Drain stdout; capture stderr so failures are diagnosable.
            _ = process.StandardOutput.BaseStream.CopyToAsync(Stream.Null, ct);
            var stderrTask = DrainStderrAsync(process.StandardError, stderrBuf, ct);

            try
            {
                await WaitForReadyAsync(process, baseUrl, ct);
                return new ShowcodeProcess(process, baseUrl);
            }
            catch (Exception ex) when (ex is InvalidOperationException or TimeoutException)
            {
                // dotnet run exited/timed-out before serving; wait for stderr to
                // flush, then retry on a fresh port.
                await Task.WhenAny(stderrTask, Task.Delay(500, ct));
                var failure = new InvalidOperationException(
                    $"Attempt {attempt}: dotnet run failed (exit {(process.HasExited ? process.ExitCode : -1)})." +
                    $" stderr:\n{stderrBuf}", ex);
                try { process.Kill(entireProcessTree: true); } catch { /* already gone */ }
                process.Dispose();

                if (attempt == 3)
                    throw failure;

                lastEx = failure;
                await Task.Delay(TimeSpan.FromSeconds(2), ct);
            }
        }

        throw lastEx ?? new TimeoutException("Showcode app failed to start after 3 attempts.");
    }

    private static async Task DrainStderrAsync(StreamReader reader, StringBuilder buf, CancellationToken ct)
    {
        try
        {
            while (!ct.IsCancellationRequested)
            {
                var line = await reader.ReadLineAsync(ct);
                if (line is null) break;
                buf.AppendLine(line);
            }
        }
        catch (OperationCanceledException) { /* expected on shutdown */ }
    }

    private static async Task WaitForReadyAsync(Process process, string baseUrl, CancellationToken ct)
    {
        using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(3) };
        var deadline = DateTime.UtcNow.AddSeconds(180);

        while (DateTime.UtcNow < deadline && !ct.IsCancellationRequested)
        {
            // Fail fast if dotnet run exited before serving (build error, port conflict, etc.)
            if (process.HasExited)
                throw new InvalidOperationException(
                    $"dotnet run exited with code {process.ExitCode} before serving at {baseUrl}.");

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

        throw new TimeoutException($"Showcode app did not start within 180 s at {baseUrl}.");
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
