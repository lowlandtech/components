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

    [Fact(DisplayName = "UAC07: Settings graph exposes API key create edge")]
    [Trait(Spec.UAC, "07")]
    [Given("the core graph descriptors are registered")]
    [When("the SessionBlock settings NodeType edges are inspected")]
    [Then("settings has a create edge targeting session-api-key")]
    public void UAC07_SettingsGraphExposesApiKeyCreateEdge()
    {
        var edgeService = Ctx.Services.GetRequiredService<IEdgeTypeService>();

        var edge = edgeService.GetOutgoingEdges("settings")
            .SingleOrDefault(candidate => string.Equals(candidate.TargetNodeTypeKey, "session-api-key", StringComparison.Ordinal));

        edge.ShouldNotBeNull();
        edge!.Intent.ShouldBe("create");
        edge.Surface.ShouldBe("settings");
        edge.Label.ShouldBe("Create API Key");
        edge.IsStatic.ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC08: Settings reducer records client key create success")]
    [Trait(Spec.UAC, "08")]
    [Given("an empty SessionBlock settings state")]
    [When("CreateSessionClientKeySucceeded is reduced")]
    [Then("the issued key is stored and the key list contains the new entry")]
    public void UAC08_SettingsReducerRecordsCreateSuccess()
    {
        var entry = new SessionClientKeyEntry("key-1", "Automation", "ltg_auto...1234", "test", DateTimeOffset.UtcNow);
        var issued = new SessionClientKeyIssued(entry, "ltg_auto_secret");

        var state = SessionBlockReducer.Reduce(
            SessionBlockDefaults.Empty,
            new CreateSessionClientKeySucceeded(issued));

        state.Settings.IssuedClientKey.ShouldBe(issued);
        state.Settings.ClientKeys.ShouldContain(entry);
        state.Settings.IsBusy.ShouldBeFalse();
    }

    [Fact(DisplayName = "UAC09: Creating an API key schedules a create mutation in the outbox")]
    [Trait(Spec.UAC, "09")]
    [Given("a SessionSettings panel with an outbox-backed state service")]
    [When("the user creates an API key")]
    [Then("a ScheduleCreateEffect for session-api-key is appended to the outbox")]
    public void UAC09_CreateApiKeySchedulesOutboxMutation()
    {
        var outbox = new InMemorySessionOutbox();
        var stateService = CreateOutboxStateService(outbox);
        var clientSettings = new TestClientSettingsService();

        var cut = Ctx.Render<SessionSettings>(p => p
            .Add(x => x.StateService, stateService)
            .Add(x => x.ClientSettings, clientSettings)
            .Add(x => x.Outbox, outbox));

        cut.Find("[data-testid='session-settings-api-keys-tab']").Click();
        cut.Find("[data-testid='session-settings-create-api-key']").Click();

        cut.WaitForAssertion(() =>
        {
            outbox.Entries.Count.ShouldBe(1);
            outbox.Entries[0].Type.ShouldBe(nameof(ScheduleCreateEffect));
            ((ScheduleCreateEffect)outbox.Entries[0].Payload).IsStatic.ShouldBeFalse();
            outbox.Entries[0].Payload.ToString().ShouldNotBeNull().ShouldContain("session-api-key");
        });
    }

    [Fact(DisplayName = "UAC10: Renaming an API key schedules an update mutation in the outbox")]
    [Trait(Spec.UAC, "10")]
    [Given("a SessionSettings panel with an existing API key")]
    [When("the user renames the key")]
    [Then("a ScheduleUpdateEffect for the API key name is appended to the outbox")]
    public void UAC10_RenameApiKeySchedulesOutboxMutation()
    {
        var outbox = new InMemorySessionOutbox();
        var stateService = CreateOutboxStateService(outbox);
        var clientSettings = new TestClientSettingsService();

        var cut = Ctx.Render<SessionSettings>(p => p
            .Add(x => x.StateService, stateService)
            .Add(x => x.ClientSettings, clientSettings)
            .Add(x => x.Outbox, outbox));

        cut.Find("[data-testid='session-settings-api-keys-tab']").Click();
        cut.Find("[data-testid='session-settings-rename-api-key']").Click();
        cut.Find("[data-testid='session-settings-api-key-name-input']").Input("Renamed API Key");
        cut.Find("[data-testid='session-settings-save-api-key']").Click();

        cut.WaitForAssertion(() =>
        {
            outbox.Entries.Count.ShouldBe(1);
            outbox.Entries[0].Type.ShouldBe(nameof(ScheduleUpdateEffect));
            outbox.Entries[0].Payload.ToString().ShouldNotBeNull().ShouldContain("Renamed API Key");
        });
    }

    [Fact(DisplayName = "UAC11: Settings renders NodeTypes and EdgeTypes management tabs")]
    [Trait(Spec.UAC, "11")]
    [Given("a SessionSettings component with registered graph descriptors")]
    [When("the component is rendered")]
    [Then("NodeTypes and EdgeTypes are the first two management tabs and NodeTypes is active")]
    public void UAC11_GraphManagementTabsAreVisible()
    {
        var cut = Ctx.Render<SessionSettings>();

        cut.FindAll("[data-testid='session-settings-node-types-tab']").Count.ShouldBe(1);
        cut.FindAll("[data-testid='session-settings-edge-types-tab']").Count.ShouldBe(1);
        cut.Find("[data-testid='session-settings-node-types-tab']").GetAttribute("data-active").ShouldBe("true");

        var tabIds = cut.Find("nav").QuerySelectorAll("button")
            .Select(button => button.GetAttribute("data-testid"))
            .ToArray();

        tabIds[0].ShouldBe("session-settings-node-types-tab");
        tabIds[1].ShouldBe("session-settings-edge-types-tab");
    }

    [Fact(DisplayName = "UAC12: Descriptor-backed NodeTypes are listed as static and read-only")]
    [Trait(Spec.UAC, "12")]
    [Given("a SessionSettings component with descriptor-loaded NodeTypes")]
    [When("the NodeTypes settings tab is selected")]
    [Then("static NodeTypes are shown and their edit actions are disabled")]
    public void UAC12_StaticNodeTypesAreReadOnly()
    {
        var cut = Ctx.Render<SessionSettings>();

        cut.Find("[data-testid='session-settings-node-types-tab']").Click();

        cut.FindAll("[data-testid='session-settings-node-type-row']").Count.ShouldBeGreaterThan(0);
        cut.Find("[data-node-type-key='settings']").TextContent.ShouldContain("Static");
        cut.Find("[data-node-type-key='settings'] [data-testid='session-settings-edit-node-type']")
            .HasAttribute("disabled")
            .ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC13: Creating a dynamic NodeType schedules a non-static create mutation")]
    [Trait(Spec.UAC, "13")]
    [Given("a SessionSettings component with an outbox-backed state service")]
    [When("the user creates a NodeType from settings")]
    [Then("a non-static node-type create effect is appended to the outbox")]
    public void UAC13_CreateNodeTypeSchedulesOutboxMutation()
    {
        var outbox = new InMemorySessionOutbox();
        var stateService = CreateOutboxStateService(outbox);

        var cut = Ctx.Render<SessionSettings>(p => p
            .Add(x => x.StateService, stateService)
            .Add(x => x.Outbox, outbox));

        cut.Find("[data-testid='session-settings-node-types-tab']").Click();
        cut.Find("[data-testid='session-settings-create-node-type']").Click();
        cut.Find("[data-testid='session-settings-node-type-label-input']").Input("Project");
        cut.Find("[data-testid='session-settings-node-type-key-input']").Input("project");
        cut.Find("[data-testid='session-settings-save-node-type']").Click();

        outbox.Entries.Count.ShouldBe(1);
        outbox.Entries[0].Type.ShouldBe(nameof(ScheduleCreateEffect));
        var effect = (ScheduleCreateEffect)outbox.Entries[0].Payload;
        effect.EntityType.ShouldBe("node-type");
        effect.Label.ShouldBe("Project");
        effect.IsStatic.ShouldBeFalse();
        cut.FindAll("[data-node-type-key='project']").Count.ShouldBeGreaterThan(0);
    }

    [Fact(DisplayName = "UAC14: Creating a dynamic EdgeType schedules a non-static create mutation")]
    [Trait(Spec.UAC, "14")]
    [Given("a SessionSettings component with an outbox-backed state service")]
    [When("the user creates an EdgeType from settings")]
    [Then("a non-static edge-type create effect is appended to the outbox")]
    public void UAC14_CreateEdgeTypeSchedulesOutboxMutation()
    {
        var outbox = new InMemorySessionOutbox();
        var stateService = CreateOutboxStateService(outbox);

        var cut = Ctx.Render<SessionSettings>(p => p
            .Add(x => x.StateService, stateService)
            .Add(x => x.Outbox, outbox));

        cut.Find("[data-testid='session-settings-edge-types-tab']").Click();
        cut.Find("[data-testid='session-settings-create-edge-type']").Click();
        cut.Find("[data-testid='session-settings-edge-type-label-input']").Input("Owns Project");
        cut.Find("[data-testid='session-settings-edge-type-key-input']").Input("owns.project");
        cut.Find("[data-testid='session-settings-edge-type-source-input']").Input("workspace");
        cut.Find("[data-testid='session-settings-edge-type-target-input']").Input("project");
        cut.Find("[data-testid='session-settings-save-edge-type']").Click();

        outbox.Entries.Count.ShouldBe(1);
        outbox.Entries[0].Type.ShouldBe(nameof(ScheduleCreateEffect));
        var effect = (ScheduleCreateEffect)outbox.Entries[0].Payload;
        effect.EntityType.ShouldBe("edge-type");
        effect.Label.ShouldBe("Owns Project");
        effect.IsStatic.ShouldBeFalse();
        cut.FindAll("[data-edge-type-key='owns-project']").Count.ShouldBeGreaterThan(0);
    }

    [Fact(DisplayName = "UAC15: Schema grids render pagination footers")]
    [Trait(Spec.UAC, "15")]
    [Given("a SessionSettings component with descriptor-loaded schema rows")]
    [When("the NodeTypes and EdgeTypes settings tabs are opened")]
    [Then("each schema list renders a left-side pagination footer")]
    public void UAC15_SchemaGridsRenderPaginationFooters()
    {
        var cut = Ctx.Render<SessionSettings>();

        cut.Find("[data-testid='session-settings-node-types-pagination-footer']")
            .TextContent.ShouldContain("Rows");

        cut.Find("[data-testid='session-settings-edge-types-tab']").Click();

        cut.Find("[data-testid='session-settings-edge-types-pagination-footer']")
            .TextContent.ShouldContain("Rows");
    }

    [Fact(DisplayName = "UAC16: Closing a selected NodeType hides the detail panel")]
    [Trait(Spec.UAC, "16")]
    [Given("a SessionSettings component with a selected NodeType detail panel")]
    [When("the user clicks the detail close button")]
    [Then("the detail panel returns to its empty state")]
    public void UAC16_CloseNodeTypePanelReturnsToEmptyState()
    {
        var cut = Ctx.Render<SessionSettings>();

        cut.Find("[data-node-type-key='action']").Click();
        cut.Find("[data-testid='session-settings-close-node-type-panel']").Click();

        cut.Find("[data-testid='session-settings-node-types-detail-slot']")
            .TextContent.ShouldContain("Select a NodeType");
    }

    [Fact(DisplayName = "UAC17: Closing a selected EdgeType hides the detail panel")]
    [Trait(Spec.UAC, "17")]
    [Given("a SessionSettings component with a selected EdgeType detail panel")]
    [When("the user clicks the detail close button")]
    [Then("the detail panel returns to its empty state")]
    public void UAC17_CloseEdgeTypePanelReturnsToEmptyState()
    {
        var cut = Ctx.Render<SessionSettings>();

        cut.Find("[data-testid='session-settings-edge-types-tab']").Click();
        cut.Find("[data-testid='session-settings-edge-type-row']").Click();
        cut.Find("[data-testid='session-settings-close-edge-type-panel']").Click();

        cut.Find("[data-testid='session-settings-edge-types-detail-slot']")
            .TextContent.ShouldContain("Select an EdgeType");
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

    private static SessionStateService CreateOutboxStateService(InMemorySessionOutbox outbox)
    {
        var stateService = new SessionStateService(outbox);
        stateService.RegisterEffect((state, action, ct) =>
        {
            if (action is ISessionSideEffect sideEffect)
                outbox.Append(new SessionOutboxEntry(sideEffect.GetType().Name, sideEffect, DateTimeOffset.UtcNow));

            return Task.CompletedTask;
        });
        return stateService;
    }

    private sealed class TestClientSettingsService : ISessionClientSettingsService
    {
        private readonly List<SessionClientKeyEntry> _keys =
        [
            new("key-1", "Default API Key", "ltg_test...0001", "test", DateTimeOffset.UtcNow.AddMinutes(-1)),
        ];

        public Task<IReadOnlyList<SessionClientKeyEntry>> ListAsync(CancellationToken ct = default) =>
            Task.FromResult<IReadOnlyList<SessionClientKeyEntry>>([.. _keys.Where(key => !key.IsRevoked)]);

        public Task<SessionClientKeyIssued> CreateAsync(string name, CancellationToken ct = default)
        {
            var entry = new SessionClientKeyEntry("key-2", name, "ltg_test...0002", "test", DateTimeOffset.UtcNow);
            _keys.Add(entry);
            return Task.FromResult(new SessionClientKeyIssued(entry, "ltg_test_secret"));
        }

        public Task RenameAsync(string id, string name, CancellationToken ct = default)
        {
            var index = _keys.FindIndex(key => string.Equals(key.Id, id, StringComparison.Ordinal));
            if (index >= 0)
                _keys[index] = _keys[index] with { Name = name };

            return Task.CompletedTask;
        }

        public Task RevokeAsync(string id, CancellationToken ct = default)
        {
            var index = _keys.FindIndex(key => string.Equals(key.Id, id, StringComparison.Ordinal));
            if (index >= 0)
                _keys[index] = _keys[index] with { IsRevoked = true };

            return Task.CompletedTask;
        }
    }
}
