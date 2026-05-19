using LowlandTech.Components.Blocks;

namespace LowlandTech.Components.Tests.SPEC2200.UIBlocks.US05.CTABand;

[Trait(Spec.SPEC, "2200")]
[Trait(Spec.US, "05")]
[Trait(Spec.SC, "01")]
[UserStory("05", "Render CallToActionBand block")]
public class SC01_RenderCallToActionBand : ViewScenario
{
    [Fact(DisplayName = "UAC01: renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<CallToActionBand>(p => p.Add(x => x.Title, "Act Now"));
        cut.Find("[data-slot='cta-band']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: renders title and subtitle")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_RendersTitleAndSubtitle()
    {
        var cut = Ctx.Render<CallToActionBand>(p =>
        {
            p.Add(x => x.Title, "Ready to start?");
            p.Add(x => x.Subtitle, "Join today.");
        });
        cut.Markup.ShouldContain("Ready to start?");
        cut.Markup.ShouldContain("Join today.");
    }

    [Fact(DisplayName = "UAC03: renders overline when provided")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_RendersOverline()
    {
        var cut = Ctx.Render<CallToActionBand>(p =>
        {
            p.Add(x => x.Title, "Act");
            p.Add(x => x.Overline, "Limited time");
        });
        cut.Markup.ShouldContain("Limited time");
    }

    [Fact(DisplayName = "UAC04: renders child content")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_RendersChildContent()
    {
        var cut = Ctx.Render<CallToActionBand>(p =>
        {
            p.Add(x => x.Title, "Act");
            p.AddChildContent("<button>Go</button>");
        });
        cut.Markup.ShouldContain("Go");
    }

    [Fact(DisplayName = "UAC05: does not inject feature-state services")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_NoFeatureStateDependencies()
    {
        var cut = Ctx.Render<CallToActionBand>(p => p.Add(x => x.Title, "Act"));
        var instance = cut.Instance;
        instance.ShouldNotBeNull();
        instance.GetType().GetProperties()
            .Any(pi => pi.PropertyType.Name.Contains("State") || pi.PropertyType.Name.Contains("Store"))
            .ShouldBeFalse("CallToActionBand must not expose stateful properties");
    }
}
