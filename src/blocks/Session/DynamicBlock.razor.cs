namespace LowlandTech.Components;

public partial class DynamicBlock : TinyComponentBase, IDynamicBlockDispatcher, IDisposable
{
    [Inject] private IDynamicBlockDiscovery? InjectedDiscovery { get; set; }
    [Inject] private IEnumerable<IDynamicReducerArm> InjectedReducerArms { get; set; } = [];
    [Inject] private IServiceProvider Services { get; set; } = null!;
    [Inject] private ISessionSurfaceDiscovery? SurfaceDiscovery { get; set; }

    [Parameter] public SessionNodeInput? Node { get; set; }
    [Parameter] public object? Model { get; set; }
    [Parameter] public IDynamicBlockDiscovery? Discovery { get; set; }
    [Parameter] public IDynamicNodeRuntime? Runtime { get; set; }
    [Parameter] public IReadOnlyList<IDynamicReducerArm>? ReducerArms { get; set; }
    [Parameter] public EventCallback<DynamicMutationEffect> OnMutationScheduled { get; set; }
    [Parameter] public EventCallback<BlockIntent> OnIntentSelected { get; set; }
    [Parameter] public bool AutoApplyMutations { get; set; } = true;
    [Parameter] public bool AutoSelectRootSession { get; set; } = true;
    [Parameter] public bool HasSidebar { get; set; } = true;
    [Parameter] public bool IsTest { get; set; } = true;
    [Parameter] public string Variant { get; set; } = "none";
    [Parameter] public int? SidebarWidth { get; set; }
    [Parameter] public string? PageOverlayContainerClass { get; set; } = "page-overlay-container absolute inset-0 z-40 flex h-full w-full flex-col bg-background";
    [Parameter] public string? AppOverlayContainerClass { get; set; } = "session-block-overlay absolute inset-0 z-50 flex h-full w-full flex-col overflow-hidden rounded-none bg-background gap-0 py-0";
    [Parameter] public string? Title { get; set; }

    private readonly InMemorySessionOutbox _outbox = new();
    private SessionStateService? _sessionState;
    private DynamicBlockStateMachine? _machine;
    private IReadOnlyList<SessionListEntry> _sessions = [];
    private IReadOnlyList<SessionContextNode> _contextNodes = [];
    private object? _currentModel;
    private SessionNodeInput? _currentNode;
    private IDynamicBlockDescriptor? _activeDescriptor;
    private int _processedMutationCount;
    private CancellationTokenSource? _loadCancellation;
    private int _loadVersion;
    private bool _isLoading;

    public DynamicBlockState DynamicState => _machine?.State ?? new DynamicBlockState();
    public IReadOnlyList<SessionOutboxEntry> OutboxEntries => _outbox.Entries;

    protected override Task OnParametersSetAsync()
    {
        if (ReferenceEquals(_currentModel, Model)
            && Equals(_currentNode, Node)
            && _machine is not null
            && _sessionState is not null)
        {
            return Task.CompletedTask;
        }

        _currentModel = Model;
        _currentNode = Node;
        _activeDescriptor = ResolveDescriptor(Node, Model);

        _sessionState = new SessionStateService(SurfaceDiscovery ?? new SessionSurfaceDiscovery([]), outbox: _outbox);
        _machine = new DynamicBlockStateMachine(ResolveReducerArms(), _outbox);
        _processedMutationCount = _outbox.Entries.Count;

        _loadCancellation?.Cancel();
        _loadCancellation?.Dispose();
        _loadCancellation = new CancellationTokenSource();
        var loadVersion = ++_loadVersion;

        if (Node is not null)
        {
            InitializeSession(Node, [], new Dictionary<string, IReadOnlyDictionary<string, object?>>(StringComparer.Ordinal));
        }

        if (Model is not null && _activeDescriptor is not null)
        {
            _isLoading = false;
            HydrateSession(_activeDescriptor, Model, Node);
            return Task.CompletedTask;
        }

        if (Node is not null && _activeDescriptor is not null && ResolveRuntime() is { } runtime)
        {
            _isLoading = true;
            _ = LoadAndHydrateAsync(Node, _activeDescriptor, runtime, loadVersion, _loadCancellation.Token);
        }
        else
        {
            _isLoading = false;
        }

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _loadCancellation?.Cancel();
        _loadCancellation?.Dispose();
        _loadCancellation = null;
    }

