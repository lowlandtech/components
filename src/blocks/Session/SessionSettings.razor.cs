namespace LowlandTech.Components;

public partial class SessionSettings : TinyComponentBase
{
    [Inject] private IStringLocalizer<SessionBlockStrings>? Localizer { get; set; }

    private string L(string key) => Localizer?[key] ?? key;

    private enum SettingsSection
    {
        ApiKeys,
        NodeTypes,
        EdgeTypes,
        Activity,
        Outbox,
    }

    private SettingsSection _section = SettingsSection.ApiKeys;
    private string? _editingKeyId;
    private string _editingName = string.Empty;
    private string? _editingNodeTypeId;
    private string? _editingEdgeTypeId;
    private NodeTypeFormModel _nodeTypeForm = NodeTypeFormModel.Empty;
    private EdgeTypeFormModel _edgeTypeForm = EdgeTypeFormModel.Empty;
    private string _nodeTypeView = "grid";
    private string _edgeTypeView = "grid";
    private string? _selectedNodeTypeId;
    private string? _selectedEdgeTypeId;
    private string _nodeTypeFilter = string.Empty;
    private string _edgeTypeFilter = string.Empty;
    private SessionStateService? _ownedStateService;

    [Inject] private IServiceProvider Services { get; set; } = default!;

    [Inject] private IEnumerable<INodeTypeDescriptor> NodeTypeDescriptors { get; set; } = [];

    [Inject] private IEnumerable<IEdgeTypeDescriptor> EdgeTypeDescriptors { get; set; } = [];

    [Parameter] public ISessionClientSettingsService? ClientSettings { get; set; }

    [Parameter] public SessionStateService? StateService { get; set; }

    [Parameter] public RenderFragment? ActivityContent { get; set; }

    [Parameter] public ISessionOutbox? Outbox { get; set; }

    [Parameter] public RenderFragment? OutboxContent { get; set; }

    private ISessionClientSettingsService EffectiveClientSettings =>
        ClientSettings
        ?? Services.GetService<ISessionClientSettingsService>()
        ?? LocalSessionClientSettingsService.Instance;

    private SessionStateService EffectiveStateService => StateService ??= (_ownedStateService ??= CreateOwnedStateService());

    private SessionSettingsState SettingsState => EffectiveStateService.State.Settings;

    private IReadOnlyList<SessionNodeTypeDefinition> FilteredNodeTypes => FilterNodeTypes(SettingsState.NodeTypes, _nodeTypeFilter);

    private IReadOnlyList<SessionEdgeTypeDefinition> FilteredEdgeTypes => FilterEdgeTypes(SettingsState.EdgeTypes, _edgeTypeFilter);

    private SessionNodeTypeDefinition? SelectedNodeType =>
        SettingsState.NodeTypes.FirstOrDefault(nodeType => string.Equals(nodeType.Id, _selectedNodeTypeId, StringComparison.Ordinal));

    private SessionEdgeTypeDefinition? SelectedEdgeType =>
        SettingsState.EdgeTypes.FirstOrDefault(edgeType => string.Equals(edgeType.Id, _selectedEdgeTypeId, StringComparison.Ordinal));

    private bool IsNodeTypeFormOpen => _nodeTypeView is "create" or "edit";

    private bool IsEdgeTypeFormOpen => _edgeTypeView is "create" or "edit";

    protected override async Task OnInitializedAsync()
    {
        LoadGraphSchema();
        await LoadAsync();
    }

    private SessionStateService CreateOwnedStateService()
    {
        var service = new SessionStateService(Outbox);
        if (Outbox is not null)
        {
            service.RegisterEffect((state, action, ct) =>
            {
                if (action is ISessionSideEffect sideEffect)
                    Outbox.Append(new SessionOutboxEntry(sideEffect.GetType().Name, sideEffect, DateTimeOffset.UtcNow));

                return Task.CompletedTask;
            });
        }

        return service;
    }

    private string GetClass()
    {
        const string baseClass = "flex h-full min-h-0 w-full overflow-hidden bg-background text-foreground";
        return string.IsNullOrWhiteSpace(Class) ? baseClass : $"{baseClass} {Class}";
    }

