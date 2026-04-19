using Bunit;
using LowlandTech.Components.DataGrid;
using LowlandTech.Components.Tests.Infrastructure;
using Microsoft.AspNetCore.Components;
using DataGridSut = LowlandTech.Components.DataGrid.DataGrid<LowlandTech.Components.Tests.SPEC1062.DataGrid.DataGridPerson>;

namespace LowlandTech.Components.Tests.SPEC1062.DataGrid.US02.Behavior;

[Trait(Spec.SPEC, "1062")]
[Trait(Spec.SC, "01")]
[UserStory("02", "Interact with DataGrid component")]
public class SC01_DataGridInteractions : ViewScenario
{
    [Fact(DisplayName = "UAC01: DataGrid sorts by column")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_SortsByColumn()
    {
        var cut = RenderGrid();

        cut.Find("[data-slot='data-grid-sort-button']").Click();

        cut.FindAll("[data-slot='data-grid-row']")[0].TextContent.ShouldContain("Ada");
    }

    [Fact(DisplayName = "UAC02: DataGrid filters by column")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_FiltersByColumn()
    {
        var cut = RenderGrid(allowFiltering: true);

        cut.FindAll("[data-slot='data-grid-filter-input']")[1].Input("Engineer");

        cut.FindAll("[data-slot='data-grid-row']").Count.ShouldBe(2);
        cut.Markup.ShouldContain("Ada");
        cut.Markup.ShouldContain("Cora");
        cut.Markup.ShouldNotContain("Bea");
    }

    [Fact(DisplayName = "UAC03: DataGrid pages rows")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_PagesRows()
    {
        var cut = RenderGrid(allowPaging: true, pageSize: 2);

        cut.FindAll("[data-slot='data-grid-row']").Count.ShouldBe(2);
        cut.Find("[data-slot='data-grid-page-next']").Click();

        cut.FindAll("[data-slot='data-grid-row']").Count.ShouldBe(1);
        cut.Find("[data-slot='data-grid-row']").TextContent.ShouldContain("Bea");
    }

    private IRenderedComponent<DataGridSut> RenderGrid(
        bool allowFiltering = false,
        bool allowPaging = false,
        int pageSize = 10)
    {
        return Ctx.Render<DataGridSut>(parameters => parameters
            .Add(component => component.Items, DataGridPerson.Sample)
            .Add(component => component.AllowFiltering, allowFiltering)
            .Add(component => component.AllowPaging, allowPaging)
            .Add(component => component.PageSize, pageSize)
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
