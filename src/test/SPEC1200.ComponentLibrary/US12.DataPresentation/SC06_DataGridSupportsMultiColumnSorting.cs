using Bunit;
using LowlandTech.Components.Components.DataGrid;
using Microsoft.AspNetCore.Components.Web;
using Shouldly;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

/// <summary>
/// SC06: DataGrid supports multi-column sorting
/// 
/// Feature: US12.DataPresentation.feature
/// Scenario: SC06 - DataGrid supports multi-column sorting
/// 
/// Given a DataGrid with AllowMultiColumnSorting enabled
/// When I click on multiple column headers while holding modifier key
/// Then data sorts by multiple columns in order
/// And each sorted header displays sort indicators with priority numbers
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "06")]
public class SC06_DataGridSupportsMultiColumnSorting : Scenario
{
    private record TestItem(int Id, string Category, string Name, decimal Price);

    protected override Task GivenAsync()
    {
        return Task.CompletedTask;
    }

    protected override Task WhenAsync()
    {
        return Task.CompletedTask;
    }

    [Fact(DisplayName = "Then clicking multiple column headers with modifier key sorts by multiple columns")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_ClickingMultipleHeadersWithModifierKey_SortsByMultipleColumns()
    {
        await ArrangeAndActAsync();
        
        // Given
        using var ctx = new BunitContext();
        var data = new List<TestItem>
        {
            new(1, "Electronics", "Tablet", 300.00m),
            new(2, "Electronics", "Phone", 500.00m),
            new(3, "Books", "Novel", 15.00m),
            new(4, "Books", "Textbook", 80.00m),
            new(5, "Electronics", "Laptop", 1000.00m)
        };

        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.AllowSorting, true)
            .Add(p => p.AllowMultiColumnSorting, true)
            .Add(p => p.Columns, builder =>
            {
                // Category column
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Category");
                builder.AddAttribute(2, "Property", "Category");
                builder.AddAttribute(3, "Sortable", true);
                builder.CloseComponent();
                
                // Name column
                builder.OpenComponent<DataGridColumn<TestItem>>(4);
                builder.AddAttribute(5, "Title", "Name");
                builder.AddAttribute(6, "Property", "Name");
                builder.AddAttribute(7, "Sortable", true);
                builder.CloseComponent();
            }));

        // When - click Category header
        cut.Find("thead tr th:nth-child(1)").Click(); // Sort by Category ascending

        // Then - click Name header with Ctrl key (multi-column sort)
        cut.Find("thead tr th:nth-child(2)").Click(new MouseEventArgs { CtrlKey = true }); // Then by Name ascending

        // Verify data is sorted by Category, then Name
        var rows = cut.FindAll("tbody tr");
        rows.Count.ShouldBe(5);
        
        // Books category should come first (sorted alphabetically)
        rows[0].TextContent.ShouldContain("Books");
        rows[0].TextContent.ShouldContain("Novel"); // Books-Novel comes before Books-Textbook
        
        rows[1].TextContent.ShouldContain("Books");
        rows[1].TextContent.ShouldContain("Textbook");
        
        // Electronics category should come after Books
        rows[2].TextContent.ShouldContain("Electronics");
        rows[2].TextContent.ShouldContain("Laptop"); // Electronics items sorted by Name
        
        rows[3].TextContent.ShouldContain("Electronics");
        rows[3].TextContent.ShouldContain("Phone");
        
        rows[4].TextContent.ShouldContain("Electronics");
        rows[4].TextContent.ShouldContain("Tablet");
    }

    [Fact(DisplayName = "And each sorted header displays sort indicators with priority numbers")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_SortedHeaders_DisplayPriorityNumbers()
    {
        await ArrangeAndActAsync();
        
        // Given
        using var ctx = new BunitContext();
        var data = new List<TestItem>
        {
            new(1, "Electronics", "Tablet", 300.00m),
            new(2, "Books", "Novel", 15.00m)
        };

        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.AllowSorting, true)
            .Add(p => p.AllowMultiColumnSorting, true)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Category");
                builder.AddAttribute(2, "Property", "Category");
                builder.AddAttribute(3, "Sortable", true);
                builder.CloseComponent();
                
                builder.OpenComponent<DataGridColumn<TestItem>>(4);
                builder.AddAttribute(5, "Title", "Name");
                builder.AddAttribute(6, "Property", "Name");
                builder.AddAttribute(7, "Sortable", true);
                builder.CloseComponent();
            }));

        // When - sort by two columns
        cut.Find("thead tr th:nth-child(1)").Click(); // Category - priority 1
        cut.Find("thead tr th:nth-child(2)").Click(new MouseEventArgs { CtrlKey = true }); // Name - priority 2

        // Then - priority numbers should display
        var sortIndicators = cut.FindAll(".sort-priority");
        sortIndicators.Count.ShouldBe(2);
        sortIndicators[0].TextContent.ShouldBe("1");
        sortIndicators[1].TextContent.ShouldBe("2");
    }

    [Fact(DisplayName = "And the data should sort by multiple columns in order")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_DataSorts_ByMultipleColumnsInOrder()
    {
        await ArrangeAndActAsync();
        
        // Given - data with same category but different names and prices
        using var ctx = new BunitContext();
        var data = new List<TestItem>
        {
            new(1, "A", "Z", 100m),
            new(2, "A", "Y", 50m),
            new(3, "A", "Y", 200m),
            new(4, "B", "X", 75m)
        };

        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.AllowSorting, true)
            .Add(p => p.AllowMultiColumnSorting, true)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Category");
                builder.AddAttribute(2, "Property", "Category");
                builder.AddAttribute(3, "Sortable", true);
                builder.CloseComponent();
                
                builder.OpenComponent<DataGridColumn<TestItem>>(4);
                builder.AddAttribute(5, "Title", "Name");
                builder.AddAttribute(6, "Property", "Name");
                builder.AddAttribute(7, "Sortable", true);
                builder.CloseComponent();
                
                builder.OpenComponent<DataGridColumn<TestItem>>(8);
                builder.AddAttribute(9, "Title", "Price");
                builder.AddAttribute(10, "Property", "Price");
                builder.AddAttribute(11, "Sortable", true);
                builder.CloseComponent();
            }));

        // When - sort by Category (asc), then Name (asc), then Price (asc)
        cut.Find("thead tr th:nth-child(1)").Click(); // Category
        cut.Find("thead tr th:nth-child(2)").Click(new MouseEventArgs { CtrlKey = true }); // Name
        cut.Find("thead tr th:nth-child(3)").Click(new MouseEventArgs { CtrlKey = true }); // Price

        // Then - verify multi-level sorting: A-Y-50, A-Y-200, A-Z-100, B-X-75
        var rows = cut.FindAll("tbody tr");
        var cells = cut.FindAll("tbody tr td");
        
        // Row 1: A, Y, 50
        cells[0].TextContent.ShouldBe("A");
        cells[1].TextContent.ShouldBe("Y");
        cells[2].TextContent.ShouldContain("50");
        
        // Row 2: A, Y, 200
        cells[3].TextContent.ShouldBe("A");
        cells[4].TextContent.ShouldBe("Y");
        cells[5].TextContent.ShouldContain("200");
        
        // Row 3: A, Z, 100
        cells[6].TextContent.ShouldBe("A");
        cells[7].TextContent.ShouldBe("Z");
        cells[8].TextContent.ShouldContain("100");
        
        // Row 4: B, X, 75
        cells[9].TextContent.ShouldBe("B");
        cells[10].TextContent.ShouldBe("X");
        cells[11].TextContent.ShouldContain("75");
    }
}
