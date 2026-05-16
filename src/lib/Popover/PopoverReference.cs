namespace LowlandTech.Components.Popover;

public class PopoverReference
{
    public Guid Id { get; }
    public PopoverState State { get; internal set; } = new();

    public PopoverReference(Guid id) => Id = id;
}
