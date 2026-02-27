using Bunit;
using TabsSut = LowlandTech.Components.Tabs.Tabs;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1050.Tabs.US01.View;

[Trait(Spec.SPEC, "1050")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Tabs component")]
public class SC01_RenderTabs : ViewScenario
{
    [Fact(DisplayName = "UAC01: Tabs renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<TabsSut>();
        cut.Find("[data-slot='tabs']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Tabs renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<TabsSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: Tabs renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<TabsSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='tabs']").ClassList.ShouldContain("test-class");
    }
}
