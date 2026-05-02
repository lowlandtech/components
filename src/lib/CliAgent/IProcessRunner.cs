namespace LowlandTech.Components.CliAgent;

/// <summary>
/// Process lifecycle surface for CLI-agent state machines. Mockable so the
/// CliAgentBlock reducer + effects can be tested without touching real
/// processes — every observable agent event (start, stdin write, stdout
/// chunk, exit) routes through this interface and dispatches as a reducer
/// action whose payload is the captured event.
/// </summary>
public interface IProcessRunner
{
    bool IsRunning(string key);

    Task<bool> StartAsync(
        string            key,
        string            command,
        string            arguments,
        string            workingDir,
        Func<string, Task> onOutput,
        Func<int, Task>    onExit,
        bool              redirectStdin = false);

    Task<bool> WriteStdinAsync(string key, string data);

    Task StopAsync(string key);
}
