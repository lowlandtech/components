namespace LowlandTech.Components;

/// <summary>
/// Aggregated context for a single session.
/// Rendered blocks publish their entity snapshots here for agent requests.
/// </summary>
public sealed class SessionContext
{
    private readonly Dictionary<Guid, SessionContextEntry> _entries = new();

    public event Action? OnChanged;

    public IReadOnlyCollection<SessionContextEntry> Entries => _entries.Values;

    public void Publish(Guid blockId, string blockType, JsonElement entityJson)
    {
        _entries[blockId] = new SessionContextEntry(blockId, blockType, entityJson.Clone());
        OnChanged?.Invoke();
    }

    public void Remove(Guid blockId)
    {
        if (_entries.Remove(blockId))
            OnChanged?.Invoke();
    }

    public JsonElement? Get(Guid blockId)
        => _entries.TryGetValue(blockId, out var entry) ? entry.Entity : null;

    public string ToPromptJson()
        => JsonSerializer.Serialize(
            new SessionPromptPayload(_entries.Values
                .Select(entry => new SessionPromptEntry(entry.BlockId, entry.BlockType, entry.Entity))
                .ToArray()),
            ComponentsJsonContext.Default.SessionPromptPayload);

    public string? Response { get; set; }

    /// <summary>
    /// The session identifier this context belongs to. Passed to the A2A
    /// server as <c>SessionId</c> so the server can correlate tasks.
    /// </summary>
    public string? SessionId { get; set; }

    /// <summary>
    /// The user's input text for the current command. Set by
    /// <see cref="LowlandTech.Components.SessionStateService.SubmitAsync"/>
    /// before invoking the agent, so streaming agents can read it from the
    /// context rather than receiving it as a separate parameter.
    /// </summary>
    public string? Input { get; set; }

    public SessionCompletionSelection? Completion { get; set; }

    /// <summary>
    /// Conversation history as (role, content) turns to be included in the
    /// LLM prompt. Populated by <see cref="SessionStateService.SubmitAsync" />
    /// so the assistant sees prior questions and answers.
    /// </summary>
    public List<(string Role, string Content)> History { get; set; } = [];

    public List<Dictionary<string, string>> ToAgentContext()
        => _entries.Values
            .Select(entry => new Dictionary<string, string>
            {
                ["blockId"] = entry.BlockId.ToString(),
                ["type"] = entry.BlockType,
                ["data"] = entry.Entity.GetRawText()
            })
            .ToList();
}

public sealed record SessionContextEntry(Guid BlockId, string BlockType, JsonElement Entity);

public sealed record SessionProviderModelOption(string Key, string Label);

public sealed record SessionProviderOption(
    string Key,
    string Label,
    IReadOnlyList<SessionProviderModelOption> Models,
    bool RequiresApiKey = false);

public sealed record SessionCompletionSelection(
    string ProviderKey,
    string ModelKey,
    string? ApiKey = null);
