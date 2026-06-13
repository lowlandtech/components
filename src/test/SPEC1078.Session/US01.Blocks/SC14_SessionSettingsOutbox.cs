namespace LowlandTech.Components.Tests.SPEC1078.Session.US01.Blocks;

[Trait(Spec.SPEC, "1078")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "14")]
[UserStory("01", "Session settings surfaces the action outbox")]
public sealed class SC14_SessionSettingsOutbox : ViewScenario
{
    [Fact(DisplayName = "UAC01: SessionSettings renders the Outbox tab when an outbox is supplied")]
    [Trait(Spec.UAC, "01")]
    [Given("a SessionSettings component with an outbox")]
    [When("the component is rendered")]
    [Then("the Outbox navigation tab is visible")]
    public void UAC01_OutboxTabVisibleWhenOutboxSupplied()
    {
        var outbox = new InMemorySessionOutbox();

        var cut = Ctx.Render<SessionSettings>(p => p
            .Add(x => x.Outbox, outbox));

        cut.FindAll("[data-testid='session-settings-outbox-tab']").Count.ShouldBe(1);
    }

    [Fact(DisplayName = "UAC02: SessionOutboxList renders entries from the supplied outbox")]
    [Trait(Spec.UAC, "02")]
    [Given("a SessionSettings outbox tab with one entry")]
    [When("the user selects the Outbox tab")]
    [Then("the entry type and payload are displayed")]
    public void UAC02_OutboxEntriesRendered()
    {
        var outbox = new InMemorySessionOutbox();
        outbox.Append(new SessionOutboxEntry(
            "TestEffect",
            new { Message = "hello" },
            DateTimeOffset.UtcNow));

        var cut = Ctx.Render<SessionSettings>(p => p
            .Add(x => x.Outbox, outbox));

        cut.Find("[data-testid='session-settings-outbox-tab']").Click();

        cut.FindAll("[data-slot='session-outbox-entry']").Count.ShouldBe(1);
        var entry = cut.Find("[data-outbox-type='TestEffect']");
        entry.TextContent.ShouldContain("TestEffect");
        entry.TextContent.ShouldContain("hello");
    }

    [Fact(DisplayName = "UAC03: SessionBlock passes its state-service outbox into SessionSettings")]
    [Trait(Spec.UAC, "03")]
    [Given("a SessionBlock that owns a SessionStateService with an outbox")]
    [When("the user opens Session Settings")]
    [Then("the Outbox tab lists the effects captured by the state service")]
    public async Task UAC03_SessionBlockSurfacesOutboxInSettings()
    {
        var outbox = new InMemorySessionOutbox();
        var stateService = new SessionStateService(outbox);
        stateService.RegisterEffect((state, action, ct) =>
        {
            outbox.Append(new SessionOutboxEntry(action.GetType().Name, action, DateTimeOffset.UtcNow));
            return Task.CompletedTask;
        });

        var sessions = new[] { new SessionListEntry("release-session", "Release notes") };
        stateService.SelectSession(new SessionNodeInput("release-session", "session", "Release notes"), sessions);
        await stateService.DispatchAsync(new ShowSessionSettingsPage(), CancellationToken.None);

        outbox.Entries.Count.ShouldBeGreaterThan(0);

        var cut = Ctx.Render<SessionBlock>(p => p
            .Add(x => x.HasSidebar, true)
            .Add(x => x.StateService, stateService)
            .Add(x => x.Sessions, sessions)
            .Add(x => x.ContextNodes, GetContextNodes()));

        cut.Find("[data-testid='session-settings-api-keys-tab']").ShouldNotBeNull();
        cut.Find("[data-testid='session-settings-outbox-tab']").Click();

        cut.FindAll("[data-slot='session-outbox-entry']").Count.ShouldBe(outbox.Entries.Count);
    }

    [Fact(DisplayName = "UAC04: SessionBlock's owned state service automatically records actions in the outbox")]
    [Trait(Spec.UAC, "04")]
    [Given("a SessionBlock using its own SessionStateService")]
    [When("the user selects a session and opens Settings")]
    [Then("the Outbox tab contains effects captured from dispatched actions")]
    public void UAC04_SessionBlockOwnedOutboxCapturesActions()
    {
        var sessions = new[] { new SessionListEntry("release-session", "Release notes") };

        var cut = Ctx.Render<SessionBlock>(p => p
            .Add(x => x.HasSidebar, true)
            .Add(x => x.Sessions, sessions)
            .Add(x => x.ContextNodes, GetContextNodes()));

        cut.Find("[data-testid='session-list-item'][data-node-id='release-session']").Click();
        cut.Find("[data-testid='session-block-settings-button']").Click();
        cut.Find("[data-testid='session-settings-outbox-tab']").Click();

        cut.FindAll("[data-slot='session-outbox-entry']").Count.ShouldBeGreaterThan(0);
    }

    [Fact(DisplayName = "UAC05: SessionOutboxList updates live when entries are appended after render")]
    [Trait(Spec.UAC, "05")]
    [Given("a SessionSettings Outbox tab already rendered")]
    [When("a new entry is appended to the outbox")]
    [Then("the list re-renders to show the new entry")]
    public void UAC05_SessionOutboxListUpdatesLive()
    {
        var outbox = new InMemorySessionOutbox();
        var cut = Ctx.Render<SessionSettings>(p => p.Add(x => x.Outbox, outbox));
        cut.Find("[data-testid='session-settings-outbox-tab']").Click();

        cut.FindAll("[data-slot='session-outbox-entry']").Count.ShouldBe(0);

        outbox.Append(new SessionOutboxEntry("LiveEffect", new { Value = 42 }, DateTimeOffset.UtcNow));

        cut.FindAll("[data-slot='session-outbox-entry']").Count.ShouldBe(1);
        cut.Find("[data-outbox-type='LiveEffect']").TextContent.ShouldContain("42");
    }

    [Fact(DisplayName = "UAC06: Outbox entries are rendered newest-first")]
    [Trait(Spec.UAC, "06")]
    [Given("an outbox with two entries of different ages")]
    [When("the Outbox tab is selected")]
    [Then("the newest entry appears first")]
    public void UAC06_OutboxEntriesAreNewestFirst()
    {
        var outbox = new InMemorySessionOutbox();
        outbox.Append(new SessionOutboxEntry("First", new { }, DateTimeOffset.UtcNow.AddMinutes(-1)));
        outbox.Append(new SessionOutboxEntry("Second", new { }, DateTimeOffset.UtcNow));

        var cut = Ctx.Render<SessionSettings>(p => p.Add(x => x.Outbox, outbox));
        cut.Find("[data-testid='session-settings-outbox-tab']").Click();

        var entries = cut.FindAll("[data-outbox-type]");
        entries.Count.ShouldBe(2);
        entries[0].GetAttribute("data-outbox-type").ShouldBe("Second");
        entries[1].GetAttribute("data-outbox-type").ShouldBe("First");
    }

    private static IReadOnlyList<SessionContextNode> GetContextNodes() =>
    [
        new(
            new("orders", "workitem", "Orders", "NodeBlock"),
            [
                new(new("invoice-approval", "invoice", "Invoice approval", "NodeBlock"), IsLoaded: true)
            ],
            true)
    ];
}
