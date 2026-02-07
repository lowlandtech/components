using Bunit;
using LowlandTech.Components.Components.DataGrid;
using Shouldly;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

/// <summary>
/// SC11: DataGrid supports programmatic filtering
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "11")]
public class SC11_DataGridSupportsProgrammaticFiltering : Scenario
{
    private record TestItem(int Id, string Name, string Department);

    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(DisplayName = "Then the grid should apply the filters")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_GridAppliesFilters()
    {
        await ArrangeAndActAsync();
        
        // Given
        using var ctx = new BunitContext();
        var data = new List<TestItem>
        {
            new(1, "Alice", "Engineering"),
            new(2, "Bob", "Marketing"),
            new(3, "Charlie", "Engineering"),
            new(4, "Diana", "Sales")
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

        // When - programmatically set filter
        var component = cut.Instance;
        await component.SetFilter("Department", "Engineering");

        // Then - grid should show only filtered rows
        var dataRows = cut.FindAll("tbody tr");
        dataRows.Count.ShouldBe(2);
        dataRows[0].TextContent.ShouldContain("Alice");
        dataRows[1].TextContent.ShouldContain("Charlie");
    }

    [Fact(DisplayName = "And the filter UI should reflect the current filters")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_FilterUIReflectsCurrentFilters()
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

        // When - programmatically set filter
        var component = cut.Instance;
        await component.SetFilter("Name", "Alice");

        // Then - filter input should show the filter value
        var filterInputs = cut.FindAll("tr.filter-row input.filter-input");
        filterInputs[0].GetAttribute("value").ShouldBe("Alice");
        
        // And - GetFilterValue should return the value
        component.GetFilterValue("Name").ShouldBe("Alice");
    }
}
