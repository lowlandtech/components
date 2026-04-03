using Bunit;
using KbdSut = LowlandTech.Components.Kbd.Kbd;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1029.Kbd.US02.Variants;

[Trait(Spec.SPEC, "1029")]
[Trait(Spec.SC, "02")]
[UserStory("02", "Kbd size variants")]
public class SC01_KbdSizes : ViewScenario
{
    [Fact(DisplayName = "UAC01: Size small renders correctly")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_SizeSmallRendersCorrectly()
    {
        var cut = Ctx.Render<KbdSut>(p => p.Add(x => x.Size, "sm"));
        var kbd = cut.Find("[data-slot='kbd']");

        kbd.GetAttribute("data-size").ShouldBe("sm");
        kbd.ClassList.ShouldContain("text-[10px]");
        kbd.ClassList.ShouldContain("px-1.5");
        kbd.ClassList.ShouldContain("h-4");
    }

    [Fact(DisplayName = "UAC02: Size medium renders correctly")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_SizeMediumRendersCorrectly()
    {
        var cut = Ctx.Render<KbdSut>(p => p.Add(x => x.Size, "md"));
        var kbd = cut.Find("[data-slot='kbd']");

        kbd.GetAttribute("data-size").ShouldBe("md");
        kbd.ClassList.ShouldContain("text-xs");
        kbd.ClassList.ShouldContain("px-2");
        kbd.ClassList.ShouldContain("h-5");
    }

    [Fact(DisplayName = "UAC03: Size large renders correctly")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_SizeLargeRendersCorrectly()
    {
        var cut = Ctx.Render<KbdSut>(p => p.Add(x => x.Size, "lg"));
        var kbd = cut.Find("[data-slot='kbd']");

        kbd.GetAttribute("data-size").ShouldBe("lg");
        kbd.ClassList.ShouldContain("text-sm");
        kbd.ClassList.ShouldContain("px-2.5");
        kbd.ClassList.ShouldContain("h-6");
    }

    [Fact(DisplayName = "UAC04: Default size renders medium classes")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_DefaultSizeRendersMediumClasses()
    {
        var cut = Ctx.Render<KbdSut>();
        var kbd = cut.Find("[data-slot='kbd']");

        // Should have md size classes by default
        kbd.ClassList.ShouldContain("text-xs");
        kbd.ClassList.ShouldContain("px-2");
        kbd.ClassList.ShouldContain("h-5");
    }
}
