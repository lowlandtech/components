namespace LowlandTech.Components.Feed;

public enum BlockMode
{
    /// <summary>Only the block header bar is visible. Content is hidden.</summary>
    Collapsed,

    /// <summary>Standard in-flow rendering at natural height.</summary>
    Normal,

    /// <summary>Block grows to fill the available vertical space in the content panel.</summary>
    Expanded,

    /// <summary>Block floats as an absolute overlay within SessionContentPanel. The session grid remains visible.</summary>
    ContentOverlaid,

    /// <summary>Block covers the full application viewport as a modal overlay.</summary>
    AppOverlaid,
}
