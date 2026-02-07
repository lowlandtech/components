using Bunit;
using LowlandTech.Components.Components.DataGrid;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

/// <summary>
/// SC03: DataGrid displays appropriate empty state when no data is available
/// 
/// Feature: US12.DataPresentation.feature
/// Scenario: SC03 - DataGrid displays empty state when no data
/// 
/// Given a DataGrid with no data or empty collection
/// When the grid renders
/// Then an appropriate empty state message or template is displayed
/// </summary>
public class SC03_DataGridDisplaysEmptyState
{
    private record TestItem(int Id, string Name);

    [Fact(DisplayName = "DataGrid with null data displays empty state")]
    public void DataGrid_WithNullData_DisplaysEmptyState()
    {
        // Given
        using var ctx = new BunitContext();

        // When
        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, (IEnumerable<TestItem>?)null)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "ID");
                builder.AddAttribute(2, "Property", "Id");
                builder.CloseComponent();
            }));

        // Then
        Assert.Throws<ElementNotFoundException>(() => cut.Find("table"));
        var emptyState = cut.Find("div p");
        Assert.Equal("No data available", emptyState.TextContent);
    }

    [Fact(DisplayName = "DataGrid with empty collection displays empty state")]
    public void DataGrid_WithEmptyCollection_DisplaysEmptyState()
    {
        // Given
        using var ctx = new BunitContext();
        var emptyData = new List<TestItem>();

        // When
        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, emptyData)
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
        Assert.Throws<ElementNotFoundException>(() => cut.Find("table"));
        var emptyState = cut.Find("div p");
        Assert.Equal("No data available", emptyState.TextContent);
    }

    [Fact(DisplayName = "Default empty state shows 'No data available' message")]
    public void DefaultEmptyState_ShowsDefaultMessage()
    {
        // Given
        using var ctx = new BunitContext();

        // When
        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, new List<TestItem>())
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Name");
                builder.AddAttribute(2, "Property", "Name");
                builder.CloseComponent();
            }));

        // Then
        var message = cut.Find("div p");
        Assert.NotNull(message);
        Assert.Equal("No data available", message.TextContent);
    }

    [Fact(DisplayName = "Custom EmptyTemplate renders when provided")]
    public void CustomEmptyTemplate_RendersWhenProvided()
    {
        // Given
        using var ctx = new BunitContext();

        // When
        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, new List<TestItem>())
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Name");
                builder.AddAttribute(2, "Property", "Name");
                builder.CloseComponent();
            })
            .Add(p => p.EmptyTemplate, builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "class", "custom-empty");
                builder.OpenElement(2, "h3");
                builder.AddContent(3, "No Records Found");
                builder.CloseElement();
                builder.OpenElement(4, "p");
                builder.AddContent(5, "Try adjusting your filters or add new items.");
                builder.CloseElement();
                builder.CloseElement();
            }));

        // Then
        var customEmpty = cut.Find("div.custom-empty");
        Assert.NotNull(customEmpty);
        
        var heading = customEmpty.QuerySelector("h3");
        Assert.NotNull(heading);
        Assert.Equal("No Records Found", heading.TextContent);

        var paragraph = customEmpty.QuerySelector("p");
        Assert.NotNull(paragraph);
        Assert.Equal("Try adjusting your filters or add new items.", paragraph.TextContent);
    }

    [Fact(DisplayName = "EmptyTemplate can contain interactive elements")]
    public void EmptyTemplate_CanContainInteractiveElements()
    {
        // Given
        using var ctx = new BunitContext();

        // When
        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, new List<TestItem>())
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Name");
                builder.AddAttribute(2, "Property", "Name");
                builder.CloseComponent();
            })
            .Add(p => p.EmptyTemplate, builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "class", "empty-with-action");
                builder.OpenElement(2, "p");
                builder.AddContent(3, "No data found");
                builder.CloseElement();
                builder.OpenElement(4, "button");
                builder.AddAttribute(5, "class", "add-item-btn");
                builder.AddContent(6, "Add New Item");
                builder.CloseElement();
                builder.OpenElement(7, "a");
                builder.AddAttribute(8, "href", "/help");
                builder.AddAttribute(9, "class", "help-link");
                builder.AddContent(10, "Need help?");
                builder.CloseElement();
                builder.CloseElement();
            }));

        // Then
        var emptyState = cut.Find("div.empty-with-action");
        
        var button = emptyState.QuerySelector("button.add-item-btn");
        Assert.NotNull(button);
        Assert.Equal("Add New Item", button.TextContent);

        var link = emptyState.QuerySelector("a.help-link");
        Assert.NotNull(link);
        Assert.Equal("Need help?", link.TextContent);
        Assert.Equal("/help", link.GetAttribute("href"));
    }

    [Fact(DisplayName = "Table is not rendered when data is empty")]
    public void Table_NotRenderedWhenDataEmpty()
    {
        // Given
        using var ctx = new BunitContext();

        // When
        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, new List<TestItem>())
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
        var tables = cut.FindAll("table");
        Assert.Empty(tables);

        var theads = cut.FindAll("thead");
        Assert.Empty(theads);

        var tbodys = cut.FindAll("tbody");
        Assert.Empty(tbodys);
    }

    [Fact(DisplayName = "Switching from data to empty shows empty state")]
    public void SwitchingFromDataToEmpty_ShowsEmptyState()
    {
        // Given
        using var ctx = new BunitContext();
        var data = new List<TestItem>
        {
            new(1, "Item 1")
        };

        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Name");
                builder.AddAttribute(2, "Property", "Name");
                builder.CloseComponent();
            }));

        // Verify table exists initially
        Assert.NotNull(cut.Find("table"));

        // When - update to empty data
        cut.Render(builder => builder
            .Add(p => p.Data, new List<TestItem>())
            .Add(p => p.Columns, colBuilder =>
            {
                colBuilder.OpenComponent<DataGridColumn<TestItem>>(0);
                colBuilder.AddAttribute(1, "Title", "Name");
                colBuilder.AddAttribute(2, "Property", "Name");
                colBuilder.CloseComponent();
            }));

        // Then
        Assert.Throws<ElementNotFoundException>(() => cut.Find("table"));
        var emptyState = cut.Find("div p");
        Assert.Equal("No data available", emptyState.TextContent);
    }
}
