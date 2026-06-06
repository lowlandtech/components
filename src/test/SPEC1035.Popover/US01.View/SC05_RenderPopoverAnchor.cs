using Bunit;
using LowlandTech.Components;
namespace LowlandTech.Components.Tests.SPEC1035.Popover.US01.View;

[Trait(Spec.SPEC, "1035")]
[Trait(Spec.SC, "05")]
[UserStory("01", "Render PopoverAnchor component")]
public class SC05_RenderPopoverAnchor : ViewScenario
{
    [Fact(DisplayName = "UAC01: PopoverAnchor renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<PopoverAnchor>();
        cut.Find("[data-slot='popover-anchor']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: PopoverAnchor renders child content")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<PopoverAnchor>(p => p.AddChildContent("Anchor"));
        cut.Find("[data-slot='popover-anchor']").TextContent.ShouldBe("Anchor");
    }
}