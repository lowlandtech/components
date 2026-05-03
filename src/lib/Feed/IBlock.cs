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
}