    private async Task LoadAndHydrateAsync(
        SessionNodeInput node,
        IDynamicBlockDescriptor descriptor,
        IDynamicNodeRuntime runtime,
        int loadVersion,
        CancellationToken cancellationToken)
    {
        try
        {
            var loaded = await runtime.LoadAsync(node, descriptor, cancellationToken);
            if (cancellationToken.IsCancellationRequested || loadVersion != _loadVersion)
                return;

            await InvokeAsync(() =>
            {
                if (loadVersion != _loadVersion)
                    return;

                if (loaded is not null)
                    HydrateSession(descriptor, loaded.Instance, loaded.Node);

                _isLoading = false;
                StateHasChanged();
            });
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch
        {
            if (loadVersion == _loadVersion)
            {
                await InvokeAsync(() =>
                {
                    _isLoading = false;
                    StateHasChanged();
                });
            }
        }
    }

    private void HydrateSession(
        IDynamicBlockDescriptor descriptor,
        object model,
        SessionNodeInput? identity)
    {
        var rootNode = CreateNode(descriptor, model, identity: identity);
        var contextNodes = CreateContextNodes(descriptor, model);
        var entityProperties = new Dictionary<string, IReadOnlyDictionary<string, object?>>(StringComparer.Ordinal)
        {
            [rootNode.Id] = CreatePropertyBag(descriptor, model)
        };

        CollectProperties(contextNodes, entityProperties);
        InitializeSession(rootNode, contextNodes, entityProperties);
    }

    private void InitializeSession(
        SessionNodeInput rootNode,
        IReadOnlyList<SessionContextNode> contextNodes,
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, object?>> entityProperties)
    {
        _machine?.Dispatch(new DynamicBlockInitialized(rootNode.Id, entityProperties, contextNodes));
        _sessions = [new SessionListEntry(rootNode.Id, rootNode.Label, 0, rootNode.NodeTypeKey)];
        _contextNodes = contextNodes;
        if (AutoSelectRootSession)
        {
            _sessionState?.SelectSession(rootNode, _sessions);
        }
    }

    public async Task DispatchAsync(DynamicBlockAction action)
    {
        _machine?.Dispatch(action);
        await ProcessScheduledMutationsAsync();
        StateHasChanged();
    }

    public Task ProjectAsync(SessionNodeInput node)
    {
        _sessionState?.ProjectContext(node);
        StateHasChanged();
        return Task.CompletedTask;
    }

    public async Task<DynamicEntity?> CreateAsync(
        string nodeTypeKey,
        string? parentEntityId,
        IReadOnlyDictionary<string, object?> values,
        string? modelTypeName = null)
    {
        _machine?.Dispatch(new DynamicEntityCreateRequested(nodeTypeKey, parentEntityId, values, modelTypeName));
        var results = await ProcessScheduledMutationsAsync();
        StateHasChanged();
        return results.LastOrDefault();
    }

    public Task<DynamicEntity?> CreateAsync(
        string nodeTypeKey,
        IReadOnlyDictionary<string, object?> values,
        string? parentEntityId = null)
        => CreateAsync(nodeTypeKey, parentEntityId, values);

    public Task EditAsync(
        SessionNodeInput node,
        IReadOnlyDictionary<string, object?> values)
        => DispatchAsync(new DynamicEntityEditRequested(node.Id, node.NodeTypeKey, values));

    public Task DeleteAsync(SessionNodeInput node)
        => DispatchAsync(new DynamicEntityDeleteRequested(node.Id, node.NodeTypeKey));

    private IDynamicBlockDescriptor? ResolveDescriptor(SessionNodeInput? node, object? model)
    {
        var discovery = Discovery
            ?? InjectedDiscovery
            ?? new DynamicBlockDiscovery([new DefaultDynamicBlockRegistry()]);

        return discovery.GetDescriptor(node?.NodeTypeKey)
            ?? discovery.GetDescriptor(model);
    }

    private IReadOnlyList<IDynamicReducerArm> ResolveReducerArms()
    {
        var arms = ReducerArms ?? InjectedReducerArms.ToList();
        return arms.Count > 0
            ? arms
            : [new DynamicInitializerReducerArm(), new DynamicFieldReducerArm(), new DynamicEntityMutationReducerArm()];
    }

    private IReadOnlyList<SessionContextNode> CreateContextNodes(IDynamicBlockDescriptor descriptor, object model)
    {
        if (descriptor.Collections.Count == 0)
            return [new SessionContextNode(CreateNode(descriptor, model), IsLoaded: true)];

        return descriptor.Collections
            .Select(collection => CreateCollectionContextNode(descriptor, model, collection))
            .ToList();
    }

    private SessionContextNode CreateCollectionContextNode(
        IDynamicBlockDescriptor parentDescriptor,
        object parent,
        DynamicBlockCollectionDescriptor collection)
    {
        var items = collection.GetItems(parent)
            .Where(item => item is not null)
            .Cast<object>()
            .ToList();
        var children = items
            .Select(item => CreateChildContextNode(collection, item))
            .ToList();
        var childDescriptor = items.Count > 0
            ? ResolveDescriptor(null, items[0])
            : ResolveDescriptor(new SessionNodeInput(string.Empty, collection.ChildNodeTypeKey, collection.Label), null);
        var properties = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["dynamic.view-mode"] = "grid",
            ["dynamic.collection.items"] = items,
            ["dynamic.collection.nodes"] = children.Select(child => child.Node).ToList(),
            ["dynamic.collection.fields"] = childDescriptor?.Fields ?? [],
            ["dynamic.collection.node-type"] = childDescriptor?.NodeTypeKey ?? collection.ChildNodeTypeKey,
            ["dynamic.collection.model-type"] = childDescriptor?.ModelType.FullName,
            ["dynamic.collection.parent-id"] = parentDescriptor.GetId(parent),
            ["dynamic.dispatcher"] = this
        };

        return new SessionContextNode(
            new SessionNodeInput(
                $"{parentDescriptor.GetId(parent)}:{collection.Key}",
                collection.HeaderNodeTypeKey,
                collection.Label,
                "DynamicNodeBlock",
                properties),
            children,
            IsLoaded: true);
    }

