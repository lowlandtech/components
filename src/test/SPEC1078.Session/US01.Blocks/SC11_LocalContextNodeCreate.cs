namespace LowlandTech.Components.Tests.SPEC1078.Session.US01.Blocks;

[Trait(Spec.SPEC, "1078")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "11")]
[UserStory("01", "Local context node creation in Create mode")]
public sealed class SC11_LocalContextNodeCreate : ViewScenario
{
    [Fact(DisplayName = "UAC01: Create mode shows a title placeholder in the command bar")]
    [Trait(Spec.UAC, "01")]
    [Given("a SessionBlock with a non-session node projected in create mode")]
    [When("the command bar is visible")]
    [Then("the placeholder prompts for a new node title")]
    public void UAC01_CreateModeShowsTitlePlaceholder()
    {
        var (cut, stateService, parentNode) = RenderInCreateMode();

        cut.Find("[data-testid='session-block-command-bar-toggle']").Click();

        var input = cut.Find("[data-testid='session-command-input']");
        input.GetAttribute("placeholder").ShouldBe("Enter a title for the new node...");
    }

    [Fact(DisplayName = "UAC02: Submitting a title in create mode creates a child under the parent")]
    [Trait(Spec.UAC, "02")]
    [Given("a SessionBlock in create mode with a context node")]
    [When("the user types a title and submits")]
    [Then("a new child node appears under the parent in the tree")]
    public async Task UAC02_CreateModeCreatesChildUnderParent()
    {
        var (cut, stateService, parentNode) = RenderInCreateMode();

        cut.Find("[data-testid='session-block-command-bar-toggle']").Click();

        var commandBar = cut.FindComponent<SessionCommandBar>();
        await commandBar.InvokeAsync(() => commandBar.Instance.OnSubmit.InvokeAsync(
            new SessionCommandSubmitted(parentNode, "create", "New Order")));

        cut.FindAll("[data-slot='session-context-node']")
            .SingleOrDefault(node => node.TextContent.Trim() == "New Order")
            .ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC03: Created child inherits the parent's component block and resolved type")]
    [Trait(Spec.UAC, "03")]
    [Given("a SessionBlock in create mode with a workitem context node")]
    [When("the user submits a title")]
    [Then("the created child is a workitem with NodeBlock")]
    public async Task UAC03_CreatedChildInheritsTypeAndBlock()
    {
        var (cut, stateService, parentNode) = RenderInCreateMode();

        cut.Find("[data-testid='session-block-command-bar-toggle']").Click();

        var commandBar = cut.FindComponent<SessionCommandBar>();
        await commandBar.InvokeAsync(() => commandBar.Instance.OnSubmit.InvokeAsync(
            new SessionCommandSubmitted(parentNode, "create", "New Order")));

        var newNode = cut.FindAll("[data-slot='session-context-node']")
            .Single(node => node.TextContent.Trim() == "New Order");

        newNode.GetAttribute("data-node-id").ShouldNotBe(parentNode.Id);
        newNode.QuerySelector("span[title='workitem']")?.TextContent.Trim().ShouldBe("New Order");
    }

    [Fact(DisplayName = "UAC04: Agent type resets to ask after local create")]
    [Trait(Spec.UAC, "04")]
    [Given("a SessionBlock in create mode")]
    [When("the user submits a title")]
    [Then("the active agent type returns to ask")]
    public async Task UAC04_AgentTypeResetsAfterLocalCreate()
    {
        var (cut, stateService, parentNode) = RenderInCreateMode();

        cut.Find("[data-testid='session-block-command-bar-toggle']").Click();

        var commandBar = cut.FindComponent<SessionCommandBar>();
        await commandBar.InvokeAsync(() => commandBar.Instance.OnSubmit.InvokeAsync(
            new SessionCommandSubmitted(parentNode, "create", "New Order")));

        stateService.State.Ui.ActiveAgentTypeKey.ShouldBe("ask");
    }

    [Fact(DisplayName = "UAC05: Wired host handler takes precedence over local fallback")]
    [Trait(Spec.UAC, "05")]
    [Given("a SessionBlock with OnHeaderCreateRequested wired")]
    [When("the user submits a title in create mode")]
    [Then("the host callback is invoked instead of creating locally")]
    public async Task UAC05_HostHandlerTakesPrecedence()
    {
        var stateService = new SessionStateService();
        var sessions = new[] { new SessionListEntry("s1", "Orders", 0, "session") };
        stateService.SelectSession(new SessionNodeInput("s1", "session", "Orders"), sessions);

        var parentNode = new SessionNodeInput("orders", "workitem", "Orders", "NodeBlock");
        var contextNodes = new[]
        {
            new SessionContextNode(
                parentNode,
                new[] { new SessionContextNode(new SessionNodeInput("existing", "workitem", "Existing", "NodeBlock"), IsLoaded: true) },
                true)
        };

        SessionHeaderCreateRequest? request = null;
        var cut = Ctx.Render<Components.SessionBlock>(p => p
            .Add(x => x.HasSidebar, true)
            .Add(x => x.StateService, stateService)
            .Add(x => x.Sessions, sessions)
            .Add(x => x.ContextNodes, contextNodes)
            .Add(x => x.OnHeaderCreateRequested, r => { request = r; return Task.CompletedTask; }));

        stateService.ProjectContext(parentNode);
        stateService.SetAgentType("create");

        cut.Find("[data-testid='session-block-command-bar-toggle']").Click();

        var commandBar = cut.FindComponent<SessionCommandBar>();
        await commandBar.InvokeAsync(() => commandBar.Instance.OnSubmit.InvokeAsync(
            new SessionCommandSubmitted(parentNode, "create", "New Order")));

        request.ShouldNotBeNull();
        request.ParentNodeId.ShouldBe(parentNode.Id);
        request.Label.ShouldBe("New Order");
    }

    private (IRenderedComponent<Components.SessionBlock> Cut, SessionStateService StateService, SessionNodeInput ParentNode) RenderInCreateMode()
    {
        var stateService = new SessionStateService();
        var sessions = new[] { new SessionListEntry("s1", "Orders", 0, "session") };
        stateService.SelectSession(new SessionNodeInput("s1", "session", "Orders"), sessions);

        var parentNode = new SessionNodeInput("orders", "workitem", "Orders", "NodeBlock");
        var contextNodes = new[]
        {
            new SessionContextNode(
                parentNode,
                new[] { new SessionContextNode(new SessionNodeInput("existing", "workitem", "Existing", "NodeBlock"), IsLoaded: true) },
                true)
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
