using Bunit;
using LowlandTech.Components;
namespace LowlandTech.Components.Tests.SPEC1035.Popover.US01.View;

[Trait(Spec.SPEC, "1035")]
[Trait(Spec.SC, "02")]
[UserStory("01", "Render PopoverTrigger component")]
public class SC02_RenderPopoverTrigger : ViewScenario
{
    [Fact(DisplayName = "UAC01: PopoverTrigger renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<PopoverTrigger>();
        cut.Find("[data-slot='popover-trigger']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: PopoverTrigger renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<PopoverTrigger>(p => p.AddChildContent("Click me"));
        cut.Markup.ShouldContain("Click me");
    }

    [Fact(DisplayName = "UAC03: PopoverTrigger renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<PopoverTrigger>(p => p.Add(x => x.Class, "trigger-class"));
        cut.Find("[data-slot='popover-trigger']").ClassList.ShouldContain("trigger-class");
    }
}