    private SessionContextNode CreateChildContextNode(DynamicBlockCollectionDescriptor collection, object item)
    {
        var descriptor = ResolveDescriptor(null, item);
        var nodeTypeKey = descriptor?.NodeTypeKey ?? CreateNodeTypeKey(item) ?? collection.ChildNodeTypeKey;
        var node = descriptor is null
            ? new SessionNodeInput(CreateId(item), nodeTypeKey, CreateLabel(item), "DynamicNodeBlock", CreatePropertyBag(item))
            : CreateNode(descriptor, item, nodeTypeKey);

        return new SessionContextNode(node, IsLoaded: true);
    }

    private SessionNodeInput CreateNode(
        IDynamicBlockDescriptor descriptor,
        object model,
        string? nodeTypeKey = null,
        SessionNodeInput? identity = null)
    {
        var properties = CreatePropertyBag(descriptor, model).ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.Ordinal);
        properties["dynamic.fields"] = descriptor.Fields;
        properties["dynamic.model"] = model;
        properties["dynamic.presentation"] = descriptor.Presentation;
        properties["dynamic.view-mode"] = "form";
        properties["dynamic.dispatcher"] = this;

        return new SessionNodeInput(
            identity?.Id ?? descriptor.GetId(model),
            identity?.NodeTypeKey ?? nodeTypeKey ?? descriptor.NodeTypeKey,
            identity?.Label ?? descriptor.GetLabel(model),
            "DynamicNodeBlock",
            properties);
    }

    private async Task HandleHeaderCreateAsync(SessionHeaderCreateRequest request)
    {
        var collection = _activeDescriptor?.Collections.FirstOrDefault(candidate =>
            string.Equals(candidate.HeaderNodeTypeKey, request.ParentNodeTypeKey, StringComparison.OrdinalIgnoreCase)
            || request.ParentNodeId.EndsWith($":{candidate.Key}", StringComparison.OrdinalIgnoreCase));
        var nodeTypeKey = collection?.ChildNodeTypeKey ?? request.ParentNodeTypeKey;
        var descriptor = ResolveDescriptor(
            new SessionNodeInput(string.Empty, nodeTypeKey, request.Label),
            null);
        var labelField = GetLabelFieldKey(descriptor);
        await CreateAsync(
            nodeTypeKey,
            DynamicState.RootNodeId,
            new Dictionary<string, object?>(StringComparer.Ordinal) { [labelField] = request.Label });
    }

    private Task HandleNodeRenameAsync(SessionNodeRenameRequest request)
    {
        var descriptor = ResolveDescriptor(
            new SessionNodeInput(request.NodeId, request.NodeTypeKey, request.OldLabel),
            null);
        return EditAsync(
            new SessionNodeInput(request.NodeId, request.NodeTypeKey, request.OldLabel),
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                [GetLabelFieldKey(descriptor)] = request.NewLabel
            });
    }

    private Task HandleNodeDeleteAsync(SessionNodeInput node) => DeleteAsync(node);

    private IDynamicNodeRuntime? ResolveRuntime() => Runtime ?? Services.GetService<IDynamicNodeRuntime>();

    private static string GetLabelFieldKey(IDynamicBlockDescriptor? descriptor)
        => descriptor?.Fields.FirstOrDefault(field =>
            string.Equals(field.Key, "Name", StringComparison.OrdinalIgnoreCase)
            || string.Equals(field.Key, "Label", StringComparison.OrdinalIgnoreCase)
            || string.Equals(field.Key, "Title", StringComparison.OrdinalIgnoreCase))?.Key
            ?? "Name";

    private async Task<IReadOnlyList<DynamicEntity>> ProcessScheduledMutationsAsync()
    {
        if (_machine is null)
            return [];

        var results = new List<DynamicEntity>();
        var pending = _outbox.Entries
            .Skip(_processedMutationCount)
            .Select(entry => entry.Payload)
            .OfType<DynamicMutationEffect>()
            .ToList();
        _processedMutationCount = _outbox.Entries.Count;

        foreach (var mutation in pending)
        {
            await OnMutationScheduled.InvokeAsync(mutation);

            if (!AutoApplyMutations || ResolveRuntime() is not { } runtime)
                continue;

            var descriptor = ResolveMutationDescriptor(mutation);
            if (descriptor is not null)
            {
                var result = await runtime.ApplyAsync(mutation, descriptor);
                if (result is not null)
                    results.Add(result);
            }
        }

        return results;
    }

    private IDynamicBlockDescriptor? ResolveMutationDescriptor(DynamicMutationEffect mutation)
    {
        var discovery = Discovery
            ?? InjectedDiscovery
            ?? new DynamicBlockDiscovery([new DefaultDynamicBlockRegistry()]);
        if (!string.IsNullOrWhiteSpace(mutation.ModelTypeName))
        {
            var typedDescriptor = discovery.GetDescriptors().FirstOrDefault(candidate =>
                string.Equals(candidate.ModelType.FullName, mutation.ModelTypeName, StringComparison.Ordinal));
            if (typedDescriptor is not null)
                return typedDescriptor;
        }

        return ResolveDescriptor(
            mutation.NodeTypeKey is { Length: > 0 }
                ? new SessionNodeInput(mutation.EntityId ?? string.Empty, mutation.NodeTypeKey, mutation.NodeTypeKey)
                : null,
            null);
    }

    private static IReadOnlyDictionary<string, object?> CreatePropertyBag(IDynamicBlockDescriptor descriptor, object model)
        => descriptor.Fields.ToDictionary(
            field => field.Key,
            field => descriptor.GetFieldValue(model, field.Key),
            StringComparer.Ordinal);

    [UnconditionalSuppressMessage("Trimming", "IL2075", Justification = "The entity or component is supplied by the host and a DynamicallyAccessedMembers requirement cannot flow from object.GetType(); the host preserves the reflected members, which the trimmed-host preservation probe checks.")]
    private static IReadOnlyDictionary<string, object?> CreatePropertyBag(object model)
        => model.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => property.CanRead)
            .Where(property => property.PropertyType == typeof(string) || !typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
            .ToDictionary(property => property.Name, property => property.GetValue(model), StringComparer.Ordinal);

    private static void CollectProperties(
        IEnumerable<SessionContextNode> nodes,
        IDictionary<string, IReadOnlyDictionary<string, object?>> propertiesById)
    {
        foreach (var node in nodes)
        {
            if (node.Node.Properties is not null)
            {
                propertiesById[node.Node.Id] = node.Node.Properties
                    .Where(pair => !pair.Key.StartsWith("dynamic.", StringComparison.Ordinal))
                    .ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.Ordinal);
            }

            if (node.Children is not null)
            {
                CollectProperties(node.Children, propertiesById);
            }
        }
    }

    [UnconditionalSuppressMessage("Trimming", "IL2075", Justification = "The entity or component is supplied by the host and a DynamicallyAccessedMembers requirement cannot flow from object.GetType(); the host preserves the reflected members, which the trimmed-host preservation probe checks.")]
    private static string CreateId(object model)
    {
        var id = model.GetType().GetProperty("Id")?.GetValue(model);
        return Convert.ToString(id, CultureInfo.InvariantCulture) ?? CreateStableId(model);
    }

    [UnconditionalSuppressMessage("Trimming", "IL2075", Justification = "The entity or component is supplied by the host and a DynamicallyAccessedMembers requirement cannot flow from object.GetType(); the host preserves the reflected members, which the trimmed-host preservation probe checks.")]
    private static string CreateLabel(object model)
    {
        var type = model.GetType();
        var label = type.GetProperty("Label")?.GetValue(model)
            ?? type.GetProperty("Name")?.GetValue(model)
            ?? type.GetProperty("Title")?.GetValue(model);
        return Convert.ToString(label, CultureInfo.InvariantCulture) ?? type.Name;
    }

    [UnconditionalSuppressMessage("Trimming", "IL2075", Justification = "The entity or component is supplied by the host and a DynamicallyAccessedMembers requirement cannot flow from object.GetType(); the host preserves the reflected members, which the trimmed-host preservation probe checks.")]
    private static string? CreateNodeTypeKey(object model)
    {
        var type = model.GetType();
        var nodeType = type.GetProperty("NodeTypeKey")?.GetValue(model)
            ?? type.GetProperty("NodeType")?.GetValue(model)
            ?? type.GetProperty("Kind")?.GetValue(model)
            ?? type.GetProperty("Type")?.GetValue(model);
        var key = Convert.ToString(nodeType, CultureInfo.InvariantCulture);

        return string.IsNullOrWhiteSpace(key)
            ? null
            : ReflectionDynamicBlockDescriptor.ToNodeTypeKey(key);
    }

    private static string CreateStableId(object model)
    {
        var label = CreateLabel(model);
        var key = ReflectionDynamicBlockDescriptor.ToNodeTypeKey(label);
        return $"{ReflectionDynamicBlockDescriptor.ToNodeTypeKey(model.GetType().Name)}:{key}";
    }

    private string GetClass()
    {
        var baseClass = "h-full w-full";
        return string.IsNullOrWhiteSpace(Class) ? baseClass : $"{baseClass} {Class}";
    }
}
