using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "57")]
public class SC57_DataGridGroupFooters : Scenario
{
    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(Skip = "Feature not yet implemented", DisplayName = "Then each group should have a footer row")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_EachGroupHasFooterRow()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when group footers feature is available
        // Given I have a grouped DataGrid with group footers
        // When I render groups
        // Then each group should have a footer row
    }

    [Fact(Skip = "Feature not yet implemented", DisplayName = "And I can display group-level aggregates")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_CanDisplayGroupLevelAggregates()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when group-level aggregates are available
        // When I render groups
        // Then I can display group-level aggregates
    }
}
