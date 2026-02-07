using Bunit;
using LowlandTech.Components.Components.DataGrid;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

/// <summary>
/// SC14: DataGrid supports page size selection
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "14")]
public class SC14_DataGridSupportsPageSizeSelection : Scenario
{
    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(Skip = "Page size selection not yet implemented", DisplayName = "Then the grid should display that many rows per page")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_DisplaysSelectedPageSize()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when page size selection is available
    }

    [Fact(Skip = "Page size selection not yet implemented", DisplayName = "And the pager should recalculate total pages")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_PagerRecalculatesTotalPages()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when page size selection is available
    }
}
