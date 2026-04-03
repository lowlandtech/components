using Bunit;
using DialogSut = LowlandTech.Components.Dialog.Dialog;
using LowlandTech.Components.Dialog;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1018.Dialog.US01.View;

[Trait(Spec.SPEC, "1018")]
[Trait(Spec.SC, "02")]
[UserStory("01", "Render Dialog subcomponents")]
public class SC02_RenderDialogSubcomponents : ViewScenario
{
    [Fact(DisplayName = "UAC01: DialogTrigger renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_TriggerRendersDataSlot()
    {
        var cut = Ctx.Render<DialogTrigger>(p => p.AddChildContent("Open"));
        cut.Find("[data-slot='dialog-trigger']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: DialogTrigger renders as button")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_TriggerRendersAsButton()
    {
        var cut = Ctx.Render<DialogTrigger>(p => p.AddChildContent("Open"));
        cut.Find("button").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC03: DialogContent renders when IsOpen true")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_ContentRendersWhenOpen()
    {
        var cut = Ctx.Render<DialogSut>(p =>
        {
            p.Add(x => x.IsOpen, true);
            p.AddChildContent<DialogContent>(cp => cp.AddChildContent("Content"));
        });
        cut.Markup.ShouldContain("Content");
        cut.Find("[data-slot='dialog-content']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC04: DialogContent does not render when IsOpen false")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_ContentDoesNotRenderWhenClosed()
    {
        var cut = Ctx.Render<DialogSut>(p =>
        {
            p.Add(x => x.IsOpen, false);
            p.AddChildContent<DialogContent>(cp => cp.AddChildContent("Content"));
        });
        cut.Markup.ShouldNotContain("Content");
    }

    [Fact(DisplayName = "UAC05: DialogContent has dialog role")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_ContentHasDialogRole()
    {
        var cut = Ctx.Render<DialogSut>(p =>
        {
            p.Add(x => x.IsOpen, true);
            p.AddChildContent<DialogContent>();
        });
        cut.Find("[role='dialog']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC06: DialogHeader renders data-slot attribute")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_HeaderRendersDataSlot()
    {
        var cut = Ctx.Render<DialogHeader>(p => p.AddChildContent("Header"));
        cut.Find("[data-slot='dialog-header']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC07: DialogFooter renders data-slot attribute")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_FooterRendersDataSlot()
    {
        var cut = Ctx.Render<DialogFooter>(p => p.AddChildContent("Footer"));
        cut.Find("[data-slot='dialog-footer']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC08: DialogTitle renders data-slot attribute")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_TitleRendersDataSlot()
    {
        var cut = Ctx.Render<DialogTitle>(p => p.AddChildContent("Title"));
        cut.Find("[data-slot='dialog-title']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC09: DialogTitle renders as h2")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_TitleRendersAsH2()
    {
        var cut = Ctx.Render<DialogTitle>(p => p.AddChildContent("Title"));
        cut.Find("h2").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC10: DialogDescription renders data-slot attribute")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_DescriptionRendersDataSlot()
    {
        var cut = Ctx.Render<DialogDescription>(p => p.AddChildContent("Description"));
        cut.Find("[data-slot='dialog-description']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC11: DialogDescription renders as p")]
    [Trait(Spec.UAC, "11")]
    public void UAC11_DescriptionRendersAsP()
    {
        var cut = Ctx.Render<DialogDescription>(p => p.AddChildContent("Description"));
        cut.Find("p").ShouldNotBeNull();
    }
}
