namespace LowlandTech.Components.Tests.SPEC1078.Session.US01.Blocks;

[Trait(Spec.SPEC, "1078")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "16")]
[UserStory("01", "Context header creates Lists header and list child")]
public sealed class SC16_ContextListCreate : ViewScenario
{
    [Fact(DisplayName = "UAC01: Clicking Context plus sets command bar to create mode for lists")]
    [Trait(Spec.UAC, "01")]
    [Given("a SessionBlock with a context tree")]
    [When("the user clicks the Context plus button")]
    [Then("the command bar opens in create mode with the lists context")]
    public async Task UAC01_ContextPlusSetsCreateMode()
    {
        var (cut, stateService) = RenderWithContext();

        await cut.Find("[data-testid='session-context-create']").ClickAsync(new MouseEventArgs());
        cut.Find("[data-testid='session-block-command-bar-toggle']").Click();

        cut.WaitForAssertion(() =>
        {
            stateService.State.Ui.ActiveAgentTypeKey.ShouldBe("create");
            stateService.State.Ui.ContextProjection.ShouldNotBeNull();
            stateService.State.Ui.ContextProjection!.Node.NodeTypeKey.ShouldBe("lists");
        });
    }

    [Fact(DisplayName = "UAC02: Submitting a list title creates a Lists header and a list child locally")]
    [Trait(Spec.UAC, "02")]
    [Given("a SessionBlock in list create mode")]
    [When("the user types a list title and submits")]
    [Then("a Lists header appears under the user-context with the new list child")]
    public async Task UAC02_SubmitCreatesListsHeaderAndListChild()
    {
        var (cut, stateService) = RenderWithContext();
        var listsContext = new SessionNodeInput(
            "lists-context:user-context",
            "lists",
            "Lists",
            "NodeBlock",
            new Dictionary<string, object?> { ["userContextId"] = "user-context" });

        stateService.ProjectContext(listsContext);
        stateService.SetAgentType("create");
        cut.Find("[data-testid='session-block-command-bar-toggle']").Click();

        var commandBar = cut.FindComponent<SessionCommandBar>();
        await commandBar.InvokeAsync(() => commandBar.Instance.OnSubmit.InvokeAsync(
            new SessionCommandSubmitted(listsContext, "create", "My List")));

        var nodes = cut.FindAll("[data-slot='session-context-node']");
        var listsHeader = nodes.SingleOrDefault(n => n.TextContent.Trim() == "Lists");
        listsHeader.ShouldNotBeNull();
        listsHeader!.QuerySelector("span")?.Click();

        nodes = cut.FindAll("[data-slot='session-context-node']");
        nodes.SingleOrDefault(n => n.TextContent.Trim() == "My List").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC03: Subsequent list submissions reuse the existing Lists header")]
    [Trait(Spec.UAC, "03")]
    [Given("a SessionBlock with an existing Lists header")]
    [When("the user submits a second list title")]
    [Then("only one Lists header exists and both lists appear under it")]
    public async Task UAC03_SubsequentSubmissionsReuseListsHeader()
    {
        var (cut, stateService) = RenderWithContext();
        var listsContext = new SessionNodeInput(
            "lists-context:user-context",
            "lists",
            "Lists",
            "NodeBlock",
            new Dictionary<string, object?> { ["userContextId"] = "user-context" });

        stateService.ProjectContext(listsContext);
        stateService.SetAgentType("create");
        cut.Find("[data-testid='session-block-command-bar-toggle']").Click();

        var commandBar = cut.FindComponent<SessionCommandBar>();
        await commandBar.InvokeAsync(() => commandBar.Instance.OnSubmit.InvokeAsync(
            new SessionCommandSubmitted(listsContext, "create", "First List")));

        stateService.ProjectContext(listsContext);
        stateService.SetAgentType("create");

        await commandBar.InvokeAsync(() => commandBar.Instance.OnSubmit.InvokeAsync(
            new SessionCommandSubmitted(listsContext, "create", "Second List")));

        var nodes = cut.FindAll("[data-slot='session-context-node']");
        nodes.Count(n => n.TextContent.Trim() == "Lists").ShouldBe(1);
        var listsHeader = nodes.Single(n => n.TextContent.Trim() == "Lists");
        listsHeader.QuerySelector("span")?.Click();

        nodes = cut.FindAll("[data-slot='session-context-node']");
        nodes.SingleOrDefault(n => n.TextContent.Trim() == "First List").ShouldNotBeNull();
        nodes.SingleOrDefault(n => n.TextContent.Trim() == "Second List").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC04: Standalone mode creates Lists as a root node")]
    [Trait(Spec.UAC, "04")]
    [Given("a SessionBlock without a user-context root")]
    [When("the user submits a list title")]
    [Then("a Lists root node appears with the new list child")]
    public async Task UAC04_StandaloneModeCreatesListsRoot()
    {
        var (cut, stateService) = RenderStandalone();
        var listsContext = new SessionNodeInput(
            "lists-context:workspaces",
            "lists",
            "Lists",
            "NodeBlock",
            new Dictionary<string, object?> { ["userContextId"] = "workspaces" });

        stateService.ProjectContext(listsContext);
        stateService.SetAgentType("create");
        cut.Find("[data-testid='session-block-command-bar-toggle']").Click();

        var commandBar = cut.FindComponent<SessionCommandBar>();
        await commandBar.InvokeAsync(() => commandBar.Instance.OnSubmit.InvokeAsync(
            new SessionCommandSubmitted(listsContext, "create", "Standalone List")));

        var nodes = cut.FindAll("[data-slot='session-context-node']");
        var listsHeader = nodes.SingleOrDefault(n => n.TextContent.Trim() == "Lists");
        listsHeader.ShouldNotBeNull();
        listsHeader!.GetAttribute("data-depth").ShouldBe("0");

        // The standalone Lists root is expanded by default (depth 0 + loaded),
        // so its child is already visible without toggling.
        var treeItem = listsHeader.ParentElement?.ParentElement;
        nodes = treeItem?.QuerySelectorAll("[data-slot='session-context-node']")
            ?? cut.FindAll("[data-slot='session-context-node']");
        nodes.SingleOrDefault(n => n.TextContent.Trim() == "Standalone List").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC05: Context plus in app mode stores the user-context id")]
    [Trait(Spec.UAC, "05")]
    [Given("a SessionBlock with a user-context root")]
    [When("the user clicks the Context plus button")]
    [Then("the lists projection stores the user-context node id")]
    public async Task UAC05_AppPlusButtonStoresUserContextId()
    {
        var (cut, stateService) = RenderWithContext();

        await cut.Find("[data-testid='session-context-create']").ClickAsync(new MouseEventArgs());

        cut.WaitForAssertion(() =>
        {
            stateService.State.Ui.ContextProjection.ShouldNotBeNull();
            stateService.State.Ui.ContextProjection!.Node.Properties
                ?.GetValueOrDefault("userContextId").ShouldBe("user-context");
        });
    }

    [Fact(DisplayName = "UAC06: Context plus in standalone mode does not store a user-context id")]
    [Trait(Spec.UAC, "06")]
    [Given("a SessionBlock without a user-context root")]
    [When("the user clicks the Context plus button")]
    [Then("the lists projection has no user-context id")]
    public async Task UAC06_StandalonePlusButtonHasNoUserContextId()
    {
        var (cut, stateService) = RenderStandalone();

        await cut.Find("[data-testid='session-context-create']").ClickAsync(new MouseEventArgs());

        cut.WaitForAssertion(() =>
        {
            stateService.State.Ui.ContextProjection.ShouldNotBeNull();
            stateService.State.Ui.ContextProjection!.Node.Properties
                ?.GetValueOrDefault("userContextId").ShouldBeNull();
        });
    }

    [Fact(DisplayName = "UAC07: Standalone plus button creates Lists as a root end-to-end")]
    [Trait(Spec.UAC, "07")]
    [Given("a SessionBlock without a user-context root")]
    [When("the user clicks the Context plus button and submits a list title")]
    [Then("a Lists root node appears with the new list child")]
    public async Task UAC07_StandalonePlusButtonCreatesRootEndToEnd()
    {
        var (cut, stateService) = RenderStandalone();

        await cut.Find("[data-testid='session-context-create']").ClickAsync(new MouseEventArgs());

        cut.WaitForState(() => cut.FindComponents<SessionCommandBar>().Count > 0);
        var commandBar = cut.FindComponent<SessionCommandBar>();
        await commandBar.InvokeAsync(() => commandBar.Instance.OnSubmit.InvokeAsync(
            new SessionCommandSubmitted(stateService.State.Ui.ContextProjection!.Node, "create", "E2E Standalone List")));

        var nodes = cut.FindAll("[data-slot='session-context-node']");
        var listsHeader = nodes.SingleOrDefault(n => n.TextContent.Trim() == "Lists");
        listsHeader.ShouldNotBeNull();
        listsHeader!.GetAttribute("data-depth").ShouldBe("0");

        var treeItem = listsHeader.ParentElement?.ParentElement;
        nodes = treeItem?.QuerySelectorAll("[data-slot='session-context-node']")
            ?? cut.FindAll("[data-slot='session-context-node']");
        nodes.SingleOrDefault(n => n.TextContent.Trim() == "E2E Standalone List").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC08: Graph service creates the list under the user-context")]
    [Trait(Spec.UAC, "08")]
    [Given("a SessionBlock backed by a graph service with a user-context")]
    [When("the user clicks the Context plus button and submits a list title")]
    [Then("the graph contains a Lists header under the user-context and the new list under it")]
    public async Task UAC08_GraphServiceCreatesListUnderUserContext()
    {
        var userContextId = Guid.NewGuid();
        var graphService = new FakeGraphService();
        graphService.SeedNode(userContextId, "user-context", "My Context");

        var stateService = new SessionStateService(null, graphService);
        var sessions = new[] { new SessionListEntry("s1", "Workspaces", 0, "session") };
        stateService.SelectSession(new SessionNodeInput("s1", "session", "Workspaces"), sessions);
        await stateService.LoadContextNodesAsync(userContextId, "My Context", Xunit.TestContext.Current.CancellationToken);

        var cut = Ctx.Render<Components.SessionBlock>(p => p
            .Add(x => x.HasSidebar, true)
            .Add(x => x.StateService, stateService)
            .Add(x => x.Sessions, sessions)
            .Add(x => x.ContextNodes, stateService.ContextNodes));

        await cut.Find("[data-testid='session-context-create']").ClickAsync(new MouseEventArgs());

        cut.WaitForState(() => cut.FindComponents<SessionCommandBar>().Count > 0);
        var commandBar = cut.FindComponent<SessionCommandBar>();
        await commandBar.InvokeAsync(() => commandBar.Instance.OnSubmit.InvokeAsync(
            new SessionCommandSubmitted(stateService.State.Ui.ContextProjection!.Node, "create", "Graph List")));

        var listsHeader = graphService.Nodes.FirstOrDefault(n => n.NodeType?.Key == "lists");
        listsHeader.ShouldNotBeNull();
        graphService.Edges.Any(e => e.SourceNodeId == userContextId && e.TargetNodeId == listsHeader!.Id).ShouldBeTrue();

        var listNode = graphService.Nodes.FirstOrDefault(n => n.Label == "Graph List");
        listNode.ShouldNotBeNull();
        graphService.Edges.Any(e => e.SourceNodeId == listsHeader!.Id && e.TargetNodeId == listNode!.Id).ShouldBeTrue();
    }

    private (IRenderedComponent<Components.SessionBlock> Cut, SessionStateService StateService) RenderStandalone()
    {
        var stateService = new SessionStateService();
        var sessions = new[] { new SessionListEntry("s1", "Workspaces", 0, "session") };
        stateService.SelectSession(new SessionNodeInput("s1", "session", "Workspaces"), sessions);

        var contextNodes = new[]
        {
            new SessionContextNode(new SessionNodeInput("workspaces", "workspaces.header", "Workspaces"), IsLoaded: true)
        };

        var cut = Ctx.Render<Components.SessionBlock>(p => p
            .Add(x => x.HasSidebar, true)
            .Add(x => x.StateService, stateService)
            .Add(x => x.Sessions, sessions)
            .Add(x => x.ContextNodes, contextNodes));

        return (cut, stateService);
    }

    private (IRenderedComponent<Components.SessionBlock> Cut, SessionStateService StateService) RenderWithContext()
    {
        var stateService = new SessionStateService();
        var sessions = new[] { new SessionListEntry("s1", "Workspaces", 0, "session") };
        stateService.SelectSession(new SessionNodeInput("s1", "session", "Workspaces"), sessions);

        var contextNodes = new[]
        {
            new SessionContextNode(
                new SessionNodeInput("user-context", "user-context", "My Context"),
                [
                    new SessionContextNode(new SessionNodeInput("workspaces", "workspaces.header", "Workspaces"), IsLoaded: true)
                ],
                true)
        };

        var cut = Ctx.Render<Components.SessionBlock>(p => p
            .Add(x => x.HasSidebar, true)
            .Add(x => x.StateService, stateService)
            .Add(x => x.Sessions, sessions)
            .Add(x => x.ContextNodes, contextNodes));

        return (cut, stateService);
    }
}
