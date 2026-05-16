using Bunit;
using LowlandTech.Components.DataGrid;
using DataGridSut = LowlandTech.Components.DataGrid.DataGrid<LowlandTech.Components.Tests.SPEC1062.DataGrid.DataGridPerson>;

namespace LowlandTech.Components.Tests.SPEC1062.DataGrid.US10.StateExportAccessibility;

[Trait(Spec.SPEC, "1062")]
[Trait(Spec.SC, "01")]
[UserStory("10", "State, Export, and Accessibility")]
public class SC01_KeyboardNavigation : ViewScenario
{
    [Fact(DisplayName = "UAC01: Cells have tabindex for keyboard navigation")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_CellsHaveTabindex()
    {
        var cut = RenderGrid();

        var cells = cut.FindAll("[data-slot='data-grid-cell']");
        cells.Count.ShouldBe(3);
        foreach (var cell in cells)
        {
            cell.GetAttribute("tabindex").ShouldBe("0");
        }
    }

    [Fact(DisplayName = "UAC02: Enter on cell toggles detail row")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_EnterTogglesDetailRow()
    {
        var cut = RenderGrid();

        var cell = cut.Find("[data-slot='data-grid-cell']");
        cell.KeyDown("Enter");

        cut.FindAll("[data-slot='data-grid-detail-row']").Count.ShouldBe(1);
    }

    [Fact(DisplayName = "UAC03: Space on cell toggles detail row")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_SpaceTogglesDetailRow()
    {
        var cut = RenderGrid();

        var cell = cut.Find("[data-slot='data-grid-cell']");
        cell.KeyDown(" ");

        cut.FindAll("[data-slot='data-grid-detail-row']").Count.ShouldBe(1);
    }

    private IRenderedComponent<DataGridSut> RenderGrid()
    {
        return Ctx.Render<DataGridSut>(parameters => parameters
            .Add(component => component.Items, DataGridPerson.Sample)
            .Add(component => component.DetailTemplate, (RenderFragment<DataGridPerson>)(person => builder => builder.AddContent(0, $"Detail: {person.Name}")))
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
        };
    }
}
