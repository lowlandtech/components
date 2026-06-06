using Bunit;
using LowlandTech.Components;
using DataGridSut = LowlandTech.Components.DataGrid<LowlandTech.Components.Tests.SPEC1062.DataGrid.DataGridPerson>;

namespace LowlandTech.Components.Tests.SPEC1062.DataGrid.US05.ColumnManagement;

[Trait(Spec.SPEC, "1062")]
[Trait(Spec.SC, "01")]
[UserStory("05", "Column Management")]
public class SC01_HeaderTemplate : ViewScenario
{
    [Fact(DisplayName = "UAC01: HeaderTemplate renders custom content")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_HeaderTemplateRendersCustomContent()
    {
        var cut = RenderGrid();

        var headerCell = cut.FindAll("[data-slot='data-grid-header-cell']")[0];
        headerCell.TextContent.ShouldContain("Custom Name Header");
    }

    [Fact(DisplayName = "UAC02: HeaderTemplate preserves sort button when column is sortable")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_HeaderTemplatePreservesSortButton()
    {
        var cut = RenderGrid();

        cut.FindAll("[data-slot='data-grid-sort-button']").Count.ShouldBe(1);
        cut.Markup.ShouldContain("Custom Name Header");
    }

    private IRenderedComponent<DataGridSut> RenderGrid()
    {
        return Ctx.Render<DataGridSut>(parameters => parameters
            .Add(component => component.Items, DataGridPerson.Sample)
            .Add(component => component.ChildContent, BuildColumns()));
    }

    private static RenderFragment BuildColumns()
    {
        return builder =>
        {
            builder.OpenComponent<DataGridColumn<DataGridPerson>>(0);
            builder.AddAttribute(1, nameof(DataGridColumn<DataGridPerson>.Property), nameof(DataGridPerson.Name));
            builder.AddAttribute(2, nameof(DataGridColumn<DataGridPerson>.Title), "Name");
            builder.AddAttribute(3, nameof(DataGridColumn<DataGridPerson>.HeaderTemplate), (RenderFragment)(b => b.AddContent(4, "Custom Name Header")));
            builder.CloseComponent();

            builder.OpenComponent<DataGridColumn<DataGridPerson>>(5);
            builder.AddAttribute(6, nameof(DataGridColumn<DataGridPerson>.Property), nameof(DataGridPerson.Role));
            builder.AddAttribute(7, nameof(DataGridColumn<DataGridPerson>.Title), "Role");
            builder.CloseComponent();
        };
    }
}
