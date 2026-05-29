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

        cut.Find("[data-slot='session-card-action']").Click();

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
        SessionUiState? changedState = null;
        var invoice = new SessionNodeInput("invoice", "invoice", "Invoice", "InvoiceBlock");
        var nodes = new[] { new SessionContextNode(invoice) };

        var cut = Ctx.Render<SessionContextExplorer>(p => p
            .Add(x => x.RootNodes, nodes)
            .Add(x => x.OnProjected, e => projected = e)
            .Add(x => x.OnPinned, e => pinned = e)
            .Add(x => x.StateChanged, s => changedState = s));

        cut.Find("[data-node-id='invoice']").Click();
        cut.Find("[data-testid='session-context-node-menu-trigger'][data-node-id='invoice']").Click();
        ClickOpenContextNodePin();

        projected.ShouldNotBeNull();
        projected.Node.ShouldBe(invoice);
        pinned.ShouldNotBeNull();
        pinned.Node.ShouldBe(invoice);
        pinned.Pin.SessionNode.NodeTypeKey.ShouldBe("session.pin");
        changedState.ShouldNotBeNull();
        changedState.ContextPins!.Count.ShouldBe(1);
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

        maximized.Find("[data-mode='maximized']").ClassList.ShouldContain("flex");
        overlayPage.Find("[data-mode='overlay-page']").ClassList.ShouldContain("opacity-60");
        overlayApp.Find("[data-mode='overlay-app']").ClassList.ShouldContain("fixed");
        overlayApp.Find("[data-mode='overlay-app']").ClassList.ShouldContain("inset-0");
    }

    [Fact(DisplayName = "UAC16: SessionApp composes sessions, context pins, blocks, and command submit")]
    [Trait(Spec.UAC, "16")]
    public void UAC16_SessionAppComposesWorkingSurface()
    {
        var cut = Ctx.Render<SessionApp>();

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

    [Fact(DisplayName = "UAC17: SessionApp projects one transient feed block and replaces it on context selection change")]
    [Trait(Spec.UAC, "17")]
    public void UAC17_SessionAppReplacesProjectedFeedBlock()
    {
        var cut = Ctx.Render<SessionApp>();

        cut.Find("[data-node-id='release-session']").Click();
        cut.Find("[data-node-id='invoice-approval']").Click();

        var projectedBlock = cut.Find("[data-block-id='projected-context-block']");
        projectedBlock.TextContent.ShouldContain("Invoice approval");
        projectedBlock.TextContent.ShouldContain("Invoice detail block projected into the active session feed.");

        cut.Find("[data-node-id='shipment-delay']").Click();

        cut.FindAll("[data-block-id='projected-context-block']").Count.ShouldBe(1);
        projectedBlock = cut.Find("[data-block-id='projected-context-block']");
        projectedBlock.TextContent.ShouldContain("Shipment delay");
        projectedBlock.TextContent.ShouldContain("Shipment status block projected into the active session feed.");
        projectedBlock.TextContent.ShouldNotContain("Invoice detail block projected into the active session feed.");
    }

    [Fact(DisplayName = "UAC18: SessionApp keeps projected context scoped to the active session")]
    [Trait(Spec.UAC, "18")]
    public void UAC18_SessionAppScopesProjectedContextPerSession()
    {
        var cut = Ctx.Render<SessionApp>();

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

    [Fact(DisplayName = "UAC19: SessionApp renders context blocks after session turns")]
    [Trait(Spec.UAC, "19")]
    public void UAC19_SessionAppRendersContextBlocksAfterTurns()
    {
        var cut = Ctx.Render<SessionApp>();

        cut.Find("[data-node-id='release-session']").Click();
        cut.Find("[data-node-id='invoice-approval']").Click();

        var turnsIndex = cut.Markup.IndexOf("data-slot=\"session-app-turns\"", StringComparison.Ordinal);
        var blocksIndex = cut.Markup.IndexOf("data-slot=\"session-app-context-blocks\"", StringComparison.Ordinal);

        turnsIndex.ShouldBeGreaterThan(-1);
        blocksIndex.ShouldBeGreaterThan(turnsIndex);
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

    [Fact(DisplayName = "UAC22: SessionApp context block header unpin removes the block and restores the tree node")]
    [Trait(Spec.UAC, "22")]
    public void UAC22_SessionAppBlockHeaderUnpinRestoresTreeNode()
    {
        var cut = Ctx.Render<SessionApp>();

        cut.Find("[data-node-id='orders-session']").Click();
        cut.Find("[data-node-id='invoice-approval']").Click();
        cut.Find("[data-testid='session-context-node-menu-trigger'][data-node-id='invoice-approval']").Click();
        ClickOpenContextNodePin();

        // Block should show Pinned badge
        cut.Find("[data-testid='session-context-block-unpin']").ShouldNotBeNull();

        // Node should be hidden from tree
        cut.FindAll("[data-node-id='invoice-approval']").Count.ShouldBe(0);

        // Unpin via block header
        cut.Find("[data-testid='session-context-block-unpin']").Click();

        // Block should be removed
        cut.FindAll("[data-block-id='context-block:invoice-approval']").Count.ShouldBe(0);

        // Node should reappear in tree
        cut.Find("[data-node-id='invoice-approval']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC23: SessionApp scrolls projected and pinned context blocks into view")]
    [Trait(Spec.UAC, "23")]
    public void UAC23_SessionAppScrollsContextBlocksIntoView()
    {
        Ctx.JSInterop.SetupVoid("eval", _ => true).SetVoidResult();
        var cut = Ctx.Render<SessionApp>();

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

    [Fact(DisplayName = "UAC24: SessionApp right sidebar renders turns and blocks as one section list")]
    [Trait(Spec.UAC, "24")]
    public void UAC24_SessionAppRightSidebarRendersUnifiedSectionLinks()
    {
        Ctx.JSInterop.SetupVoid("eval", _ => true).SetVoidResult();
        var cut = Ctx.Render<SessionApp>();

        cut.Find("[data-node-id='release-session']").Click();

        var sidebar = cut.Find("[data-testid='session-app-right-sidebar']");
        sidebar.ShouldNotBeNull();
        sidebar.ClassList.ShouldContain("p-3");
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

    [Fact(DisplayName = "UAC25: SessionApp right sidebar starts empty until a session is active")]
    [Trait(Spec.UAC, "25")]
    public void UAC25_SessionAppRightSidebarStartsEmpty()
    {
        var cut = Ctx.Render<SessionApp>();

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

    [Fact(DisplayName = "UAC26: SessionApp session row counts match each session's child sections")]
    [Trait(Spec.UAC, "26")]
    public void UAC26_SessionAppSessionCountsMatchChildSections()
    {
        var cut = Ctx.Render<SessionApp>();

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

    [Fact(DisplayName = "UAC27: SessionApp composes left, content, and sections as resizable panels")]
    [Trait(Spec.UAC, "27")]
    public void UAC27_SessionAppUsesResizablePanels()
    {
        var cut = Ctx.Render<SessionApp>();

        cut.Find("[data-slot='resizable-panel-group']").ShouldNotBeNull();
        cut.FindAll("[data-slot='resizable-panel']").Count.ShouldBe(3);
        cut.FindAll("[data-slot='resizable-handle']").Count.ShouldBe(2);

        cut.Find("[data-panel='session-app-left-panel']").ShouldNotBeNull();
        cut.Find("[data-panel='session-app-main-panel']").ShouldNotBeNull();
        cut.Find("[data-panel='session-app-sections-panel']").ShouldNotBeNull();
        cut.FindAll("[data-slot='resizable-handle']")
            .ShouldAllBe(handle => handle.GetAttribute("data-reveal-on-hover") == "true");
        cut.FindAll("[data-slot='resizable-handle']")
            .ShouldAllBe(handle => handle.ClassList.Contains("bg-transparent"));
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

    private static string FindSessionCount(IRenderedComponent<SessionApp> cut, string sessionId) =>
        cut.Find($"[data-testid='session-list-item-count'][data-node-id='{sessionId}']").TextContent.Trim();
}
