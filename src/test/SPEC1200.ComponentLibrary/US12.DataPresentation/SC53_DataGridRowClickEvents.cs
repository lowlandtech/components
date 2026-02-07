using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "53")]
public class SC53_DataGridRowClickEvents : Scenario
{
    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(Skip = "Feature not yet implemented", DisplayName = "Then the RowClick event should fire with the row item")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_RowClickEventFiresWithRowItem()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when RowClick event is available
        // Given I have a DataGrid with RowClick event handler
        // When I click on a row
        // Then the RowClick event should fire with the row item
    }
}
