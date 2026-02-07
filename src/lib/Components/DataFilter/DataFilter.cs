using LowlandTech.Components.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Text.Json;

namespace LowlandTech.Components.Components.DataFilter;

/// <summary>
/// Represents a filter field definition for the DataFilter component.
/// </summary>
public record FilterField
{
    /// <summary>
    /// Gets or sets the field name (property name to filter on).
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the display label for the field.
    /// </summary>
    public string Label { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the data type of the field.
    /// </summary>
    public FilterFieldType Type { get; init; } = FilterFieldType.Text;

    /// <summary>
    /// Gets or sets the available operators for this field.
    /// </summary>
    public FilterOperator[] Operators { get; init; } = Array.Empty<FilterOperator>();

    /// <summary>
    /// Gets or sets the available options for select-based filters.
    /// </summary>
    public FilterOption[]? Options { get; init; }
}

/// <summary>
/// Represents a filter option for select-based filters.
/// </summary>
public record FilterOption
{
    /// <summary>
    /// Gets or sets the option value.
    /// </summary>
    public object Value { get; init; } = null!;

    /// <summary>
    /// Gets or sets the option label.
    /// </summary>
    public string Label { get; init; } = string.Empty;
}

/// <summary>
/// Represents the data type of a filter field.
/// </summary>
public enum FilterFieldType
{
    Text,
    Number,
    Date,
    Select
}

/// <summary>
/// Represents filter operators.
/// </summary>
public enum FilterOperator
{
    Contains,
    NotContains,
    Equals,
    NotEquals,
    StartsWith,
    EndsWith,
    GreaterThan,
    LessThan,
    GreaterThanOrEqual,
    LessThanOrEqual,
    Before,
    After,
    Between,
    IsEmpty,
    IsNotEmpty
}

/// <summary>
/// Represents a filter condition.
/// </summary>
public record FilterCondition
{
    /// <summary>
    /// Gets or sets the field name.
    /// </summary>
    public string FieldName { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the filter operator.
    /// </summary>
    public FilterOperator Operator { get; init; } = FilterOperator.Equals;

    /// <summary>
    /// Gets or sets the filter value(s).
    /// </summary>
    public object? Value { get; init; }

    /// <summary>
    /// Gets or sets the second value for operators like Between.
    /// </summary>
    public object? Value2 { get; init; }

    /// <summary>
    /// Gets or sets the logical operator to combine with previous condition.
    /// </summary>
    public LogicalOperator LogicalOperator { get; init; } = LogicalOperator.And;
}

/// <summary>
/// Represents logical operators for combining filter conditions.
/// </summary>
public enum LogicalOperator
{
    And,
    Or
}

/// <summary>
/// Dynamic query builder for filtering data.
/// Supports multiple filter types, operators, and logical combinations.
/// </summary>
public partial class DataFilter : ComponentBase
{
    /// <summary>
    /// Gets or sets the available filter field definitions.
    /// </summary>
    [Parameter]
    public FilterField[] Fields { get; set; } = Array.Empty<FilterField>();

    /// <summary>
    /// Gets or sets the current filter conditions.
    /// </summary>
    [Parameter]
    public FilterCondition[] Conditions { get; set; } = Array.Empty<FilterCondition>();