    private string GetNavButtonClass(SettingsSection section)
    {
        var active = _section == section;
        return active
            ? "flex w-full items-center rounded-md bg-muted px-2 py-2 text-left text-sm font-semibold text-foreground"
            : "flex w-full items-center rounded-md px-2 py-2 text-left text-sm text-muted-foreground transition-colors hover:bg-muted/60 hover:text-foreground";
    }

    private async Task LoadAsync()
    {
        EffectiveStateService.Dispatch(new LoadSessionClientKeysRequested());

        try
        {
            var keys = await EffectiveClientSettings.ListAsync();
            EffectiveStateService.Dispatch(new LoadSessionClientKeysSucceeded(keys));
        }
        catch (Exception ex)
        {
            EffectiveStateService.Dispatch(new LoadSessionClientKeysFailed(ex.Message));
        }
    }

    private async Task CreateKeyAsync()
    {
        const string name = "Default API Key";
        EffectiveStateService.Dispatch(new CreateSessionClientKeyRequested(name));

        try
        {
            var issued = await EffectiveClientSettings.CreateAsync(name);
            EffectiveStateService.Dispatch(new CreateSessionClientKeySucceeded(issued));
            EffectiveStateService.Dispatch(new ScheduleCreateEffect("session-api-key", issued.Entry.Id, issued.Entry.Name));
        }
        catch (Exception ex)
        {
            EffectiveStateService.Dispatch(new CreateSessionClientKeyFailed(ex.Message));
        }
    }

    private void BeginRename(SessionClientKeyEntry key)
    {
        _editingKeyId = key.Id;
        _editingName = key.Name;
    }

    private void CancelRename()
    {
        _editingKeyId = null;
        _editingName = string.Empty;
    }

    private async Task RenameKeyAsync(SessionClientKeyEntry key)
    {
        var name = string.IsNullOrWhiteSpace(_editingName) ? key.Name : _editingName.Trim();
        EffectiveStateService.Dispatch(new RenameSessionClientKeyRequested(key.Id, name));

        try
        {
            await EffectiveClientSettings.RenameAsync(key.Id, name);
            EffectiveStateService.Dispatch(new RenameSessionClientKeySucceeded(key.Id, name));
            EffectiveStateService.Dispatch(new ScheduleUpdateEffect("session-api-key", key.Id, "Name", name));
            CancelRename();
        }
        catch (Exception ex)
        {
            EffectiveStateService.Dispatch(new RenameSessionClientKeyFailed(ex.Message));
        }
    }

    private async Task RevokeKeyAsync(string id)
    {
        EffectiveStateService.Dispatch(new RevokeSessionClientKeyRequested(id));

        try
        {
            await EffectiveClientSettings.RevokeAsync(id);
            EffectiveStateService.Dispatch(new RevokeSessionClientKeySucceeded(id));
            EffectiveStateService.Dispatch(new ScheduleDeleteEffect("session-api-key", id, id));
        }
        catch (Exception ex)
        {
            EffectiveStateService.Dispatch(new RevokeSessionClientKeyFailed(ex.Message));
        }
    }

    private void DismissIssuedSecret() => EffectiveStateService.Dispatch(new DismissSessionClientKeySecret());

    private void LoadGraphSchema()
    {
        EffectiveStateService.Dispatch(new LoadSessionGraphSchemaRequested());

        try
        {
            var nodeTypes = NodeTypeDescriptors
                .Select(d => new SessionNodeTypeDefinition(
                    d.Id.ToString("N"),
                    d.Key,
                    d.Label,
                    d.Icon,
                    d.BlockComponent,
                    d.IsHeader,
                    d.IsLeaf,
                    true))
                .OrderBy(item => item.Key, StringComparer.Ordinal)
                .ToList();

            var nodeTypesById = NodeTypeDescriptors
                .GroupBy(d => d.Id)
                .ToDictionary(group => group.Key, group => group.First().Key);

            var edgeTypes = EdgeTypeDescriptors
                .Select(d => new SessionEdgeTypeDefinition(
                    d.Id.ToString("N"),
                    d.Key,
                    d.Label,
                    nodeTypesById.TryGetValue(d.SourceNodeTypeId, out var source) ? source : d.SourceNodeTypeId.ToString("N"),
                    nodeTypesById.TryGetValue(d.TargetNodeTypeId, out var target) ? target : d.TargetNodeTypeId.ToString("N"),
                    d.Intent,
                    d.Tool,
                    true))
                .OrderBy(item => item.Key, StringComparer.Ordinal)
                .ToList();

            EffectiveStateService.Dispatch(new LoadSessionGraphSchemaSucceeded(nodeTypes, edgeTypes));
        }
        catch (Exception ex)
        {
            EffectiveStateService.Dispatch(new LoadSessionGraphSchemaFailed(ex.Message));
        }
    }

