using Bunit;
using LowlandTech.Components.DataGrid;
using DataGridSut = LowlandTech.Components.DataGrid.DataGrid<LowlandTech.Components.Tests.SPEC1062.DataGrid.DataGridPerson>;

namespace LowlandTech.Components.Tests.SPEC1062.DataGrid.US05.ColumnManagement;

[Trait(Spec.SPEC, "1062")]
[Trait(Spec.SC, "02")]
[UserStory("05", "Column Management")]
public class SC02_FooterTemplate : ViewScenario
{
    [Fact(DisplayName = "UAC01: FooterTemplate renders in tfoot")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_FooterTemplateRendersInTfoot()
    {
        var cut = RenderGrid();

        cut.Find("[data-slot='data-grid-footer']").ShouldNotBeNull();
        cut.Find("[data-slot='data-grid-footer-cell']").TextContent.ShouldContain("3 rows total");
    }

    private IRenderedComponent<DataGridSut> RenderGrid()
    {
        return Ctx.Render<DataGridSut>(parameters => parameters
            .Add(component => component.Items, DataGridPerson.Sample)
            .Add(component => component.FooterTemplate, (RenderFragment)(b => b.AddContent(0, "3 rows total")))
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
