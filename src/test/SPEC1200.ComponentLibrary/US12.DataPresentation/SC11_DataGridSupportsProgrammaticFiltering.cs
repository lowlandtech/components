using Bunit;
using LowlandTech.Components.Components.DataGrid;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

/// <summary>
/// SC11: DataGrid supports programmatic filtering
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "11")]
public class SC11_DataGridSupportsProgrammaticFiltering : Scenario
{
    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(Skip = "Programmatic filtering not yet implemented", DisplayName = "Then the grid should apply the filters")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_GridAppliesFilters()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when programmatic filtering is available
    }

    [Fact(Skip = "Programmatic filtering not yet implemented", DisplayName = "And the filter UI should reflect the current filters")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_FilterUIReflectsCurrentFilters()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when programmatic filtering is available
    }
}
