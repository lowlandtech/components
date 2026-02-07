using Bunit;
using LowlandTech.Components.Components.DataGrid;
using Microsoft.AspNetCore.Components;
using Shouldly;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

/// <summary>
/// SC18: DataGrid supports programmatic selection
/// 
/// Feature: US12.DataPresentation.feature
/// Scenario: SC18 - DataGrid supports programmatic selection
/// 
/// Given I have a DataGrid bound to a selection property
/// When I programmatically set selected rows
/// Then those rows should be highlighted
/// And the UI should reflect the selection state
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "18")]
public class SC18_DataGridSupportsProgrammaticSelection : Scenario
{
    private record TestItem(int Id, string Name);

    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(DisplayName = "Then rows should be highlighted when SelectedItem is set")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_SelectedItemHighlightsRow()
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

        var selectedItem = data[1]; // Bob

        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.SelectionMode, SelectionMode.Single)
            .Add(p => p.SelectedItem, selectedItem)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Name");
                builder.AddAttribute(2, "Property", "Name");
                builder.CloseComponent();
            }));

        // Then - the selected item should be highlighted
        var rows = cut.FindAll("tbody tr");
        rows.Count.ShouldBe(3);
        
        rows[0].ClassList.ShouldNotContain("row-selected");
        rows[1].ClassList.ShouldContain("row-selected"); // Bob is selected
        rows[2].ClassList.ShouldNotContain("row-selected");
    }

    [Fact(DisplayName = "And UI should reflect selection state with SelectedItems")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_SelectedItemsHighlightsMultipleRows()
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

        var selectedItems = new List<TestItem> { data[0], data[2] }; // Alice and Charlie

        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.SelectionMode, SelectionMode.Multiple)
            .Add(p => p.SelectedItems, selectedItems)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Name");
                builder.AddAttribute(2, "Property", "Name");
                builder.CloseComponent();
            }));

        // Then - the selected items should be highlighted
        var rows = cut.FindAll("tbody tr");
        rows.Count.ShouldBe(3);
        
        rows[0].ClassList.ShouldContain("row-selected"); // Alice is selected
        rows[1].ClassList.ShouldNotContain("row-selected");
        rows[2].ClassList.ShouldContain("row-selected"); // Charlie is selected
    }
}
