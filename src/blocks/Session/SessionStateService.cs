namespace LowlandTech.Components;

public sealed class SessionStateService
{
    private SessionBlockState _state;

    private static readonly ISessionSurfaceDiscovery DefaultSurfaceDiscovery = new SessionSurfaceDiscovery([]);

    private readonly ISessionSurfaceDiscovery _surfaceDiscovery;
    private readonly List<Func<SessionBlockState, object, CancellationToken, Task>> _effects = [];
    private readonly IGraphService? _graphService;
    private readonly ILogger? _logger;

    public SessionStateService()
        : this(SessionBlockDefaults.Empty, null, null, null, null)
    {
    }

    public SessionStateService(ISessionOutbox? outbox)
        : this(SessionBlockDefaults.Empty, null, null, null, outbox)
    {
    }

    public SessionStateService(SessionBlockState initialState)
        : this(initialState, null, null, null, null)
    {
    }

    public SessionStateService(ISessionSurfaceDiscovery? surfaceDiscovery)
        : this(SessionBlockDefaults.Empty, surfaceDiscovery, null, null, null)
    {
    }

    public SessionStateService(ISessionSurfaceDiscovery? surfaceDiscovery, ISessionOutbox? outbox)
        : this(SessionBlockDefaults.Empty, surfaceDiscovery, null, null, outbox)
    {
    }

    public SessionStateService(ISessionSurfaceDiscovery? surfaceDiscovery, IGraphService? graphService)
        : this(SessionBlockDefaults.Empty, surfaceDiscovery, graphService, null, null)
    {
    }

    public SessionStateService(ISessionSurfaceDiscovery? surfaceDiscovery, IGraphService? graphService, ILogger? logger)
        : this(SessionBlockDefaults.Empty, surfaceDiscovery, graphService, logger, null)
    {
    }

    public SessionStateService(SessionBlockState initialState, ISessionSurfaceDiscovery? surfaceDiscovery)
        : this(initialState, surfaceDiscovery, null, null, null)
    {
    }

    public SessionStateService(SessionBlockState initialState, ISessionSurfaceDiscovery? surfaceDiscovery, IGraphService? graphService)
        : this(initialState, surfaceDiscovery, graphService, null, null)
    {
    }

    public SessionStateService(SessionBlockState initialState, ISessionSurfaceDiscovery? surfaceDiscovery, IGraphService? graphService, ILogger? logger, ISessionOutbox? outbox = null)
    {
        _state = initialState;
        _surfaceDiscovery = surfaceDiscovery ?? DefaultSurfaceDiscovery;
        _graphService = graphService;
        _logger = logger;
        Outbox = outbox;
    }

    public SessionBlockState State => _state;

    public ISessionOutbox? Outbox { get; }

    public bool HasGraphService => _graphService is not null;

    public IReadOnlyList<SessionListEntry> Sessions { get; private set; } = [];

    public IReadOnlyList<SessionContextNode> ContextNodes { get; private set; } = [];

    public event Action<SessionBlockState>? StateChanged;

    public SessionBlockState Dispatch(object action)
    {
        if (!ApplyReducer(action))
            return _state;

        RunEffectsAsync(action);
        return _state;
    }

    public async Task<SessionBlockState> DispatchAsync(object action, CancellationToken ct = default)
    {
        ApplyReducer(action);
        await WaitForEffectsAsync(action, ct);
        return _state;
    }

    private bool ApplyReducer(object action)
    {
        var next = SessionBlockReducer.Reduce(_state, action);
        if (EqualityComparer<SessionBlockState>.Default.Equals(_state, next))
            return false;

        _state = next;
        StateChanged?.Invoke(_state);
        return true;
    }

    private void RunEffectsAsync(object action)
    {
        foreach (var effect in _effects)
        {
            _ = RunEffectAsync(effect, action, CancellationToken.None);
        }
    }

    private async Task WaitForEffectsAsync(object action, CancellationToken ct)
    {
        foreach (var effect in _effects)
        {
            await RunEffectAsync(effect, action, ct);
        }
    }

    private async Task RunEffectAsync(Func<SessionBlockState, object, CancellationToken, Task> effect, object action, CancellationToken ct)
    {
        try
        {
            await effect(_state, action, ct);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.TraceError($"SessionStateService effect for action '{action.GetType().Name}' failed: {ex}");
        }
    }

    public void RegisterEffect(Func<SessionBlockState, object, CancellationToken, Task> effect)
        => _effects.Add(effect);

    public void Reset(SessionBlockState? state = null)
    {
        _state = state ?? SessionBlockDefaults.Empty;
        StateChanged?.Invoke(_state);
    }

    #region Graph lifecycle

    public async Task InitializeAsync(string userId, string? userName, CancellationToken ct = default)
    {
        if (_graphService is null)
        {
            Sessions = [];
            ContextNodes = [];
            StateChanged?.Invoke(_state);
            return;
        }

        try
        {
            if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out _))
            {
                Sessions = [];
                ContextNodes = [];
                StateChanged?.Invoke(_state);
                return;
            }

            var userContext = await _graphService.GetUserContextAsync(userId, ct);
            if (userContext is null)
            {
                Sessions = [];
                ContextNodes = [];
                StateChanged?.Invoke(_state);
                return;
            }

            var view = await _graphService.GetUserContextViewAsync(userContext.Id, ct);
            Sessions = view?.Sessions
                .Select((s, i) => new SessionListEntry(s.Id.ToString(), s.Label ?? "Session", i, "session"))
                .ToList() ?? [];

