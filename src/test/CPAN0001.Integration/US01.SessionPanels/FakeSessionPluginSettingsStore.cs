namespace LowlandTech.Components.Tests.CPAN0001.Integration.US01.SessionPanels;

/// <summary>
/// An in-memory session plugin settings store used to arrange the acceptance
/// of the plugin owner bridge: it keeps the enablement flags and the tab order
/// a host persisted, so a withdrawal and a reactivation can be observed
/// without erasing the preferences.
/// </summary>
public sealed class FakeSessionPluginSettingsStore : ISessionPluginSettingsStore
{
    private readonly Dictionary<string, bool> _enabled = new(StringComparer.Ordinal);
    private readonly List<string> _order = [];
    private readonly Dictionary<string, Dictionary<string, object?>> _values = new(StringComparer.Ordinal);

    /// <summary>How often the state was read.</summary>
    public int ReadCount { get; private set; }

    /// <summary>Whether the store reports a failure on the next write.</summary>
    public bool FailsOnSave { get; set; }

    public Task<SessionPluginSettingsState> GetStateAsync(CancellationToken ct = default)
    {
        ReadCount++;

        var state = new SessionPluginSettingsState
        {
            TabOrder = [.. _order],
        };

        foreach (var key in _enabled.Keys)
        {
            state.DirtyKeys.Add(key);
        }

        return Task.FromResult(state);
    }

    public Task ToggleEnabledAsync(string pluginKey, bool isEnabled, CancellationToken ct = default)
    {
        _enabled[pluginKey] = isEnabled;
        return Task.CompletedTask;
    }

    public Task UpdateSettingAsync(string pluginKey, string fieldKey, object? value, CancellationToken ct = default)
    {
        if (!_values.TryGetValue(pluginKey, out var fields))
        {
            fields = new Dictionary<string, object?>(StringComparer.Ordinal);
            _values[pluginKey] = fields;
        }

        fields[fieldKey] = value;
        return Task.CompletedTask;
    }

    public Task ResetPluginSettingsAsync(string pluginKey, CancellationToken ct = default)
    {
        _values.Remove(pluginKey);
        return Task.CompletedTask;
    }

    public Task ReorderTabsAsync(IReadOnlyList<string> orderedKeys, CancellationToken ct = default)
    {
        _order.Clear();
        _order.AddRange(orderedKeys);
        return Task.CompletedTask;
    }

    public Task MovePluginTabAsync(string pluginKey, string direction, CancellationToken ct = default)
    {
        var index = _order.IndexOf(pluginKey);

        if (index < 0)
        {
            return Task.CompletedTask;
        }

        var target = direction switch
        {
            "up" => index - 1,
            "down" => index + 1,
            _ => index,
        };

        if (target < 0 || target >= _order.Count)
        {
            return Task.CompletedTask;
        }

        _order.RemoveAt(index);
        _order.Insert(target, pluginKey);
        return Task.CompletedTask;
    }

    public Task SaveAsync(CancellationToken ct = default) =>
        FailsOnSave
            ? Task.FromException(new InvalidOperationException("The settings could not be saved."))
            : Task.CompletedTask;

    public Task<bool> IsEnabledAsync(string pluginKey, CancellationToken ct = default) =>
        Task.FromResult(_enabled.TryGetValue(pluginKey, out var enabled) ? enabled : true);

    public Task<IReadOnlyList<string>> GetOrderAsync(CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<string>>([.. _order]);
}