    private void BeginCreateNodeType()
    {
        _editingNodeTypeId = null;
        _selectedNodeTypeId = null;
        _nodeTypeView = "create";
        _nodeTypeForm = NodeTypeFormModel.Empty with { Label = "New Node Type" };
    }

    private void SelectNodeType(SessionNodeTypeDefinition nodeType)
    {
        _selectedNodeTypeId = nodeType.Id;
        _editingNodeTypeId = null;
        _nodeTypeForm = NodeTypeFormModel.Empty;
        _nodeTypeView = "view";
    }

    private void BeginEditNodeType(SessionNodeTypeDefinition nodeType)
    {
        if (nodeType.IsStatic)
            return;

        _selectedNodeTypeId = nodeType.Id;
        _editingNodeTypeId = nodeType.Id;
        _nodeTypeView = "edit";
        _nodeTypeForm = new NodeTypeFormModel(
            nodeType.Key,
            nodeType.Label,
            nodeType.Icon ?? string.Empty,
            nodeType.BlockComponent ?? string.Empty,
            nodeType.IsHeader,
            nodeType.IsLeaf);
    }

    private void CancelNodeTypeEdit()
    {
        _editingNodeTypeId = null;
        _nodeTypeForm = NodeTypeFormModel.Empty;
        _nodeTypeView = SelectedNodeType is null ? "grid" : "view";
    }

    private void SaveNodeType()
    {
        var key = NormalizeKey(_nodeTypeForm.Key, _nodeTypeForm.Label, "node-type");
        var label = string.IsNullOrWhiteSpace(_nodeTypeForm.Label) ? key : _nodeTypeForm.Label.Trim();
        var nodeType = new SessionNodeTypeDefinition(
            string.IsNullOrWhiteSpace(_editingNodeTypeId) ? Guid.NewGuid().ToString("N") : _editingNodeTypeId,
            key,
            label,
            string.IsNullOrWhiteSpace(_nodeTypeForm.Icon) ? null : _nodeTypeForm.Icon.Trim(),
            string.IsNullOrWhiteSpace(_nodeTypeForm.BlockComponent) ? null : _nodeTypeForm.BlockComponent.Trim(),
            _nodeTypeForm.IsHeader,
            _nodeTypeForm.IsLeaf,
            false);

        if (string.IsNullOrWhiteSpace(_editingNodeTypeId))
        {
            EffectiveStateService.Dispatch(new CreateSessionNodeTypeRequested(nodeType));
            EffectiveStateService.Dispatch(new CreateSessionNodeTypeSucceeded(nodeType));
            EffectiveStateService.Dispatch(new ScheduleCreateEffect("node-type", nodeType.Id, nodeType.Label, IsStatic: false));
        }
        else
        {
            EffectiveStateService.Dispatch(new UpdateSessionNodeTypeRequested(nodeType));
            EffectiveStateService.Dispatch(new UpdateSessionNodeTypeSucceeded(nodeType));
            EffectiveStateService.Dispatch(new ScheduleUpdateEffect("node-type", nodeType.Id, "Definition", nodeType.Label));
        }

        _selectedNodeTypeId = nodeType.Id;
        _editingNodeTypeId = null;
        _nodeTypeForm = NodeTypeFormModel.Empty;
        _nodeTypeView = "view";
    }

