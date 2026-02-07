using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "40")]
public class SC40_DataGridLoadingIndicator : Scenario
{
    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(Skip = "Feature not yet implemented", DisplayName = "Then a loading overlay should appear")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_LoadingOverlayAppears()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when IsLoading property is available
        // Given I have a DataGrid with IsLoading bound to a property
        // When IsLoading is true
        // Then a loading overlay should appear
    }

    [Fact(Skip = "Feature not yet implemented", DisplayName = "And the grid should indicate data is being fetched")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_GridIndicatesDataIsBeingFetched()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when loading indicator is available
        // When IsLoading is true
        // Then the grid should indicate data is being fetched
    }
}
