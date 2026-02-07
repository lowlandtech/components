using Bunit;
using LowlandTech.Components.Components.DataGrid;
using Shouldly;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

/// <summary>
/// SC13: DataGrid pager supports top and bottom positions
/// 
/// Feature: US12.DataPresentation.feature
/// Scenario: SC13 - DataGrid pager supports top and bottom positions
/// 
/// Given I have a DataGrid with PagerPosition set to TopAndBottom
/// When I render the DataGrid
/// Then a pager should appear above the grid
/// And a pager should appear below the grid
/// And both pagers should synchronize
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "13")]
public class SC13_DataGridPagerPositions : Scenario
{
    private record TestItem(int Id, string Name);

    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(DisplayName = "Then a pager should appear above the grid")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_PagerAppearsAboveGrid()
    {
        await ArrangeAndActAsync();
        
        // Given
        using var ctx = new BunitContext();
        var data = Enumerable.Range(1, 30).Select(i => new TestItem(i, $"Item {i}")).ToList();

        // When
        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.AllowPaging, true)
            .Add(p => p.PageSize, 10)
            .Add(p => p.PagerPosition, PagerPosition.Top)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Name");
                builder.AddAttribute(2, "Property", "Name");
                builder.CloseComponent();
            }));

        // Then
        var pagers = cut.FindAll(".data-grid-pager");
        pagers.Count.ShouldBe(1);
        
        // Pager should appear before the table (check index in parent)
        var html = cut.Markup;
        var pagerIndex = html.IndexOf("data-grid-pager");
        var tableIndex = html.IndexOf("<table");
        pagerIndex.ShouldBeLessThan(tableIndex);
    }

    [Fact(DisplayName = "And a pager should appear below the grid")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_PagerAppearsBelowGrid()
    {
        await ArrangeAndActAsync();
        
        // Given
        using var ctx = new BunitContext();
        var data = Enumerable.Range(1, 30).Select(i => new TestItem(i, $"Item {i}")).ToList();

        // When
        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.AllowPaging, true)
            .Add(p => p.PageSize, 10)
            .Add(p => p.PagerPosition, PagerPosition.Bottom)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Name");
                builder.AddAttribute(2, "Property", "Name");
                builder.CloseComponent();
            }));

        // Then
        var pagers = cut.FindAll(".data-grid-pager");
        pagers.Count.ShouldBe(1);
        
        // Pager should appear after the table (check index in parent)
        var html = cut.Markup;
        var pagerIndex = html.IndexOf("data-grid-pager");
        var tableIndex = html.IndexOf("<table");
        pagerIndex.ShouldBeGreaterThan(tableIndex);
    }

    [Fact(DisplayName = "And both pagers should synchronize")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_PagersSync()
    {
        await ArrangeAndActAsync();
        
        // Given
        using var ctx = new BunitContext();
        var data = Enumerable.Range(1, 30).Select(i => new TestItem(i, $"Item {i}")).ToList();

        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.AllowPaging, true)
            .Add(p => p.PageSize, 10)
            .Add(p => p.PagerPosition, PagerPosition.TopAndBottom)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Name");
                builder.AddAttribute(2, "Property", "Name");
                builder.CloseComponent();
            }));

        // Then - both pagers should exist
        var pagers = cut.FindAll(".data-grid-pager");
        pagers.Count.ShouldBe(2);

        // When - click next on first pager
        var nextButtons = cut.FindAll(".pager-next");
        nextButtons[0].Click();

        // Then - both pagers should show page 2
        var pageInfos = cut.FindAll(".pager-info");
        pageInfos[0].TextContent.ShouldContain("Page 2");
        pageInfos[1].TextContent.ShouldContain("Page 2");
    }
}
