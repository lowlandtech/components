using Bunit;
using LowlandTech.Components;
using DataGridSut = LowlandTech.Components.DataGrid<LowlandTech.Components.Tests.SPEC1062.DataGrid.DataGridPerson>;

namespace LowlandTech.Components.Tests.SPEC1062.DataGrid.US08.HierarchyTemplates;

[Trait(Spec.SPEC, "1062")]
[Trait(Spec.SC, "02")]
[UserStory("08", "Hierarchy and Templates")]
public class SC02_DensityGridLines : ViewScenario
{
    [Fact(DisplayName = "UAC01: Density attribute is rendered on grid root")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_DensityAttributeRendered()
    {
        var cut = RenderGrid(density: DataGridDensity.Compact);

        cut.Find("[data-density='compact']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: GridLines attribute is rendered on grid root")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_GridLinesAttributeRendered()
    {
        var cut = RenderGrid(gridLines: DataGridGridLines.Both);

        cut.Find("[data-grid-lines='both']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC03: Default density is default and grid lines is horizontal")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_Defaults()
    {
        var cut = RenderGrid();

        cut.Find("[data-density='default']").ShouldNotBeNull();
        cut.Find("[data-grid-lines='horizontal']").ShouldNotBeNull();
    }

    private IRenderedComponent<DataGridSut> RenderGrid(
        DataGridDensity density = DataGridDensity.Default,
        DataGridGridLines gridLines = DataGridGridLines.Horizontal)
    {
        return Ctx.Render<DataGridSut>(parameters => parameters
            .Add(component => component.Items, DataGridPerson.Sample)
            .Add(component => component.Density, density)
            .Add(component => component.GridLines, gridLines)
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
