namespace LowlandTech.Components;

/// <summary>
/// Read-only context passed to <see cref="TreeView{TItem}.ItemTemplate"/> and
/// <see cref="TreeView{TItem}.BodyContent"/> for each rendered node.
/// </summary>
/// <typeparam name="TItem">The element type rendered by the tree.</typeparam>
public sealed record TreeNodeContext<TItem>
{
    public required TItem Item { get; init; }
    public required string Key { get; init; }
    public required int Depth { get; init; }
    public required bool IsExpanded { get; init; }
    public required bool HasChildren { get; init; }
    public required bool IsSelected { get; init; }
    public required bool IsChecked { get; init; }
    public required bool IsIndeterminate { get; init; }
    public required bool IsDisabled { get; init; }
    public required bool IsFocused { get; init; }

    /// <summary>Callback to toggle expansion of this node.</summary>
    public required Action ToggleExpand { get; init; }

    /// <summary>Callback to select/toggle selection of this node.</summary>
    public required Action Select { get; init; }

    /// <summary>Callback to toggle the checked state of this node.</summary>
    public required Action ToggleCheck { get; init; }
}
