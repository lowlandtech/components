using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Components.Accordion;
using System.Collections.Generic;

namespace LowlandTech.Components.Components.Accordion;

public partial class AccordionContent : ComponentBase
{
    [CascadingParameter] private AccordionItem? ParentItem { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }

    private bool IsOpen => ParentItem != null && ParentItem.IsOpen;
}
