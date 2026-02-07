using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "47")]
public class SC47_DataGridSupportsDensityModes : Scenario
{
    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(Skip = "Feature not yet implemented", DisplayName = "Then rows should have reduced padding")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_RowsHaveReducedPadding()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when Density property is available
        // Given I have a DataGrid with Density property
        // When I set Density to Compact
        // Then rows should have reduced padding
    }

    [Fact(Skip = "Feature not yet implemented", DisplayName = "And more data should fit in the same space")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_MoreDataFitsInSameSpace()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when compact density mode is available
        // When I set Density to Compact
        // Then more data should fit in the same space
    }
}
