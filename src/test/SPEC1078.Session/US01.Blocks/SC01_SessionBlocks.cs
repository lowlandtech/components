namespace LowlandTech.Components.Tests.SPEC1078.Session.US01.Blocks;

[Trait(Spec.SPEC, "1078")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Session blocks expose shared state, actions, reducers, events, and node-shaped callbacks")]
public class SC01_SessionBlocks : ViewScenario
{
    private static readonly SessionNodeInput OrdersNode = new("orders", "session", "Orders");

    [Fact(DisplayName = "UAC01: Reducer selects a node and appends it to open nodes")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_ReducerSelectsNode()
    {
        var state = SessionUiReducer.Reduce(SessionUiDefaults.Empty, new SelectSessionNode(OrdersNode));

        state.SelectedNode.ShouldBe(OrdersNode);
        state.OpenNodes.ShouldContain(OrdersNode);
    }

    [Fact(DisplayName = "UAC02: SessionCommandBar emits node-shaped submit event and clears shared input")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_CommandBarSubmitsNodeInput()
    {
        SessionCommandSubmitted? submitted = null;
        SessionUiState? changedState = null;
        var state = SessionUiDefaults.Empty with
        {
            SelectedNode = OrdersNode,
            Input = "Summarize orders",
            ActiveAgentTypeKey = "ask"
        };

        var cut = Ctx.Render<SessionCommandBar>(p => p
            .Add(x => x.State, state)
            .Add(x => x.OnSubmit, e => submitted = e)
            .Add(x => x.StateChanged, s => changedState = s));

        cut.Find("[data-testid='session-command-submit']").Click();

        submitted.ShouldNotBeNull();
        submitted.Node.ShouldBe(OrdersNode);
        submitted.Input.ShouldBe("Summarize orders");
        submitted.AgentTypeKey.ShouldBe("ask");
        changedState.ShouldNotBeNull();
        changedState.Input.ShouldBe("");
    }

    [Fact(DisplayName = "UAC03: SessionCommandBar agent type invokes configured node callback")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_CommandBarAgentTypeInvokesNodeCallback()
    {
        SessionNodeInput? callbackNode = null;
        SessionAgentTypeSelected? selected = null;
        var agents = new[]
        {
            new SessionAgentType("ask", "Ask"),
            new SessionAgentType("plan", "Plan", OnSelected: EventCallback.Factory.Create<SessionNodeInput>(this, node => callbackNode = node))
        };

        var cut = Ctx.Render<SessionCommandBar>(p => p
            .Add(x => x.SelectedNode, OrdersNode)
            .Add(x => x.AgentTypes, agents)
            .Add(x => x.OnAgentTypeSelected, e => selected = e));

        cut.Find("[data-agent-type='plan']").Click();

        callbackNode.ShouldBe(OrdersNode);
        selected.ShouldNotBeNull();
        selected.Node.ShouldBe(OrdersNode);
        selected.AgentTypeKey.ShouldBe("plan");
    }

    [Fact(DisplayName = "UAC04: SessionList emits selected node and updates shared state")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_SessionListSelectsNode()
    {
        SessionNodeSelected? selected = null;
        SessionUiState? changedState = null;
        var items = new[]
        {
            new SessionListEntry("orders", "Orders", 12),
            new SessionListEntry("support", "Support triage", 7)
        };

        var cut = Ctx.Render<SessionList>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.OnSelected, e => selected = e)
            .Add(x => x.StateChanged, s => changedState = s));

        cut.Find("[data-node-id='support']").Click();

        selected.ShouldNotBeNull();
        selected.Node.Id.ShouldBe("support");
        selected.Node.NodeTypeKey.ShouldBe("session");
        changedState.ShouldNotBeNull();
        changedState.SelectedNode!.Id.ShouldBe("support");
    }

    [Fact(DisplayName = "UAC05: SessionCard action emits node-shaped action event")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_SessionCardActionEmitsNodeInput()
    {
        SessionNodeInput? configuredNode = null;
        SessionCardActionSelected? selected = null;
        var actions = new[]
        {
            new SessionCardAction("create-task", "Create task", EventCallback.Factory.Create<SessionNodeInput>(this, node => configuredNode = node))
        };

        var cut = Ctx.Render<SessionCard>(p => p
            .Add(x => x.Node, OrdersNode)
            .Add(x => x.Actions, actions)
            .Add(x => x.OnActionSelected, e => selected = e));

        cut.Find("button").Click();

        configuredNode.ShouldBe(OrdersNode);
        selected.ShouldNotBeNull();
        selected.Node.ShouldBe(OrdersNode);
        selected.ActionKey.ShouldBe("create-task");
    }

    [Fact(DisplayName = "UAC06: SessionContent uses IDE tabs and emits selected node")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_SessionContentUsesIdeTabsAndSelectsTab()
    {
        SessionNodeSelected? selected = null;
        var nodes = new[]
        {
            OrdersNode,
            new SessionNodeInput("support", "session", "Support triage")
        };

        var cut = Ctx.Render<SessionContent>(p => p
            .Add(x => x.OpenNodes, nodes)
            .Add(x => x.SelectedNode, OrdersNode)
            .Add(x => x.OnSelected, e => selected = e));

        cut.Find("[data-tabs-mode='dynamic']").ShouldNotBeNull();
        cut.Find("[data-variant='ide']").ShouldNotBeNull();

        cut.Find("#tabs-trigger-support").Click();

        selected.ShouldNotBeNull();
        selected.Node.Id.ShouldBe("support");
    }

    [Fact(DisplayName = "UAC07: SessionCommandBar replaces send icon with loader while busy")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_CommandBarShowsBusyLoader()
    {
        SessionCommandSubmitted? submitted = null;

        var cut = Ctx.Render<SessionCommandBar>(p => p
            .Add(x => x.SelectedNode, OrdersNode)
            .Add(x => x.Value, "Summarize orders")
            .Add(x => x.IsBusy, true)
            .Add(x => x.OnSubmit, e => submitted = e));

        cut.Find("[data-testid='session-command-busy-loader']").ShouldNotBeNull();
        cut.FindAll("[data-slot='icon']").Count.ShouldBe(0);
        cut.Find("[data-testid='session-command-submit']").Click();

        submitted.ShouldBeNull();
    }

    [Fact(DisplayName = "UAC08: SessionCommandBar submit executes the active agent's configured action")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_CommandBarExecutesConfiguredAgentSubmit()
    {
        SessionCommandSubmitted? configuredSubmit = null;
        SessionCommandSubmitted? commonSubmit = null;
        var agents = new[]
        {
            new SessionAgentType("ask", "Ask"),
            new SessionAgentType(
                "plan",
                "Plan",
                OnSubmit: EventCallback.Factory.Create<SessionCommandSubmitted>(this, e => configuredSubmit = e))
        };

        var cut = Ctx.Render<SessionCommandBar>(p => p
            .Add(x => x.SelectedNode, OrdersNode)
            .Add(x => x.AgentTypes, agents)
            .Add(x => x.ActiveAgentTypeKey, "plan")
            .Add(x => x.Value, "Make a plan")
            .Add(x => x.OnSubmit, e => commonSubmit = e));

        cut.Find("[data-testid='session-command-submit']").Click();

        configuredSubmit.ShouldNotBeNull();
        configuredSubmit.Node.ShouldBe(OrdersNode);
        configuredSubmit.AgentTypeKey.ShouldBe("plan");
        configuredSubmit.Input.ShouldBe("Make a plan");
        commonSubmit.ShouldNotBeNull();
        commonSubmit.AgentTypeKey.ShouldBe("plan");
    }
}
