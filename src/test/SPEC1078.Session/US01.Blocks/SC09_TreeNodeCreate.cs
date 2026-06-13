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
        var sessions = new[] { new SessionListEntry("s1", "Orders", 0, "session") };
        var stateService = new SessionStateService();
        stateService.SelectSession(new SessionNodeInput("s1", "session", "Orders"), sessions);

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

    [Fact(DisplayName = "UAC04: SessionBlock HandleSubmitAsync delegates non-session create to OnHeaderCreateRequested")]
    [Trait(Spec.UAC, "04")]
    [Given("a SessionBlock receives a submit for a non-session node in create mode")]
    [When("HandleSubmitAsync runs")]
    [Then("OnHeaderCreateRequested is invoked and agent type resets to ask")]
    public void UAC04_SessionBlockDelegatesNonSessionCreate()
    {
        SessionHeaderCreateRequest? request = null;
        var stateService = new SessionStateService();
        var headerNode = new SessionNodeInput("ws-header", "workspaces.header", "Workspaces");

        stateService.ProjectContext(headerNode);
        stateService.SetAgentType("create");

        var cut = Ctx.Render<Components.SessionBlock>(p => p
            .Add(x => x.StateService, stateService)
            .Add(x => x.OnHeaderCreateRequested, r => { request = r; return Task.CompletedTask; }));

        cut.Find("[data-testid='session-block-command-bar-toggle']").Click();

        // Trigger submit directly via the command bar's OnSubmit parameter
        var commandBar = cut.FindComponent<SessionCommandBar>();
        commandBar.InvokeAsync(() => commandBar.Instance.OnSubmit.InvokeAsync(
            new SessionCommandSubmitted(headerNode, "create", "My Workspace")));

        request.ShouldNotBeNull();
        request.ParentNodeId.ShouldBe("ws-header");
        request.ParentNodeTypeKey.ShouldBe("workspaces.header");
        request.Label.ShouldBe("My Workspace");
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

        cut.Find("[data-node-id='release-session']").Click();
        cut.Find("[data-node-id='invoice-approval']").Click();

        cut.Find("[data-slot='session-block-page-overlay']").ShouldNotBeNull();

        cut.Find("[data-testid='session-context-node-menu-trigger'][data-node-id='invoice-approval']").Click();

        var popoverService = Ctx.Services.GetRequiredService<IPopoverService>();
        var popover = popoverService.ActivePopovers.Single(reference => reference.State.Open);
        popover.State.Open.ShouldBeTrue();

        var fragmentCut = Ctx.Render(builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddContent(1, popover.State.Fragment);
            builder.CloseElement();
        });
        fragmentCut.Find(".node-menu-item").Click();

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

        cut.Find("[data-node-id='release-session']").Click();
        cut.Find("[data-node-id='invoice-approval']").Click();

        cut.Find("[data-slot='session-block-page-overlay']").ShouldNotBeNull();

        cut.Find("[data-testid='session-context-node-menu-trigger'][data-node-id='invoice-approval']").Click();

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

}
