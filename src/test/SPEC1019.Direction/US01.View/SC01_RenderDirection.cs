using Bunit;
using DirectionSut = LowlandTech.Components.Direction.Direction;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1019.Direction.US01.View;

[Trait(Spec.SPEC, "1019")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Direction component")]
public class SC01_RenderDirection : ViewScenario
{
    [Fact(DisplayName = "UAC01: Direction renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<DirectionSut>();
        cut.Find("[data-slot='direction-provider']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Direction renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<DirectionSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: Direction renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<DirectionSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='direction-provider']").ClassList.ShouldContain("test-class");
    }

    [Fact(DisplayName = "UAC04: Direction defaults to ltr")]
    [Trait(Spec.UAC, "04")]
    [Then("the dir attribute defaults to ltr")]
    public void UAC04_DefaultsToLtr()
    {
        var cut = Ctx.Render<DirectionSut>();
        cut.Find("[data-slot='direction-provider']").GetAttribute("dir").ShouldBe("ltr");
    }

    [Fact(DisplayName = "UAC05: Direction applies rtl dir attribute")]
    [Trait(Spec.UAC, "05")]
    [Then("the dir attribute is set to rtl")]
    public void UAC05_RendersRtl()
    {
        var cut = Ctx.Render<DirectionSut>(p => p.Add(x => x.dir, "rtl"));
        cut.Find("[data-slot='direction-provider']").GetAttribute("dir").ShouldBe("rtl");
    }
}
