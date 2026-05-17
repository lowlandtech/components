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
    /// up from <see cref="AppContext.BaseDirectory"/> until the solution
    /// root is found.
    /// </summary>
    public static async Task<ShowcodeProcess> StartAsync(CancellationToken ct = default)
    {
        var projectPath = FindProjectFile(
            "src", "components", "app", "LowlandTech.Components.App.csproj");

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
        var deadline = DateTime.UtcNow.AddSeconds(90);

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

    private static string FindProjectFile(params string[] relativeParts)
    {
        var attempts = new List<string>();

        foreach (var root in GetSearchRoots())
        {
            foreach (var dir in EnumerateSelfAndParents(root))
            {
                var candidate = Path.Combine([dir.FullName, .. relativeParts]);
                attempts.Add(candidate);

                if (File.Exists(candidate))
                    return candidate;

                var componentSiblingCandidate = Path.Combine(
                    dir.FullName,
                    "app",
                    "LowlandTech.Components.App.csproj");

                attempts.Add(componentSiblingCandidate);

                if (File.Exists(componentSiblingCandidate))
                    return componentSiblingCandidate;
            }
        }

        var diagnostics = string.Join(Environment.NewLine, attempts.Select((p, i) => $"  [{i}] {p}"));
        throw new FileNotFoundException(
            $"Could not find project file: {string.Join("/", relativeParts)}{Environment.NewLine}" +
            $"AppContext.BaseDirectory: {AppContext.BaseDirectory}{Environment.NewLine}" +
            $"CurrentDirectory: {Directory.GetCurrentDirectory()}{Environment.NewLine}" +
            $"Paths checked:{Environment.NewLine}{diagnostics}");
    }

    private static IEnumerable<DirectoryInfo> GetSearchRoots()
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
