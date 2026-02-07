using Bunit;
using LowlandTech.Components.Components.DataGrid;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

/// <summary>
/// SC38: DataGrid supports column text alignment
/// 
/// Feature: US12.DataPresentation.feature
/// Scenario: SC38 - DataGrid column text alignment
/// 
/// Given a DataGrid with columns using TextAlign property
/// When the grid renders
/// Then cell content aligns as specified (Left, Center, Right)
/// </summary>
public class SC38_DataGridColumnTextAlignment
{
    private record TestItem(int Id, string Name, decimal Price);

    [Fact(DisplayName = "Column with TextAlign Left aligns content to the left")]
    public void Column_WithTextAlignLeft_AlignsContentLeft()
    {
        // Given
        using var ctx = new BunitContext();
        var data = new List<TestItem>
        {
            new(1, "Product A", 99.99m)
        };

        // When
        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Name");
                builder.AddAttribute(2, "Property", "Name");
                builder.AddAttribute(3, "TextAlign", TextAlign.Left);
                builder.CloseComponent();
            }));

        // Then
        var cell = cut.Find("tbody tr td");
        var cellClass = cell.GetAttribute("class");
        Assert.Contains("text-left", cellClass);
    }

    [Fact(DisplayName = "Column with TextAlign Center aligns content to center")]
    public void Column_WithTextAlignCenter_AlignsContentCenter()
    {
        // Given
        using var ctx = new BunitContext();
        var data = new List<TestItem>
        {
            new(1, "Product A", 99.99m)
        };

        // When
        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "ID");
                builder.AddAttribute(2, "Property", "Id");
                builder.AddAttribute(3, "TextAlign", TextAlign.Center);
                builder.CloseComponent();
            }));

        // Then
        var cell = cut.Find("tbody tr td");
        var cellClass = cell.GetAttribute("class");
        Assert.Contains("text-center", cellClass);
    }

    [Fact(DisplayName = "Column with TextAlign Right aligns content to the right")]
    public void Column_WithTextAlignRight_AlignsContentRight()
    {
        // Given
        using var ctx = new BunitContext();
        var data = new List<TestItem>
        {
            new(1, "Product A", 99.99m)
        };

        // When
        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Price");
                builder.AddAttribute(2, "Property", "Price");
                builder.AddAttribute(3, "TextAlign", TextAlign.Right);
                builder.CloseComponent();
            }));

        // Then
        var cell = cut.Find("tbody tr td");
        var cellClass = cell.GetAttribute("class");
        Assert.Contains("text-right", cellClass);
    }

    [Fact(DisplayName = "Multiple columns can have different text alignments")]
    public void MultipleColumns_CanHaveDifferentAlignments()
    {
        // Given
        using var ctx = new BunitContext();
        var data = new List<TestItem>
        {
            new(1, "Product A", 99.99m)
        };

        // When
        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.Columns, builder =>
            {
                // Left aligned
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "ID");
                builder.AddAttribute(2, "Property", "Id");
                builder.AddAttribute(3, "TextAlign", TextAlign.Left);
                builder.CloseComponent();

                // Center aligned
                builder.OpenComponent<DataGridColumn<TestItem>>(4);
                builder.AddAttribute(5, "Title", "Name");
                builder.AddAttribute(6, "Property", "Name");
                builder.AddAttribute(7, "TextAlign", TextAlign.Center);
                builder.CloseComponent();

                // Right aligned
                builder.OpenComponent<DataGridColumn<TestItem>>(8);
                builder.AddAttribute(9, "Title", "Price");
                builder.AddAttribute(10, "Property", "Price");
                builder.AddAttribute(11, "TextAlign", TextAlign.Right);
                builder.CloseComponent();
            }));

        // Then
        var cells = cut.FindAll("tbody tr td");
        Assert.Equal(3, cells.Count);
        
        Assert.Contains("text-left", cells[0].GetAttribute("class"));
        Assert.Contains("text-center", cells[1].GetAttribute("class"));
        Assert.Contains("text-right", cells[2].GetAttribute("class"));
    }

    [Fact(DisplayName = "Column without TextAlign uses default alignment")]
    public void Column_WithoutTextAlign_UsesDefaultAlignment()
    {
        // Given
        using var ctx = new BunitContext();
        var data = new List<TestItem>
        {
            new(1, "Product A", 99.99m)
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
        var cell = cut.Find("tbody tr td");
        // Default is left alignment
        var cellClass = cell.GetAttribute("class");
        Assert.Contains("text-left", cellClass);
    }

    [Fact(DisplayName = "TextAlign applies to both header and data cells")]
    public void TextAlign_AppliesToHeaderAndDataCells()
    {
        // Given
        using var ctx = new BunitContext();
        var data = new List<TestItem>
        {
            new(1, "Product A", 99.99m)
        };

        // When
        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Price");
                builder.AddAttribute(2, "Property", "Price");
                builder.AddAttribute(3, "TextAlign", TextAlign.Right);
                builder.CloseComponent();
            }));

        // Then
        var headerCell = cut.Find("thead tr th");
        Assert.Contains("text-right", headerCell.GetAttribute("class"));

        var dataCell = cut.Find("tbody tr td");
        Assert.Contains("text-right", dataCell.GetAttribute("class"));
    }
}
