using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "49")]
public class SC49_DataGridGridLines : Scenario
{
    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(Skip = "Feature not yet implemented", DisplayName = "Then appropriate border lines should appear between cells")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_AppropriateBorderLinesAppearBetweenCells()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when GridLines property is available
        // Given I have a DataGrid with GridLines property
        // When I set GridLines to Horizontal, Vertical, or Both
        // Then appropriate border lines should appear between cells
    }

    [Fact(Skip = "Feature not yet implemented", DisplayName = "And the grid should have clear visual separation")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_GridHasClearVisualSeparation()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when grid lines feature is available
        // When I set GridLines to Horizontal, Vertical, or Both
        // Then the grid should have clear visual separation
    }
}
