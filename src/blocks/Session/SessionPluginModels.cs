namespace LowlandTech.Components;

/// <summary>
/// Placement surface for a session plugin panel.
/// </summary>
public enum SessionPluginPlacement
{
    RightSidebar,
    WorkspaceTab,
    FooterTool
}

/// <summary>
/// Current status of the plugin settings state machine.
/// </summary>
public enum SessionPluginSettingsStatus
{
    Idle,
    Loading,
    Ready,
    Saving,
    Error
}

/// <summary>
/// Immutable descriptor published by a session plugin.
/// </summary>
public sealed record SessionPluginDescriptor(
    string Key,
    string Label,
    string? Icon,
    int Order,
    SessionPluginPlacement Placement,
    bool IsPinnedByDefault,
    bool IsEnabledByDefault = true,
    bool IsProtected = false,
    bool IsOrderLocked = false,
    Type? SettingsType = null);

/// <summary>
/// Context snapshot passed to every session plugin so panels can render
/// without depending on host-page internals.
/// </summary>
public sealed record SessionPluginContext(
    string? ActiveSessionId,
    IReadOnlyList<SessionListEntry> Sessions,
    IReadOnlyList<SessionContextNode> ContextNodes,
    BlockContext BlockContext,
    IReadOnlyDictionary<string, object?> Parameters)
{
    /// <summary>
    /// Optional callback a plugin can invoke to ask the host to reload the
    /// plugin list after settings or visibility changes.
    /// </summary>
    public Func<Task>? OnReloadRequested { get; init; }
}

/// <summary>
/// Projection of a plugin descriptor used by hosts to render tabs or lists.
/// </summary>
public sealed record SessionPluginDescriptorRepresentation(
    string Key,
    string Label,
    string? Icon,
    int Order,
    SessionPluginPlacement Placement,
    bool IsPinnedByDefault,
    bool IsOrderLocked,
    string? Badge,
    bool IsVisible);

/// <summary>
/// Single field inside a plugin settings group.
/// </summary>
public sealed record SessionPluginSettingField(
    string Key,
    string Label,
    string? Description,
    string Editor,
    object? Value,
    IReadOnlyList<string> Options,
    bool IsReadOnly,
    int Order);

/// <summary>
/// Group of settings fields rendered as one collapsible section.
/// </summary>
public sealed record SessionPluginSettingGroup(
    string Key,
    string Title,
    int Order,
    bool IsExpanded,
    IReadOnlyList<SessionPluginSettingField> Fields);

/// <summary>
/// Settings representation for one plugin.
/// </summary>
public sealed record SessionPluginSettingsRepresentation(
    string Key,
    string Label,
    string? Icon,
    int Order,
    bool IsEnabled,
    bool IsProtected,
    bool IsOrderLocked,
    IReadOnlyList<SessionPluginSettingGroup> Groups);

/// <summary>
/// Mutable state for the plugin settings UI/state machine.
/// </summary>
public sealed class SessionPluginSettingsState
{
    public SessionPluginSettingsStatus Status { get; set; } = SessionPluginSettingsStatus.Idle;
    public List<SessionPluginSettingsRepresentation> Plugins { get; init; } = [];
    public string? SelectedPluginKey { get; set; }
    public List<string> TabOrder { get; init; } = [];
    public HashSet<string> DirtyKeys { get; init; } = new(StringComparer.Ordinal);
    public string? Error { get; set; }
}

/// <summary>
/// Contract for a session-aware panel that contributes to the workspace shell.
/// </summary>
public interface ISessionPlugin
{
    SessionPluginDescriptor Descriptor { get; }

    ValueTask<bool> IsVisibleAsync(SessionPluginContext context, CancellationToken ct = default);

    ValueTask<string?> GetBadgeAsync(SessionPluginContext context, CancellationToken ct = default)
        => ValueTask.FromResult<string?>(null);

    object? GetDefaultSettings() => null;

    RenderFragment Render(SessionPluginContext context);
}

/// <summary>
/// Optional persistence boundary for enabled flags and tab ordering.
/// </summary>
public interface ISessionPluginSettingsStore
{
    Task<SessionPluginSettingsState> GetStateAsync(CancellationToken ct = default);

    Task ToggleEnabledAsync(string pluginKey, bool isEnabled, CancellationToken ct = default);

    Task UpdateSettingAsync(string pluginKey, string fieldKey, object? value, CancellationToken ct = default);

    Task ResetPluginSettingsAsync(string pluginKey, CancellationToken ct = default);

    Task ReorderTabsAsync(IReadOnlyList<string> orderedKeys, CancellationToken ct = default);

    Task MovePluginTabAsync(string pluginKey, string direction, CancellationToken ct = default);

    Task SaveAsync(CancellationToken ct = default);

    Task<bool> IsEnabledAsync(string pluginKey, CancellationToken ct = default);

    Task<IReadOnlyList<string>> GetOrderAsync(CancellationToken ct = default);
}

/// <summary>
/// Discovers and projects registered session plugins for a given placement.
/// </summary>
public interface ISessionPluginRegistry
{
    IReadOnlyList<ISessionPlugin> GetAllPlugins();

    Task<IReadOnlyList<SessionPluginDescriptorRepresentation>> ListAsync(
        SessionPluginContext context,
        SessionPluginPlacement placement,
        CancellationToken ct = default);
}
