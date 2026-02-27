using Bunit;
using HoverCardSut = LowlandTech.Components.HoverCard.HoverCard;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1024.HoverCard.US01.View;

[Trait(Spec.SPEC, "1024")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render HoverCard component")]
public class SC01_RenderHoverCard : ViewScenario
{
    [Fact(DisplayName = "UAC01: HoverCard renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<HoverCardSut>();
        cut.Find("[data-slot='hover-card']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: HoverCard renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<HoverCardSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: HoverCard renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<HoverCardSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='hover-card']").ClassList.ShouldContain("test-class");
    }
}
