using LowlandTech.Components.Core.Transitions;

namespace LowlandTech.Components.Tests.SPEC1082.TransitionResults.US01.ResultCases;

[Trait(Spec.SPEC, "1082")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "01")]
[UserStory("01", "A transition result carries its cases")]
public class SC01_ResultCases : ViewScenario
{
    private static Func<string, string> Resolver() =>
        key => key switch
        {
            "doc.title" => "Saved",
            "doc.message" => "The record {name} was saved.",
            "doc.refused" => "The record cannot be saved.",
            "doc.field" => "A country is required.",
            "doc.retry" => "Try again",
            _ => key,
        };

    [Fact(DisplayName = "UAC01: the root reports the outcome and each case its severity")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_OutcomeAndSeveritiesRender()
    {
        var result = new TransitionResult(TransitionOutcome.Refused,
        [
            new ResultCase("doc.refused", ResultSeverity.Error),
            new ResultCase("doc.applied", ResultSeverity.Success)
        ]);

        var cut = Ctx.Render<TransitionResultView>(p => p
            .Add(x => x.Result, result)
            .Add(x => x.ResolveText, Resolver()));

        cut.Find("[data-slot='transition-result']").GetAttribute("data-outcome").ShouldBe("refused");
        cut.Find("[data-result-case='doc.refused']").GetAttribute("data-severity").ShouldBe("error");
        cut.Find("[data-result-case='doc.applied']").GetAttribute("data-severity").ShouldBe("success");
    }

    [Fact(DisplayName = "UAC02: title and message resolve through the host and arguments substitute")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_TextResolvesAndArgumentsSubstitute()
    {
        var arguments = new Dictionary<string, string?> { ["name"] = "Ada" };
        var result = new TransitionResult(TransitionOutcome.Applied,
        [
            new ResultCase("doc.saved", ResultSeverity.Success, TitleKey: "doc.title", MessageKey: "doc.message", Arguments: arguments)
        ]);

        var cut = Ctx.Render<TransitionResultView>(p => p
            .Add(x => x.Result, result)
            .Add(x => x.ResolveText, Resolver()));

        cut.Find("[data-result-title='doc.saved']").TextContent.Trim().ShouldBe("Saved");
        cut.Find("[data-result-message='doc.saved']").TextContent.Trim().ShouldBe("The record Ada was saved.");
        cut.Markup.ShouldNotContain("doc.title");
        cut.Markup.ShouldNotContain("doc.message");
    }

    [Fact(DisplayName = "UAC03: a refusal shows its message against the field it names")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_FieldRefusalRendersAgainstItsField()
    {
        var fields = new Dictionary<string, string?> { ["country"] = "doc.field" };
        var result = new TransitionResult(TransitionOutcome.Refused,
        [
            new ResultCase("doc.refused", ResultSeverity.Error, MessageKey: "doc.refused", Fields: fields)
        ]);

        var cut = Ctx.Render<TransitionResultView>(p => p
            .Add(x => x.Result, result)
            .Add(x => x.ResolveText, Resolver()));

        cut.Find("[data-result-field='country']").TextContent.Trim().ShouldBe("A country is required.");
    }

    [Fact(DisplayName = "UAC04: offered actions raise their key; a disabled action raises nothing")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_OfferedActionsRaiseAndDisabledStaysQuiet()
    {
        ResultAction? chosen = null;
        var result = new TransitionResult(TransitionOutcome.Conflict,
        [
            new ResultCase("doc.conflict", ResultSeverity.Warning, TitleKey: "doc.title",
                Actions:
                [
                    new ResultAction("retry", "doc.retry", Primary: true),
                    new ResultAction("dismiss", "doc.retry", Disabled: true)
                ])
        ]);

        var cut = Ctx.Render<TransitionResultView>(p => p
            .Add(x => x.Result, result)
            .Add(x => x.ResolveText, Resolver())
            .Add(x => x.OnAction, (ResultAction action) => chosen = action));

        var buttons = cut.FindAll("[data-result-actions='doc.conflict'] button");
        buttons.Count.ShouldBe(2);
        buttons[1].HasAttribute("disabled").ShouldBeTrue();

        buttons[0].Click();
        chosen.ShouldNotBeNull();
        chosen!.Key.ShouldBe("retry");

        chosen = null;
        buttons[1].Click();
        chosen.ShouldBeNull();
    }

    [Fact(DisplayName = "UAC05: observed evidence stays hidden unless asked for")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_ObservedEvidenceIsOptIn()
    {
        var observed = new Dictionary<string, string?> { ["edge"] = "doc.contains", ["target"] = "doc-7" };
        var result = new TransitionResult(TransitionOutcome.Invalid,
        [
            new ResultCase("doc.problem", ResultSeverity.Info, TitleKey: "doc.title", Observed: observed)
        ]);

        var hidden = Ctx.Render<TransitionResultView>(p => p
            .Add(x => x.Result, result)
            .Add(x => x.ResolveText, Resolver()));

        hidden.FindAll("[data-result-observed='doc.problem']").Count.ShouldBe(0);
        hidden.Markup.ShouldNotContain("doc.contains");

        var shown = Ctx.Render<TransitionResultView>(p => p
            .Add(x => x.Result, result)
            .Add(x => x.ResolveText, Resolver())
            .Add(x => x.ShowObserved, true));

        shown.Find("[data-result-observed='doc.problem']").ShouldNotBeNull();
        shown.Markup.ShouldContain("doc.contains");
        shown.Markup.ShouldContain("doc-7");
    }

    [Fact(DisplayName = "UAC06: the document is carried, never parsed or rendered")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_DocumentStaysOpaque()
    {
        const string payload = "{\"properties\":{\"title\":\"Ada\"}}";
        var result = new TransitionResult(TransitionOutcome.Applied,
        [
            new ResultCase("doc.saved", ResultSeverity.Success, TitleKey: "doc.title")
        ], Document: payload, Head: "sha256:abc");

        var cut = Ctx.Render<TransitionResultView>(p => p
            .Add(x => x.Result, result)
            .Add(x => x.ResolveText, Resolver()));

        result.HasDocument.ShouldBeTrue();
        result.Document.ShouldBe(payload);
        result.Head.ShouldBe("sha256:abc");
        cut.Markup.ShouldNotContain("properties");
        cut.FindAll("[data-result-document]").Count.ShouldBe(0);
    }
}
