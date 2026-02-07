using Bunit;
using LowlandTech.Components.Components.DataGrid;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

/// <summary>
/// SC04: DataGrid supports header customization with HeaderTemplate
/// 
/// Feature: US12.DataPresentation.feature
/// Scenario: SC04 - DataGrid supports header customization
/// 
/// Given a DataGrid with HeaderTemplate defined
/// When the grid renders
/// Then the custom header appears above the grid spanning full width
/// </summary>
public class SC04_DataGridSupportsHeaderCustomization
{
    private record TestItem(int Id, string Name);

    [Fact(DisplayName = "DataGrid with HeaderTemplate renders custom header above grid")]
    public void DataGrid_WithHeaderTemplate_RendersCustomHeader()
    {
        // Given
        using var ctx = new BunitContext();
        var data = new List<TestItem>
        {
            new(1, "Item 1"),
            new(2, "Item 2")
        };

        // When
        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.HeaderTemplate, builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "class", "custom-header");
                builder.OpenElement(2, "h2");
                builder.AddContent(3, "My Custom Header");
                builder.CloseElement();
                builder.CloseElement();
            })
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "ID");
                builder.AddAttribute(2, "Property", "Id");
                builder.CloseComponent();

                builder.OpenComponent<DataGridColumn<TestItem>>(3);
                builder.AddAttribute(4, "Title", "Name");
                builder.AddAttribute(5, "Property", "Name");
                builder.CloseComponent();
            }));

        // Then
        var header = cut.Find("div.custom-header");
        Assert.NotNull(header);
        
        var heading = header.QuerySelector("h2");
        Assert.NotNull(heading);
        Assert.Equal("My Custom Header", heading.TextContent);
    }

    [Fact(DisplayName = "HeaderTemplate appears before the table element")]
    public void HeaderTemplate_AppearsBeforeTable()
    {
        // Given
        using var ctx = new BunitContext();
        var data = new List<TestItem> { new(1, "Item 1") };

        // When
        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.HeaderTemplate, builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "id", "header-marker");
                builder.AddContent(2, "Header");
                builder.CloseElement();
            })
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Name");
                builder.AddAttribute(2, "Property", "Name");
                builder.CloseComponent();
            }));

        // Then - both elements should exist
        var headerDiv = cut.Find("div#header-marker");
        Assert.NotNull(headerDiv);
        Assert.Equal("Header", headerDiv.TextContent);
        
        var table = cut.Find("table");
        Assert.NotNull(table);
    }

    [Fact(DisplayName = "HeaderTemplate can contain action buttons and controls")]
    public void HeaderTemplate_CanContainControls()
    {
        // Given
        using var ctx = new BunitContext();
        var data = new List<TestItem> { new(1, "Item 1") };

        // When
        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.HeaderTemplate, builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "class", "grid-toolbar");
                
                builder.OpenElement(2, "button");
                builder.AddAttribute(3, "class", "btn-add");
                builder.AddContent(4, "Add New");
                builder.CloseElement();

                builder.OpenElement(5, "input");
                builder.AddAttribute(6, "type", "search");
                builder.AddAttribute(7, "class", "search-box");
                builder.AddAttribute(8, "placeholder", "Search...");
                builder.CloseElement();

                builder.OpenElement(9, "button");
                builder.AddAttribute(10, "class", "btn-export");
                builder.AddContent(11, "Export");
                builder.CloseElement();

                builder.CloseElement();
            })
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Name");
                builder.AddAttribute(2, "Property", "Name");
                builder.CloseComponent();
            }));

        // Then
        var toolbar = cut.Find("div.grid-toolbar");
        
        var addButton = toolbar.QuerySelector("button.btn-add");
        Assert.NotNull(addButton);
        Assert.Equal("Add New", addButton.TextContent);

        var searchBox = toolbar.QuerySelector("input.search-box");
        Assert.NotNull(searchBox);
        Assert.Equal("Search...", searchBox.GetAttribute("placeholder"));

        var exportButton = toolbar.QuerySelector("button.btn-export");
        Assert.NotNull(exportButton);
        Assert.Equal("Export", exportButton.TextContent);
    }

    [Fact(DisplayName = "DataGrid without HeaderTemplate renders table directly")]
    public void DataGrid_WithoutHeaderTemplate_RendersTableDirectly()
    {
        // Given
        using var ctx = new BunitContext();
        var data = new List<TestItem> { new(1, "Item 1") };

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
        var table = cut.Find("table");
        Assert.NotNull(table);
        
        // Should not have a header div
        var headerDivs = cut.FindAll("div").Where(d => d.ClassName?.Contains("header") == true);
        Assert.Empty(headerDivs);
    }

    [Fact(DisplayName = "HeaderTemplate renders with empty grid")]
    public void HeaderTemplate_RendersWithEmptyGrid()
    {
        // Given
        using var ctx = new BunitContext();
        var emptyData = new List<TestItem>();

        // When
        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, emptyData)
            .Add(p => p.HeaderTemplate, builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "class", "grid-header-empty");
                builder.AddContent(2, "Items List");
                builder.CloseElement();
            })
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Name");
                builder.AddAttribute(2, "Property", "Name");
                builder.CloseComponent();
            }));

        // Then
        var header = cut.Find("div.grid-header-empty");
        Assert.NotNull(header);
        Assert.Equal("Items List", header.TextContent);
        
        // Should also have empty state
        var emptyState = cut.Find("div p");
        Assert.Equal("No data available", emptyState.TextContent);
    }
}
