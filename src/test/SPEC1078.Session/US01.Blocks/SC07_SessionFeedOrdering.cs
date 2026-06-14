namespace LowlandTech.Components.Tests.SPEC1078.Session.US01.Blocks;

[Trait(Spec.SPEC, "1078")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "07")]
[UserStory("01", "Session feed workspaces restored messages and context blocks predictably")]
public sealed class SC07_SessionFeedOrdering : ViewScenario
{
    private static readonly SessionNodeInput WorkspacesNode =
        new("workspaces-session", "session", "Workspaces");

    private static readonly SessionListEntry WorkspacesEntry =
        new("workspaces-session", "Workspaces");

    [Fact(DisplayName = "UAC01: RestoreSessionCommandHistory assigns increasing feed sequences to commands")]
    [Trait(Spec.UAC, "01")]
    [Given("two commands restored in list order")]
    [When("RestoreSessionCommandHistory reduces")]
    [Then("the later command receives a higher feed sequence than the earlier one")]
    public void UAC01_RestoredCommandsReceiveFeedSequences()
    {
        var state = SessionBlockDefaults.Empty;
        var sessionId = "workspaces-session";

        var first = new SessionCommandSubmitted(WorkspacesNode, "ask", "First") { Id = "cmd-a" };
        var second = new SessionCommandSubmitted(WorkspacesNode, "ask", "Second") { Id = "cmd-b" };

        state = SessionBlockReducer.Reduce(state, new SetActiveSession(sessionId));
        state = SessionBlockReducer.Reduce(state, new RestoreSessionCommandHistory(sessionId, new[] { first, second }));

        var firstKey = SessionStateService.GetFeedItemKey(sessionId, "command", first.Id);
        var secondKey = SessionStateService.GetFeedItemKey(sessionId, "command", second.Id);

        state.FeedSequenceByItemKey.ShouldContainKey(firstKey);
        state.FeedSequenceByItemKey.ShouldContainKey(secondKey);

        var firstSequence = state.FeedSequenceByItemKey[firstKey];
        var secondSequence = state.FeedSequenceByItemKey[secondKey];

        secondSequence.ShouldBeGreaterThan(firstSequence);
    }

    [Fact(DisplayName = "UAC02: Feed items order chronologically by assigned sequence")]
    [Trait(Spec.UAC, "02")]
    [Given("a response restored before a context block")]
    [When("GetFeedItems returns the ordered list")]
    [Then("the response appears before the block because it has the lower sequence")]
    public void UAC02_FeedItemsOrderChronologicallyBySequence()
    {
        var service = new SessionStateService();
        service.SelectSession(WorkspacesNode, new[] { WorkspacesEntry });

        var block = new SessionContextBlockModel("context-block:test", WorkspacesNode);
        var command = new SessionCommandSubmitted(WorkspacesNode, "ask", "Hello")
        {
            Id = "cmd-c",
            Response = "Done"
        };

        // Command is restored first, so it receives the lower sequence.
        service.Dispatch(new RestoreSessionCommandHistory(WorkspacesNode.Id, new[] { command }));

        // Block is restored afterward, so it receives a higher sequence and sorts after the response.
        service.Dispatch(new RestoreSessionUiState(new SessionUiState(
            ActiveAgentTypeKey: "ask",
            Input: string.Empty,
            SelectedNode: WorkspacesNode,
            OpenNodes: new[] { WorkspacesNode },
            ContextBlocks: new[] { block })));

        var feed = service.GetFeedItems(new[] { WorkspacesEntry });

        feed.Count.ShouldBe(2);
        feed[0].Kind.ShouldBe(SessionFeedItemKind.Response);
        feed[1].Kind.ShouldBe(SessionFeedItemKind.Block);
    }
}
