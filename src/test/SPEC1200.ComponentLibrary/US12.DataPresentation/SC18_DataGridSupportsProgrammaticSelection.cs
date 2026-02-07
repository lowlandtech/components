using Bunit;
using LowlandTech.Components.Components.DataGrid;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

/// <summary>
/// SC18: DataGrid supports programmatic selection
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "18")]
public class SC18_DataGridSupportsProgrammaticSelection : Scenario
{
    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(Skip = "Programmatic selection not yet implemented", DisplayName = "Then those rows should be highlighted")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_ProgrammaticallySelectedRowsHighlighted()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when programmatic selection is available
    }

    [Fact(Skip = "Programmatic selection not yet implemented", DisplayName = "And the UI should reflect the selection state")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_UIReflectsSelectionState()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when programmatic selection is available
    }
}
