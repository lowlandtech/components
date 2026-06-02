namespace LowlandTech.Components;

/// <summary>
/// Reference-equality comparer for <typeparamref name="T"/> used by TreeView's key cache
/// so that structurally-identical but distinct item instances are stored as separate entries.
/// </summary>
internal sealed class TreeViewRefComparer<T> : IEqualityComparer<T>
{
    public static readonly TreeViewRefComparer<T> Instance = new();

    public bool Equals(T? x, T? y) => ReferenceEquals(x, y);
    public int GetHashCode(T? obj) => RuntimeHelpers.GetHashCode(obj!);
}