    /// <summary>
    /// Gets or sets the callback invoked when conditions change.
    /// </summary>
    [Parameter]
    public EventCallback<FilterCondition[]> ConditionsChanged { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when apply is clicked.
    /// </summary>
    [Parameter]
    public EventCallback<FilterCondition[]> OnApply { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when reset is clicked.
    /// </summary>
    [Parameter]
    public EventCallback OnReset { get; set; }

    /// <summary>
    /// Gets or sets the CSS class for the component.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets any additional attributes to apply.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Gets the computed CSS class.
    /// </summary>
    protected string CssClass => CssClassUtils.Merge("data-filter", Class);

    /// <summary>
    /// Adds a new filter condition.
    /// </summary>
    public void AddCondition()
    {
        if (Fields.Length == 0) return;

        var field = Fields[0];
        var newConditions = Conditions.ToList();
        newConditions.Add(new FilterCondition
        {
            FieldName = field.Name,
            Operator = field.Operators.Length > 0 ? field.Operators[0] : FilterOperator.Equals
        });
        Conditions = newConditions.ToArray();
        ConditionsChanged.InvokeAsync(Conditions);
    }

    /// <summary>
    /// Removes a filter condition at the specified index.
    /// </summary>
    public void RemoveCondition(int index)
    {
        if (index < 0 || index >= Conditions.Length) return;

        var newConditions = Conditions.ToList();
        newConditions.RemoveAt(index);
        Conditions = newConditions.ToArray();
        ConditionsChanged.InvokeAsync(Conditions);
    }

    /// <summary>
    /// Updates a filter condition.
    /// </summary>
    public void UpdateCondition(int index, string? fieldName = null, FilterOperator? op = null, object? value = null, object? value2 = null, LogicalOperator? logicalOperator = null)
    {
        if (index < 0 || index >= Conditions.Length) return;

        var existing = Conditions[index];
        var updated = new FilterCondition
        {
            FieldName = fieldName ?? existing.FieldName,
            Operator = op ?? existing.Operator,
            Value = value ?? existing.Value,
            Value2 = value2 ?? existing.Value2,
            LogicalOperator = logicalOperator ?? existing.LogicalOperator
        };
        var newConditions = Conditions.ToArray();
        newConditions[index] = updated;
        Conditions = newConditions;
        ConditionsChanged.InvokeAsync(Conditions);
    }

    /// <summary>
    /// Resets all filter conditions.
    /// </summary>
    public void Reset()
    {
        Conditions = Array.Empty<FilterCondition>();
        ConditionsChanged.InvokeAsync(Conditions);
        OnReset.InvokeAsync();
    }

    /// <summary>
    /// Applies the current filter conditions.
    /// </summary>
    public async Task ApplyAsync()
    {
        await OnApply.InvokeAsync(Conditions);
    }

    /// <summary>
    /// Gets the field definition for a given field name.
    /// </summary>
    private FilterField? GetField(string fieldName)
    {
        return Fields.FirstOrDefault(f => f.Name == fieldName);
    }

    /// <summary>
    /// Gets the default operators for a filter type.
    /// </summary>
    private static FilterOperator[] GetDefaultOperators(FilterFieldType type) => type switch
    {
        FilterFieldType.Text => new[]
        {
            FilterOperator.Contains,
            FilterOperator.NotContains,
            FilterOperator.Equals,
            FilterOperator.NotEquals,
            FilterOperator.StartsWith,
            FilterOperator.EndsWith,
            FilterOperator.IsEmpty,
            FilterOperator.IsNotEmpty
        },
        FilterFieldType.Number => new[]
        {
            FilterOperator.Equals,
            FilterOperator.NotEquals,
            FilterOperator.GreaterThan,
            FilterOperator.LessThan,
            FilterOperator.GreaterThanOrEqual,
            FilterOperator.LessThanOrEqual
        },
        FilterFieldType.Date => new[]
        {
            FilterOperator.Before,
            FilterOperator.After,
            FilterOperator.Between
        },
        FilterFieldType.Select => new[] { FilterOperator.Equals },
        _ => Array.Empty<FilterOperator>()
    };

    /// <summary>
    /// Determines if the value input should be shown for the given operator.
    /// </summary>
    private static bool ShouldShowValueInput(FilterField? field, FilterOperator op) => op switch
    {
        FilterOperator.IsEmpty => false,
        FilterOperator.IsNotEmpty => false,
        _ => true
    };

    /// <summary>
    /// Gets the display label for a filter operator.
    /// </summary>
    private static string GetOperatorLabel(FilterOperator op) => op switch
    {
        FilterOperator.Contains => "Contains",
        FilterOperator.NotContains => "Not Contains",
        FilterOperator.Equals => "Equals",
        FilterOperator.NotEquals => "Not Equals",
        FilterOperator.StartsWith => "Starts With",
        FilterOperator.EndsWith => "Ends With",
        FilterOperator.GreaterThan => "Greater Than",
        FilterOperator.LessThan => "Less Than",
        FilterOperator.GreaterThanOrEqual => "Greater Or Equal",
        FilterOperator.LessThanOrEqual => "Less Or Equal",
        FilterOperator.Before => "Before",
        FilterOperator.After => "After",
        FilterOperator.Between => "Between",
        FilterOperator.IsEmpty => "Is Empty",
        FilterOperator.IsNotEmpty => "Is Not Empty",
        _ => op.ToString()
    };

    private void HandleLogicalChange(int index, ChangeEventArgs args)
    {
        var value = args.Value?.ToString() ?? LogicalOperator.And.ToString();
        if (!Enum.TryParse<LogicalOperator>(value, out var logicalOperator))
            logicalOperator = LogicalOperator.And;

        UpdateCondition(index, logicalOperator: logicalOperator);
    }

    private void HandleFieldChange(int index, ChangeEventArgs args)
    {
        var value = args.Value?.ToString();
        UpdateCondition(index, fieldName: value);
    }

    private void HandleOperatorChange(int index, ChangeEventArgs args)
    {
        var value = args.Value?.ToString() ?? FilterOperator.Equals.ToString();
        if (!Enum.TryParse<FilterOperator>(value, out var filterOperator))
            filterOperator = FilterOperator.Equals;

        UpdateCondition(index, op: filterOperator);
    }

    private void HandleValueChange(int index, ChangeEventArgs args)
    {
        UpdateCondition(index, value: args.Value);
    }

    private void HandleValue2Change(int index, ChangeEventArgs args)
    {
        UpdateCondition(index, value2: args.Value);
    }
}

/// <summary>
/// Helper methods for the DataFilter component.
/// </summary>
internal static class FilterHelpers
{
    /// <summary>
    /// Determines if two objects are equal.
    /// </summary>
    internal static bool IsEqual(object? a, object? b)
    {
        if (a == null && b == null) return true;
        if (a == null || b == null) return false;
        return a.Equals(b);
    }
}
