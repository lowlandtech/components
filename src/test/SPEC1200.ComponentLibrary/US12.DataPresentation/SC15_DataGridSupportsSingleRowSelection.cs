using Bunit;
using LowlandTech.Components.Components.DataGrid;
using Microsoft.AspNetCore.Components;
using Shouldly;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

/// <summary>
/// SC15: DataGrid supports single row selection
/// 
/// Feature: US12.DataPresentation.feature
/// Scenario: SC15 - DataGrid supports single row selection
/// 
/// Given I have a DataGrid with SelectionMode Single
/// When I click on a row
/// Then the row should be highlighted as selected
/// And the RowSelect event should fire with the selected item
/// When I click another row
/// Then the previous row should be deselected
/// And the new row should be selected
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "15")]
public class SC15_DataGridSupportsSingleRowSelection : Scenario
{
    private record TestItem(int Id, string Name);

    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(DisplayName = "Then the row should be highlighted as selected")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_RowHighlightedWhenSelected()
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

        // When
        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.SelectionMode, SelectionMode.Single)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Name");
                builder.AddAttribute(2, "Property", "Name");
                builder.CloseComponent();
            }));

        // Click the first row
        var rows = cut.FindAll("tbody tr");
        rows[0].Click();

        // Then - row should have selected class
        rows = cut.FindAll("tbody tr");
        rows[0].ClassList.ShouldContain("row-selected");
        rows[1].ClassList.ShouldNotContain("row-selected");
        rows[2].ClassList.ShouldNotContain("row-selected");
    }

    [Fact(DisplayName = "And the RowSelect event should fire with the selected item")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_RowSelectEventFires()
    {
        await ArrangeAndActAsync();
        
        // Given
        using var ctx = new BunitContext();
        var data = new List<TestItem>
        {
            new(1, "Alice"),
            new(2, "Bob")
        };

        TestItem? selectedItem = null;
        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.SelectionMode, SelectionMode.Single)
            .Add(p => p.RowSelect, EventCallback.Factory.Create<TestItem>(this, item => selectedItem = item))
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Name");
                builder.AddAttribute(2, "Property", "Name");
                builder.CloseComponent();
            }));

        // When - click the second row
        var rows = cut.FindAll("tbody tr");
        rows[1].Click();

        // Then - event should fire with correct item
        selectedItem.ShouldNotBeNull();
        selectedItem.Id.ShouldBe(2);
        selectedItem.Name.ShouldBe("Bob");
    }

    [Fact(DisplayName = "Then the previous row should be deselected")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_PreviousRowDeselected()
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
            .Add(p => p.SelectionMode, SelectionMode.Single)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Name");
                builder.AddAttribute(2, "Property", "Name");
                builder.CloseComponent();
            }));

        // When - click first row
        var rows = cut.FindAll("tbody tr");
        rows[0].Click();

        // Then - first row should be selected
        rows = cut.FindAll("tbody tr");
        rows[0].ClassList.ShouldContain("row-selected");

        // When - click second row
        rows[1].Click();

        // Then - only second row should be selected
        rows = cut.FindAll("tbody tr");
        rows[0].ClassList.ShouldNotContain("row-selected");
        rows[1].ClassList.ShouldContain("row-selected");
        rows[2].ClassList.ShouldNotContain("row-selected");
    }
}
