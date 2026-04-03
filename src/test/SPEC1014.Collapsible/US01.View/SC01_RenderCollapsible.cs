using Bunit;
using CollapsibleSut = LowlandTech.Components.Collapsible.Collapsible;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1014.Collapsible.US01.View;

[Trait(Spec.SPEC, "1014")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Collapsible component")]
public class SC01_RenderCollapsible : ViewScenario
{
    [Fact(DisplayName = "UAC01: Collapsible renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<CollapsibleSut>();
        cut.Find("[data-slot='collapsible']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Collapsible renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<CollapsibleSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: Collapsible renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<CollapsibleSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='collapsible']").ClassList.ShouldContain("test-class");
    }

    [Fact(DisplayName = "UAC04: Collapsible renders IsOpen state")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_RendersIsOpenState()
    {
        var cut = Ctx.Render<CollapsibleSut>(p => p.Add(x => x.IsOpen, true));
        cut.Instance.IsOpen.ShouldBeTrue();
    }
}
