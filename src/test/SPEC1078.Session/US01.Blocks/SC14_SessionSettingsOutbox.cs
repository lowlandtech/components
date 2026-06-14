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

    [Fact(DisplayName = "UAC02: SessionOutboxList renders side-effect entries with type and payload")]
    [Trait(Spec.UAC, "02")]
    [Given("a SessionSettings outbox tab with side-effect entries")]
    [When("the user selects the Outbox tab")]
    [Then("each side-effect type and payload are displayed")]
    public void UAC02_SideEffectEntriesRendered()
    {
        var outbox = new InMemorySessionOutbox();
        outbox.Append(new SessionOutboxEntry(
            typeof(ScheduleCreateEffect).Name,
            new ScheduleCreateEffect("workspace", "ws-1", "Billing"),
            DateTimeOffset.UtcNow));
        outbox.Append(new SessionOutboxEntry(
            typeof(ScheduleUpdateEffect).Name,
            new ScheduleUpdateEffect("workitem", "inv-1", "Label", "Approved"),
            DateTimeOffset.UtcNow));

        var cut = Ctx.Render<SessionSettings>(p => p
            .Add(x => x.Outbox, outbox));

        cut.Find("[data-testid='session-settings-outbox-tab']").Click();

        cut.FindAll("[data-slot='session-outbox-entry']").Count.ShouldBe(2);
        cut.Find("[data-outbox-type='ScheduleCreateEffect']").TextContent.ShouldContain("Billing");
        cut.Find("[data-outbox-type='ScheduleUpdateEffect']").TextContent.ShouldContain("Approved");
    }

    [Fact(DisplayName = "UAC03: SessionBlock passes its state-service outbox into SessionSettings")]
    [Trait(Spec.UAC, "03")]
    [Given("a SessionBlock with a state service that has an outbox")]
    [When("the settings page is shown")]
    [Then("the Outbox tab lists the side-effects from the state service")]
    public async Task UAC03_SessionBlockSurfacesOutboxInSettings()
    {
        var outbox = new InMemorySessionOutbox();
        var stateService = new SessionStateService(outbox);
        stateService.RegisterEffect((state, action, ct) =>
        {
            if (action is ISessionSideEffect sideEffect)
            {
                outbox.Append(new SessionOutboxEntry(sideEffect.GetType().Name, sideEffect, DateTimeOffset.UtcNow));
            }
            return Task.CompletedTask;
        });

        var sessions = new[] { new SessionListEntry("workitems-session", "Workitems") };
        stateService.SelectSession(new SessionNodeInput("workitems-session", "session", "Workitems"), sessions);
        await stateService.DispatchAsync(new ShowSessionSettingsPage(), CancellationToken.None);
        await stateService.DispatchAsync(
            new ScheduleCreateEffect("workspace", "ws-1", "Billing"),
            CancellationToken.None);

        outbox.Entries.Count.ShouldBe(1);

        var cut = Ctx.Render<SessionBlock>(p => p
            .Add(x => x.HasSidebar, true)
            .Add(x => x.StateService, stateService)
            .Add(x => x.Sessions, sessions)
            .Add(x => x.ContextNodes, GetContextNodes()));

        cut.Find("[data-testid='session-settings-outbox-tab']").Click();

        cut.FindAll("[data-slot='session-outbox-entry']").Count.ShouldBe(1);
    }

    [Fact(DisplayName = "UAC04: Plain UI actions do not appear in the owned outbox")]
    [Trait(Spec.UAC, "04")]
    [Given("a SessionBlock using its own SessionStateService")]
    [When("the user selects a session and opens Settings")]
    [Then("the Outbox tab contains no entries because no side-effects were scheduled")]
    public void UAC04_PlainUiActionsDoNotPopulateOutbox()
    {
        var sessions = new[] { new SessionListEntry("workitems-session", "Workitems") };

        var cut = Ctx.Render<SessionBlock>(p => p
            .Add(x => x.HasSidebar, true)
            .Add(x => x.Sessions, sessions)
            .Add(x => x.ContextNodes, GetContextNodes()));

        cut.Find("[data-testid='session-list-item'][data-node-id='workitems-session']").Click();
        cut.Find("[data-testid='session-block-settings-button']").Click();
        cut.Find("[data-testid='session-settings-outbox-tab']").Click();

        cut.FindAll("[data-slot='session-outbox-entry']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC05: Scheduling side-effects appends entries to the owned outbox")]
    [Trait(Spec.UAC, "05")]
    [Given("a SessionBlock using its own SessionStateService")]
    [When("the user renames a context node")]
    [Then("a ScheduleUpdateEffect appears in the Outbox tab")]
    public void UAC05_SideEffectActionsPopulateOwnedOutbox()
    {
        var sessions = new[] { new SessionListEntry("workitems-session", "Workitems") };

        var cut = Ctx.Render<SessionBlock>(p => p
            .Add(x => x.HasSidebar, true)
            .Add(x => x.Sessions, sessions)
            .Add(x => x.ContextNodes, GetContextNodes()));

        cut.Find("[data-testid='session-list-item'][data-node-id='workitems-session']").Click();
        OpenRenameMenu(cut);

        var commandBar = cut.FindComponent<SessionCommandBar>();
        commandBar.InvokeAsync(() => commandBar.Instance.ValueChanged.InvokeAsync("Workitem review"));
        commandBar.InvokeAsync(() => commandBar.Instance.OnSubmit.InvokeAsync(
            new SessionCommandSubmitted(
                new SessionNodeInput("setup-ci", "workitem", "Set up CI", "NodeBlock"),
                "edit",
                "Workitem review")));

        cut.Find("[data-testid='session-block-settings-button']").Click();
        cut.Find("[data-testid='session-settings-outbox-tab']").Click();

        var entries = cut.FindAll("[data-slot='session-outbox-entry']");
        entries.Count.ShouldBe(1);
        cut.Find("[data-outbox-type='ScheduleUpdateEffect']").TextContent.ShouldContain("Workitem review");
    }

    [Fact(DisplayName = "UAC06: Outbox entries are rendered newest-first")]
    [Trait(Spec.UAC, "06")]
    [Given("an outbox with two entries of different ages")]
    [When("the Outbox tab is selected")]
    [Then("the newest entry appears first")]
    public void UAC06_OutboxEntriesAreNewestFirst()
    {
        var outbox = new InMemorySessionOutbox();
        outbox.Append(new SessionOutboxEntry(
            typeof(ScheduleCreateEffect).Name,
            new ScheduleCreateEffect("workspace", "ws-1", "First"),
            DateTimeOffset.UtcNow.AddMinutes(-1)));
        outbox.Append(new SessionOutboxEntry(
            typeof(ScheduleUpdateEffect).Name,
            new ScheduleUpdateEffect("workspace", "ws-1", "Label", "Second"),
            DateTimeOffset.UtcNow));

        var cut = Ctx.Render<SessionSettings>(p => p.Add(x => x.Outbox, outbox));
        cut.Find("[data-testid='session-settings-outbox-tab']").Click();

        var entries = cut.FindAll("[data-outbox-type]");
        entries.Count.ShouldBe(2);
        entries[0].GetAttribute("data-outbox-type").ShouldBe(typeof(ScheduleUpdateEffect).Name);
        entries[1].GetAttribute("data-outbox-type").ShouldBe(typeof(ScheduleCreateEffect).Name);
    }

    private void OpenRenameMenu(IRenderedComponent<SessionBlock> cut)
    {
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
    }

    private static IReadOnlyList<SessionContextNode> GetContextNodes() =>
    [
        new(
            new("workspaces", "workitem", "Workspaces", "NodeBlock"),
            [
                new(new("setup-ci", "workitem", "Set up CI", "NodeBlock"), IsLoaded: true)
            ],
            true)
    ];
}
