namespace LowlandTech.Components.Tests.SPEC1078.Session.US01.Blocks;

[Trait(Spec.SPEC, "1078")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "09")]
[UserStory("01", "Tree node create affordance triggers create mode and delegates to host")]
public sealed class SC09_TreeNodeCreate : ViewScenario
{
    [Fact(DisplayName = "UAC01: Clicking the plus icon on a header node fires OnNodeNew")]
    [Trait(Spec.UAC, "01")]
    [Given("a SessionContextExplorer with a header node")]
    [When("the plus icon on the header is clicked")]
    [Then("OnNodeNew fires with the header node")]
    public void UAC01_PlusIconOnHeaderFiresOnNodeNew()
    {
        SessionNodeInput? fired = null;
        var headerNode = new SessionNodeInput("ws-header", "workspaces.header", "Workspaces");
        var contextNodes = new[]
        {
            new SessionContextNode(
                new SessionNodeInput("user", "user-context", "User"),
                new[] { new SessionContextNode(headerNode, IsLoaded: true) },
                true)
        };

        var cut = Ctx.Render<SessionContextExplorer>(p => p
            .Add(x => x.RootNodes, contextNodes)
            .Add(x => x.OnNodeNew, node => { fired = node; return Task.CompletedTask; }));

        cut.Find("[data-slot='session-context-node-create']").Click();

        fired.ShouldNotBeNull();
        fired.Id.ShouldBe("ws-header");
    }

    [Fact(DisplayName = "UAC02: SessionBlock HandleNodeNewAsync sets create agent type and focuses input")]
    [Trait(Spec.UAC, "02")]
    [Given("a SessionBlock with an active session and a context tree")]
    [When("the plus icon on a header is clicked")]
    [Then("agent type is create and JS focuses the command input")]
    public void UAC02_SessionBlockSetsCreateAndFocusesInput()
    {
        var sessions = new[] { new SessionListEntry("s1", "Workspaces", 0, "session") };
        var stateService = new SessionStateService();
        stateService.SelectSession(new SessionNodeInput("s1", "session", "Workspaces"), sessions);

        var headerNode = new SessionNodeInput("ws-header", "workspaces.header", "Workspaces");
        var contextNodes = new[]
        {
            new SessionContextNode(
                new SessionNodeInput("user", "user-context", "User"),
                new[] { new SessionContextNode(headerNode, IsLoaded: true) },
                true)
        };

        var cut = Ctx.Render<Components.SessionBlock>(p => p
            .Add(x => x.HasSidebar, true)
            .Add(x => x.StateService, stateService)
            .Add(x => x.Sessions, sessions)
            .Add(x => x.ContextNodes, contextNodes));

        cut.Find("[data-slot='session-context-node-create']").Click();

        stateService.State.Ui.ActiveAgentTypeKey.ShouldBe("create");

        var focusInvocations = Ctx.JSInterop.Invocations.Where(invocation =>
            invocation.Identifier == "eval"
            && invocation.Arguments.Count > 0
            && invocation.Arguments[0] is string script
            && script.Contains("session-command-input", StringComparison.Ordinal)
            && script.Contains("focus", StringComparison.Ordinal))
            .ToList();
        focusInvocations.Count.ShouldBeGreaterThan(0);
    }

    [Fact(DisplayName = "UAC03: SessionCommandBar submit in create mode with a header node emits the header node")]
    [Trait(Spec.UAC, "03")]
    [Given("a SessionCommandBar with a header SelectedNode and create agent type")]
    [When("submit is invoked")]
    [Then("OnSubmit fires with the header node, not a session node")]
    public void UAC03_CommandBarSubmitPreservesHeaderNode()
    {
        SessionCommandSubmitted? submitted = null;
        var headerNode = new SessionNodeInput("ws-header", "workspaces.header", "Workspaces");
        var state = SessionUiDefaults.Empty with
        {
            SelectedNode = headerNode,
            ActiveAgentTypeKey = "create"
        };

        var cut = Ctx.Render<SessionCommandBar>(p => p
            .Add(x => x.State, state)
            .Add(x => x.SelectedNode, headerNode)
            .Add(x => x.ActiveAgentTypeKey, "create")
            .Add(x => x.Value, "My Workspace")
            .Add(x => x.OnSubmit, e => submitted = e));

        cut.Find("[data-testid='session-command-submit']").Click();

        submitted.ShouldNotBeNull();
        submitted.Node.ShouldBe(headerNode);
        submitted.AgentTypeKey.ShouldBe("create");
        submitted.Input.ShouldBe("My Workspace");
    }

