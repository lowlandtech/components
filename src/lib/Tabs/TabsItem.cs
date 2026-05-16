namespace LowlandTech.Components.Tabs;

public sealed record TabsItem
{
    public string Value { get; init; } = "";
    public string Title { get; init; } = "";
    public bool Disabled { get; init; }
    public bool Closeable { get; init; }
    public bool Pinned { get; init; }
    public bool Pinnable { get; init; }
    public bool Renameable { get; init; }
    public bool Draggable { get; init; } = true;
}

public sealed record TabsReorderEventArgs(string DraggedValue, string TargetValue, int OldIndex, int NewIndex);

public sealed record TabsRenameEventArgs(string Value, string OldTitle, string NewTitle);
