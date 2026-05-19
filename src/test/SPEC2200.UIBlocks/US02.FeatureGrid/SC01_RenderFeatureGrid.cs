using Blocks = LowlandTech.Components.Blocks;

namespace LowlandTech.Components.Tests.SPEC2200.UIBlocks.US02.FeatureGrid;

[Trait(Spec.SPEC, "2200")]
[Trait(Spec.US, "02")]
[Trait(Spec.SC, "01")]
[UserStory("02", "Render FeatureGrid block")]
public class SC01_RenderFeatureGrid : ViewScenario
{
    [Fact(DisplayName = "UAC01: renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<Blocks.FeatureGrid>(p => p.Add(x => x.Features, []));
        cut.Find("[data-slot='feature-grid']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: renders title and subtitle")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_RendersTitleAndSubtitle()
    {
        var cut = Ctx.Render<Blocks.FeatureGrid>(p =>
        {
            p.Add(x => x.Title, "Features");
            p.Add(x => x.Subtitle, "What we offer");
            p.Add(x => x.Features, []);
        });
        cut.Markup.ShouldContain("Features");
        cut.Markup.ShouldContain("What we offer");
    }

    [Fact(DisplayName = "UAC03: renders feature items")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_RendersFeatureItems()
    {
        var features = new[]
        {
            new Blocks.FeatureGrid.FeatureItem("Fast", "Blazing speed", null),
            new Blocks.FeatureGrid.FeatureItem("Secure", "Enterprise-grade security", null),
        };
        var cut = Ctx.Render<Blocks.FeatureGrid>(p => p.Add(x => x.Features, features));
        var items = cut.FindAll("[data-testid='feature-grid-item']");
        items.Count.ShouldBe(2);
        cut.Markup.ShouldContain("Fast");
        cut.Markup.ShouldContain("Secure");
    }

    [Fact(DisplayName = "UAC04: respects column parameter")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_RespectsColumns()
    {
        var features = Enumerable.Range(0, 4)
            .Select(i => new Blocks.FeatureGrid.FeatureItem($"F{i}", "D", null))
            .ToList();
        var cut = Ctx.Render<Blocks.FeatureGrid>(p =>
        {
            p.Add(x => x.Features, features);
            p.Add(x => x.Columns, 2);
        });
        var grid = cut.Find("[data-slot='feature-grid'] > div:last-child > div:last-child");
        grid.ClassList.ShouldContain("sm:grid-cols-2");
    }

    [Fact(DisplayName = "UAC05: does not inject feature-state services")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_NoFeatureStateDependencies()
    {
        var cut = Ctx.Render<Blocks.FeatureGrid>(p => p.Add(x => x.Features, []));
        var instance = cut.Instance;
        instance.ShouldNotBeNull();
        instance.GetType().GetProperties()
            .Any(pi => pi.PropertyType.Name.Contains("State") || pi.PropertyType.Name.Contains("Store"))
            .ShouldBeFalse("FeatureGrid must not expose stateful properties");
    }
}
