namespace LowlandTech.Components;

public partial class SidebarProvider : TinyComponentBase, IAsyncDisposable
{
    [Parameter] public bool DefaultOpen { get; set; } = true;
    [Parameter] public bool? Open { get; set; }
    [Parameter] public EventCallback<bool> OpenChanged { get; set; }
    [Parameter] public string Width { get; set; } = "16rem";
    [Parameter] public string MobileWidth { get; set; } = "18rem";
    [Parameter] public bool IsResizable { get; set; }
    [Parameter] public int SidebarPanelDefaultSize { get; set; } = 30;
    [Parameter] public int SidebarPanelMinSize { get; set; } = 16;
    [Parameter] public int SidebarPanelMaxSize { get; set; } = 45;
    [Parameter] public int InsetPanelDefaultSize { get; set; } = 70;
    [Parameter] public int InsetPanelMinSize { get; set; } = 55;
    [Parameter] public bool RevealHandleOnHover { get; set; } = true;
    [Parameter] public int MobileBreakpoint { get; set; } = 768;
    [Parameter] public RenderFragment? Inset { get; set; }

    [Inject] private IJSRuntime JS { get; set; } = default!;

    private SidebarState _state = new();
    private IJSObjectReference? _module;
    private bool _jsInitialized;

    protected override void OnInitialized()
    {
        _state.Open = Open ?? DefaultOpen;
        _state.OnChange += OnStateChanged;
    }

    protected override void OnParametersSet()
    {
        if (Open.HasValue && Open.Value != _state.Open)
        {
            _state.Open = Open.Value;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !_jsInitialized)
        {
            _module = await JS.InvokeAsync<IJSObjectReference>(
                "import", "./_content/LowlandTech.Components/js/sidebar.js");

            await _module.InvokeVoidAsync("initialize", DotNetObjectReference.Create(this), MobileBreakpoint);
            _jsInitialized = true;
        }
    }

    [JSInvokable]
    public void OnBreakpointChanged(bool isMobile)
    {
        _state.SetIsMobile(isMobile);
    }

    private async void OnStateChanged()
    {
        await InvokeAsync(async () =>
        {
            if (Open.HasValue)
            {
                await OpenChanged.InvokeAsync(_state.Open);
            }
            StateHasChanged();
        });
    }

    private void HandleKeyDown(KeyboardEventArgs e)
    {
        if ((e.CtrlKey || e.MetaKey) && string.Equals(e.Key, "b", StringComparison.OrdinalIgnoreCase))
        {
            _state.Toggle();
        }
    }

    public async ValueTask DisposeAsync()
    {
        _state.OnChange -= OnStateChanged;

        if (!_jsInitialized || _module is null)
        {
            return;
        }

        try
        {
            await _module.InvokeVoidAsync("dispose");
        }
        catch (JSDisconnectedException)
        {
        }
        catch (OperationCanceledException)
        {
        }

        try
        {
            await _module.DisposeAsync();
        }
        catch (JSDisconnectedException)
        {
        }
        catch (OperationCanceledException)
        {
        }
    }
}
