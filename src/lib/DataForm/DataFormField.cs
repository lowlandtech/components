namespace LowlandTech.Components.DataForm;

/// <summary>
/// Represents a discovered field in a DataForm.
/// </summary>
public sealed record DataFormField(
    string PropertyName,
    string DisplayName,
    Type PropertyType,
    string InputKind,
    bool Required,
    bool ReadOnly,
    int Order);
