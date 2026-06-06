using Bunit;
using LowlandTech.Components;
using DataGridSut = LowlandTech.Components.DataGrid<LowlandTech.Components.Tests.SPEC1062.DataGrid.DataGridPerson>;

namespace LowlandTech.Components.Tests.SPEC1062.DataGrid.US01.View;

[Trait(Spec.SPEC, "1062")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render DataGrid component")]
public class SC01_RenderDataGrid : ViewScenario
{
    [Fact(DisplayName = "UAC01: DataGrid renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RendersDataSlot()
    {
        var cut = RenderGrid();

        cut.Find("[data-slot='data-grid']").ShouldNotBeNull();
        cut.Find("[data-slot='data-grid-table']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: DataGrid renders columns and rows")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_RendersColumnsAndRows()
    {
        var cut = RenderGrid();

        cut.Markup.ShouldContain("Name");
        cut.Markup.ShouldContain("Role");
        cut.FindAll("[data-slot='data-grid-row']").Count.ShouldBe(3);
        cut.Find(".cn-data-grid-sort-indicator").ClassList.ShouldContain("text-[10px]");
        cut.Find(".cn-data-grid-sort-indicator").GetAttribute("style").ShouldContain("translateY(1px)");
    }

    [Fact(DisplayName = "UAC03: DataGrid renders empty state")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_RendersEmptyState()
    {
        var cut = RenderGrid([]);

        cut.Find("[data-slot='data-grid-empty']").TextContent.ShouldContain("No results found.");
    }

    private IRenderedComponent<DataGridSut> RenderGrid(IReadOnlyList<DataGridPerson>? people = null)
    {
        return Ctx.Render<DataGridSut>(parameters => parameters
            .Add(component => component.Items, people ?? DataGridPerson.Sample)
            .Add(component => component.ChildContent, BuildColumns()));
    }

    private static RenderFragment BuildColumns()
    {
        return builder =>
        {
            builder.OpenComponent<DataGridColumn<DataGridPerson>>(0);
            builder.AddAttribute(1, nameof(DataGridColumn<DataGridPerson>.Property), nameof(DataGridPerson.Name));
            builder.AddAttribute(2, nameof(DataGridColumn<DataGridPerson>.Title), "Name");
            builder.CloseComponent();

            builder.OpenComponent<DataGridColumn<DataGridPerson>>(3);
            builder.AddAttribute(4, nameof(DataGridColumn<DataGridPerson>.Property), nameof(DataGridPerson.Role));
            builder.AddAttribute(5, nameof(DataGridColumn<DataGridPerson>.Title), "Role");
            builder.CloseComponent();
        };
    }
}

