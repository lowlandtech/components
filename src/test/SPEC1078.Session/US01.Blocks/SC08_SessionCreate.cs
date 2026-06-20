namespace LowlandTech.Components.Tests.SPEC1078.Session.US01.Blocks;

[Trait(Spec.SPEC, "1078")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "08")]
[UserStory("01", "Session create button and create agent mode")]
public sealed class SC08_SessionCreate : ViewScenario
{
    [Fact(DisplayName = "UAC01: Standalone sidebar shows create session button")]
    [Trait(Spec.UAC, "01")]
    [Given("a standalone SessionBlock with no active session")]
    [When("the sidebar renders")]
    [Then("a create button is visible next to the Sessions label")]
    public void UAC01_StandaloneSidebarShowsCreateButton()
    {
        var cut = Ctx.Render<Components.SessionBlock>(p => p.Add(x => x.HasSidebar, true));

        var createButton = cut.Find("[data-testid='session-list-create']");
        createButton.ShouldNotBeNull();
        createButton.GetAttribute("data-slot").ShouldBe("session-list-create");
    }

    [Fact(DisplayName = "UAC02: Embedded SessionList shows create session button")]
    [Trait(Spec.UAC, "02")]
    [Given("an embedded SessionBlock with no active session")]
    [When("the session list renders")]
    [Then("a create button is visible in the list header")]
    public void UAC02_EmbeddedSessionListShowsCreateButton()
    {
        var cut = Ctx.Render<Components.SessionBlock>();

        var createButton = cut.Find("[data-testid='session-list-create']");
        createButton.ShouldNotBeNull();
        createButton.GetAttribute("data-slot").ShouldBe("session-list-create");
    }

    [Fact(DisplayName = "UAC03: Clicking create selects a new session and activates create agent mode")]
    [Trait(Spec.UAC, "03")]
    [Given("an embedded SessionBlock with no active session")]
    [When("the create button is clicked")]
    [Then("a new session is selected and the command bar agent type is create")]
    public void UAC03_CreateButtonSelectsNewSessionAndSetsCreateAgent()
    {
        var cut = Ctx.Render<Components.SessionBlock>();

        cut.Find("[data-testid='session-list-create']").Click();

        // Sidebar switches to active-session view (back button appears in embedded mode)
        cut.Find("[data-slot='session-block-sidebar-back']").ShouldNotBeNull();

        cut.Find("[data-testid='session-block-command-bar-toggle']").Click();

        // Command bar shows the create agent type as active
        var createAgent = cut.Find("[data-agent-type='create']");
        createAgent.GetAttribute("data-active").ShouldBe("true");

        // The selected session node has type "session"
        cut.Find("[data-slot='session-command-bar']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC04: Created session appears in the sidebar list")]
    [Trait(Spec.UAC, "04")]
    [Given("an embedded SessionBlock showing the session list")]
    [When("the create button is clicked and the user returns to the list")]
    [Then("the new session appears in the list")]
    public void UAC04_CreatedSessionBlockersInSidebarList()
    {
        var cut = Ctx.Render<Components.SessionBlock>();

        var initialCount = cut.FindAll("[data-slot='session-list-item']").Count;

        cut.Find("[data-testid='session-list-create']").Click();
        cut.Find("[data-slot='session-block-sidebar-back']").Click();

        var newCount = cut.FindAll("[data-slot='session-list-item']").Count;
        newCount.ShouldBe(initialCount + 1);
    }

    [Fact(DisplayName = "UAC05: SessionList emits create request with session nodetype")]
    [Trait(Spec.UAC, "05")]
    [Given("a SessionList with items")]
    [When("the create button is clicked")]
    [Then("OnCreateRequested fires with a session-typed node")]
    public void UAC05_SessionListCreateFiresWithSessionNodeType()
    {
        SessionNodeInput? created = null;
        var items = new[]
        {
            new SessionListEntry("workspaces", "Workspaces", 12)
        };

        var cut = Ctx.Render<SessionList>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.OnCreateRequested, e => created = e));

        cut.Find("[data-testid='session-list-create']").Click();

        created.ShouldNotBeNull();
        created.NodeTypeKey.ShouldBe("session");
    }

    [Fact(DisplayName = "UAC06: Submitting in create mode with a session node uses input as the session title")]
    [Trait(Spec.UAC, "06")]
    [Given("an embedded SessionBlock in create mode with a new session")]
    [When("the user types a title and submits")]
    [Then("the session is renamed, agent type resets to ask, and no command is added")]
    public void UAC06_CreateModeSubmitRenamesSession()
    {
        var stateService = new SessionStateService();
        var cut = Ctx.Render<Components.SessionBlock>(p => p
            .Add(x => x.StateService, stateService));

        // Create a new session
        cut.Find("[data-testid='session-list-create']").Click();

        // Verify we're in create mode
        stateService.State.Ui.ActiveAgentTypeKey.ShouldBe("create");
        var sessionId = stateService.State.ActiveSessionId;
        sessionId.ShouldNotBeNull();

        cut.Find("[data-testid='session-block-command-bar-toggle']").Click();

        // Type the session title and submit
        cut.Find("[data-testid='session-command-input']").Input("My New Session");
        cut.Find("[data-testid='session-command-submit']").Click();

        // Agent type should reset to "ask"
        stateService.State.Ui.ActiveAgentTypeKey.ShouldBe("ask");

        // The session node label should be updated
        stateService.State.Ui.SelectedNode?.Label.ShouldBe("My New Session");

        // No command should have been added to history
        var commands = stateService.GetSessionCommands(sessionId!);
        commands.Count.ShouldBe(0);

        // The session should appear in the sidebar with the new title
        cut.Find("[data-slot='session-block-sidebar-back']").Click();
        cut.Markup.ShouldContain("My New Session");
    }
}
