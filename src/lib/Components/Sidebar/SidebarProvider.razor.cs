using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Sidebar;

public partial class SidebarProvider : ComponentBase, IAsyncDisposable
{
    private const string SidebarWidthDefault = "16rem";
    private const string SidebarWidthIcon = "3rem";
    private const string SidebarKeyboardShortcut = "b";
    
    private ElementReference _wrapperRef;
    private IJSObjectReference? _module;
    private DotNetObjectReference<SidebarProvider>? _dotNetRef;
    private bool _isMobile;
    private bool _openMobile;
    private bool _internalOpen;

    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    [Parameter] public bool DefaultOpen { get; set; } = true;
    [Parameter] public bool? Open { get; set; }
    [Parameter] public EventCallback<bool> OpenChanged { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public string? Class { get; set; }
    [Parameter] public string? Style { get; set; }
    [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object>? AdditionalAttributes { get; set; }

    public string State => IsOpen ? "expanded" : "collapsed";
    public bool IsOpen => Open ?? _internalOpen;
    public bool IsMobile => _isMobile;
    public bool OpenMobile { get => _openMobile; set => _openMobile = value; }

    private string WrapperClasses => CssClassUtils.Merge(
        "group/sidebar-wrapper",
        "has-data-[variant=inset]:bg-sidebar",
        "flex",
        "min-h-svh",
        "w-full",
        Class
    );

    private string WrapperStyle
    {
        get
        {
            var styles = new List<string>
            {
                $"--sidebar-width: {SidebarWidthDefault}",
                $"--sidebar-width-icon: {SidebarWidthIcon}"
            };
            
            if (!string.IsNullOrWhiteSpace(Style))
                styles.Add(Style);
            
            return string.Join("; ", styles);
        }
    }

    protected override void OnInitialized()
    {
        _internalOpen = DefaultOpen;
        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _dotNetRef = DotNetObjectReference.Create(this);
            _module = await JSRuntime.InvokeAsync<IJSObjectReference>("import", 
                "./_content/LowlandTech.Components/Components/Sidebar/SidebarProvider.razor.js");
            
            await _module.InvokeVoidAsync("initialize", _wrapperRef, _dotNetRef, SidebarKeyboardShortcut);
            
            // Check if mobile
            _isMobile = await _module.InvokeAsync<bool>("isMobile");
            StateHasChanged();
        }
    }

    public async Task SetOpenAsync(bool open)
    {
        if (Open.HasValue && OpenChanged.HasDelegate)
        {
            await OpenChanged.InvokeAsync(open);
        }
        else
        {
            _internalOpen = open;
            StateHasChanged();
        }

        // Persist state via JS (cookie)
        if (_module != null)
        {
            await _module.InvokeVoidAsync("saveState", open);
        }
    }

    public async Task ToggleSidebarAsync()
    {
        if (_isMobile)
        {
            _openMobile = !_openMobile;
            StateHasChanged();
        }
        else
        {
            await SetOpenAsync(!IsOpen);
        }
    }

    [JSInvokable]
    public async Task OnKeyboardShortcut()
    {
        await ToggleSidebarAsync();
    }

    [JSInvokable]
    public void OnMobileChange(bool isMobile)
    {
        _isMobile = isMobile;
        StateHasChanged();
    }

    public async ValueTask DisposeAsync()
    {
        if (_module != null)
        {
            await _module.InvokeVoidAsync("dispose");
            await _module.DisposeAsync();
        }
        
        _dotNetRef?.Dispose();
    }
}
