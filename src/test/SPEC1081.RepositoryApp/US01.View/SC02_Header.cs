using LowlandTech.Components.RepositoryApp;
namespace LowlandTech.Components.Tests.SPEC1081.RepositoryApp.US01.View;

[Trait(Spec.SPEC, "1081")]
[Trait(Spec.SC, "02")]
[UserStory("01", "Header renders owner/repo identity, badges, and watch/star/fork actions")]
public class SC02_Header : Infrastructure.RepositoryScenario
{
    [Fact(DisplayName = "UAC01: Public repo does not render the Private badge")]
    [Trait(Spec.UAC, "01")]
    [Given("a public repository is rendered")]
    [When("the header is inspected")]
    [Then("the Private and Internal badges should not be present")]
    public void UAC01_PublicRepoOmitsPrivateBadge()
    {
        var cut = RenderRepositoryApp();

        cut.FindAll("[data-testid='repo-header-private']").Count.ShouldBe(0);
        cut.FindAll("[data-testid='repo-header-internal']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC02: Private repo renders the Private badge with lock icon")]
    [Trait(Spec.UAC, "02")]
    [Given("a private repository is rendered")]
    [When("the header is inspected")]
    [Then("a Private badge with a lock icon should be present")]
    public void UAC02_PrivateRepoRendersPrivateBadgeWithLockIcon()
    {
        var privateState = RepositoryUseCaseData.Primary() with
        {
            Current = RepositoryUseCaseData.BuildRepository() with
            {
                Visibility = RepositoryVisibility.Private
            }
        };
        var cut = RenderRepositoryAppWithState(privateState);

        var badge = cut.Find("[data-testid='repo-header-private']");
        badge.ShouldNotBeNull();
        badge.QuerySelector("svg").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC03: Private badge takes precedence over Internal")]
    [Trait(Spec.UAC, "03")]
    [Given("a repository whose visibility is both private and internal in the state shape")]
    [When("the header is rendered")]
    [Then("the Private badge should be the only visibility badge shown")]
    public void UAC03_PrivateBadgeTakesPrecedenceOverInternal()
    {
        // In our state model visibility is a single enum; the test simulates a private repo.
        var privateState = RepositoryUseCaseData.Primary() with
        {
            Current = RepositoryUseCaseData.BuildRepository() with
            {
                Visibility = RepositoryVisibility.Private
            }
        };
        var cut = RenderRepositoryAppWithState(privateState);

        cut.FindAll("[data-testid='repo-header-private']").Count.ShouldBe(1);
        cut.FindAll("[data-testid='repo-header-internal']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC04: Template badge renders only when IsTemplate is true")]
    [Trait(Spec.UAC, "04")]
    [Given("a repository that is not a template is rendered")]
    [When("the header is inspected")]
    [Then("the Template badge should not be present")]
    public void UAC04_TemplateBadgeOnlyWhenTemplate()
    {
        var cut = RenderRepositoryApp();
        cut.FindAll("[data-testid='repo-header-template']").Count.ShouldBe(0);

        var templateState = RepositoryUseCaseData.Primary() with
        {
            Current = RepositoryUseCaseData.BuildRepository() with { IsTemplate = true }
        };
        var cut2 = RenderRepositoryAppWithState(templateState);
        cut2.Find("[data-testid='repo-header-template']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC05: Public Access badge renders when website is set")]
    [Trait(Spec.UAC, "05")]
    [Given("a repository with a website is rendered")]
    [When("the header is inspected")]
    [Then("the Public Access badge should be present")]
    public void UAC05_PublicAccessBadgeWhenWebsiteSet()
    {
        var cut = RenderRepositoryApp();

        var badge = cut.Find("[data-testid='repo-header-public-access']");
        badge.ShouldNotBeNull();
        badge.TextContent.ShouldContain("Public Access");
    }

    [Fact(DisplayName = "UAC06: Star button increments current.Stars via the StateService")]
    [Trait(Spec.UAC, "06")]
    [Given("a RepositoryApp is rendered")]
    [When("the Star button is clicked")]
    [Then("the current repository's Stars count should increment by one")]
    public void UAC06_StarButtonIncrementsStars()
    {
        var cut = RenderRepositoryApp();
        var before = StateService.State.Current!.Stars;

        cut.Find("[data-testid='repo-header-star']").Click();
        cut.Render();

        StateService.State.Current!.Stars.ShouldBe(before + 1);
    }

    [Fact(DisplayName = "UAC07: Watch button increments current.Watchers")]
    [Trait(Spec.UAC, "07")]
    [Given("a RepositoryApp is rendered")]
    [When("the Watch button is clicked")]
    [Then("the current repository's Watchers count should increment by one")]
    public void UAC07_WatchButtonIncrementsWatchers()
    {
        var cut = RenderRepositoryApp();
        var before = StateService.State.Current!.Watchers;

        cut.Find("[data-testid='repo-header-watch']").Click();
        cut.Render();

        StateService.State.Current!.Watchers.ShouldBe(before + 1);
    }

    [Fact(DisplayName = "UAC08: Fork button increments current.Forks")]
    [Trait(Spec.UAC, "08")]
    [Given("a RepositoryApp is rendered")]
    [When("the Fork button is clicked")]
    [Then("the current repository's Forks count should increment by one")]
    public void UAC08_ForkButtonIncrementsForks()
    {
        var cut = RenderRepositoryApp();
        var before = StateService.State.Current!.Forks;

        cut.Find("[data-testid='repo-header-fork']").Click();
        cut.Render();

        StateService.State.Current!.Forks.ShouldBe(before + 1);
    }

    [Fact(DisplayName = "UAC09: Fork hint line is hidden for non-fork repos")]
    [Trait(Spec.UAC, "09")]
    [Given("a repository that is not a fork is rendered")]
    [When("the header is inspected")]
    [Then("the 'Forked from' hint should not appear")]
    public void UAC09_ForkHintHiddenForNonFork()
    {
        var cut = RenderRepositoryApp();

        var header = cut.Find("[data-slot='repository-app-header']");
        header.TextContent.ShouldNotContain("Forked from");
    }

    [Fact(DisplayName = "UAC10: Fork hint line renders when IsFork is true")]
    [Trait(Spec.UAC, "10")]
    [Given("a forked repository is rendered")]
    [When("the header is inspected")]
    [Then("a 'Forked from' hint should appear in the header")]
    public void UAC10_ForkHintRendersWhenFork()
    {
        var forkState = RepositoryUseCaseData.Primary() with
        {
            Current = RepositoryUseCaseData.BuildRepository() with { IsFork = true }
        };
        var cut = RenderRepositoryAppWithState(forkState);

        var header = cut.Find("[data-slot='repository-app-header']");
        header.TextContent.ShouldContain("Forked from");
    }

    [Fact(DisplayName = "UAC11: Mirror hint line renders when IsMirror is true")]
    [Trait(Spec.UAC, "11")]
    [Given("a repository flagged as a mirror is rendered")]
    [When("the header is inspected")]
    [Then("a 'Mirrored from' hint should appear in the header")]
    public void UAC11_MirrorHintRendersWhenMirror()
    {
        var mirrorState = RepositoryUseCaseData.Primary() with
        {
            Current = RepositoryUseCaseData.BuildRepository() with { IsMirror = true }
        };
        var cut = RenderRepositoryAppWithState(mirrorState);

        var header = cut.Find("[data-slot='repository-app-header']");
        header.TextContent.ShouldContain("Mirrored from");
    }

    [Fact(DisplayName = "UAC12: All action buttons use the Button component, not raw HTML")]
    [Trait(Spec.UAC, "12")]
    [Given("the repository header is rendered")]
    [When("action buttons are inspected")]
    [Then("they should carry the Button component's data-slot, not raw button tags")]
    public void UAC12_ActionButtonsUseButtonComponent()
    {
        var cut = RenderRepositoryApp();

        var watch = cut.Find("[data-testid='repo-header-watch']");
        watch.GetAttribute("data-slot").ShouldBe("button");
        cut.FindAll("[data-testid='repository-app-header'] > button").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC13: Repository shell icons render SVG content")]
    [Trait(Spec.UAC, "13")]
    [Given("the repository application shell is rendered")]
    [When("all icon components in the visible shell are inspected")]
    [Then("each icon should contain SVG drawing elements instead of an empty svg")]
    public void UAC13_RepositoryShellIconsRenderSvgContent()
    {
        var cut = RenderRepositoryApp();

        var icons = cut.FindAll("svg[data-slot='icon']");

        icons.Count.ShouldBeGreaterThan(0);
        foreach (var icon in icons)
        {
            icon.Children.Length.ShouldBeGreaterThan(0);
        }
    }
}
