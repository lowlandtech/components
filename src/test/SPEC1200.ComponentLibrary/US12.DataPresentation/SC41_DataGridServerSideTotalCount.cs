using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "41")]
public class SC41_DataGridServerSideTotalCount : Scenario
{
    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(Skip = "Feature not yet implemented", DisplayName = "Then the pager should show correct page numbers")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_PagerShowsCorrectPageNumbers()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when Count property with LoadData is available
        // Given I have a DataGrid with LoadData
        // When I set the Count property to the server total
        // Then the pager should show correct page numbers
    }

    [Fact(Skip = "Feature not yet implemented", DisplayName = "And paging should work with server-side data")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_PagingWorksWithServerSideData()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when server-side paging is available
        // When I set the Count property to the server total
        // Then paging should work with server-side data
    }
}
