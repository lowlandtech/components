using Bunit;
using LowlandTech.Components.Popover;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1035.Popover.US01.View;

[Trait(Spec.SPEC, "1035")]
[Trait(Spec.SC, "04")]
[UserStory("01", "Render PopoverArrow component")]
public class SC04_RenderPopoverArrow : ViewScenario
{
    [Fact(DisplayName = "UAC01: PopoverArrow renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<PopoverArrow>();
        cut.Find("[data-slot='popover-arrow']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: PopoverArrow renders SVG arrow")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_RendersSvgArrow()
    {
        var cut = Ctx.Render<PopoverArrow>();
        cut.Find("svg").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC03: PopoverArrow has data-side attribute")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_HasDataSide()
    {
        var cut = Ctx.Render<PopoverArrow>(p => p.Add(x => x.Side, "top"));
        var arrow = cut.Find("[data-slot='popover-arrow']");
        arrow.GetAttribute("data-side").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC04: PopoverArrow Side parameter changes data-side")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_SideParameterChangesDataSide()
    {
        var cut = Ctx.Render<PopoverArrow>(p => p.Add(x => x.Side, "bottom"));
        var arrow = cut.Find("[data-slot='popover-arrow']");
        // When Side is bottom, arrow should point up (data-side="top")
        arrow.GetAttribute("data-side").ShouldBe("top");
    }

    [Fact(DisplayName = "UAC05: PopoverArrow has positioning style")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_HasPositioningStyle()
    {
        var cut = Ctx.Render<PopoverArrow>();
        var arrow = cut.Find("[data-slot='popover-arrow']");
        arrow.GetAttribute("style").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC06: PopoverArrow renders Class parameter")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_RendersClass()
    {
        var cut = Ctx.Render<PopoverArrow>(p => p.Add(x => x.Class, "arrow-class"));
        cut.Find("[data-slot='popover-arrow']").ClassList.ShouldContain("arrow-class");
    }

    [Fact(DisplayName = "UAC07: PopoverArrow Width and Height parameters work")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_WidthHeightParameters()
    {
        var cut = Ctx.Render<PopoverArrow>(p =>
        {
            p.Add(x => x.Width, 20);
            p.Add(x => x.Height, 10);
        });

        cut.Instance.Width.ShouldBe(20);
        cut.Instance.Height.ShouldBe(10);
    }

    [Fact(DisplayName = "UAC08: PopoverArrow points correctly for each side")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_ArrowPointsCorrectlyForEachSide()
    {
        // Test that Side="top" results in data-side="bottom" (arrow points down)
        var topCut = Ctx.Render<PopoverArrow>(p => p.Add(x => x.Side, "top"));
        topCut.Find("[data-side='bottom']").ShouldNotBeNull();

        // Test that Side="bottom" results in data-side="top" (arrow points up)
        var bottomCut = Ctx.Render<PopoverArrow>(p => p.Add(x => x.Side, "bottom"));
        bottomCut.Find("[data-side='top']").ShouldNotBeNull();

        // Test that Side="left" results in data-side="right" (arrow points right)
        var leftCut = Ctx.Render<PopoverArrow>(p => p.Add(x => x.Side, "left"));
        leftCut.Find("[data-side='right']").ShouldNotBeNull();

        // Test that Side="right" results in data-side="left" (arrow points left)
        var rightCut = Ctx.Render<PopoverArrow>(p => p.Add(x => x.Side, "right"));
        rightCut.Find("[data-side='left']").ShouldNotBeNull();
    }
}
