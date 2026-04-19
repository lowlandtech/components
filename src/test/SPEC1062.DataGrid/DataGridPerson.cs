namespace LowlandTech.Components.Tests.SPEC1062.DataGrid;

public sealed record DataGridPerson(int Id, string Name, string Role)
{
    public static IReadOnlyList<DataGridPerson> Sample { get; } =
    [
        new(1, "Cora", "Engineer"),
        new(2, "Ada", "Engineer"),
        new(3, "Bea", "Designer")
    ];
}

