using Bunit;
using PopoverSut = LowlandTech.Components.Popover.Popover;

namespace LowlandTech.Components.Tests.SPEC1035.Popover.US01.View;

[Trait(Spec.SPEC, "1035")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Popover component")]
public class SC01_RenderPopover : ViewScenario
{
    [Fact(DisplayName = "UAC01: Popover renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<PopoverSut>();
        cut.Find("[data-slot='popover']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Popover renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<PopoverSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: Popover renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<PopoverSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='popover']").ClassList.ShouldContain("test-class");
    }

    [Fact(DisplayName = "UAC04: Popover has IsOpen property")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_HasIsOpenProperty()
    {
        var cut = Ctx.Render<PopoverSut>();
        cut.Instance.IsOpen.ShouldBeFalse();
    }
}
