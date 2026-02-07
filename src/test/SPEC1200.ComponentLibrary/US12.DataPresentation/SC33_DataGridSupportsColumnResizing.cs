using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "33")]
public class SC33_DataGridSupportsColumnResizing : Scenario
{
    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(Skip = "Feature not yet implemented", DisplayName = "Then the column width should adjust")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_ColumnWidthAdjusts()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when AllowColumnResize feature is available
        // Given I have a DataGrid with AllowColumnResize enabled
        // When I drag a column header border
        // Then the column width should adjust
    }

    [Fact(Skip = "Feature not yet implemented", DisplayName = "And the ColumnResized event should fire")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_ColumnResizedEventFires()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when ColumnResized event is available
        // When I drag a column header border
        // Then the ColumnResized event should fire
    }
}
