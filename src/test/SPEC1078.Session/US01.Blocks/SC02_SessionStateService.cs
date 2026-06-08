namespace LowlandTech.Components.Tests.SPEC1078.Session.US01.Blocks;

[Trait(Spec.SPEC, "1078")]
[Trait(Spec.SC, "02")]
[UserStory("01", "SessionStateService runs the session state machine fully in memory")]
public class SC02_SessionStateService : ViewScenario
{
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

    [Fact(DisplayName = "UAC35: SessionStateService projects display sessions and visible sections in memory")]
    [Trait(Spec.UAC, "35")]
    public async Task UAC35_SessionStateServiceProjectsFeedAndSectionsInMemory()
    {
        var sessions = new[]
        {
            new SessionListEntry("orders-session", "Orders"),
            new SessionListEntry("support-session", "Support triage")
        };
        var sessionNode = new SessionNodeInput("orders-session", "session", "Orders");
        var invoiceNode = new SessionNodeInput("invoice-approval", "invoice", "Invoice approval", "InvoiceBlock");
        var service = new SessionStateService();

        service.SelectSession(sessionNode, sessions);
        service.PinContext(invoiceNode);
        await service.SubmitAsync(
            new SessionCommandSubmitted(sessionNode, "ask", "Summarize invoice risk"),
            false,
            0,
            Ctx.Services.GetRequiredService<IGraphDiscoveryService>());

        var displaySessions = service.GetDisplaySessions(sessions);
        var visibleSections = service.GetVisibleSections(sessions);
        var feedItems = service.GetFeedItems(sessions);

        displaySessions.Single(item => item.Id == "orders-session").Count.ShouldBe(2);
        displaySessions.Single(item => item.Id == "support-session").Count.ShouldBe(0);

        visibleSections.Select(section => section.Id).ShouldBe(
        [
            "context-block:invoice-approval",
            SessionStateService.GetCommandTurnId(service.GetSessionCommands("orders-session").Single())
        ]);

        feedItems.Select(item => item.Kind).ShouldBe([SessionFeedItemKind.Block, SessionFeedItemKind.Response]);
        feedItems[0].Block?.Context.ShouldBe(invoiceNode);
        feedItems[1].Command?.Input.ShouldBe("Summarize invoice risk");
    }

    [Fact(DisplayName = "UAC36: SessionStateService resolves presentation targets in memory")]
    [Trait(Spec.UAC, "36")]
    public async Task UAC36_SessionStateServiceResolvesPresentationTargetsInMemory()
    {
        SessionListEntry[] sessions =
        [
            new SessionListEntry("orders-session", "Orders")
        ];
        var sessionNode = new SessionNodeInput("orders-session", "session", "Orders");
        var invoiceNode = new SessionNodeInput("invoice-approval", "invoice", "Invoice approval", "InvoiceBlock");
        var service = new SessionStateService();

        service.SelectSession(sessionNode, sessions);
        service.PinContext(invoiceNode);
        await service.SubmitAsync(
            new SessionCommandSubmitted(sessionNode, "ask", "Summarize invoice risk"),
            false,
            0,
            Ctx.Services.GetRequiredService<IGraphDiscoveryService>());

        var block = service.GetViewState(sessions).ContextBlocks!.Single();
        var command = service.GetSessionCommands("orders-session").Single();

        service.SetBlockMode(block, SessionContextBlock.ModeMaximized);
        service.GetMaximizedBlock(sessions)?.Id.ShouldBe(block.Id);
        service.GetPageOverlayBlock(sessions).ShouldBeNull();
        service.GetAppOverlayBlock(sessions).ShouldBeNull();

        service.SetCommandMode(command, SessionContextBlock.ModeOverlayPage);
        service.GetMaximizedCommand().ShouldBeNull();
        service.GetPageOverlayCommand()?.Id.ShouldBe(command.Id);
        service.GetAppOverlayCommand().ShouldBeNull();
    }

    [Fact(DisplayName = "UAC37: SessionStateService returns response projection metadata in memory")]
    [Trait(Spec.UAC, "37")]
    public async Task UAC37_SessionStateServiceReturnsResponseProjectionInMemory()
    {
        SessionListEntry[] sessions =
        [
            new SessionListEntry("orders-session", "Orders")
        ];
        var sessionNode = new SessionNodeInput("orders-session", "session", "Orders");
        var invoiceNode = new SessionNodeInput("invoice-approval", "invoice", "Invoice approval", "InvoiceBlock");
        var service = new SessionStateService();

        service.SelectSession(sessionNode, sessions);
        service.PinContext(invoiceNode);
        await service.SubmitAsync(
            new SessionCommandSubmitted(sessionNode, "ask", "Summarize invoice risk"),
            false,
            0,
            Ctx.Services.GetRequiredService<IGraphDiscoveryService>());

        var command = service.GetSessionCommands("orders-session").Single();
        var projection = service.GetResponseProjection(command);

        projection.Status.ShouldBe("ask");
        projection.StatusVariant.ShouldBe("outline");
        projection.Description.ShouldContain("Summarize invoice risk");
        projection.Description.ShouldContain("1 context block");
    }

