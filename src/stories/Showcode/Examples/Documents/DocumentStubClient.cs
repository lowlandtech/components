using LowlandTech.Components.Core.Transitions;

namespace LowlandTech.Components.Stories.Showcode.Examples.Documents;

/// <summary>
/// The scripted transport behind the document story. It stands in for a host's runtime so the
/// story can show applied, refused and conflict without a network, and so the browser acceptance
/// drives the components' real behavior rather than a mock of them.
/// </summary>
public sealed class DocumentStubClient : ITransitionClient
{
    /// <summary>Which outcome the next request answers with.</summary>
    public TransitionOutcome Next { get; set; } = TransitionOutcome.Applied;

    /// <summary>The last request the components sent.</summary>
    public TransitionRequest? LastRequest { get; private set; }

    public Task<TransitionResult> ApplyAsync(TransitionRequest request, CancellationToken cancellationToken = default)
    {
        LastRequest = request;

        return Task.FromResult(Next switch
        {
            TransitionOutcome.Refused => new TransitionResult(TransitionOutcome.Refused,
            [
                new ResultCase("document.refused", ResultSeverity.Error,
                    TitleKey: "document.refused.title", MessageKey: "document.refused",
                    Fields: new Dictionary<string, string?> { ["headline"] = "document.refused.field" })
            ]),
            TransitionOutcome.Conflict => new TransitionResult(TransitionOutcome.Conflict,
            [
                new ResultCase("document.conflict", ResultSeverity.Warning,
                    TitleKey: "document.conflict.title", MessageKey: "document.conflict",
                    Actions: [new ResultAction("retry", "document.retry", Primary: true)])
            ], Document: "{\"head\":\"sha256:moved\"}", Head: "sha256:moved"),
            _ => new TransitionResult(TransitionOutcome.Applied,
            [
                new ResultCase("document.applied", ResultSeverity.Success,
                    TitleKey: "document.applied.title", MessageKey: "document.applied")
            ], Document: "{\"head\":\"sha256:next\"}", Head: "sha256:next"),
        });
    }

    /// <summary>The words the story's host resolves, standing in for the app's resources.</summary>
    public static string Resolve(string key) => key switch
    {
        "document.applied.title" => "Saved",
        "document.applied" => "The record was saved.",
        "document.refused.title" => "Refused",
        "document.refused" => "The record cannot be saved.",
        "document.refused.field" => "The title is required.",
        "document.conflict.title" => "Moved",
        "document.conflict" => "The record moved while you were editing.",
        "document.retry" => "Try again",
        _ => key,
    };
}
