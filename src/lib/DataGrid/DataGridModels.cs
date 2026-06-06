namespace LowlandTech.Components;

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

public sealed record DataGridFilterDescriptor(
    string ColumnKey,
    string PropertyPath,
    string Operator,
    string Value);

public sealed record DataGridStateRepresentation(
    int PageIndex,
    int PageSize,
    IReadOnlyList<DataGridSortDescriptor> SortDescriptors,
    IReadOnlyList<DataGridFilterDescriptor> FilterDescriptors,
    IReadOnlyList<string> SelectedRowKeys,
    IReadOnlyList<string> ExpandedRowKeys);

