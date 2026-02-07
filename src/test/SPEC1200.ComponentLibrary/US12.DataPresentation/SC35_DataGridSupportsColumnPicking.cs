using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "35")]
public class SC35_DataGridSupportsColumnPicking : Scenario
{
    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(Skip = "Feature not yet implemented", DisplayName = "Then a column picker dropdown should appear")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_ColumnPickerDropdownAppears()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when AllowColumnPicking feature is available
        // Given I have a DataGrid with AllowColumnPicking enabled
        // When I render the DataGrid
        // Then a column picker dropdown should appear
    }

    [Fact(Skip = "Feature not yet implemented", DisplayName = "And I can show/hide columns by selecting them")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_CanShowHideColumnsBySelectingThem()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when column picker selection is available
        // When I render the DataGrid
        // Then I can show/hide columns by selecting them
    }

    [Fact(Skip = "Feature not yet implemented", DisplayName = "And hidden columns should not appear in the grid")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_HiddenColumnsDoNotAppearInGrid()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when column hiding is available
        // When I hide columns via the picker
        // Then hidden columns should not appear in the grid
    }
}
