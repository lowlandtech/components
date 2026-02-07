using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Components.Accordion;
using System.Collections.Generic;

namespace LowlandTech.Components.Components.Accordion;

public partial class AccordionTrigger : ComponentBase
{
    [CascadingParameter] private AccordionItem? ParentItem { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }

    private async Task HandleClick()
    {
        if (ParentItem != null)
        {
            await ParentItem.HandleClick();
        }
    }
}
