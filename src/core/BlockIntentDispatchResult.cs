namespace LowlandTech.Core.Graph;

/// <summary>
/// Closed result type for <c>BlockBase.DispatchIntentAsync</c>. The block
/// executor returns one of three variants; the channel
/// (<c>BlockContext.DispatchIntentAsync</c>) wraps any thrown exception
/// into <see cref="Failure"/> so the caller never has to handle exceptions
/// — matches the effect-handler contract codified in US31 SC16 (every
/// failure surfaces as a typed value, never blocks the calling thread).
///
/// Pattern-match at the call site:
/// <code>
/// var r = await ctx.DispatchIntentAsync(blockId, "add");
/// switch (r)
/// {
///     case BlockIntentDispatchResult.Success:        /* commit */ break;
///     case BlockIntentDispatchResult.Failure f:      /* log f.Error */ break;
///     case BlockIntentDispatchResult.Unsupported u:  /* hide affordance for u.Key */ break;
/// }
/// </code>
/// </summary>
public abstract record BlockIntentDispatchResult
{
    private BlockIntentDispatchResult() { }

    /// <summary>The executor handled the intent and committed the transition.</summary>
    public sealed record Success : BlockIntentDispatchResult;

    /// <summary>The executor recognised the intent but failed to apply it.
    /// <paramref name="Error"/> is a human-readable message — keep it
    /// short; structured payloads are not part of this contract yet.</summary>
    public sealed record Failure(string Error) : BlockIntentDispatchResult;

    /// <summary>The executor (or no executor) does not handle <paramref name="Key"/>.
    /// Returned both when no block is registered for the requested
    /// <c>BlockId</c> AND when a block is registered but its
    /// <c>DispatchIntentAsync</c> override doesn't recognise the key.
    /// Callers should hide affordances or refuse the dispatch — this is
    /// not an error, it's a "not in your surface" signal.</summary>
    public sealed record Unsupported(string Key) : BlockIntentDispatchResult;
}
