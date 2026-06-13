namespace LowlandTech.Components.Tests.SPEC1078.Session.US01.Blocks;

[Trait(Spec.SPEC, "1078")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "13")]
[UserStory("01", "Local context node rename from context menu")]
public sealed class SC13_LocalContextNodeRename : ViewScenario
{
    [Fact(DisplayName = "UAC01: Rename from context menu enters edit mode with the current label selected")]
    [Trait(Spec.UAC, "01")]
    [Given("a SessionBlock with context nodes")]
    [When("the user chooses Rename from the context menu")]
    [Then("agent type is edit, input contains the node label, and the text is selected")]
    public void UAC01_RenameFromContextMenuEntersEditMode()
    {
        var stateService = new SessionStateService();
        var sessions = new[] { new SessionListEntry("release-session", "Release notes") };
        stateService.SelectSession(new SessionNodeInput("release-session", "session", "Release notes"), sessions);

        var cut = Ctx.Render<Components.SessionBlock>(p => p
            .Add(x => x.HasSidebar, true)
            .Add(x => x.StateService, stateService)
            .Add(x => x.Sessions, sessions)
            .Add(x => x.ContextNodes, GetContextNodes()));

        OpenRenameMenu(cut);

        stateService.State.Ui.ActiveAgentTypeKey.ShouldBe("edit");
        stateService.State.Ui.Input.ShouldBe("Invoice approval");

        var selectionInvocations = Ctx.JSInterop.Invocations.Where(invocation =>
            invocation.Identifier == "eval"
            && invocation.Arguments.Count > 0
            && invocation.Arguments[0] is string script
            && script.Contains("session-command-input", StringComparison.Ordinal)
            && script.Contains("setSelectionRange", StringComparison.Ordinal))
            .ToList();

        selectionInvocations.Count.ShouldBeGreaterThan(0);
    }

    [Fact(DisplayName = "UAC02: Submitting in edit mode renames the selected node")]
    [Trait(Spec.UAC, "02")]
    [Given("the command bar is in edit mode for a context node")]
    [When("the user changes the input and submits")]
    [Then("the node label is updated, the projected block reflects it, and mode resets to ask")]
    public void UAC02_SubmitInEditModeRenamesNode()
    {
        var stateService = new SessionStateService();
        var sessions = new[] { new SessionListEntry("release-session", "Release notes") };
        stateService.SelectSession(new SessionNodeInput("release-session", "session", "Release notes"), sessions);

        var cut = Ctx.Render<Components.SessionBlock>(p => p
            .Add(x => x.HasSidebar, true)
            .Add(x => x.StateService, stateService)
            .Add(x => x.Sessions, sessions)
            .Add(x => x.ContextNodes, GetContextNodes()));

        OpenRenameMenu(cut);

        var commandBar = cut.FindComponent<SessionCommandBar>();
        commandBar.InvokeAsync(() => commandBar.Instance.ValueChanged.InvokeAsync("Invoice review"));
        commandBar.InvokeAsync(() => commandBar.Instance.OnSubmit.InvokeAsync(
            new SessionCommandSubmitted(
                new SessionNodeInput("invoice-approval", "invoice", "Invoice approval", "NodeBlock"),
                "edit",
                "Invoice review")));

        stateService.State.Ui.ActiveAgentTypeKey.ShouldBe("ask");
        stateService.State.Ui.Input.ShouldBeEmpty();

        cut.FindAll("[data-node-id='invoice-approval']").Count.ShouldBeGreaterThan(0);
        cut.Find("[data-node-id='invoice-approval']").TextContent.ShouldContain("Invoice review");

        var projectedBlock = cut.Find("[data-block-id='projected-context-block']");
        projectedBlock.ShouldNotBeNull();
        projectedBlock.TextContent.ShouldContain("Invoice review");
    }

