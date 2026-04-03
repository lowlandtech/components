using Bunit;
using AlertDialogSut = LowlandTech.Components.AlertDialog.AlertDialog;
using LowlandTech.Components.AlertDialog;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1002.AlertDialog.US01.View;

[Trait(Spec.SPEC, "1002")]
[Trait(Spec.SC, "02")]
[UserStory("01", "Render AlertDialog subcomponents")]
public class SC02_RenderAlertDialogSubcomponents : ViewScenario
{
    [Fact(DisplayName = "UAC01: AlertDialogTrigger renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_TriggerRendersDataSlot()
    {
        var cut = Ctx.Render<AlertDialogTrigger>(p => p.AddChildContent("Open"));
        cut.Find("[data-slot='alert-dialog-trigger']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: AlertDialogTrigger renders as button")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_TriggerRendersAsButton()
    {
        var cut = Ctx.Render<AlertDialogTrigger>(p => p.AddChildContent("Open"));
        cut.Find("button").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC03: AlertDialogContent renders when IsOpen true")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_ContentRendersWhenOpen()
    {
        var cut = Ctx.Render<AlertDialogSut>(p =>
        {
            p.Add(x => x.IsOpen, true);
            p.AddChildContent<AlertDialogContent>(cp => cp.AddChildContent("Content"));
        });
        cut.Markup.ShouldContain("Content");
        cut.Find("[data-slot='alert-dialog-content']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC04: AlertDialogContent does not render when IsOpen false")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_ContentDoesNotRenderWhenClosed()
    {
        var cut = Ctx.Render<AlertDialogSut>(p =>
        {
            p.Add(x => x.IsOpen, false);
            p.AddChildContent<AlertDialogContent>(cp => cp.AddChildContent("Content"));
        });
        cut.Markup.ShouldNotContain("Content");
    }

    [Fact(DisplayName = "UAC05: AlertDialogContent has alertdialog role")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_ContentHasAlertDialogRole()
    {
        var cut = Ctx.Render<AlertDialogSut>(p =>
        {
            p.Add(x => x.IsOpen, true);
            p.AddChildContent<AlertDialogContent>();
        });
        cut.Find("[role='alertdialog']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC06: AlertDialogHeader renders data-slot attribute")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_HeaderRendersDataSlot()
    {
        var cut = Ctx.Render<AlertDialogHeader>(p => p.AddChildContent("Header"));
        cut.Find("[data-slot='alert-dialog-header']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC07: AlertDialogFooter renders data-slot attribute")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_FooterRendersDataSlot()
    {
        var cut = Ctx.Render<AlertDialogFooter>(p => p.AddChildContent("Footer"));
        cut.Find("[data-slot='alert-dialog-footer']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC08: AlertDialogTitle renders data-slot attribute")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_TitleRendersDataSlot()
    {
        var cut = Ctx.Render<AlertDialogTitle>(p => p.AddChildContent("Title"));
        cut.Find("[data-slot='alert-dialog-title']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC09: AlertDialogTitle renders as h2")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_TitleRendersAsH2()
    {
        var cut = Ctx.Render<AlertDialogTitle>(p => p.AddChildContent("Title"));
        cut.Find("h2").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC10: AlertDialogDescription renders data-slot attribute")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_DescriptionRendersDataSlot()
    {
        var cut = Ctx.Render<AlertDialogDescription>(p => p.AddChildContent("Description"));
        cut.Find("[data-slot='alert-dialog-description']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC11: AlertDialogDescription renders as p")]
    [Trait(Spec.UAC, "11")]
    public void UAC11_DescriptionRendersAsP()
    {
        var cut = Ctx.Render<AlertDialogDescription>(p => p.AddChildContent("Description"));
        cut.Find("p").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC12: AlertDialogAction renders data-slot attribute")]
    [Trait(Spec.UAC, "12")]
    public void UAC12_ActionRendersDataSlot()
    {
        var cut = Ctx.Render<AlertDialogAction>(p => p.AddChildContent("Action"));
        cut.Find("[data-slot='alert-dialog-action']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC13: AlertDialogCancel renders data-slot attribute")]
    [Trait(Spec.UAC, "13")]
    public void UAC13_CancelRendersDataSlot()
    {
        var cut = Ctx.Render<AlertDialogCancel>(p => p.AddChildContent("Cancel"));
        cut.Find("[data-slot='alert-dialog-cancel']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC14: AlertDialogCancel renders as button")]
    [Trait(Spec.UAC, "14")]
    public void UAC14_CancelRendersAsButton()
    {
        var cut = Ctx.Render<AlertDialogCancel>(p => p.AddChildContent("Cancel"));
        cut.Find("button").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC15: AlertDialogContent renders overlay when IsOpen true")]
    [Trait(Spec.UAC, "15")]
    public void UAC15_ContentRendersOverlayWhenOpen()
    {
        var cut = Ctx.Render<AlertDialogSut>(p =>
        {
            p.Add(x => x.IsOpen, true);
            p.AddChildContent<AlertDialogContent>();
        });
        cut.Find("[data-slot='alert-dialog-overlay']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC16: AlertDialogTrigger opens dialog when clicked")]
    [Trait(Spec.UAC, "16")]
    public void UAC16_TriggerOpensDialogWhenClicked()
    {
        var cut = Ctx.Render<AlertDialogSut>(p =>
        {
            p.AddChildContent<AlertDialogTrigger>(tp => tp.AddChildContent("Open"));
        });
        cut.Find("button").Click();
        cut.Instance.IsOpen.ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC17: AlertDialogCancel closes dialog when clicked")]
    [Trait(Spec.UAC, "17")]
    public void UAC17_CancelClosesDialogWhenClicked()
    {
        var cut = Ctx.Render<AlertDialogSut>(p =>
        {
            p.Add(x => x.IsOpen, true);
            p.AddChildContent<AlertDialogContent>(cp =>
                cp.AddChildContent<AlertDialogCancel>(cp => cp.AddChildContent("Cancel")));
        });
        cut.Find("[data-slot='alert-dialog-cancel']").Click();
        cut.Instance.IsOpen.ShouldBeFalse();
    }
}
