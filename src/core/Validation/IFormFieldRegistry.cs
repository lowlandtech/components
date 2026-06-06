namespace LowlandTech.Components.Core.Validation;

/// <summary>
/// Registry of form field behaviors. Part of the DRD pattern.
/// Multiple registries can be registered and queried by priority.
/// </summary>
public interface IFormFieldRegistry
{
    int Priority { get; }
    IFormFieldBehavior? GetBehavior(FormFieldType fieldType);
    IFormFieldBehavior? GetBehavior(string fieldName);
}
