namespace LowlandTech.Components;

public record EdgeFormField(
    string Name,
    string Label,
    FieldType FieldType,
    bool Required = false,
    string? Placeholder = null,
    IReadOnlyList<SelectOption>? Options = null,
    int? Debounce = null);
