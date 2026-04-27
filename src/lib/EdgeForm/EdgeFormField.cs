namespace LowlandTech.Components.EdgeForm;

public record EdgeFormField(
    string Name,
    string Label,
    FieldType FieldType,
    bool Required = false,
    string? Placeholder = null,
    IReadOnlyList<SelectOption>? Options = null,
    int? Debounce = null);
