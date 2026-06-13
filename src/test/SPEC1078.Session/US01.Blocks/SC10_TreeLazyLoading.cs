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

    [Fact(DisplayName = "UAC07: Non-root loaded nodes start collapsed")]
    [Trait(Spec.UAC, "07")]
    [Given("a SessionContextExplorer with a loaded root and a loaded non-root header")]
    [When("the tree renders")]
    [Then("the non-root header's children are hidden until the header is expanded")]
    public void UAC07_NonRootLoadedNodesStartCollapsed()
    {
        var grandchild = new SessionContextNode(new SessionNodeInput("g1", "workspace", "Accounting"), IsLoaded: false);
        var header = new SessionContextNode(
            new SessionNodeInput("h1", "header", "Workspaces"),
            new[] { grandchild },
            true);
        var root = new SessionContextNode(
            new SessionNodeInput("user", "user-context", "User"),
            new[] { header },
            true);

        var cut = Ctx.Render<SessionContextExplorer>(p => p
            .Add(x => x.RootNodes, new[] { root }));

        cut.FindAll("[data-node-id='h1']").Count.ShouldBeGreaterThan(0);
        cut.FindAll("[data-node-id='g1']").Count.ShouldBe(0);

        cut.Find("[data-node-id='h1']")
           .ParentElement?.QuerySelector("span.cursor-pointer")
           ?.Click();

        cut.FindAll("[data-node-id='g1']").Count.ShouldBeGreaterThan(0);
    }

    [Fact(DisplayName = "UAC08: Header nodes show a folder icon that switches to open folder when expanded")]
    [Trait(Spec.UAC, "08")]
    [Given("a SessionContextExplorer with a loaded root and a loaded header")]
    [When("the header is expanded and collapsed")]
    [Then("the icon changes between folder and open-folder")]
    public void UAC08_HeaderFolderIconTogglesWithExpansion()
    {
        var header = new SessionContextNode(
            new SessionNodeInput("h1", "header", "Workspaces"),
            new[] { new SessionContextNode(new SessionNodeInput("c1", "workspace", "Accounting"), IsLoaded: false) },
            true);
        var root = new SessionContextNode(
            new SessionNodeInput("user", "user-context", "User"),
            new[] { header },
            true);

        var cut = Ctx.Render<SessionContextExplorer>(p => p
            .Add(x => x.RootNodes, new[] { root }));

        cut.Markup.ShouldContain(Icons.Folder);
        cut.Markup.ShouldNotContain(Icons.FolderOpen);

        cut.Find("[data-node-id='h1']")
           .ParentElement?.QuerySelector("span.cursor-pointer")
           ?.Click();

        cut.Markup.ShouldContain(Icons.FolderOpen);
        cut.Markup.ShouldNotContain(Icons.Folder);
    }

    [Fact(DisplayName = "UAC09: Clicking a pinned tree node invokes OnNavigateToPin instead of OnProject")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_PinnedNodeClickNavigatesToPin()
    {
        var node = new SessionContextNode(new SessionNodeInput("n1", "invoice", "Invoice"), IsLoaded: true);
        var pin = new SessionContextPinModel("pin:n1", node.Node, new SessionNodeInput("pin:n1", "session.pin", "Invoice"));

        SessionNodeInput? projected = null;
        SessionNodeInput? navigated = null;

        var cut = Ctx.Render<SessionContextTreeNode>(p => p
            .Add(x => x.Item, node)
            .Add(x => x.Pins, new[] { pin })
            .Add(x => x.OnProject, n => { projected = n; return Task.CompletedTask; })
            .Add(x => x.OnNavigateToPin, n => { navigated = n; return Task.CompletedTask; }));

        cut.Find("[data-slot='session-context-node']").Click();

        projected.ShouldBeNull();
        navigated.ShouldNotBeNull();
        navigated.Id.ShouldBe("n1");
    }

    [Fact(DisplayName = "UAC10: SessionContextExplorer forwards pinned node clicks to OnNavigateToPin")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_ExplorerForwardsPinnedNodeClick()
    {
        var node = new SessionContextNode(new SessionNodeInput("n1", "invoice", "Invoice"), IsLoaded: true);
        var pin = new SessionContextPinModel("pin:n1", node.Node, new SessionNodeInput("pin:n1", "session.pin", "Invoice"));

        SessionNodeInput? navigated = null;

        var cut = Ctx.Render<SessionContextExplorer>(p => p
            .Add(x => x.RootNodes, new[] { node })
            .Add(x => x.Pins, new[] { pin })
            .Add(x => x.OnNavigateToPin, n => { navigated = n; return Task.CompletedTask; }));

        cut.Find("[data-slot='session-context-node']").Click();

        navigated.ShouldNotBeNull();
        navigated.Id.ShouldBe("n1");
    }
}
