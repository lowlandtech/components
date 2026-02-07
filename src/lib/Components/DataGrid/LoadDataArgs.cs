namespace LowlandTech.Components.Components.DataGrid;

/// <summary>
/// Represents a sort configuration for server-side operations.
/// </summary>
public class SortInfo
{
    /// <summary>
    /// Gets or sets the property name to sort by.
    /// </summary>
    public string PropertyName { get; set; } = string.Empty;

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

/// <summary>
/// Arguments passed to the LoadData event handler for server-side data operations.
/// </summary>
public class LoadDataArgs
{
    /// <summary>
    /// Gets or sets the number of items to skip (for paging).
    /// </summary>
    public int Skip { get; set; }

    /// <summary>
    /// Gets or sets the number of items to take (page size).
    /// </summary>
    public int Top { get; set; }

    /// <summary>
    /// Gets or sets the OData-style OrderBy string for sorting.
    /// Example: "Name asc" or "Name asc,Age desc"
    /// </summary>
    public string? OrderBy { get; set; }

    /// <summary>
    /// Gets or sets the OData-style Filter string for filtering.
    /// Example: "contains(Name,'John')"
    /// </summary>
    public string? Filter { get; set; }

    /// <summary>
    /// Gets or sets the collection of sort descriptors.
    /// </summary>
    public IEnumerable<SortInfo>? SortDescriptors { get; set; }

    /// <summary>
    /// Gets or sets the collection of filter descriptors (property name to filter value).
    /// </summary>
    public IDictionary<string, string>? Filters { get; set; }
}
