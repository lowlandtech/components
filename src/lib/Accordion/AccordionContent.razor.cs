namespace LowlandTech.Components;

public partial class AccordionContent : TinyComponentBase
{
    [CascadingParameter] public AccordionItemContext? ItemContext { get; set; }
}
