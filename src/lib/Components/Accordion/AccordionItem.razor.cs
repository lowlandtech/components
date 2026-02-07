using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Components.Accordion;
using System.Collections.Generic;

namespace LowlandTech.Components.Components.Accordion;

public partial class AccordionItem : ComponentBase
{
    [CascadingParameter] private Accordion? ParentAccordion { get; set; }
    [Parameter] public string? Value { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public EventCallback<string> OnToggle { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }

    private bool _isOpen;

    protected override void OnInitialized()
    {
        _isOpen = ParentAccordion != null && ParentAccordion.IsItemVisible(Value ?? string.Empty);
    }

    public async Task HandleClick()
    {
        if (!Disabled)
        {
            _isOpen = !_isOpen;
            if (ParentAccordion != null && Value != null)
            {
                await ParentAccordion.HandleToggle(Value, _isOpen);
            }
            if (OnToggle.HasDelegate && Value != null)
            {
                await OnToggle.InvokeAsync(Value);
            }
        }
    }

    public bool IsVisible => ParentAccordion != null && ParentAccordion.IsItemVisible(Value ?? string.Empty);

    public bool IsOpen => ParentAccordion != null && ParentAccordion.IsOpen(Value ?? string.Empty);
}
