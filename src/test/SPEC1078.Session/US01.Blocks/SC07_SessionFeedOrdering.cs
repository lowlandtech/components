namespace LowlandTech.Components.Tests.SPEC1078.Session.US01.Blocks;

[Trait(Spec.SPEC, "1078")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "07")]
[UserStory("01", "Session feed orders restored messages and context blocks predictably")]
public sealed class SC07_SessionFeedOrdering : ViewScenario
{
    private static readonly SessionNodeInput OrdersNode =
        new("orders-session", "session", "Orders");

    private static readonly SessionListEntry OrdersEntry =
        new("orders-session", "Orders");

    [Fact(DisplayName = "UAC01: RestoreSessionCommandHistory assigns increasing feed sequences to commands")]
    [Trait(Spec.UAC, "01")]
    [Given("two commands restored in list order")]
    [When("RestoreSessionCommandHistory reduces")]
    [Then("the later command receives a higher feed sequence than the earlier one")]
    public void UAC01_RestoredCommandsReceiveFeedSequences()
    {
        var state = SessionAppDefaults.Empty;
        var sessionId = "orders-session";

        var first = new SessionCommandSubmitted(OrdersNode, "ask", "First") { Id = "cmd-a" };
        var second = new SessionCommandSubmitted(OrdersNode, "ask", "Second") { Id = "cmd-b" };

        state = SessionAppReducer.Reduce(state, new SetActiveSession(sessionId));
        state = SessionAppReducer.Reduce(state, new RestoreSessionCommandHistory(sessionId, new[] { first, second }));

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
        service.SelectSession(OrdersNode, new[] { OrdersEntry });

        var block = new SessionContextBlockModel("context-block:test", OrdersNode);
        var command = new SessionCommandSubmitted(OrdersNode, "ask", "Hello")
        {
            Id = "cmd-c",
            Response = "Done"
        };

        // Command is restored first, so it receives the lower sequence.
        service.Dispatch(new RestoreSessionCommandHistory(OrdersNode.Id, new[] { command }));

        // Block is restored afterward, so it receives a higher sequence and sorts after the response.
        service.Dispatch(new RestoreSessionUiState(new SessionUiState(
            ActiveAgentTypeKey: "ask",
            Input: string.Empty,
            SelectedNode: OrdersNode,
            OpenNodes: new[] { OrdersNode },
            ContextBlocks: new[] { block })));

        var feed = service.GetFeedItems(new[] { OrdersEntry });

        feed.Count.ShouldBe(2);
        feed[0].Kind.ShouldBe(SessionFeedItemKind.Response);
        feed[1].Kind.ShouldBe(SessionFeedItemKind.Block);
    }
}
