using Bunit;
using LowlandTech.Components.Components.DataGrid;
using Shouldly;
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
    private record TestItem(int Id, string Name, string Department);

    protected override Task GivenAsync()
    {
        return Task.CompletedTask;
    }

    protected override Task WhenAsync()
    {
        return Task.CompletedTask;
    }

    [Fact(DisplayName = "Then a filter row should appear below the headers")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_FilterRowAppearsWithSimpleMode()
    {
        await ArrangeAndActAsync();
        
        // Given
        using var ctx = new BunitContext();
        var data = new List<TestItem>
        {
            new(1, "Alice", "Engineering"),
            new(2, "Bob", "Marketing"),
            new(3, "Charlie", "Engineering")
        };

        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.AllowFiltering, true)
            .Add(p => p.FilterMode, FilterMode.Simple)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Name");
                builder.AddAttribute(2, "Property", "Name");
                builder.AddAttribute(3, "Filterable", true);
                builder.CloseComponent();
                
                builder.OpenComponent<DataGridColumn<TestItem>>(4);
                builder.AddAttribute(5, "Title", "Department");
                builder.AddAttribute(6, "Property", "Department");
                builder.AddAttribute(7, "Filterable", true);
                builder.CloseComponent();
            }));

        // Then - filter row should exist
        var filterRow = cut.Find("tr.filter-row");
        filterRow.ShouldNotBeNull();
    }

    [Fact(DisplayName = "And each filterable column should display a filter input")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_FilterableColumnsDisplayInput()
    {
        await ArrangeAndActAsync();
        
        // Given
        using var ctx = new BunitContext();
        var data = new List<TestItem>
        {
            new(1, "Alice", "Engineering"),
            new(2, "Bob", "Marketing")
        };

        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.AllowFiltering, true)
            .Add(p => p.FilterMode, FilterMode.Simple)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Name");
                builder.AddAttribute(2, "Property", "Name");
                builder.AddAttribute(3, "Filterable", true);
                builder.CloseComponent();
                
                builder.OpenComponent<DataGridColumn<TestItem>>(4);
                builder.AddAttribute(5, "Title", "Department");
                builder.AddAttribute(6, "Property", "Department");
                builder.AddAttribute(7, "Filterable", true);
                builder.CloseComponent();
            }));

        // Then - filter inputs should exist for filterable columns
        var filterInputs = cut.FindAll("tr.filter-row input.filter-input");
        filterInputs.Count.ShouldBe(2);
    }

    [Fact(DisplayName = "Then the data should filter to matching rows only")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_DataFiltersToMatchingRows()
    {
        await ArrangeAndActAsync();
        
        // Given
        using var ctx = new BunitContext();
        var data = new List<TestItem>
        {
            new(1, "Alice", "Engineering"),
            new(2, "Bob", "Marketing"),
            new(3, "Charlie", "Engineering")
        };

        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.AllowFiltering, true)
            .Add(p => p.FilterMode, FilterMode.Simple)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Name");
                builder.AddAttribute(2, "Property", "Name");
                builder.AddAttribute(3, "Filterable", true);
                builder.CloseComponent();
                
                builder.OpenComponent<DataGridColumn<TestItem>>(4);
                builder.AddAttribute(5, "Title", "Department");
                builder.AddAttribute(6, "Property", "Department");
                builder.AddAttribute(7, "Filterable", true);
                builder.CloseComponent();
            }));

        // When - enter filter value for Department
        var filterInputs = cut.FindAll("tr.filter-row input.filter-input");
        cut.InvokeAsync(() => filterInputs[1].Change("Engineering"));

        // Then - only Engineering rows should be visible
        var dataRows = cut.FindAll("tbody tr");
        dataRows.Count.ShouldBe(2);
        
        // Verify the filtered data
        dataRows[0].TextContent.ShouldContain("Alice");
        dataRows[1].TextContent.ShouldContain("Charlie");
    }
}