    [Fact(DisplayName = "UAC38: SessionStateService resolves request and response surfaces through DRD registries")]
    [Trait(Spec.UAC, "38")]
    public void UAC38_SessionStateServiceResolvesSurfacesThroughDrdRegistries()
    {
        var discovery = new SessionSurfaceDiscovery([new PluginSessionSurfaceRegistry()]);
        var service = new SessionStateService(discovery);

        service.GetRequestNode("orders-session").Label.ShouldBe("Plugin order intake");
        service.GetResponseNode("orders-session").Label.ShouldBe("Plugin order output");
        service.GetRequestNode("support-session").Label.ShouldBe("Support ticket summary");
        service.GetResponseNode("support-session").Label.ShouldBe("Support context");
    }

    [Fact(DisplayName = "UAC38b: Session event log captures before and after state plus deltas")]
    [Trait(Spec.UAC, "38b")]
    public void UAC38b_SessionEventLogCapturesSnapshotsAndDeltas()
    {
        var service = new SessionStateService();

        service.SetInput("Trace me");

        var entry = service.State.EventLogEntries.ShouldHaveSingleItem();
        entry.ActionType.ShouldBe(nameof(SetSessionInput));
        entry.BeforeStateJson.ShouldContain("\"Input\": \"\"");
        entry.AfterStateJson.ShouldContain("\"Input\": \"Trace me\"");
        entry.Deltas.ShouldContain(delta =>
            delta.Path == "Ui.Input"
            && delta.Kind == "draft"
            && delta.Label == "Draft input"
            && delta.After == "Trace me"
            && delta.Description.Contains("Draft input changed", StringComparison.Ordinal));
    }

    [Fact(DisplayName = "UAC38c: Session event log emits typed deltas for active session and page changes")]
    [Trait(Spec.UAC, "38c")]
    public void UAC38c_SessionEventLogEmitsTypedDeltasForSessionAndPage()
    {
        var service = new SessionStateService();

        service.SelectSession(new SessionNodeInput("orders-session", "session", "Orders"), [new SessionListEntry("orders-session", "Orders")]);
        service.ShowEventLogPage();

        service.State.EventLogEntries.ShouldContain(entry =>
            entry.ActionType == nameof(SetActiveSession)
            && entry.Deltas.Any(delta => delta.Kind == "session" && delta.Label == "Active session" && delta.After == "orders-session"));

        service.State.EventLogEntries.ShouldContain(entry =>
            entry.ActionType == nameof(ShowSessionEventLogPage)
            && entry.Deltas.Any(delta => delta.Kind == "page" && delta.Label == "Active page" && delta.After == "EventLog"));
    }

    [Fact(DisplayName = "UAC39: SessionAppState round-trips through JSON and resumes in memory")]
    [Trait(Spec.UAC, "39")]
    public async Task UAC39_SessionAppStateRoundTripsThroughJsonAndResumesInMemory()
    {
        var sessions = new[]
        {
            new SessionListEntry("orders-session", "Orders"),
            new SessionListEntry("support-session", "Support triage")
        };
        var sessionNode = new SessionNodeInput("orders-session", "session", "Orders");
        var invoiceNode = new SessionNodeInput("invoice-approval", "invoice", "Invoice approval", "InvoiceBlock");
        var service = new SessionStateService();

        service.SelectSession(sessionNode, sessions);
        service.PinContext(invoiceNode);
        await service.SubmitAsync(
            new SessionCommandSubmitted(sessionNode, "ask", "Summarize invoice risk"),
            false,
            0,
            Ctx.Services.GetRequiredService<IGraphDiscoveryService>());
        service.SetInput("Persist me");
        service.ShowEventLogPage();

        var json = JsonSerializer.Serialize(service.State);
        var restoredState = JsonSerializer.Deserialize<SessionAppState>(json);

        restoredState.ShouldNotBeNull();
        var resumed = new SessionStateService(restoredState!);

        resumed.State.ActiveSessionId.ShouldBe("orders-session");
        resumed.State.ActivePage.ShouldBe(SessionAppPage.EventLog);
        resumed.State.EventLogEntries.ShouldNotBeEmpty();
        resumed.GetSessionCommands("orders-session").Count.ShouldBe(1);
        resumed.GetSessionCommands("orders-session")[0].Input.ShouldBe("Summarize invoice risk");
        resumed.GetViewState(sessions).ContextBlocks.ShouldNotBeNull();
        resumed.GetViewState(sessions).ContextBlocks!.Count.ShouldBe(1);
        resumed.GetViewState(sessions).Input.ShouldBe("Persist me");
        resumed.GetFeedItems(sessions).Select(item => item.Kind).ShouldBe([SessionFeedItemKind.Block, SessionFeedItemKind.Response]);
    }
}