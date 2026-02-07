using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "25")]
public class SC25_DataGridSupportsBatchEditing : Scenario
{
    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(Skip = "Feature not yet implemented", DisplayName = "Then all specified rows should enter edit mode simultaneously")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_AllSpecifiedRowsEnterEditModeSimultaneously()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when EditMode Multiple feature is available
        // Given I have a DataGrid with EditMode Multiple
        // When I call EditRow for multiple items
        // Then all specified rows should enter edit mode simultaneously
    }

    [Fact(Skip = "Feature not yet implemented", DisplayName = "And I can edit multiple rows before saving")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_CanEditMultipleRowsBeforeSaving()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when batch editing feature is available
        // When I call EditRow for multiple items
        // Then I can edit multiple rows before saving
    }
}
