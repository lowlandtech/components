using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "29")]
public class SC29_DataGridDeleteRows : Scenario
{
    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(Skip = "Feature not yet implemented", DisplayName = "Then the RowRemove event should fire")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_RowRemoveEventFires()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when DeleteRow feature is available
        // Given I have a DataGrid with row selection
        // When I call DeleteRow for a selected item
        // Then the RowRemove event should fire
    }

    [Fact(Skip = "Feature not yet implemented", DisplayName = "And I can handle the deletion in my code")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_CanHandleDeletionInCode()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when RowRemove event handler is available
        // When I call DeleteRow for a selected item
        // Then I can handle the deletion in my code
    }
}
