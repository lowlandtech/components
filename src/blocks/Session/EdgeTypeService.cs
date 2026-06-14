namespace LowlandTech.Components;

/// <summary>
/// Tool that materialises a graph entity from a property bag. Each concrete
/// implementation is keyed by <see cref="ToolKey"/> so the discovery service
/// can locate it from an edge descriptor.
/// </summary>
public interface IGraphToolService : IKeyedGraphService
{
    string ToolKey { get; }

    string IKeyedGraphService.Key => ToolKey;

    Task<object> ExecuteAsync(
        IExecutionEngine engine,
        IDictionary<string, object?> properties,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Describes a single create-oriented edge that leaves a source node type and
/// targets another node type, including the tool that can create the target.
/// </summary>
public sealed record EdgeTypeInfo(
    string EdgeKey,
    string Intent,
    string? Tool,
    string SourceNodeTypeKey,
    bool SourceIsHeader,
    string TargetNodeTypeKey,
    Type? TargetType,
    INodeTypeDescriptor? TargetDescriptor);

/// <summary>
/// Resolves outgoing create edges for a node type by inspecting the registered
/// <see cref="IEdgeTypeDescriptor"/> and <see cref="INodeTypeDescriptor"/> sets.
/// </summary>
public interface IEdgeTypeService
{
    /// <summary>
    /// Finds the first outgoing edge from <paramref name="sourceNodeTypeKey"/> whose
    /// <see cref="IEdgeTypeDescriptor.Intent"/> equals "create".
    /// </summary>
    EdgeTypeInfo? FindCreateEdge(string sourceNodeTypeKey);

    /// <summary>
    /// Returns all outgoing edges from <paramref name="sourceNodeTypeKey"/>.
    /// </summary>
    IReadOnlyList<EdgeTypeInfo> GetOutgoingEdges(string sourceNodeTypeKey);

    /// <summary>
    /// Resolves the outgoing edge that corresponds to the supplied agent type key.
    /// The default <c>create</c> agent maps to the first outgoing edge whose intent
    /// is <c>create</c>; other keys can be matched by intent for dynamic command-bar
    /// modes discovered from the graph schema.
    /// </summary>
    EdgeTypeInfo? FindEdgeByAgentTypeKey(string sourceNodeTypeKey, string agentTypeKey);
}

/// <summary>
/// Default implementation of <see cref="IEdgeTypeService"/> that collects all
/// edge and node descriptors from the DI container.
/// </summary>
public sealed class EdgeTypeService : IEdgeTypeService
{
    private readonly IReadOnlyDictionary<Guid, INodeTypeDescriptor> _nodeTypesById;
    private readonly IReadOnlyList<IEdgeTypeDescriptor> _edgeTypes;

    public EdgeTypeService(IEnumerable<INodeTypeDescriptor> nodeTypes, IEnumerable<IEdgeTypeDescriptor> edgeTypes)
    {
        _nodeTypesById = nodeTypes
            .GroupBy(n => n.Id)
            .ToDictionary(g => g.Key, g => g.First());
        _edgeTypes = edgeTypes.ToList();
    }

    public EdgeTypeInfo? FindCreateEdge(string sourceNodeTypeKey)
    {
        return GetOutgoingEdges(sourceNodeTypeKey)
            .FirstOrDefault(edge => string.Equals(edge.Intent, "create", StringComparison.OrdinalIgnoreCase));
    }

    public EdgeTypeInfo? FindEdgeByAgentTypeKey(string sourceNodeTypeKey, string agentTypeKey)
    {
        if (string.IsNullOrWhiteSpace(agentTypeKey))
            return null;

        // The default Create agent is the canonical surface for any create intent.
        if (string.Equals(agentTypeKey, "create", StringComparison.OrdinalIgnoreCase))
            return FindCreateEdge(sourceNodeTypeKey);

        return GetOutgoingEdges(sourceNodeTypeKey)
            .FirstOrDefault(edge =>
                !string.IsNullOrWhiteSpace(edge.Intent)
                && string.Equals(edge.Intent, agentTypeKey, StringComparison.OrdinalIgnoreCase));
    }

    public IReadOnlyList<EdgeTypeInfo> GetOutgoingEdges(string sourceNodeTypeKey)
    {
        var sourceNodeType = _nodeTypesById.Values
            .FirstOrDefault(n => string.Equals(n.Key, sourceNodeTypeKey, StringComparison.OrdinalIgnoreCase));

        if (sourceNodeType is null)
            return [];

        return _edgeTypes
            .Where(edge => edge.SourceNodeTypeId == sourceNodeType.Id)
            .Select(edge => Map(edge, sourceNodeType))
            .ToList();
    }

    private EdgeTypeInfo Map(IEdgeTypeDescriptor edge, INodeTypeDescriptor sourceNodeType)
    {
        var targetNodeType = _nodeTypesById.TryGetValue(edge.TargetNodeTypeId, out var target) ? target : null;

        return new EdgeTypeInfo(
            edge.Key,
            edge.Intent ?? string.Empty,
            edge.Tool,
            sourceNodeType.Key,
            sourceNodeType.IsHeader,
            targetNodeType?.Key ?? string.Empty,
            targetNodeType?.EntityType,
            targetNodeType);
    }
}
