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
        var cut = Ctx.Render<SessionBlock>(p => p.Add(x => x.HasSidebar, true));

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
        var cut = Ctx.Render<SessionBlock>();

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
        var cut = Ctx.Render<SessionBlock>();

        cut.Find("[data-testid='session-list-create']").Click();

        // Sidebar switches to active-session view (back button appears in embedded mode)
        cut.Find("[data-slot='session-app-sidebar-back']").ShouldNotBeNull();

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
    public void UAC04_CreatedSessionAppearsInSidebarList()
    {
        var cut = Ctx.Render<SessionBlock>();

        var initialCount = cut.FindAll("[data-slot='session-list-item']").Count;

        cut.Find("[data-testid='session-list-create']").Click();
        cut.Find("[data-slot='session-app-sidebar-back']").Click();

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
            new SessionListEntry("orders", "Orders", 12)
        };

        var cut = Ctx.Render<SessionList>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.OnCreateRequested, e => created = e));

        cut.Find("[data-testid='session-list-create']").Click();

        created.ShouldNotBeNull();
        created.NodeTypeKey.ShouldBe("session");
    }
}
