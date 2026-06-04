using System.Text.Json;

namespace LowlandTech.Components.Feed;

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
        => JsonSerializer.Serialize(new
        {
            context = _entries.Values.Select(entry => new
            {
                blockId = entry.BlockId,
                blockType = entry.BlockType,
                entity = entry.Entity
            })
        });

    public string? Response { get; set; }

    /// <summary>
    /// The user's input text for the current command. Set by
    /// <see cref="LowlandTech.Components.Session.SessionStateService.SubmitAsync"/>
    /// before invoking the agent, so streaming agents can read it from the
    /// context rather than receiving it as a separate parameter.
    /// </summary>
    public string? Input { get; set; }

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