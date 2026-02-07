using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "28")]
public class SC28_DataGridInsertNewRows : Scenario
{
    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(Skip = "Feature not yet implemented", DisplayName = "Then a new empty row should appear at the top")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_NewEmptyRowAppearsAtTop()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when InsertRow feature is available
        // Given I have a DataGrid with editing enabled
        // When I call InsertRow
        // Then a new empty row should appear at the top
    }

    [Fact(Skip = "Feature not yet implemented", DisplayName = "And it should be in edit mode")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_NewRowIsInEditMode()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when InsertRow edit mode is available
        // When I call InsertRow
        // Then it should be in edit mode
    }

    [Fact(Skip = "Feature not yet implemented", DisplayName = "And I can fill in values and save")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_CanFillInValuesAndSave()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when InsertRow save feature is available
        // When I call InsertRow
        // Then I can fill in values and save
    }
}
