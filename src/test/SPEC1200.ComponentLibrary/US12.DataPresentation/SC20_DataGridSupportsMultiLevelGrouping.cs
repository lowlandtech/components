using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "20")]
public class SC20_DataGridSupportsMultiLevelGrouping : Scenario
{
    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(Skip = "Feature not yet implemented", DisplayName = "Then the data should group hierarchically")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_DataGroupsHierarchically()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when multi-level grouping feature is available
        // Given I have a DataGrid with AllowGrouping enabled
        // When I drag multiple columns to the group panel
        // Then the data should group hierarchically
    }

    [Fact(Skip = "Feature not yet implemented", DisplayName = "And I should be able to expand/collapse each group level")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_CanExpandCollapseEachGroupLevel()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when expand/collapse group feature is available
        // When I drag multiple columns to the group panel
        // Then I should be able to expand/collapse each group level
    }
}
