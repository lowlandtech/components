using Blocks = LowlandTech.Components.Blocks;

namespace LowlandTech.Components.Tests.SPEC2200.UIBlocks.US10.ArticleCardGrid;

[Trait(Spec.SPEC, "2200")]
[Trait(Spec.US, "10")]
[Trait(Spec.SC, "01")]
[UserStory("10", "Render ArticleCardGrid block")]
public class SC01_RenderArticleCardGrid : ViewScenario
{
    [Fact(DisplayName = "UAC01: renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<Blocks.ArticleCardGrid>(p => p.Add(x => x.Articles, []));
        cut.Find("[data-slot='article-card-grid']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: renders title when provided")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_RendersTitle()
    {
        var cut = Ctx.Render<Blocks.ArticleCardGrid>(p =>
        {
            p.Add(x => x.Title, "Latest");
            p.Add(x => x.Articles, []);
        });
        cut.Markup.ShouldContain("Latest");
    }

    [Fact(DisplayName = "UAC03: renders article items")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_RendersArticleItems()
    {
        var articles = new[]
        {
            new Blocks.ArticleCardGrid.ArticleItem("Post A", "Desc A"),
            new Blocks.ArticleCardGrid.ArticleItem("Post B", "Desc B"),
        };
        var cut = Ctx.Render<Blocks.ArticleCardGrid>(p => p.Add(x => x.Articles, articles));
        var items = cut.FindAll("[data-testid='article-card-grid-item']");
        items.Count.ShouldBe(2);
        cut.Markup.ShouldContain("Post A");
        cut.Markup.ShouldContain("Post B");
    }

    [Fact(DisplayName = "UAC04: does not inject feature-state services")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_NoFeatureStateDependencies()
    {
        var cut = Ctx.Render<Blocks.ArticleCardGrid>(p => p.Add(x => x.Articles, []));
        var instance = cut.Instance;
        instance.ShouldNotBeNull();
        instance.GetType().GetProperties()
            .Any(pi => pi.PropertyType.Name.Contains("State") || pi.PropertyType.Name.Contains("Store"))
            .ShouldBeFalse("ArticleCardGrid must not expose stateful properties");
    }
}
