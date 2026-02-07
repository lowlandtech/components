using LowlandTech.Components.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace LowlandTech.Components.Components.Grid;

public partial class GridSection
{
    [CascadingParameter]
    private Grid? ParentGrid { get; set; }

    [Parameter]
    public string Id { get; set; } = string.Empty;

    [Parameter]
    public int Row { get; set; } = 1;

    [Parameter]
    public int Column { get; set; } = 1;

    [Parameter]
    public int RowSpan { get; set; } = 1;

    [Parameter]
    public int ColSpan { get; set; } = 1;

    [Parameter]
    public bool Movable { get; set; }

    [Parameter]
    public bool Resizable { get; set; }

    [Parameter]
    public bool WrapInGroupBox { get; set; }

    [Parameter]
    public string? Title { get; set; }

    [Parameter]
    public int MinRow { get; set; } = 1;

    [Parameter]
    public int MaxRow { get; set; } = int.MaxValue;

    [Parameter]
    public int MinCol { get; set; } = 1;

    [Parameter]
    public int MaxCol { get; set; } = int.MaxValue;

    [Parameter]
    public int MinRowSpan { get; set; } = 1;

    [Parameter]
    public int MaxRowSpan { get; set; } = int.MaxValue;

    [Parameter]
    public int MinColSpan { get; set; } = 1;

    [Parameter]
    public int MaxColSpan { get; set; } = int.MaxValue;

    [Parameter]
    public bool LockX { get; set; }

    [Parameter]
    public bool LockY { get; set; }

    [Parameter]
    public bool Animate { get; set; }

    [Parameter]
    public int AnimationMs { get; set; } = 200;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public string? Style { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    [Parameter]
    public EventCallback<int> RowChanged { get; set; }

    [Parameter]
    public EventCallback<int> ColumnChanged { get; set; }

    [Parameter]
    public EventCallback<int> RowSpanChanged { get; set; }

    [Parameter]
    public EventCallback<int> ColSpanChanged { get; set; }

    [Parameter]
    public EventCallback<GridSectionChangedEventArgs> OnManualPositionChanged { get; set; }

    [Parameter]
    public EventCallback<MouseEventArgs> OnClick { get; set; }

    [Parameter]
    public EventCallback<MouseEventArgs> OnDblClick { get; set; }

    [Parameter]
    public EventCallback<MouseEventArgs> OnContextMenu { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        ParentGrid?.RegisterSection(this);
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        EnforceConstraints();
    }

    private void EnforceConstraints()
    {
        Row = Math.Max(MinRow, Math.Min(MaxRow, Row));
        Column = Math.Max(MinCol, Math.Min(MaxCol, Column));
        RowSpan = Math.Max(MinRowSpan, Math.Min(MaxRowSpan, RowSpan));
        ColSpan = Math.Max(MinColSpan, Math.Min(MaxColSpan, ColSpan));

        if (ParentGrid != null)
        {
            Row = Math.Min(Row, ParentGrid.Row);
            Column = Math.Min(Column, ParentGrid.Column);
            RowSpan = Math.Min(RowSpan, ParentGrid.Row - Row + 1);
            ColSpan = Math.Min(ColSpan, ParentGrid.Column - Column + 1);
        }
    }

    private string GetClasses()
    {
        var classes = new List<string> { "grid-section" };

        if (Animate)
        {
            classes.Add("grid-section-animate");
        }

        classes.Add(Class ?? string.Empty);

        return CssClassUtils.Merge(classes.ToArray());
    }

    private string GetStyle()
    {
        var styles = new List<string?>();

        styles.Add($"grid-row: {Row} / span {RowSpan};");
        styles.Add($"grid-column: {Column} / span {ColSpan};");

        if (Animate)
        {
            styles.Add($"transition-duration: {AnimationMs}ms;");
        }

        if (!string.IsNullOrEmpty(Style))
        {
            styles.Add(Style);
        }

        return string.Join(" ", styles.Where(s => !string.IsNullOrEmpty(s)));
    }

    internal void UpdatePosition(int newRow, int newCol, bool manual = true)
    {
        if (LockX) newCol = Column;
        if (LockY) newRow = Row;

        Row = Math.Max(MinRow, Math.Min(MaxRow, newRow));
        Column = Math.Max(MinCol, Math.Min(MaxCol, newCol));

        if (manual)
        {
            RowChanged.InvokeAsync(Row);
            ColumnChanged.InvokeAsync(Column);
            OnManualPositionChanged.InvokeAsync(new GridSectionChangedEventArgs
            {
                Id = Id,
                Row = Row,
                Column = Column,
                RowSpan = RowSpan,
                ColSpan = ColSpan
            });
        }
    }

    internal void UpdateSpan(int newRowSpan, int newColSpan, bool manual = true)
    {
        RowSpan = Math.Max(MinRowSpan, Math.Min(MaxRowSpan, newRowSpan));
        ColSpan = Math.Max(MinColSpan, Math.Min(MaxColSpan, newColSpan));

        if (manual)
        {
            RowSpanChanged.InvokeAsync(RowSpan);
            ColSpanChanged.InvokeAsync(ColSpan);
            OnManualPositionChanged.InvokeAsync(new GridSectionChangedEventArgs
            {
                Id = Id,
                Row = Row,
                Column = Column,
                RowSpan = RowSpan,
                ColSpan = ColSpan
            });
        }
    }

    private async Task HandleClick(MouseEventArgs e)
    {
        await OnClick.InvokeAsync(e);
    }

    private async Task HandleDoubleClick(MouseEventArgs e)
    {
        await OnDblClick.InvokeAsync(e);
    }

    private async Task HandleContextMenu(MouseEventArgs e)
    {
        await OnContextMenu.InvokeAsync(e);
    }

    public void Dispose()
    {
        ParentGrid?.UnregisterSection(this);
    }
}