            await LoadContextNodesAsync(userContext.Id, userName, ct);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to load sessions from graph");
            Sessions = [];
            ContextNodes = [];
            StateChanged?.Invoke(_state);
        }
    }

    public async Task LoadContextNodesAsync(Guid userContextId, string? userName, CancellationToken ct = default)
    {
        if (_graphService is null) return;

        var childrenTree = await _graphService.GetChildrenAsync(userContextId, ct: ct);
        var edgeSortOrders = childrenTree.Edges
            .Where(e => e.SourceNodeId == userContextId)
            .GroupBy(e => e.TargetNodeId)
            .ToDictionary(g => g.Key, g => g.First().SortOrder);

        var headers = childrenTree.Nodes
            .Where(n => n.Id != userContextId)
            .OrderBy(n => edgeSortOrders.GetValueOrDefault(n.Id, 0))
            .Select(n => new SessionContextNode(
                new SessionNodeInput(n.Id.ToString(), n.NodeType?.Key ?? "header", n.Label ?? "Header"),
                null,
                false))
            .ToList();

        ContextNodes =
        [
            new SessionContextNode(
                new SessionNodeInput(userContextId.ToString(), "user-context", userName ?? "My Context"),
                headers,
                true)
        ];

        StateChanged?.Invoke(_state);
    }

    public async Task LoadChildrenAsync(SessionNodeInput node, CancellationToken ct = default)
    {
        if (_graphService is null) return;
        if (!Guid.TryParse(node.Id, out var nodeId))
        {
            _logger?.LogWarning("Cannot load children: node id '{NodeId}' is not a valid GUID", node.Id);
            return;
        }

        try
        {
            _logger?.LogDebug("Loading children for node {NodeId}", nodeId);
            var tree = await _graphService.GetChildrenAsync(nodeId, ct: ct);

            var edgeSortOrders = tree.Edges
                .Where(e => e.SourceNodeId == nodeId)
                .GroupBy(e => e.TargetNodeId)
                .ToDictionary(g => g.Key, g => g.First().SortOrder);

            var children = tree.Nodes
                .Where(n => n.Id != nodeId)
                .OrderBy(n => edgeSortOrders.GetValueOrDefault(n.Id, 0))
                .Select(n => new SessionContextNode(
                    new SessionNodeInput(n.Id.ToString(), n.NodeType?.Key ?? "node", n.Label ?? "Node")))
                .ToList();

            ContextNodes = UpdateNodeChildren(ContextNodes, node.Id, children);
            _logger?.LogDebug("Loaded {Count} children for node {NodeId}", children.Count, nodeId);
            StateChanged?.Invoke(_state);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to load children for node {NodeId}", nodeId);
        }
    }

    public async Task<SessionHeaderCreateResult> CreateHeaderChildAsync(SessionHeaderCreateRequest request, string userId, CancellationToken ct = default)
    {
        if (_graphService is null)
            return SessionHeaderCreateResult.Failed("Graph service not available");

        try
        {
            if (!Guid.TryParse(request.ParentNodeId, out var parentId))
            {
                _logger?.LogWarning("Cannot create child node: parent id '{ParentId}' is not a valid GUID", request.ParentNodeId);
                return SessionHeaderCreateResult.Failed("Invalid parent node id");
            }

            var parentNode = await _graphService.GetNodeAsync(parentId, ct);
            var schema = await _graphService.GetSchemaAsync(ct);
            var edgeType = ResolveCreateEdge(schema, request.ParentNodeTypeKey, parentNode?.Label);
            if (edgeType is null)
            {
                _logger?.LogWarning("Cannot create child node: no create edge found for header type '{HeaderType}' (label: '{Label}')", request.ParentNodeTypeKey, parentNode?.Label);
                return SessionHeaderCreateResult.Failed("No create edge found");
            }

            var targetNodeType = schema.NodeTypes.FirstOrDefault(nt => nt.Id == edgeType.TargetNodeTypeId);
            if (targetNodeType is null)
            {
                _logger?.LogWarning("Cannot create child node: target node type not found for edge '{EdgeKey}'", edgeType.Key);
                return SessionHeaderCreateResult.Failed("Target node type not found");
            }

            var parentTree = await _graphService.GetChildrenAsync(parentId, ct: ct);
            var sortOrder = parentTree.Edges
                .Where(e => e.SourceNodeId == parentId)
                .Select(e => e.SortOrder)
                .DefaultIfEmpty(-1)
                .Max() + 1;

            var child = await _graphService.CreateNodeAsync(new Node
            {
                NodeTypeId = targetNodeType.Id,
                Label = request.Label,
                SortOrder = sortOrder,
            }, ct);

            await _graphService.CreateEdgeAsync(new Edge
            {
                EdgeTypeId = edgeType.Id,
                SourceNodeId = parentId,
                TargetNodeId = child.Id,
                SortOrder = sortOrder,
            }, ct);

            _logger?.LogInformation("Created {NodeType} '{Label}' under header {HeaderId}", targetNodeType.Key, request.Label, parentId);

            // Refresh the context tree so the new node appears
            var userContext = await _graphService.GetUserContextAsync(userId, ct);
            if (userContext is not null)
            {
                await LoadContextNodesAsync(userContext.Id, null, ct);
            }

            return SessionHeaderCreateResult.Succeeded(child.Id, targetNodeType.Key, request.Label);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to create child node for header {HeaderType}", request.ParentNodeTypeKey);
            return SessionHeaderCreateResult.Failed(ex.Message);
        }
    }

    public async Task<SessionHeaderCreateResult> CreateListAsync(string? userContextNodeId, string label, CancellationToken ct = default)
    {
        if (_graphService is null)
            return SessionHeaderCreateResult.Failed("Graph service not available");

        if (!Guid.TryParse(userContextNodeId, out var userContextId))
        {
            _logger?.LogWarning("Cannot create list: user context id '{UserContextId}' is not a valid GUID", userContextNodeId);
            return SessionHeaderCreateResult.Failed("Invalid user context node id");
        }

        try
        {
            var schema = await _graphService.GetSchemaAsync(ct);
            var listsNodeType = schema.NodeTypes.FirstOrDefault(nt => string.Equals(nt.Key, "lists", StringComparison.OrdinalIgnoreCase));
            var listNodeType = schema.NodeTypes.FirstOrDefault(nt => string.Equals(nt.Key, "list", StringComparison.OrdinalIgnoreCase));
            var hasListsEdgeType = schema.EdgeTypes.FirstOrDefault(e => string.Equals(e.Key, "has.lists", StringComparison.OrdinalIgnoreCase));
            var hasListEdgeType = schema.EdgeTypes.FirstOrDefault(e => string.Equals(e.Key, "has.list", StringComparison.OrdinalIgnoreCase));

            if (listsNodeType is null || listNodeType is null || hasListsEdgeType is null || hasListEdgeType is null)
            {
                _logger?.LogWarning("Cannot create list: missing list schema types");
                return SessionHeaderCreateResult.Failed("Missing list schema types");
            }

            var childrenTree = await _graphService.GetChildrenAsync(userContextId, ct: ct);

            var listsHeader = childrenTree.Nodes
                .Where(n => n.Id != userContextId)
                .FirstOrDefault(n =>
                    string.Equals(n.NodeType?.Key, "lists", StringComparison.OrdinalIgnoreCase)
                    || childrenTree.Edges.Any(e =>
                        e.SourceNodeId == userContextId
                        && e.TargetNodeId == n.Id
                        && e.EdgeTypeId == hasListsEdgeType.Id));

            if (listsHeader is null)
            {
                var sortOrder = childrenTree.Edges
                    .Where(e => e.SourceNodeId == userContextId)
                    .Select(e => e.SortOrder)
                    .DefaultIfEmpty(-1)
                    .Max() + 1;

                listsHeader = await _graphService.CreateNodeAsync(new Node
                {
                    NodeTypeId = listsNodeType.Id,
                    Label = "Lists",
                    SortOrder = sortOrder,
                }, ct);

                await _graphService.CreateEdgeAsync(new Edge
                {
                    EdgeTypeId = hasListsEdgeType.Id,
                    SourceNodeId = userContextId,
                    TargetNodeId = listsHeader.Id,
                    SortOrder = sortOrder,
                }, ct);
            }

            var listSortOrder = childrenTree.Edges
                .Where(e => e.SourceNodeId == listsHeader.Id)
                .Select(e => e.SortOrder)
                .DefaultIfEmpty(-1)
                .Max() + 1;

            var listNode = await _graphService.CreateNodeAsync(new Node
            {
                NodeTypeId = listNodeType.Id,
                Label = label,
                SortOrder = listSortOrder,
            }, ct);

            await _graphService.CreateEdgeAsync(new Edge
            {
                EdgeTypeId = hasListEdgeType.Id,
                SourceNodeId = listsHeader.Id,
                TargetNodeId = listNode.Id,
                SortOrder = listSortOrder,
            }, ct);

            _logger?.LogInformation("Created list '{Label}' under Lists header {ListsHeaderId} for user context {UserContextId}", label, listsHeader.Id, userContextId);

            await LoadContextNodesAsync(userContextId, null, ct);
            await LoadChildrenAsync(new SessionNodeInput(listsHeader.Id.ToString(), "lists", listsHeader.Label ?? "Lists"), ct);

            return SessionHeaderCreateResult.Succeeded(listNode.Id, "list", label);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to create list '{Label}' for user context {UserContextId}", label, userContextNodeId);
            return SessionHeaderCreateResult.Failed(ex.Message);
        }
    }

    public IEnumerable<SessionNodeInput> GetAllLists()
        => GetAllLists(ContextNodes);

    private static IEnumerable<SessionNodeInput> GetAllLists(IReadOnlyList<SessionContextNode> nodes)
    {
        foreach (var node in nodes)
        {
            if (string.Equals(node.Node.NodeTypeKey, "list", StringComparison.OrdinalIgnoreCase))
                yield return node.Node;

            if (node.Children is { Count: > 0 })
            {
                foreach (var list in GetAllLists(node.Children))
                    yield return list;
            }
        }
    }

    public async Task<IReadOnlySet<string>> GetListIdsForNodeAsync(string nodeId, CancellationToken ct = default)
    {
        var result = new HashSet<string>(StringComparer.Ordinal);
        if (_graphService is null || !Guid.TryParse(nodeId, out var nodeIdGuid))
            return result;

        var schema = await _graphService.GetSchemaAsync(ct);
        var edgeType = schema.EdgeTypes.FirstOrDefault(e =>
            string.Equals(e.Key, "list.contains", StringComparison.OrdinalIgnoreCase));
        if (edgeType is null)
            return result;

        foreach (var list in GetAllLists())
        {
            if (!Guid.TryParse(list.Id, out var listIdGuid))
                continue;

            var childrenTree = await _graphService.GetChildrenAsync(listIdGuid, ct: ct);
            if (childrenTree.Edges.Any(e =>
                e.EdgeTypeId == edgeType.Id &&
                e.SourceNodeId == listIdGuid &&
                e.TargetNodeId == nodeIdGuid))
            {
                result.Add(list.Id);
            }
        }

        return result;
    }

    public async Task ToggleListMembershipAsync(string nodeId, string listId, bool isMember, CancellationToken ct = default)
    {
        if (_graphService is null)
        {
            _logger?.LogWarning("Cannot toggle list membership: graph service not available");
            return;
        }

        if (!Guid.TryParse(nodeId, out var nodeIdGuid))
        {
            _logger?.LogWarning("Cannot toggle list membership: node id '{NodeId}' is not a valid GUID", nodeId);
            return;
        }

        if (!Guid.TryParse(listId, out var listIdGuid))
        {
            _logger?.LogWarning("Cannot toggle list membership: list id '{ListId}' is not a valid GUID", listId);
            return;
        }

        try
        {
            var schema = await _graphService.GetSchemaAsync(ct);
            var edgeType = schema.EdgeTypes.FirstOrDefault(e =>
                string.Equals(e.Key, "list.contains", StringComparison.OrdinalIgnoreCase));
            if (edgeType is null)
            {
                _logger?.LogWarning("Cannot toggle list membership: edge type 'list.contains' not found");
                return;
            }

            if (isMember)
            {
                await _graphService.CreateEdgeAsync(new Edge
                {
                    EdgeTypeId = edgeType.Id,
                    SourceNodeId = listIdGuid,
                    TargetNodeId = nodeIdGuid,
                    SortOrder = 0,
                }, ct);

                _logger?.LogInformation("Added node {NodeId} to list {ListId}", nodeId, listId);
            }
            else
            {
                var childrenTree = await _graphService.GetChildrenAsync(listIdGuid, ct: ct);
                var edge = childrenTree.Edges.FirstOrDefault(e =>
                    e.EdgeTypeId == edgeType.Id &&
                    e.SourceNodeId == listIdGuid &&
                    e.TargetNodeId == nodeIdGuid);
                if (edge is not null)
                {
                    await _graphService.DeleteEdgeAsync(edge.Id, ct);
                    _logger?.LogInformation("Removed node {NodeId} from list {ListId}", nodeId, listId);
                }
            }

            await LoadChildrenAsync(new SessionNodeInput(listId, "list", "List"), ct);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to toggle list membership for node {NodeId} in list {ListId}", nodeId, listId);
        }
    }

    public async Task<SessionNodeInput?> GetOrCreateNavigationHeaderAsync(
        string parentNodeId,
        string headerLabel,
        string headerNodeTypeKey,
        CancellationToken ct = default)
    {
        if (_graphService is null)
            return null;

        if (!Guid.TryParse(parentNodeId, out var parentId))
        {
            _logger?.LogWarning("Cannot ensure navigation header: parent id '{ParentId}' is not a valid GUID", parentNodeId);
            return null;
        }

        try
        {
            var parentNode = await _graphService.GetNodeAsync(parentId, ct);
            if (parentNode is null)
            {
                _logger?.LogWarning("Cannot ensure navigation header: parent node {ParentId} not found", parentId);
                return null;
            }

            var parentNodeTypeKey = parentNode.NodeType?.Key;
            if (string.IsNullOrWhiteSpace(parentNodeTypeKey))
            {
                _logger?.LogWarning("Cannot ensure navigation header: parent node {ParentId} has no type", parentId);
                return null;
            }

            var schema = await _graphService.GetSchemaAsync(ct);
            var headerNodeType = schema.NodeTypes.FirstOrDefault(nt =>
                string.Equals(nt.Key, headerNodeTypeKey, StringComparison.OrdinalIgnoreCase));
            if (headerNodeType is null)
            {
                _logger?.LogWarning("Cannot ensure navigation header: header node type '{HeaderType}' not found", headerNodeTypeKey);
                return null;
            }

            var parentToHeaderEdgeKey = NavigationHeaderConventions.ParentToHeaderEdgeKey(parentNodeTypeKey, headerLabel);
            var parentToHeaderEdgeType = schema.EdgeTypes.FirstOrDefault(e =>
                string.Equals(e.Key, parentToHeaderEdgeKey, StringComparison.OrdinalIgnoreCase));
            if (parentToHeaderEdgeType is null)
            {
                _logger?.LogWarning("Cannot ensure navigation header: edge type '{EdgeKey}' not found", parentToHeaderEdgeKey);
                return null;
            }

            var childrenTree = await _graphService.GetChildrenAsync(parentId, ct: ct);
            var header = childrenTree.Nodes
                .Where(n => n.Id != parentId)
                .FirstOrDefault(n =>
                    string.Equals(n.NodeType?.Key, headerNodeTypeKey, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(n.Label, headerLabel, StringComparison.Ordinal)
                    && childrenTree.Edges.Any(e =>
                        e.SourceNodeId == parentId
                        && e.TargetNodeId == n.Id
                        && e.EdgeTypeId == parentToHeaderEdgeType.Id));

            if (header is not null)
            {
                return new SessionNodeInput(
                    header.Id.ToString(),
                    headerNodeTypeKey,
                    header.Label ?? headerLabel,
                    "NodeBlock");
            }

            var sortOrder = childrenTree.Edges
                .Where(e => e.SourceNodeId == parentId)
                .Select(e => e.SortOrder)
                .DefaultIfEmpty(-1)
                .Max() + 1;

            var createdHeader = await _graphService.CreateNodeAsync(new Node
            {
                NodeTypeId = headerNodeType.Id,
                Label = headerLabel,
                SortOrder = sortOrder,
            }, ct);

            await _graphService.CreateEdgeAsync(new Edge
            {
                EdgeTypeId = parentToHeaderEdgeType.Id,
                SourceNodeId = parentId,
                TargetNodeId = createdHeader.Id,
                SortOrder = sortOrder,
            }, ct);

            _logger?.LogInformation("Created navigation header '{HeaderLabel}' under {ParentNodeType} {ParentId}", headerLabel, parentNodeTypeKey, parentId);

            return new SessionNodeInput(
                createdHeader.Id.ToString(),
                headerNodeTypeKey,
                createdHeader.Label ?? headerLabel,
                "NodeBlock");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to ensure navigation header '{HeaderLabel}' under parent {ParentNodeId}", headerLabel, parentNodeId);
            return null;
        }
    }

    public async Task LoadSessionCommandsAsync(string sessionId, string? sessionLabel, CancellationToken ct = default)
    {
        if (!Guid.TryParse(sessionId, out var nodeId))
        {
            _logger?.LogWarning("Cannot load session commands: session id '{SessionId}' is not a valid GUID", sessionId);
            return;
        }

        if (_graphService is null) return;

        try
        {
            var node = await _graphService.GetNodeAsync(nodeId, ct);
            if (node is null)
            {
                _logger?.LogWarning("Cannot load session commands: node {NodeId} not found", nodeId);
                return;
            }

            var json = node.Properties?.FirstOrDefault(p => p.Key == "sessionCommands")?.Value;
            if (string.IsNullOrWhiteSpace(json))
            {
                _logger?.LogInformation("No session commands found for {SessionId}", sessionId);
                return;
            }

            var snapshots = JsonSerializer.Deserialize<List<SessionCommandSnapshot>>(json);
            if (snapshots is null || snapshots.Count == 0)
            {
                _logger?.LogInformation("Session commands empty for {SessionId}", sessionId);
                return;
            }

            var sessionNode = new SessionNodeInput(sessionId, "session", sessionLabel ?? "Session");
            var commands = snapshots.Select(s => new SessionCommandSubmitted(
                sessionNode,
                s.AgentTypeKey,
                s.Input,
                s.Response)
            {
                Id = s.Id,
                IsStreaming = false
            }).ToList();

            Dispatch(new RestoreSessionCommandHistory(sessionId, commands));
            _logger?.LogInformation("Loaded {Count} commands for session {SessionId}", commands.Count, sessionId);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to load session commands for {SessionId}", sessionId);
        }
    }

    public async Task PersistSessionCommandsAsync(
        SessionBlockState state,
        PersistSessionCommands persist,
        Guid sessionNodeTypeId,
        string? userName,
        CancellationToken ct)
    {
        if (_graphService is null) return;
        if (!Guid.TryParse(persist.SessionId, out var nodeId))
        {
            _logger?.LogWarning("Cannot persist session commands: session id '{SessionId}' is not a valid GUID", persist.SessionId);
            return;
        }

        try
        {
            var node = await _graphService.GetNodeAsync(nodeId, ct);
            var label = state.Ui.SelectedNode?.Label ?? "New Session";

            if (node is null)
            {
                // Client-created session: materialize it in the graph before persisting properties.
                var userCtx = await _graphService.GetUserContextAsync(state.ActiveSessionId ?? string.Empty, ct);
                if (userCtx is null) return;

                var view = await _graphService.GetUserContextViewAsync(userCtx.Id, ct);
                if (view is null) return;

                var newNode = new Node
                {
                    Id = nodeId,
                    NodeTypeId = sessionNodeTypeId,
                    Label = label
                };
                await _graphService.CreateNodeAsync(newNode, ct);

                var edgeTypeId = GuidV5.Create(GuidV5.Namespace, "edgetype:activates:session");
                await _graphService.CreateEdgeAsync(new Edge
                {
                    EdgeTypeId = edgeTypeId,
                    SourceNodeId = view.Header.Id,
                    TargetNodeId = nodeId,
                    SortOrder = view.Sessions.Count
                }, ct);
            }
            else if (!string.Equals(node.Label, label, StringComparison.Ordinal))
            {
                await _graphService.UpdateNodeAsync(new Node { Id = node.Id, Label = label, SortOrder = node.SortOrder }, ct);
            }

            var commands = state.SessionCommandsBySessionId.TryGetValue(persist.SessionId, out var cmds)
                ? cmds
                : [];
            var snapshots = commands.Select(SessionBlockReducer.CreateCommandSnapshot).ToList();
            var json = JsonSerializer.Serialize(snapshots);
            await _graphService.SetPropertyAsync(nodeId, "sessionCommands", json, ct);
            _logger?.LogInformation("Persisted {Count} commands for session {SessionId}", snapshots.Count, persist.SessionId);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to persist session commands for {SessionId}", persist.SessionId);
        }
    }

    #endregion

    #region Edge resolution

    public static EdgeType? ResolveCreateEdge(GraphSchema schema, string parentNodeTypeKey, string? headerLabel = null)
    {
        // Specific header-to-child mappings
        var childTypeKey = parentNodeTypeKey switch
        {
            "workspaces.header" => "workspace",
            "flow.header" => "flow",
            "package.header" => "package",
            "step.header" => "step",
            "workitem.header" => "workitem",
            _ => null
        };

        if (childTypeKey is not null)
        {
            var edge = FindCreateEdgeByChildTypeKey(schema, childTypeKey);
            if (edge is not null) return edge;
        }

        // For generic headers, infer child type from the header label
        if (string.Equals(parentNodeTypeKey, "header", StringComparison.OrdinalIgnoreCase)
            && !string.IsNullOrWhiteSpace(headerLabel))
        {
            childTypeKey = headerLabel switch
            {
                "Data Tables" => "datatable",
                "Relationships" => "relationship",
                "Branches" => "branch",
                "User Stories" => "userstory",
                "Scenarios" => "scenario",
                "Tables" => "table",
                "Pages" => "page",
                "Workflows" => "workflow",
                "Steps" => "step",
                "Flows" => "flow",
                "Templates" => "template",
                "Tools" => "tool",
                "Packages" => "package",
                "Workspaces" => "workspace",
                "WorkItems" => "session",
                "Settings" => "setting",
                _ => null
            };

            if (childTypeKey is not null)
            {
                var edge = FindCreateEdgeByChildTypeKey(schema, childTypeKey);
                if (edge is not null) return edge;
            }
        }

        // Fallback: discover any outgoing create edge from this node type
        var nodeType = schema.NodeTypes.FirstOrDefault(nt =>
            string.Equals(nt.Key, parentNodeTypeKey, StringComparison.OrdinalIgnoreCase));

        if (nodeType is null) return null;

        return schema.EdgeTypes.FirstOrDefault(e =>
            e.SourceNodeTypeId == nodeType.Id
            && e.Properties.Any(p => string.Equals(p.Key, "verb", StringComparison.OrdinalIgnoreCase)
                && string.Equals(p.Value, "create", StringComparison.OrdinalIgnoreCase)));
    }

    public static EdgeType? FindCreateEdgeByChildTypeKey(GraphSchema schema, string childTypeKey)
    {
        var childNodeType = schema.NodeTypes.FirstOrDefault(nt =>
            string.Equals(nt.Key, childTypeKey, StringComparison.OrdinalIgnoreCase));

        return schema.EdgeTypes.FirstOrDefault(e =>
            e.Properties.Any(p => string.Equals(p.Key, "verb", StringComparison.OrdinalIgnoreCase)
                && string.Equals(p.Value, "create", StringComparison.OrdinalIgnoreCase))
            && (e.Key.Contains(childTypeKey, StringComparison.OrdinalIgnoreCase)
                || (childNodeType is not null && e.TargetNodeTypeId == childNodeType.Id)));
    }

    #endregion

    #region Tree manipulation

    public static IReadOnlyList<SessionContextNode> UpdateNodeChildren(
        IReadOnlyList<SessionContextNode> nodes, string targetId, IReadOnlyList<SessionContextNode> newChildren)
    {
        return nodes.Select(n => UpdateNodeChildren(n, targetId, newChildren)).ToList();
    }

    public static SessionContextNode UpdateNodeChildren(
        SessionContextNode node, string targetId, IReadOnlyList<SessionContextNode> newChildren)
    {
        if (node.Node.Id == targetId)
            return new SessionContextNode(node.Node, newChildren, true);

        if (node.Children is null || node.Children.Count == 0)
            return node;

        return new SessionContextNode(
            node.Node,
            node.Children.Select(child => UpdateNodeChildren(child, targetId, newChildren)).ToList(),
            node.IsLoaded);
    }

    #endregion

    public void ShowSessions()
    {
        var activeSessionId = _state.ActiveSessionId;
        PersistCurrentSessionState();
        Dispatch(new SetActiveSession(null));
        Dispatch(new RestoreSessionUiState(SessionUiDefaults.Empty));
        Dispatch(new SetOpenSessionNodes(_state.OpenSessionNodes.Where(node => _state.PinnedNodeIds.Contains(node.Id)).ToList()));
        Dispatch(new SetSessionPresentationState(null, null, null, null, null, null));
        Dispatch(new ShowSessionFeedPage());
        if (!string.IsNullOrWhiteSpace(activeSessionId))
            Dispatch(new SetPendingScrollSection(null));
    }

    public void SelectSession(SessionNodeInput node, IReadOnlyList<SessionListEntry> sessions)
    {
        PersistCurrentSessionState();

        var openNodes = _state.OpenSessionNodes.Where(existing => _state.PinnedNodeIds.Contains(existing.Id)).ToList();
        if (openNodes.All(existing => existing.Id != node.Id))
            openNodes.Add(node);

        Dispatch(new SetActiveSession(node.Id));
        Dispatch(new SetSessionPresentationState(null, null, null, null, null, null));
        Dispatch(new SetOpenSessionNodes(openNodes));
        Dispatch(new RestoreSessionUiState(GetSessionState(node.Id, sessions)));
        Dispatch(new SelectSessionNode(node));
        Dispatch(new ShowSessionFeedPage());
    }

    public void SetInput(string value)
    {
        Dispatch(new SetSessionInput(value));
        PersistCurrentSessionState();
    }

    public void SetAgentType(string agentTypeKey)
    {
        Dispatch(new SelectSessionAgentType(agentTypeKey));
        PersistCurrentSessionState();
    }

    public void ProjectContext(SessionNodeInput node)
    {
        Dispatch(new ProjectSessionContext(node));
        PersistCurrentSessionState();
        MoveFeedItemToEnd(GetFeedItemKey(CurrentSessionId, "block", "projected-context-block"));
        Dispatch(new SetPendingScrollSection("projected-context-block"));
    }

    public void PinContext(SessionNodeInput node)
    {
        Dispatch(new PinSessionContext(new SessionContextProjectionModel(node)));
        ClearPresentationIds("projected-context-block", null);
        PersistCurrentSessionState();
        var blockId = $"context-block:{node.Id}";
        EnsureFeedSequence(GetFeedItemKey(CurrentSessionId, "block", blockId));
        Dispatch(new SetPendingScrollSection(blockId));
    }

    public void UnpinContext(string pinId)
    {
        Dispatch(new UnpinSessionContext(pinId));
        PersistCurrentSessionState();
    }

    public void SetTabPinned(string nodeId, bool isPinned)
    {
        var next = new HashSet<string>(_state.PinnedNodeIds, StringComparer.Ordinal);
        if (isPinned)
            next.Add(nodeId);
        else
            next.Remove(nodeId);

        Dispatch(new SetPinnedSessionNodes(next));
    }

    public void CloseTab(SessionNodeInput node, IReadOnlyList<SessionListEntry> sessions)
    {
        Dispatch(new CloseSessionNode(node.Id));
        var openNodes = _state.OpenSessionNodes.Where(existing => !string.Equals(existing.Id, node.Id, StringComparison.Ordinal)).ToList();
        Dispatch(new SetOpenSessionNodes(openNodes));

        if (!string.Equals(_state.ActiveSessionId, node.Id, StringComparison.Ordinal))
        {
            PersistCurrentSessionState();
            return;
        }

        var nextNode = openNodes.LastOrDefault();
        if (nextNode is null)
        {
            ShowSessions();
            return;
        }

        SelectSession(nextNode, sessions);
    }

    public async Task SubmitAsync(SessionCommandSubmitted command, bool isTest, int testStreamDelayMilliseconds, IGraphDiscoveryService discovery)
    {
        var sessionId = CurrentSessionId;
        Dispatch(new SubmitSessionCommand(command.Node, command.AgentTypeKey, command.Input));
        PersistCurrentSessionState();

        if (isTest)
        {
            var testCommand = command with { Response = string.Empty, IsStreaming = true };
            EnsureFeedSequence(GetFeedItemKey(sessionId, "command", testCommand.Id));
            SetCommandHistory(sessionId, GetSessionCommands(sessionId).Append(testCommand).ToList());
            MoveProjectedBlockToEnd(sessionId);
            Dispatch(new SetPendingScrollSection(GetCommandTurnId(testCommand)));
            await StreamTestResponseAsync(sessionId, testCommand, testStreamDelayMilliseconds);
            await DispatchAsync(new PersistSessionCommands(sessionId), CancellationToken.None);
            return;
        }

        var service = discovery.Get(command.AgentTypeKey);
        if (service is null)
        {
            EnsureFeedSequence(GetFeedItemKey(sessionId, "command", command.Id));
            SetCommandHistory(sessionId, GetSessionCommands(sessionId).Append(command).ToList());
            MoveProjectedBlockToEnd(sessionId);
            Dispatch(new SetPendingScrollSection(GetCommandTurnId(command)));
            await DispatchAsync(new PersistSessionCommands(sessionId), CancellationToken.None);
            return;
        }

        var previousCommands = GetSessionCommands(sessionId)
            .Where(c => !string.IsNullOrWhiteSpace(c.Response))
            .ToList();

        var context = new SessionContext
        {
            SessionId = sessionId,
            Input = command.Input,
            Completion = command.Completion,
            History = previousCommands
                .SelectMany(c => new (string Role, string Content)[]
                {
                    ("user", c.Input),
                    ("agent", c.Response!)
                })
                .ToList()
        };

        // Prefer streaming when the agent supports it. The default interface
        // implementation throws NotSupportedException; catch it at the call site
        // to fall back to ExecuteAsync for agents that haven't opted in.
        IAsyncEnumerable<string>? stream = null;
        try
        {
            stream = service.StreamAsync(context);
        }
        catch (NotSupportedException)
        {
            stream = null;
        }

        if (stream is null)
        {
            try
            {
                await service.ExecuteAsync(context);
                if (!string.IsNullOrWhiteSpace(context.Response))
                    command = command with { Response = context.Response };
            }
            finally
            {
                EnsureFeedSequence(GetFeedItemKey(sessionId, "command", command.Id));
                SetCommandHistory(sessionId, GetSessionCommands(sessionId).Append(command).ToList());
                MoveProjectedBlockToEnd(sessionId);
                Dispatch(new SetPendingScrollSection(GetCommandTurnId(command)));
                await DispatchAsync(new PersistSessionCommands(sessionId), CancellationToken.None);
            }
            return;
        }

        var streamingCommand = command with { Response = string.Empty, IsStreaming = true };
        EnsureFeedSequence(GetFeedItemKey(sessionId, "command", streamingCommand.Id));
        SetCommandHistory(sessionId, GetSessionCommands(sessionId).Append(streamingCommand).ToList());
        MoveProjectedBlockToEnd(sessionId);
        Dispatch(new SetPendingScrollSection(GetCommandTurnId(streamingCommand)));

        var response = string.Empty;
        try
        {
            await foreach (var chunk in stream)
            {
                response += chunk;
                UpdateCommand(sessionId, streamingCommand.Id, existing => existing with { Response = response, IsStreaming = true });
            }
        }
        finally
        {
            UpdateCommand(sessionId, streamingCommand.Id, existing => existing with { Response = response, IsStreaming = false });
            await DispatchAsync(new PersistSessionCommands(sessionId), CancellationToken.None);
        }
    }

    public void UnpinBlock(SessionContextBlockModel block)
    {
        Dispatch(new UnpinSessionContext($"pin:{block.Context.Id}"));
        ClearBlockMode(block);
        ClearPresentationIds(block.Id, null);
        PersistCurrentSessionState();
    }

    public void SetBlockMode(SessionContextBlockModel block, string mode)
    {
        var modeKey = GetBlockModeKey(CurrentSessionId, block);
        if (string.Equals(mode, SessionContextBlock.ModeMaximized, StringComparison.Ordinal))
        {
            Dispatch(new SetBlockModeEntry(modeKey, null));
            Dispatch(new SetSessionPresentationState(block.Id, null, null, null, null, null));
            return;
        }

        if (string.Equals(mode, SessionContextBlock.ModeOverlayPage, StringComparison.Ordinal))
        {
            Dispatch(new SetBlockModeEntry(modeKey, null));
            Dispatch(new SetSessionPresentationState(null, null, block.Id, null, null, null));
            return;
        }

        if (string.Equals(mode, SessionContextBlock.ModeOverlayApp, StringComparison.Ordinal))
        {
            Dispatch(new SetBlockModeEntry(modeKey, null));
            Dispatch(new SetSessionPresentationState(null, null, null, null, block.Id, null));
            return;
        }

        Dispatch(new SetBlockModeEntry(modeKey, string.Equals(mode, SessionContextBlock.ModeMinimized, StringComparison.Ordinal) ? SessionContextBlock.ModeMinimized : null));
        ClearPresentationIds(block.Id, null);
    }

    public string GetCommandMode(SessionCommandSubmitted command) =>
        _state.CommandModesByKey.TryGetValue(command.Id, out var mode)
            ? mode
            : SessionContextBlock.ModeNormal;

    public void SetCommandMode(SessionCommandSubmitted command, string mode)
    {
        if (string.Equals(mode, SessionContextBlock.ModeMaximized, StringComparison.Ordinal))
        {
            Dispatch(new SetSessionCommandMode(command.Id, SessionContextBlock.ModeNormal));
            Dispatch(new SetSessionPresentationState(null, command.Id, null, null, null, null));
            return;
        }

        if (string.Equals(mode, SessionContextBlock.ModeOverlayPage, StringComparison.Ordinal))
        {
            Dispatch(new SetSessionCommandMode(command.Id, SessionContextBlock.ModeNormal));
            Dispatch(new SetSessionPresentationState(null, null, null, command.Id, null, null));
            return;
        }

        if (string.Equals(mode, SessionContextBlock.ModeOverlayApp, StringComparison.Ordinal))
        {
            Dispatch(new SetSessionCommandMode(command.Id, SessionContextBlock.ModeNormal));
            Dispatch(new SetSessionPresentationState(null, null, null, null, null, command.Id));
            return;
        }

        if (string.Equals(mode, SessionContextBlock.ModeMinimized, StringComparison.Ordinal))
            Dispatch(new SetSessionCommandMode(command.Id, SessionContextBlock.ModeMinimized));
        else
            Dispatch(new SetSessionCommandMode(command.Id, null));

        ClearPresentationIds(null, command.Id);
    }

    public void ShowEventLogPage() => Dispatch(new ShowSessionEventLogPage());

    public void ShowSettingsPage() => Dispatch(new ShowSessionSettingsPage());

    public void ShowFeedPage() => Dispatch(new ShowSessionFeedPage());

    public void ClearEventLog() => Dispatch(new ClearSessionEventLog());

    public string GetCurrentEventLogStateJson() => SessionBlockReducer.SerializeEventLogState(_state);

    public string GetInitialEventLogStateJson() => _state.EventLogEntries.FirstOrDefault()?.BeforeStateJson ?? GetCurrentEventLogStateJson();

    public SessionUiState GetViewState(IReadOnlyList<SessionListEntry> sessions)
    {
        var currentNode = GetCurrentSessionNode(sessions);
        return currentNode is null
            ? SessionUiDefaults.Empty with { OpenNodes = _state.OpenSessionNodes }
            : _state.Ui with { SelectedNode = currentNode, OpenNodes = _state.OpenSessionNodes };
    }

    public IReadOnlyList<SessionListEntry> GetDisplaySessions(IReadOnlyList<SessionListEntry> sessions) =>
        [.. sessions.Select(session => session with { Count = GetSessionSectionCount(session.Id) })];

    public IReadOnlyList<SessionSectionItem> GetVisibleSections(IReadOnlyList<SessionListEntry> sessions)
    {
        if (string.IsNullOrWhiteSpace(_state.ActiveSessionId))
            return [];

        var sections = new List<SessionSectionItem>();

        foreach (var item in GetFeedItems(sessions))
        {
            if (item.Kind == SessionFeedItemKind.Response)
            {
                sections.Add(new SessionSectionItem(GetCommandTurnId(item.Command!), CreateResponseNode(item.Command!).Label, "turn"));
            }
            else
            {
                sections.Add(new SessionSectionItem(item.Block!.Id, item.Block.Context.Label, item.Block.Context.NodeTypeKey));
            }
        }

        return sections;
    }

    public IReadOnlyList<SessionFeedItemDescriptor> GetFeedItems(IReadOnlyList<SessionListEntry> sessions)
    {
        var sessionId = CurrentSessionId;
        var viewState = GetViewState(sessions);

        var responses = GetSessionCommands(sessionId).Select(cmd =>
        {
            var key = GetFeedItemKey(sessionId, "command", cmd.Id);
            var sequence = _state.FeedSequenceByItemKey.TryGetValue(key, out var value) ? value : long.MaxValue;
            return new SessionFeedItemDescriptor(key, SessionFeedItemKind.Response, sequence, Command: cmd);
        });

        var blocks = (viewState.ContextBlocks ?? []).Select(block =>
        {
            var key = GetFeedItemKey(sessionId, "block", block.Id);
            var sequence = _state.FeedSequenceByItemKey.TryGetValue(key, out var value) ? value : long.MaxValue;
            return new SessionFeedItemDescriptor(key, SessionFeedItemKind.Block, sequence, Block: block);
        });

        return [.. responses.Concat(blocks).OrderBy(item => item.Sequence).ThenBy(item => item.Key, StringComparer.Ordinal)];
    }

    public int GetSessionSectionCount(string sessionId) => GetSessionTurnSectionCount(sessionId) + GetSessionBlockSectionCount(sessionId);

    public int GetSessionTurnSectionCount(string sessionId) => GetSessionCommands(sessionId).Count;

    public int GetSessionBlockSectionCount(string sessionId)
    {
        var state = string.Equals(_state.ActiveSessionId, sessionId, StringComparison.Ordinal)
            ? _state.Ui
            : _state.SessionStatesBySessionId.TryGetValue(sessionId, out var storedState)
                ? storedState
                : SessionUiDefaults.Empty;

        return state.ContextBlocks?.Count ?? 0;
    }

    public SessionContextBlockModel? GetMaximizedBlock(IReadOnlyList<SessionListEntry> sessions)
    {
        var blockId = _state.MaximizedBlockId;
        if (string.IsNullOrWhiteSpace(blockId))
            return null;

        return (GetViewState(sessions).ContextBlocks ?? []).FirstOrDefault(block => string.Equals(block.Id, blockId, StringComparison.Ordinal));
    }

    public SessionCommandSubmitted? GetMaximizedCommand()
    {
        var commandId = _state.MaximizedCommandId;
        if (string.IsNullOrWhiteSpace(commandId))
            return null;

        return GetSessionCommands(CurrentSessionId).FirstOrDefault(command => string.Equals(command.Id, commandId, StringComparison.Ordinal));
    }

    public SessionContextBlockModel? GetPageOverlayBlock(IReadOnlyList<SessionListEntry> sessions)
    {
        var blockId = _state.PageOverlayBlockId;
        if (string.IsNullOrWhiteSpace(blockId))
            return null;

        return (GetViewState(sessions).ContextBlocks ?? []).FirstOrDefault(block => string.Equals(block.Id, blockId, StringComparison.Ordinal));
    }

    public SessionCommandSubmitted? GetPageOverlayCommand()
    {
        var commandId = _state.PageOverlayCommandId;
        if (string.IsNullOrWhiteSpace(commandId))
            return null;

        return GetSessionCommands(CurrentSessionId).FirstOrDefault(command => string.Equals(command.Id, commandId, StringComparison.Ordinal));
    }

    public SessionContextBlockModel? GetAppOverlayBlock(IReadOnlyList<SessionListEntry> sessions)
    {
        var blockId = _state.AppOverlayBlockId;
        if (string.IsNullOrWhiteSpace(blockId))
            return null;

        return (GetViewState(sessions).ContextBlocks ?? []).FirstOrDefault(block => string.Equals(block.Id, blockId, StringComparison.Ordinal));
    }

    public SessionCommandSubmitted? GetAppOverlayCommand()
    {
        var commandId = _state.AppOverlayCommandId;
        if (string.IsNullOrWhiteSpace(commandId))
            return null;

        return GetSessionCommands(CurrentSessionId).FirstOrDefault(command => string.Equals(command.Id, commandId, StringComparison.Ordinal));
    }

    public SessionResponseProjection GetResponseProjection(SessionCommandSubmitted command)
    {
        var contextCount = GetSessionBlockSectionCount(command.Node.Id);
        return new SessionResponseProjection(
            GetResponseStatus(command),
            GetResponseStatusVariant(command),
            CreateResponseDescription(command, contextCount));
    }

    public static string GetResponseStatus(SessionCommandSubmitted command) => command.IsStreaming ? "Streaming" : command.AgentTypeKey;

    public static string GetResponseStatusVariant(SessionCommandSubmitted command) => command.IsStreaming ? "secondary" : "outline";

    private static string CreateResponseDescription(SessionCommandSubmitted command, int contextCount)
    {
        if (!string.IsNullOrWhiteSpace(command.Response))
            return command.Response;

        if (command.IsStreaming)
            return "Thinking...";

        return $"{command.AgentTypeKey} processed \"{command.Input}\" with {contextCount} context block(s).";
    }

    public SessionNodeInput? GetCurrentSessionNode(IReadOnlyList<SessionListEntry> sessions) =>
        _state.ActiveSessionId is null ? null : FindSessionNode(_state.ActiveSessionId, sessions);

    public string CurrentSessionId => _state.ActiveSessionId ?? "workspaces-session";

    public IReadOnlyList<SessionCommandSubmitted> GetSessionCommands(string sessionId)
    {
        return _state.SessionCommandsBySessionId.TryGetValue(sessionId, out var commands)
            ? commands
            : [];
    }

    public SessionUiState GetSessionState(string sessionId, IReadOnlyList<SessionListEntry> sessions)
    {
        if (_state.SessionStatesBySessionId.TryGetValue(sessionId, out var state))
            return state;

        var sessionNode = FindSessionNode(sessionId, sessions);
        return sessionNode is null
            ? SessionUiDefaults.Empty
            : SessionUiDefaults.Empty with { SelectedNode = sessionNode };
    }

    public long EnsureFeedSequence(string key)
    {
        if (_state.FeedSequenceByItemKey.TryGetValue(key, out var sequence))
            return sequence;

        sequence = _state.NextFeedSequence + 1;
        Dispatch(new SetFeedSequence(key, sequence, sequence));
        return sequence;
    }

    public long MoveFeedItemToEnd(string key)
    {
        var sequence = _state.NextFeedSequence + 1;
        Dispatch(new SetFeedSequence(key, sequence, sequence));
        return sequence;
    }

    public string GetBlockMode(string sessionId, SessionContextBlockModel block)
    {
        if (string.Equals(_state.MaximizedBlockId, block.Id, StringComparison.Ordinal))
            return SessionContextBlock.ModeMaximized;

        if (string.Equals(_state.PageOverlayBlockId, block.Id, StringComparison.Ordinal))
            return SessionContextBlock.ModeOverlayPage;

        if (string.Equals(_state.AppOverlayBlockId, block.Id, StringComparison.Ordinal))
            return SessionContextBlock.ModeOverlayApp;

        return _state.BlockModesByKey.TryGetValue(GetBlockModeKey(sessionId, block), out var mode)
            ? mode
            : SessionContextBlock.ModeNormal;
    }

    public SessionNodeInput GetRequestNode(string sessionId) => _surfaceDiscovery.Find(sessionId).RequestNode;

    public SessionNodeInput GetResponseNode(string sessionId) => _surfaceDiscovery.Find(sessionId).ResponseNode;

    public static SessionNodeInput CreateResponseNode(SessionCommandSubmitted command) =>
        new($"response:{command.Id}", "session-response", $"{command.AgentTypeKey} response");

    public static string GetCommandTurnId(SessionCommandSubmitted command) => $"session-turn-response:{command.Id}";

    public static string GetFeedItemKey(string sessionId, string itemKind, string itemId) =>
        $"{sessionId}:{itemKind}:{itemId}";

    public static string GetBlockModeKey(string sessionId, SessionContextBlockModel block) =>
        $"{sessionId}:block-mode:{block.Id}";

    private void SetCommandHistory(string sessionId, IReadOnlyList<SessionCommandSubmitted> commands)
    {
        Dispatch(new SetSessionCommandHistory(sessionId, commands));
    }

    private async Task StreamTestResponseAsync(string sessionId, SessionCommandSubmitted command, int delay)
    {
        var response = string.Empty;

        await foreach (var chunk in DummyAskAgentGraphService.StreamReplyAsync(command.Input, delay))
        {
            response += chunk;
            UpdateCommand(sessionId, command.Id, existing => existing with { Response = response, IsStreaming = true });
        }

        UpdateCommand(sessionId, command.Id, existing => existing with { Response = response, IsStreaming = false });
    }

    private void UpdateCommand(string sessionId, string commandId, Func<SessionCommandSubmitted, SessionCommandSubmitted> update)
    {
        var commands = GetSessionCommands(sessionId).ToList();
        var index = commands.FindIndex(command => string.Equals(command.Id, commandId, StringComparison.Ordinal));
        if (index < 0)
            return;

        commands[index] = update(commands[index]);
        SetCommandHistory(sessionId, commands);
    }

    private void PersistCurrentSessionState()
    {
        if (_state.ActiveSessionId is null)
            return;

        Dispatch(new SetStoredSessionState(_state.ActiveSessionId, StripNavigationState(_state.Ui)));
    }

    private SessionUiState StripNavigationState(SessionUiState state)
    {
        var currentSessionNode = _state.ActiveSessionId is null ? null : new SessionNodeInput(_state.ActiveSessionId, "session", _state.ActiveSessionId);
        return currentSessionNode is null
            ? state with { OpenNodes = [] }
            : state with { SelectedNode = currentSessionNode, OpenNodes = [] };
    }

    private void MoveProjectedBlockToEnd(string sessionId)
    {
        if (_state.Ui.ContextProjection is null)
            return;

        MoveFeedItemToEnd(GetFeedItemKey(sessionId, "block", "projected-context-block"));
    }

    private void ClearBlockMode(SessionContextBlockModel block)
    {
        Dispatch(new SetBlockModeEntry(GetBlockModeKey(CurrentSessionId, block), null));
    }

    private void ClearPresentationIds(string? blockId, string? commandId)
    {
        var next = new SetSessionPresentationState(
            string.Equals(_state.MaximizedBlockId, blockId, StringComparison.Ordinal) ? null : _state.MaximizedBlockId,
            string.Equals(_state.MaximizedCommandId, commandId, StringComparison.Ordinal) ? null : _state.MaximizedCommandId,
            string.Equals(_state.PageOverlayBlockId, blockId, StringComparison.Ordinal) ? null : _state.PageOverlayBlockId,
            string.Equals(_state.PageOverlayCommandId, commandId, StringComparison.Ordinal) ? null : _state.PageOverlayCommandId,
            string.Equals(_state.AppOverlayBlockId, blockId, StringComparison.Ordinal) ? null : _state.AppOverlayBlockId,
            string.Equals(_state.AppOverlayCommandId, commandId, StringComparison.Ordinal) ? null : _state.AppOverlayCommandId);
        Dispatch(next);
    }

    private static SessionNodeInput? FindSessionNode(string sessionId, IReadOnlyList<SessionListEntry> sessions)
    {
        var entry = sessions.FirstOrDefault(candidate => string.Equals(candidate.Id, sessionId, StringComparison.Ordinal));
        return entry is null ? null : new SessionNodeInput(entry.Id, entry.NodeTypeKey, entry.Label);
    }
}

public sealed record SessionHeaderCreateResult(bool Success, Guid? NodeId, string? NodeTypeKey, string? Label, string? Error)
{
    public static SessionHeaderCreateResult Succeeded(Guid nodeId, string nodeTypeKey, string label)
        => new(true, nodeId, nodeTypeKey, label, null);

    public static SessionHeaderCreateResult Failed(string error)
        => new(false, null, null, null, error);
}
