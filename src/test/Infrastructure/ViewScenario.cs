namespace LowlandTech.Components.Tests.Infrastructure;

public abstract class ViewScenario : IAsyncLifetime
{
    protected BunitContext Ctx = null!;

    public virtual ValueTask InitializeAsync()
    {
        Ctx = new BunitContext();
        Ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        Ctx.Services.AddSingleton<IComponentDescriptorRegistry, ComponentDescriptorRegistry>();
        Ctx.Services.AddSingleton<IPopoverService, PopoverService>();
        Ctx.Services.AddSingleton<ISonnerService, SonnerService>();
        Ctx.Services.AddSingleton<IPageOverlayService, PageOverlayService>();
        Ctx.Services.AddGraphDescriptors(typeof(INodeTypeDescriptor).Assembly);
        Ctx.Services.AddSingleton<ISessionGraphSchemaQueryService, TestSessionGraphSchemaQueryService>();
        Ctx.Services.AddSingleton<IEdgeTypeService, EdgeTypeService>();
        Ctx.Services.AddSingleton<IExecutionEngine>(sp => new ComponentsExecutionEngine(sp.GetRequiredService<IEdgeTypeService>()) { UserId = "test-user" });
        Ctx.Services.AddSingleton<IGraphToolService, WorkspaceCreatorTool>();
        Ctx.Services.AddSingleton<IGraphToolService, TableCreatorTool>();
        Ctx.Services.AddSingleton<IGraphToolService, WorkflowCreatorTool>();
        Ctx.Services.AddSingleton<IGraphToolService, PageCreatorTool>();
        Ctx.Services.AddSingleton<IGraphToolService, FeatureCreatorTool>();
        Ctx.Services.AddSingleton<IGraphToolService, BranchCreatorTool>();
        Ctx.Services.AddSingleton<IGraphToolService, FlowCreatorTool>();
        Ctx.Services.AddSingleton<IGraphToolService, StepCreatorTool>();
        Ctx.Services.AddSingleton<IGraphToolService, PackageCreatorTool>();
        Ctx.Services.AddSingleton<IGraphToolService, WorkitemCreatorTool>();
        Ctx.Services.AddSingleton<IGraphToolService, UserStoryCreatorTool>();
        Ctx.Services.AddSingleton<IGraphToolService, ScenarioCreatorTool>();
        Ctx.Services.AddSingleton<IGraphToolService, ScenarioStepCreatorTool>();
        Ctx.Services.AddSingleton<IGraphToolService, ActionCreatorTool>();
        Ctx.Services.AddSingleton<IGraphToolService, HandlerCreatorTool>();
        Ctx.Services.AddSingleton<IGraphToolService, EventCreatorTool>();
        Ctx.Services.AddSingleton<IAgentGraphService, AskAgentGraphService>();
        Ctx.Services.AddSingleton<IAgentGraphService, PlanAgentGraphService>();
        Ctx.Services.AddSingleton<IAgentGraphService, EditAgentGraphService>();
        Ctx.Services.AddSingleton<IAgentGraphService, RunAgentGraphService>();
        Ctx.Services.AddSingleton<IAgentGraphService, CreateAgentGraphService>();
        Ctx.Services.AddSingleton<IAgentGraphService, ExecuteAgentGraphService>();
        Ctx.Services.AddSingleton<IAgentGraphRegistry, AgentGraphRegistry>();
        Ctx.Services.AddSingleton<IGraphDiscoveryService, GraphDiscoveryService>();
        Ctx.Services.AddScoped<ISessionSurfaceDiscovery, SessionSurfaceDiscovery>();
        Ctx.Services.AddSingleton<ISessionBlockRegistry, DefaultSessionBlockRegistry>();
        Ctx.Services.AddSingleton<ISessionBlockRegistry, AssemblyScanningSessionBlockRegistry>();
        Ctx.Services.AddSingleton<ISessionBlockDiscovery, SessionBlockDiscovery>();
        Ctx.Services.AddSingleton<IDynamicBlockRegistry, DefaultDynamicBlockRegistry>();
        Ctx.Services.AddSingleton<IDynamicBlockDiscovery, DynamicBlockDiscovery>();
        Ctx.Services.AddSingleton<IDynamicReducerArm, DynamicInitializerReducerArm>();
        Ctx.Services.AddSingleton<IDynamicReducerArm, DynamicFieldReducerArm>();
        Ctx.Services.AddSingleton<IDynamicReducerArm, DynamicEntityMutationReducerArm>();
        Ctx.Services.AddSingleton<IAgentSelectionService, AgentSelectionService>();
        Ctx.Services.AddLocalization(options => options.ResourcesPath = "Resources");
        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        Ctx.Dispose();
        return ValueTask.CompletedTask;
    }
}

