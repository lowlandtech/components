namespace LowlandTech.Components.App.Showcode.Examples.Navigation;

public sealed record TreeViewFileItem(
    string Id,
    string Name,
    IReadOnlyList<TreeViewFileItem> Children);
