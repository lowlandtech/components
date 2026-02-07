using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using LowlandTech.Components.Utilities;
using System.Linq;

namespace LowlandTech.Components.Components.DataGrid;

/// <summary>
/// A data grid component for displaying tabular data with support for sorting, filtering, paging, and more.
/// </summary>
/// <typeparam name="TItem">The type of data items displayed in the grid.</typeparam>
public partial class DataGrid<TItem> : ComponentBase where TItem : notnull
{
    private List<DataGridColumn<TItem>> _columns = new();
    private DataGridColumn<TItem>? _sortColumn;
    private SortDirection _sortDirection = SortDirection.None;
    private List<SortDescriptor<TItem>> _sortDescriptors = new();

    /// <summary>
    /// Gets or sets the data items to display in the grid.
    /// </summary>
    [Parameter]
    public IEnumerable<TItem>? Data { get; set; }

    /// <summary>
    /// Gets or sets whether sorting is enabled for the grid.
    /// </summary>
    [Parameter]
    public bool AllowSorting { get; set; }

    /// <summary>
    /// Gets or sets whether multi-column sorting is enabled.
    /// When true, users can hold Ctrl/Cmd while clicking headers to sort by multiple columns.
    /// </summary>
    [Parameter]
    public bool AllowMultiColumnSorting { get; set; }

    /// <summary>
    /// Gets or sets the columns for the data grid.
    /// </summary>
    [Parameter]
    public RenderFragment? Columns { get; set; }

    /// <summary>
    /// Gets or sets the child content (alternative way to define content).
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the template to display above the grid (header toolbar).
    /// </summary>
    [Parameter]
    public RenderFragment? HeaderTemplate { get; set; }

    /// <summary>
    /// Gets or sets the template to display when there is no data.
    /// </summary>
    [Parameter]
    public RenderFragment? EmptyTemplate { get; set; }

