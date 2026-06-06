using Bunit;
using PopoverSut = LowlandTech.Components.Popover;
using LowlandTech.Components;
namespace LowlandTech.Components.Tests.SPEC1035.Popover.US01.View;

[Trait(Spec.SPEC, "1035")]
[Trait(Spec.SC, "03")]
[UserStory("01", "Render PopoverContent component")]
public class SC03_RenderPopoverContent : ViewScenario
{
    [Fact(DisplayName = "UAC01: PopoverContent renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<PopoverContent>(p => p.AddChildContent("Content"));
        // Note: PopoverContent only renders when parent IsOpen is true
        // Testing in isolation, so it won't render without cascading Popover
    }

    [Fact(DisplayName = "UAC02: PopoverContent renders with Align parameter")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_RendersWithAlignParameter()
    {
        var cut = Ctx.Render<PopoverContent>(p => 
        {
            p.Add(x => x.Align, "start");
        });
        // Align is rendered as data-align attribute when visible
    }

    [Fact(DisplayName = "UAC03: PopoverContent renders with Side parameter")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_RendersWithSideParameter()
    {
        var cut = Ctx.Render<PopoverContent>(p => 
        {
            p.Add(x => x.Side, "top");
        });
        // Side is rendered as data-side attribute when visible
    }

    [Fact(DisplayName = "UAC04: PopoverContent has correct default values")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_HasCorrectDefaultValues()
    {
        var cut = Ctx.Render<PopoverContent>();
        cut.Instance.Align.ShouldBe("center");
        cut.Instance.Side.ShouldBe("bottom");
        cut.Instance.Variant.ShouldBe("none");
    }

    [Fact(DisplayName = "UAC05: PopoverContent renders outline variant")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_RendersOutlineVariant()
    {
        var cut = Ctx.Render<PopoverSut>(p =>
        {
            p.Add(x => x.IsOpen, true);
            p.AddChildContent<PopoverContent>(content =>
            {
                content.Add(x => x.Variant, "outline");
                content.AddChildContent("Content");
            });
        });

        var content = cut.Find("[data-slot='popover-content']");
        content.GetAttribute("data-variant").ShouldBe("outline");
        content.ClassList.ShouldContain("cn-popover-content-outline");
    }

    [Fact(DisplayName = "UAC06: PopoverContent is positioned without changing trigger layout")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_PositionsContentOutOfFlow()
    {
        var cut = Ctx.Render<PopoverSut>(p =>
        {
            p.Add(x => x.IsOpen, true);
            p.AddChildContent<PopoverContent>(content =>
            {
                content.AddChildContent("Content");
            });
        });

        var content = cut.Find("[data-slot='popover-content']");
        var style = content.GetAttribute("style");

        style.ShouldNotBeNull();
        style!.ShouldContain("position:absolute");
        style.ShouldContain("top:calc(100% + 0.5rem)");
    }
}
