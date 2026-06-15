namespace LowlandTech.Components.Tests.Infrastructure;

public sealed class FakeGraphService : IGraphService
{
    private readonly Dictionary<Guid, Node> _nodes = [];
    private readonly List<Edge> _edges = [];
    private readonly Dictionary<string, NodeType> _nodeTypes = [];
    private readonly Dictionary<string, EdgeType> _edgeTypes = [];

    public IReadOnlyList<Node> Nodes => _nodes.Values.ToList();
    public IReadOnlyList<Edge> Edges => _edges.ToList();

    public FakeGraphService()
    {
        SeedNodeType("user-context");
        SeedNodeType("lists");
        SeedNodeType("list");
        SeedNodeType("workspaces.header");

        SeedEdgeType("has.lists", "user-context", "lists");
        SeedEdgeType("has.list", "lists", "list");
        SeedEdgeType("list.contains", "list", "list");
    }

    public Node SeedNode(Guid id, string nodeTypeKey, string label, int sortOrder = 0)
    {
        var node = new Node
        {
            Id = id,
            NodeTypeId = _nodeTypes[nodeTypeKey].Id,
            Label = label,
            SortOrder = sortOrder,
            NodeType = _nodeTypes[nodeTypeKey]
        };
        _nodes[id] = node;
        return node;
    }

    public void SeedNodeType(string key)
    {
        if (!_nodeTypes.ContainsKey(key))
            _nodeTypes[key] = new NodeType { Id = Guid.NewGuid(), Key = key };
    }

    public void SeedEdgeType(string key, string sourceNodeTypeKey, string targetNodeTypeKey)
    {
        if (!_edgeTypes.ContainsKey(key))
        {
            var sourceType = _nodeTypes[sourceNodeTypeKey];
            var targetType = _nodeTypes[targetNodeTypeKey];
            _edgeTypes[key] = new EdgeType
            {
                Id = Guid.NewGuid(),
                Key = key,
                SourceNodeTypeId = sourceType.Id,
                SourceNodeType = sourceType,
                TargetNodeTypeId = targetType.Id,
                TargetNodeType = targetType
            };
        }
    }

    public Task<GraphSchema> GetSchemaAsync(CancellationToken ct = default)
        => Task.FromResult(new GraphSchema(
            _nodeTypes.Values.ToList(),
            _edgeTypes.Values.ToList(),
            []));

    public Task<GraphTree> GetTreeAsync(Guid rootNodeId, string? effectiveUserId = null, CancellationToken ct = default)
        => Task.FromResult(new GraphTree(_nodes.Values.ToList(), _edges.ToList()));

    public Task<GraphTree> GetChildrenAsync(Guid parentNodeId, string? effectiveUserId = null, CancellationToken ct = default)
    {
        var parent = _nodes.GetValueOrDefault(parentNodeId);
        var outgoingEdges = _edges.Where(e => e.SourceNodeId == parentNodeId).ToList();
        var childIds = outgoingEdges.Select(e => e.TargetNodeId).ToHashSet();
        var children = _nodes.Values.Where(n => childIds.Contains(n.Id)).ToList();

        var nodes = parent is null ? children : new List<Node>(children) { parent };
        return Task.FromResult(new GraphTree(nodes, outgoingEdges));
    }

    public Task<GraphPath> GetPathAsync(Guid nodeId, string? effectiveUserId = null, CancellationToken ct = default)
        => Task.FromResult(new GraphPath([], [], []));

    public Task<Node?> GetNodeAsync(Guid nodeId, CancellationToken ct = default)
        => Task.FromResult(_nodes.TryGetValue(nodeId, out var node) ? node : null);

    public Task<Node> CreateNodeAsync(Node node, CancellationToken ct = default)
    {
        if (node.NodeType is null && _nodeTypes.Values.FirstOrDefault(nt => nt.Id == node.NodeTypeId) is { } nt)
            node.NodeType = nt;

        _nodes[node.Id] = node;
        return Task.FromResult(node);
    }

    public Task<Node> UpdateNodeAsync(Node node, CancellationToken ct = default)
    {
        _nodes[node.Id] = node;
        return Task.FromResult(node);
    }

    public Task DeleteNodeAsync(Guid nodeId, CancellationToken ct = default)
    {
        _nodes.Remove(nodeId);
        return Task.CompletedTask;
    }

    public Task<Edge> CreateEdgeAsync(Edge edge, CancellationToken ct = default)
    {
        if (edge.EdgeType is null && _edgeTypes.Values.FirstOrDefault(et => et.Id == edge.EdgeTypeId) is { } et)
            edge.EdgeType = et;

        _edges.Add(edge);
        return Task.FromResult(edge);
    }

    public Edge SeedEdge(string key, string edgeTypeKey, Guid sourceNodeId, Guid targetNodeId, int sortOrder = 0)
    {
        var edgeType = _edgeTypes[edgeTypeKey];
        var edge = new Edge
        {
            Id = Guid.NewGuid(),
            EdgeTypeId = edgeType.Id,
            EdgeType = edgeType,
            SourceNodeId = sourceNodeId,
            TargetNodeId = targetNodeId,
            SortOrder = sortOrder
        };
        _edges.Add(edge);
        return edge;
    }

    public Task<Edge> UpdateEdgeAsync(Edge edge, CancellationToken ct = default)
        => Task.FromResult(edge);

    public Task DeleteEdgeAsync(Guid edgeId, CancellationToken ct = default)
    {
        _edges.RemoveAll(e => e.Id == edgeId);
        return Task.CompletedTask;
    }

    public Task SetPropertyAsync(Guid nodeId, string key, string? value, CancellationToken ct = default)
        => Task.CompletedTask;

    public Task RemovePropertyAsync(Guid nodeId, string key, CancellationToken ct = default)
        => Task.CompletedTask;

    public Task<Node?> GetUserContextAsync(string userId, CancellationToken ct = default)
        => Task.FromResult<Node?>(null);

    public Task<Node> EnsureUserContextAsync(string userId, string userName, CancellationToken ct = default)
        => Task.FromResult(new Node { Id = Guid.NewGuid(), Label = userName });

    public Task<UserContextView?> GetUserContextViewAsync(Guid userContextNodeId, CancellationToken ct = default)
        => Task.FromResult<UserContextView?>(null);
}
