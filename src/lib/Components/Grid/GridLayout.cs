namespace LowlandTech.Components.Components.Grid;

public class GridLayout
{
    public int Rows { get; set; }
    public int Columns { get; set; }
    public string? Height { get; set; }
    public bool FloatMode { get; set; }
    public bool CompactOnDrop { get; set; }
    public bool ResolveCollisions { get; set; }
    public List<GridSectionLayout> Sections { get; set; } = new();
}
