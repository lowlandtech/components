using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "48")]
public class SC48_DataGridAlternatingRowColors : Scenario
{
    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(Skip = "Feature not yet implemented", DisplayName = "Then odd and even rows should have different background colors")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_OddAndEvenRowsHaveDifferentBackgroundColors()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when AllowAlternatingRows property is available
        // Given I have a DataGrid with AllowAlternatingRows enabled
        // When I render the DataGrid
        // Then odd and even rows should have different background colors
    }

    [Fact(Skip = "Feature not yet implemented", DisplayName = "And it should improve readability")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_ImprovesReadability()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when alternating row colors feature is available
        // When I render the DataGrid
        // Then it should improve readability
    }
}
