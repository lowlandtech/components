using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.DataGrid;

/// <summary>
/// A data grid component for displaying tabular data with support for sorting, filtering, paging, and more.
/// </summary>
/// <typeparam name="TItem">The type of data items displayed in the grid.</typeparam>
public partial class DataGrid<TItem> : ComponentBase where TItem : notnull
{
    private List<DataGridColumn<TItem>> _columns = new();

    /// <summary>
    /// Gets or sets the data items to display in the grid.
    /// </summary>
    [Parameter]
    public IEnumerable<TItem>? Data { get; set; }

    /// <summary>
    /// Gets or sets the columns for the data grid.
    /// </summary>
    [Parameter]
    public RenderFragment? Columns { get; set; }

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
        }
    }

    /// <summary>
    /// Unregisters a column from the data grid.
    /// </summary>
    internal void RemoveColumn(DataGridColumn<TItem> column)
    {
        _columns.Remove(column);
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
