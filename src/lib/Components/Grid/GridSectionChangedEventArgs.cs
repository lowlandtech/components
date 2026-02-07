namespace LowlandTech.Components.Components.Grid;

public class GridSectionChangedEventArgs
{
    public string Id { get; set; } = string.Empty;
    public int Row { get; set; }
    public int Column { get; set; }
    public int RowSpan { get; set; }
    public int ColSpan { get; set; }
}