internal sealed class TestSessionGraphSchemaQueryService(
    IEnumerable<INodeTypeDescriptor> nodeTypeDescriptors,
    IEnumerable<IEdgeTypeDescriptor> edgeTypeDescriptors) : ISessionGraphSchemaQueryService
{
    private readonly IReadOnlyList<INodeTypeDescriptor> _nodeTypeDescriptors = [.. nodeTypeDescriptors];
    private readonly IReadOnlyList<IEdgeTypeDescriptor> _edgeTypeDescriptors = [.. edgeTypeDescriptors];

    public Task<SessionNodeTypePage> QueryNodeTypesAsync(
        SessionGraphSchemaPageRequest request,
        CancellationToken ct = default)
    {
        request = Normalize(request);
        var query = _nodeTypeDescriptors
            .Select(descriptor => new SessionNodeTypeDefinition(
                descriptor.Id.ToString("N"),
                descriptor.Key,
                descriptor.Label,
                descriptor.Icon,
                descriptor.BlockComponent,
                descriptor.IsHeader,
                descriptor.IsLeaf,
                true))
            .Where(nodeType => Matches(request.Search, nodeType.Key, nodeType.Label, nodeType.BlockComponent))
            .OrderBy(nodeType => nodeType.Key, StringComparer.Ordinal)
            .ToList();

        var items = query
            .Skip(request.PageIndex * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return Task.FromResult(new SessionNodeTypePage(items, query.Count, request.PageIndex, request.PageSize, request.Search));
    }

    public Task<SessionEdgeTypePage> QueryEdgeTypesAsync(
        SessionGraphSchemaPageRequest request,
        CancellationToken ct = default)
    {
        request = Normalize(request);
        var nodeTypesById = _nodeTypeDescriptors
            .GroupBy(descriptor => descriptor.Id)
            .ToDictionary(group => group.Key, group => group.First().Key);
        var query = _edgeTypeDescriptors
            .Select(descriptor => new SessionEdgeTypeDefinition(
                descriptor.Id.ToString("N"),
                descriptor.Key,
                descriptor.Label,
                nodeTypesById.TryGetValue(descriptor.SourceNodeTypeId, out var source) ? source : descriptor.SourceNodeTypeId.ToString("N"),
                nodeTypesById.TryGetValue(descriptor.TargetNodeTypeId, out var target) ? target : descriptor.TargetNodeTypeId.ToString("N"),
                descriptor.Intent,
                descriptor.Tool,
                true))
            .Where(edgeType => Matches(request.Search, edgeType.Key, edgeType.Label, edgeType.SourceNodeTypeKey, edgeType.TargetNodeTypeKey, edgeType.Intent))
            .OrderBy(edgeType => edgeType.Key, StringComparer.Ordinal)
            .ToList();

        var items = query
            .Skip(request.PageIndex * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return Task.FromResult(new SessionEdgeTypePage(items, query.Count, request.PageIndex, request.PageSize, request.Search));
    }

    private static SessionGraphSchemaPageRequest Normalize(SessionGraphSchemaPageRequest request) =>
        new(
            Math.Max(0, request.PageIndex),
            Math.Clamp(request.PageSize, 1, 500),
            request.Search?.Trim() ?? string.Empty);

    private static bool Matches(string search, params string?[] values) =>
        string.IsNullOrWhiteSpace(search)
        || values.Any(value => value?.Contains(search, StringComparison.OrdinalIgnoreCase) == true);
}
