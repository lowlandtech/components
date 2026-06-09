namespace LowlandTech.Components;

public partial class SessionBlock : IAsyncDisposable
{
    [Inject] private IServiceProvider Services { get; set; } = default!;

    /// <summary>Graph node id used to hydrate pipeline state on mount.</summary>
    [Parameter] public string? SessionId { get; set; }

    /// <summary>Child content rendered inside the session app container.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Callback invoked after all pipeline stages have executed.</summary>
    [Parameter] public EventCallback<PipelineContext> OnPipelineComplete { get; set; }

    private PipelineContext? _context;
    private bool _isHydrated;
    private bool _pipelineComplete;

    /// <summary>True when state was successfully hydrated from the graph layer.</summary>
    public bool IsHydrated => _isHydrated;

    /// <summary>True when the pipeline has finished executing.</summary>
    public bool PipelineComplete => _pipelineComplete;

    /// <summary>The pipeline context created during initialization.</summary>
    public PipelineContext? Context => _context;

    protected override async Task OnInitializedAsync()
    {
        _context = new PipelineContext();

        // Hydrate state from graph layer
        var graphSession = Services.GetService<IGraphSession>();
        if (graphSession is not null
            && !string.IsNullOrWhiteSpace(SessionId)
            && Guid.TryParse(SessionId, out var sessionGuid))
        {
            try
            {
                var node = await graphSession.GetAsync<Node>(sessionGuid);
                if (node is not null)
                {
                    foreach (var prop in node.Properties)
                    {
                        if (!string.IsNullOrEmpty(prop.Key))
                        {
                            _context.State[prop.Key] = prop.Value;
                        }
                    }
                    _isHydrated = true;
                }
            }
            catch
            {
                // Graph session may not have the node type configured;
                // hydration is best-effort.
            }
        }

        // Execute pipeline stages
        var registry = Services.GetService<PipelineRegistry>();
        if (registry is not null)
        {
            foreach (var stage in registry.Stages)
            {
                await stage.ExecuteAsync(_context);
            }
        }

        _pipelineComplete = true;
        await OnPipelineComplete.InvokeAsync(_context);
    }

    public async ValueTask DisposeAsync()
    {
        var registry = Services.GetService<PipelineRegistry>();
        if (registry is not null)
        {
            foreach (var stage in registry.Stages)
            {
                await stage.DisposeAsync();
            }
        }
    }
}
