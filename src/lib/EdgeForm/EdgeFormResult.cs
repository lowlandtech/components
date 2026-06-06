namespace LowlandTech.Components;

public record EdgeFormResult(
    bool IsValid,
    IReadOnlyDictionary<string, object?> Values,
    IReadOnlyList<string> Errors);
