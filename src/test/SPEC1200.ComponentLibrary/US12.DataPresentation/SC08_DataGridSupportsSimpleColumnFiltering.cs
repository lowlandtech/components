using Bunit;
using LowlandTech.Components.Components.DataGrid;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

/// <summary>
/// SC08: DataGrid supports simple column filtering
/// 
/// Feature: US12.DataPresentation.feature
/// Scenario: SC08 - DataGrid supports simple column filtering
/// 
/// Given a DataGrid with AllowFiltering enabled and FilterMode set to Simple
/// When I render the DataGrid
/// Then a filter row should appear below the headers
/// And each filterable column should display a filter input
/// When I enter a filter value
/// Then the data should filter to matching rows only
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "08")]
public class SC08_DataGridSupportsSimpleColumnFiltering : Scenario
{
    protected override Task GivenAsync()
    {
        return Task.CompletedTask;
    }

    protected override Task WhenAsync()
    {
        return Task.CompletedTask;
    }

    [Fact(Skip = "Filtering not yet implemented", DisplayName = "Then a filter row should appear below the headers")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_FilterRowAppearsWithSimpleMode()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when filtering is available
    }

    [Fact(Skip = "Filtering not yet implemented", DisplayName = "And each filterable column should display a filter input")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_FilterableColumnsDisplayInput()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when filtering is available
    }

    [Fact(Skip = "Filtering not yet implemented", DisplayName = "Then the data should filter to matching rows only")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_DataFiltersToMatchingRows()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when filtering is available
    }
}