    [Fact(DisplayName = "UAC04: Edit mode submit does not create an agent command")]
    [Trait(Spec.UAC, "04")]
    [Given("a SessionBlock in edit mode for a context node")]
    [When("the user submits the rename")]
    [Then("no session command is recorded and the node is renamed")]
    public void UAC04_EditModeSubmitDoesNotCreateAgentCommand()
    {
        var stateService = new SessionStateService();
        var sessions = new[] { new SessionListEntry("release-session", "Release notes") };
        stateService.SelectSession(new SessionNodeInput("release-session", "session", "Release notes"), sessions);

        var cut = Ctx.Render<Components.SessionBlock>(p => p
            .Add(x => x.HasSidebar, true)
            .Add(x => x.StateService, stateService)
            .Add(x => x.Sessions, sessions)
            .Add(x => x.ContextNodes, GetContextNodes()));

        OpenRenameMenu(cut);

        var commandBar = cut.FindComponent<SessionCommandBar>();
        commandBar.InvokeAsync(() => commandBar.Instance.ValueChanged.InvokeAsync("Invoice review"));
        commandBar.InvokeAsync(() => commandBar.Instance.OnSubmit.InvokeAsync(
            new SessionCommandSubmitted(
                new SessionNodeInput("invoice-approval", "invoice", "Invoice approval", "NodeBlock"),
                "edit",
                "Invoice review")));

        stateService.GetSessionCommands(stateService.CurrentSessionId).Count.ShouldBe(0);
        cut.Find("[data-node-id='invoice-approval']").TextContent.ShouldContain("Invoice review");
    }

    [Fact(DisplayName = "UAC03: Wired host handler receives the rename request")]
    [Trait(Spec.UAC, "03")]
    [Given("a SessionBlock with OnNodeRenameRequested wired")]
    [When("the user confirms a rename")]
    [Then("the host callback is invoked and the local node is not renamed")]
    public void UAC03_HostHandlerReceivesRenameRequest()
    {
        SessionNodeRenameRequest? request = null;
        var stateService = new SessionStateService();
        var sessions = new[] { new SessionListEntry("release-session", "Release notes") };
        stateService.SelectSession(new SessionNodeInput("release-session", "session", "Release notes"), sessions);

        var cut = Ctx.Render<Components.SessionBlock>(p => p
            .Add(x => x.HasSidebar, true)
            .Add(x => x.StateService, stateService)
            .Add(x => x.Sessions, sessions)
            .Add(x => x.ContextNodes, GetContextNodes())
            .Add(x => x.OnNodeRenameRequested, r => { request = r; return Task.CompletedTask; }));

        OpenRenameMenu(cut);

        var commandBar = cut.FindComponent<SessionCommandBar>();
        commandBar.InvokeAsync(() => commandBar.Instance.OnSubmit.InvokeAsync(
            new SessionCommandSubmitted(
                new SessionNodeInput("invoice-approval", "invoice", "Invoice approval", "NodeBlock"),
                "edit",
                "Invoice review")));

        request.ShouldNotBeNull();
        request.NodeId.ShouldBe("invoice-approval");
        request.NodeTypeKey.ShouldBe("invoice");
        request.OldLabel.ShouldBe("Invoice approval");
        request.NewLabel.ShouldBe("Invoice review");

        cut.Find("[data-node-id='invoice-approval']").TextContent.ShouldContain("Invoice approval");
    }

    private static IReadOnlyList<SessionContextNode> GetContextNodes() =>
    [
        new(
            new SessionNodeInput("orders", "orders.header", "Orders", "NodeBlock"),
            [
                new SessionContextNode(new SessionNodeInput("invoice-approval", "invoice", "Invoice approval", "NodeBlock"), IsLoaded: true),
                new SessionContextNode(new SessionNodeInput("shipment-delay", "shipment", "Shipment delay", "NodeBlock"), IsLoaded: true)
            ],
            IsLoaded: true)
    ];

    private void OpenRenameMenu(IRenderedComponent<Components.SessionBlock> cut)
    {
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
    }
}
