namespace LowlandTech.Components.Core;

/// <summary>
/// Maps spec-level PropertyTypes to concrete form field types.
/// Each value corresponds to a specialized FormField component.
/// </summary>
public enum FormFieldType
{
    Text = 0,
    Number = 1,
    Decimal = 2,
    Bool = 3,
    Date = 4,
    DateTime = 5,
    Guid = 6,
    Enum = 7,
    Json = 8,
    NodeRef = 9
}
