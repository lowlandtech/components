using Bunit;
using LowlandTech.Components.DataGrid;
using DataGridSut = LowlandTech.Components.DataGrid.DataGrid<LowlandTech.Components.Tests.SPEC1062.DataGrid.DataGridPerson>;

namespace LowlandTech.Components.Tests.SPEC1062.DataGrid.US04.SortingGrouping;

[Trait(Spec.SPEC, "1062")]
[Trait(Spec.SC, "01")]
[UserStory("04", "Sorting and Grouping")]
public class SC01_MultiSort : ViewScenario
{
    [Fact(DisplayName = "UAC01: Multi-sort adds secondary descriptor with Ctrl click")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_MultiSortAddsSecondaryDescriptor()
    {
        var cut = RenderGrid(allowMultiSort: true);

        cut.FindAll("[data-slot='data-grid-sort-button']")[0].Click();
        cut.FindAll("[data-slot='data-grid-sort-button']")[1].Click(new MouseEventArgs { CtrlKey = true });

        cut.FindAll("[data-sort-direction='ascending']").Count.ShouldBe(2);
    }

    [Fact(DisplayName = "UAC02: Rows are ordered by first sort then second sort")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_RowsOrderedByMultipleSorts()
    {
        var cut = RenderGrid(allowMultiSort: true);

        cut.FindAll("[data-slot='data-grid-sort-button']")[1].Click();
        cut.FindAll("[data-slot='data-grid-sort-button']")[0].Click(new MouseEventArgs { CtrlKey = true });

        var rows = cut.FindAll("[data-slot='data-grid-row']");
        rows[0].TextContent.ShouldContain("Ada");
        rows[1].TextContent.ShouldContain("Bea");
        rows[2].TextContent.ShouldContain("Cora");
    }

    [Fact(DisplayName = "UAC03: Non-Ctrl click clears previous sorts when multi-sort is enabled")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_NonCtrlClickClearsPreviousSorts()
    {
        var cut = RenderGrid(allowMultiSort: true);

        cut.FindAll("[data-slot='data-grid-sort-button']")[0].Click();
        cut.FindAll("[data-slot='data-grid-sort-button']")[1].Click();

        cut.FindAll("[data-sort-direction='ascending']").Count.ShouldBe(1);
    }

    private IRenderedComponent<DataGridSut> RenderGrid(bool allowMultiSort = false)
    {
        return Ctx.Render<DataGridSut>(parameters => parameters
            .Add(component => component.Items, DataGridPerson.Sample)
            .Add(component => component.AllowMultiSort, allowMultiSort)
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
