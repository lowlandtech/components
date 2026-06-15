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
    string Label,
    string? Surface,
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

        var edges = GetOutgoingEdges(sourceNodeTypeKey);

        // Dynamic create options encode the selected edge as "intent:edgeKey"
        // (e.g. "create:has.workspace") so a source with multiple create edges
        // can create the exact target chosen from the context menu.
        var colonIndex = agentTypeKey.IndexOf(':');
        if (colonIndex > 0)
        {
            var intent = agentTypeKey[..colonIndex];
            var edgeKey = agentTypeKey[(colonIndex + 1)..];
            return edges.FirstOrDefault(edge =>
                !string.IsNullOrWhiteSpace(edge.Intent)
                && string.Equals(edge.Intent, intent, StringComparison.OrdinalIgnoreCase)
                && string.Equals(edge.EdgeKey, edgeKey, StringComparison.OrdinalIgnoreCase));
        }

        // The default Create agent is the canonical surface for any create intent.
        if (string.Equals(agentTypeKey, "create", StringComparison.OrdinalIgnoreCase))
            return edges.FirstOrDefault(edge => string.Equals(edge.Intent, "create", StringComparison.OrdinalIgnoreCase));

        return edges.FirstOrDefault(edge =>
            !string.IsNullOrWhiteSpace(edge.Intent)
            && string.Equals(edge.Intent, agentTypeKey, StringComparison.OrdinalIgnoreCase));
    }

    public IReadOnlyList<EdgeTypeInfo> GetOutgoingEdges(string sourceNodeTypeKey)
    {
        var sourceNodeType = _nodeTypesById.Values
            .FirstOrDefault(n => string.Equals(n.Key, sourceNodeTypeKey, StringComparison.OrdinalIgnoreCase));

        if (sourceNodeType is null)
            return [];

        var explicitEdges = _edgeTypes
            .Where(edge => edge.SourceNodeTypeId == sourceNodeType.Id)
            .Select(edge => Map(edge, sourceNodeType))
            .ToList();

        var explicitKeys = explicitEdges
            .Select(e => e.EdgeKey)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var headerCreateEdges = GetNavigationHeaderCreateEdges(sourceNodeType)
            .Where(edge => !explicitKeys.Contains(edge.EdgeKey))
            .ToList();

        return [.. explicitEdges, .. headerCreateEdges];
    }

    private IEnumerable<EdgeTypeInfo> GetNavigationHeaderCreateEdges(INodeTypeDescriptor sourceNodeType)
    {
        foreach (var header in sourceNodeType.NavigationHeaders ?? [])
        {
            var childNodeType = _nodeTypesById.Values
                .FirstOrDefault(n => string.Equals(n.Key, header.ChildNodeTypeKey, StringComparison.OrdinalIgnoreCase));

            if (childNodeType is null)
                continue;

            var edgeKey = header.HeaderToChildEdgeKey
                ?? NavigationHeaderConventions.HeaderToChildEdgeKey(header.ChildNodeTypeKey);

            yield return new EdgeTypeInfo(
                edgeKey,
                "create",
                childNodeType.Label,
                null,
                DeriveCreatorToolKey(childNodeType.Key),
                sourceNodeType.Key,
                sourceNodeType.IsHeader,
                childNodeType.Key,
                childNodeType.EntityType,
                childNodeType);
        }
    }

    private static string DeriveCreatorToolKey(string nodeTypeKey)
    {
        if (string.IsNullOrWhiteSpace(nodeTypeKey))
            return string.Empty;

        var segments = nodeTypeKey.Split('-', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length == 0)
            return string.Empty;

        var builder = new System.Text.StringBuilder();
        foreach (var segment in segments)
        {
            builder.Append(char.ToUpperInvariant(segment[0]));
            if (segment.Length > 1)
                builder.Append(segment.AsSpan(1));
        }

        builder.Append("Creator");
        return builder.ToString();
    }

    private EdgeTypeInfo Map(IEdgeTypeDescriptor edge, INodeTypeDescriptor sourceNodeType)
    {
        var targetNodeType = _nodeTypesById.TryGetValue(edge.TargetNodeTypeId, out var target) ? target : null;

        return new EdgeTypeInfo(
            edge.Key,
            edge.Intent ?? string.Empty,
            edge.DefaultProperties.TryGetValue("ui.label", out var label) ? label : edge.Label,
            edge.DefaultProperties.TryGetValue("ui.surface", out var surface) ? surface : null,
            edge.Tool,
            sourceNodeType.Key,
            sourceNodeType.IsHeader,
            targetNodeType?.Key ?? string.Empty,
            targetNodeType?.EntityType,
            targetNodeType);
    }
}
