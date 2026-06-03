namespace LowlandTech.Components;

/// <summary>
/// Reference-equality comparer for <typeparamref name="T"/> used by TreeView's key cache
/// so that structurally-identical but distinct item instances are stored as separate entries.
/// </summary>
internal sealed class TreeViewObjectKeyComparer : IEqualityComparer<object>
{
    public static readonly TreeViewObjectKeyComparer Instance = new();

    public new bool Equals(object? x, object? y)
    {
        if (ReferenceEquals(x, y))
            return true;

        if (x is null || y is null)
            return false;

        var xType = x.GetType();
        var yType = y.GetType();
        if (xType.IsValueType && yType.IsValueType)
            return x.Equals(y);

        return false;
    }

    public int GetHashCode(object obj)
        => obj.GetType().IsValueType ? obj.GetHashCode() : RuntimeHelpers.GetHashCode(obj);
}