    [Fact(DisplayName = "UAC04: SessionBlock HandleSubmitAsync delegates non-session create to OnHeaderCreateRequested when no create edge exists")]
    [Trait(Spec.UAC, "04")]
    [Given("a SessionBlock receives a submit for a node with no registered create edges")]
    [When("HandleSubmitAsync runs")]
    [Then("OnHeaderCreateRequested is invoked and agent type resets to ask")]
    public void UAC04_SessionBlockDelegatesNonSessionCreate()
    {
        SessionHeaderCreateRequest? request = null;
        var stateService = new SessionStateService();
        var parentNode = new SessionNodeInput("custom-node", "custom", "Custom", "NodeBlock");

        stateService.ProjectContext(parentNode);
        stateService.SetAgentType("create");

        var cut = Ctx.Render<Components.SessionBlock>(p => p
            .Add(x => x.StateService, stateService)
            .Add(x => x.OnHeaderCreateRequested, r => { request = r; return Task.CompletedTask; }));

        cut.Find("[data-testid='session-block-command-bar-toggle']").Click();

        // Trigger submit directly via the command bar's OnSubmit parameter
        var commandBar = cut.FindComponent<SessionCommandBar>();
        commandBar.InvokeAsync(() => commandBar.Instance.OnSubmit.InvokeAsync(
            new SessionCommandSubmitted(parentNode, "create", "My Custom Child")));

        request.ShouldNotBeNull();
        request.ParentNodeId.ShouldBe("custom-node");
        request.Label.ShouldBe("My Custom Child");
        stateService.State.Ui.ActiveAgentTypeKey.ShouldBe("ask");
    }

    [Fact(DisplayName = "UAC05: SessionContextTreeNodeMenu New button fires NewRequested")]
    [Trait(Spec.UAC, "05")]
    [Given("a SessionContextTreeNodeMenu for a workspace node")]
    [When("the New button is clicked")]
    [Then("NewRequested fires with the workspace node")]
    public void UAC05_MenuNewFiresNewRequested()
    {
        SessionNodeInput? fired = null;
        var workspaceNode = new SessionNodeInput("ws-1", "workspace", "Accounting");

        var cut = Ctx.Render<SessionContextTreeNodeMenu>(p => p
            .Add(x => x.Node, workspaceNode)
            .Add(x => x.NewRequested, node => { fired = node; return Task.CompletedTask; }));

        cut.Find(".node-menu-item").Click();

        fired.ShouldNotBeNull();
        fired.Id.ShouldBe("ws-1");
    }

    [Fact(DisplayName = "UAC06: SessionBlock New from context menu resets Page Overlay to Normal")]
    [Trait(Spec.UAC, "06")]
    [Given("a SessionBlock with a context card open in Page Overlay mode")]
    [When("the New option is chosen from the node's context menu")]
    [Then("the projected context block returns to Normal mode")]
    public void UAC06_SessionBlockNewResetsPageOverlay()
    {
        var cut = Ctx.Render<Components.SessionBlock>();

        cut.Find("[data-node-id='workitems-session']").Click();
        cut.Find("[data-node-id='setup-ci']").Click();

        cut.Find("[data-slot='session-block-page-overlay']").ShouldNotBeNull();

        cut.Find("[data-testid='session-context-node-menu-trigger'][data-node-id='setup-ci']").Click();

        var popoverService = Ctx.Services.GetRequiredService<IPopoverService>();
        var popover = popoverService.ActivePopovers.Single(reference => reference.State.Open);
        popover.State.Open.ShouldBeTrue();

        var fragmentCut = Ctx.Render(builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddContent(1, popover.State.Fragment);
            builder.CloseElement();
        });
        fragmentCut.Find("[data-testid='session-context-node-new-item']").Click();

