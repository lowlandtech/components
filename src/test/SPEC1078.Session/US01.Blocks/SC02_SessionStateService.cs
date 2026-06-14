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
            => string.Equals(sessionId, "workspaces-session", StringComparison.Ordinal)
                ? new SessionSurfaceDescriptor(
                    sessionId,
                    new SessionNodeInput("plugin-request-workspaces", "session-request", "Plugin workspace intake"),
                    new SessionNodeInput("plugin-response-workspaces", "session-response", "Plugin workspace output"))
                : null;
    }

    [Fact(DisplayName = "UAC35: SessionStateService projects display sessions and visible sections in memory")]
    [Trait(Spec.UAC, "35")]
    public async Task UAC35_SessionStateServiceProjectsFeedAndSectionsInMemory()
    {
        var sessions = new[]
        {
            new SessionListEntry("workspaces-session", "Workspaces"),
            new SessionListEntry("flows-session", "Flows")
        };
        var sessionNode = new SessionNodeInput("workspaces-session", "session", "Workspaces");
        var workitemNode = new SessionNodeInput("setup-ci", "workitem", "Set up CI", "WorkitemBlock");
        var service = new SessionStateService();

        service.SelectSession(sessionNode, sessions);
        service.PinContext(workitemNode);
        await service.SubmitAsync(
            new SessionCommandSubmitted(sessionNode, "ask", "Summarize workitem risk"),
            false,
            0,
            Ctx.Services.GetRequiredService<IGraphDiscoveryService>());

        var displaySessions = service.GetDisplaySessions(sessions);
        var visibleSections = service.GetVisibleSections(sessions);
        var feedItems = service.GetFeedItems(sessions);

        displaySessions.Single(item => item.Id == "workspaces-session").Count.ShouldBe(2);
        displaySessions.Single(item => item.Id == "flows-session").Count.ShouldBe(0);

        visibleSections.Select(section => section.Id).ShouldBe(
        [
            "context-block:setup-ci",
            SessionStateService.GetCommandTurnId(service.GetSessionCommands("workspaces-session").Single())
        ]);

        feedItems.Select(item => item.Kind).ShouldBe([SessionFeedItemKind.Block, SessionFeedItemKind.Response]);
        feedItems[0].Block?.Context.ShouldBe(workitemNode);
        feedItems[1].Command?.Input.ShouldBe("Summarize workitem risk");
    }

    [Fact(DisplayName = "UAC36: SessionStateService resolves presentation targets in memory")]
    [Trait(Spec.UAC, "36")]
    public async Task UAC36_SessionStateServiceResolvesPresentationTargetsInMemory()
    {
        SessionListEntry[] sessions =
        [
            new SessionListEntry("workspaces-session", "Workspaces")
        ];
        var sessionNode = new SessionNodeInput("workspaces-session", "session", "Workspaces");
        var workitemNode = new SessionNodeInput("setup-ci", "workitem", "Set up CI", "WorkitemBlock");
        var service = new SessionStateService();

        service.SelectSession(sessionNode, sessions);
        service.PinContext(workitemNode);
        await service.SubmitAsync(
            new SessionCommandSubmitted(sessionNode, "ask", "Summarize workitem risk"),
            false,
            0,
            Ctx.Services.GetRequiredService<IGraphDiscoveryService>());

        var block = service.GetViewState(sessions).ContextBlocks!.Single();
        var command = service.GetSessionCommands("workspaces-session").Single();

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
            new SessionListEntry("workspaces-session", "Workspaces")
        ];
        var sessionNode = new SessionNodeInput("workspaces-session", "session", "Workspaces");
        var workitemNode = new SessionNodeInput("setup-ci", "workitem", "Set up CI", "WorkitemBlock");
        var service = new SessionStateService();

        service.SelectSession(sessionNode, sessions);
        service.PinContext(workitemNode);
        await service.SubmitAsync(
            new SessionCommandSubmitted(sessionNode, "ask", "Summarize workitem risk"),
            false,
            0,
            Ctx.Services.GetRequiredService<IGraphDiscoveryService>());

        var command = service.GetSessionCommands("workspaces-session").Single();
        var projection = service.GetResponseProjection(command);

        projection.Status.ShouldBe("ask");
        projection.StatusVariant.ShouldBe("outline");
        projection.Description.ShouldContain("Summarize workitem risk");
        projection.Description.ShouldContain("1 context block");
    }

    [Fact(DisplayName = "UAC38: SessionStateService resolves request and response surfaces through DRD registries")]
    [Trait(Spec.UAC, "38")]
    public void UAC38_SessionStateServiceResolvesSurfacesThroughDrdRegistries()
    {
        var discovery = new SessionSurfaceDiscovery([new PluginSessionSurfaceRegistry()]);
        var service = new SessionStateService(discovery);

        service.GetRequestNode("workspaces-session").Label.ShouldBe("Plugin workspace intake");
        service.GetResponseNode("workspaces-session").Label.ShouldBe("Plugin workspace output");
        service.GetRequestNode("flows-session").Label.ShouldBe("Flow summary");
        service.GetResponseNode("flows-session").Label.ShouldBe("Flows context");
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

        service.SelectSession(new SessionNodeInput("workspaces-session", "session", "Workspaces"), [new SessionListEntry("workspaces-session", "Workspaces")]);
        service.ShowEventLogPage();

        service.State.EventLogEntries.ShouldContain(entry =>
            entry.ActionType == nameof(SetActiveSession)
            && entry.Deltas.Any(delta => delta.Kind == "session" && delta.Label == "Active session" && delta.After == "workspaces-session"));

        service.State.EventLogEntries.ShouldContain(entry =>
            entry.ActionType == nameof(ShowSessionEventLogPage)
            && entry.Deltas.Any(delta => delta.Kind == "page" && delta.Label == "Active page" && delta.After == "EventLog"));
    }

    [Fact(DisplayName = "UAC39: SessionBlockState round-trips through JSON and resumes in memory")]
    [Trait(Spec.UAC, "39")]
    public async Task UAC39_SessionBlockStateRoundTripsThroughJsonAndResumesInMemory()
    {
        var sessions = new[]
        {
            new SessionListEntry("workspaces-session", "Workspaces"),
            new SessionListEntry("flows-session", "Flows")
        };
        var sessionNode = new SessionNodeInput("workspaces-session", "session", "Workspaces");
        var workitemNode = new SessionNodeInput("setup-ci", "workitem", "Set up CI", "WorkitemBlock");
        var service = new SessionStateService();

        service.SelectSession(sessionNode, sessions);
        service.PinContext(workitemNode);
        await service.SubmitAsync(
            new SessionCommandSubmitted(sessionNode, "ask", "Summarize workitem risk"),
            false,
            0,
            Ctx.Services.GetRequiredService<IGraphDiscoveryService>());
        service.SetInput("Persist me");
        service.ShowEventLogPage();

        var json = JsonSerializer.Serialize(service.State);
        var restoredState = JsonSerializer.Deserialize<SessionBlockState>(json);

        restoredState.ShouldNotBeNull();
        var resumed = new SessionStateService(restoredState!);

        resumed.State.ActiveSessionId.ShouldBe("workspaces-session");
        resumed.State.ActivePage.ShouldBe(SessionBlockPage.EventLog);
        resumed.State.EventLogEntries.ShouldNotBeEmpty();
        resumed.GetSessionCommands("workspaces-session").Count.ShouldBe(1);
        resumed.GetSessionCommands("workspaces-session")[0].Input.ShouldBe("Summarize workitem risk");
        resumed.GetViewState(sessions).ContextBlocks.ShouldNotBeNull();
        resumed.GetViewState(sessions).ContextBlocks!.Count.ShouldBe(1);
        resumed.GetViewState(sessions).Input.ShouldBe("Persist me");
        resumed.GetFeedItems(sessions).Select(item => item.Kind).ShouldBe([SessionFeedItemKind.Block, SessionFeedItemKind.Response]);
    }
}