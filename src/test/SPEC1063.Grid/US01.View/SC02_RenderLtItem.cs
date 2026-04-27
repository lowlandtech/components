using LowlandTech.Components.Grid;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1063.Grid.US01.View;

[Trait(Spec.SPEC, "1063")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "02")]
[UserStory("01", "Render LtItem component")]
public class SC02_RenderLtItem : ViewScenario
{
    [Fact(DisplayName = "UAC01: LtItem renders data-slot='grid-item'")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<LtItem>();
        cut.Find("[data-slot='grid-item']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: LtItem renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<LtItem>(p => p.AddChildContent("<span>hello</span>"));
        cut.Find("[data-slot='grid-item']").InnerHtml.ShouldContain("hello");
    }

    [Fact(DisplayName = "UAC03: LtItem xs=8 sets --lt-xs CSS variable")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_SetsXsVariable()
    {
        var cut = Ctx.Render<LtItem>(p => p.Add(x => x.xs, 8));
        cut.Find("[data-slot='grid-item']").GetAttribute("style").ShouldContain("--lt-xs:8");
    }

    [Fact(DisplayName = "UAC04: LtItem md=4 sets --lt-md CSS variable")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_SetsMdVariable()
    {
        var cut = Ctx.Render<LtItem>(p =>
            p.Add(x => x.xs, 12)
             .Add(x => x.md, 4));
        var style = cut.Find("[data-slot='grid-item']").GetAttribute("style")!;
        style.ShouldContain("--lt-xs:12");
        style.ShouldContain("--lt-md:4");
    }

    [Fact(DisplayName = "UAC05: LtItem with sm=0 does not set --lt-sm variable")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_ZeroSmNotSet()
    {
        var cut = Ctx.Render<LtItem>(p => p.Add(x => x.xs, 12));
        cut.Find("[data-slot='grid-item']").GetAttribute("style").ShouldNotContain("--lt-sm");
    }

    [Fact(DisplayName = "UAC06: LtItem sets all explicit breakpoints")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_SetsAllBreakpoints()
    {
        var cut = Ctx.Render<LtItem>(p =>
            p.Add(x => x.xs,  12)
             .Add(x => x.sm,  6)
             .Add(x => x.md,  4)
             .Add(x => x.lg,  3)
             .Add(x => x.xl,  2));
        var style = cut.Find("[data-slot='grid-item']").GetAttribute("style")!;
        style.ShouldContain("--lt-xs:12");
        style.ShouldContain("--lt-sm:6");
        style.ShouldContain("--lt-md:4");
        style.ShouldContain("--lt-lg:3");
        style.ShouldContain("--lt-xl:2");
    }

    [Fact(DisplayName = "UAC07: LtItem applies lt-item CSS class")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_AppliesLtItemClass()
    {
        var cut = Ctx.Render<LtItem>();
        cut.Find("[data-slot='grid-item']").ClassList.ShouldContain("lt-item");
    }

    [Fact(DisplayName = "UAC08: LtItem forwards additional attributes")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_ForwardsAdditionalAttributes()
    {
        var cut = Ctx.Render<LtItem>(p => p.AddUnmatched("data-testid", "my-item"));
        cut.Find("[data-testid='my-item']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC09: LtItem default xs is 12")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_DefaultXsIs12()
    {
        var cut = Ctx.Render<LtItem>();
        cut.Find("[data-slot='grid-item']").GetAttribute("style").ShouldContain("--lt-xs:12");
    }
}
