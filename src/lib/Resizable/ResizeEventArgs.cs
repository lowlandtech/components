namespace LowlandTech.Components.Resizable;

/// <summary>
/// Event arguments for resize operations.
/// </summary>
public class ResizeEventArgs : EventArgs
{
    /// <summary>
    /// The ID of the panel being resized.
    /// </summary>
    public string PanelId { get; set; } = string.Empty;

    /// <summary>
    /// The new size of the panel (percentage 0-100).
    /// </summary>
    public int Size { get; set; }

    /// <summary>
    /// The direction of the resizable group.
    /// </summary>
    public string Direction { get; set; } = "horizontal";

    /// <summary>
    /// The handle group identifier, if using grouped handles.
    /// </summary>
    public string? Group { get; set; }
}
