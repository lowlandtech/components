using Blocks = LowlandTech.Components.Blocks;

namespace LowlandTech.Components.Tests.SPEC2200.UIBlocks.US04.HeroSplit;

[Trait(Spec.SPEC, "2200")]
[Trait(Spec.US, "04")]
[Trait(Spec.SC, "01")]
[UserStory("04", "Render HeroSplit block")]
public class SC01_RenderHeroSplit : ViewScenario
{
    [Fact(DisplayName = "UAC01: renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<Blocks.HeroSplit>(p => p.Add(x => x.Title, "Hello World"));
        cut.Find("[data-slot='hero-split']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: renders title")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_RendersTitle()
    {
        var cut = Ctx.Render<Blocks.HeroSplit>(p => p.Add(x => x.Title, "Split Hero"));
        cut.Markup.ShouldContain("Split Hero");
    }

    [Fact(DisplayName = "UAC03: renders overline when provided")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_RendersOverline()
    {
        var cut = Ctx.Render<Blocks.HeroSplit>(p =>
        {
            p.Add(x => x.Title, "Split Hero");
            p.Add(x => x.Overline, "New Release");
        });
        cut.Markup.ShouldContain("New Release");
    }

    [Fact(DisplayName = "UAC04: renders subtitle when provided")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_RendersSubtitle()
    {
        var cut = Ctx.Render<Blocks.HeroSplit>(p =>
        {
            p.Add(x => x.Title, "Split Hero");
            p.Add(x => x.Subtitle, "A brief description");
        });
        cut.Markup.ShouldContain("A brief description");
    }

    [Fact(DisplayName = "UAC05: renders child content")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_RendersChildContent()
    {
        var cut = Ctx.Render<Blocks.HeroSplit>(p =>
        {
            p.Add(x => x.Title, "Split Hero");
            p.AddChildContent("<button>Action</button>");
        });
        cut.Markup.ShouldContain("Action");
    }

    [Fact(DisplayName = "UAC06: does not inject feature-state services")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_NoFeatureStateDependencies()
    {
        var cut = Ctx.Render<Blocks.HeroSplit>(p => p.Add(x => x.Title, "Split Hero"));
        var instance = cut.Instance;
        instance.ShouldNotBeNull();
        instance.GetType().GetProperties()
            .Any(pi => pi.PropertyType.Name.Contains("State") || pi.PropertyType.Name.Contains("Store"))
            .ShouldBeFalse("HeroSplit must not expose stateful properties");
    }
}
