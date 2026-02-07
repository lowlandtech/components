using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "36")]
public class SC36_DataGridColumnVisibility : Scenario
{
    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(Skip = "Feature not yet implemented", DisplayName = "Then that column should not render")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_ColumnDoesNotRender()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when Visible property feature is available
        // Given I have columns with Visible property
        // When I set a column's Visible to false
        // Then that column should not render
    }

    [Fact(Skip = "Feature not yet implemented", DisplayName = "And I can toggle visibility programmatically")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_CanToggleVisibilityProgrammatically()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when programmatic visibility toggle is available
        // When I set a column's Visible to false
        // Then I can toggle visibility programmatically
    }
}
