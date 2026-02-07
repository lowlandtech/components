namespace LowlandTech.Components.Components.DataGrid;

/// <summary>
/// Represents a column sort configuration.
/// </summary>
/// <typeparam name="TItem">The type of data items.</typeparam>
internal class SortDescriptor<TItem> where TItem : notnull
{
    /// <summary>
    /// Gets or sets the column to sort by.
    /// </summary>
    public DataGridColumn<TItem> Column { get; set; } = null!;

    /// <summary>
    /// Gets or sets the sort direction.
    /// </summary>
    public SortDirection Direction { get; set; }

    /// <summary>
    /// Gets or sets the priority (order) of this sort in multi-column sorting.
    /// Lower numbers = higher priority.
    /// </summary>
    public int Priority { get; set; }
}
