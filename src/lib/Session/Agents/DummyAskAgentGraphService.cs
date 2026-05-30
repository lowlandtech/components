namespace LowlandTech.Components.Session.Agents;

/// <summary>Dummy ask agent for test mode — returns random static replies and streaming chunks.</summary>
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

    public static async IAsyncEnumerable<string> StreamReplyAsync(string input, int delayMilliseconds = 35)
    {
        var reply = TestReplies[Random.Shared.Next(TestReplies.Length)];
        var prefix = string.IsNullOrWhiteSpace(input)
            ? string.Empty
            : $"You asked: {input.Trim()} ";

        foreach (var chunk in SplitIntoChunks($"{prefix}{reply}"))
        {
            if (delayMilliseconds > 0)
            {
                await Task.Delay(delayMilliseconds);
            }

            yield return chunk;
        }
    }

    private static IEnumerable<string> SplitIntoChunks(string value)
    {
        var words = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        for (var i = 0; i < words.Length; i++)
        {
            yield return i == 0 ? words[i] : $" {words[i]}";
        }
    }
}
