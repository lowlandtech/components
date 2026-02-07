using Bunit;
using LowlandTech.Components.Components.DataGrid;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

/// <summary>
/// SC12: DataGrid supports client-side paging
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "12")]
public class SC12_DataGridSupportsClientSidePaging : Scenario
{
    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(Skip = "Paging not yet implemented", DisplayName = "Then only 10 rows should be visible")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_OnlyPageSizeRowsVisible()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when paging is available
    }

    [Fact(Skip = "Paging not yet implemented", DisplayName = "And a pager should appear showing page info")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_PagerShowsPageInfo()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when paging is available
    }

    [Fact(Skip = "Paging not yet implemented", DisplayName = "Then rows 11-20 should be visible after clicking next")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_NextPageShowsCorrectRows()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when paging is available
    }
}
