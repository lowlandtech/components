using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Accordion;

public partial class AccordionItem : LtComponentBase
{
    [CascadingParameter] public AccordionState? AccordionState { get; set; }
    [Parameter] public string? Value { get; set; }
    [Parameter] public bool Disabled { get; set; }
}

public class AccordionItemContext(AccordionItem item)
{
    public string? Value => item.Value;
    public bool Disabled => item.Disabled;
    public AccordionState? AccordionState => item.AccordionState;
    public bool IsOpen => AccordionState?.IsOpen(Value ?? "") ?? false;
}
