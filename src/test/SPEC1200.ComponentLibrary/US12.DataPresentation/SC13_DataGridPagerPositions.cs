using Bunit;
using LowlandTech.Components.Components.DataGrid;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

/// <summary>
/// SC13: DataGrid pager supports top and bottom positions
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "13")]
public class SC13_DataGridPagerPositions : Scenario
{
    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(Skip = "Pager positioning not yet implemented", DisplayName = "Then a pager should appear above the grid")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_PagerAppearsAboveGrid()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when pager positioning is available
    }

    [Fact(Skip = "Pager positioning not yet implemented", DisplayName = "And a pager should appear below the grid")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_PagerAppearsBelowGrid()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when pager positioning is available
    }

    [Fact(Skip = "Pager positioning not yet implemented", DisplayName = "And both pagers should synchronize")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_PagersSync()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when pager positioning is available
    }
}
