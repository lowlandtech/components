using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Accordion;

public partial class AccordionTrigger : LtComponentBase
{
    [CascadingParameter] public AccordionItemContext? ItemContext { get; set; }

    private void OnClick()
    {
        if (ItemContext is { Disabled: false, Value: not null })
        {
            ItemContext.AccordionState?.Toggle(ItemContext.Value);
        }
    }
}
