using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Components.Collapsible;

/// <summary>
/// A component that provides a collapsible/expandable content region with trigger and content areas.
/// </summary>
public class CollapsibleBase : ComponentBase
{
    private bool _isOpen;

    /// <summary>
    /// Gets or sets the child content to be rendered inside the collapsible.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes to apply to the collapsible wrapper.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets whether the collapsible is open (controlled mode).
    /// When null, the component manages its own state.
    /// </summary>
    [Parameter]
    public bool? Open { get; set; }

    /// <summary>
    /// Gets or sets the default open state when in uncontrolled mode.
    /// </summary>
    [Parameter]
    public bool DefaultOpen { get; set; } = false;

    /// <summary>
    /// Event fired when the open state changes.
    /// </summary>
    [Parameter]
    public EventCallback<bool> OpenChanged { get; set; }

    /// <summary>
    /// Gets or sets any additional attributes to apply to the root element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Gets whether the collapsible is currently open.
    /// </summary>
    public bool IsOpen => Open ?? _isOpen;

    protected override void OnInitialized()
    {
        // Initialize internal state if not controlled
        if (!Open.HasValue)
        {
            _isOpen = DefaultOpen;
        }
    }

    /// <summary>
    /// Toggles the open state of the collapsible.
    /// </summary>
    public async Task Toggle()
    {
        var newState = !IsOpen;
        
        if (Open.HasValue)
        {
            // Controlled mode - notify parent
            await OpenChanged.InvokeAsync(newState);
        }
        else
        {
            // Uncontrolled mode - manage internal state
            _isOpen = newState;
            await OpenChanged.InvokeAsync(_isOpen);
        }

        StateHasChanged();
    }

    /// <summary>
    /// Gets the combined CSS classes for the collapsible wrapper.
    /// </summary>
    protected string GetCssClasses()
    {
        var classes = "collapsible";
        
        if (IsOpen)
        {
            classes += " collapsible-open";
        }
        
        if (!string.IsNullOrWhiteSpace(Class))
        {
            classes += $" {Class}";
        }

        return classes;
    }
}
