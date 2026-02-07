using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "19")]
public class SC19_DataGridSupportsInteractiveGrouping : Scenario
{
    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(Skip = "Feature not yet implemented", DisplayName = "Then a group panel should appear above the grid")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_GroupPanelAppearsAboveGrid()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when AllowGrouping feature is available
        // Given I have a DataGrid with AllowGrouping enabled
        // When I render the DataGrid
        // Then a group panel should appear above the grid
    }

    [Fact(Skip = "Feature not yet implemented", DisplayName = "And I should be able to drag column headers to the group panel")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_CanDragColumnHeadersToGroupPanel()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when column drag-to-group feature is available
        // When I drag column headers to the group panel
        // Then columns should group in the panel
    }

    [Fact(Skip = "Feature not yet implemented", DisplayName = "And the data should re-group accordingly")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_DataReGroupsAccordingly()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when data grouping feature is available
        // When I drag column headers to the group panel
        // Then the data should re-group accordingly
    }

    [Fact(Skip = "Feature not yet implemented", DisplayName = "And grouped column values should appear in group headers")]
    [Trait(Spec.UAC, "04")]
    public async Task UAC04_GroupedColumnValuesAppearInHeaders()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when group headers feature is available
        // When data is grouped
        // Then grouped column values should appear in group headers
    }
}
