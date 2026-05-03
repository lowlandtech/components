using LowlandTech.Core.Graph;
using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Feed;

public interface IBlock
{
    string? NodeId { get; set; }
    string? Name { get; set; }
    string? ParentNodeId { get; set; }
    Guid MessageId { get; set; }
    Guid BlockId { get; set; }
    EventCallback<Guid> OnDeleted { get; set; }
    bool SupportsActivationToggle { get; }

    /// <summary>
    /// Validation contract. Override to expose the block's current validity
    /// state — the host (session, sibling blocks) reads it through
    /// <c>BlockContext</c> without knowing how the block computes it. Default
    /// is <c>true</c> (a block with no validation rules is always valid).
    /// </summary>
    bool IsValid { get; }

    /// <summary>
    /// Semantic actions the block currently supports. This is block-local state
    /// derived from the block's own reducer or view model.
    /// </summary>
    IReadOnlyList<BlockIntent> IntentSurface { get; }

    /// <summary>
    /// Maps a semantic intent key plus optional payload into the block's own
    /// store actions. Hosts call this boundary by <see cref="BlockId"/>; they do
    /// not dispatch foreign reducer actions directly.
    /// </summary>
    Task<BlockIntentDispatchResult> DispatchIntentAsync(
        string key,
        IReadOnlyDictionary<string, object?>? payload,
        CancellationToken ct = default);
}
