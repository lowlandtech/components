using Bunit;
using LowlandTech.Components;
using DataGridSut = LowlandTech.Components.DataGrid<LowlandTech.Components.Tests.SPEC1062.DataGrid.DataGridPerson>;

namespace LowlandTech.Components.Tests.SPEC1062.DataGrid.US06.SelectionInteraction;

[Trait(Spec.SPEC, "1062")]
[Trait(Spec.SC, "01")]
[UserStory("06", "Selection and Interaction")]
public class SC01_SelectAll : ViewScenario
{
    [Fact(DisplayName = "UAC01: Select all checkbox selects all visible rows")]
    [Trait(Spec.UAC, "01")]
    [Given("A grid with multiple selection enabled")]
    [When("The user clicks the select-all Checkbox")]
    [Then("All visible rows should be selected")]
    public void UAC01_SelectAllSelectsVisibleRows()
    {
        var cut = RenderGrid();

        cut.Find("[data-slot='data-grid-select-all']").Click();

        cut.FindAll("[data-selected='true']").Count.ShouldBe(3);
    }

    [Fact(DisplayName = "UAC02: Unchecking select all deselects all visible rows")]
    [Trait(Spec.UAC, "02")]
    [Given("A grid with all visible rows selected")]
    [When("The user clicks the select-all Checkbox again")]
    [Then("All visible row selections should be cleared")]
    public void UAC02_UncheckSelectAllDeselectsVisibleRows()
    {
        var cut = RenderGrid();

        cut.Find("[data-slot='data-grid-select-all']").Click();
        cut.Find("[data-slot='data-grid-select-all']").Click();

        cut.FindAll("[data-selected='true']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC03: Select all is checked when all visible rows are already selected")]
    [Trait(Spec.UAC, "03")]
    [Given("A grid with multiple selection enabled")]
    [When("Each visible row Checkbox is clicked")]
    [Then("The select-all Checkbox should report a checked state")]
    public void UAC03_SelectAllCheckedWhenAllSelected()
    {
        var cut = RenderGrid();

        var rowCount = cut.FindAll("[data-slot='data-grid-row']").Count;
        for (var i = 0; i < rowCount; i++)
        {
            cut.FindAll("[data-slot='data-grid-selection-input']")[i].Click();
        }

        cut.Find("[data-slot='data-grid-select-all']").GetAttribute("data-state").ShouldBe("checked");
    }

    [Fact(DisplayName = "UAC04: Selection controls use Checkbox component")]
    [Trait(Spec.UAC, "04")]
    [Given("A grid with multiple selection enabled")]
    [When("Selection controls are rendered")]
    [Then("The grid should use the shared Checkbox component instead of native checkbox inputs")]
    public void UAC04_SelectionControlsUseCheckboxComponent()
    {
        var cut = RenderGrid();

        var selectAll = cut.Find("[data-slot='data-grid-select-all']");
        var rowSelection = cut.Find("[data-slot='data-grid-selection-input']");

        selectAll.GetAttribute("role").ShouldBe("checkbox");
        rowSelection.GetAttribute("role").ShouldBe("checkbox");
        selectAll.ClassList.ShouldContain("cn-checkbox");
        rowSelection.ClassList.ShouldContain("cn-checkbox");
        cut.FindAll("input[type='checkbox']").Count.ShouldBe(0);
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
