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
    INodeTypeDescriptor? TargetDescriptor)
{
    /// <summary>
    /// Optional icon name supplied by edge metadata or inferred from the target node type.
    /// </summary>
    public string? Icon { get; init; }

    /// <summary>
    /// True when this create edge is a convenience edge from a non-header parent
    /// to a navigation-header child. Selecting it should project the header first.
    /// </summary>
    public bool IsNavigationHeaderChild { get; init; }

    /// <summary>
    /// The node type key of the intermediate navigation header.
    /// </summary>
    public string? HeaderNodeTypeKey { get; init; }

    /// <summary>
    /// The display label of the intermediate navigation header.
    /// </summary>
    public string? HeaderLabel { get; init; }

    /// <summary>
    /// True when the edge comes from descriptor/registry metadata rather than user-created settings.
    /// </summary>
    public bool IsStatic { get; init; } = true;
}

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

public static class EdgeTypeServiceExtensions
{
    public static IReadOnlyList<EdgeTypeInfo> GetCreateEdges(
        this IEdgeTypeService service,
        SessionNodeInput sourceNode)
    {
        var createEdges = service.GetOutgoingEdges(sourceNode.NodeTypeKey)
            .Where(edge => string.Equals(edge.Intent, "create", StringComparison.OrdinalIgnoreCase));

        if (!IsGenericHeaderNode(sourceNode.NodeTypeKey))
            return createEdges.ToList();

        var targetNodeTypeKey = ResolveHeaderTargetNodeTypeKey(sourceNode);
        if (string.IsNullOrWhiteSpace(targetNodeTypeKey))
            return [];

        return createEdges
            .Where(edge => string.Equals(
                edge.TargetNodeTypeKey,
                targetNodeTypeKey,
                StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public static EdgeTypeInfo? FindEdgeByAgentTypeKey(
        this IEdgeTypeService service,
        SessionNodeInput sourceNode,
        string agentTypeKey)
    {
        if (string.Equals(agentTypeKey, "create", StringComparison.OrdinalIgnoreCase)
            && IsGenericHeaderNode(sourceNode.NodeTypeKey))
        {
            return service.GetCreateEdges(sourceNode).FirstOrDefault();
        }

        return service.FindEdgeByAgentTypeKey(sourceNode.NodeTypeKey, agentTypeKey);
    }

    private static bool IsGenericHeaderNode(string nodeTypeKey) =>
        string.Equals(nodeTypeKey, "header", StringComparison.OrdinalIgnoreCase);

    private static string? ResolveHeaderTargetNodeTypeKey(SessionNodeInput sourceNode)
    {
        if (sourceNode.Properties?.GetValueOrDefault("createTargetNodeTypeKey") is string targetNodeTypeKey
            && !string.IsNullOrWhiteSpace(targetNodeTypeKey))
            return targetNodeTypeKey;

        return ResolveHeaderTargetNodeTypeKey(sourceNode.Label);
    }

    private static string? ResolveHeaderTargetNodeTypeKey(string headerLabel) => headerLabel.Trim().ToLowerInvariant() switch
    {
        "data tables" => "datatable",
        "relationships" => "relationship",
        "branches" => "branch",
        "user stories" => "userstory",
        "scenarios" => "scenario",
        "tables" => "table",
        "columns" => "column",
        "pages" => "page",
        "workflows" => "workflow",
        "steps" => "step",
        "flows" => "flow",
        "templates" => "template",
        "tools" => "tool",
        "packages" => "package",
        "workspaces" => "workspace",
        "workitems" => "session",
        "settings" => "setting",
        _ => null
    };
}

/// <summary>
/// Default implementation of <see cref="IEdgeTypeService"/> that collects all
/// edge and node descriptors from the DI container.
/// </summary>
public sealed class EdgeTypeService : IEdgeTypeService
{
    private readonly IReadOnlyDictionary<Guid, INodeTypeDescriptor> _nodeTypesById;
    private readonly IReadOnlyDictionary<string, INodeTypeDescriptor> _nodeTypesByKey;
    private readonly IReadOnlyList<IEdgeTypeDescriptor> _edgeTypes;
    private readonly ConcurrentDictionary<string, IReadOnlyList<EdgeTypeInfo>> _outgoingEdgesBySource = new(StringComparer.OrdinalIgnoreCase);

    public EdgeTypeService(IEnumerable<INodeTypeDescriptor> nodeTypes, IEnumerable<IEdgeTypeDescriptor> edgeTypes)
    {
        var nodeTypeList = nodeTypes.ToList();
        _nodeTypesById = nodeTypeList
            .GroupBy(n => n.Id)
            .ToDictionary(g => g.Key, g => g.First());
        _nodeTypesByKey = _nodeTypesById.Values
            .GroupBy(n => n.Key, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
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
        if (string.IsNullOrWhiteSpace(sourceNodeTypeKey))
            return [];

        return _outgoingEdgesBySource.GetOrAdd(sourceNodeTypeKey, BuildOutgoingEdges);
    }

    private IReadOnlyList<EdgeTypeInfo> BuildOutgoingEdges(string sourceNodeTypeKey)
    {
        var sourceNodeType = _nodeTypesByKey.GetValueOrDefault(sourceNodeTypeKey);

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
        var genericHeaderNodeType = _nodeTypesById.Values
            .FirstOrDefault(n => string.Equals(n.Key, "header", StringComparison.OrdinalIgnoreCase));

        var sourceIsGenericHeader = genericHeaderNodeType is not null
            && string.Equals(sourceNodeType.Key, genericHeaderNodeType.Key, StringComparison.OrdinalIgnoreCase);

        // The generic header node type exposes the canonical header -> child create edges
        // derived from every parent node type's NavigationHeaders.
        if (sourceIsGenericHeader)
        {
            foreach (var parentNodeType in _nodeTypesById.Values)
            {
                foreach (var header in parentNodeType.NavigationHeaders ?? [])
                {
                    var childNodeType = _nodeTypesByKey.GetValueOrDefault(header.ChildNodeTypeKey);

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
        }

        // Non-header parents also get a convenience create edge that projects the
        // navigation header first, so users can create header children from the root.
        var headers = sourceNodeType.NavigationHeaders;
        if (headers is null || headers.Count == 0)
            yield break;

        var sourceIsHeaderLike = sourceNodeType.IsHeader
            || string.Equals(sourceNodeType.Key, "header", StringComparison.OrdinalIgnoreCase)
            || sourceNodeType.Key.EndsWith(".header", StringComparison.OrdinalIgnoreCase);

        if (sourceIsHeaderLike)
            yield break;

        foreach (var header in headers)
        {
            var childNodeType = _nodeTypesByKey.GetValueOrDefault(header.ChildNodeTypeKey);

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
                childNodeType)
            {
                IsNavigationHeaderChild = true,
                HeaderNodeTypeKey = genericHeaderNodeType?.Key ?? "header",
                HeaderLabel = header.HeaderLabel,
            };
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
        var label = edge.DefaultProperties.TryGetValue("ui.label", out var uiLabel)
            ? uiLabel
            : edge.DefaultProperties.TryGetValue("label", out var metadataLabel)
                ? metadataLabel
                : edge.Label;
        var icon = edge.DefaultProperties.TryGetValue("ui.icon", out var uiIcon)
            ? uiIcon
            : edge.DefaultProperties.TryGetValue("icon", out var metadataIcon)
                ? metadataIcon
                : targetNodeType?.Key;

        return new EdgeTypeInfo(
            edge.Key,
            edge.Intent ?? string.Empty,
            label,
            edge.DefaultProperties.TryGetValue("ui.surface", out var surface) ? surface : null,
            edge.Tool,
            sourceNodeType.Key,
            sourceNodeType.IsHeader,
            targetNodeType?.Key ?? string.Empty,
            targetNodeType?.EntityType,
            targetNodeType)
        {
            Icon = icon
        };
    }
}
