using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

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

    private void OnKeyDown(KeyboardEventArgs e)
    {
        if (ItemContext?.Item == null)
            return;

        if (ItemContext.Disabled)
            return;

        var accordion = ItemContext.ParentAccordion;

        switch (e.Key)
        {
            case "Enter":
            case " ":
                if (ItemContext.Value != null)
                {
                    ItemContext.AccordionState?.Toggle(ItemContext.Value);
                }
                break;
            case "ArrowDown":
                if (accordion == null)
                    return;
                FocusNextItem(accordion);
                break;
            case "ArrowUp":
                if (accordion == null)
                    return;
                FocusPreviousItem(accordion);
                break;
            case "Home":
                if (accordion == null)
                    return;
                FocusFirstItem(accordion);
                break;
            case "End":
                if (accordion == null)
                    return;
                FocusLastItem(accordion);
                break;
        }
    }

    private void FocusNextItem(Accordion accordion)
    {
        var next = accordion.GetNextItem(ItemContext!.Item);
        if (next != null)
            accordion.FocusItem(next);
    }

    private void FocusPreviousItem(Accordion accordion)
    {
        var prev = accordion.GetPreviousItem(ItemContext!.Item);
        if (prev != null)
            accordion.FocusItem(prev);
    }

    private void FocusFirstItem(Accordion accordion)
    {
        var first = accordion.GetFirstItem();
        if (first != null)
            accordion.FocusItem(first);
    }

    private void FocusLastItem(Accordion accordion)
    {
        var last = accordion.GetLastItem();
        if (last != null)
            accordion.FocusItem(last);
    }
}