    private void BeginCreateEdgeType()
    {
        _editingEdgeTypeId = null;
        _selectedEdgeTypeId = null;
        _edgeTypeView = "create";
        _edgeTypeForm = EdgeTypeFormModel.Empty with
        {
            Label = "New Edge Type",
            Intent = "create",
            SourceNodeTypeKey = SettingsState.NodeTypes.FirstOrDefault()?.Key ?? string.Empty,
            TargetNodeTypeKey = SettingsState.NodeTypes.Skip(1).FirstOrDefault()?.Key ?? SettingsState.NodeTypes.FirstOrDefault()?.Key ?? string.Empty,
        };
    }

    private void SelectEdgeType(SessionEdgeTypeDefinition edgeType)
    {
        _selectedEdgeTypeId = edgeType.Id;
        _editingEdgeTypeId = null;
        _edgeTypeForm = EdgeTypeFormModel.Empty;
        _edgeTypeView = "view";
    }

    private void BeginEditEdgeType(SessionEdgeTypeDefinition edgeType)
    {
        if (edgeType.IsStatic)
            return;

        _selectedEdgeTypeId = edgeType.Id;
        _editingEdgeTypeId = edgeType.Id;
        _edgeTypeView = "edit";
        _edgeTypeForm = new EdgeTypeFormModel(
            edgeType.Key,
            edgeType.Label,
            edgeType.SourceNodeTypeKey,
            edgeType.TargetNodeTypeKey,
            edgeType.Intent ?? string.Empty,
            edgeType.Tool ?? string.Empty);
    }

    private void CancelEdgeTypeEdit()
    {
        _editingEdgeTypeId = null;
        _edgeTypeForm = EdgeTypeFormModel.Empty;
        _edgeTypeView = SelectedEdgeType is null ? "grid" : "view";
    }

    private void SaveEdgeType()
    {
        var key = NormalizeKey(_edgeTypeForm.Key, _edgeTypeForm.Label, "edge-type");
        var label = string.IsNullOrWhiteSpace(_edgeTypeForm.Label) ? key : _edgeTypeForm.Label.Trim();
        var edgeType = new SessionEdgeTypeDefinition(
            string.IsNullOrWhiteSpace(_editingEdgeTypeId) ? Guid.NewGuid().ToString("N") : _editingEdgeTypeId,
            key,
            label,
            _edgeTypeForm.SourceNodeTypeKey.Trim(),
            _edgeTypeForm.TargetNodeTypeKey.Trim(),
            string.IsNullOrWhiteSpace(_edgeTypeForm.Intent) ? null : _edgeTypeForm.Intent.Trim(),
            string.IsNullOrWhiteSpace(_edgeTypeForm.Tool) ? null : _edgeTypeForm.Tool.Trim(),
            false);

        if (string.IsNullOrWhiteSpace(_editingEdgeTypeId))
        {
            EffectiveStateService.Dispatch(new CreateSessionEdgeTypeRequested(edgeType));
            EffectiveStateService.Dispatch(new CreateSessionEdgeTypeSucceeded(edgeType));
            EffectiveStateService.Dispatch(new ScheduleCreateEffect("edge-type", edgeType.Id, edgeType.Label, IsStatic: false));
        }
        else
        {
            EffectiveStateService.Dispatch(new UpdateSessionEdgeTypeRequested(edgeType));
            EffectiveStateService.Dispatch(new UpdateSessionEdgeTypeSucceeded(edgeType));
            EffectiveStateService.Dispatch(new ScheduleUpdateEffect("edge-type", edgeType.Id, "Definition", edgeType.Label));
        }

        _selectedEdgeTypeId = edgeType.Id;
        _editingEdgeTypeId = null;
        _edgeTypeForm = EdgeTypeFormModel.Empty;
        _edgeTypeView = "view";
    }

    private Task OnNodeTypeFilterChanged(string? value)
    {
        _nodeTypeFilter = value ?? string.Empty;
        return Task.CompletedTask;
    }

    private Task OnEdgeTypeFilterChanged(string? value)
    {
        _edgeTypeFilter = value ?? string.Empty;
        return Task.CompletedTask;
    }

