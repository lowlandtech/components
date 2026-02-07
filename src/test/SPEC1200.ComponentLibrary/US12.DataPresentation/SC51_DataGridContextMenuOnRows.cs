using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "51")]
public class SC51_DataGridContextMenuOnRows : Scenario
{
    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(Skip = "Feature not yet implemented", DisplayName = "Then the ContextMenu event should fire")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_ContextMenuEventFires()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when ContextMenu event is available
        // Given I have a DataGrid with ContextMenu event
        // When I right-click on a row
        // Then the ContextMenu event should fire
    }

    [Fact(Skip = "Feature not yet implemented", DisplayName = "And I can display a custom context menu")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_CanDisplayCustomContextMenu()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when custom context menu is available
        // When I right-click on a row
        // Then I can display a custom context menu
    }

    [Fact(Skip = "Feature not yet implemented", DisplayName = "And the event args should include the row item and position")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_EventArgsIncludeRowItemAndPosition()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when ContextMenu event args are available
        // When I right-click on a row
        // Then the event args should include the row item and position
    }
}
