using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "34")]
public class SC34_DataGridSupportsColumnReordering : Scenario
{
    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(Skip = "Feature not yet implemented", DisplayName = "Then the column should move to the new position")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_ColumnMovesToNewPosition()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when AllowColumnReorder feature is available
        // Given I have a DataGrid with AllowColumnReorder enabled
        // When I drag a column header to a new position
        // Then the column should move to the new position
    }

    [Fact(Skip = "Feature not yet implemented", DisplayName = "And the ColumnReordered event should fire")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_ColumnReorderedEventFires()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when ColumnReordered event is available
        // When I drag a column header to a new position
        // Then the ColumnReordered event should fire
    }
}
