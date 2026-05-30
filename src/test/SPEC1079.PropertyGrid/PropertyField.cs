namespace LowlandTech.Components.Tests.SPEC1079.PropertyGrid;

public sealed record PropertyField(string Label, string Value, string? Badge = null, bool IsReadOnly = false)
{
    public static IReadOnlyList<PropertyField> Sample { get; } =
    [
        new("Name", "Ada Lovelace"),
        new("Email", "ada@example.com", Badge: "user"),
        new("ID", "usr-1042", IsReadOnly: true)
    ];
}
