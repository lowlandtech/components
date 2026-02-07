using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "32")]
public class SC32_DataGridSupportsFrozenColumns : Scenario
{
    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(Skip = "Feature not yet implemented", DisplayName = "Then frozen columns should remain visible while scrolling")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_FrozenColumnsRemainVisibleWhileScrolling()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when Frozen column feature is available
        // Given I have columns with Frozen property set
        // When I render the DataGrid with horizontal scrolling
        // Then frozen columns should remain visible while scrolling
    }

    [Fact(Skip = "Feature not yet implemented", DisplayName = "And they should stay pinned to the left side")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_FrozenColumnsStayPinnedToLeftSide()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when Frozen column pinning is available
        // When I render the DataGrid with horizontal scrolling
        // Then they should stay pinned to the left side
    }
}
