using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "58")]
public class SC58_DataGridSupportsCompositeColumnHeaders : Scenario
{
    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(Skip = "Feature not yet implemented", DisplayName = "Then columns should group under parent headers")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_ColumnsGroupUnderParentHeaders()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when Parent property for composite headers is available
        // Given I have columns with Parent property set
        // When I render the DataGrid
        // Then columns should group under parent headers
    }

    [Fact(Skip = "Feature not yet implemented", DisplayName = "And I can create multi-level header hierarchies")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_CanCreateMultiLevelHeaderHierarchies()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when multi-level header hierarchies are available
        // When I render the DataGrid
        // Then I can create multi-level header hierarchies
    }
}
