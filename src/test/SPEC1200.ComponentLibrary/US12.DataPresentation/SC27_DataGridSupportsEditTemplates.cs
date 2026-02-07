using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "27")]
public class SC27_DataGridSupportsEditTemplates : Scenario
{
    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(Skip = "Feature not yet implemented", DisplayName = "Then cells should render using the EditTemplate")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_CellsRenderUsingEditTemplate()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when EditTemplate feature is available
        // Given I have columns with EditTemplate defined
        // When a row enters edit mode
        // Then cells should render using the EditTemplate
    }

    [Fact(Skip = "Feature not yet implemented", DisplayName = "And I can use any input component (TextBox, NumericBox, DropDown, etc.)")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_CanUseAnyInputComponent()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when EditTemplate supports various input components
        // When a row enters edit mode
        // Then I can use any input component (TextBox, NumericBox, DropDown, etc.)
    }
}
