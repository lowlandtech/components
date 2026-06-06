namespace LowlandTech.Components.Core.Validation;

/// <summary>
/// Describes the behavior, component type, and validation for a form field.
/// Part of the DRD (Descriptor, Registry, Discovery) pattern.
/// </summary>
public interface IFormFieldBehavior
{
    FormFieldType FieldType { get; }
    Type ComponentType { get; }
    IValidatorDescription? Validator { get; }
    string? Mask { get; }
    IReadOnlyDictionary<string, string> EventMap { get; }
}
