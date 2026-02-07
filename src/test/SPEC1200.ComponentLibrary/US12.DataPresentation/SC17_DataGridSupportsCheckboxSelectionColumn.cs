using Bunit;
using LowlandTech.Components.Components.DataGrid;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

/// <summary>
/// SC17: DataGrid supports checkbox selection column
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "17")]
public class SC17_DataGridSupportsCheckboxSelectionColumn : Scenario
{
    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(Skip = "Checkbox selection not yet implemented", DisplayName = "Then a checkbox column should appear as the first column")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_CheckboxColumnAppears()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when checkbox selection is available
    }

    [Fact(Skip = "Checkbox selection not yet implemented", DisplayName = "And the header should show a select all checkbox")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_HeaderShowsSelectAllCheckbox()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when checkbox selection is available
    }

    [Fact(Skip = "Checkbox selection not yet implemented", DisplayName = "Then all rows should be selected")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_AllRowsSelectedWhenHeaderChecked()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when checkbox selection is available
    }
}
