using Bunit;
using LowlandTech.Components.Components.DataGrid;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "01")]
public class SC01_DataGridRendersBasicTableStructure : Scenario
{
    public class TestItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    [Fact(DisplayName = "DataGrid renders table element")]
    public void DataGridRendersTableElement()
    {
        using var ctx = new BunitContext();
        
        // Given I have a DataGrid specification with basic columns
        var data = new List<TestItem>
        {
            new TestItem { Id = 1, Name = "Item 1", Price = 10.50m },
            new TestItem { Id = 2, Name = "Item 2", Price = 20.75m }
        };

        // When I render the DataGrid with a collection of items
        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Property", "Id");
                builder.AddAttribute(2, "Title", "ID");
                builder.CloseComponent();
                
                builder.OpenComponent<DataGridColumn<TestItem>>(3);
                builder.AddAttribute(4, "Property", "Name");
                builder.AddAttribute(5, "Title", "Name");
                builder.CloseComponent();
            }));

        // Then the grid should render a table element
        var table = cut.Find("table");
        Assert.NotNull(table);
    }

    [Fact(DisplayName = "DataGrid displays column headers")]
    public void DataGridDisplaysColumnHeaders()
    {
        using var ctx = new BunitContext();
        
        var data = new List<TestItem>
        {
            new TestItem { Id = 1, Name = "Item 1", Price = 10.50m }
        };

        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Property", "Name");
                builder.AddAttribute(2, "Title", "Product Name");
                builder.CloseComponent();
            }));

        // Then it should display column headers
        var header = cut.Find("thead");
        Assert.NotNull(header);
        Assert.Contains("Product Name", header.TextContent);
    }

    [Fact(DisplayName = "DataGrid renders one row per data item")]
    public void DataGridRendersOneRowPerDataItem()
    {
        using var ctx = new BunitContext();
        
        var data = new List<TestItem>
        {
            new TestItem { Id = 1, Name = "Item 1", Price = 10.50m },
            new TestItem { Id = 2, Name = "Item 2", Price = 20.75m },
            new TestItem { Id = 3, Name = "Item 3", Price = 30.00m }
        };

        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Property", "Name");
                builder.AddAttribute(2, "Title", "Name");
                builder.CloseComponent();
            }));

        // Then it should render one row per data item
        var rows = cut.FindAll("tbody tr");
        Assert.Equal(3, rows.Count);
    }

    [Fact(DisplayName = "DataGrid cells display correct property values")]
    public void DataGridCellsDisplayCorrectPropertyValues()
    {
        using var ctx = new BunitContext();
        
        var data = new List<TestItem>
        {
            new TestItem { Id = 1, Name = "Test Product", Price = 99.99m }
        };

        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Property", "Name");
                builder.AddAttribute(2, "Title", "Name");
                builder.CloseComponent();
                
                builder.OpenComponent<DataGridColumn<TestItem>>(3);
                builder.AddAttribute(4, "Property", "Price");
                builder.AddAttribute(5, "Title", "Price");
                builder.CloseComponent();
            }));

        // Then each cell should display the correct property value
        var cells = cut.FindAll("tbody td");
        Assert.Contains("Test Product", cells[0].TextContent);
        Assert.Contains("99.99", cells[1].TextContent);
    }
}
