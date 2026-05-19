using Blocks = LowlandTech.Components.Blocks;

namespace LowlandTech.Components.Tests.SPEC2200.UIBlocks.US11.PricingTable;

[Trait(Spec.SPEC, "2200")]
[Trait(Spec.US, "11")]
[Trait(Spec.SC, "01")]
[UserStory("11", "Render PricingTable block")]
public class SC01_RenderPricingTable : ViewScenario
{
    [Fact(DisplayName = "UAC01: renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<Blocks.PricingTable>(p => p.Add(x => x.Tiers, []));
        cut.Find("[data-slot='pricing-table']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: renders title and subtitle")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_RendersTitleAndSubtitle()
    {
        var cut = Ctx.Render<Blocks.PricingTable>(p =>
        {
            p.Add(x => x.Title, "Pricing");
            p.Add(x => x.Subtitle, "Choose wisely");
            p.Add(x => x.Tiers, []);
        });
        cut.Markup.ShouldContain("Pricing");
        cut.Markup.ShouldContain("Choose wisely");
    }

    [Fact(DisplayName = "UAC03: renders pricing tiers")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_RendersTiers()
    {
        var tiers = new[]
        {
            new Blocks.PricingTable.PricingTier("Free", "$0", "mo", ["Feature A"]),
            new Blocks.PricingTable.PricingTier("Pro", "$10", "mo", ["Feature A", "Feature B"], IsPopular: true),
        };
        var cut = Ctx.Render<Blocks.PricingTable>(p => p.Add(x => x.Tiers, tiers));
        var items = cut.FindAll("[data-testid='pricing-table-tier']");
        items.Count.ShouldBe(2);
        cut.Markup.ShouldContain("Free");
        cut.Markup.ShouldContain("Pro");
        cut.Markup.ShouldContain("Popular");
    }

    [Fact(DisplayName = "UAC04: does not inject feature-state services")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_NoFeatureStateDependencies()
    {
        var cut = Ctx.Render<Blocks.PricingTable>(p => p.Add(x => x.Tiers, []));
        var instance = cut.Instance;
        instance.ShouldNotBeNull();
        instance.GetType().GetProperties()
            .Any(pi => pi.PropertyType.Name.Contains("State") || pi.PropertyType.Name.Contains("Store"))
            .ShouldBeFalse("PricingTable must not expose stateful properties");
    }
}
