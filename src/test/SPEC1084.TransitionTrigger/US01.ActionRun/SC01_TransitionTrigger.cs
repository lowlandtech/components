using LowlandTech.Components.Core.Documents;
using LowlandTech.Components.Core.Transitions;
using Microsoft.AspNetCore.Components.Web;

namespace LowlandTech.Components.Tests.SPEC1084.TransitionTrigger.US01.ActionRun;

using TriggerSut = LowlandTech.Components.TransitionTrigger;

[Trait(Spec.SPEC, "1084")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "01")]
[UserStory("01", "A declared action runs and its result lands on the document")]
public class SC01_TransitionTrigger : ViewScenario
{
    private static DocumentAction Action() => new(
        "approve",
        "Approve",
        Primary: true,
        Inputs: [new DocumentInput("note", "Note", Required: true)],
        TargetType: "document.approval");

    private static DocumentSnapshot Document() => new(
        "document.record",
        "3f2a9c11-0000-4000-8000-000000000007",
        "Cold open",
        "sha256:head",
        new DocumentDefinition("document.record",
        [
            new DocumentPropertyDefinition("headline", "Title"),
        ]),
        new Dictionary<string, string?> { ["headline"] = "The tide turns" },
        Actions: [Action()]);

    private static Func<string, string> Resolver() => key => key switch
    {
        "approve.saved" => "Approved.",
        "approve.refused" => "Not allowed.",
        "approve.conflict" => "The record moved.",
        "approve.retry" => "Try again",
        _ => key,
    };

    private static TransitionResult Applied() => new(TransitionOutcome.Applied,
    [
        new ResultCase("approve.saved", ResultSeverity.Success, MessageKey: "approve.saved")
    ], Document: "{\"ok\":true}", Head: "sha256:next");

    private sealed class FakeClient(Func<TransitionRequest, Task<TransitionResult>> respond) : ITransitionClient
    {
        public List<TransitionRequest> Requests { get; } = [];

        public Task<TransitionResult> ApplyAsync(TransitionRequest request, CancellationToken cancellationToken = default)
        {
            Requests.Add(request);
            return respond(request);
        }
    }

