using Bunit;
using LowlandTech.Components.Components.DataGrid;
using Microsoft.AspNetCore.Components.Web;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

/// <summary>
/// SC05: DataGrid supports single column sorting
/// 
/// Feature: US12.DataPresentation.feature
/// Scenario: SC05 - DataGrid supports single column sorting
/// 
/// Given a DataGrid with AllowSorting enabled
/// When I click on a sortable column header
/// Then data sorts by that column in ascending order
/// When I click the same header again, it sorts descending
/// </summary>
public class SC05_DataGridSupportsSingleColumnSorting
{
    private record TestItem(int Id, string Name, decimal Price);

    [Fact(DisplayName = "Clicking sortable column header sorts data ascending")]
    public void ClickingSortableHeader_SortsDataAscending()
    {
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
            }));

        // When - click the Name header
        var header = cut.Find("thead tr th");
        header.Click();

        // Then - data should be sorted ascending by Name
        var cells = cut.FindAll("tbody tr td");
        Assert.Equal("Alice", cells[0].TextContent);
        Assert.Equal("Bob", cells[1].TextContent);
        Assert.Equal("Charlie", cells[2].TextContent);
    }

    [Fact(DisplayName = "Clicking sorted column again sorts descending")]
    public void ClickingSortedColumnAgain_SortsDescending()
    {
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
            }));

        var header = cut.Find("thead tr th");
        
        // When - click once for ascending
        header.Click();
        
        // Then - verify ascending
        var cellsAsc = cut.FindAll("tbody tr td");
        Assert.Equal("Alice", cellsAsc[0].TextContent);

        // When - click again for descending
        header.Click();

        // Then - data should be sorted descending
        var cellsDesc = cut.FindAll("tbody tr td");
        Assert.Equal("Charlie", cellsDesc[0].TextContent);
        Assert.Equal("Bob", cellsDesc[1].TextContent);
        Assert.Equal("Alice", cellsDesc[2].TextContent);
    }

    [Fact(DisplayName = "Sort indicator displays in header when sorted ascending")]
    public void SortIndicator_DisplaysInHeaderWhenSortedAscending()
    {
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
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Name");
                builder.AddAttribute(2, "Property", "Name");
                builder.AddAttribute(3, "Sortable", true);
                builder.CloseComponent();
            }));

        // When
        var header = cut.Find("thead tr th");
        header.Click();

        // Then - should have ascending sort indicator
        var sortIcon = header.QuerySelector(".sort-indicator");
        Assert.NotNull(sortIcon);
        Assert.Contains("ascending", sortIcon.GetAttribute("data-sort-direction") ?? "");
    }

    [Fact(DisplayName = "Sort indicator displays in header when sorted descending")]
    public void SortIndicator_DisplaysInHeaderWhenSortedDescending()
    {
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
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Name");
                builder.AddAttribute(2, "Property", "Name");
                builder.AddAttribute(3, "Sortable", true);
                builder.CloseComponent();
            }));

        var header = cut.Find("thead tr th");
        
        // When - click twice for descending
        header.Click();
        header.Click();

        // Then - should have descending sort indicator
        var sortIcon = header.QuerySelector(".sort-indicator");
        Assert.NotNull(sortIcon);
        Assert.Contains("descending", sortIcon.GetAttribute("data-sort-direction") ?? "");
    }

    [Fact(DisplayName = "Non-sortable column header does not sort on click")]
    public void NonSortableColumn_DoesNotSortOnClick()
    {
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
                builder.AddAttribute(3, "Sortable", false);
                builder.CloseComponent();
            }));

        var originalOrder = cut.FindAll("tbody tr td").Select(c => c.TextContent).ToList();

        // When - click the header
        var header = cut.Find("thead tr th");
        header.Click();

        // Then - order should remain unchanged
        var currentOrder = cut.FindAll("tbody tr td").Select(c => c.TextContent).ToList();
        Assert.Equal(originalOrder, currentOrder);
    }

    [Fact(DisplayName = "Sorting works with numeric columns")]
    public void Sorting_WorksWithNumericColumns()
    {
        // Given
        using var ctx = new BunitContext();
        var data = new List<TestItem>
        {
            new(3, "Item C", 15.50m),
            new(1, "Item A", 99.99m),
            new(2, "Item B", 5.25m)
        };

        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.AllowSorting, true)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Price");
                builder.AddAttribute(2, "Property", "Price");
                builder.AddAttribute(3, "Sortable", true);
                builder.CloseComponent();
            }));

        // When - click to sort ascending
        var header = cut.Find("thead tr th");
        header.Click();

        // Then - should sort numerically
        var cells = cut.FindAll("tbody tr td");
        Assert.Contains("5.25", cells[0].TextContent.Replace(",", "."));
        Assert.Contains("15.5", cells[1].TextContent.Replace(",", "."));
        Assert.Contains("99.99", cells[2].TextContent.Replace(",", "."));
    }

    [Fact(DisplayName = "AllowSorting false prevents all sorting")]
    public void AllowSortingFalse_PreventsAllSorting()
    {
        // Given
        using var ctx = new BunitContext();
        var data = new List<TestItem>
        {
            new(3, "Charlie", 30.00m),
            new(1, "Alice", 10.00m)
        };

        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.AllowSorting, false)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Name");
                builder.AddAttribute(2, "Property", "Name");
                builder.AddAttribute(3, "Sortable", true);
                builder.CloseComponent();
            }));

        var originalOrder = cut.FindAll("tbody tr td").Select(c => c.TextContent).ToList();

        // When - click the header
        var header = cut.Find("thead tr th");
        header.Click();

        // Then - order should remain unchanged
        var currentOrder = cut.FindAll("tbody tr td").Select(c => c.TextContent).ToList();
        Assert.Equal(originalOrder, currentOrder);
    }
}
