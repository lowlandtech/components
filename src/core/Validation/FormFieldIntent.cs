namespace LowlandTech.Components.Core.Validation;

/// <summary>
/// Represents a dispatched form field event intent.
/// </summary>
public sealed record FormFieldIntent
(
    string FieldName,
    string EventName,
    string IntentKey,
    object? Payload = null
);
