namespace LowlandTech.Components.Tests.SPEC1078.Session.US01.Blocks;

[Trait(Spec.SPEC, "1078")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "06")]
[UserStory("01", "Session feed keeps streaming responses in view")]
public sealed class SC06_SessionStreamingScroll : ViewScenario
{
    private static readonly SessionNodeInput OrdersNode =
        new("orders-session", "session", "Orders");

    private static readonly SessionListEntry OrdersEntry =
        new("orders-session", "Orders");

    [Fact(DisplayName = "UAC01: SessionBlock scrolls the streaming response into view after render")]
    [Trait(Spec.UAC, "01")]
    [Given("a session with a response that has data-streaming=true")]
    [When("SessionBlock completes a render while the command is streaming")]
    [Then("it invokes JS that scrolls the streaming response element into view")]
    public void UAC01_StreamingResponseScrollsIntoView()
    {
        var service = new SessionStateService();
        service.SelectSession(OrdersNode, new[] { OrdersEntry });

        var command = new SessionCommandSubmitted(OrdersNode, "ask", "Hello")
        {
            Id = "session-command:test-stream",
            Response = "Typing...",
            IsStreaming = true
        };

        service.Dispatch(new SetSessionCommandHistory(OrdersNode.Id, new[] { command }));

        var cut = Ctx.Render<SessionBlock>(p => p
            .Add(x => x.StateService, service)
            .Add(x => x.Sessions, new[] { OrdersEntry }));

        cut.WaitForAssertion(() =>
        {
            Ctx.JSInterop.Invocations.Any(invocation =>
                invocation.Identifier == "eval"
                && invocation.Arguments.Count > 0
                && invocation.Arguments[0] is string script
                && script.Contains("session-turn-response-text", StringComparison.Ordinal)
                && script.Contains("data-streaming", StringComparison.Ordinal)
                && script.Contains("scrollTo", StringComparison.Ordinal))
                .ShouldBeTrue();
        }, TimeSpan.FromSeconds(1));
    }
}
