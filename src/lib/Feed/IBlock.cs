using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Feed;

public interface IBlock
{
    string? NodeId { get; set; }
    string? Name { get; set; }
    string? ParentNodeId { get; set; }
    Guid MessageId { get; set; }
    EventCallback<Guid> OnDeleted { get; set; }
    bool SupportsActivationToggle { get; }
}
