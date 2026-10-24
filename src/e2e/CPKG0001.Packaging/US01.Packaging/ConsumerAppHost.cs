namespace LowlandTech.Components.E2E.CPKG0001.Packaging;

/// <summary>
/// Runs the published consumer application on an ephemeral port so a browser
/// can render what the packages deliver.
/// </summary>
public sealed class ConsumerAppHost : IAsyncDisposable
{
    private readonly Process _process;
    private readonly StringBuilder _output = new();

    private ConsumerAppHost(Process process, string baseUrl)
    {
        _process = process;
        BaseUrl = baseUrl;
    }

    /// <summary>Address the consumer application listens on.</summary>
    public string BaseUrl { get; }

    /// <summary>Everything the consumer application wrote to stdout and stderr.</summary>
    public string Output
    {
        get
        {
            lock (_output)
            {
                return _output.ToString();
            }
        }
    }

    /// <summary>
    /// Starts the published consumer and waits until it answers requests.
    /// </summary>
    /// <param name="publishDirectory">Publish output of the consumer application.</param>
    public static async Task<ConsumerAppHost> StartAsync(string publishDirectory)
    {
        var assembly = Path.Combine(publishDirectory, "LowlandTech.Components.Consumer.dll");
        if (!File.Exists(assembly))
        {
            throw new FileNotFoundException(
                "The consumer application was not published.", assembly);
        }

        var baseUrl = $"http://127.0.0.1:{FindFreePort()}";

        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            WorkingDirectory = publishDirectory,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };

        startInfo.ArgumentList.Add(assembly);
        startInfo.ArgumentList.Add("--urls");
        startInfo.ArgumentList.Add(baseUrl);

        var process = Process.Start(startInfo)
                      ?? throw new InvalidOperationException(
                          "Could not start the consumer application.");

        var host = new ConsumerAppHost(process, baseUrl);
        host._process.OutputDataReceived += host.OnOutput;
        host._process.ErrorDataReceived += host.OnOutput;
        host._process.BeginOutputReadLine();
        host._process.BeginErrorReadLine();

        await host.WaitUntilReadyAsync();
        return host;
    }

    private void OnOutput(object sender, DataReceivedEventArgs e)
    {
        if (e.Data is null)
        {
            return;
        }

        lock (_output)
        {
            _output.AppendLine(e.Data);
        }
    }

    private async Task WaitUntilReadyAsync()
    {
        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
        HttpStatusCode? lastStatus = null;

        for (var attempt = 0; attempt < 60; attempt++)
        {
            try
            {
                var response = await client.GetAsync(BaseUrl);
                lastStatus = response.StatusCode;
                if (response.StatusCode is HttpStatusCode.OK or HttpStatusCode.NotFound)
                {
                    return;
                }
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                // The consumer is not listening yet.
            }

            await Task.Delay(500);
        }

        throw new TimeoutException(
            $"The consumer application did not start within 30 s at {BaseUrl}. " +
            $"Last status: {lastStatus?.ToString() ?? "none"}." +
            $"{Environment.NewLine}{Output}");
    }

    private static int FindFreePort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();

        try
        {
            return ((IPEndPoint)listener.LocalEndpoint).Port;
        }
        finally
        {
            listener.Stop();
        }
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (!_process.HasExited)
            {
                _process.Kill(entireProcessTree: true);
                await _process.WaitForExitAsync();
            }
        }
        catch (InvalidOperationException)
        {
            // The process already ended.
        }
        catch (System.ComponentModel.Win32Exception)
        {
            // The process already ended.
        }

        _process.Dispose();
    }
}