    private static IReadOnlyList<SessionNodeTypeDefinition> FilterNodeTypes(
        IReadOnlyList<SessionNodeTypeDefinition> nodeTypes,
        string filter)
    {
        if (string.IsNullOrWhiteSpace(filter))
            return nodeTypes;

        return [.. nodeTypes.Where(nodeType =>
            Contains(nodeType.Key, filter)
            || Contains(nodeType.Label, filter)
            || Contains(nodeType.BlockComponent, filter))];
    }

    private static IReadOnlyList<SessionEdgeTypeDefinition> FilterEdgeTypes(
        IReadOnlyList<SessionEdgeTypeDefinition> edgeTypes,
        string filter)
    {
        if (string.IsNullOrWhiteSpace(filter))
            return edgeTypes;

        return [.. edgeTypes.Where(edgeType =>
            Contains(edgeType.Key, filter)
            || Contains(edgeType.Label, filter)
            || Contains(edgeType.SourceNodeTypeKey, filter)
            || Contains(edgeType.TargetNodeTypeKey, filter)
            || Contains(edgeType.Intent, filter))];
    }

    private static bool Contains(string? value, string filter) =>
        value?.Contains(filter, StringComparison.OrdinalIgnoreCase) == true;

    private static string NormalizeKey(string key, string label, string fallback)
    {
        var source = string.IsNullOrWhiteSpace(key) ? label : key;
        source = string.IsNullOrWhiteSpace(source) ? fallback : source;

        var normalized = Regex.Replace(source.Trim().ToLowerInvariant(), "[^a-z0-9]+", "-").Trim('-');
        return string.IsNullOrWhiteSpace(normalized) ? fallback : normalized;
    }

    private sealed record NodeTypeFormModel(
        string Key,
        string Label,
        string Icon,
        string BlockComponent,
        bool IsHeader,
        bool IsLeaf)
    {
        public static NodeTypeFormModel Empty { get; } = new(string.Empty, string.Empty, string.Empty, "NodeBlock", false, false);
    }

    private sealed record EdgeTypeFormModel(
        string Key,
        string Label,
        string SourceNodeTypeKey,
        string TargetNodeTypeKey,
        string Intent,
        string Tool)
    {
        public static EdgeTypeFormModel Empty { get; } = new(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
    }
}

internal sealed class LocalSessionClientSettingsService : ISessionClientSettingsService
{
    public static LocalSessionClientSettingsService Instance { get; } = new();

    private readonly List<SessionClientKeyEntry> _keys =
    [
        new("local-default", "Default API Key", "ltg_6d5cff...demo", "local user", DateTimeOffset.UtcNow.AddDays(-1)),
    ];

    public Task<IReadOnlyList<SessionClientKeyEntry>> ListAsync(CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<SessionClientKeyEntry>>([.. _keys.Where(key => !key.IsRevoked)]);

    public Task<SessionClientKeyIssued> CreateAsync(string name, CancellationToken ct = default)
    {
        var id = Guid.NewGuid().ToString("N");
        var secret = $"ltg_local_{id[..8]}_{Guid.NewGuid():N}";
        var entry = new SessionClientKeyEntry(
            id,
            string.IsNullOrWhiteSpace(name) ? "Default API Key" : name,
            $"{secret[..12]}...{secret[^4..]}",
            "local user",
            DateTimeOffset.UtcNow);
        _keys.Add(entry);
        return Task.FromResult(new SessionClientKeyIssued(entry, secret));
    }

    public Task RevokeAsync(string id, CancellationToken ct = default)
    {
        var index = _keys.FindIndex(key => string.Equals(key.Id, id, StringComparison.Ordinal));
        if (index >= 0)
            _keys[index] = _keys[index] with { IsRevoked = true };

        return Task.CompletedTask;
    }

    public Task RenameAsync(string id, string name, CancellationToken ct = default)
    {
        var index = _keys.FindIndex(key => string.Equals(key.Id, id, StringComparison.Ordinal));
        if (index >= 0)
            _keys[index] = _keys[index] with { Name = name };

        return Task.CompletedTask;
    }
}
