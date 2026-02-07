using Bunit;
using LowlandTech.Components.Components.DataGrid;
using Microsoft.AspNetCore.Components;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

/// <summary>
/// SC02: DataGrid supports column templates for custom cell rendering
/// 
/// Feature: US12.DataPresentation.feature
/// Scenario: SC02 - DataGrid supports column templates
/// 
/// Given a DataGrid with columns using custom templates
/// When the grid renders
/// Then cells display content from the template instead of raw property values
/// </summary>
public class SC02_DataGridSupportsColumnTemplates
{
    private record TestItem(int Id, string Name, decimal Price, bool IsActive);

    [Fact(DisplayName = "Column with custom Template renders using the template")]
    public void Column_WithTemplate_RendersUsingTemplate()
    {
        // Given
        using var ctx = new BunitContext();
        var data = new List<TestItem>
        {
            new(1, "Product A", 99.99m, true),
            new(2, "Product B", 149.99m, false)
        };

        // When
        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Product");
                builder.AddAttribute(2, "Property", "Name");
                builder.CloseComponent();

                builder.OpenComponent<DataGridColumn<TestItem>>(3);
                builder.AddAttribute(4, "Title", "Price");
                builder.AddAttribute(5, "Property", "Price");
                builder.AddAttribute(6, "Template", (RenderFragment<TestItem>)(item => builder =>
                {
                    builder.OpenElement(0, "strong");
                    builder.AddAttribute(1, "class", "price-value");
                    builder.AddContent(2, $"${item.Price:F2}");
                    builder.CloseElement();
                }));
                builder.CloseComponent();
            }));

        // Then
        var priceCell = cut.Find("tbody tr:first-child td:nth-child(2) strong.price-value");
        Assert.NotNull(priceCell);
        Assert.Equal("$99.99", priceCell.TextContent);
    }

    [Fact(DisplayName = "Template context provides access to row item properties")]
    public void Template_ProvidesAccessToRowItem()
    {
        // Given
        using var ctx = new BunitContext();
        var data = new List<TestItem>
        {
            new(1, "Product A", 99.99m, true),
            new(2, "Product B", 149.99m, false)
        };

        // When
        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Details");
                builder.AddAttribute(2, "Property", "Id");
                builder.AddAttribute(3, "Template", (RenderFragment<TestItem>)(item => builder =>
                {
                    builder.OpenElement(0, "div");
                    builder.AddAttribute(1, "class", "item-details");
                    builder.AddContent(2, $"#{item.Id} - {item.Name} - ${item.Price}");
                    builder.CloseElement();
                }));
                builder.CloseComponent();
            }));

        // Then
        var firstRow = cut.Find("tbody tr:first-child td .item-details");
        Assert.Contains("#1", firstRow.TextContent);
        Assert.Contains("Product A", firstRow.TextContent);
        Assert.Contains("$99.99", firstRow.TextContent);

        var secondRow = cut.Find("tbody tr:nth-child(2) td .item-details");
        Assert.Contains("#2", secondRow.TextContent);
        Assert.Contains("Product B", secondRow.TextContent);
        Assert.Contains("$149.99", secondRow.TextContent);
    }

    [Fact(DisplayName = "Template can contain interactive Blazor components")]
    public void Template_CanContainBlazorMarkup()
    {
        // Given
        using var ctx = new BunitContext();
        var data = new List<TestItem>
        {
            new(1, "Product A", 99.99m, true),
            new(2, "Product B", 149.99m, false)
        };

        // When
        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Actions");
                builder.AddAttribute(2, "Property", "Id");
                builder.AddAttribute(3, "Template", (RenderFragment<TestItem>)(item => builder =>
                {
                    builder.OpenElement(0, "button");
                    builder.AddAttribute(1, "class", "edit-btn");
                    builder.AddAttribute(2, "data-id", item.Id);
                    builder.AddContent(3, "Edit");
                    builder.CloseElement();

                    builder.AddContent(4, " ");

                    builder.OpenElement(5, "a");
                    builder.AddAttribute(6, "href", $"/product/{item.Id}");
                    builder.AddAttribute(7, "class", "view-link");
                    builder.AddContent(8, "View");
                    builder.CloseElement();
                }));
                builder.CloseComponent();
            }));

        // Then
        var firstRowCell = cut.Find("tbody tr:first-child td");
        var editButton = firstRowCell.QuerySelector("button.edit-btn");
        var viewLink = firstRowCell.QuerySelector("a.view-link");

        Assert.NotNull(editButton);
        Assert.Equal("Edit", editButton.TextContent);
        Assert.Equal("1", editButton.GetAttribute("data-id"));

        Assert.NotNull(viewLink);
        Assert.Equal("View", viewLink.TextContent);
        Assert.Equal("/product/1", viewLink.GetAttribute("href"));
    }

    [Fact(DisplayName = "Template can conditionally render based on item properties")]
    public void Template_CanConditionallyRender()
    {
        // Given
        using var ctx = new BunitContext();
        var data = new List<TestItem>
        {
            new(1, "Product A", 99.99m, true),
            new(2, "Product B", 149.99m, false)
        };

        // When
        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Status");
                builder.AddAttribute(2, "Property", "IsActive");
                builder.AddAttribute(3, "Template", (RenderFragment<TestItem>)(item => builder =>
                {
                    builder.OpenElement(0, "span");
                    builder.AddAttribute(1, "class", item.IsActive ? "status-active" : "status-inactive");
                    builder.AddContent(2, item.IsActive ? "Active" : "Inactive");
                    builder.CloseElement();
                }));
                builder.CloseComponent();
            }));

        // Then
        var firstStatus = cut.Find("tbody tr:first-child td span");
        Assert.Equal("status-active", firstStatus.ClassName);
        Assert.Equal("Active", firstStatus.TextContent);

        var secondStatus = cut.Find("tbody tr:nth-child(2) td span");
        Assert.Equal("status-inactive", secondStatus.ClassName);
        Assert.Equal("Inactive", secondStatus.TextContent);
    }

    [Fact(DisplayName = "Multiple columns can use templates simultaneously")]
    public void MultipleColumns_CanUseTemplates()
    {
        // Given
        using var ctx = new BunitContext();
        var data = new List<TestItem>
        {
            new(1, "Product A", 99.99m, true)
        };

        // When
        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.Columns, builder =>
            {
                // Column 1: Template for Name
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Product");
                builder.AddAttribute(2, "Property", "Name");
                builder.AddAttribute(3, "Template", (RenderFragment<TestItem>)(item => builder =>
                {
                    builder.OpenElement(0, "strong");
                    builder.AddContent(1, item.Name);
                    builder.CloseElement();
                }));
                builder.CloseComponent();

                // Column 2: Template for Price
                builder.OpenComponent<DataGridColumn<TestItem>>(4);
                builder.AddAttribute(5, "Title", "Price");
                builder.AddAttribute(6, "Property", "Price");
                builder.AddAttribute(7, "Template", (RenderFragment<TestItem>)(item => builder =>
                {
                    builder.OpenElement(0, "em");
                    builder.AddContent(1, $"${item.Price}");
                    builder.CloseElement();
                }));
                builder.CloseComponent();

                // Column 3: Template for Status
                builder.OpenComponent<DataGridColumn<TestItem>>(8);
                builder.AddAttribute(9, "Title", "Status");
                builder.AddAttribute(10, "Property", "IsActive");
                builder.AddAttribute(11, "Template", (RenderFragment<TestItem>)(item => builder =>
                {
                    builder.OpenElement(0, "span");
                    builder.AddContent(1, item.IsActive ? "✓" : "✗");
                    builder.CloseElement();
                }));
                builder.CloseComponent();
            }));

        // Then
        var row = cut.Find("tbody tr");
        var cells = row.QuerySelectorAll("td");

        Assert.Equal(3, cells.Length);
        Assert.NotNull(cells[0].QuerySelector("strong"));
        Assert.Equal("Product A", cells[0].TextContent);
        Assert.NotNull(cells[1].QuerySelector("em"));
        Assert.Contains("$99.99", cells[1].TextContent);
        Assert.Equal("✓", cells[2].TextContent);
    }
}
