using Bunit;
using LowlandTech.Components.Components.DataGrid;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

/// <summary>
/// SC37: DataGrid supports column formatting with FormatString
/// 
/// Feature: US12.DataPresentation.feature
/// Scenario: SC37 - DataGrid supports column formatting
/// 
/// Given a DataGrid with columns using FormatString
/// When the grid renders
/// Then cell values format according to the format string
/// And I can use standard or custom .NET format strings
/// </summary>
public class SC37_DataGridSupportsColumnFormatting
{
    private record TestItem(int Id, string Name, decimal Price, DateTime CreatedDate, double Percentage);

    [Fact(DisplayName = "Column with FormatString formats numeric values")]
    public void Column_WithFormatString_FormatsNumericValues()
    {
        // Given
        using var ctx = new BunitContext();
        var data = new List<TestItem>
        {
            new(1, "Item 1", 1234.567m, DateTime.Parse("2024-01-15"), 0.856)
        };

        // When
        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Price");
                builder.AddAttribute(2, "Property", "Price");
                builder.AddAttribute(3, "FormatString", "{0:C2}");
                builder.CloseComponent();
            }));

        // Then
        var priceCell = cut.Find("tbody tr td");
        // Currency format varies by culture, check for formatted number
        Assert.Contains("1,234.57", priceCell.TextContent);
    }

    [Fact(DisplayName = "Column with FormatString formats date values")]
    public void Column_WithFormatString_FormatsDateValues()
    {
        // Given
        using var ctx = new BunitContext();
        var data = new List<TestItem>
        {
            new(1, "Item 1", 99.99m, DateTime.Parse("2024-01-15 14:30:00"), 0.5)
        };

        // When
        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Created");
                builder.AddAttribute(2, "Property", "CreatedDate");
                builder.AddAttribute(3, "FormatString", "{0:yyyy-MM-dd}");
                builder.CloseComponent();
            }));

        // Then
        var dateCell = cut.Find("tbody tr td");
        Assert.Equal("2024-01-15", dateCell.TextContent);
    }

    [Fact(DisplayName = "Column with FormatString formats percentage values")]
    public void Column_WithFormatString_FormatsPercentageValues()
    {
        // Given
        using var ctx = new BunitContext();
        var data = new List<TestItem>
        {
            new(1, "Item 1", 99.99m, DateTime.Parse("2024-01-15"), 0.856)
        };

        // When
        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Completion");
                builder.AddAttribute(2, "Property", "Percentage");
                builder.AddAttribute(3, "FormatString", "{0:P1}");
                builder.CloseComponent();
            }));

        // Then
        var percentCell = cut.Find("tbody tr td");
        // Percentage format may have space, just check for the value
        Assert.Contains("85.6", percentCell.TextContent);
        Assert.Contains("%", percentCell.TextContent);
    }

    [Fact(DisplayName = "Multiple columns can have different format strings")]
    public void MultipleColumns_CanHaveDifferentFormatStrings()
    {
        // Given
        using var ctx = new BunitContext();
        var data = new List<TestItem>
        {
            new(1, "Product A", 1234.567m, DateTime.Parse("2024-01-15"), 0.75)
        };

        // When
        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.Columns, builder =>
            {
                // Column 1: Currency format
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Price");
                builder.AddAttribute(2, "Property", "Price");
                builder.AddAttribute(3, "FormatString", "{0:C2}");
                builder.CloseComponent();

                // Column 2: Date format
                builder.OpenComponent<DataGridColumn<TestItem>>(4);
                builder.AddAttribute(5, "Title", "Date");
                builder.AddAttribute(6, "Property", "CreatedDate");
                builder.AddAttribute(7, "FormatString", "{0:MMM dd, yyyy}");
                builder.CloseComponent();

                // Column 3: Percentage format
                builder.OpenComponent<DataGridColumn<TestItem>>(8);
                builder.AddAttribute(9, "Title", "Complete");
                builder.AddAttribute(10, "Property", "Percentage");
                builder.AddAttribute(11, "FormatString", "{0:P0}");
                builder.CloseComponent();
            }));

        // Then
        var cells = cut.FindAll("tbody tr td");
        Assert.Equal(3, cells.Count);
        Assert.Contains("1,234.57", cells[0].TextContent);
        Assert.Equal("Jan 15, 2024", cells[1].TextContent);
        Assert.Contains("75", cells[2].TextContent);
        Assert.Contains("%", cells[2].TextContent);
    }

    [Fact(DisplayName = "Column without FormatString displays raw value")]
    public void Column_WithoutFormatString_DisplaysRawValue()
    {
        // Given
        using var ctx = new BunitContext();
        var data = new List<TestItem>
        {
            new(1, "Item 1", 1234.567m, DateTime.Parse("2024-01-15"), 0.856)
        };

        // When
        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Name");
                builder.AddAttribute(2, "Property", "Name");
                builder.CloseComponent();
            }));

        // Then
        var nameCell = cut.Find("tbody tr td");
        Assert.Equal("Item 1", nameCell.TextContent);
    }

    [Fact(DisplayName = "FormatString supports custom number formatting")]
    public void FormatString_SupportsCustomNumberFormatting()
    {
        // Given
        using var ctx = new BunitContext();
        var data = new List<TestItem>
        {
            new(42, "Item", 9876.543m, DateTime.Now, 0.5)
        };

        // When
        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "ID");
                builder.AddAttribute(2, "Property", "Id");
                builder.AddAttribute(3, "FormatString", "{0:D5}");
                builder.CloseComponent();
            }));

        // Then
        var idCell = cut.Find("tbody tr td");
        Assert.Equal("00042", idCell.TextContent);
    }

    [Fact(DisplayName = "FormatString with fixed decimal places")]
    public void FormatString_WithFixedDecimalPlaces()
    {
        // Given
        using var ctx = new BunitContext();
        var data = new List<TestItem>
        {
            new(1, "Item", 123.4m, DateTime.Now, 0.123456)
        };

        // When
        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Percentage");
                builder.AddAttribute(2, "Property", "Percentage");
                builder.AddAttribute(3, "FormatString", "{0:F4}");
                builder.CloseComponent();
            }));

        // Then
        var cell = cut.Find("tbody tr td");
        Assert.Equal("0.1235", cell.TextContent);
    }
}
