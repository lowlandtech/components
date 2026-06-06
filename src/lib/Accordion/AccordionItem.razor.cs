namespace LowlandTech.Components;

public partial class AccordionItem : TinyComponentBase, IDisposable
{
}

public class AccordionItemContext(AccordionItem item, Accordion? parentAccordion = null)
{
    public string? Value => item.Value;
    public bool Disabled => item.Disabled;
    public AccordionState? AccordionState => item.AccordionState;
    public bool IsOpen => AccordionState?.IsOpen(Value ?? "") ?? false;
    public string TriggerId => item.TriggerId;
    public string ContentId => item.ContentId;
    public AccordionItem Item => item;
    public Accordion? ParentAccordion => parentAccordion;
}
