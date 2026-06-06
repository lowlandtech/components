namespace LowlandTech.Components.Core.Validation;

/// <summary>
/// Describes validation rules and metadata for a single form field.
/// Part of the DRD (Descriptor, Registry, Discovery) pattern.
/// </summary>
public interface IValidatorDescription
{
    string FieldName { get; }
    IReadOnlyList<ValidationRule> Rules { get; }
    string? Mask { get; }
    int? MaxLength { get; }
    string? Pattern { get; }
    string? ErrorMessage { get; }
}

/// <summary>
/// A single validation rule within a validator description.
/// </summary>
public sealed record ValidationRule
(
    string Name,
    string Condition,
    string Message,
    string Severity = "error"
);
