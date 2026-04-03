using Bunit;
using AlertDialogSut = LowlandTech.Components.AlertDialog.AlertDialog;
using LowlandTech.Components.AlertDialog;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1002.AlertDialog.US01.View;

[Trait(Spec.SPEC, "1002")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render AlertDialog component")]
public class SC01_RenderAlertDialog : ViewScenario
{
    [Fact(DisplayName = "UAC01: AlertDialog renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<AlertDialogSut>();
        cut.Find("[data-slot='alert-dialog']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: AlertDialog renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<AlertDialogSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: AlertDialog renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<AlertDialogSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='alert-dialog']").ClassList.ShouldContain("test-class");
    }

    [Fact(DisplayName = "UAC04: AlertDialog renders with IsOpen false by default")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_IsOpenFalseByDefault()
    {
        var cut = Ctx.Render<AlertDialogSut>();
        cut.Instance.IsOpen.ShouldBeFalse();
    }

    [Fact(DisplayName = "UAC05: AlertDialog renders with IsOpen parameter")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_RendersWithIsOpen()
    {
        var cut = Ctx.Render<AlertDialogSut>(p => p.Add(x => x.IsOpen, true));
        cut.Instance.IsOpen.ShouldBeTrue();
    }
}
