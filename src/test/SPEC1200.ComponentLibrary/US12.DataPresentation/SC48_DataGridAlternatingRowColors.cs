using Bunit;
using LowlandTech.Components.Components.DataGrid;
using Shouldly;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "48")]
public class SC48_DataGridAlternatingRowColors : Scenario
{
    private record TestItem(int Id, string Name);

    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(DisplayName = "Then odd and even rows should have different background colors")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_OddAndEvenRowsHaveDifferentBackgroundColors()
    {
        await ArrangeAndActAsync();
        
        // Given I have a DataGrid with AllowAlternatingRows enabled
        using var ctx = new BunitContext();
        var data = new List<TestItem>
        {
            new(1, "Alice"),
            new(2, "Bob"),
            new(3, "Charlie"),
            new(4, "Diana")
        };

        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.AllowAlternatingRows, true)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Name");
                builder.AddAttribute(2, "Property", "Name");
                builder.CloseComponent();
            }));

        // Then odd and even rows should have different background colors
        var rows = cut.FindAll("tbody tr");
        rows.Count.ShouldBe(4);
        
        // Even rows (index 0, 2) should have row-even class
        rows[0].ClassList.ShouldContain("row-even");
        rows[2].ClassList.ShouldContain("row-even");
        
        // Odd rows (index 1, 3) should have row-odd class
        rows[1].ClassList.ShouldContain("row-odd");
        rows[3].ClassList.ShouldContain("row-odd");
    }

    [Fact(DisplayName = "And it should improve readability")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_ImprovesReadability()
    {
        await ArrangeAndActAsync();
        
        // When AllowAlternatingRows is false, no alternating classes
        using var ctx = new BunitContext();
        var data = new List<TestItem>
        {
            new(1, "Alice"),
            new(2, "Bob")
        };

        var cut = ctx.Render<DataGrid<TestItem>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.AllowAlternatingRows, false)
            .Add(p => p.Columns, builder =>
            {
                builder.OpenComponent<DataGridColumn<TestItem>>(0);
                builder.AddAttribute(1, "Title", "Name");
                builder.AddAttribute(2, "Property", "Name");
                builder.CloseComponent();
            }));

        // Then rows should not have alternating classes
        var rows = cut.FindAll("tbody tr");
        rows[0].ClassList.ShouldNotContain("row-even");
        rows[0].ClassList.ShouldNotContain("row-odd");
        rows[1].ClassList.ShouldNotContain("row-even");
        rows[1].ClassList.ShouldNotContain("row-odd");
    }
}
