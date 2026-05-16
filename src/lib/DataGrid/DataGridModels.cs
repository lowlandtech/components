namespace LowlandTech.Components.DataGrid;

public enum DataGridSelectionMode
{
    None,
    Single,
    Multiple
}

public enum DataGridSortDirection
{
    None,
    Ascending,
    Descending
}

public enum DataGridTextAlign
{
    Start,
    Center,
    End
}

public enum DataGridDensity
{
    Compact,
    Default,
    Comfortable
}

public enum DataGridGridLines
{
    None,
    Horizontal,
    Vertical,
    Both
}

public sealed record DataGridSortDescriptor(
    string ColumnKey,
    string PropertyPath,
    DataGridSortDirection Direction,
    int Order);

