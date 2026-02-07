using Bunit;
using LowlandTech.Components.Components.DataGrid;
using Shouldly;
using Xunit;
using SortDir = LowlandTech.Components.Components.DataGrid.SortDirection;

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
    private record TestItem(int Id, string Name, decimal Price);

    protected override Task GivenAsync()
    {
        return Task.CompletedTask;
    }

    protected override Task WhenAsync()
    {
        return Task.CompletedTask;
    }

    [Fact(DisplayName = "Then the grid should apply the sorting")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_GridAppliesProgrammaticSorting()
    {
        await ArrangeAndActAsync();
        
        // Given
        using var ctx = new BunitContext();
        var data = new List<TestItem>
        {
            new(3, "Charlie", 30.00m),
            new(1, "Alice", 10.00m),
            new(2, "Bob", 20.00m)
        };

        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.AllowSorting, true)
            .Add(p => p.SortBy, "Name")
            .Add(p => p.SortDirection, SortDir.Ascending)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Name");
                builder.AddAttribute(2, "Property", "Name");
                builder.AddAttribute(3, "Sortable", true);
                builder.CloseComponent();
            }));

        // Then - data should be sorted by Name ascending
        var cells = cut.FindAll("tbody tr td");
        cells[0].TextContent.ShouldBe("Alice");
        cells[1].TextContent.ShouldBe("Bob");
        cells[2].TextContent.ShouldBe("Charlie");
    }

    [Fact(DisplayName = "And the headers should reflect the current sort state")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_HeadersReflectSortState()
    {
        await ArrangeAndActAsync();
        
        // Given
        using var ctx = new BunitContext();
        var data = new List<TestItem>
        {
            new(2, "Bob", 20.00m),
            new(1, "Alice", 10.00m)
        };

        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.AllowSorting, true)
            .Add(p => p.SortBy, "Name")
            .Add(p => p.SortDirection, SortDir.Descending)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Name");
                builder.AddAttribute(2, "Property", "Name");
                builder.AddAttribute(3, "Sortable", true);
                builder.CloseComponent();
            }));

        // Then - header should show descending sort indicator
        var sortIndicator = cut.Find(".sort-indicator");
        sortIndicator.ShouldNotBeNull();
        sortIndicator.GetAttribute("data-sort-direction").ShouldBe("descending");
        sortIndicator.TextContent.ShouldContain("▼");
    }

    [Fact(DisplayName = "SetSort method changes sorting programmatically")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_SetSortMethodChangesSorting()
    {
        await ArrangeAndActAsync();
        
        // Given
        using var ctx = new BunitContext();
        var data = new List<TestItem>
        {
            new(3, "Charlie", 30.00m),
            new(1, "Alice", 10.00m),
            new(2, "Bob", 20.00m)
        };

        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.AllowSorting, true)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Name");
                builder.AddAttribute(2, "Property", "Name");
                builder.AddAttribute(3, "Sortable", true);
                builder.CloseComponent();
                
                builder.OpenComponent<DataGridColumn<TestItem>>(4);
                builder.AddAttribute(5, "Title", "Price");
                builder.AddAttribute(6, "Property", "Price");
                builder.AddAttribute(7, "Sortable", true);
                builder.CloseComponent();
            }));

        var instance = cut.Instance;

        // When - programmatically sort by Price descending
        await instance.SetSort("Price", SortDir.Descending);

        // Then - data should be sorted by Price descending
        var cells = cut.FindAll("tbody tr td");
        cells[1].TextContent.ShouldContain("30"); // Charlie - highest price
        cells[3].TextContent.ShouldContain("20"); // Bob
        cells[5].TextContent.ShouldContain("10"); // Alice - lowest price
    }

    [Fact(DisplayName = "ClearSort method removes all sorting")]
    [Trait(Spec.UAC, "04")]
    public async Task UAC04_ClearSortRemovesAllSorting()
    {
        await ArrangeAndActAsync();
        
        // Given
        using var ctx = new BunitContext();
        var data = new List<TestItem>
        {
            new(3, "Charlie", 30.00m),
            new(1, "Alice", 10.00m)
        };

        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.AllowSorting, true)
            .Add(p => p.SortBy, "Name")
            .Add(p => p.SortDirection, SortDir.Ascending)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Name");
                builder.AddAttribute(2, "Property", "Name");
                builder.AddAttribute(3, "Sortable", true);
                builder.CloseComponent();
            }));

        var instance = cut.Instance;

        // When - clear the sort
        await instance.ClearSort();

        // Then - data should return to original order
        var cells = cut.FindAll("tbody tr td");
        cells[0].TextContent.ShouldBe("Charlie"); // Original order
        cells[1].TextContent.ShouldBe("Alice");
        
        // And sort indicator should show none
        var sortIndicator = cut.Find(".sort-indicator");
        sortIndicator.GetAttribute("data-sort-direction").ShouldBe("none");
    }
}
