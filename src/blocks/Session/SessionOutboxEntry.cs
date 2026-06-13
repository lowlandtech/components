namespace LowlandTech.Components;

/// <summary>
/// A single side-effect queued in the session outbox.
/// </summary>
/// <param name="Type">Discriminator describing the kind of effect.</param>
/// <param name="Payload">JSON-serializable payload for the effect.</param>
/// <param name="CreatedAt">Timestamp when the effect was appended.</param>
public sealed record SessionOutboxEntry(string Type, object Payload, DateTimeOffset CreatedAt);
