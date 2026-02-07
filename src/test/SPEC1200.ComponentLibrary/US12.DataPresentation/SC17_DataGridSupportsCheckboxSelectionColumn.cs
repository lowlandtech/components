using Bunit;
using LowlandTech.Components.Components.DataGrid;
using Microsoft.AspNetCore.Components;
using Shouldly;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

/// <summary>
/// SC17: DataGrid supports checkbox selection column
/// 
/// Feature: US12.DataPresentation.feature
/// Scenario: SC17 - DataGrid supports checkbox selection column
/// 
/// Given I have a DataGrid with ShowSelectColumn enabled
/// When I render the DataGrid
/// Then a checkbox column should appear as the first column
/// And the header should show a "select all" checkbox
/// When I check the header checkbox
/// Then all rows should be selected
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "17")]
public class SC17_DataGridSupportsCheckboxSelectionColumn : Scenario
{
    private record TestItem(int Id, string Name);

    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(DisplayName = "Then a checkbox column should appear as the first column")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_CheckboxColumnAppearsFirst()
    {
        await ArrangeAndActAsync();
        
        // Given
        using var ctx = new BunitContext();
        var data = new List<TestItem>
        {
            new(1, "Alice"),
            new(2, "Bob")
        };

        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.ShowSelectColumn, true)
            .Add(p => p.SelectionMode, SelectionMode.Multiple)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Name");
                builder.AddAttribute(2, "Property", "Name");
                builder.CloseComponent();
            }));

        // Then - checkbox column should be first
        var headerCells = cut.FindAll("thead tr th");
        headerCells.Count.ShouldBeGreaterThan(1);
        
        // First column should contain a checkbox
        var firstHeaderCheckbox = headerCells[0].QuerySelector("input[type='checkbox']");
        firstHeaderCheckbox.ShouldNotBeNull();
        
        // Each body row should have a checkbox in the first cell
        var rows = cut.FindAll("tbody tr");
        rows.Count.ShouldBe(2);
        
        foreach (var row in rows)
        {
            var cells = row.QuerySelectorAll("td");
            var firstCellCheckbox = cells[0].QuerySelector("input[type='checkbox']");
            firstCellCheckbox.ShouldNotBeNull();
        }
    }

    [Fact(DisplayName = "And the header should show a select all checkbox")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_HeaderShowsSelectAllCheckbox()
    {
        await ArrangeAndActAsync();
        
        // Given
        using var ctx = new BunitContext();
        var data = new List<TestItem>
        {
            new(1, "Alice"),
            new(2, "Bob")
        };

        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.ShowSelectColumn, true)
            .Add(p => p.SelectionMode, SelectionMode.Multiple)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Name");
                builder.AddAttribute(2, "Property", "Name");
                builder.CloseComponent();
            }));

        // Then - header should have a select all checkbox
        var headerCheckbox = cut.Find("thead tr th input[type='checkbox']");
        headerCheckbox.ShouldNotBeNull();
    }

    [Fact(DisplayName = "Then all rows should be selected when header checkbox is checked")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_SelectAllCheckboxSelectsAllRows()
    {
        await ArrangeAndActAsync();
        
        // Given
        using var ctx = new BunitContext();
        var data = new List<TestItem>
        {
            new(1, "Alice"),
            new(2, "Bob"),
            new(3, "Charlie")
        };

        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.ShowSelectColumn, true)
            .Add(p => p.SelectionMode, SelectionMode.Multiple)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Name");
                builder.AddAttribute(2, "Property", "Name");
                builder.CloseComponent();
            }));

        // When - check the header checkbox
        cut.InvokeAsync(() => cut.Find("thead tr th input[type='checkbox']").Change(true));

        // Then - all rows should be selected
        var rows = cut.FindAll("tbody tr");
        rows.Count.ShouldBe(3);
        
        foreach (var row in rows)
        {
            row.ClassList.ShouldContain("row-selected");
        }
    }
}
