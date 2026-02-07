using Bunit;
using LowlandTech.Components.Components.DataGrid;
using Shouldly;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

/// <summary>
/// SC14: DataGrid supports page size selection
/// 
/// Feature: US12.DataPresentation.feature
/// Scenario: SC14 - DataGrid supports page size selection
/// 
/// Given I have a DataGrid with PageSizeOptions
/// When I select a different page size from the dropdown
/// Then the grid should display that many rows per page
/// And the pager should recalculate total pages
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "14")]
public class SC14_DataGridSupportsPageSizeSelection : Scenario
{
    private record TestItem(int Id, string Name);

    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(DisplayName = "Then the grid should display that many rows per page")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_DisplaysSelectedPageSize()
    {
        await ArrangeAndActAsync();
        
        // Given
        using var ctx = new BunitContext();
        var data = Enumerable.Range(1, 50).Select(i => new TestItem(i, $"Item {i}")).ToList();

        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.AllowPaging, true)
            .Add(p => p.PageSize, 10)
            .Add(p => p.PageSizeOptions, new[] { 10, 20, 50 })
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Name");
                builder.AddAttribute(2, "Property", "Name");
                builder.CloseComponent();
            }));

        // When - select page size 20 from dropdown
        var pageSizeSelect = cut.Find(".page-size-select");
        pageSizeSelect.Change("20");

        // Then - grid should show 20 rows
        var rows = cut.FindAll("tbody tr");
        rows.Count.ShouldBe(20);
    }

    [Fact(DisplayName = "And the pager should recalculate total pages")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_PagerRecalculatesTotalPages()
    {
        await ArrangeAndActAsync();
        
        // Given
        using var ctx = new BunitContext();
        var data = Enumerable.Range(1, 50).Select(i => new TestItem(i, $"Item {i}")).ToList();

        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.AllowPaging, true)
            .Add(p => p.PageSize, 10)
            .Add(p => p.PageSizeOptions, new[] { 10, 20, 50 })
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Name");
                builder.AddAttribute(2, "Property", "Name");
                builder.CloseComponent();
            }));

        // Original: 50 items / 10 per page = 5 pages
        var pagerInfo = cut.Find(".pager-info");
        pagerInfo.TextContent.ShouldContain("Page 1 of 5");

        // When - select page size 20
        var pageSizeSelect = cut.Find(".page-size-select");
        pageSizeSelect.Change("20");

        // Then - should recalculate: 50 items / 20 per page = 3 pages
        pagerInfo = cut.Find(".pager-info");
        pagerInfo.TextContent.ShouldContain("Page 1 of 3");
    }
}
