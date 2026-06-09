namespace LowlandTech.Components.Tests.SPEC1078.Session.US01.Blocks;

[Trait(Spec.SPEC, "1078")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "10")]
[UserStory("01", "Context tree supports lazy loading of child nodes")]
public sealed class SC10_TreeLazyLoading : ViewScenario
{
    [Fact(DisplayName = "UAC01: Loaded root node renders its children")]
    [Trait(Spec.UAC, "01")]
    [Given("a SessionContextExplorer with a loaded root containing a header child")]
    [When("the tree renders")]
    [Then("the header child is visible")]
    public void UAC01_LoadedRootRendersChildren()
    {
        var root = new SessionContextNode(
            new SessionNodeInput("user", "user-context", "User"),
            new[] { new SessionContextNode(new SessionNodeInput("h1", "header", "Workspaces"), IsLoaded: false) },
            true);

        var cut = Ctx.Render<SessionContextExplorer>(p => p
            .Add(x => x.RootNodes, new[] { root }));

        cut.FindAll("[data-node-id='h1']").Count.ShouldBeGreaterThan(0);
    }

    [Fact(DisplayName = "UAC02: Unloaded header does not render its children")]
    [Trait(Spec.UAC, "02")]
    [Given("a SessionContextExplorer with a loaded root and an unloaded header that has grandchildren")]
    [When("the tree renders")]
    [Then("the grandchildren are not visible")]
    public void UAC02_UnloadedHeaderDoesNotRenderChildren()
    {
        var grandchild = new SessionContextNode(new SessionNodeInput("g1", "workspace", "Accounting"), IsLoaded: false);
        var header = new SessionContextNode(
            new SessionNodeInput("h1", "header", "Workspaces"),
            new[] { grandchild },
            false);
        var root = new SessionContextNode(
            new SessionNodeInput("user", "user-context", "User"),
            new[] { header },
            true);

        var cut = Ctx.Render<SessionContextExplorer>(p => p
            .Add(x => x.RootNodes, new[] { root }));

        cut.FindAll("[data-node-id='g1']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC03: Expanding an unloaded node fires OnExpand")]
    [Trait(Spec.UAC, "03")]
    [Given("a SessionContextExplorer with an unloaded header")]
    [When("the chevron is clicked")]
    [Then("OnExpand fires with the header node")]
    public void UAC03_ExpandingUnloadedNodeFiresOnExpand()
    {
        SessionNodeInput? expanded = null;
        var header = new SessionContextNode(new SessionNodeInput("h1", "header", "Workspaces"), IsLoaded: false);
        var root = new SessionContextNode(
            new SessionNodeInput("user", "user-context", "User"),
            new[] { header },
            true);

        var cut = Ctx.Render<SessionContextExplorer>(p => p
            .Add(x => x.RootNodes, new[] { root })
            .Add(x => x.OnLoadChildren, node => { expanded = node; return Task.CompletedTask; }));

        // Click the chevron (the span wrapping the icon inside the node row)
        cut.Find("[data-node-id='h1']")
           .ParentElement?.QuerySelector("span.cursor-pointer")
           ?.Click();

        expanded.ShouldNotBeNull();
        expanded.Id.ShouldBe("h1");
    }

    [Fact(DisplayName = "UAC04: Unloaded node shows chevron even with no children")]
    [Trait(Spec.UAC, "04")]
    [Given("a SessionContextTreeNode with IsLoaded=false and no children")]
    [When("the node renders")]
    [Then("the chevron is visible")]
    public void UAC04_UnloadedNodeShowsChevron()
    {
        var node = new SessionContextNode(new SessionNodeInput("h1", "header", "Workspaces"), IsLoaded: false);

        var cut = Ctx.Render<SessionContextTreeNode>(p => p
            .Add(x => x.Item, node));

        var chevron = cut.Find("span.cursor-pointer");
        chevron.ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC05: Loaded node with children shows chevron")]
    [Trait(Spec.UAC, "05")]
    [Given("a SessionContextTreeNode with IsLoaded=true and children")]
    [When("the node renders")]
    [Then("the chevron is visible")]
    public void UAC05_LoadedNodeWithChildrenShowsChevron()
    {
        var node = new SessionContextNode(
            new SessionNodeInput("h1", "header", "Workspaces"),
            new[] { new SessionContextNode(new SessionNodeInput("c1", "workspace", "Accounting"), IsLoaded: false) },
            true);

        var cut = Ctx.Render<SessionContextTreeNode>(p => p
            .Add(x => x.Item, node));

        var chevron = cut.Find("span.cursor-pointer");
        chevron.ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC06: Expanding a loaded node does not fire OnExpand")]
    [Trait(Spec.UAC, "06")]
    [Given("a SessionContextExplorer with a loaded header that has children")]
    [When("the chevron is clicked")]
    [Then("OnExpand does not fire")]
    public void UAC06_ExpandingLoadedNodeDoesNotFireOnExpand()
    {
        var fired = false;
        var header = new SessionContextNode(
            new SessionNodeInput("h1", "header", "Workspaces"),
            new[] { new SessionContextNode(new SessionNodeInput("c1", "workspace", "Accounting"), IsLoaded: false) },
            true);
        var root = new SessionContextNode(
            new SessionNodeInput("user", "user-context", "User"),
            new[] { header },
            true);

        var cut = Ctx.Render<SessionContextExplorer>(p => p
            .Add(x => x.RootNodes, new[] { root })
            .Add(x => x.OnLoadChildren, _ => { fired = true; return Task.CompletedTask; }));

        cut.Find("[data-node-id='h1']")
           .ParentElement?.QuerySelector("span.cursor-pointer")
           ?.Click();

        fired.ShouldBeFalse();
    }
}
