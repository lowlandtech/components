namespace LowlandTech.Components;

public sealed class TreeViewSelectEventArgs<TItem> : EventArgs
{
    public required TItem Item { get; init; }
    public required string Key { get; init; }
    public required bool Selected { get; init; }
}
