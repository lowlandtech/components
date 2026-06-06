namespace LowlandTech.Components;

/// <summary>
/// Event arguments for form field value changes.
/// </summary>
public sealed class FormFieldChangeArgs : EventArgs
{
    public string FieldName { get; init; } = string.Empty;
    public object? OldValue { get; init; }
    public object? NewValue { get; init; }
    public bool IsValid { get; init; }
    public IReadOnlyList<string> Errors { get; init; } = Array.Empty<string>();
}
