namespace LowlandTech.Components.Components.DataGrid;

/// <summary>
/// Describes a filter applied to a DataGrid column.
/// </summary>
/// <typeparam name="TItem">The type of data items in the grid.</typeparam>
public class FilterDescriptor<TItem> where TItem : notnull
{
    /// <summary>
    /// Gets or sets the property name to filter on.
    /// </summary>
    public string PropertyName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the filter value.
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the filter operator (for future advanced filtering).
    /// </summary>
    public string Operator { get; set; } = "contains";
}
