using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "22")]
public class SC22_DataGridGroupTemplates : Scenario
{
    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(Skip = "Feature not yet implemented", DisplayName = "Then group headers should use the custom template")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_GroupHeadersUseCustomTemplate()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when GroupHeaderTemplate feature is available
        // Given I have a DataGrid with GroupHeaderTemplate
        // When data is grouped
        // Then group headers should use the custom template
    }

    [Fact(Skip = "Feature not yet implemented", DisplayName = "And I can display custom aggregate information")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_CanDisplayCustomAggregateInformation()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when custom group aggregates feature is available
        // When data is grouped
        // Then I can display custom aggregate information
    }
}
