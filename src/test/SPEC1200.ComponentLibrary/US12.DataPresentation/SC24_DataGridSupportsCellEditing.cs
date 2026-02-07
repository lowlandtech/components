using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "24")]
public class SC24_DataGridSupportsCellEditing : Scenario
{
    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(Skip = "Feature not yet implemented", DisplayName = "Then only that cell should become editable")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_OnlyClickedCellBecomesEditable()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when EditMode Cell feature is available
        // Given I have a DataGrid with EditMode Cell
        // When I click on a cell
        // Then only that cell should become editable
    }

    [Fact(Skip = "Feature not yet implemented", DisplayName = "And other cells should remain read-only")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_OtherCellsRemainReadOnly()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when cell edit mode feature is available
        // When I click on a cell
        // Then other cells should remain read-only
    }
}
