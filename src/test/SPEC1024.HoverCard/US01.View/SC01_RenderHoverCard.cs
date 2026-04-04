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

    [Fact(DisplayName = "UAC04: HoverCard has cn-hover-card and group/hover-card base classes")]
    [Trait(Spec.UAC, "04")]
    [Then("the root element has cn-hover-card and group/hover-card classes")]
    public void UAC04_HasBaseClasses()
    {
        var cut = Ctx.Render<HoverCardSut>();
        var el = cut.Find("[data-slot='hover-card']");
        el.ClassList.ShouldContain("cn-hover-card");
        el.ClassList.ShouldContain("group/hover-card");
    }

    [Fact(DisplayName = "UAC05: HoverCardTrigger renders data-slot")]
    [Trait(Spec.UAC, "05")]
    [Then("HoverCardTrigger emits data-slot='hover-card-trigger'")]
    public void UAC05_TriggerRendersDataSlot()
    {
        var cut = Ctx.Render<LowlandTech.Components.HoverCard.HoverCardTrigger>();
        cut.Find("[data-slot='hover-card-trigger']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC06: HoverCardContent renders data-slot with default side and align")]
    [Trait(Spec.UAC, "06")]
    [Then("HoverCardContent has data-slot, data-side='bottom', data-align='center'")]
    public void UAC06_ContentRendersDefaults()
    {
        var cut = Ctx.Render<LowlandTech.Components.HoverCard.HoverCardContent>();
        var el = cut.Find("[data-slot='hover-card-content']");
        el.ShouldNotBeNull();
        el.GetAttribute("data-side").ShouldBe("bottom");
        el.GetAttribute("data-align").ShouldBe("center");
        el.ClassList.ShouldContain("cn-hover-card-content");
    }
}
