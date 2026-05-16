namespace LowlandTech.Components;

public sealed class TreeViewExpandEventArgs<TItem> : EventArgs
{
    public required TItem Item { get; init; }
    public required string Key { get; init; }
    public required bool Expanded { get; init; }
}
