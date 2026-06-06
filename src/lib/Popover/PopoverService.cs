using System.Collections.Concurrent;

namespace LowlandTech.Components;

public class PopoverService : IPopoverService
{
    private readonly ConcurrentDictionary<Guid, PopoverReference> _popovers = new();

    public IReadOnlyCollection<PopoverReference> ActivePopovers => _popovers.Values.ToList();

    public event Action? OnChanged;

    public PopoverReference Register(Guid id)
    {
        var reference = new PopoverReference(id);
        _popovers.TryAdd(id, reference);
        OnChanged?.Invoke();
        return reference;
    }

    public void Update(Guid id, PopoverState state)
    {
        if (_popovers.TryGetValue(id, out var reference))
        {
            reference.State = state;
            OnChanged?.Invoke();
        }
    }

    public void Unregister(Guid id)
    {
        _popovers.TryRemove(id, out _);
        OnChanged?.Invoke();
    }
}
