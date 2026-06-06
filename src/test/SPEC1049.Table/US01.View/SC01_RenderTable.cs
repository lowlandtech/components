using Bunit;
using TableSut = LowlandTech.Components.Table;
using LowlandTech.Components;
namespace LowlandTech.Components.Tests.SPEC1049.Table.US01.View;

[Trait(Spec.SPEC, "1049")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Table component")]
public class SC01_RenderTable : ViewScenario
{
    [Fact(DisplayName = "UAC01: Table renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<TableSut>();
        cut.Find("[data-slot='table']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Table renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<TableSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: Table renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<TableSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='table']").ClassList.ShouldContain("test-class");
    }

    [Fact(DisplayName = "UAC04: Table has proper HTML structure")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_HasProperHtmlStructure()
    {
        var cut = Ctx.Render<TableSut>();
        cut.Find("table").ShouldNotBeNull();
    }
}
