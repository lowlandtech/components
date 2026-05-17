using LowlandTech.Components.Popover;

namespace LowlandTech.Components.App.Showcode.Examples.Overlay;

public interface IPopoverService
{
    IReadOnlyCollection<PopoverReference> ActivePopovers { get; }
    event Action? OnChanged;

    PopoverReference Register(Guid id);
    void Update(Guid id, PopoverState state);
    void Unregister(Guid id);
}
