using Bunit;
using LowlandTech.Components.Components.DataGrid;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

/// <summary>
/// SC15: DataGrid supports single row selection
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "15")]
public class SC15_DataGridSupportsSingleRowSelection : Scenario
{
    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(Skip = "Row selection not yet implemented", DisplayName = "Then the row should be highlighted as selected")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_RowHighlightedWhenSelected()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when row selection is available
    }

    [Fact(Skip = "Row selection not yet implemented", DisplayName = "And the RowSelect event should fire with the selected item")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_RowSelectEventFires()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when row selection is available
    }

    [Fact(Skip = "Row selection not yet implemented", DisplayName = "Then the previous row should be deselected")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_PreviousRowDeselected()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when row selection is available
    }
}
