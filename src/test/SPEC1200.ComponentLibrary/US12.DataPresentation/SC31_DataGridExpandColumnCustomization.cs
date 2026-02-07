using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "31")]
public class SC31_DataGridExpandColumnCustomization : Scenario
{
    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(Skip = "Feature not yet implemented", DisplayName = "Then an expand/collapse icon column should appear")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_ExpandCollapseIconColumnAppears()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when ShowExpandColumn feature is available
        // Given I have a DataGrid with Template and ShowExpandColumn
        // When I render the DataGrid
        // Then an expand/collapse icon column should appear
    }

    [Fact(Skip = "Feature not yet implemented", DisplayName = "And I can control which rows are expanded programmatically")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_CanControlExpandedRowsProgrammatically()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when programmatic expand control is available
        // When I render the DataGrid
        // Then I can control which rows are expanded programmatically
    }
}
