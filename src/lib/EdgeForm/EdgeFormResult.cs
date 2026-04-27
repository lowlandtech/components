namespace LowlandTech.Components.EdgeForm;

public record EdgeFormResult(
    bool IsValid,
    IReadOnlyDictionary<string, object?> Values,
    IReadOnlyList<string> Errors);
