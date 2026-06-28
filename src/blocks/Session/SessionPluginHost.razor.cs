namespace LowlandTech.Components;

public partial class SessionPluginHost
{
    [Inject] private IServiceProvider Services { get; set; } = default!;
    [Inject] private ILogger<SessionPluginHost> Logger { get; set; } = default!;

    private IReadOnlyList<SessionPluginDescriptorRepresentation> _plugins = [];
    private IReadOnlyList<SessionPluginDescriptorRepresentation> _visiblePlugins = [];
    private SessionPluginDescriptorRepresentation? _activePlugin;
    private RenderFragment? _activeRender;
    private SessionPluginContext? _context;
    private bool _isCollapsed;
    private readonly Dictionary<string, string> _renderErrors = new(StringComparer.Ordinal);

    [Parameter] public string? ActiveSessionId { get; set; }
    [Parameter] public IReadOnlyList<SessionListEntry> Sessions { get; set; } = [];
    [Parameter] public IReadOnlyList<SessionContextNode> ContextNodes { get; set; } = [];
    [Parameter] public BlockContext? BlockContext { get; set; }
    [Parameter] public IReadOnlyDictionary<string, object?>? Parameters { get; set; }
    [Parameter] public string ExpandLabel { get; set; } = "Panels";
    [Parameter] public string EmptyLabel { get; set; } = "No panels";
    [Parameter] public string PanelFailedLabel { get; set; } = "Panel failed to render.";
    [Parameter] public EventCallback<string> OnPluginSelected { get; set; }
    [Parameter] public EventCallback<bool> OnCollapsedChanged { get; set; }

    private ISessionPluginRegistry Registry =>
        Services.GetService<ISessionPluginRegistry>()
        ?? new SessionPluginRegistry(
            Services.GetServices<ISessionPlugin>(),
            Services.GetService<ISessionPluginSettingsStore>());

    protected override async Task OnInitializedAsync()
    {
        await LoadAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (_context is not null)
            await LoadAsync();
    }

    private async Task LoadAsync()
    {
        try
        {
            _context = new SessionPluginContext(
                ActiveSessionId,
                Sessions,
                ContextNodes,
                BlockContext ?? Services.GetService<BlockContext>() ?? new BlockContext(),
                Parameters ?? new Dictionary<string, object?>())
            {
                OnReloadRequested = ReloadAsync
            };

            _plugins = await Registry.ListAsync(_context, SessionPluginPlacement.RightSidebar).ConfigureAwait(false);
            _visiblePlugins = _plugins.Where(p => p.IsVisible).ToList();

            _activePlugin = _visiblePlugins.FirstOrDefault(p => p.Key == _activePlugin?.Key)
                ?? _visiblePlugins.FirstOrDefault();

            await RenderActiveAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load session plugins.");
        }
    }

    private async Task RenderActiveAsync()
    {
        _activeRender = null;
        if (_activePlugin is null || _context is null) return;

        var plugin = Registry.GetAllPlugins()
            .FirstOrDefault(p => string.Equals(p.Descriptor.Key, _activePlugin.Key, StringComparison.Ordinal));

        if (plugin is null) return;

        try
        {
            _activeRender = plugin.Render(_context);
            _renderErrors.Remove(_activePlugin.Key);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Session plugin '{PluginKey}' render failed.", _activePlugin.Key);
            _renderErrors[_activePlugin.Key] = ex.Message;
        }

        await OnPluginSelected.InvokeAsync(_activePlugin.Key);
    }

    private async Task SelectAsync(string key)
    {
        var plugin = _visiblePlugins.FirstOrDefault(p => string.Equals(p.Key, key, StringComparison.Ordinal));
        if (plugin is null) return;

        _activePlugin = plugin;
        await RenderActiveAsync();
    }

    private async Task ExpandAsync()
    {
        _isCollapsed = false;
        await OnCollapsedChanged.InvokeAsync(false);
    }

    private async Task ReloadAsync()
    {
        await LoadAsync();
        StateHasChanged();
    }

    private string GetTabClass(SessionPluginDescriptorRepresentation plugin)
        => string.Equals(_activePlugin?.Key, plugin.Key, StringComparison.Ordinal)
            ? "rounded bg-accent font-medium"
            : "rounded hover:bg-accent/50";

    private string SetRenderError(string pluginKey)
    {
        _renderErrors[pluginKey] = PanelFailedLabel;
        return string.Empty;
    }
}

