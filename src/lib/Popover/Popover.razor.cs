namespace LowlandTech.Components;

public partial class Popover : TinyComponentBase
{
    private bool _previousIsOpen;

    /// <summary>
    /// Whether the popover is open.
    /// </summary>
    [Parameter]
    public bool IsOpen { get; set; }

    /// <summary>
    /// Event callback when IsOpen changes.
    /// </summary>
    [Parameter]
    public EventCallback<bool> IsOpenChanged { get; set; }

    /// <summary>
    /// Alias for IsOpen for backward compatibility.
    /// </summary>
    [Parameter]
    public bool Open
    {
        get => IsOpen;
        set => IsOpen = value;
    }

    /// <summary>
    /// The placement of the popover content.
    /// </summary>
    [Parameter] public string? Placement { get; set; }

    /// <summary>
    /// Additional CSS class for the popover content.
    /// </summary>
    [Parameter] public string? PopoverClass { get; set; }

    /// <summary>
    /// Whether to render the popover in modal mode (traps focus).
    /// </summary>
    [Parameter] public bool Modal { get; set; }

    /// <summary>
    /// Whether to render the popover in a portal (outside parent DOM).
    /// </summary>
    [Parameter] public bool Portal { get; set; }

    /// <summary>
    /// The trigger element reference for focus restoration.
    /// </summary>
    internal ElementReference TriggerElement { get; set; }

    /// <summary>
    /// Tracks the previous IsOpen state for focus restoration.
    /// </summary>
    internal bool PreviousIsOpen => _previousIsOpen;

    /// <summary>
    /// Toggles the popover.
    /// </summary>
    internal async Task Toggle()
    {
        _previousIsOpen = IsOpen;
        IsOpen = !IsOpen;
        await IsOpenChanged.InvokeAsync(IsOpen);
        StateHasChanged();
    }

    /// <summary>
    /// Closes the popover.
    /// </summary>
    internal async Task Close()
    {
        _previousIsOpen = IsOpen;
        IsOpen = false;
        await IsOpenChanged.InvokeAsync(IsOpen);
        StateHasChanged();
    }

    /// <summary>
    /// Opens the popover.
    /// </summary>
    internal async Task OpenPopover()
    {
        _previousIsOpen = IsOpen;
        IsOpen = true;
        await IsOpenChanged.InvokeAsync(IsOpen);
        StateHasChanged();
    }
}
