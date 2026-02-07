using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "46")]
public class SC46_DataGridVirtualizationOverscan : Scenario
{
    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(Skip = "Feature not yet implemented", DisplayName = "Then that many extra rows should render outside viewport")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_ExtraRowsRenderOutsideViewport()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when VirtualizationOverscanCount property is available
        // Given I have a DataGrid with virtualization
        // When I set VirtualizationOverscanCount
        // Then that many extra rows should render outside viewport
    }

    [Fact(Skip = "Feature not yet implemented", DisplayName = "And scrolling should feel smoother")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_ScrollingFeelsSmoother()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when overscan smoothness can be tested
        // When I set VirtualizationOverscanCount
        // Then scrolling should feel smoother
    }
}
