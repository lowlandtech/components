using Blocks = LowlandTech.Components.Blocks;

namespace LowlandTech.Components.Tests.SPEC2200.UIBlocks.US06.StatsBand;

[Trait(Spec.SPEC, "2200")]
[Trait(Spec.US, "06")]
[Trait(Spec.SC, "01")]
[UserStory("06", "Render StatsBand block")]
public class SC01_RenderStatsBand : ViewScenario
{
    [Fact(DisplayName = "UAC01: renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<Blocks.StatsBand>(p => p.Add(x => x.Stats, []));
        cut.Find("[data-slot='stats-band']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: renders title when provided")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_RendersTitle()
    {
        var cut = Ctx.Render<Blocks.StatsBand>(p =>
        {
            p.Add(x => x.Title, "Our numbers");
            p.Add(x => x.Stats, []);
        });
        cut.Markup.ShouldContain("Our numbers");
    }

    [Fact(DisplayName = "UAC03: renders stat items")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_RendersStatItems()
    {
        var stats = new[]
        {
            new Blocks.StatsBand.StatItem("10k", "Users"),
            new Blocks.StatsBand.StatItem("99%", "Uptime"),
        };
        var cut = Ctx.Render<Blocks.StatsBand>(p => p.Add(x => x.Stats, stats));
        var items = cut.FindAll("[data-testid='stats-band-item']");
        items.Count.ShouldBe(2);
        cut.Markup.ShouldContain("10k");
        cut.Markup.ShouldContain("Users");
    }

    [Fact(DisplayName = "UAC04: does not inject feature-state services")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_NoFeatureStateDependencies()
    {
        var cut = Ctx.Render<Blocks.StatsBand>(p => p.Add(x => x.Stats, []));
        var instance = cut.Instance;
        instance.ShouldNotBeNull();
        instance.GetType().GetProperties()
            .Any(pi => pi.PropertyType.Name.Contains("State") || pi.PropertyType.Name.Contains("Store"))
            .ShouldBeFalse("StatsBand must not expose stateful properties");
    }
}