    /// <summary>
    /// Gets or sets custom CSS class for the table.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets additional attributes for the table element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
    }

    /// <summary>
    /// Registers a column with the data grid.
    /// </summary>
    internal void AddColumn(DataGridColumn<TItem> column)
    {
        if (!_columns.Contains(column))
        {
            _columns.Add(column);
            StateHasChanged();
        }
    }

    /// <summary>
    /// Unregisters a column from the data grid.
    /// </summary>
    internal void RemoveColumn(DataGridColumn<TItem> column)
    {
        _columns.Remove(column);
        StateHasChanged();
    }

    /// <summary>
    /// Handles column header click with mouse event args for multi-sort detection.
    /// </summary>
    private void HandleHeaderClick(DataGridColumn<TItem> column, MouseEventArgs args)
    {
        bool isMultiSort = args.CtrlKey || args.MetaKey; // Ctrl on Windows/Linux, Cmd on Mac
        HandleColumnHeaderClick(column, isMultiSort);
    }

    /// <summary>
    /// Handles column header click for sorting.
    /// </summary>
    internal void HandleColumnHeaderClick(DataGridColumn<TItem> column, bool isMultiSort = false)
    {
        if (!AllowSorting || !column.Sortable)
            return;

        // If multi-column sorting is disabled or not requested, use single-column mode
        if (!AllowMultiColumnSorting || !isMultiSort)
        {
            HandleSingleColumnSort(column);
        }
        else
        {
            HandleMultiColumnSort(column);
        }

        StateHasChanged();
    }

    private void HandleSingleColumnSort(DataGridColumn<TItem> column)
    {
        // Clear multi-column sort descriptors
        _sortDescriptors.Clear();

        if (_sortColumn == column)
        {
            // Toggle sort direction: None -> Ascending -> Descending -> None
            _sortDirection = _sortDirection switch
            {
                SortDirection.None => SortDirection.Ascending,
                SortDirection.Ascending => SortDirection.Descending,
                SortDirection.Descending => SortDirection.None,
                _ => SortDirection.None
            };

            if (_sortDirection == SortDirection.None)
            {
                _sortColumn = null;
            }
        }
        else
        {
            // New column - start with ascending
            _sortColumn = column;
            _sortDirection = SortDirection.Ascending;
        }
    }

    private void HandleMultiColumnSort(DataGridColumn<TItem> column)
    {
        // If we have a single-column sort active, migrate it to multi-column mode
        if (_sortColumn != null && _sortDirection != SortDirection.None && !_sortDescriptors.Any())
        {
            _sortDescriptors.Add(new SortDescriptor<TItem>
            {
                Column = _sortColumn,
                Direction = _sortDirection,
                Priority = 1
            });
        }

        // Clear single-column sort state (we're now in multi-column mode)
        _sortColumn = null;
        _sortDirection = SortDirection.None;

        var existing = _sortDescriptors.FirstOrDefault(s => s.Column == column);
        
        if (existing != null)
        {
            // Toggle sort direction for existing column
            existing.Direction = existing.Direction switch
            {
                SortDirection.None => SortDirection.Ascending,
                SortDirection.Ascending => SortDirection.Descending,
                SortDirection.Descending => SortDirection.None,
                _ => SortDirection.None
            };

            // Remove if direction is None
            if (existing.Direction == SortDirection.None)
            {
                _sortDescriptors.Remove(existing);
                // Re-assign priorities
                for (int i = 0; i < _sortDescriptors.Count; i++)
                {
                    _sortDescriptors[i].Priority = i + 1;
                }
            }
        }
        else
        {
            // Add new sort column
            _sortDescriptors.Add(new SortDescriptor<TItem>
            {
                Column = column,
                Direction = SortDirection.Ascending,
                Priority = _sortDescriptors.Count + 1
            });
        }
    }

    /// <summary>
    /// Gets the sort direction for a column.
    /// </summary>
    internal SortDirection GetColumnSortDirection(DataGridColumn<TItem> column)
    {
        // Check multi-column sort first
        var descriptor = _sortDescriptors.FirstOrDefault(s => s.Column == column);
        if (descriptor != null)
            return descriptor.Direction;

        // Fall back to single-column sort
        return _sortColumn == column ? _sortDirection : SortDirection.None;
    }

    /// <summary>
    /// Gets the sort priority for a column (for multi-column sorting).
    /// Returns 0 if column is not sorted.
    /// </summary>
    internal int GetColumnSortPriority(DataGridColumn<TItem> column)
    {
        var descriptor = _sortDescriptors.FirstOrDefault(s => s.Column == column);
        return descriptor?.Priority ?? 0;
    }

    /// <summary>
    /// Gets the data with sorting applied.
    /// </summary>
    private IEnumerable<TItem> GetSortedData()
    {
        if (Data == null)
            return Enumerable.Empty<TItem>();

        // Use multi-column sort if available
        if (_sortDescriptors.Any())
        {
            return ApplyMultiColumnSort(Data);
        }

        // Fall back to single-column sort
        if (_sortColumn == null || _sortDirection == SortDirection.None)
            return Data;

        var propertyInfo = _sortColumn.GetPropertyInfo();
        if (propertyInfo == null)
            return Data;

        return _sortDirection == SortDirection.Ascending
            ? Data.OrderBy(item => propertyInfo.GetValue(item))
            : Data.OrderByDescending(item => propertyInfo.GetValue(item));
    }

    private IEnumerable<TItem> ApplyMultiColumnSort(IEnumerable<TItem> data)
    {
        IOrderedEnumerable<TItem>? orderedData = null;

        // Sort by descriptors in priority order
        foreach (var descriptor in _sortDescriptors.OrderBy(d => d.Priority))
        {
            var propertyInfo = descriptor.Column.GetPropertyInfo();
            if (propertyInfo == null)
                continue;

            if (orderedData == null)
            {
                // First sort
                orderedData = descriptor.Direction == SortDirection.Ascending
                    ? data.OrderBy(item => propertyInfo.GetValue(item))
                    : data.OrderByDescending(item => propertyInfo.GetValue(item));
            }
            else
            {
                // Subsequent sorts (ThenBy)
                orderedData = descriptor.Direction == SortDirection.Ascending
                    ? orderedData.ThenBy(item => propertyInfo.GetValue(item))
                    : orderedData.ThenByDescending(item => propertyInfo.GetValue(item));
            }
        }

        return orderedData ?? data;
    }

    private string GetTableClasses()
    {
        return CssClassUtils.Merge(
            "data-grid",
            "border-collapse",
            "w-full",
            Class
        );
    }

    private string GetEmptyStateClasses()
    {
        return CssClassUtils.Merge(
            "data-grid-empty",
            "text-center",
            "p-4",
            "text-muted-foreground"
        );
    }
}
