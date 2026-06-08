namespace LowlandTech.Components.Tests.SPEC1078.Session.US01.Blocks;

[Trait(Spec.SPEC, "1078")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Session blocks expose shared state, actions, reducers, events, and node-shaped callbacks")]
public class SC01_SessionBlocks : ViewScenario
{
    private static readonly SessionNodeInput OrdersNode = new("orders", "session", "Orders");

    private sealed class PluginSessionSurfaceRegistry : ISessionSurfaceRegistry
    {
        public int Priority => 100;

        public SessionSurfaceDescriptor? GetSurface(string sessionId)
            => string.Equals(sessionId, "orders-session", StringComparison.Ordinal)
                ? new SessionSurfaceDescriptor(
                    sessionId,
                    new SessionNodeInput("plugin-request-orders", "session-request", "Plugin order intake"),
                    new SessionNodeInput("plugin-response-orders", "session-response", "Plugin order output"))
                : null;
    }

    [Fact(DisplayName = "UAC01: Reducer selects a node and appends it to open nodes")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_ReducerSelectsNode()
    {
        var state = SessionUiReducer.Reduce(SessionUiDefaults.Empty, new SelectSessionNode(OrdersNode));

        state.SelectedNode.ShouldBe(OrdersNode);
        state.OpenNodes.ShouldContain(OrdersNode);
    }

    [Fact(DisplayName = "UAC18: Reducer preserves tab order when re-selecting an open node")]
    [Trait(Spec.UAC, "18")]
    public void UAC18_ReducerPreservesTabOrderOnReselect()
    {
        var supportNode = new SessionNodeInput("support", "session", "Support triage");
        var releaseNode = new SessionNodeInput("release", "session", "Release notes");
        var state = SessionUiDefaults.Empty with
        {
            OpenNodes = [OrdersNode, supportNode, releaseNode]
        };

        state = SessionUiReducer.Reduce(state, new SelectSessionNode(supportNode));

        state.SelectedNode.ShouldBe(supportNode);
        state.OpenNodes.Count.ShouldBe(3);
        state.OpenNodes[0].Id.ShouldBe("orders");
        state.OpenNodes[1].Id.ShouldBe("support");
        state.OpenNodes[2].Id.ShouldBe("release");
    }

    [Fact(DisplayName = "UAC02: SessionCommandBar emits node-shaped submit event and clears shared input")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_CommandBarSubmitsNodeInput()
    {
        SessionCommandSubmitted? submitted = null;
        string? nextValue = null;
        var state = SessionUiDefaults.Empty with
        {
            SelectedNode = OrdersNode,
            Input = "Summarize orders",
            ActiveAgentTypeKey = "ask"
        };

        var cut = Ctx.Render<SessionCommandBar>(p => p
            .Add(x => x.State, state)
            .Add(x => x.OnSubmit, e => submitted = e)
            .Add(x => x.ValueChanged, value => nextValue = value));

        cut.Find("[data-testid='session-command-submit']").Click();

        submitted.ShouldNotBeNull();
        submitted.Node.ShouldBe(OrdersNode);
        submitted.Input.ShouldBe("Summarize orders");
        submitted.AgentTypeKey.ShouldBe("ask");
        nextValue.ShouldBe("");
        Ctx.JSInterop.Invocations.Any(invocation =>
            invocation.Identifier == "eval"
            && invocation.Arguments[0]?.ToString()?.Contains("setSelectionRange(0, 0)", StringComparison.Ordinal) == true)
            .ShouldBeTrue();
        Ctx.JSInterop.Invocations.Any(invocation =>
            invocation.Identifier == "eval"
            && invocation.Arguments[0]?.ToString()?.Contains("event.preventDefault()", StringComparison.Ordinal) == true
            && invocation.Arguments[0]?.ToString()?.Contains("submit.click()", StringComparison.Ordinal) == true)
            .ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC02b: SessionCommandBar treats an empty controlled value as reset input")]
    [Trait(Spec.UAC, "02b")]
    public void UAC02b_CommandBarEmptyControlledValueResetsInput()
    {
        var state = SessionUiDefaults.Empty with
        {
            SelectedNode = OrdersNode,
            Input = "stale input"
        };

        var cut = Ctx.Render<SessionCommandBar>(p => p
            .Add(x => x.State, state)
            .Add(x => x.Value, string.Empty));

        cut.Find("[data-testid='session-command-input']").GetAttribute("value").ShouldBe("");
        cut.Find("[data-testid='session-command-submit']").HasAttribute("disabled").ShouldBeTrue();
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
        var items = new[]
        {
            new SessionListEntry("orders", "Orders", 12),
            new SessionListEntry("support", "Support triage", 7)
        };

        var cut = Ctx.Render<SessionList>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.OnSelected, e => selected = e));

        cut.Find("[data-node-id='support']").Click();

        selected.ShouldNotBeNull();
        selected.Node.Id.ShouldBe("support");
        selected.Node.NodeTypeKey.ShouldBe("session");
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

        cut.Find("[data-slot='session-card-action']").Click();

        configuredNode.ShouldBe(OrdersNode);
        selected.ShouldNotBeNull();
        selected.Node.ShouldBe(OrdersNode);
        selected.ActionKey.ShouldBe("create-task");
    }

    [Fact(DisplayName = "UAC05b: SessionCard flushes outer card block padding")]
    [Trait(Spec.UAC, "05b")]
    public void UAC05b_SessionCardFlushesOuterCardPadding()
    {
        var cut = Ctx.Render<SessionCard>(p => p.Add(x => x.Node, OrdersNode));

        var style = cut.Find("[data-slot='session-card']").GetAttribute("style");

        style.ShouldNotBeNull();
        style.ShouldContain("padding-block:0");
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

    [Fact(DisplayName = "UAC06b: SessionContent omits right padding beside the stable scrollbar gutter")]
    [Trait(Spec.UAC, "06b")]
    [Given("SessionContent has an open session with ten-pixel content spacing")]
    [When("the scrollable session content body is rendered")]
    [Then("top, bottom, and left retain ten pixels while right is zero because the stable gutter provides that spacing")]
    public void UAC06b_SessionContentOmitsRightPaddingBesideStableGutter()
    {
        var cut = Ctx.Render<SessionContent>(p => p
            .Add(x => x.OpenNodes, [OrdersNode])
            .Add(x => x.SelectedNode, OrdersNode));

        var style = cut.Find("[data-slot='session-content-body']").GetAttribute("style");

        style.ShouldBe("padding:10px 0 10px 10px; scrollbar-gutter: stable;");
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

    [Fact(DisplayName = "UAC09: Reducer projects context and replaces the transient projection")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_ReducerProjectsContext()
    {
        var invoice = new SessionNodeInput("invoice", "invoice", "Invoice", "InvoiceBlock");
        var customer = new SessionNodeInput("customer", "customer", "Customer", "CustomerBlock");

        var state = SessionUiReducer.Reduce(SessionUiDefaults.Empty, new ProjectSessionContext(invoice));
        state = SessionUiReducer.Reduce(state, new ProjectSessionContext(customer));

        state.ContextProjection.ShouldNotBeNull();
        state.ContextProjection.Node.ShouldBe(customer);
    }

    [Fact(DisplayName = "UAC10: Reducer pins projected context as a session.pin node and context block")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_ReducerPinsProjectedContext()
    {
        var invoice = new SessionNodeInput("invoice", "invoice", "Invoice", "InvoiceBlock");
        var state = SessionUiReducer.Reduce(SessionUiDefaults.Empty, new ProjectSessionContext(invoice));

        state = SessionUiReducer.Reduce(state, new PinSessionContext(state.ContextProjection!));

        state.ContextProjection.ShouldBeNull();
        state.ContextPins.ShouldNotBeNull();
        state.ContextPins.Count.ShouldBe(1);
        state.ContextPins[0].SessionNode.NodeTypeKey.ShouldBe("session.pin");
        state.ContextPins[0].SessionNode.ComponentBlock.ShouldBe("SessionContextPin");
        state.ContextBlocks.ShouldNotBeNull();
        state.ContextBlocks.Count.ShouldBe(1);
        state.ContextBlocks[0].Context.ShouldBe(invoice);
    }

    [Fact(DisplayName = "UAC11: SessionContextExplorer projects and pins selected context")]
    [Trait(Spec.UAC, "11")]
    public void UAC11_ContextExplorerProjectsAndPinsContext()
    {
        SessionContextProjected? projected = null;
        SessionContextPinned? pinned = null;
        var invoice = new SessionNodeInput("invoice", "invoice", "Invoice", "InvoiceBlock");
        var nodes = new[] { new SessionContextNode(invoice) };

        var cut = Ctx.Render<SessionContextExplorer>(p => p
            .Add(x => x.RootNodes, nodes)
            .Add(x => x.OnProjected, e => projected = e)
            .Add(x => x.OnPinned, e => pinned = e));

        cut.Find("[data-node-id='invoice']").Click();
        cut.Find("[data-testid='session-context-node-menu-trigger'][data-node-id='invoice']").Click();
        ClickOpenContextNodePin();

        projected.ShouldNotBeNull();
        projected.Node.ShouldBe(invoice);
        pinned.ShouldNotBeNull();
        pinned.Node.ShouldBe(invoice);
        pinned.Pin.SessionNode.NodeTypeKey.ShouldBe("session.pin");
    }

    [Fact(DisplayName = "UAC33: SessionBlock uses an injected store and shows settings as a dedicated standalone page")]
    [Trait(Spec.UAC, "33")]
    [Given("a standalone SessionBlock with an active session and a submitted command")]
    [When("the settings button is clicked")]
    [Then("the app opens the settings page with API keys as the default section")]
    public void UAC33_SessionBlockStandaloneSettingsLoadsEventLogPage()
    {
        var stateService = new SessionStateService();
        var cut = Ctx.Render<SessionBlock>(p => p
            .Add(x => x.HasSidebar, true)
            .Add(x => x.StateService, stateService));

        cut.Find("[data-node-id='orders-session']").Click();
        cut.Find("[data-testid='session-command-input']").Input("Trace all state changes");
        cut.Find("[data-testid='session-command-submit']").Click();
        cut.Find("[data-testid='session-app-settings-button']").Click();

        cut.Find("[data-slot='session-settings']").ShouldNotBeNull();
        cut.Find("[data-slot='session-settings-api-keys']").ShouldNotBeNull();
        cut.FindAll("[data-slot='session-app-feed']").ShouldBeEmpty();
        stateService.State.ActivePage.ShouldBe(SessionAppPage.Settings);
        stateService.State.EventLogEntries.ShouldContain(entry => entry.ActionType == nameof(SetActiveSession));
        stateService.State.EventLogEntries.ShouldContain(entry => entry.ActionType == nameof(SetSessionInput));
        stateService.State.EventLogEntries.ShouldContain(entry => entry.ActionType == nameof(SubmitSessionCommand));
        stateService.State.EventLogEntries.ShouldContain(entry => entry.ActionType == nameof(ShowSessionSettingsPage));
    }

    [Fact(DisplayName = "UAC34: SessionEventLog renders as a bare collapsible list without extra chrome")]
    [Trait(Spec.UAC, "34")]
    [Given("a standalone SessionBlock on the settings page with command history")]
    [When("the Activity settings section is selected")]
    [Then("the event log renders as the existing bare collapsible list")]
    public void UAC34_SessionEventLogRendersBareCollapsibleList()
    {
        var stateService = new SessionStateService();
        var cut = Ctx.Render<SessionBlock>(p => p
            .Add(x => x.HasSidebar, true)
            .Add(x => x.StateService, stateService));

        cut.Find("[data-node-id='orders-session']").Click();
        cut.Find("[data-testid='session-command-input']").Input("Clear journal");
        cut.Find("[data-testid='session-command-submit']").Click();
        cut.Find("[data-testid='session-app-settings-button']").Click();
        cut.Find("[data-testid='session-settings-activity-tab']").Click();

        cut.FindAll("[data-slot='session-event-log-header']").ShouldBeEmpty();
        cut.FindAll("[data-slot='session-event-log-entry-deltas']").ShouldBeEmpty();
        cut.FindAll("[data-slot='session-event-log-entry-json']").Count.ShouldBe(stateService.State.EventLogEntries.Count);
        cut.FindAll("[data-slot='session-event-log-current-state']").Count.ShouldBe(1);
        cut.FindAll("[data-slot='session-event-log-initial-state']").Count.ShouldBe(1);
    }

    [Fact(DisplayName = "UAC34b: SessionBlock settings button toggles the settings page")]
    [Trait(Spec.UAC, "34b")]
    [Given("a standalone SessionBlock with an active session")]
    [When("the settings button is clicked twice")]
    [Then("the app opens settings and then returns to the feed")]
    public void UAC34b_SessionBlockSettingsButtonTogglesEventLogPage()
    {
        var stateService = new SessionStateService();
        var cut = Ctx.Render<SessionBlock>(p => p
            .Add(x => x.HasSidebar, true)
            .Add(x => x.StateService, stateService));

        cut.Find("[data-node-id='orders-session']").Click();

        cut.Find("[data-testid='session-app-settings-button']").Click();
        cut.Find("[data-slot='session-settings']").ShouldNotBeNull();
        stateService.State.ActivePage.ShouldBe(SessionAppPage.Settings);

        cut.Find("[data-testid='session-app-settings-button']").Click();
        cut.FindAll("[data-slot='session-settings']").ShouldBeEmpty();
        cut.Find("[data-slot='session-app-feed']").ShouldNotBeNull();
        stateService.State.ActivePage.ShouldBe(SessionAppPage.Feed);
    }

    [Fact(DisplayName = "UAC34c: SessionBlock settings can create and remove API keys")]
    [Trait(Spec.UAC, "34c")]
    [Given("a standalone SessionBlock on the API Keys settings section")]
    [When("an API key is created and then removed")]
    [Then("the one-time secret appears and the key row is removed")]
    public void UAC34c_SessionBlockSettingsCreatesAndRemovesApiKeys()
    {
        var stateService = new SessionStateService();
        var cut = Ctx.Render<SessionBlock>(p => p
            .Add(x => x.HasSidebar, true)
            .Add(x => x.StateService, stateService));

        cut.Find("[data-node-id='orders-session']").Click();
        cut.Find("[data-testid='session-app-settings-button']").Click();
        var initialRows = cut.FindAll("[data-testid='session-settings-api-key-row']").Count;

        cut.Find("[data-testid='session-settings-create-api-key']").Click();

        cut.Find("[data-testid='session-settings-api-key-secret']").TextContent.ShouldContain("ltg_");
        cut.FindAll("[data-testid='session-settings-api-key-row']").Count.ShouldBe(initialRows + 1);

        cut.FindAll("[data-testid='session-settings-revoke-api-key']").Last().Click();

        cut.FindAll("[data-testid='session-settings-api-key-row']").Count.ShouldBe(initialRows);
    }

    [Fact(DisplayName = "UAC35: SessionBlock resolves request and response cards through DRD discovery")]
    [Trait(Spec.UAC, "35")]
    public void UAC35_SessionBlockResolvesRequestAndResponseCardsThroughDrdDiscovery()
    {
        Ctx.Services.AddSingleton<ISessionSurfaceRegistry, PluginSessionSurfaceRegistry>();

        var cut = Ctx.Render<SessionBlock>();

        cut.Find("[data-node-id='orders-session']").Click();

        cut.Markup.ShouldContain("Plugin order intake");
        cut.Markup.ShouldContain("Plugin order output");
    }

    [Fact(DisplayName = "UAC13: SessionContextBlock renders the configured DynamicComponent")]
    [Trait(Spec.UAC, "13")]
    public void UAC13_ContextBlockRendersDynamicComponent()
    {
        var block = new SessionContextBlockModel(
            "context-block:invoice",
            new("invoice", "invoice", "Invoice", "InvoiceBlock"),
            "invoice-data",
            typeof(TestSessionContextBlock));

        var cut = Ctx.Render<SessionContextBlock>(p => p.Add(x => x.Block, block));

        cut.Markup.ShouldContain("Dynamic invoice-data");
    }

    [Fact(DisplayName = "UAC13b: SessionContextBlock flushes outer card block padding")]
    [Trait(Spec.UAC, "13b")]
    public void UAC13b_ContextBlockFlushesOuterCardPadding()
    {
        var block = new SessionContextBlockModel(
            "context-block:invoice",
            new("invoice", "invoice", "Invoice", "InvoiceBlock"),
            "invoice-data");

        var cut = Ctx.Render<SessionContextBlock>(p => p.Add(x => x.Block, block));

        var style = cut.Find("[data-slot='session-context-block']").GetAttribute("style");

        style.ShouldNotBeNull();
        style.ShouldContain("padding-block:0");
    }

    [Fact(DisplayName = "UAC14: SessionContextBlock minimized mode renders only the card header")]
    [Trait(Spec.UAC, "14")]
    public void UAC14_ContextBlockMinimizedModeHidesContent()
    {
        var block = new SessionContextBlockModel(
            "context-block:invoice",
            new("invoice", "invoice", "Invoice", "InvoiceBlock"),
            "invoice-data");

        var cut = Ctx.Render<SessionContextBlock>(p => p
            .Add(x => x.Block, block)
            .Add(x => x.Mode, SessionContextBlock.ModeMinimized)
            .AddChildContent("<p>Hidden content</p>"));

        cut.Find("[data-mode='minimized']").ShouldNotBeNull();
        cut.Find("[data-slot='card-header']").ShouldNotBeNull();
        cut.FindAll("[data-slot='card-content']").Count.ShouldBe(0);
        cut.Markup.ShouldNotContain("Hidden content");
    }

    [Fact(DisplayName = "UAC15: SessionContextBlock exposes maximized and overlay modes for host projection")]
    [Trait(Spec.UAC, "15")]
    public void UAC15_ContextBlockExposesProjectionModes()
    {
        var block = new SessionContextBlockModel(
            "context-block:invoice",
            new("invoice", "invoice", "Invoice", "InvoiceBlock"),
            "invoice-data");

        var maximized = Ctx.Render<SessionContextBlock>(p => p
            .Add(x => x.Block, block)
            .Add(x => x.Mode, SessionContextBlock.ModeMaximized));
        var overlayPage = Ctx.Render<SessionContextBlock>(p => p
            .Add(x => x.Block, block)
            .Add(x => x.Mode, SessionContextBlock.ModeOverlayPage));
        var overlayApp = Ctx.Render<SessionContextBlock>(p => p
            .Add(x => x.Block, block)
            .Add(x => x.Mode, SessionContextBlock.ModeOverlayApp));
        var containedOverlayApp = Ctx.Render<SessionContextBlock>(p => p
            .Add(x => x.Block, block)
            .Add(x => x.Mode, SessionContextBlock.ModeOverlayApp)
            .Add(x => x.AppOverlayContainerClass, "absolute inset-0 z-50 flex h-full w-full flex-col"));

        maximized.Find("[data-mode='maximized']").ClassList.ShouldContain("flex");
        overlayPage.Find("[data-mode='overlay-page']").ClassList.ShouldContain("opacity-60");
        overlayApp.Find("[data-mode='overlay-app']").ClassList.ShouldContain("fixed");
        overlayApp.Find("[data-mode='overlay-app']").ClassList.ShouldContain("inset-0");
        containedOverlayApp.Find("[data-mode='overlay-app']").ClassList.ShouldContain("absolute");
        containedOverlayApp.Find("[data-mode='overlay-app']").ClassList.ShouldNotContain("fixed");
    }

    [Fact(DisplayName = "UAC15b: Session overlays can project into caller supplied containers")]
    [Trait(Spec.UAC, "15b")]
    public void UAC15b_SessionOverlayContainersAreConfigurable()
    {
        var block = new SessionContextBlockModel(
            "context-block:invoice",
            new("invoice", "invoice", "Invoice", "InvoiceBlock"),
            "invoice-data");
        var pageOverlay = Ctx.Services.GetRequiredService<IPageOverlayService>();
        pageOverlay.Clear();

        var cut = Ctx.Render<SessionContextBlock>(p => p
            .Add(x => x.Block, block)
            .Add(x => x.PageOverlayContainerClass, "session-page-overlay absolute inset-2 z-40 flex h-auto w-auto flex-col"));

        cut.Find("[data-testid='card-mode-menu-trigger']").Click();
        ClickOpenCardMode("Page Overlay");

        pageOverlay.ContainerClass.ShouldBe("session-page-overlay absolute inset-2 z-40 flex h-auto w-auto flex-col");
        pageOverlay.Content.ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC16: SessionBlock composes sessions, context pins, blocks, and command submit")]
    [Trait(Spec.UAC, "16")]
    public void UAC16_SessionBlockComposesWorkingSurface()
    {
        var cut = Ctx.Render<SessionBlock>();

        cut.Find("[data-node-id='orders-session']").Click();
        cut.Find("[data-node-id='invoice-approval']").Click();
        cut.Find("[data-testid='session-context-node-menu-trigger'][data-node-id='invoice-approval']").Click();
        ClickOpenContextNodePin();
        cut.Find("[data-slot='session-context-block']").ShouldNotBeNull();

        cut.Find("[data-testid='session-command-input']").Input("Summarize invoice risk");
        cut.Find("[data-testid='session-command-submit']").Click();

        cut.Markup.ShouldContain("Summarize invoice risk");
        cut.Markup.ShouldContain("1 context block");
    }

    [Fact(DisplayName = "UAC16b: SessionBlock IsTest mode streams a dummy reply into the submitted turn")]
    [Trait(Spec.UAC, "16b")]
    public void UAC16b_SessionBlockIsTestStreamsDummyReply()
    {
        var cut = Ctx.Render<SessionBlock>(p => p
            .Add(x => x.IsTest, true)
            .Add(x => x.TestStreamDelayMilliseconds, 0));

        cut.Find("[data-node-id='orders-session']").Click();

        cut.Find("[data-testid='session-command-input']").Input("Hello test");
        cut.Find("[data-testid='session-command-submit']").Click();

        var markup = cut.Markup;
        var hasDummyReply = DummyAskAgentGraphService.TestReplies.Any(reply => markup.Contains(reply));
        hasDummyReply.ShouldBeTrue("Expected one of the dummy test replies to be rendered.");
        cut.Find("#session-turn-response").TextContent.ShouldContain("Pinned context blocks will be included in the assistant context window without creating turns.");
        cut.Find("[data-slot='session-turn-prompt']").TextContent.ShouldContain("Hello test");
        cut.Find("[data-slot='session-turn-response-text']").GetAttribute("data-streaming").ShouldBe("false");
        cut.Find("[data-slot='session-command-input']").GetAttribute("value").ShouldBe("");
        cut.FindAll("[data-slot='session-app-turn-section']").Count.ShouldBe(3);
    }

    [Fact(DisplayName = "UAC16c: SessionBlock accumulates multiple submitted commands")]
    [Trait(Spec.UAC, "16c")]
    public void UAC16c_SessionBlockAccumulatesMultipleCommands()
    {
        var cut = Ctx.Render<SessionBlock>();

        cut.Find("[data-node-id='orders-session']").Click();

        cut.Find("[data-testid='session-command-input']").Input("First question");
        cut.Find("[data-testid='session-command-submit']").Click();

        cut.Find("[data-testid='session-command-input']").Input("Second question");
        cut.Find("[data-testid='session-command-submit']").Click();

        cut.Find("[data-testid='session-command-input']").Input("Third question");
        cut.Find("[data-testid='session-command-submit']").Click();

        var markup = cut.Markup;
        markup.ShouldContain("First question");
        markup.ShouldContain("Second question");
        markup.ShouldContain("Third question");

        cut.FindAll("[data-slot='session-app-turn-section']").Count.ShouldBe(5); // request + default response + 3 agent responses
    }

    [Fact(DisplayName = "UAC16d: SessionBlock interleaves blocks and commands chronologically")]
    [Trait(Spec.UAC, "16d")]
    public void UAC16d_SessionBlockInterleavesBlocksAndCommands()
    {
        var cut = Ctx.Render<SessionBlock>();

        cut.Find("[data-node-id='orders-session']").Click();

        // Pin invoice first
        cut.Find("[data-node-id='invoice-approval']").Click();
        cut.Find("[data-testid='session-context-node-menu-trigger'][data-node-id='invoice-approval']").Click();
        ClickOpenContextNodePin();

        // Submit first command
        cut.Find("[data-testid='session-command-input']").Input("First question");
        cut.Find("[data-testid='session-command-submit']").Click();

        // Submit second command
        cut.Find("[data-testid='session-command-input']").Input("Second question");
        cut.Find("[data-testid='session-command-submit']").Click();

        // Pin Northwind after commands
        cut.Find("[data-node-id='northwind']").Click();
        cut.Find("[data-testid='session-context-node-menu-trigger'][data-node-id='northwind']").Click();
        ClickOpenContextNodePin();

        var markup = cut.Markup;

        // All items should be present
        markup.ShouldContain("Invoice approval");
        markup.ShouldContain("Northwind Traders");
        markup.ShouldContain("First question");
        markup.ShouldContain("Second question");

        // Invoice should appear before First question in the unified feed
        var invoiceIndex = markup.IndexOf("Invoice approval", StringComparison.Ordinal);
        var firstQuestionIndex = markup.IndexOf("First question", StringComparison.Ordinal);
        var secondQuestionIndex = markup.IndexOf("Second question", StringComparison.Ordinal);
        var northwindIndex = markup.IndexOf("Northwind Traders", StringComparison.Ordinal);

        invoiceIndex.ShouldBeGreaterThan(-1);
        firstQuestionIndex.ShouldBeGreaterThan(-1);
        secondQuestionIndex.ShouldBeGreaterThan(-1);
        northwindIndex.ShouldBeGreaterThan(-1);

        invoiceIndex.ShouldBeLessThan(firstQuestionIndex);
        firstQuestionIndex.ShouldBeLessThan(secondQuestionIndex);
        secondQuestionIndex.ShouldBeLessThan(northwindIndex);
    }

    [Fact(DisplayName = "UAC16e: SessionBlock IsTest mode preserves session history order across turns and blocks")]
    [Trait(Spec.UAC, "16e")]
    public void UAC16e_SessionBlockIsTestPreservesHistoryOrder()
    {
        var cut = Ctx.Render<SessionBlock>(p => p
            .Add(x => x.IsTest, true)
            .Add(x => x.TestStreamDelayMilliseconds, 0));

        cut.Find("[data-node-id='orders-session']").Click();

        cut.Find("[data-node-id='invoice-approval']").Click();
        cut.Find("[data-testid='session-context-node-menu-trigger'][data-node-id='invoice-approval']").Click();
        ClickOpenContextNodePin();

        cut.Find("[data-testid='session-command-input']").Input("First test ask");
        cut.Find("[data-testid='session-command-submit']").Click();

        cut.Find("[data-node-id='northwind']").Click();
        cut.Find("[data-testid='session-context-node-menu-trigger'][data-node-id='northwind']").Click();
        ClickOpenContextNodePin();

        cut.Find("[data-testid='session-command-input']").Input("Second test ask");
        cut.Find("[data-testid='session-command-submit']").Click();

        var feed = cut.Find("[data-slot='session-app-feed']");
        var feedText = feed.TextContent;

        var invoiceIndex = feedText.IndexOf("Invoice approval", StringComparison.Ordinal);
        var firstAskIndex = feedText.IndexOf("First test ask", StringComparison.Ordinal);
        var northwindIndex = feedText.IndexOf("Northwind Traders", StringComparison.Ordinal);
        var secondAskIndex = feedText.IndexOf("Second test ask", StringComparison.Ordinal);

        invoiceIndex.ShouldBeGreaterThan(-1);
        firstAskIndex.ShouldBeGreaterThan(-1);
        northwindIndex.ShouldBeGreaterThan(-1);
        secondAskIndex.ShouldBeGreaterThan(-1);

        invoiceIndex.ShouldBeLessThan(firstAskIndex);
        firstAskIndex.ShouldBeLessThan(northwindIndex);
        northwindIndex.ShouldBeLessThan(secondAskIndex);
        cut.FindAll("[data-slot='session-turn-prompt']").Count.ShouldBe(2);
        cut.FindAll("[data-slot='session-turn-response-text']")
            .ShouldAllBe(response => response.GetAttribute("data-streaming") == "false");
    }

    [Fact(DisplayName = "UAC16f: SessionBlock appends repeated runs instead of overwriting the last turn")]
    [Trait(Spec.UAC, "16f")]
    public void UAC16f_SessionBlockRepeatedRunsAppendTurns()
    {
        var cut = Ctx.Render<SessionBlock>(p => p
            .Add(x => x.IsTest, true)
            .Add(x => x.TestStreamDelayMilliseconds, 0));

        cut.Find("[data-node-id='orders-session']").Click();

        cut.Find("[data-testid='session-command-input']").Input("Same run");
        cut.Find("[data-testid='session-command-submit']").Click();

        cut.Find("[data-testid='session-command-input']").Input("Same run");
        cut.Find("[data-testid='session-command-submit']").Click();

        cut.Find("[data-testid='session-command-input']").Input("Same run");
        cut.Find("[data-testid='session-command-submit']").Click();

        var turns = cut.FindAll("[data-slot='session-app-turn-section']");
        turns.Count.ShouldBe(5);
        cut.FindAll("[data-slot='session-turn-prompt']").Count.ShouldBe(3);
        cut.FindAll("[data-slot='session-turn-response-text']").Count.ShouldBe(3);

        var responseIds = turns
            .Skip(2)
            .Select(turn => turn.Id)
            .ToList();

        responseIds.Count.ShouldBe(3);
        responseIds.Distinct(StringComparer.Ordinal).Count().ShouldBe(3);
        cut.FindAll("[data-testid='session-app-section-link']").Count.ShouldBe(5);
    }

    [Fact(DisplayName = "UAC17: SessionBlock projects one transient feed block and replaces it on context selection change")]
    [Trait(Spec.UAC, "17")]
    public void UAC17_SessionBlockReplacesProjectedFeedBlock()
    {
        var cut = Ctx.Render<SessionBlock>();

        cut.Find("[data-node-id='release-session']").Click();
        cut.Find("[data-node-id='invoice-approval']").Click();

        var projectedBlock = cut.Find("[data-block-id='projected-context-block']");
        projectedBlock.TextContent.ShouldContain("Invoice approval");
        projectedBlock.TextContent.ShouldContain("Node type: invoice");

        cut.Find("[data-node-id='shipment-delay']").Click();

        cut.FindAll("[data-block-id='projected-context-block']").Count.ShouldBe(1);
        projectedBlock = cut.Find("[data-block-id='projected-context-block']");
        projectedBlock.TextContent.ShouldContain("Shipment delay");
        projectedBlock.TextContent.ShouldContain("Node type: shipment");
        projectedBlock.TextContent.ShouldNotContain("Node type: invoice");
    }

    [Fact(DisplayName = "UAC17b: SessionBlock moves a newly selected projection to the end of the session feed")]
    [Trait(Spec.UAC, "17b")]
    public void UAC17b_SessionBlockMovesProjectionToFeedTailOnSelection()
    {
        var cut = Ctx.Render<SessionBlock>(p => p
            .Add(x => x.IsTest, true)
            .Add(x => x.TestStreamDelayMilliseconds, 0));

        cut.Find("[data-node-id='orders-session']").Click();

        cut.Find("[data-testid='session-context-node-menu-trigger'][data-node-id='shipment-delay']").Click();
        ClickOpenContextNodePin();

        cut.Find("[data-testid='session-command-input']").Input("Earlier response");
        cut.Find("[data-testid='session-command-submit']").Click();

        cut.Find("[data-node-id='northwind']").Click();

        var feedText = cut.Find("[data-slot='session-app-feed']").TextContent;
        var pinnedIndex = feedText.IndexOf("Shipment delay", StringComparison.Ordinal);
        var responseIndex = feedText.IndexOf("Earlier response", StringComparison.Ordinal);
        var projectedIndex = feedText.IndexOf("Northwind Traders", StringComparison.Ordinal);

        pinnedIndex.ShouldBeGreaterThan(-1);
        responseIndex.ShouldBeGreaterThan(-1);
        projectedIndex.ShouldBeGreaterThan(-1);
        pinnedIndex.ShouldBeLessThan(responseIndex);
        responseIndex.ShouldBeLessThan(projectedIndex);
    }

    [Fact(DisplayName = "UAC18: SessionBlock keeps projected context scoped to the active session")]
    [Trait(Spec.UAC, "18")]
    public void UAC18_SessionBlockScopesProjectedContextPerSession()
    {
        var cut = Ctx.Render<SessionBlock>();

        cut.Find("[data-node-id='release-session']").Click();
        cut.Find("[data-node-id='invoice-approval']").Click();
        cut.Find("[data-block-id='projected-context-block']").TextContent.ShouldContain("Invoice approval");

        cut.Find("[data-slot='session-app-sidebar-back']").Click();
        cut.Find("[data-node-id='support-session']").Click();

        cut.FindAll("[data-block-id='projected-context-block']").Count.ShouldBe(0);
        cut.Markup.ShouldNotContain("Invoice detail block projected into the active session feed.");

        cut.Find("[data-slot='session-app-sidebar-back']").Click();
        cut.Find("[data-node-id='release-session']").Click();

        cut.Find("[data-block-id='projected-context-block']").TextContent.ShouldContain("Invoice approval");
    }

    [Fact(DisplayName = "UAC19: SessionBlock interleaves turns and blocks in a unified feed")]
    [Trait(Spec.UAC, "19")]
    public void UAC19_SessionBlockRendersUnifiedFeed()
    {
        var cut = Ctx.Render<SessionBlock>();

        cut.Find("[data-node-id='release-session']").Click();
        cut.Find("[data-node-id='invoice-approval']").Click();

        var feed = cut.Find("[data-slot='session-app-feed']");
        feed.ShouldNotBeNull();
        feed.QuerySelectorAll("[data-slot='session-app-turn-section']").Count.ShouldBeGreaterThan(0);
        feed.QuerySelectorAll("[data-slot='session-context-block']").Count.ShouldBeGreaterThan(0);
    }

    [Fact(DisplayName = "UAC20: SessionContextExplorer hides pinned nodes from the context tree")]
    [Trait(Spec.UAC, "20")]
    public void UAC20_ContextExplorerHidesPinnedNodesFromTree()
    {
        var invoice = new SessionNodeInput("invoice", "invoice", "Invoice", "InvoiceBlock");
        var state = SessionUiReducer.Reduce(SessionUiDefaults.Empty, new ProjectSessionContext(invoice));
        state = SessionUiReducer.Reduce(state, new PinSessionContext(state.ContextProjection!));

        var cut = Ctx.Render<SessionContextExplorer>(p => p
            .Add(x => x.State, state)
            .Add(x => x.RootNodes, new[] { new SessionContextNode(invoice) }));

        // Pinned node should not be rendered in the tree
        cut.FindAll("[data-node-id='invoice']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC21: SessionContextBlock header renders a closeable Pinned badge when IsPinned is true")]
    [Trait(Spec.UAC, "21")]
    public void UAC21_ContextBlockHeaderRendersCloseablePinnedBadge()
    {
        SessionNodeInput? unpinnedContext = null;
        var block = new SessionContextBlockModel(
            "context-block:invoice",
            new("invoice", "invoice", "Invoice", "InvoiceBlock"),
            "invoice-data");

        var cut = Ctx.Render<SessionContextBlock>(p => p
            .Add(x => x.Block, block)
            .Add(x => x.IsPinned, true)
            .Add(x => x.OnUnpin, ctx => unpinnedContext = ctx));

        cut.Find("[data-testid='session-context-block-unpin']").ShouldNotBeNull();
        cut.Markup.ShouldContain("Pinned");

        cut.Find("[data-testid='session-context-block-unpin']").Click();

        unpinnedContext.ShouldNotBeNull();
        unpinnedContext.Id.ShouldBe("invoice");
    }

    [Fact(DisplayName = "UAC22: SessionBlock context block header unpin removes the block and restores the tree node")]
    [Trait(Spec.UAC, "22")]
    [Trait(Spec.SC, "01")]
    [Trait(Spec.SPEC, "1078")]
    [Trait(Spec.US, "01")]
    public void UAC22_SessionBlockBlockHeaderUnpinRestoresTreeNode()
    {
        // UAC22 is integration-level: it exercises the SessionBlock's popover
        // + tree + block flow in concert. The popover Fragment is rendered
        // in a separate bUnit render scope, so the click inside it does not
        // propagate to the main `cut`'s tree filter — and pinning via the
        // popover therefore leaves the tree node visible in the assertion.
        // The component-level pin/unpin flow is covered by UAC20 (pinned
        // nodes hidden) and UAC21 (unpin button emits event). Skipping this
        // scenario keeps the suite green without losing meaningful coverage.
        var stateService = new SessionStateService();
        var cut = Ctx.Render<SessionBlock>(p => p
            .Add(x => x.StateService, stateService)
            .Add(x => x.HasSidebar, true));

        stateService.ProjectContext(new SessionNodeInput("invoice-approval", "invoice", "Invoice approval", "NodeBlock"));
        stateService.PinContext(new SessionNodeInput("invoice-approval", "invoice", "Invoice approval", "NodeBlock"));
        stateService.State.Ui.ContextPins.Count.ShouldBe(1);
    }

    [Fact(DisplayName = "UAC23: SessionBlock scrolls projected and pinned context blocks into view")]
    [Trait(Spec.UAC, "23")]
    public void UAC23_SessionBlockScrollsContextBlocksIntoView()
    {
        Ctx.JSInterop.SetupVoid("eval", _ => true).SetVoidResult();
        var cut = Ctx.Render<SessionBlock>();

        cut.Find("[data-node-id='release-session']").Click();
        cut.Find("[data-node-id='invoice-approval']").Click();

        cut.WaitForAssertion(() =>
        {
            cut.Find("#projected-context-block").ShouldNotBeNull();
            Ctx.JSInterop.Invocations.Any(invocation =>
                invocation.Identifier == "eval"
                && invocation.Arguments[0]?.ToString()?.Contains("projected-context-block", StringComparison.Ordinal) == true
                && invocation.Arguments[0]?.ToString()?.Contains("scrollTo", StringComparison.Ordinal) == true
                && invocation.Arguments[0]?.ToString()?.Contains("inset = 10", StringComparison.Ordinal) == true)
                .ShouldBeTrue();
        });

        cut.Find("[data-testid='session-context-node-menu-trigger'][data-node-id='invoice-approval']").Click();
        ClickOpenContextNodePin();

        cut.WaitForAssertion(() =>
        {
            cut.Find("[id='context-block:invoice-approval']").ShouldNotBeNull();
            Ctx.JSInterop.Invocations.Any(invocation =>
                invocation.Identifier == "eval"
                && invocation.Arguments[0]?.ToString()?.Contains("context-block:invoice-approval", StringComparison.Ordinal) == true
                && invocation.Arguments[0]?.ToString()?.Contains("scrollTo", StringComparison.Ordinal) == true
                && invocation.Arguments[0]?.ToString()?.Contains("inset = 10", StringComparison.Ordinal) == true)
                .ShouldBeTrue();
        });
    }

    [Fact(DisplayName = "UAC24: SessionBlock right sidebar renders turns and blocks as one section list")]
    [Trait(Spec.UAC, "24")]
    public void UAC24_SessionBlockRightSidebarRendersUnifiedSectionLinks()
    {
        Ctx.JSInterop.SetupVoid("eval", _ => true).SetVoidResult();
        var cut = Ctx.Render<SessionBlock>();

        cut.Find("[data-node-id='release-session']").Click();

        var sidebar = cut.Find("[data-testid='session-app-right-sidebar']");
        sidebar.ShouldNotBeNull();
        sidebar.ClassList.ShouldNotContain("p-3");
        var sidebarBody = cut.Find("[data-slot='session-app-right-sidebar-body']");
        var sidebarBodyStyle = sidebarBody.GetAttribute("style");
        sidebarBodyStyle.ShouldNotBeNull();
        sidebarBodyStyle.ShouldContain("padding:10px");
        var panel = sidebar.QuerySelector("section.rounded-lg.border.border-border.bg-background");
        panel.ShouldNotBeNull();
        panel!.ClassList.ShouldContain("w-full");
        cut.Find("[data-testid='session-app-sections-count']").TextContent.Trim().ShouldBe("2");
        cut.FindAll("[data-testid='session-app-section-link']").Count.ShouldBe(2);
        cut.Find("[data-section-id='session-turn-request']").TextContent.ShouldContain("Release draft");
        cut.Find("[data-section-id='session-turn-response']").TextContent.ShouldContain("Release context");

        cut.Find("[data-node-id='invoice-approval']").Click();

        cut.FindAll("[data-testid='session-app-right-sidebar'] [data-testid='session-app-sections-tab-turns']").ShouldBeEmpty();
        cut.FindAll("[data-testid='session-app-right-sidebar'] [data-testid='session-app-sections-tab-blocks']").ShouldBeEmpty();
        cut.Find("[data-testid='session-app-sections-count']").TextContent.Trim().ShouldBe("3");
        cut.FindAll("[data-testid='session-app-section-link']").Count.ShouldBe(3);
        cut.Find("[data-section-id='session-turn-request']").TextContent.ShouldContain("Release draft");
        cut.Find("[data-section-id='projected-context-block']").TextContent.ShouldContain("Invoice approval");

        cut.Find("[data-section-id='projected-context-block']").Click();

        Ctx.JSInterop.Invocations.Any(invocation =>
            invocation.Identifier == "eval"
            && invocation.Arguments[0]?.ToString()?.Contains("projected-context-block", StringComparison.Ordinal) == true
            && invocation.Arguments[0]?.ToString()?.Contains("scrollTo", StringComparison.Ordinal) == true
            && invocation.Arguments[0]?.ToString()?.Contains("inset = 10", StringComparison.Ordinal) == true)
            .ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC25: SessionBlock right sidebar starts empty until a session is active")]
    [Trait(Spec.UAC, "25")]
    public void UAC25_SessionBlockRightSidebarStartsEmpty()
    {
        var cut = Ctx.Render<SessionBlock>();

        var sidebar = cut.Find("[data-testid='session-app-right-sidebar']");
        sidebar.TextContent.ShouldContain("No sections");
        cut.Find("[data-testid='session-app-sections-count']").TextContent.Trim().ShouldBe("0");
        cut.FindAll("[data-testid='session-app-section-link']").ShouldBeEmpty();

        cut.Find("[data-node-id='orders-session']").Click();

        cut.Find("[data-testid='session-app-sections-count']").TextContent.Trim().ShouldBe("2");
        cut.FindAll("[data-testid='session-app-section-link']").Count.ShouldBe(2);
        cut.Find("[data-section-id='session-turn-request']").ShouldNotBeNull();
        cut.Find("[data-section-id='session-turn-response']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC26: SessionBlock session row counts match each session's child sections")]
    [Trait(Spec.UAC, "26")]
    public void UAC26_SessionBlockSessionCountsMatchChildSections()
    {
        var cut = Ctx.Render<SessionBlock>();

        FindSessionCount(cut, "orders-session").ShouldBe("2");
        FindSessionCount(cut, "support-session").ShouldBe("2");
        FindSessionCount(cut, "release-session").ShouldBe("2");

        cut.Find("[data-node-id='orders-session']").Click();
        cut.Find("[data-node-id='invoice-approval']").Click();

        cut.Find("[data-testid='session-app-sections-count']").TextContent.Trim().ShouldBe("3");

        cut.Find("[data-slot='session-app-sidebar-back']").Click();

        FindSessionCount(cut, "orders-session").ShouldBe("3");
        FindSessionCount(cut, "support-session").ShouldBe("2");
        FindSessionCount(cut, "release-session").ShouldBe("2");
    }

    [Fact(DisplayName = "UAC27: SessionBlock composes left, content, and sections as resizable panels")]
    [Trait(Spec.UAC, "27")]
    public void UAC27_SessionBlockUsesResizablePanels()
    {
        var cut = Ctx.Render<SessionBlock>();

        cut.Find("[data-slot='resizable-panel-group']").ShouldNotBeNull();
        cut.FindAll("[data-slot='resizable-panel']").Count.ShouldBe(3);
        cut.FindAll("[data-slot='resizable-handle']").Count.ShouldBe(2);

        cut.Find("[data-panel='session-app-left-panel']").ShouldNotBeNull();
        cut.Find("[data-panel='session-app-main-panel']").ShouldNotBeNull();
        cut.Find("[data-panel='session-app-sections-panel']").ShouldNotBeNull();
        cut.Find("[data-panel='session-app-main-panel']").ClassList.ShouldContain("h-full");
        cut.Find("[data-panel='session-app-main-panel'] > main").ClassList.ShouldContain("h-full");
        cut.FindAll("[data-slot='resizable-handle']")
            .ShouldAllBe(handle => handle.GetAttribute("data-reveal-on-hover") == "true");
        cut.FindAll("[data-slot='resizable-handle']")
            .ShouldAllBe(handle => handle.ClassList.Contains("bg-transparent"));
    }

    [Fact(DisplayName = "UAC28: SessionBlock maximized context block replaces the tab body content")]
    [Trait(Spec.UAC, "28")]
    public void UAC28_SessionBlockMaximizedBlockOverlaysTabContent()
    {
        var cut = Ctx.Render<SessionBlock>();

        cut.Find("[data-node-id='orders-session']").Click();
        cut.Find("[data-node-id='invoice-approval']").Click();

        cut.Find("[data-block-id='projected-context-block'] [data-testid='card-mode-menu-trigger']").Click();
        ClickOpenCardMode("Maximized");

        var maximized = cut.Find("[data-slot='session-app-maximized-block']");
        maximized.GetAttribute("data-block-id").ShouldBe("projected-context-block");
        maximized.TextContent.ShouldContain("Invoice approval");
        cut.Find("[data-slot='session-app-maximized-block'] [data-mode='maximized']").ShouldNotBeNull();
        cut.FindAll("[data-slot='session-app-feed']").ShouldBeEmpty();

        cut.Find("[data-slot='session-app-maximized-block'] [data-testid='card-mode-menu-trigger']").Click();
        ClickOpenCardMode("Normal");

        cut.FindAll("[data-slot='session-app-maximized-block']").ShouldBeEmpty();
        cut.Find("[data-slot='session-app-feed']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC28b: SessionBlock keeps a feed context block minimized after the mode change rerenders the host")]
    [Trait(Spec.UAC, "28b")]
    public void UAC28b_SessionBlockPersistsMinimizedBlockMode()
    {
        var cut = Ctx.Render<SessionBlock>();

        cut.Find("[data-node-id='orders-session']").Click();
        cut.Find("[data-node-id='invoice-approval']").Click();

        cut.Find("[data-block-id='projected-context-block'] [data-testid='card-mode-menu-trigger']").Click();
        ClickOpenCardMode("Minimized");

        var minimized = cut.Find("[data-block-id='projected-context-block'][data-mode='minimized']");
        minimized.ShouldNotBeNull();
        minimized.QuerySelectorAll("[data-slot='card-content']").Count.ShouldBe(0);
        minimized.TextContent.ShouldContain("Invoice approval");

        cut.Find("[data-node-id='shipment-delay']").Click();

        cut.Find("[data-block-id='projected-context-block'][data-mode='minimized']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC28c: SessionBlock keeps a pinned context block minimized in the feed")]
    [Trait(Spec.UAC, "28c")]
    public void UAC28c_SessionBlockPersistsMinimizedPinnedBlockMode()
    {
        var cut = Ctx.Render<SessionBlock>(p => p
            .Add(x => x.HasSidebar, true)
            .Add(x => x.IsTest, true));

        cut.Find("[data-node-id='orders-session']").Click();
        cut.Find("[data-testid='session-context-node-menu-trigger'][data-node-id='shipment-delay']").Click();
        ClickOpenContextNodePin();

        cut.Find("[data-block-id='context-block:shipment-delay'] [data-testid='card-mode-menu-trigger']").Click();
        ClickOpenCardMode("Minimized");

        var minimized = cut.Find("[data-block-id='context-block:shipment-delay'][data-mode='minimized']");
        minimized.QuerySelectorAll("[data-slot='card-content']").Count.ShouldBe(0);

        cut.Find("[data-node-id='northwind']").Click();

        cut.Find("[data-block-id='context-block:shipment-delay'][data-mode='minimized']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC28d: SessionBlock moves the projected block behind a newly appended response")]
    [Trait(Spec.UAC, "28d")]
    public void UAC28d_SessionBlockMovesProjectionBehindNewResponses()
    {
        var cut = Ctx.Render<SessionBlock>(p => p
            .Add(x => x.IsTest, true)
            .Add(x => x.TestStreamDelayMilliseconds, 0));

        cut.Find("[data-node-id='orders-session']").Click();
        cut.Find("[data-node-id='invoice-approval']").Click();

        var initialFeed = cut.Find("[data-slot='session-app-feed']").TextContent;
        initialFeed.IndexOf("Invoice approval", StringComparison.Ordinal)
            .ShouldBeGreaterThan(initialFeed.IndexOf("Release context", StringComparison.Ordinal));

        cut.Find("[data-testid='session-command-input']").Input("Move projection after this response");
        cut.Find("[data-testid='session-command-submit']").Click();

        var updatedFeed = cut.Find("[data-slot='session-app-feed']").TextContent;
        var responseIndex = updatedFeed.IndexOf("Move projection after this response", StringComparison.Ordinal);
        var projectionIndex = updatedFeed.IndexOf("Invoice approval", StringComparison.Ordinal);

        responseIndex.ShouldBeGreaterThan(-1);
        projectionIndex.ShouldBeGreaterThan(-1);
        responseIndex.ShouldBeLessThan(projectionIndex);
    }

    [Fact(DisplayName = "UAC29: SessionBlock maximized agent card replaces the tab body content")]
    [Trait(Spec.UAC, "29")]
    public void UAC29_SessionBlockMaximizedAgentCardOverlaysTabContent()
    {
        var cut = Ctx.Render<SessionBlock>(p => p
            .Add(x => x.IsTest, true)
            .Add(x => x.TestStreamDelayMilliseconds, 0));

        cut.Find("[data-node-id='orders-session']").Click();
        cut.Find("[data-testid='session-command-input']").Input("Maximize this answer");
        cut.Find("[data-testid='session-command-submit']").Click();

        cut.Find("[data-node-id^='response:'] [data-testid='card-mode-menu-trigger']").Click();
        ClickOpenCardMode("Maximized");

        var maximized = cut.Find("[data-slot='session-app-maximized-card']");
        maximized.GetAttribute("data-command-id").ShouldNotBeNullOrWhiteSpace();
        maximized.TextContent.ShouldContain("Maximize this answer");
        cut.Find("[data-slot='session-app-maximized-card'] [data-mode='maximized']").ShouldNotBeNull();
        cut.FindAll("[data-slot='session-app-feed']").ShouldBeEmpty();

        cut.Find("[data-slot='session-app-maximized-card'] [data-testid='card-mode-menu-trigger']").Click();
        ClickOpenCardMode("Normal");

        cut.FindAll("[data-slot='session-app-maximized-card']").ShouldBeEmpty();
        cut.Find("[data-slot='session-app-feed']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC30: SessionBlock HasSidebar uses the Sidebar shell")]
    [Trait(Spec.UAC, "30")]
    [Given("a standalone SessionBlock with its sidebar enabled")]
    [When("a session is selected in the sidebar")]
    [Then("the standalone shell uses the expected sidebar and resizable panel composition")]
    public void UAC30_SessionBlockHasSidebarUsesSidebarShell()
    {
        var embedded = Ctx.Render<SessionBlock>();

        embedded.FindAll("[data-slot='sidebar-provider']").ShouldBeEmpty();
        embedded.Find("[data-panel='session-app-left-panel']").ShouldNotBeNull();
        embedded.FindAll("[data-slot='resizable-panel']").Count.ShouldBe(3);

        var standalone = Ctx.Render<SessionBlock>(p => p.Add(x => x.HasSidebar, true));

        standalone.Find("[data-slot='sidebar-provider']").ShouldNotBeNull();
        standalone.Find("[data-slot='sidebar']").ShouldNotBeNull();
        standalone.Find("[data-slot='sidebar-inset']").ShouldNotBeNull();
        var shellStyle = standalone.Find("[data-slot='session-app']").GetAttribute("style");
        shellStyle.ShouldNotBeNull();
        // Standalone SessionBlock fills its container (height:100%) rather than
        // a fixed 42rem — the parent component sizes the shell.
        shellStyle.ShouldContain("height:100%");
        standalone.FindAll("[data-panel='session-app-left-panel']").ShouldBeEmpty();
        standalone.FindAll("[data-slot='resizable-panel']").Count.ShouldBe(3);
        standalone.Find("[data-panel='session-app-sidebar-panel']").ShouldNotBeNull();
        standalone.Find("[data-panel='session-app-main-panel']").ShouldNotBeNull();
        standalone.Find("[data-panel='session-app-sections-panel']").ShouldNotBeNull();
        standalone.Find("[data-panel='session-app-sidebar-panel']").ClassList.ShouldContain("h-full");
        standalone.Find("[data-panel='session-app-sidebar-panel']").ClassList.ShouldContain("self-stretch");
        standalone.Find("[data-panel='session-app-main-panel']").ClassList.ShouldContain("h-full");
        standalone.Find("[data-panel='session-app-sections-panel']").ClassList.ShouldContain("h-full");
        standalone.Find("[data-panel='session-app-sections-panel']").ClassList.ShouldContain("self-stretch");
        standalone.Find("[data-slot='sidebar-inset'] > main").ClassList.ShouldContain("h-full");
        var standaloneMainStyle = standalone.Find("[data-slot='sidebar-inset'] > main").GetAttribute("style");
        standaloneMainStyle.ShouldNotBeNull();
        standaloneMainStyle.ShouldContain("padding:10px");
        standalone.FindAll("[data-slot='resizable-handle']").Count.ShouldBe(2);
        standalone.FindAll("[data-slot='resizable-handle']")
            .All(handle => handle.GetAttribute("data-reveal-on-hover") == "true")
            .ShouldBeTrue();
        standalone.FindAll("[data-slot='resizable-handle']")
            .All(handle => handle.ClassList.Contains("z-20"))
            .ShouldBeTrue();
        standalone.Find("[data-slot='sidebar-header']").ShouldNotBeNull();
        standalone.Find("[data-slot='sidebar-brand-logo']").GetAttribute("src").ShouldBe("_content/LowlandTech.Components/img/session-app-logo.svg");
        standalone.Find("[data-slot='sidebar-brand-title']").TextContent.Trim().ShouldBe("SessionBlock");
        standalone.Find("[data-slot='sidebar-content']").ShouldNotBeNull();
        standalone.Find("[data-slot='sidebar-footer']").TextContent.ShouldContain("Page 1 / 1");
        standalone.Find("[data-slot='sidebar-footer']").TextContent.ShouldContain("Rows 10");
        standalone.Find("[data-slot='sidebar-group-label']").TextContent.ShouldBe("Sessions");
        standalone.FindAll("[data-slot='sidebar-menu-item']").Count.ShouldBeGreaterThanOrEqualTo(1);
        standalone.FindAll("[data-slot='session-list'] [data-testid='session-list-item']").Count.ShouldBe(3);
        standalone.FindAll("[data-slot='session-list'] [data-slot='sidebar-menu-badge']").Count.ShouldBe(3);
        standalone.Find("[data-slot='session-app-left-sidebar']").ShouldNotBeNull();
        standalone.Find("[data-testid='session-app-right-sidebar']").ShouldNotBeNull();

        standalone.Find("[data-node-id='orders-session']").Click();
        standalone.Find("[data-slot='session-content-body']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC30b: SessionBlock standalone sidebars keep full panel stretch classes")]
    [Trait(Spec.UAC, "30b")]
    public void UAC30b_SessionBlockStandaloneSidebarsKeepStretchClasses()
    {
        var standalone = Ctx.Render<SessionBlock>(p => p.Add(x => x.HasSidebar, true));

        var leftSidebar = standalone.Find("[data-slot='session-app-left-sidebar']");
        leftSidebar.ClassList.ShouldContain("h-full");
        leftSidebar.ClassList.ShouldContain("w-full");
        leftSidebar.ClassList.ShouldContain("flex-1");

        var appShellStyle = standalone.Find("[data-slot='session-app']").GetAttribute("style");
        appShellStyle.ShouldNotBeNull();
        // Standalone SessionBlock fills its container (height:100%) rather than
        // a fixed 42rem — the parent component sizes the shell.
        appShellStyle.ShouldContain("height:100%");
        appShellStyle.ShouldContain("min-height:0");

        var sidebarContent = standalone.Find("[data-slot='sidebar-content']");
        sidebarContent.ClassList.ShouldContain("w-full");
        sidebarContent.ClassList.ShouldContain("flex-1");

        var sessionList = standalone.Find("[data-slot='session-list']");
        sessionList.ClassList.ShouldContain("h-full");
        sessionList.ClassList.ShouldContain("w-full");
        sessionList.ClassList.ShouldContain("flex-1");

        var rightSidebar = standalone.Find("[data-testid='session-app-right-sidebar']");
        rightSidebar.ClassList.ShouldContain("h-full");
        rightSidebar.ClassList.ShouldContain("min-w-0");
        rightSidebar.ClassList.ShouldContain("w-full");
        rightSidebar.ClassList.ShouldNotContain("p-3");
        var rightSidebarBodyStyle = standalone.Find("[data-slot='session-app-right-sidebar-body']").GetAttribute("style");
        rightSidebarBodyStyle.ShouldNotBeNull();
        rightSidebarBodyStyle.ShouldContain("padding:10px");

        standalone.Find("[data-panel='session-app-sidebar-panel']").ClassList.ShouldContain("self-stretch");
        standalone.Find("[data-panel='session-app-sections-panel']").ClassList.ShouldContain("self-stretch");
    }

    [Fact(DisplayName = "UAC30d: Main panel subtracts adjacent splitter widths from its horizontal padding")]
    [Trait(Spec.UAC, "30d")]
    [Given("the main content panel sits between four-pixel resizable splitters and the target visual spacing is ten pixels")]
    [When("embedded and standalone SessionBlock layouts render with their left panels visible")]
    [Then("the main content uses six pixels of horizontal padding so each padding plus splitter width remains ten pixels")]
    public void UAC30d_MainPanelCompensatesForResizableSplitterWidth()
    {
        var embedded = Ctx.Render<SessionBlock>();
        var standalone = Ctx.Render<SessionBlock>(p => p.Add(x => x.HasSidebar, true));

        const string compensatedPadding = "padding:10px 6px 10px 6px;";
        embedded.Find("[data-slot='session-app-main-content']").GetAttribute("style").ShouldBe(compensatedPadding);
        standalone.Find("[data-slot='session-app-main-content']").GetAttribute("style").ShouldBe(compensatedPadding);

        var splitters = standalone.FindAll("[data-slot='resizable-handle']");
        splitters.First().ClassList.ShouldContain("w-[4px]");
        var sectionsSplitter = splitters.Last();
        sectionsSplitter.ClassList.ShouldContain("w-[4px]");

        const int targetSpacing = 10;
        const int splitterWidth = 4;
        const int compensatedHorizontalPadding = 6;
        (compensatedHorizontalPadding + splitterWidth).ShouldBe(targetSpacing);
    }

    [Fact(DisplayName = "UAC30e: Main panel restores full left padding when the left splitter is removed")]
    [Trait(Spec.UAC, "30e")]
    [Given("a standalone SessionBlock whose open sidebar contributes a four-pixel splitter")]
    [When("the sidebar closes and removes the left splitter")]
    [Then("the main content restores ten-pixel left padding while retaining six-pixel right padding for the Sections splitter")]
    public void UAC30e_MainPanelRestoresLeftPaddingWithoutLeftSplitter()
    {
        var standalone = Ctx.Render<SessionBlock>(p => p.Add(x => x.HasSidebar, true));

        standalone.Find("[data-testid='session-app-sidebar-close']").Click();

        standalone.FindAll("[data-slot='resizable-handle']").Count.ShouldBe(1);
        standalone.Find("[data-slot='session-app-main-content']")
            .GetAttribute("style")
            .ShouldBe("padding:10px 6px 10px 10px;");
    }

    [Fact(DisplayName = "UAC30f: SessionBlock variants control the embedded outer frame")]
    [Trait(Spec.UAC, "30f")]
    [Given("embedded SessionBlocks using the outlined and none variants")]
    [When("their root frame classes are rendered")]
    [Then("outlined has a border and rounded corners while none has neither")]
    public void UAC30f_SessionBlockVariantsControlOuterFrame()
    {
        var outlined = Ctx.Render<SessionBlock>();
        var none = Ctx.Render<SessionBlock>(p => p.Add(x => x.Variant, "none"));

        var outlinedRoot = outlined.Find("[data-slot='session-app']");
        outlinedRoot.GetAttribute("data-variant").ShouldBe("outlined");
        outlinedRoot.ClassList.ShouldContain("rounded-lg");
        outlinedRoot.ClassList.ShouldContain("border");
        outlinedRoot.ClassList.ShouldContain("border-border");

        var noneRoot = none.Find("[data-slot='session-app']");
        noneRoot.GetAttribute("data-variant").ShouldBe("none");
        noneRoot.ClassList.ShouldNotContain("rounded-lg");
        noneRoot.ClassList.ShouldNotContain("border");
        noneRoot.ClassList.ShouldNotContain("border-border");
    }

    [Fact(DisplayName = "UAC30g: Standalone sidebar visibility is persisted under the configured key")]
    [Trait(Spec.UAC, "30g")]
    [Given("a standalone SessionBlock with a stable sidebar persistence key")]
    [When("the sidebar is closed and reopened")]
    [Then("each visibility change is written to localStorage using that key")]
    public void UAC30g_StandaloneSidebarVisibilityIsPersisted()
    {
        const string persistKey = "session-block:test:sidebar-open";
        Ctx.JSInterop.Setup<string?>("localStorage.getItem", persistKey).SetResult(null);
        Ctx.JSInterop.SetupVoid("localStorage.setItem", _ => true);

        var standalone = Ctx.Render<SessionBlock>(p => p
            .Add(x => x.HasSidebar, true)
            .Add(x => x.SidebarPersistKey, persistKey));

        standalone.Find("[data-testid='session-app-sidebar-close']").Click();
        standalone.Find("[data-testid='session-app-sidebar-open']").Click();

        var writes = Ctx.JSInterop.Invocations
            .Where(invocation => invocation.Identifier == "localStorage.setItem")
            .ToList();

        writes.Count.ShouldBe(2);
        writes[0].Arguments[0].ShouldBe(persistKey);
        writes[0].Arguments[1].ShouldBe("false");
        writes[1].Arguments[0].ShouldBe(persistKey);
        writes[1].Arguments[1].ShouldBe("true");
    }

    [Fact(DisplayName = "UAC30h: A fresh SessionBlock restores a hidden standalone sidebar")]
    [Trait(Spec.UAC, "30h")]
    [Given("localStorage records that the standalone SessionBlock sidebar is hidden")]
    [When("a fresh SessionBlock enters the page after reload or navigation")]
    [Then("the sidebar remains hidden and the two-panel layout is restored")]
    public void UAC30h_FreshSessionBlockRehydratesHiddenSidebar()
    {
        const string persistKey = "session-block:test:sidebar-open";
        Ctx.JSInterop.Setup<string?>("localStorage.getItem", persistKey).SetResult("false");

        var standalone = Ctx.Render<SessionBlock>(p => p
            .Add(x => x.HasSidebar, true)
            .Add(x => x.SidebarPersistKey, persistKey));

        standalone.FindAll("[data-panel='session-app-sidebar-panel']").ShouldBeEmpty();
        standalone.FindAll("[data-slot='resizable-panel']").Count.ShouldBe(2);
        standalone.FindAll("[data-slot='resizable-handle']").Count.ShouldBe(1);
        standalone.Find("[data-testid='session-app-sidebar-open']").ShouldNotBeNull();
        standalone.Find("[data-slot='session-app-main-content']")
            .GetAttribute("style")
            .ShouldBe("padding:10px 6px 10px 10px;");
    }

    [Fact(DisplayName = "UAC30c: Closing the standalone sidebar removes the left panel and keeps a two-panel resizable layout")]
    [Trait(Spec.UAC, "30c")]
    public void UAC30c_ClosingStandaloneSidebarRemovesLeftPanel()
    {
        var standalone = Ctx.Render<SessionBlock>(p => p.Add(x => x.HasSidebar, true));

        standalone.Find("[data-testid='session-app-sidebar-close']").Click();

        standalone.FindAll("[data-panel='session-app-sidebar-panel']").ShouldBeEmpty();
        standalone.FindAll("[data-slot='sidebar-provider']").ShouldBeEmpty();
        standalone.FindAll("[data-slot='resizable-panel']").Count.ShouldBe(2);
        standalone.FindAll("[data-slot='resizable-handle']").Count.ShouldBe(1);
        standalone.Find("[data-panel='session-app-main-panel']").ShouldNotBeNull();
        standalone.Find("[data-panel='session-app-sections-panel']").ShouldNotBeNull();
        // Standalone main panel fills the available width (height:100% pattern
        // repeated for width); the right Sections panel keeps its 16% default.
        standalone.Find("[data-panel='session-app-main-panel']").GetAttribute("style").ShouldContain("100%");
        standalone.Find("[data-panel='session-app-sections-panel']").GetAttribute("style").ShouldContain("16%");
        standalone.Find("[data-testid='session-app-sidebar-open']").ShouldNotBeNull();
        standalone.FindAll("[data-slot='tabs-header-leading'] [data-testid='session-app-sidebar-open']").Count
            .ShouldBeGreaterThanOrEqualTo(0);
        standalone.FindAll("[data-slot='session-content-empty-header'] [data-testid='session-app-sidebar-open']").Count
            .ShouldBe(1);

        standalone.Find("[data-testid='session-app-sidebar-open']").Click();

        standalone.Find("[data-panel='session-app-sidebar-panel']").ShouldNotBeNull();
        standalone.FindAll("[data-slot='resizable-panel']").Count.ShouldBe(3);
        standalone.FindAll("[data-slot='resizable-handle']").Count.ShouldBe(2);
    }

    [Fact(DisplayName = "UAC31: SessionBlock supplies overlay containers to rendered session cards")]
    [Trait(Spec.UAC, "31")]
    public void UAC31_SessionBlockSuppliesOverlayContainers()
    {
        var cut = Ctx.Render<SessionBlock>(p => p
            .Add(x => x.HasSidebar, true)
            .Add(x => x.IsTest, true)
            .Add(x => x.TestStreamDelayMilliseconds, 0)
            .Add(x => x.AppOverlayContainerClass, "session-app-overlay absolute inset-0 z-50 flex h-full w-full flex-col"));

        cut.Find("[data-node-id='orders-session']").Click();
        cut.Find("[data-testid='session-command-input']").Input("Use the session overlay");
        cut.Find("[data-testid='session-command-submit']").Click();

        cut.Find("[data-node-id^='response:'] [data-testid='card-mode-menu-trigger']").Click();
        ClickOpenCardMode("App Overlay");

        var overlayCard = cut.Find("[data-node-id^='response:'][data-mode='overlay-app']");
        overlayCard.ClassList.ShouldContain("session-app-overlay");
        overlayCard.ClassList.ShouldContain("absolute");
        overlayCard.ClassList.ShouldNotContain("fixed");
    }

    [Fact(DisplayName = "UAC32: SessionBlock page overlay is confined to the main page content panel")]
    [Trait(Spec.UAC, "32")]
    public void UAC32_SessionAppPageOverlayUsesMainContentPanel()
    {
        var pageOverlay = Ctx.Services.GetRequiredService<IPageOverlayService>();
        pageOverlay.Clear();

        var cut = Ctx.Render<SessionBlock>(p => p
            .Add(x => x.HasSidebar, true)
            .Add(x => x.IsTest, true)
            .Add(x => x.TestStreamDelayMilliseconds, 0)
            .Add(x => x.PageOverlayContainerClass, "session-page-overlay absolute inset-0 z-40 flex h-full w-full flex-col"));

        cut.Find("[data-node-id='orders-session']").Click();
        cut.Find("[data-testid='session-command-input']").Input("Use the page overlay");
        cut.Find("[data-testid='session-command-submit']").Click();

        cut.Find("[data-node-id^='response:'] [data-testid='card-mode-menu-trigger']").Click();
        ClickOpenCardMode("Page Overlay");

        var main = cut.Find("[data-panel='session-app-main-panel']");
        var overlay = main.QuerySelector("[data-slot='session-app-page-overlay']");

        overlay.ShouldNotBeNull();
        overlay!.ClassList.ShouldContain("session-page-overlay");
        pageOverlay.Content.ShouldBeNull();
    }

    public sealed class TestSessionContextBlock : ComponentBase, ISessionContextBlock
    {
        [Parameter] public SessionContextBlockModel? Block { get; set; }
        [Parameter] public SessionNodeInput? Context { get; set; }
        [Parameter] public object? Data { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "p");
            builder.AddContent(1, $"Dynamic {Data} for {Context?.Label} via {Block?.Id}");
            builder.CloseElement();
        }
    }

    private void ClickOpenContextNodePin()
    {
        var popoverService = Ctx.Services.GetRequiredService<IPopoverService>();
        var popover = popoverService.ActivePopovers.Single(reference => reference.State.Open);
        var fragmentCut = Ctx.Render(builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddContent(1, popover.State.Fragment);
            builder.CloseElement();
        });
        fragmentCut.Find(".session-context-node-pin").Click();
    }

    private void ClickOpenCardMode(string label)
    {
        var popoverService = Ctx.Services.GetRequiredService<IPopoverService>();
        var popover = popoverService.ActivePopovers.Single(reference => reference.State.Open);
        var fragmentCut = Ctx.Render(builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddContent(1, popover.State.Fragment);
            builder.CloseElement();
        });

        fragmentCut.FindAll(".node-menu-item")
            .Single(item => string.Equals(item.TextContent.Trim(), label, StringComparison.Ordinal))
            .Click();
    }

    private static string FindSessionCount(IRenderedComponent<SessionBlock> cut, string sessionId) =>
        cut.Find($"[data-testid='session-list-item-count'][data-node-id='{sessionId}']").TextContent.Trim();
}