    [Fact(DisplayName = "UAC01: one request carries the head, the record, the transition and its inputs")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_RequestCarriesTheTripleAndInputs()
    {
        var client = new FakeClient(_ => Task.FromResult(Applied()));
        var cut = Ctx.Render<TriggerSut>(p => p
            .Add(x => x.Document, Document())
            .Add(x => x.Action, Action())
            .Add(x => x.Client, client)
            .Add(x => x.ResolveText, Resolver()));

        await cut.Find("[data-trigger-input='note']").InputAsync("Looks right");
        await cut.Find("[data-trigger-submit] button").ClickAsync(new MouseEventArgs());

        client.Requests.Count.ShouldBe(1);
        var request = client.Requests[0];
        request.Head.ShouldBe("sha256:head");
        request.SourceId.ShouldBe("3f2a9c11-0000-4000-8000-000000000007");
        request.EdgeKey.ShouldBe("approve");
        request.TargetType.ShouldBe("document.approval");
        request.Inputs!["note"].ShouldBe("Looks right");
    }

    [Fact(DisplayName = "UAC02: an applied result shows its cases and reaches the host")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_AppliedResultRenders()
    {
        TransitionResult? seen = null;
        var client = new FakeClient(_ => Task.FromResult(Applied()));
        var cut = Ctx.Render<TriggerSut>(p => p
            .Add(x => x.Document, Document())
            .Add(x => x.Action, Action())
            .Add(x => x.Client, client)
            .Add(x => x.ResolveText, Resolver())
            .Add(x => x.OnResult, (TransitionResult result) => seen = result));

        await cut.Find("[data-trigger-submit] button").ClickAsync(new MouseEventArgs());

        seen.ShouldNotBeNull();
        seen!.Outcome.ShouldBe(TransitionOutcome.Applied);
        seen.Document.ShouldBe("{\"ok\":true}");
        cut.Find("[data-trigger-result]").TextContent.ShouldContain("Approved.");
    }

    [Fact(DisplayName = "UAC03: a refusal shows its case and carries no document")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_RefusalRendersWithoutDocument()
    {
        TransitionResult? seen = null;
        var refused = new TransitionResult(TransitionOutcome.Refused,
        [
            new ResultCase("approve.refused", ResultSeverity.Error, MessageKey: "approve.refused")
        ]);
        var client = new FakeClient(_ => Task.FromResult(refused));

        var cut = Ctx.Render<TriggerSut>(p => p
            .Add(x => x.Document, Document())
            .Add(x => x.Action, Action())
            .Add(x => x.Client, client)
            .Add(x => x.ResolveText, Resolver())
            .Add(x => x.OnResult, (TransitionResult result) => seen = result));

        await cut.Find("[data-trigger-submit] button").ClickAsync(new MouseEventArgs());

        seen!.Outcome.ShouldBe(TransitionOutcome.Refused);
        seen.HasDocument.ShouldBeFalse();
        cut.Find("[data-trigger-result]").TextContent.ShouldContain("Not allowed.");
    }

    [Fact(DisplayName = "UAC04: a conflict shows the moved record and offers its action")]
    [Trait(Spec.UAC, "04")]
    public async Task UAC04_ConflictRendersRetry()
    {
        TransitionResult? seen = null;
        ResultAction? chosen = null;
        var conflict = new TransitionResult(TransitionOutcome.Conflict,
        [
            new ResultCase("approve.conflict", ResultSeverity.Warning, MessageKey: "approve.conflict",
                Actions: [new ResultAction("retry", "approve.retry", Primary: true)])
        ], Document: "{\"head\":\"sha256:moved\"}", Head: "sha256:moved");
        var client = new FakeClient(_ => Task.FromResult(conflict));

        var cut = Ctx.Render<TriggerSut>(p => p
            .Add(x => x.Document, Document())
            .Add(x => x.Action, Action())
            .Add(x => x.Client, client)
            .Add(x => x.ResolveText, Resolver())
            .Add(x => x.OnResult, (TransitionResult result) => seen = result)
            .Add(x => x.OnResultAction, (ResultAction action) => chosen = action));

        await cut.Find("[data-trigger-submit] button").ClickAsync(new MouseEventArgs());

        seen!.Outcome.ShouldBe(TransitionOutcome.Conflict);
        seen.Document.ShouldBe("{\"head\":\"sha256:moved\"}");
        cut.Find("[data-trigger-result]").TextContent.ShouldContain("The record moved.");

        await cut.Find("[data-result-actions='approve.conflict'] button").ClickAsync(new MouseEventArgs());
        chosen.ShouldNotBeNull();
        chosen!.Key.ShouldBe("retry");
    }

    [Fact(DisplayName = "UAC05: the result region is a polite live region and carries the outcome")]
    [Trait(Spec.UAC, "05")]
    public async Task UAC05_ResultRegionAnnounces()
    {
        var client = new FakeClient(_ => Task.FromResult(Applied()));
        var cut = Ctx.Render<TriggerSut>(p => p
            .Add(x => x.Document, Document())
            .Add(x => x.Action, Action())
            .Add(x => x.Client, client)
            .Add(x => x.ResolveText, Resolver()));

        var region = cut.Find("[data-trigger-result]");
        region.GetAttribute("role").ShouldBe("status");
        region.GetAttribute("aria-live").ShouldBe("polite");
        region.GetAttribute("tabindex").ShouldBe("-1");

        await cut.Find("[data-trigger-submit] button").ClickAsync(new MouseEventArgs());
        cut.Find("[data-trigger-result]").TextContent.ShouldContain("Approved.");
    }

    [Fact(DisplayName = "UAC06: a missing client refuses with one diagnostic")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_MissingClientRefuses()
    {
        var cut = Ctx.Render<TriggerSut>(p => p
            .Add(x => x.Document, Document())
            .Add(x => x.Action, Action()));

        cut.FindAll("[data-trigger-no-client]").Count.ShouldBe(1);
        cut.Find("[data-trigger-submit] button").HasAttribute("disabled").ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC07: an in-flight action cannot send a second request")]
    [Trait(Spec.UAC, "07")]
    public async Task UAC07_InFlightActionSendsOnce()
    {
        var gate = new TaskCompletionSource<TransitionResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        var client = new FakeClient(_ => gate.Task);
        var cut = Ctx.Render<TriggerSut>(p => p
            .Add(x => x.Document, Document())
            .Add(x => x.Action, Action())
            .Add(x => x.Client, client)
            .Add(x => x.ResolveText, Resolver()));

        var pending = cut.Find("[data-trigger-submit] button").ClickAsync(new MouseEventArgs());

        cut.WaitForAssertion(() => cut.Find("[data-slot='transition-trigger']").GetAttribute("data-state").ShouldBe("submitting"), TimeSpan.FromSeconds(2));
        cut.WaitForAssertion(() => cut.Find("[data-trigger-submit] button").HasAttribute("disabled").ShouldBeTrue(), TimeSpan.FromSeconds(2));

        client.Requests.Count.ShouldBe(1);

        gate.SetResult(Applied());
        await pending;

        cut.Find("[data-trigger-result]").TextContent.ShouldContain("Approved.");
        cut.Find("[data-trigger-submit] button").HasAttribute("disabled").ShouldBeFalse();
        cut.Find("[data-slot='transition-trigger']").GetAttribute("data-state").ShouldBe("idle");
    }
}
