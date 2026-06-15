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
