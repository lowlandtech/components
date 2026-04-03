using Bunit;
using DialogSut = LowlandTech.Components.Dialog.Dialog;
using LowlandTech.Components.Dialog;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1018.Dialog.US01.View;

[Trait(Spec.SPEC, "1018")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Dialog component")]
public class SC01_RenderDialog : ViewScenario
{
    [Fact(DisplayName = "UAC01: Dialog renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<DialogSut>();
        cut.Find("[data-slot='dialog']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Dialog renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<DialogSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: Dialog renders with IsOpen false by default")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_IsOpenFalseByDefault()
    {
        var cut = Ctx.Render<DialogSut>();
        cut.Instance.IsOpen.ShouldBeFalse();
    }

    [Fact(DisplayName = "UAC04: Dialog renders with IsOpen parameter")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_RendersWithIsOpen()
    {
        var cut = Ctx.Render<DialogSut>(p => p.Add(x => x.IsOpen, true));
        cut.Instance.IsOpen.ShouldBeTrue();
    }
}
