using Bunit;
using LowlandTech.Components.DataGrid;
using DataGridSut = LowlandTech.Components.DataGrid.DataGrid<LowlandTech.Components.Tests.SPEC1062.DataGrid.DataGridPerson>;

namespace LowlandTech.Components.Tests.SPEC1062.DataGrid.US08.HierarchyTemplates;

[Trait(Spec.SPEC, "1062")]
[Trait(Spec.SC, "03")]
[UserStory("08", "Hierarchy and Templates")]
public class SC03_EmptyText : ViewScenario
{
    [Fact(DisplayName = "UAC01: EmptyText renders when items are empty")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_EmptyTextRendersWhenEmpty()
    {
        var cut = RenderGrid(emptyText: "No people found");

        cut.Find("[data-slot='data-grid-empty']").TextContent.ShouldContain("No people found");
    }

    [Fact(DisplayName = "UAC02: EmptyTemplate takes precedence over EmptyText")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_EmptyTemplateTakesPrecedence()
    {
        var cut = RenderGrid(
            emptyText: "No people found",
            emptyTemplate: builder => builder.AddContent(0, "Custom empty"));

        cut.Find("[data-slot='data-grid-empty']").TextContent.ShouldContain("Custom empty");
        cut.Find("[data-slot='data-grid-empty']").TextContent.ShouldNotContain("No people found");
    }

    private IRenderedComponent<DataGridSut> RenderGrid(
        string? emptyText = null,
        RenderFragment? emptyTemplate = null)
    {
        return Ctx.Render<DataGridSut>(parameters =>
        {
            parameters.Add(component => component.Items, Array.Empty<DataGridPerson>());
            parameters.Add(component => component.EmptyText, emptyText);
            parameters.Add(component => component.ChildContent, BuildColumns());
            if (emptyTemplate is not null)
            {
                parameters.Add(component => component.EmptyTemplate, emptyTemplate);
            }
        });
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
