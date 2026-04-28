using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace LowlandTech.Components.ContainerGrid;

/// <summary>
/// A positioned section placed inside a <see cref="ContainerGrid"/>.
/// Column, Row, ColSpan, and RowSpan define its position and size on the grid.
/// Cloned from MudExGridSection (MudBlazor.Extensions) and adapted for the LowlandTech stack.
/// </summary>
public partial class ContainerGridSection : IAsyncDisposable
{
    private int _column = 1;
    private int _colSpan = 1;
    private int _row = 1;
    private int _rowSpan = 1;
    private ContainerSectionLayout? _committedLayout;

    [Inject] private IJSRuntime JS { get; set; } = default!;
    private IJSObjectReference? _module;

    internal ElementReference HandleSE, HandleE, HandleS;

    // ── Parameters ──────────────────────────────────────────────────────────

    /// <summary>Section id. Auto-assigned by the parent grid when not set.</summary>
    [Parameter] public string? Id { get; set; }

    /// <summary>Reference to the rendered root element.</summary>
    public ElementReference ElementReference { get; set; }

    /// <summary>Injected by the parent <see cref="ContainerGrid"/> via CascadingValue.</summary>
    [CascadingParameter] public ContainerGrid Grid { get; set; } = default!;

    /// <summary>One-based column start position. Default is 1.</summary>
    [Parameter]
    public int Column
    {
        get => _committedLayout?.Column ?? _column;
        set => SetParam(ref _column, value, v => ColumnChanged.InvokeAsync(v));
    }

    /// <summary>Number of columns this section spans. Default is 1.</summary>
    [Parameter]
    public int ColSpan
    {
        get => _committedLayout?.ColSpan ?? _colSpan;
        set => SetParam(ref _colSpan, value, v => ColSpanChanged.InvokeAsync(v));
    }

    /// <summary>One-based row start position. Default is 1.</summary>
    [Parameter]
    public int Row
    {
        get => _committedLayout?.Row ?? _row;
        set => SetParam(ref _row, value, v => RowChanged.InvokeAsync(v));
    }

    /// <summary>Number of rows this section spans. Default is 1.</summary>
    [Parameter]
    public int RowSpan
    {
        get => _committedLayout?.RowSpan ?? _rowSpan;
        set => SetParam(ref _rowSpan, value, v => RowSpanChanged.InvokeAsync(v));
    }

    [Parameter] public EventCallback<int> ColumnChanged { get; set; }
    [Parameter] public EventCallback<int> ColSpanChanged { get; set; }
    [Parameter] public EventCallback<int> RowChanged { get; set; }
    [Parameter] public EventCallback<int> RowSpanChanged { get; set; }

    /// <summary>Whether this section can be dragged to a new position.</summary>
    [Parameter] public bool Movable { get; set; }

    /// <summary>Whether this section exposes resize handles.</summary>
    [Parameter] public bool Resizable { get; set; }

    // Constraint parameters forwarded to the JS binding
    [Parameter] public int MinCol { get; set; } = 1;
    [Parameter] public int MaxCol { get; set; } = int.MaxValue;
    [Parameter] public int MinRow { get; set; } = 1;
    [Parameter] public int MaxRow { get; set; } = int.MaxValue;
    [Parameter] public int MinColSpan { get; set; } = 1;
    [Parameter] public int MaxColSpan { get; set; } = 12;
    [Parameter] public int MinRowSpan { get; set; } = 1;
    [Parameter] public int MaxRowSpan { get; set; } = int.MaxValue;

    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }
    [Parameter] public EventCallback<MouseEventArgs> OnDblClick { get; set; }
    [Parameter] public bool OnClickStopPropagation { get; set; }
    [Parameter] public EventCallback OnContextMenu { get; set; }
    [Parameter] public bool OnContextMenuPreventDefault { get; set; }
    [Parameter] public bool OnContextMenuStopPropagation { get; set; }

    // ── Lifecycle ────────────────────────────────────────────────────────────

    protected override void OnInitialized()
    {
        Id ??= Grid?.GetNewSectionId();
        Grid?.RegisterSection(this);
        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && (Movable || Resizable))
        {
            _module = await JS.InvokeAsync<IJSObjectReference>(
                "import", "./_content/LowlandTech.Components/js/containerGrid.js");

            await _module.InvokeVoidAsync(
                "containerGrid.bindSection",
                ElementReference,
                DotNetObjectReference.Create(this),
                new { Movable, Resizable, MinCol, MaxCol, MinRow, MaxRow, MinColSpan, MaxColSpan, MinRowSpan, MaxRowSpan });
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    public async ValueTask DisposeAsync()
    {
        Grid?.UnregisterSection(this);
        if (_module is not null)
        {
            try { await _module.InvokeVoidAsync("containerGrid.unbindSection", ElementReference); } catch { }
            await _module.DisposeAsync();
        }
    }

    // ── JS-invokable ─────────────────────────────────────────────────────────

    [JSInvokable("ContainerGrid_CommitLayout")]
    public Task CommitLayoutJs(List<ContainerSectionLayout> changes) =>
        Grid.CommitLayout(changes);

    // ── Internal ─────────────────────────────────────────────────────────────

    internal void SetLayout(ContainerSectionLayout layout) => _committedLayout = layout;

    internal ContainerSectionLayout GetLayout() =>
        new(Id!, Row, Column, RowSpan, ColSpan);

    // ── Styles ───────────────────────────────────────────────────────────────

    private string SectionStyle =>
        $"grid-column:{Column} / span {ColSpan};grid-row:{Row} / span {RowSpan};";

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static void SetParam<T>(ref T backing, T value, Func<T, Task> callback)
    {
        if (!EqualityComparer<T>.Default.Equals(backing, value))
        {
            backing = value;
            _ = callback(value);
        }
    }

    protected virtual Task OnClickHandler(MouseEventArgs ev) => OnClick.InvokeAsync(ev);
    protected virtual Task OnDblClickHandler(MouseEventArgs ev) => OnDblClick.InvokeAsync(ev);
}
