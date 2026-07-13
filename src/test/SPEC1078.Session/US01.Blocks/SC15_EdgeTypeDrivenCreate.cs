namespace LowlandTech.Components.Tests.SPEC1078.Session.US01.Blocks;

[Trait(Spec.SPEC, "1078")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "15")]
[UserStory("01", "Edge-type-driven node creation in Create mode")]
public sealed class SC15_EdgeTypeDrivenCreate : ViewScenario
{
    [Fact(DisplayName = "UAC01: Submitting create on a workspaces header resolves the WorkspaceCreator tool")]
    [Trait(Spec.UAC, "01")]
    [Given("a SessionBlock with a workspaces.header node selected in create mode")]
    [When("the user submits a title")]
    [Then("WorkspaceCreatorTool.ExecuteAsync is invoked with the title")]
    public async Task UAC01_CreateOnWorkspacesHeaderInvokesWorkspaceCreator()
    {
        var (cut, stateService, parentNode) = RenderWorkspacesHeaderInCreateMode();
        var tool = (WorkspaceCreatorTool)Ctx.Services.GetServices<IGraphToolService>()
            .Single(s => s is WorkspaceCreatorTool);

        object? executedEntity = null;
        string? capturedTitle = null;
        tool.OnExecuted = (entity, title) =>
        {
            executedEntity = entity;
            capturedTitle = title;
        };

        cut.Find("[data-testid='session-block-command-bar-toggle']").Click();

        var commandBar = cut.FindComponent<SessionCommandBar>();
        await commandBar.InvokeAsync(() => commandBar.Instance.OnSubmit.InvokeAsync(
            new SessionCommandSubmitted(parentNode, "create", "Accounting")));

        capturedTitle.ShouldBe("Accounting");
        executedEntity.ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Edge-driven create adds a workspace child under the workspaces header")]
    [Trait(Spec.UAC, "02")]
    [Given("a SessionBlock with a workspaces.header node selected in create mode")]
    [When("the user submits a title")]
    [Then("a new workspace node appears under the header")]
    public async Task UAC02_CreateOnWorkspacesHeaderAddsWorkspaceChild()
    {
        var (cut, stateService, parentNode) = RenderWorkspacesHeaderInCreateMode();

        cut.Find("[data-testid='session-block-command-bar-toggle']").Click();

        var commandBar = cut.FindComponent<SessionCommandBar>();
        await commandBar.InvokeAsync(() => commandBar.Instance.OnSubmit.InvokeAsync(
            new SessionCommandSubmitted(parentNode, "create", "Accounting")));

        cut.Markup.ShouldContain("title=\"workspace\"");
        cut.Markup.ShouldContain(">Accounting</span>");
    }

    [Fact(DisplayName = "UAC03: Edge-driven create resets agent type to ask after normal submit")]
    [Trait(Spec.UAC, "03")]
    [Given("a SessionBlock with a workspaces.header node selected in create mode")]
    [When("the user submits a title")]
    [Then("agent type resets to ask and input is cleared")]
    public async Task UAC03_EdgeDrivenCreateResetsAgentType()
    {
        var (cut, stateService, parentNode) = RenderWorkspacesHeaderInCreateMode();

        cut.Find("[data-testid='session-block-command-bar-toggle']").Click();

        var commandBar = cut.FindComponent<SessionCommandBar>();
        await commandBar.InvokeAsync(() => commandBar.Instance.OnSubmit.InvokeAsync(
            new SessionCommandSubmitted(parentNode, "create", "Accounting")));

        stateService.State.Ui.ActiveAgentTypeKey.ShouldBe("ask");
        stateService.State.Ui.Input.ShouldBeEmpty();
    }

    [Fact(DisplayName = "UAC04: Submitting create on a workitems header resolves the WorkitemCreator tool")]
    [Trait(Spec.UAC, "04")]
    [Given("a SessionBlock with a workitems.header node selected in create mode")]
    [When("the user submits a title")]
    [Then("WorkitemCreatorTool.ExecuteAsync is invoked and a workitem child is added")]
    public async Task UAC04_CreateOnWorkitemsHeaderInvokesWorkitemCreator()
    {
        var stateService = new SessionStateService();
        var sessions = new[] { new SessionListEntry("s1", "Workitems", 0, "session") };
        stateService.SelectSession(new SessionNodeInput("s1", "session", "Workitems"), sessions);

        var parentNode = new SessionNodeInput("wi-header", "workitems.header", "Workitems");
        var contextNodes = new[] { new SessionContextNode(parentNode, IsLoaded: true) };

        var cut = Ctx.Render<Components.SessionBlock>(p => p
            .Add(x => x.HasSidebar, true)
            .Add(x => x.StateService, stateService)
            .Add(x => x.Sessions, sessions)
            .Add(x => x.ContextNodes, contextNodes));

        stateService.ProjectContext(parentNode);
        stateService.SetAgentType("create");

        cut.Find("[data-testid='session-block-command-bar-toggle']").Click();

        var commandBar = cut.FindComponent<SessionCommandBar>();
        await commandBar.InvokeAsync(() => commandBar.Instance.OnSubmit.InvokeAsync(
            new SessionCommandSubmitted(parentNode, "create", "Write tests")));

        cut.Markup.ShouldContain("title=\"workitem\"");
        cut.Markup.ShouldContain(">Write tests</span>");
        stateService.State.Ui.ActiveAgentTypeKey.ShouldBe("ask");
    }

    [Fact(DisplayName = "UAC05: Submitting create on a use-case node resolves the UserStoryCreator tool")]
    [Trait(Spec.UAC, "05")]
    [Given("a SessionBlock with a use-case node selected in create mode")]
    [When("the user submits a title")]
    [Then("UserStoryCreatorTool.ExecuteAsync is invoked and a user-story child is added")]
    public async Task UAC05_CreateOnUseCaseNodeInvokesUserStoryCreator()
    {
        var (cut, stateService, parentNode) = RenderNodeInCreateMode("use-case", "Use Cases");

        cut.Find("[data-testid='session-block-command-bar-toggle']").Click();

        var commandBar = cut.FindComponent<SessionCommandBar>();
        await commandBar.InvokeAsync(() => commandBar.Instance.OnSubmit.InvokeAsync(
            new SessionCommandSubmitted(parentNode, "create", "As a user I can sign in")));

        cut.Markup.ShouldContain("title=\"user-story\"");
        cut.Markup.ShouldContain(">As a user I can sign in</span>");
        stateService.State.Ui.ActiveAgentTypeKey.ShouldBe("ask");
    }

    [Fact(DisplayName = "UAC06: Submitting create on a user-story node resolves the ScenarioCreator tool")]
    [Trait(Spec.UAC, "06")]
    [Given("a SessionBlock with a user-story node selected in create mode")]
    [When("the user submits a title")]
    [Then("ScenarioCreatorTool.ExecuteAsync is invoked and a scenario child is added")]
    public async Task UAC06_CreateOnUserStoryNodeInvokesScenarioCreator()
    {
        var (cut, stateService, parentNode) = RenderNodeInCreateMode("user-story", "User Stories");

        cut.Find("[data-testid='session-block-command-bar-toggle']").Click();

        var commandBar = cut.FindComponent<SessionCommandBar>();
        await commandBar.InvokeAsync(() => commandBar.Instance.OnSubmit.InvokeAsync(
            new SessionCommandSubmitted(parentNode, "create", "Happy path sign-in")));

        cut.Markup.ShouldContain("title=\"scenario\"");
        cut.Markup.ShouldContain(">Happy path sign-in</span>");
        stateService.State.Ui.ActiveAgentTypeKey.ShouldBe("ask");
    }

    [Fact(DisplayName = "UAC07: Submitting create on a scenario node resolves the ScenarioStepCreator tool")]
    [Trait(Spec.UAC, "07")]
    [Given("a SessionBlock with a scenario node selected in create mode")]
    [When("the user submits a title")]
    [Then("ScenarioStepCreatorTool.ExecuteAsync is invoked and a scenario-step child is added")]
    public async Task UAC07_CreateOnScenarioNodeInvokesScenarioStepCreator()
    {
        var (cut, stateService, parentNode) = RenderNodeInCreateMode("scenario", "Scenarios");

        cut.Find("[data-testid='session-block-command-bar-toggle']").Click();

        var commandBar = cut.FindComponent<SessionCommandBar>();
        await commandBar.InvokeAsync(() => commandBar.Instance.OnSubmit.InvokeAsync(
            new SessionCommandSubmitted(parentNode, "create", "Enter credentials")));

        cut.Markup.ShouldContain("title=\"scenario-step\"");
        cut.Markup.ShouldContain(">Enter credentials</span>");
        stateService.State.Ui.ActiveAgentTypeKey.ShouldBe("ask");
    }

    [Fact(DisplayName = "UAC08: Create mode command-bar context shows the edge target noun and transition key")]
    [Trait(Spec.UAC, "08")]
    [Given("a SessionBlock with a workspaces.header node selected in create mode")]
    [When("the command bar context chip is inspected")]
    [Then("it displays the target noun workspace and the computed transition key")]
    public void UAC08_CreateModeContextShowsTargetNounAndTransitionKey()
    {
        var (cut, _, _) = RenderWorkspacesHeaderInCreateMode();

        cut.Find("[data-testid='session-block-command-bar-toggle']").Click();

        var context = cut.Find("[data-testid='session-command-context'] span");
        context.GetAttribute("data-node-type").ShouldBe("workspace");
        context.GetAttribute("data-transition-key").ShouldBe("workspaces.create.workspace");
    }

    [Fact(DisplayName = "UAC09: Workspace create edges expose navigation-header child metadata")]
    [Trait(Spec.UAC, "09")]
    [Given("the EdgeTypeService for workspace")]
    [When("outgoing create edges are inspected")]
    [Then("page, table, workflow, branch and feature edges carry header metadata")]
    public void UAC09_WorkspaceCreateEdgesAreHeaderChildConvenienceEdges()
    {
        var edgeService = Ctx.Services.GetRequiredService<IEdgeTypeService>();
        var edges = edgeService.GetOutgoingEdges("workspace");

        var pageEdge = edges.FirstOrDefault(e =>
            string.Equals(e.TargetNodeTypeKey, "page", StringComparison.OrdinalIgnoreCase)
            && string.Equals(e.Intent, "create", StringComparison.OrdinalIgnoreCase));

        pageEdge.ShouldNotBeNull();
        pageEdge!.IsNavigationHeaderChild.ShouldBeTrue();
        pageEdge.HeaderLabel.ShouldBe("Pages");
        pageEdge.HeaderNodeTypeKey.ShouldBe("header");
    }

    [Fact(DisplayName = "UAC10: Generic navigation headers resolve create mode to their labelled target type")]
    [Trait(Spec.UAC, "10")]
    [Given("a generic Tables header selected in create mode")]
    [When("the command bar context chip is inspected")]
    [Then("it displays table rather than the first generic header target")]
    public void UAC10_GenericHeaderCreateModeUsesLabelledTargetType()
    {
        var (cut, _, _) = RenderNodeInCreateMode("header", "Tables");

        cut.Find("[data-testid='session-block-command-bar-toggle']").Click();

        var context = cut.Find("[data-testid='session-command-context'] span");
        context.GetAttribute("data-node-type").ShouldBe("table");
        context.GetAttribute("data-transition-key").ShouldBe("header.create.table");
    }

    [Fact(DisplayName = "UAC11: Creating under a lazy header immediately refreshes that parent's children")]
    [Trait(Spec.UAC, "11")]
    [Given("a Tables header loaded beneath the user context")]
    [When("a table is created under that header")]
    [Then("the table appears as a child of the Tables header")]
    public async Task UAC11_CreateRefreshesLazyParentHeader()
    {
        var graph = new FakeGraphService();
        graph.SeedNodeType("header");
        graph.SeedNodeType("table");
        graph.SeedEdgeType("has.header", "user-context", "header");
        graph.SeedEdgeType("has.table", "header", "table", "create");

        var userContextId = Guid.NewGuid();
        var tablesHeaderId = Guid.NewGuid();
        graph.SeedUserContext(userContextId, "User");
        graph.SeedNode(tablesHeaderId, "header", "Tables");
        graph.SeedEdge("user.has.header", "has.header", userContextId, tablesHeaderId);

        var stateService = new SessionStateService(null as ISessionSurfaceDiscovery, graph);
        var ct = CancellationToken.None;
        await stateService.InitializeAsync(userContextId.ToString(), "User", ct);

        var result = await stateService.CreateHeaderChildAsync(
            new SessionHeaderCreateRequest(tablesHeaderId.ToString(), "header", "SomeTable"),
            userContextId.ToString(),
            ct);

        result.Success.ShouldBeTrue();
        var tablesHeader = stateService.ContextNodes.Single().Children!.Single();
        tablesHeader.Node.Label.ShouldBe("Tables");
        tablesHeader.Children.ShouldNotBeNull();
        tablesHeader.Children!.Single().Node.NodeTypeKey.ShouldBe("table");
        tablesHeader.Children.Single().Node.Label.ShouldBe("SomeTable");
    }

    [Fact(DisplayName = "UAC12: Table exposes Column as a navigation-header child create option")]
    [Trait(Spec.UAC, "12")]
    [Given("the registered table node type")]
    [When("its outgoing create edges are inspected")]
    [Then("a Column option targets a Columns navigation header")]
    public void UAC12_TableExposesColumnCreateOption()
    {
        var edgeService = Ctx.Services.GetRequiredService<IEdgeTypeService>();

        var columnEdges = edgeService.GetOutgoingEdges("table").Where(edge =>
            string.Equals(edge.TargetNodeTypeKey, "column", StringComparison.OrdinalIgnoreCase)
            && string.Equals(edge.Intent, "create", StringComparison.OrdinalIgnoreCase)
            && edge.IsNavigationHeaderChild).ToList();
        columnEdges.Count.ShouldBe(1);
        var columnEdge = columnEdges.Single();

        columnEdge.IsNavigationHeaderChild.ShouldBeTrue();
        columnEdge.HeaderLabel.ShouldBe("Columns");
        columnEdge.TargetDescriptor.ShouldNotBeNull();
        columnEdge.TargetDescriptor!.Label.ShouldBe("Column");
    }

    [Fact(DisplayName = "UAC13: Columns header only exposes the Column create edge")]
    [Trait(Spec.UAC, "13")]
    [Given("a generic navigation header whose role is Columns")]
    [When("its create edges are projected for the context menu")]
    [Then("only the header-to-column edge is returned")]
    public void UAC13_ColumnsHeaderOnlyExposesColumnCreateEdge()
    {
        var edgeService = Ctx.Services.GetRequiredService<IEdgeTypeService>();
        var columnsHeader = new SessionNodeInput("columns-header", "header", "Columns", IsHeader: true);

        var createEdges = edgeService.GetCreateEdges(columnsHeader);

        createEdges.Count.ShouldBe(1);
        createEdges.Single().TargetNodeTypeKey.ShouldBe("column");
        createEdges.Single().SourceNodeTypeKey.ShouldBe("header");
    }

    [Fact(DisplayName = "UAC14: Unknown generic header exposes no create edges")]
    [Trait(Spec.UAC, "14")]
    [Given("a generic navigation header with no declared role")]
    [When("its create edges are projected for the context menu")]
    [Then("no unrelated global header edges are returned")]
    public void UAC14_UnknownHeaderExposesNoCreateEdges()
    {
        var edgeService = Ctx.Services.GetRequiredService<IEdgeTypeService>();
        var unknownHeader = new SessionNodeInput("unknown-header", "header", "Unknown", IsHeader: true);

        edgeService.GetCreateEdges(unknownHeader).ShouldBeEmpty();
    }

    private (IRenderedComponent<Components.SessionBlock> Cut, SessionStateService StateService, SessionNodeInput ParentNode) RenderNodeInCreateMode(string nodeTypeKey, string sessionLabel)
    {
        var stateService = new SessionStateService();
        var sessions = new[] { new SessionListEntry("s1", sessionLabel, 0, "session") };
        stateService.SelectSession(new SessionNodeInput("s1", "session", sessionLabel), sessions);

        var parentNode = new SessionNodeInput($"{nodeTypeKey}-node", nodeTypeKey, sessionLabel);
        var contextNodes = new[]
        {
            new SessionContextNode(parentNode, IsLoaded: true)
        };

        var cut = Ctx.Render<Components.SessionBlock>(p => p
            .Add(x => x.HasSidebar, true)
            .Add(x => x.StateService, stateService)
            .Add(x => x.Sessions, sessions)
            .Add(x => x.ContextNodes, contextNodes));

        stateService.ProjectContext(parentNode);
        stateService.SetAgentType("create");

        return (cut, stateService, parentNode);
    }

    private (IRenderedComponent<Components.SessionBlock> Cut, SessionStateService StateService, SessionNodeInput ParentNode) RenderWorkspacesHeaderInCreateMode()
    {
        var stateService = new SessionStateService();
        var sessions = new[] { new SessionListEntry("s1", "Workspaces", 0, "session") };
        stateService.SelectSession(new SessionNodeInput("s1", "session", "Workspaces"), sessions);

        var parentNode = new SessionNodeInput("ws-header", "workspaces.header", "Workspaces");
        var contextNodes = new[]
        {
            new SessionContextNode(parentNode, IsLoaded: true)
        };

        var cut = Ctx.Render<Components.SessionBlock>(p => p
            .Add(x => x.HasSidebar, true)
            .Add(x => x.StateService, stateService)
            .Add(x => x.Sessions, sessions)
            .Add(x => x.ContextNodes, contextNodes));

        stateService.ProjectContext(parentNode);
        stateService.SetAgentType("create");

        return (cut, stateService, parentNode);
    }
}
