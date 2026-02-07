using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "52")]
public class SC52_DataGridKeyboardNavigation : Scenario
{
    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(Skip = "Feature not yet implemented", DisplayName = "Then focus should move between cells")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_FocusMovesBetweenCells()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when keyboard navigation with arrow keys is available
        // Given I have a DataGrid with keyboard focus
        // When I press arrow keys
        // Then focus should move between cells
    }

    [Fact(Skip = "Feature not yet implemented", DisplayName = "Then focus should move to the next editable cell")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_FocusMovesToNextEditableCell()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when Tab key navigation is available
        // When I press Tab
        // Then focus should move to the next editable cell
    }

    [Fact(Skip = "Feature not yet implemented", DisplayName = "Then the RowDoubleClick or RowClick event should fire")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_RowDoubleClickOrRowClickEventFires()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when Enter key row activation is available
        // When I press Enter on a row
        // Then the RowDoubleClick or RowClick event should fire
    }
}
