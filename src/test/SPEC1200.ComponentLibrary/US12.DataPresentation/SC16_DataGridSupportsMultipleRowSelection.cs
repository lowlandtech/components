using Bunit;
using LowlandTech.Components.Components.DataGrid;
using Microsoft.AspNetCore.Components;
using Shouldly;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

/// <summary>
/// SC16: DataGrid supports multiple row selection
/// 
/// Feature: US12.DataPresentation.feature
/// Scenario: SC16 - DataGrid supports multiple row selection
/// 
/// Given I have a DataGrid with SelectionMode Multiple
/// When I click on multiple rows
/// Then all clicked rows should be selected
/// And the RowSelect event should fire for each selection
/// When I click a selected row
/// Then it should be deselected
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "16")]
public class SC16_DataGridSupportsMultipleRowSelection : Scenario
{
    private record TestItem(int Id, string Name);

    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(DisplayName = "Then all clicked rows should be selected")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_AllClickedRowsSelected()
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
            .Add(p => p.SelectionMode, SelectionMode.Multiple)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Name");
                builder.AddAttribute(2, "Property", "Name");
                builder.CloseComponent();
            }));

        // When - click first and third rows
        cut.InvokeAsync(() => cut.FindAll("tbody tr")[0].Click());
        cut.InvokeAsync(() => cut.FindAll("tbody tr")[2].Click());

        // Then - both should be selected
        var rows = cut.FindAll("tbody tr");
        rows[0].ClassList.ShouldContain("row-selected");
        rows[1].ClassList.ShouldNotContain("row-selected");
        rows[2].ClassList.ShouldContain("row-selected");
    }

    [Fact(DisplayName = "And the RowSelect event should fire for each selection")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_RowSelectEventFiresForEach()
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

        var selectedItems = new List<TestItem>();
        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.SelectionMode, SelectionMode.Multiple)
            .Add(p => p.RowSelect, EventCallback.Factory.Create<TestItem>(this, item => selectedItems.Add(item)))
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Name");
                builder.AddAttribute(2, "Property", "Name");
                builder.CloseComponent();
            }));

        // When - click two rows
        cut.InvokeAsync(() => cut.FindAll("tbody tr")[0].Click());
        cut.InvokeAsync(() => cut.FindAll("tbody tr")[1].Click());

        // Then - event should fire twice
        selectedItems.Count.ShouldBe(2);
        selectedItems[0].Name.ShouldBe("Alice");
        selectedItems[1].Name.ShouldBe("Bob");
    }

    [Fact(DisplayName = "Then it should be deselected when clicked again")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_ClickedRowDeselected()
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
            .Add(p => p.SelectionMode, SelectionMode.Multiple)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Name");
                builder.AddAttribute(2, "Property", "Name");
                builder.CloseComponent();
            }));

        // When - click first row twice
        cut.InvokeAsync(() => cut.FindAll("tbody tr")[0].Click());
        
        // Then - should be selected
        var rows = cut.FindAll("tbody tr");
        rows[0].ClassList.ShouldContain("row-selected");

        // When - click again
        cut.InvokeAsync(() => cut.FindAll("tbody tr")[0].Click());

        // Then - should be deselected
        rows = cut.FindAll("tbody tr");
        rows[0].ClassList.ShouldNotContain("row-selected");
    }
}
