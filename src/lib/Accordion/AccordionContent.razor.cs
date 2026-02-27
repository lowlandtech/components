using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Accordion;

public partial class AccordionContent : LtComponentBase
{
    [CascadingParameter] public AccordionItemContext? ItemContext { get; set; }
}
