using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace LowlandTech.Components.ContainerGrid;

/// <summary>
/// A drag-and-drop resizable grid component whose sections snap to column/row tracks.
/// Cloned from MudExGrid (MudBlazor.Extensions) and adapted for the LowlandTech component stack.
/// </summary>
public partial class ContainerGrid : IAsyncDisposable
{
    private int _sectionIdCounter;
    private bool _layoutSuspend;
    private ConcurrentBag<ContainerGridSection> _childSections = new();
    internal ElementReference GridElement;

    [Inject] private IJSRuntime JS { get; set; } = default!;
    private IJSObjectReference? _module;

    // ── Parameters ──────────────────────────────────────────────────────────

    /// <summary>Unique id for the grid. Auto-generated from the CSS class when not set.</summary>
    [Parameter] public string? Id { get; set; }

    /// <summary>Number of column tracks. Default is 12.</summary>
    [Parameter] public int Columns { get; set; } = 12;

    /// <summary>Number of row tracks. Default is 4.</summary>
    [Parameter] public int Rows { get; set; } = 4;

    /// <summary>Explicit height of the grid (e.g. "600px"). Null = auto.</summary>
    [Parameter] public string? Height { get; set; }

    /// <summary>Allow sections to float upward when space is freed above them.</summary>
    [Parameter] public bool FloatMode { get; set; }

    /// <summary>Re-compact the layout after a section is dropped.</summary>
    [Parameter] public bool CompactOnDrop { get; set; }

    /// <summary>Push overlapping sections rather than blocking the move.</summary>
    [Parameter] public bool ResolveCollisions { get; set; } = true;

    /// <summary>Raised after the JS drag engine commits a new layout.</summary>
    [Parameter] public EventCallback<ContainerGridLayout> OnLayoutCommit { get; set; }

    /// <summary>Raised when a section is clicked.</summary>
    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

    [Parameter] public bool OnClickStopPropagation { get; set; }
    [Parameter] public EventCallback OnContextMenu { get; set; }
    [Parameter] public bool OnContextMenuPreventDefault { get; set; }
    [Parameter] public bool OnContextMenuStopPropagation { get; set; }

    // ── Lifecycle ────────────────────────────────────────────────────────────

    protected override void OnInitialized()
    {
        Id ??= "lt-container-grid";
        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _module = await JS.InvokeAsync<IJSObjectReference>(
                "import", "./_content/LowlandTech.Components/js/containerGrid.js");

            await _module.InvokeVoidAsync(
                "containerGrid.bind",
                GridElement,
                DotNetObjectReference.Create(this),
                Rows,
                Columns);
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    public async ValueTask DisposeAsync()
    {
        if (_module is not null)
        {
            try { await _module.InvokeVoidAsync("containerGrid.unbind", GridElement); } catch { }
            await _module.DisposeAsync();
        }
    }

    // ── JS-invokable ─────────────────────────────────────────────────────────

    [JSInvokable("ContainerGrid_CommitLayout")]
    public async Task CommitLayout(List<ContainerSectionLayout> changes)
    {
        if (_layoutSuspend) return;
        _layoutSuspend = true;

        foreach (var ch in changes)
        {
            var section = _childSections.FirstOrDefault(x => x.Id == ch.Id);
            if (section is null) continue;
            section.SetLayout(ch);
        }

        if (CompactOnDrop) Compact();

        await InvokeAsync(StateHasChanged);

        if (OnLayoutCommit.HasDelegate)
            await OnLayoutCommit.InvokeAsync(GetSnapshot());

        await Task.Delay(200);
        _layoutSuspend = false;
    }

    // ── Layout helpers ───────────────────────────────────────────────────────

    public (int Row, int Column)? GetNextFreeSpace()
    {
        for (int r = 1; r <= Rows; r++)
            for (int c = 1; c <= Columns; c++)
                if (!IsOccupied(null, r, c))
                    return (r, c);
        return null;
    }

    public void Compact()
    {
        foreach (var s in _childSections.OrderBy(x => x.Row).ThenBy(x => x.Column))
        {
            int r = s.Row;
            while (r > 1 && CanPlace(s, r - 1, s.Column, s.RowSpan, s.ColSpan)) r--;
            s.Row = r;
        }
    }

    public Task<string> SaveLayoutAsync() =>
        Task.FromResult(JsonSerializer.Serialize(GetSnapshot()));

    public void SetLayout(ContainerGridLayout layout)
    {
        Columns = layout.Columns;
        Rows = layout.Rows;
        foreach (var ch in layout.Sections)
        {
            var s = _childSections.FirstOrDefault(x => x.Id == ch.Id);
            s?.SetLayout(ch);
        }
    }

    public Task RestoreLayoutAsync(string json)
    {
        var layout = JsonSerializer.Deserialize<ContainerGridLayout>(json);
        if (layout is null) return Task.CompletedTask;
        SetLayout(layout);
        return Task.CompletedTask;
    }

    public ContainerGridSection[] GetAllSections() => _childSections.ToArray();

    // ── Internal registration ────────────────────────────────────────────────

    internal void RegisterSection(ContainerGridSection section)
    {
        if (!_childSections.Contains(section))
            _childSections.Add(section);
    }

    internal void UnregisterSection(ContainerGridSection section)
    {
        var updated = new ConcurrentBag<ContainerGridSection>(_childSections.Where(s => s != section));
        Interlocked.Exchange(ref _childSections, updated);
    }

    internal string GetNewSectionId() => $"{Id}-section-{++_sectionIdCounter}";

    // ── Private helpers ──────────────────────────────────────────────────────

    private string GridStyle
    {
        get
        {
            var parts = new List<string>
            {
                "display:grid",
                $"grid-template-columns:repeat({Columns},1fr)",
                $"grid-template-rows:repeat({Rows},1fr)",
                $"--lt-grid-cols:{Columns}",
                $"--lt-grid-rows:{Rows}"
            };
            if (Height is not null) parts.Add($"height:{Height}");
            return string.Join(";", parts);
        }
    }

    /// <summary>
    /// Grid-level options serialised onto <c>data-options</c> so containerGrid.js
    /// can read them per pointer event without round-tripping to .NET.
    /// </summary>
    private string GridOptionsJson => JsonSerializer.Serialize(new
    {
        floatMode = FloatMode,
        compactOnDrop = CompactOnDrop,
        resolveCollisions = ResolveCollisions,
    }, JsonOpts);

    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

    private bool IsOccupied(ContainerGridSection? ignore, int r, int c) =>
        _childSections.Any(s => s != ignore
            && r >= s.Row && r < s.Row + s.RowSpan
            && c >= s.Column && c < s.Column + s.ColSpan);

    private bool CanPlace(ContainerGridSection ignore, int r, int c, int rs, int cs)
    {
        if (r < 1 || c < 1 || r + rs - 1 > Rows || c + cs - 1 > Columns) return false;
        for (int i = r; i < r + rs; i++)
            for (int j = c; j < c + cs; j++)
                if (IsOccupied(ignore, i, j)) return false;
        return true;
    }

    private ContainerGridLayout GetSnapshot() =>
        new(Columns, Rows, _childSections.Select(s => s.GetLayout()).ToList());

    protected virtual Task OnClickHandler(MouseEventArgs ev) => OnClick.InvokeAsync(ev);
}
