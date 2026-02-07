using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Combobox;

/// <summary>
/// Combobox component that combines an input with a selectable list.
/// Supports filtering items based on user input.
/// Based on shadcn/ui Combobox component with dark theme styling.
/// </summary>
public partial class Combobox : ComponentBase
{
    /// <summary>
    /// The content to render inside the combobox (ComboboxItem children).
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// The currently selected value.
    /// </summary>
    [Parameter]
    public string? Value { get; set; }

    /// <summary>
    /// Event callback for when the selected value changes.
    /// </summary>
    [Parameter]
    public EventCallback<string?> ValueChanged { get; set; }

    /// <summary>
    /// Placeholder text to display in the input.
    /// </summary>
    [Parameter]
    public string? Placeholder { get; set; } = "Search...";

    /// <summary>
    /// Whether the combobox is disabled.
    /// </summary>
    [Parameter]
    public bool IsDisabled { get; set; }

    /// <summary>
    /// Content to display when no items match the search.
    /// </summary>
    [Parameter]
    public RenderFragment? EmptyContent { get; set; }

    /// <summary>
    /// Optional CSS classes to apply to the combobox.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to apply to the combobox element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    internal List<ComboboxItem> Items { get; } = new();

    private string SearchText { get; set; } = string.Empty;
    private bool IsOpen { get; set; }
    private List<ComboboxItem> FilteredItems => GetFilteredItems();

    private string GetClasses()
    {
        return CssClassUtils.Merge("combobox", Class);
    }

    internal void RegisterItem(ComboboxItem item)
    {
        if (!Items.Contains(item))
        {
            Items.Add(item);
            StateHasChanged();
        }
    }

    internal void UnregisterItem(ComboboxItem item)
    {
        Items.Remove(item);
        StateHasChanged();
    }

    private List<ComboboxItem> GetFilteredItems()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            return Items;
        }

        var searchLower = SearchText.ToLower();
        return Items.Where(item => 
            item.Label?.ToLower().Contains(searchLower) == true).ToList();
    }

    private async Task OnSearchInputAsync(ChangeEventArgs e)
    {
        SearchText = e.Value?.ToString() ?? string.Empty;
        IsOpen = true;
        await InvokeAsync(StateHasChanged);
    }

    private async Task OnFocusAsync()
    {
        IsOpen = true;
        await InvokeAsync(StateHasChanged);
    }

    private async Task OnBlurAsync()
    {
        // Delay to allow item click to register
        await Task.Delay(200);
        IsOpen = false;
        await InvokeAsync(StateHasChanged);
    }

    private async Task SelectItemAsync(ComboboxItem item)
    {
        if (Value != item.Value)
        {
            Value = item.Value;
            await ValueChanged.InvokeAsync(Value);
        }

        SearchText = item.Label ?? string.Empty;
        IsOpen = false;
        await InvokeAsync(StateHasChanged);
    }
}
