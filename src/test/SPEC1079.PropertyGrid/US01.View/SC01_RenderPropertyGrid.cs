using Bunit;
using PropertyGridSut = LowlandTech.Components.PropertyGrid<LowlandTech.Components.Tests.SPEC1079.PropertyGrid.PropertyField>;

namespace LowlandTech.Components.Tests.SPEC1079.PropertyGrid.US01.View;

[Trait(Spec.SPEC, "1079")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render PropertyGrid component")]
public class SC01_RenderPropertyGrid : ViewScenario
{
    [Fact(DisplayName = "UAC01: PropertyGrid renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = RenderGrid();
        cut.Find("[data-slot='property-grid']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: PropertyGrid renders rows with labels")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains property rows")]
    public void UAC02_RendersRowsWithLabels()
    {
        var cut = RenderGrid();

        cut.Markup.ShouldContain("Name");
        cut.Markup.ShouldContain("Email");
        cut.FindAll("[data-slot='property-grid-row']").Count.ShouldBe(3);
    }

    [Fact(DisplayName = "UAC03: PropertyGrid renders editor template content")]
    [Trait(Spec.UAC, "03")]
    [Then("the editor column contains rendered content")]
    public void UAC03_RendersEditorTemplate()
    {
        var cut = RenderGrid();
        cut.FindAll("[data-slot='property-grid-editor']").Count.ShouldBe(3);
    }

    [Fact(DisplayName = "UAC04: PropertyGrid renders empty state")]
    [Trait(Spec.UAC, "04")]
    [Then("the empty message is displayed")]
    public void UAC04_RendersEmptyState()
    {
        var cut = RenderGrid([], emptyText: "No properties.");
        cut.Find("[data-slot='property-grid-empty']").TextContent.ShouldContain("No properties.");
    }

    [Fact(DisplayName = "UAC05: PropertyGrid renders badges")]
    [Trait(Spec.UAC, "05")]
    [Then("badges are rendered in rows")]
    public void UAC05_RendersBadges()
    {
        var cut = RenderGrid();
        cut.FindAll("[data-slot='property-grid-badge']").Count.ShouldBe(1);
        cut.Find("[data-slot='property-grid-badge']").TextContent.ShouldBe("user");
    }

    [Fact(DisplayName = "UAC06: PropertyGrid renders read-only attribute")]
    [Trait(Spec.UAC, "06")]
    [Then("read-only rows have data-readonly attribute")]
    public void UAC06_RendersReadOnlyAttribute()
    {
        var cut = RenderGrid();
        var rows = cut.FindAll("[data-slot='property-grid-row']");
        rows[2].GetAttribute("data-readonly").ShouldBe("true");
    }

    [Fact(DisplayName = "UAC07: PropertyGrid renders header")]
    [Trait(Spec.UAC, "07")]
    [Then("the header slot is rendered")]
    public void UAC07_RendersHeader()
    {
        var cut = Ctx.Render<PropertyGridSut>(p => p
            .Add(x => x.Items, PropertyField.Sample)
            .Add(x => x.LabelSelector, (PropertyField f) => f.Label)
            .Add(x => x.Header, (RenderFragment)(builder => builder.AddContent(0, "Section Header"))));

        cut.Find("[data-slot='property-grid-header']").TextContent.ShouldContain("Section Header");
    }

    [Fact(DisplayName = "UAC08: PropertyGrid applies Class parameter")]
    [Trait(Spec.UAC, "08")]
    [Then("the custom CSS class is present")]
    public void UAC08_RendersClass()
    {
        var cut = Ctx.Render<PropertyGridSut>(p => p
            .Add(x => x.Items, PropertyField.Sample)
            .Add(x => x.LabelSelector, (PropertyField f) => f.Label)
            .Add(x => x.Class, "my-grid"));

        cut.Find("[data-slot='property-grid']").ClassList.ShouldContain("my-grid");
    }

    private IRenderedComponent<PropertyGridSut> RenderGrid(
        IReadOnlyList<PropertyField>? fields = null,
        string? emptyText = null)
    {
        return Ctx.Render<PropertyGridSut>(p => p
            .Add(x => x.Items, fields ?? PropertyField.Sample)
            .Add(x => x.LabelSelector, (PropertyField f) => f.Label)
            .Add(x => x.BadgeSelector, (PropertyField f) => f.Badge)
            .Add(x => x.IsReadOnlySelector, (PropertyField f) => f.IsReadOnly)
            .Add(x => x.EditorTemplate, (PropertyField field) => (RenderFragment)(builder => builder.AddContent(0, field.Value)))
            .Add(x => x.EmptyText, emptyText));
    }
}
