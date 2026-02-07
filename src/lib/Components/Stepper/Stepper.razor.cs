using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Stepper;

/// <summary>
/// A stepper component for multi-step workflows.
/// Based on MudBlazor Stepper component.
/// </summary>
public partial class Stepper : ComponentBase
{
    /// <summary>
    /// The steps to display.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Additional CSS classes to apply to the stepper.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional HTML attributes to apply to the stepper.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// The index of the currently active step.
    /// </summary>
    [Parameter]
    public int ActiveIndex { get; set; }

    /// <summary>
    /// Event callback when the active step index changes.
    /// </summary>
    [Parameter]
    public EventCallback<int> ActiveIndexChanged { get; set; }

    /// <summary>
    /// The default active step index.
    /// </summary>
    [Parameter]
    public int DefaultIndex { get; set; }

    /// <summary>
    /// Whether to display the stepper vertically.
    /// </summary>
    [Parameter]
    public bool Vertical { get; set; }

    /// <summary>
    /// Whether to allow non-linear navigation (jumping to any step).
    /// </summary>
    [Parameter]
    public bool NonLinear { get; set; }

    private int _currentIndex;
    private readonly List<Step> _steps = new();

    protected override void OnInitialized()
    {
        _currentIndex = ActiveIndex > 0 ? ActiveIndex : DefaultIndex;
    }

    public int GetCurrentIndex() => _currentIndex;

    public void RegisterStep(Step step)
    {
        if (!_steps.Contains(step))
        {
            _steps.Add(step);
        }
    }

    public int GetStepIndex(Step step)
    {
        return _steps.IndexOf(step);
    }

    public async Task SetActiveIndexAsync(int index)
    {
        _currentIndex = index;
        await ActiveIndexChanged.InvokeAsync(index);
    }

    private string GetClasses()
    {
        var orientation = Vertical ? "stepper-vertical" : "stepper-horizontal";
        return CssClassUtils.Merge("stepper", orientation, Class);
    }
}
