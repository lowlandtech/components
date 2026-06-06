using Bunit;
using LowlandTech.Components;
using DataGridSut = LowlandTech.Components.DataGrid<LowlandTech.Components.Tests.SPEC1062.DataGrid.DataGridPerson>;

namespace LowlandTech.Components.Tests.SPEC1062.DataGrid.US08.HierarchyTemplates;

[Trait(Spec.SPEC, "1062")]
[Trait(Spec.SC, "01")]
[UserStory("08", "Hierarchy and Templates")]
public class SC01_DetailTemplate : ViewScenario
{
    [Fact(DisplayName = "UAC01: Detail row renders when expand button clicked")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_DetailRowRendersWhenExpanded()
    {
        var cut = RenderGrid();

        cut.FindAll("[data-slot='data-grid-detail-row']").Count.ShouldBe(0);

        cut.Find("[data-slot='data-grid-expand-button']").Click();

        cut.FindAll("[data-slot='data-grid-detail-row']").Count.ShouldBe(1);
        cut.Find("[data-slot='data-grid-detail-cell']").TextContent.ShouldContain("Detail: Cora");
    }

    [Fact(DisplayName = "UAC02: Expand button toggles aria-expanded")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_ExpandButtonTogglesAriaExpanded()
    {
        var cut = RenderGrid();
        var button = cut.Find("[data-slot='data-grid-expand-button']");

        button.GetAttribute("aria-expanded").ShouldBe("false");

        button.Click();

        cut.Find("[data-slot='data-grid-expand-button']").GetAttribute("aria-expanded").ShouldBe("true");
    }

    [Fact(DisplayName = "UAC03: Collapsing hides detail row")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_CollapsingHidesDetailRow()
    {
        var cut = RenderGrid();

        cut.Find("[data-slot='data-grid-expand-button']").Click();
        cut.Find("[data-slot='data-grid-expand-button']").Click();

        cut.FindAll("[data-slot='data-grid-detail-row']").Count.ShouldBe(0);
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
