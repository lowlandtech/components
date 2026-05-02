using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace LowlandTech.Components.CliAgent;

/// <summary>
/// Registry of live CLI-agent sessions. One entry per CliAgentBlock.
/// Tracks the agent's identity, the configured CLI runtime, the resolved
/// worktree, and (once captured) the CLI's own internal session id parsed
/// from its first output. Process lifecycle is owned by ProcessRunner;
/// this registry only tracks metadata.
/// </summary>
public class CliAgentRegistry
{
    private readonly ConcurrentDictionary<Guid, CliAgentSession> _sessions = new();

    public CliAgentSession Register(CliAgentSession session)
    {
        _sessions[session.BlockId] = session;
        return session;
    }

    public CliAgentSession? Get(Guid blockId)
        => _sessions.TryGetValue(blockId, out var s) ? s : null;

    public bool Remove(Guid blockId) => _sessions.TryRemove(blockId, out _);

    public IReadOnlyList<CliAgentSession> List() => _sessions.Values.ToList();

    /// <summary>
    /// Try to extract the CLI's internal session id from a chunk of stdout
    /// using the per-runtime regex on the session's runtime config. Records
    /// the captured id on the session and returns true on first capture.
    /// Subsequent calls are no-ops once an id is set.
    /// </summary>
    public bool TryCaptureInternalSessionId(Guid blockId, string stdoutChunk)
    {
        if (!_sessions.TryGetValue(blockId, out var session)) return false;
        if (session.InternalSessionId is not null) return false;

        var pattern = session.Runtime.SessionIdRegex;
        if (string.IsNullOrEmpty(pattern)) return false;

        var match = Regex.Match(stdoutChunk, pattern);
        if (!match.Success || match.Groups.Count < 2) return false;

        session.InternalSessionId = match.Groups[1].Value;
        return true;
    }
}

/// <summary>
/// In-memory record of one live CliAgentBlock session. Mutable for the
/// captured InternalSessionId; everything else is set at registration.
/// </summary>
public sealed class CliAgentSession
{
    public required Guid              BlockId           { get; init; }
    public required string            AgentName         { get; init; }   // "kimi1", "codex-gpt-5-4", …
    public required CliAgentRuntime   Runtime           { get; init; }   // executable, args, regex
    public required string            Worktree          { get; init; }   // resolved working directory
    public required Guid              SessionId         { get; init; }   // dashboard session id this block belongs to

    /// <summary>The CLI's own internal session id, parsed from first output. Null until captured.</summary>
    public string? InternalSessionId { get; set; }

    /// <summary>Block-level state for UI/orchestrator queries. Owned by the block.</summary>
    public string Status { get; set; } = "idle";

    public DateTimeOffset StartedAt { get; init; } = DateTimeOffset.UtcNow;
}

/// <summary>
/// Per-CLI runtime config. Captures executable name, default args, the regex
/// to extract the internal session id from stdout, and the reply terminator
/// hint. All optional except <see cref="Executable"/>.
/// </summary>
public sealed record CliAgentRuntime(
    string  Executable,
    string  Arguments         = "",
    string? SessionIdRegex    = null,
    string? ReplyTerminatorRx = null);

/// <summary>
/// One model option exposed to the Coder mode picker. <see cref="ModelKey"/>
/// is the user-visible model identifier ("k2p6", "claude-sonnet-4.6",
/// "gpt-5.4"); <see cref="RuntimeKey"/> is the CliAgentRuntimeMap key it
/// dispatches through ("kimi", "claude", "codex"); <see cref="AgentName"/>
/// is the orchestrator agent identity that owns the worktree (matches an
/// entry in .agents/orchestrator/sessions.json so the dispatch path can
/// resolve the per-agent worktree).
/// </summary>
public sealed record CliAgentModel(
    string ModelKey,
    string Label,
    string RuntimeKey,
    string AgentName);

/// <summary>
/// Default registry of known CLI runtimes. Consumers register their
/// preferred runtime by name (typically the agent identity prefix or the
/// raw CLI name). Override via DI to add custom runtimes.
/// </summary>
public sealed class CliAgentRuntimeMap
{
    private readonly Dictionary<string, CliAgentRuntime> _map;
    private readonly List<CliAgentModel> _coderModels;

    public CliAgentRuntimeMap()
    {
        _map = new(StringComparer.OrdinalIgnoreCase)
        {
            ["claude"] = new(
                Executable:        "claude",
                Arguments:         "--print --output-format stream-json",
                SessionIdRegex:    "\"sessionId\"\\s*:\\s*\"([^\"]+)\"",
                ReplyTerminatorRx: "\"type\"\\s*:\\s*\"result\""),

            ["codex"] = new(
                Executable:        "codex",
                Arguments:         "exec -",
                SessionIdRegex:    "Session:\\s*([a-z0-9-]+)",
                ReplyTerminatorRx: null),  // codex uses idle-timeout default

            ["kimi"] = new(
                Executable:        "kimi",
                Arguments:         "chat --stdin",
                SessionIdRegex:    "\"session\"\\s*:\\s*\"([^\"]+)\"",
                ReplyTerminatorRx: null),
        };

        // Single source of truth for the Coder mode picker. Both the
        // SessionPageLayout's CommandContext-driven picker and the dashboard
        // SessionContentPanel's Redux-driven picker consume this list.
        _coderModels = new()
        {
            new("k2p6",              "Kimi K2.6",         RuntimeKey: "kimi",   AgentName: "kimi1"),
            new("claude-sonnet-4.6", "Claude Sonnet 4.6", RuntimeKey: "claude", AgentName: "copilot-1-sonnet-4-6"),
            new("gpt-5.4",           "ChatGPT 5.4",       RuntimeKey: "codex",  AgentName: "chatgpt-5-4"),
        };
    }

    public CliAgentRuntime Resolve(string workerType)
        => _map.TryGetValue(workerType, out var r)
            ? r
            : throw new InvalidOperationException(
                $"No CliAgentRuntime registered for workerType '{workerType}'. " +
                $"Known: {string.Join(", ", _map.Keys)}.");

    public void Register(string workerType, CliAgentRuntime runtime)
        => _map[workerType] = runtime;

    /// <summary>
    /// The canonical list of models the Coder mode exposes in its picker.
    /// Both SessionPageLayout and dashboard SessionContentPanel consume
    /// this so the model roster never drifts between surfaces.
    /// </summary>
    public IReadOnlyList<CliAgentModel> GetCoderModels() => _coderModels;

    /// <summary>Look up a Coder model by its <see cref="CliAgentModel.ModelKey"/>.</summary>
    public CliAgentModel? FindCoderModel(string modelKey)
        => _coderModels.FirstOrDefault(m => string.Equals(m.ModelKey, modelKey, StringComparison.OrdinalIgnoreCase));
}
