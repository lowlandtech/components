namespace LowlandTech.Components.Session.Agents;

/// <summary>Dummy ask agent for test mode — returns random static replies.</summary>
public sealed class DummyAskAgentGraphService : IAgentGraphService
{
    public string AgentTypeKey => "ask";

    public static readonly string[] TestReplies =
    [
        "Got it — here's what I found based on your request.",
        "Interesting question! Based on the context, I'd suggest reviewing the highlighted items.",
        "I've analyzed the data and here are the key takeaways.",
        "Quick summary: the main points are covered in the response below.",
        "Here's a breakdown of what you're looking for.",
        "Based on the pinned context, this seems to be the most relevant answer.",
        "I've cross-referenced the available data — here's what stands out.",
        "Short answer: yes, with the caveats noted in the details.",
        "The data points to a clear pattern. Here's my interpretation.",
        "I've synthesized the information — let me know if you need me to go deeper on any part."
    ];

    public Task ExecuteAsync(SessionContext context)
    {
        context.Response = TestReplies[Random.Shared.Next(TestReplies.Length)];
        return Task.CompletedTask;
    }
}
