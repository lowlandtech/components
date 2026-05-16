using Bunit;
using LowlandTech.Components.DataGrid;
using DataGridSut = LowlandTech.Components.DataGrid.DataGrid<LowlandTech.Components.Tests.SPEC1062.DataGrid.DataGridPerson>;

namespace LowlandTech.Components.Tests.SPEC1062.DataGrid.US06.SelectionInteraction;

[Trait(Spec.SPEC, "1062")]
[Trait(Spec.SC, "01")]
[UserStory("06", "Selection and Interaction")]
public class SC01_SelectAll : ViewScenario
{
    [Fact(DisplayName = "UAC01: Select all checkbox selects all visible rows")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_SelectAllSelectsVisibleRows()
    {
        var cut = RenderGrid();

        cut.Find("[data-slot='data-grid-select-all']").Change(true);

        cut.FindAll("[data-selected='true']").Count.ShouldBe(3);
    }

    [Fact(DisplayName = "UAC02: Unchecking select all deselects all visible rows")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_UncheckSelectAllDeselectsVisibleRows()
    {
        var cut = RenderGrid();

        cut.Find("[data-slot='data-grid-select-all']").Change(true);
        cut.Find("[data-slot='data-grid-select-all']").Change(false);

        cut.FindAll("[data-selected='true']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC03: Select all is checked when all visible rows are already selected")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_SelectAllCheckedWhenAllSelected()
    {
        var cut = RenderGrid();

        foreach (var checkbox in cut.FindAll("[data-slot='data-grid-selection-input']"))
        {
            checkbox.Change(true);
        }

        cut.Find("[data-slot='data-grid-select-all']").GetAttribute("checked").ShouldNotBeNull();
    }

    private IRenderedComponent<DataGridSut> RenderGrid()
    {
        return Ctx.Render<DataGridSut>(parameters => parameters
            .Add(component => component.Items, DataGridPerson.Sample)
            .Add(component => component.SelectionMode, DataGridSelectionMode.Multiple)
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
