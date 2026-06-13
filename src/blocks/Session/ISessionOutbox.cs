namespace LowlandTech.Components;

/// <summary>
/// A simple outbox that collects side-effects produced by session actions.
/// Effects are appended by handlers and can be inspected in the UI (e.g.
/// the session settings outbox tab) or drained by a processor.
/// </summary>
public interface ISessionOutbox
{
    IReadOnlyList<SessionOutboxEntry> Entries { get; }

    event Action? Changed;

    void Append(SessionOutboxEntry entry);

    void Clear();
}
