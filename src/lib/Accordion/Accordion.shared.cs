using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Accordion;

// This file extends the auto-generated Accordion partial class.
public partial class Accordion : IDisposable
{
    [Parameter] public string Type { get; set; } = "single";
    [Parameter] public bool Collapsible { get; set; }
    [Parameter] public string? DefaultValue { get; set; }

    internal AccordionState State { get; private set; } = new();

    protected override void OnInitialized()
    {
        State.Type = Type;
        State.Collapsible = Collapsible;

        if (!string.IsNullOrEmpty(DefaultValue))
            State.SetOpen(DefaultValue);

        State.OnChange += OnStateChanged;
    }

    protected override void OnParametersSet()
    {
        State.Type = Type;
        State.Collapsible = Collapsible;
    }

    private void OnStateChanged()
    {
        InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        State.OnChange -= OnStateChanged;
    }
}
