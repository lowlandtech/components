namespace LowlandTech.Components;

/// <summary>
/// A single side-effect queued in the session outbox.
/// </summary>
/// <param name="Type">Discriminator describing the kind of effect.</param>
/// <param name="Payload">JSON-serializable payload for the effect.</param>
/// <param name="CreatedAt">Timestamp when the effect was appended.</param>
/// <param name="TypeInfo">
/// The generated JSON contract of <paramref name="Payload"/>, supplied by a host that
/// publishes a payload type the packages cannot name. A trimmed host passes it so the
/// payload is written through the generated contract instead of the reflection path.
/// </param>
public sealed record SessionOutboxEntry(
    string Type,
    object Payload,
    DateTimeOffset CreatedAt,
    System.Text.Json.Serialization.Metadata.JsonTypeInfo? TypeInfo = null);
