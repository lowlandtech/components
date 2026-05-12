using Bunit;
using LowlandTech.Components.Alert;

namespace LowlandTech.Components.Tests.SPEC1001.Alert.US01.View;

[Trait(Spec.SPEC, "1001")]
[Trait(Spec.SC, "02")]
[UserStory("01", "Render Alert subcomponents")]
public class SC02_RenderAlertSubcomponents : ViewScenario
{
    [Fact(DisplayName = "UAC01: AlertTitle renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_TitleRendersDataSlot()
    {
        var cut = Ctx.Render<AlertTitle>(p => p.AddChildContent("Title"));
        cut.Find("[data-slot='alert-title']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: AlertTitle renders as h5 element")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_TitleRendersAsH5()
    {
        var cut = Ctx.Render<AlertTitle>(p => p.AddChildContent("Title"));
        cut.Find("h5").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC03: AlertTitle renders ChildContent")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_TitleRendersChildContent()
    {
        var cut = Ctx.Render<AlertTitle>(p => p.AddChildContent("Alert Title"));
        cut.Markup.ShouldContain("Alert Title");
    }

    [Fact(DisplayName = "UAC04: AlertDescription renders data-slot attribute")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_DescriptionRendersDataSlot()
    {
        var cut = Ctx.Render<AlertDescription>(p => p.AddChildContent("Description"));
        cut.Find("[data-slot='alert-description']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC05: AlertDescription renders ChildContent")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_DescriptionRendersChildContent()
    {
        var cut = Ctx.Render<AlertDescription>(p => p.AddChildContent("Alert description text"));
        cut.Markup.ShouldContain("Alert description text");
    }

    [Fact(DisplayName = "UAC06: AlertAction renders data-slot attribute")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_ActionRendersDataSlot()
    {
        var cut = Ctx.Render<AlertAction>(p => p.AddChildContent("Action"));
        cut.Find("[data-slot='alert-action']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC07: Complete Alert composition works")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_CompleteCompositionWorks()
    {
        var cut = Ctx.Render<LowlandTech.Components.Alert.Alert>(p =>
        {
            p.AddChildContent<AlertTitle>(title => title.AddChildContent("Alert Title"));
            p.AddChildContent<AlertDescription>(desc => desc.AddChildContent("Alert description"));
        });

        cut.Markup.ShouldContain("Alert Title");
        cut.Markup.ShouldContain("Alert description");
        cut.Find("[data-slot='alert-title']").ShouldNotBeNull();
        cut.Find("[data-slot='alert-description']").ShouldNotBeNull();
    }
}
