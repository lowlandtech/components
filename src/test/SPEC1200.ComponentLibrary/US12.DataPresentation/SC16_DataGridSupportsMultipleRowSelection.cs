using Bunit;
using LowlandTech.Components.Components.DataGrid;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

/// <summary>
/// SC16: DataGrid supports multiple row selection
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "16")]
public class SC16_DataGridSupportsMultipleRowSelection : Scenario
{
    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(Skip = "Multiple row selection not yet implemented", DisplayName = "Then all clicked rows should be selected")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_AllClickedRowsSelected()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when multiple row selection is available
    }

    [Fact(Skip = "Multiple row selection not yet implemented", DisplayName = "And the RowSelect event should fire for each selection")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_RowSelectEventFiresForEach()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when multiple row selection is available
    }

    [Fact(Skip = "Multiple row selection not yet implemented", DisplayName = "Then it should be deselected when clicked again")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_ClickedRowDeselected()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when multiple row selection is available
    }
}
