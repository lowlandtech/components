namespace LowlandTech.Components.TreeView;

/// <summary>
/// Read-only context passed to <see cref="TreeView{TItem}.ItemTemplate"/> for
/// each rendered node. Templates use it to drive depth-aware indentation
/// (e.g. <c>tree-node depth-@ctx.Depth</c>) and chevron visibility.
/// </summary>
/// <typeparam name="TItem">The element type rendered by the tree.</typeparam>
/// <param name="Item">The bound item.</param>
/// <param name="Depth">Zero-indexed depth from the tree root.</param>
/// <param name="IsExpanded">Whether this node is currently expanded.</param>
/// <param name="HasChildren">Whether <c>GetChildren(Item)</c> returned a non-empty enumerable.</param>
public record TreeNodeContext<TItem>(TItem Item, int Depth, bool IsExpanded, bool HasChildren);
