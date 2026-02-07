using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "23")]
public class SC23_DataGridSupportsInlineRowEditing : Scenario
{
    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(Skip = "Feature not yet implemented", DisplayName = "Then that row should enter edit mode")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_RowEntersEditMode()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when EditMode Single feature is available
        // Given I have a DataGrid with EditMode Single
        // When I call EditRow for a specific item
        // Then that row should enter edit mode
    }

    [Fact(Skip = "Feature not yet implemented", DisplayName = "And EditTemplate components should render in cells")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_EditTemplateComponentsRenderInCells()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when EditTemplate feature is available
        // When I call EditRow for a specific item
        // Then EditTemplate components should render in cells
    }

    [Fact(Skip = "Feature not yet implemented", DisplayName = "And other rows should remain in view mode")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_OtherRowsRemainInViewMode()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when single row edit feature is available
        // When I call EditRow for a specific item
        // Then other rows should remain in view mode
    }
}
