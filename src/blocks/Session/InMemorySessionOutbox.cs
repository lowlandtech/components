namespace LowlandTech.Components;

/// <summary>
/// In-memory outbox for tests and single-process scenarios.
/// </summary>
public sealed class InMemorySessionOutbox : ISessionOutbox
{
    private readonly List<SessionOutboxEntry> _entries = [];

    public IReadOnlyList<SessionOutboxEntry> Entries => _entries;

    public event Action? Changed;

    public void Append(SessionOutboxEntry entry)
    {
        _entries.Add(entry);
        Changed?.Invoke();
    }

    public void Clear()
    {
        if (_entries.Count == 0)
            return;

        _entries.Clear();
        Changed?.Invoke();
    }
}
