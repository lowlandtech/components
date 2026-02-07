using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Command;

public partial class Command : ComponentBase
{
    /// <summary>
    /// Child content for the command component.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Additional CSS class for the command root element.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to be applied to the command root element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Whether the command is in loading state.
    /// </summary>
    [Parameter]
    public bool IsLoading { get; set; }

    /// <summary>
    /// The current search/filter value.
    /// </summary>
    internal string FilterValue { get; set; } = string.Empty;

    /// <summary>
    /// List of registered command items.
    /// </summary>
    internal List<CommandItem> Items { get; } = new();

    /// <summary>
    /// Currently selected/focused item index.
    /// </summary>
    internal int SelectedIndex { get; set; } = -1;

    private string RootClass => CssClassUtils.Merge("command", Class);

    /// <summary>
    /// Register a command item with the parent command.
    /// </summary>
    internal void RegisterItem(CommandItem item)
    {
        Items.Add(item);
        StateHasChanged();
    }

    /// <summary>
    /// Unregister a command item from the parent command.
    /// </summary>
    internal void UnregisterItem(CommandItem item)
    {
        Items.Remove(item);
        StateHasChanged();
    }

    /// <summary>
    /// Update the filter value from the input.
    /// </summary>
    internal void UpdateFilter(string value)
    {
        FilterValue = value;
        SelectedIndex = -1; // Reset selection when filter changes
        StateHasChanged();
    }

    /// <summary>
    /// Handle keyboard navigation.
    /// </summary>
    internal async Task HandleKeyDownAsync(KeyboardEventArgs e)
    {
        var visibleItems = GetVisibleItems();
        
        switch (e.Key)
        {
            case "ArrowDown":
                if (visibleItems.Count > 0)
                {
                    SelectedIndex = (SelectedIndex + 1) % visibleItems.Count;
                    StateHasChanged();
                }
                break;
                
            case "ArrowUp":
                if (visibleItems.Count > 0)
                {
                    SelectedIndex = SelectedIndex <= 0 ? visibleItems.Count - 1 : SelectedIndex - 1;
                    StateHasChanged();
                }
                break;
                
            case "Enter":
                if (SelectedIndex >= 0 && SelectedIndex < visibleItems.Count)
                {
                    var selectedItem = visibleItems[SelectedIndex];
                    await selectedItem.InvokeSelectAsync();
                }
                break;
                
            case "Escape":
                FilterValue = string.Empty;
                SelectedIndex = -1;
                StateHasChanged();
                break;
        }
    }

    /// <summary>
    /// Get items that match the current filter.
    /// </summary>
    internal List<CommandItem> GetVisibleItems()
    {
        if (string.IsNullOrWhiteSpace(FilterValue))
        {
            return Items;
        }

        return Items
            .Where(item => item.Label?.Contains(FilterValue, StringComparison.OrdinalIgnoreCase) ?? false)
            .ToList();
    }
}
