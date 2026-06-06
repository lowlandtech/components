namespace LowlandTech.Components;

/// <summary>
/// Immutable state for a DataForm managed by Store&lt;DataFormState&gt;.
/// </summary>
public sealed record DataFormState
{
    public IReadOnlyList<DataFormField> Fields { get; init; } = Array.Empty<DataFormField>();
    public IReadOnlyDictionary<string, object?> Values { get; init; } = new Dictionary<string, object?>();
    public IReadOnlyDictionary<string, IReadOnlyList<string>> Errors { get; init; } = new Dictionary<string, IReadOnlyList<string>>();
    public IReadOnlySet<string> Touched { get; init; } = new HashSet<string>();
    public bool IsValid { get; init; } = true;
    public bool IsSubmitting { get; init; }
    public bool IsDirty { get; init; }
    public object? Model { get; init; }
}
