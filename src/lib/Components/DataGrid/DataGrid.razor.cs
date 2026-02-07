using Microsoft.AspNetCore.Components;
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
    /// Handles column header click for sorting.
    /// </summary>
    internal void HandleColumnHeaderClick(DataGridColumn<TItem> column)
    {
        if (!AllowSorting || !column.Sortable)
            return;

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

        StateHasChanged();
    }

    /// <summary>
    /// Gets the sort direction for a column.
    /// </summary>
    internal SortDirection GetColumnSortDirection(DataGridColumn<TItem> column)
    {
        return _sortColumn == column ? _sortDirection : SortDirection.None;
    }

    /// <summary>
    /// Gets the data with sorting applied.
    /// </summary>
    private IEnumerable<TItem> GetSortedData()
    {
        if (Data == null)
            return Enumerable.Empty<TItem>();

        if (_sortColumn == null || _sortDirection == SortDirection.None)
            return Data;

        var propertyInfo = _sortColumn.GetPropertyInfo();
        if (propertyInfo == null)
            return Data;

        return _sortDirection == SortDirection.Ascending
            ? Data.OrderBy(item => propertyInfo.GetValue(item))
            : Data.OrderByDescending(item => propertyInfo.GetValue(item));
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
