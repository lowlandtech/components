using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace LowlandTech.Components.Calendar;

/// <summary>
/// Represents a preset date option for quick selection.
/// </summary>
public class CalendarPresetItem
{
    /// <summary>
    /// The label to display on the preset button.
    /// </summary>
    public required string Label { get; set; }

    /// <summary>
    /// The date to select when this preset is clicked.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Optional icon to display before the label.
    /// </summary>
    public string? Icon { get; set; }
}

/// <summary>
/// A component that displays quick-select preset buttons for common dates.
/// </summary>
public partial class CalendarPresets
{
    /// <summary>
    /// The list of preset items to display.
    /// </summary>
    [Parameter]
    public List<CalendarPresetItem> Items { get; set; } = new();

    /// <summary>
    /// Event callback when a preset is selected.
    /// </summary>
    [Parameter]
    public EventCallback<CalendarPresetItem> OnSelect { get; set; }

    /// <summary>
    /// The variant style for preset buttons.
    /// </summary>
    [Parameter]
    public string Variant { get; set; } = "ghost";

    /// <summary>
    /// The size of preset buttons.
    /// </summary>
    [Parameter]
    public string Size { get; set; } = "sm";

    /// <summary>
    /// Orientation of the preset buttons: "horizontal" or "vertical".
    /// </summary>
    [Parameter]
    public string Orientation { get; set; } = "vertical";

    /// <summary>
    /// Custom CSS class for preset buttons.
    /// </summary>
    [Parameter]
    public string? ButtonClass { get; set; }

    private string GetContainerClass()
    {
        var baseClass = "cn-calendar-presets";
        var orientationClass = Orientation.ToLower() switch
        {
            "horizontal" => "flex flex-wrap gap-2",
            _ => "flex flex-col gap-1"
        };
        return string.IsNullOrEmpty(Class) 
            ? $"{baseClass} {orientationClass}" 
            : $"{baseClass} {orientationClass} {Class}";
    }

    private string GetButtonClass()
    {
        var baseClass = "cn-calendar-preset-button inline-flex items-center justify-start whitespace-nowrap rounded-md text-sm font-medium transition-colors focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring disabled:pointer-events-none disabled:opacity-50";
        
        var variantClass = Variant.ToLower() switch
        {
            "default" => "bg-primary text-primary-foreground shadow hover:bg-primary/90",
            "destructive" => "bg-destructive text-destructive-foreground shadow-sm hover:bg-destructive/90",
            "outline" => "border border-input bg-background shadow-sm hover:bg-accent hover:text-accent-foreground",
            "secondary" => "bg-secondary text-secondary-foreground shadow-sm hover:bg-secondary/80",
            "ghost" => "hover:bg-accent hover:text-accent-foreground",
            "link" => "text-primary underline-offset-4 hover:underline",
            _ => "hover:bg-accent hover:text-accent-foreground"
        };

        var sizeClass = Size.ToLower() switch
        {
            "default" => "h-9 px-4 py-2",
            "sm" => "h-8 px-3 text-xs",
            "lg" => "h-10 px-8",
            "icon" => "h-9 w-9",
            _ => "h-8 px-3 text-xs"
        };

        var fullClass = $"{baseClass} {variantClass} {sizeClass}";
        
        if (!string.IsNullOrEmpty(ButtonClass))
        {
            fullClass += $" {ButtonClass}";
        }

        // For vertical orientation, make buttons full width
        if (Orientation.ToLower() == "vertical")
        {
            fullClass += " w-full";
        }

        return fullClass;
    }

    private async Task HandlePresetClick(CalendarPresetItem preset)
    {
        await OnSelect.InvokeAsync(preset);
    }

    /// <summary>
    /// Gets the default preset items (Today, Tomorrow, In 3 days, In a week).
    /// </summary>
    public static List<CalendarPresetItem> GetDefaultPresets()
    {
        var today = DateTime.Today;
        return new List<CalendarPresetItem>
        {
            new() { Label = "Today", Date = today },
            new() { Label = "Tomorrow", Date = today.AddDays(1) },
            new() { Label = "In 3 days", Date = today.AddDays(3) },
            new() { Label = "In a week", Date = today.AddDays(7) }
        };
    }

    /// <summary>
    /// Gets preset items for date range selection.
    /// </summary>
    public static List<CalendarPresetItem> GetRangePresets()
    {
        var today = DateTime.Today;
        return new List<CalendarPresetItem>
        {
            new() { Label = "This week", Date = today },
            new() { Label = "Next week", Date = today.AddDays(7) },
            new() { Label = "In 2 weeks", Date = today.AddDays(14) },
            new() { Label = "Next month", Date = today.AddMonths(1) }
        };
    }
}