        cut.FindAll("[data-slot='session-block-page-overlay']").Count.ShouldBe(0);
        cut.Find("[data-slot='session-block-feed'] [data-block-id='projected-context-block'][data-mode='normal']")
            .ShouldNotBeNull();
        cut.Find("[data-slot='session-command-bar']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC07: SessionBlock Rename from context menu resets Page Overlay and reveals command bar")]
    [Trait(Spec.UAC, "07")]
    [Given("a SessionBlock with a context card open in Page Overlay mode")]
    [When("the Rename option is chosen from the node's context menu")]
    [Then("the projected context block returns to Normal mode and the command bar is visible")]
    public void UAC07_SessionBlockRenameResetsPageOverlayAndRevealsCommandBar()
    {
        var cut = Ctx.Render<Components.SessionBlock>();

        cut.Find("[data-node-id='workitems-session']").Click();
        cut.Find("[data-node-id='setup-ci']").Click();

        cut.Find("[data-slot='session-block-page-overlay']").ShouldNotBeNull();

        cut.Find("[data-testid='session-context-node-menu-trigger'][data-node-id='setup-ci']").Click();

        var popoverService = Ctx.Services.GetRequiredService<IPopoverService>();
        var popover = popoverService.ActivePopovers.Single(reference => reference.State.Open);

        var fragmentCut = Ctx.Render(builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddContent(1, popover.State.Fragment);
            builder.CloseElement();
        });
        fragmentCut.FindAll(".node-menu-item")
            .Single(item => string.Equals(item.TextContent.Trim(), "Rename", StringComparison.Ordinal))
            .Click();

        cut.FindAll("[data-slot='session-block-page-overlay']").Count.ShouldBe(0);
        cut.Find("[data-slot='session-block-feed'] [data-block-id='projected-context-block'][data-mode='normal']")
            .ShouldNotBeNull();
        cut.Find("[data-slot='session-command-bar']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC08: Header create button is hidden until hover or focus")]
    [Trait(Spec.UAC, "08")]
    [Given("a SessionContextTreeNode for a header node")]
    [When("the node's markup is inspected")]
    [Then("the create button uses opacity-0 with group-hover/focus visibility")]
    public void UAC08_HeaderCreateButtonHidesUntilHoverOrFocus()
    {
        var headerNode = new SessionNodeInput("ws-header", "workspaces.header", "Workspaces");
        var cut = Ctx.Render<SessionContextTreeNode>(p => p
            .Add(x => x.Item, new SessionContextNode(headerNode, IsLoaded: true)));

        var button = cut.Find("[data-slot='session-context-node-create']");
        var classList = button.ClassList;

        classList.ShouldContain("opacity-0");
        classList.ShouldContain("group-hover:opacity-100");
        classList.ShouldContain("focus:opacity-100");
    }

    [Fact(DisplayName = "UAC09: Context menu renders one New option per outgoing create edge")]
    [Trait(Spec.UAC, "09")]
    [Given("a SessionContextTreeNodeMenu with two create edges")]
    [When("the menu is rendered")]
    [Then("each create edge renders a New <Target> item")]
    public void UAC09_ContextMenuRendersNewOptionPerCreateEdge()
    {
        var workspaceNode = new SessionNodeInput("ws-1", "workspace", "Foundry");
        var createEdges = new[]
        {
            new EdgeTypeInfo("has.table", "create", "Create Table", null, "TableCreator", "workspace", false, "table", null, null),
            new EdgeTypeInfo("has.workflow", "create", "Create Workflow", null, "WorkflowCreator", "workspace", false, "workflow", null, null)
        };

        var cut = Ctx.Render<SessionContextTreeNodeMenu>(p => p
            .Add(x => x.Node, workspaceNode)
            .Add(x => x.CreateEdges, createEdges));

        cut.Find("[data-testid='session-context-node-new-submenu']").Click();
        var items = cut.FindAll("[data-testid='session-context-node-new-item']").ToList();
        items.Count.ShouldBe(2);
        items[0].TextContent.Trim().ShouldContain("Table");
        items[0].GetAttribute("data-edge-key").ShouldBe("has.table");
        items[1].TextContent.Trim().ShouldContain("Workflow");
        items[1].GetAttribute("data-edge-key").ShouldBe("has.workflow");
    }

    [Fact(DisplayName = "UAC09b: Dynamic New uses a shadcn-style flyout submenu")]
    [Trait(Spec.UAC, "09b")]
    [Given("a SessionContextTreeNodeMenu with outgoing create edges")]
    [When("the New submenu is opened")]
    [Then("the child menu is exposed as a right-side menu without turning the chevron downward")]
    public void UAC09b_ContextMenuUsesRightSideFlyoutSubmenu()
    {
        var workspaceNode = new SessionNodeInput("ws-1", "workspace", "Foundry");
        var createEdges = new[]
        {
            new EdgeTypeInfo("has.table", "create", "Create Table", null, "TableCreator", "workspace", false, "table", null, null)
        };

        var cut = Ctx.Render<SessionContextTreeNodeMenu>(parameters => parameters
            .Add(component => component.Node, workspaceNode)
            .Add(component => component.CreateEdges, createEdges));

        var trigger = cut.Find("[data-testid='session-context-node-new-submenu']");
        trigger.Click();

        trigger.GetAttribute("aria-haspopup").ShouldBe("menu");
        trigger.GetAttribute("aria-expanded").ShouldBe("true");
        trigger.GetAttribute("data-state").ShouldBe("open");
        trigger.QuerySelector("svg")!.ClassList.ShouldNotContain("rotate-90");

        var submenu = cut.Find("[data-slot='session-context-node-submenu']");
        submenu.GetAttribute("role").ShouldBe("menu");
        submenu.GetAttribute("data-side").ShouldBe("right");
        submenu.GetAttribute("data-state").ShouldBe("open");
    }

    [Fact(DisplayName = "UAC09c: Portal node menus preserve flyout overflow")]
    [Trait(Spec.UAC, "09c")]
    [Given("the PopoverProvider stylesheet used by explorer node menus")]
    [When("the node-menu overflow contract is inspected")]
    [Then("the root menu permits visible overflow while the flyout owns vertical scrolling")]
    public void UAC09c_PopoverProviderPreservesSubmenuOverflow()
    {
        var cut = Ctx.Render<PopoverProvider>();

        cut.Markup.ShouldContain(".node-menu {");
        cut.Markup.ShouldContain("overflow: visible;");
        cut.Markup.ShouldContain(".node-menu-submenu-flyout");
        cut.Markup.ShouldContain("overflow-y: auto;");
    }

    [Fact(DisplayName = "UAC10: Clicking a dynamic New option fires NewRequested with the selected edge key")]
    [Trait(Spec.UAC, "10")]
    [Given("a SessionContextTreeNodeMenu with a create edge")]
    [When("the New <Target> item is clicked")]
    [Then("NewRequested fires with the node carrying the createEdgeKey property")]
    public void UAC10_DynamicNewFiresNewRequestedWithEdgeKey()
    {
        SessionNodeInput? fired = null;
        var workspaceNode = new SessionNodeInput("ws-1", "workspace", "Foundry");
        var createEdges = new[]
        {
            new EdgeTypeInfo("has.table", "create", "Create Table", null, "TableCreator", "workspace", false, "table", null, null)
        };

        var cut = Ctx.Render<SessionContextTreeNodeMenu>(p => p
            .Add(x => x.Node, workspaceNode)
            .Add(x => x.CreateEdges, createEdges)
            .Add(x => x.NewRequested, node => { fired = node; return Task.CompletedTask; }));

        cut.Find("[data-testid='session-context-node-new-submenu']").Click();
        cut.Find("[data-testid='session-context-node-new-item']").Click();

        fired.ShouldNotBeNull();
        fired.Id.ShouldBe("ws-1");
        fired.Properties.ShouldNotBeNull();
        fired.Properties.ShouldContainKey("createEdgeKey");
        fired.Properties!["createEdgeKey"].ShouldBe("has.table");
    }

    [Fact(DisplayName = "UAC11: SessionBlock sets create:<edgeKey> agent type when a dynamic New option is chosen")]
    [Trait(Spec.UAC, "11")]
    [Given("a SessionBlock with a workspaces.header node")]
    [When("the New Workspace context-menu item is clicked")]
    [Then("the active agent type is create:has.workspace")]
    public void UAC11_SessionBlockSetsCreateEdgeAgentType()
    {
        var stateService = new SessionStateService();
        var sessions = new[] { new SessionListEntry("s1", "Workspaces", 0, "session") };
        stateService.SelectSession(new SessionNodeInput("s1", "session", "Workspaces"), sessions);

        var headerNode = new SessionNodeInput("ws-header", "workspaces.header", "Workspaces");
        var contextNodes = new[] { new SessionContextNode(headerNode, IsLoaded: true) };

        var cut = Ctx.Render<Components.SessionBlock>(p => p
            .Add(x => x.HasSidebar, true)
            .Add(x => x.StateService, stateService)
            .Add(x => x.Sessions, sessions)
            .Add(x => x.ContextNodes, contextNodes));

        cut.Find("[data-testid='session-context-node-menu-trigger'][data-node-id='ws-header']").Click();

        var popoverService = Ctx.Services.GetRequiredService<IPopoverService>();
        var popover = popoverService.ActivePopovers.Single(reference => reference.State.Open);

        var fragmentCut = Ctx.Render(builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddContent(1, popover.State.Fragment);
            builder.CloseElement();
        });

        fragmentCut.Find("[data-testid='session-context-node-new-submenu']").Click();
        fragmentCut.Find("[data-testid='session-context-node-new-item']").Click();

        stateService.State.Ui.ActiveAgentTypeKey.ShouldBe("create:has.workspace");
    }

    [Fact(DisplayName = "UAC12: Workspace context menu shows New options for each create edge")]
    [Trait(Spec.UAC, "12")]
    [Given("a SessionBlock with a workspace node")]
    [When("the workspace context menu is opened")]
    [Then("it renders New Table, New Workflow, New Page, New Feature and New Branch")]
    public void UAC12_WorkspaceContextMenuShowsCreateEdgeOptions()
    {
        var cut = Ctx.Render<Components.SessionBlock>(p => p.Add(x => x.HasSidebar, true));

        cut.Find("[data-node-id='workspaces-session']").Click();
        cut.Find("[data-testid='session-context-node-menu-trigger'][data-node-id='foundry']").Click();

        var popoverService = Ctx.Services.GetRequiredService<IPopoverService>();
        var popover = popoverService.ActivePopovers.Single(reference => reference.State.Open);

        var fragmentCut = Ctx.Render(builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddContent(1, popover.State.Fragment);
            builder.CloseElement();
        });

        fragmentCut.Find("[data-testid='session-context-node-new-submenu']").Click();
        var items = fragmentCut.FindAll("[data-testid='session-context-node-new-item']").Select(e => e.TextContent.Trim()).ToList();
        items.ShouldContain("Branch");
        items.ShouldContain("Feature");
        items.ShouldContain("Page");
        items.ShouldContain("Table");
        items.ShouldContain("Workflow");
    }

}
