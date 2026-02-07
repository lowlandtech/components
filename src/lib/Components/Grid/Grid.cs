using LowlandTech.Components.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Text.Json;

namespace LowlandTech.Components.Components.Grid;

public partial class Grid
{
    private List<GridSection> _sections = new();
    private int _nextSectionIndex = 1;
    private bool _isSuspended;

    [Parameter]
    public int Row { get; set; } = 4;

    [Parameter]
    public int Column { get; set; } = 4;

    [Parameter]
    public string? Height { get; set; }

    [Parameter]
    public bool FloatMode { get; set; }

    [Parameter]
    public bool CompactOnDrop { get; set; }

    [Parameter]
    public bool ResolveCollisions { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public string? Style { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    [Parameter]
    public EventCallback<MouseEventArgs> OnClick { get; set; }

    [Parameter]
    public EventCallback<MouseEventArgs> OnContextMenu { get; set; }

    private string GetClasses()
    {
        return CssClassUtils.Merge(
            "grid-layout",
            Class
        );
    }

    private string? GetStyle()
    {
        var styles = new List<string?>();

        if (!string.IsNullOrEmpty(Style))
        {
            styles.Add(Style);
        }

        styles.Add($"grid-template-rows: repeat({Row}, minmax(0, 1fr));");
        styles.Add($"grid-template-columns: repeat({Column}, minmax(0, 1fr));");

        if (!string.IsNullOrEmpty(Height))
        {
            styles.Add($"height: {Height};");
        }

        return string.Join(" ", styles.Where(s => !string.IsNullOrEmpty(s)));
    }

    internal void RegisterSection(GridSection section)
    {
        if (!_sections.Contains(section))
        {
            if (string.IsNullOrEmpty(section.Id))
            {
                section.Id = $"grid-section-{_nextSectionIndex++}";
            }
            _sections.Add(section);
        }
    }

    internal void UnregisterSection(GridSection section)
    {
        _sections.Remove(section);
    }

    public GridSection[] GetAllChildSections()
    {
        return _sections.ToArray();
    }

    public (int Row, int Column)? GetNextFreeSpace()
    {
        for (int row = 1; row <= Row; row++)
        {
            for (int col = 1; col <= Column; col++)
            {
                if (!IsPositionOccupied(row, col, 1, 1))
                {
                    return (row, col);
                }
            }
        }
        return null;
    }

    private bool IsPositionOccupied(int startRow, int startCol, int rowSpan, int colSpan)
    {
        foreach (var section in _sections)
        {
            if (IsOverlapping(startRow, startCol, rowSpan, colSpan, section.Row, section.Column, section.RowSpan, section.ColSpan))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsOverlapping(int r1, int c1, int rs1, int cs1, int r2, int c2, int rs2, int cs2)
    {
        return !(r1 + rs1 - 1 < r2 || r2 + rs2 - 1 < r1 || c1 + cs1 - 1 < c2 || c2 + cs2 - 1 < c1);
    }

    public bool CanPlace(int row, int col, int rowSpan, int colSpan)
    {
        if (row < 1 || col < 1 || row + rowSpan - 1 > Row || col + colSpan - 1 > Column)
        {
            return false;
        }
        return !IsPositionOccupied(row, col, rowSpan, colSpan);
    }

    public void Compact()
    {
        var sortedSections = _sections.OrderBy(s => s.Row).ThenBy(s => s.Column).ToList();
        var occupied = new HashSet<(int Row, int Col)>();

        foreach (var section in sortedSections)
        {
            var newPosition = FindFirstAvailablePosition(section.RowSpan, section.ColSpan, occupied);
            if (newPosition.HasValue)
            {
                section.UpdatePosition(newPosition.Value.Row, newPosition.Value.Col, false);
                MarkOccupied(newPosition.Value.Row, newPosition.Value.Col, section.RowSpan, section.ColSpan, occupied);
            }
        }
    }

    private (int Row, int Col)? FindFirstAvailablePosition(int rowSpan, int colSpan, HashSet<(int Row, int Col)> occupied)
    {
        for (int row = 1; row <= Row - rowSpan + 1; row++)
        {
            for (int col = 1; col <= Column - colSpan + 1; col++)
            {
                if (IsPositionAvailable(row, col, rowSpan, colSpan, occupied))
                {
                    return (row, col);
                }
            }
        }
        return null;
    }

    private bool IsPositionAvailable(int startRow, int startCol, int rowSpan, int colSpan, HashSet<(int Row, int Col)> occupied)
    {
        for (int r = startRow; r < startRow + rowSpan; r++)
        {
            for (int c = startCol; c < startCol + colSpan; c++)
            {
                if (occupied.Contains((r, c)))
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void MarkOccupied(int startRow, int startCol, int rowSpan, int colSpan, HashSet<(int Row, int Col)> occupied)
    {
        for (int r = startRow; r < startRow + rowSpan; r++)
        {
            for (int c = startCol; c < startCol + colSpan; c++)
            {
                occupied.Add((r, c));
            }
        }
    }

    public async Task<string> SaveLayoutAsync()
    {
        var layout = new GridLayout
        {
            Rows = Row,
            Columns = Column,
            Height = Height,
            FloatMode = FloatMode,
            CompactOnDrop = CompactOnDrop,
            ResolveCollisions = ResolveCollisions,
            Sections = _sections.Select(s => new GridSectionLayout
            {
                Id = s.Id,
                Row = s.Row,
                Column = s.Column,
                RowSpan = s.RowSpan,
                ColSpan = s.ColSpan
            }).ToList()
        };

        return JsonSerializer.Serialize(layout);
    }

    public async Task RestoreLayoutAsync(string json)
    {
        var layout = JsonSerializer.Deserialize<GridLayout>(json);
        if (layout == null) return;

        Row = layout.Rows;
        Column = layout.Columns;
        Height = layout.Height;
        FloatMode = layout.FloatMode;
        CompactOnDrop = layout.CompactOnDrop;
        ResolveCollisions = layout.ResolveCollisions;

        foreach (var sectionLayout in layout.Sections)
        {
            var section = _sections.FirstOrDefault(s => s.Id == sectionLayout.Id);
            if (section != null)
            {
                section.UpdatePosition(sectionLayout.Row, sectionLayout.Column, false);
                section.UpdateSpan(sectionLayout.RowSpan, sectionLayout.ColSpan, false);
            }
        }

        StateHasChanged();
        await Task.CompletedTask;
    }

    internal void SuspendLayout()
    {
        _isSuspended = true;
    }

    internal void ResumeLayout()
    {
        _isSuspended = false;
        StateHasChanged();
    }

    internal void CommitLayout()
    {
        if (!_isSuspended)
        {
            StateHasChanged();
        }
    }

    private async Task HandleClick(MouseEventArgs e)
    {
        await OnClick.InvokeAsync(e);
    }

    private async Task HandleContextMenu(MouseEventArgs e)
    {
        await OnContextMenu.InvokeAsync(e);
    }
}
