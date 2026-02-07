using Microsoft.AspNetCore.Components;
using System.Linq.Expressions;
using System.Reflection;

namespace LowlandTech.Components.Components.DataGrid;

/// <summary>
/// Defines a column in a DataGrid component.
/// </summary>
/// <typeparam name="TItem">The type of data items in the grid.</typeparam>
public partial class DataGridColumn<TItem> : ComponentBase where TItem : notnull
{
    private PropertyInfo? _propertyInfo;

    [CascadingParameter]
    public DataGrid<TItem>? Grid { get; set; }

    /// <summary>
    /// Gets or sets the property name to display in this column.
    /// </summary>
    [Parameter]
    public string? Property { get; set; }

    /// <summary>
    /// Gets or sets the title/header text for the column.
    /// </summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets a custom template for rendering cell content.
    /// </summary>
    [Parameter]
    public RenderFragment<TItem>? Template { get; set; }

    /// <summary>
    /// Gets or sets the format string for the column value.
    /// </summary>
    [Parameter]
    public string? FormatString { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        
        if (Grid != null)
        {
            Grid.AddColumn(this);
        }

        if (!string.IsNullOrEmpty(Property))
        {
            _propertyInfo = typeof(TItem).GetProperty(Property);
        }
    }

    /// <summary>
    /// Gets the value for this column from the given item.
    /// </summary>
    public object? GetValue(TItem item)
    {
        if (_propertyInfo == null)
            return null;

        var value = _propertyInfo.GetValue(item);

        if (value == null)
            return null;

        if (!string.IsNullOrEmpty(FormatString))
        {
            return string.Format(FormatString, value);
        }

        return value;
    }

    public void Dispose()
    {
        Grid?.RemoveColumn(this);
    }
}
