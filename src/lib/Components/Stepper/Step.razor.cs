using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Stepper;

/// <summary>
/// An individual step within a stepper component.
/// Based on MudBlazor Step component.
/// </summary>
public partial class Step : ComponentBase
{
    private int _stepIndex;
    private static int _nextStepId = 0;

    [CascadingParameter]
    private Stepper? ParentStepper { get; set; }

    /// <summary>
    /// The content to display in the step.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Additional CSS classes to apply to the step.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional HTML attributes to apply to the step.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// The label for the step.
    /// </summary>
    [Parameter]
    public string? Label { get; set; }

    /// <summary>
    /// Whether the step is disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Whether the step is completed.
    /// </summary>
    [Parameter]
    public bool Completed { get; set; }

    protected override void OnInitialized()
    {
        _stepIndex = _nextStepId++;
    }

    private string GetClasses()
    {
        var classes = new List<string> { "step" };

        if (ParentStepper != null)
        {
            var currentIndex = ParentStepper.GetCurrentIndex();
            if (_stepIndex == currentIndex)
                classes.Add("step-active");
        }

        if (Completed)
            classes.Add("step-complete");

        if (Disabled)
            classes.Add("step-disabled");

        return CssClassUtils.Merge(string.Join(" ", classes), Class);
    }
}
