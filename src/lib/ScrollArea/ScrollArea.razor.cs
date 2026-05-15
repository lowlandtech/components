using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LowlandTech.Components.ScrollArea;

public partial class ScrollArea : LtComponentBase, IDisposable
{
    [Parameter] public string? Orientation { get; set; }

    // Scrollbar customization parameters
    [Parameter] public bool ShowScrollbar { get; set; } = true;
    [Parameter] public bool ScrollbarHoverOnly { get; set; } = true;
    [Parameter] public string? ScrollbarWidth { get; set; } = "8px";
    [Parameter] public string? ScrollbarTrackColor { get; set; }
    [Parameter] public string? ScrollbarThumbColor { get; set; }

    private ElementReference _viewportRef;
    private ElementReference _scrollbarRef;
    private ElementReference _trackRef;
    private ElementReference _thumbRef;
    private IJSObjectReference? _jsModule;
    private readonly string _areaId = Guid.NewGuid().ToString("n")[..8];

    private string EffectiveOrientation => Orientation switch
    {
        "horizontal" => "horizontal",
        _ => "vertical"
    };

    private string ViewportOverflowClass => Orientation switch
    {
        "horizontal" => "overflow-x-auto overflow-y-hidden",
        "both" => "overflow-auto",
        _ => "overflow-y-auto overflow-x-hidden"
    };

    private string GetScrollbarPositionClass() => EffectiveOrientation switch
    {
        "horizontal" => "bottom-0 left-0 right-0 h-[var(--sa-thumb-width)]",
        _ => "top-0 right-0 bottom-0 w-[var(--sa-thumb-width)]"
    };

    private string GetScrollbarVisibilityClass() => ScrollbarHoverOnly
        ? "opacity-0 group-hover/scroll-area:opacity-100"
        : "opacity-100";

    private string GetScrollbarStyle()
    {
        var width = string.IsNullOrWhiteSpace(ScrollbarWidth) ? "8px" : ScrollbarWidth;
        return $"--sa-thumb-width: {width};";
    }

    private string GetTrackStyle()
    {
        var color = string.IsNullOrWhiteSpace(ScrollbarTrackColor) ? "transparent" : ScrollbarTrackColor;
        return $"background-color: {color};";
    }

    private string GetThumbStyle()
    {
        var color = string.IsNullOrWhiteSpace(ScrollbarThumbColor) ? "var(--border, #ccc)" : ScrollbarThumbColor;
        return $"background-color: {color};";
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && ShowScrollbar)
        {
            _jsModule ??= await JSRuntime.InvokeAsync<IJSObjectReference>("import",
                "./_content/LowlandTech.Components/js/scroll-area.js");
            await _jsModule.InvokeVoidAsync("init", _areaId, _viewportRef, _scrollbarRef, _thumbRef, _trackRef, EffectiveOrientation);
        }
    }

    public void Dispose()
    {
        _jsModule?.InvokeVoidAsync("destroy", _areaId);
        _jsModule?.DisposeAsync();
    }
}
