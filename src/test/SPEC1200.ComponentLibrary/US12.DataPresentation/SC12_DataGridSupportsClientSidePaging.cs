using Bunit;
using LowlandTech.Components.Components.DataGrid;
using Shouldly;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

/// <summary>
/// SC12: DataGrid supports client-side paging
/// 
/// Feature: US12.DataPresentation.feature
/// Scenario: SC12 - DataGrid supports client-side paging
/// 
/// Given I have a DataGrid specification with AllowPaging enabled
/// And PageSize is set to 10
/// And I have 50 data items
/// When I render the DataGrid
/// Then only 10 rows should be visible
/// And a pager should appear showing "1 of 5 pages"
/// When I click the next page button
/// Then rows 11-20 should be visible
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "12")]
public class SC12_DataGridSupportsClientSidePaging : Scenario
{
    private record TestItem(int Id, string Name);

    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(DisplayName = "Then only 10 rows should be visible")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_OnlyPageSizeRowsVisible()
    {
        await ArrangeAndActAsync();
        
        // Given
        using var ctx = new BunitContext();
        var data = Enumerable.Range(1, 50)
            .Select(i => new TestItem(i, $"Item {i}"))
            .ToList();

        // When
        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.AllowPaging, true)
            .Add(p => p.PageSize, 10)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Name");
                builder.AddAttribute(2, "Property", "Name");
                builder.CloseComponent();
            }));

        // Then - only 10 rows should be visible
        var rows = cut.FindAll("tbody tr");
        rows.Count.ShouldBe(10);
        
        // And the first row should be "Item 1"
        rows[0].TextContent.ShouldContain("Item 1");
        // And the last row should be "Item 10"
        rows[9].TextContent.ShouldContain("Item 10");
    }

    [Fact(DisplayName = "And a pager should appear showing page info")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_PagerShowsPageInfo()
    {
        await ArrangeAndActAsync();
        
        // Given
        using var ctx = new BunitContext();
        var data = Enumerable.Range(1, 50)
            .Select(i => new TestItem(i, $"Item {i}"))
            .ToList();

        // When
        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.AllowPaging, true)
            .Add(p => p.PageSize, 10)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Name");
                builder.AddAttribute(2, "Property", "Name");
                builder.CloseComponent();
            }));

        // Then - pager should appear with page info
        var pager = cut.Find(".data-grid-pager");
        pager.ShouldNotBeNull();
        pager.TextContent.ShouldContain("1");
        pager.TextContent.ShouldContain("5"); // 50 items / 10 per page = 5 pages
    }

    [Fact(DisplayName = "Then rows 11-20 should be visible after clicking next")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_NextPageShowsCorrectRows()
    {
        await ArrangeAndActAsync();
        
        // Given
        using var ctx = new BunitContext();
        var data = Enumerable.Range(1, 50)
            .Select(i => new TestItem(i, $"Item {i}"))
            .ToList();

        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.AllowPaging, true)
            .Add(p => p.PageSize, 10)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Name");
                builder.AddAttribute(2, "Property", "Name");
                builder.CloseComponent();
            }));

        // When - click next page button
        var nextButton = cut.Find(".pager-next");
        nextButton.Click();

        // Then - rows 11-20 should be visible
        var rows = cut.FindAll("tbody tr");
        rows.Count.ShouldBe(10);
        rows[0].TextContent.ShouldContain("Item 11");
        rows[9].TextContent.ShouldContain("Item 20");
    }
}
