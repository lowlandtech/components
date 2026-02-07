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
    private bool _initialSortApplied = false;
    private int _currentPage = 1;
    private TItem? _selectedItem;
    private HashSet<TItem> _selectedItems = new();
    private Dictionary<string, string> _columnFilters = new();

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
    /// Gets or sets the initial sort column property name.
    /// </summary>
    [Parameter]
    public string? SortBy { get; set; }

    /// <summary>
    /// Gets or sets the initial sort direction.
    /// </summary>
    [Parameter]
    public SortDirection SortDirection { get; set; } = SortDirection.None;

    /// <summary>
    /// Gets or sets whether paging is enabled for the grid.
    /// </summary>
    [Parameter]
    public bool AllowPaging { get; set; }

    /// <summary>
    /// Gets or sets the number of items per page.
    /// </summary>
    [Parameter]
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Gets or sets the current page number (1-based).
    /// </summary>
    [Parameter]
    public int CurrentPage { get; set; } = 1;

    /// <summary>
    /// Gets or sets the position of the pager(s).
    /// </summary>
    [Parameter]
    public PagerPosition PagerPosition { get; set; } = PagerPosition.Bottom;

    /// <summary>
    /// Gets or sets the available page size options for the dropdown.
    /// </summary>
    [Parameter]
    public int[]? PageSizeOptions { get; set; }

    /// <summary>
    /// Gets or sets whether filtering is enabled for the grid.
    /// </summary>
    [Parameter]
    public bool AllowFiltering { get; set; }

    /// <summary>
    /// Gets or sets the filter mode (Simple or Advanced).
    /// </summary>
    [Parameter]
    public FilterMode FilterMode { get; set; } = FilterMode.None;

    /// <summary>
    /// Gets or sets the row selection mode.
    /// </summary>
    [Parameter]
    public SelectionMode SelectionMode { get; set; } = SelectionMode.None;

    /// <summary>
    /// Gets or sets whether to show a checkbox column for selection.
    /// </summary>
    [Parameter]
    public bool ShowSelectColumn { get; set; }

    /// <summary>
    /// Gets or sets the selected item (for single selection mode).
    /// </summary>
    [Parameter]
    public TItem? SelectedItem { get; set; }

    /// <summary>
    /// Gets or sets the event callback fired when the selected item changes.
    /// </summary>
    [Parameter]
    public EventCallback<TItem?> SelectedItemChanged { get; set; }

    /// <summary>
    /// Gets or sets the selected items (for multiple selection mode).
    /// </summary>
    [Parameter]
    public IEnumerable<TItem>? SelectedItems { get; set; }

    /// <summary>
    /// Gets or sets the event callback fired when the selected items change.
    /// </summary>
    [Parameter]
    public EventCallback<IEnumerable<TItem>> SelectedItemsChanged { get; set; }

    /// <summary>
    /// Gets or sets the event callback fired when a row is selected.
    /// </summary>
    [Parameter]
    public EventCallback<TItem> RowSelect { get; set; }

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

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        
        // Reset the flag when parameters change so sort gets applied
        if (!string.IsNullOrEmpty(SortBy) && SortDirection != SortDirection.None)
        {
            _initialSortApplied = false;
        }

        // Sync internal page state with parameter
        if (CurrentPage != _currentPage)
        {
            _currentPage = CurrentPage;
        }

        // Sync selected item/items from parameters
        if (SelectionMode == SelectionMode.Single && SelectedItem != null)
        {
            _selectedItem = SelectedItem;
            _selectedItems.Clear();
            _selectedItems.Add(SelectedItem);
        }
        else if (SelectionMode == SelectionMode.Multiple && SelectedItems != null)
        {
            _selectedItems.Clear();
            foreach (var item in SelectedItems)
            {
                _selectedItems.Add(item);
            }
            _selectedItem = _selectedItems.FirstOrDefault();
        }
    }

    /// <summary>
    /// Applies sorting based on SortBy and SortDirection parameters.
    /// </summary>
    private void ApplyProgrammaticSort()
    {
        if (string.IsNullOrEmpty(SortBy) || _initialSortApplied)
            return;

        var column = _columns.FirstOrDefault(c => c.Property == SortBy);
        if (column != null && column.Sortable)
        {
            _sortColumn = column;
            _sortDirection = SortDirection;
            _sortDescriptors.Clear();
            _initialSortApplied = true;
        }
    }

    /// <summary>
    /// Clears all sorting.
    /// </summary>
    public async Task ClearSort()
    {
        _sortColumn = null;
        _sortDirection = SortDirection.None;
        _sortDescriptors.Clear();
        await InvokeAsync(StateHasChanged);
    }

    /// <summary>
    /// Sets sorting for a specific column programmatically.
    /// </summary>
    public async Task SetSort(string propertyName, SortDirection direction)
    {
        var column = _columns.FirstOrDefault(c => c.Property == propertyName);
        if (column != null && column.Sortable)
        {
            _sortColumn = column;
            _sortDirection = direction;
            _sortDescriptors.Clear();
            await InvokeAsync(StateHasChanged);
        }
    }

    /// <summary>
    /// Adds a sort column programmatically (for multi-column sorting).
    /// </summary>
    public async Task AddSort(string propertyName, SortDirection direction)
    {
        if (!AllowMultiColumnSorting)
        {
            await SetSort(propertyName, direction);
            return;
        }

        var column = _columns.FirstOrDefault(c => c.Property == propertyName);
        if (column != null && column.Sortable)
        {
            // Migrate single-column sort if needed
            if (_sortColumn != null && _sortDirection != SortDirection.None && !_sortDescriptors.Any())
            {
                _sortDescriptors.Add(new SortDescriptor<TItem>
                {
                    Column = _sortColumn,
                    Direction = _sortDirection,
                    Priority = 1
                });
                _sortColumn = null;
                _sortDirection = SortDirection.None;
            }

            // Add or update the column
            var existing = _sortDescriptors.FirstOrDefault(s => s.Column == column);
            if (existing != null)
            {
                existing.Direction = direction;
                if (direction == SortDirection.None)
                {
                    _sortDescriptors.Remove(existing);
                    // Re-assign priorities
                    for (int i = 0; i < _sortDescriptors.Count; i++)
                    {
                        _sortDescriptors[i].Priority = i + 1;
                    }
                }
            }
            else if (direction != SortDirection.None)
            {
                _sortDescriptors.Add(new SortDescriptor<TItem>
                {
                    Column = column,
                    Direction = direction,
                    Priority = _sortDescriptors.Count + 1
                });
            }

            await InvokeAsync(StateHasChanged);
        }
    }

    /// <summary>
    /// Sets a filter programmatically.
    /// </summary>
    public async Task SetFilter(string propertyName, string value)
    {
        var column = _columns.FirstOrDefault(c => c.Property == propertyName);
        if (column != null && column.Filterable)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                _columnFilters.Remove(propertyName);
            }
            else
            {
                _columnFilters[propertyName] = value;
            }
            
            _currentPage = 1; // Reset to first page
            await InvokeAsync(StateHasChanged);
        }
    }

    /// <summary>
    /// Clears all filters.
    /// </summary>
    public async Task ClearFilters()
    {
        _columnFilters.Clear();
        _currentPage = 1;
        await InvokeAsync(StateHasChanged);
    }

    /// <summary>
    /// Gets the current filter value for a column.
    /// </summary>
    public string GetFilterValue(string propertyName)
    {
        return _columnFilters.TryGetValue(propertyName, out var value) ? value : string.Empty;
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
        // Apply programmatic sort if not yet applied
        if (!_initialSortApplied && !string.IsNullOrEmpty(SortBy) && SortDirection != SortDirection.None)
        {
            ApplyProgrammaticSort();
        }

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
    /// Gets the data with filtering applied.
    /// </summary>
    private IEnumerable<TItem> GetFilteredData()
    {
        if (Data == null)
            return Enumerable.Empty<TItem>();

        if (!AllowFiltering || !_columnFilters.Any())
            return Data;

        var filteredData = Data;

        foreach (var filter in _columnFilters)
        {
            var propertyName = filter.Key;
            var filterValue = filter.Value;

            if (string.IsNullOrWhiteSpace(filterValue))
                continue;

            var column = _columns.FirstOrDefault(c => c.Property == propertyName);
            if (column == null)
                continue;

            var propertyInfo = column.GetPropertyInfo();
            if (propertyInfo == null)
                continue;

            filteredData = filteredData.Where(item =>
            {
                var value = propertyInfo.GetValue(item);
                if (value == null)
                    return false;

                var stringValue = value.ToString() ?? string.Empty;
                return stringValue.Contains(filterValue, StringComparison.OrdinalIgnoreCase);
            });
        }

        return filteredData;
    }

    /// <summary>
    /// Gets the data with sorting applied.
    /// </summary>
    private IEnumerable<TItem> GetSortedData()
    {
        var filteredData = GetFilteredData();

        // Use multi-column sort if available
        if (_sortDescriptors.Any())
        {
            return ApplyMultiColumnSort(filteredData);
        }

        // Fall back to single-column sort
        if (_sortColumn == null || _sortDirection == SortDirection.None)
            return filteredData;

        var propertyInfo = _sortColumn.GetPropertyInfo();
        if (propertyInfo == null)
            return filteredData;

        return _sortDirection == SortDirection.Ascending
            ? filteredData.OrderBy(item => propertyInfo.GetValue(item))
            : filteredData.OrderByDescending(item => propertyInfo.GetValue(item));
    }

    /// <summary>
    /// Gets the data with sorting and paging applied.
    /// </summary>
    private IEnumerable<TItem> GetPagedData()
    {
        var sortedData = GetSortedData();

        if (!AllowPaging)
            return sortedData;

        var skip = (_currentPage - 1) * PageSize;
        return sortedData.Skip(skip).Take(PageSize);
    }

    /// <summary>
    /// Gets the total number of pages.
    /// </summary>
    private int GetTotalPages()
    {
        if (!AllowPaging || Data == null)
            return 1;

        var totalCount = Data.Count();
        return (int)Math.Ceiling((double)totalCount / PageSize);
    }

    /// <summary>
    /// Navigates to the next page.
    /// </summary>
    private void GoToNextPage()
    {
        if (_currentPage < GetTotalPages())
        {
            _currentPage++;
            StateHasChanged();
        }
    }

    /// <summary>
    /// Navigates to the previous page.
    /// </summary>
    private void GoToPreviousPage()
    {
        if (_currentPage > 1)
        {
            _currentPage--;
            StateHasChanged();
        }
    }

    /// <summary>
    /// Navigates to a specific page.
    /// </summary>
    private void GoToPage(int page)
    {
        if (page >= 1 && page <= GetTotalPages())
        {
            _currentPage = page;
            StateHasChanged();
        }
    }

    /// <summary>
    /// Handles page size change from dropdown.
    /// </summary>
    private void OnPageSizeChanged(ChangeEventArgs e)
    {
        if (int.TryParse(e.Value?.ToString(), out var newSize))
        {
            PageSize = newSize;
            _currentPage = 1; // Reset to first page
            StateHasChanged();
        }
    }

    /// <summary>
    /// Handles row click for selection.
    /// </summary>
    private async Task HandleRowClick(TItem item)
    {
        if (SelectionMode == SelectionMode.None)
            return;

        if (SelectionMode == SelectionMode.Single)
        {
            _selectedItem = item;
            _selectedItems.Clear();
            _selectedItems.Add(item);
        }
        else if (SelectionMode == SelectionMode.Multiple)
        {
            if (_selectedItems.Contains(item))
            {
                _selectedItems.Remove(item);
                if (ReferenceEquals(_selectedItem, item))
                    _selectedItem = default;
            }
            else
            {
                _selectedItems.Add(item);
                _selectedItem = item;
            }
        }

        await RowSelect.InvokeAsync(item);
        StateHasChanged();
    }

    /// <summary>
    /// Handles checkbox change for a row.
    /// </summary>
    private async Task HandleCheckboxChange(TItem item, ChangeEventArgs e)
    {
        if (e.Value is bool isChecked)
        {
            if (isChecked && !_selectedItems.Contains(item))
            {
                _selectedItems.Add(item);
                _selectedItem = item;
                await RowSelect.InvokeAsync(item);
            }
            else if (!isChecked && _selectedItems.Contains(item))
            {
                _selectedItems.Remove(item);
                if (ReferenceEquals(_selectedItem, item))
                    _selectedItem = default;
            }
            
            StateHasChanged();
        }
    }

    /// <summary>
    /// Handles select all checkbox change.
    /// </summary>
    private async Task HandleSelectAllChange(ChangeEventArgs e)
    {
        if (e.Value is bool isChecked)
        {
            if (isChecked)
            {
                // Select all items
                var allItems = GetPagedData().ToList();
                foreach (var item in allItems)
                {
                    if (!_selectedItems.Contains(item))
                    {
                        _selectedItems.Add(item);
                    }
                }
            }
            else
            {
                // Deselect all items
                _selectedItems.Clear();
                _selectedItem = default;
            }
            
            StateHasChanged();
        }
    }

    /// <summary>
    /// Checks if all visible rows are selected.
    /// </summary>
    private bool AreAllRowsSelected()
    {
        var visibleItems = GetPagedData().ToList();
        return visibleItems.Any() && visibleItems.All(item => _selectedItems.Contains(item));
    }

    /// <summary>
    /// Checks if a row is selected.
    /// </summary>
    private bool IsRowSelected(TItem item)
    {
        return _selectedItems.Contains(item);
    }

    /// <summary>
    /// Handles filter input change for a column.
    /// </summary>
    private void HandleFilterChange(string propertyName, ChangeEventArgs e)
    {
        var filterValue = e.Value?.ToString() ?? string.Empty;
        
        if (string.IsNullOrWhiteSpace(filterValue))
        {
            _columnFilters.Remove(propertyName);
        }
        else
        {
            _columnFilters[propertyName] = filterValue;
        }
        
        // Reset to first page when filtering
        _currentPage = 1;
        StateHasChanged();
    }

    /// <summary>
    /// Gets the CSS class for a row.
    /// </summary>
    private string GetRowClass(TItem item)
    {
        return IsRowSelected(item) ? "row-selected" : "";
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

    private RenderFragment RenderPager() => builder =>
    {
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", "data-grid-pager");
        
        // First button
        builder.OpenElement(2, "button");
        builder.AddAttribute(3, "class", "pager-first");
        builder.AddAttribute(4, "disabled", _currentPage == 1);
        builder.AddAttribute(5, "onclick", EventCallback.Factory.Create(this, () => GoToPage(1)));
        builder.AddContent(6, "First");
        builder.CloseElement();
        
        // Previous button
        builder.OpenElement(7, "button");
        builder.AddAttribute(8, "class", "pager-prev");
        builder.AddAttribute(9, "disabled", _currentPage == 1);
        builder.AddAttribute(10, "onclick", EventCallback.Factory.Create(this, GoToPreviousPage));
        builder.AddContent(11, "Previous");
        builder.CloseElement();
        
        // Page info
        builder.OpenElement(12, "span");
        builder.AddAttribute(13, "class", "pager-info");
        builder.AddContent(14, $"Page {_currentPage} of {GetTotalPages()}");
        builder.CloseElement();
        
        // Next button
        builder.OpenElement(15, "button");
        builder.AddAttribute(16, "class", "pager-next");
        builder.AddAttribute(17, "disabled", _currentPage == GetTotalPages());
        builder.AddAttribute(18, "onclick", EventCallback.Factory.Create(this, GoToNextPage));
        builder.AddContent(19, "Next");
        builder.CloseElement();
        
        // Last button
        builder.OpenElement(20, "button");
        builder.AddAttribute(21, "class", "pager-last");
        builder.AddAttribute(22, "disabled", _currentPage == GetTotalPages());
        builder.AddAttribute(23, "onclick", EventCallback.Factory.Create(this, () => GoToPage(GetTotalPages())));
        builder.AddContent(24, "Last");
        builder.CloseElement();
        
        // Page size selector
        if (PageSizeOptions != null && PageSizeOptions.Length > 0)
        {
            builder.OpenElement(25, "select");
            builder.AddAttribute(26, "class", "page-size-select");
            builder.AddAttribute(27, "value", PageSize);
            builder.AddAttribute(28, "onchange", EventCallback.Factory.Create<ChangeEventArgs>(this, OnPageSizeChanged));
            
            var seq = 29;
            foreach (var size in PageSizeOptions)
            {
                builder.OpenElement(seq++, "option");
                builder.AddAttribute(seq++, "value", size);
                builder.AddContent(seq++, $"{size} per page");
                builder.CloseElement();
            }
            
            builder.CloseElement(); // Close select
        }
        
        builder.CloseElement(); // Close pager div
    };
}
