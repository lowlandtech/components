using Bunit;
using LowlandTech.Components.Components.DataGrid;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

/// <summary>
/// SC07: DataGrid supports programmatic sorting
/// 
/// Feature: US12.DataPresentation.feature
/// Scenario: SC07 - DataGrid supports programmatic sorting
/// 
/// Given a DataGrid specification
/// When I programmatically set sort descriptors
/// Then the grid should apply the sorting
/// And the headers should reflect the current sort state
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "07")]
public class SC07_DataGridSupportsProgrammaticSorting : Scenario
{
    protected override Task GivenAsync()
    {
        return Task.CompletedTask;
    }

    protected override Task WhenAsync()
    {
        return Task.CompletedTask;
    }

    [Fact(Skip = "Programmatic sorting not yet implemented", DisplayName = "Then the grid should apply the sorting")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_GridAppliesProgrammaticSorting()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when programmatic sorting is available
    }

    [Fact(Skip = "Programmatic sorting not yet implemented", DisplayName = "And the headers should reflect the current sort state")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_HeadersReflectSortState()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when programmatic sorting is available
    }
}
