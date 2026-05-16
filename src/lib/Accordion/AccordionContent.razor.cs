namespace LowlandTech.Components.Accordion;

public partial class AccordionContent : TinyComponentBase
{
    [CascadingParameter] public AccordionItemContext? ItemContext { get; set; }
}
