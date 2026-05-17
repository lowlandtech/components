namespace LowlandTech.Components.App.Showcode.Examples.Navigation;

public static class TreeViewSampleData
{
    public static readonly IReadOnlyList<TreeViewFileItem> RootItems =
    [
        new("src", "src",
        [
            new("components", "components",
            [
                new("Button.razor", "Button.razor", []),
                new("Card.razor", "Card.razor", []),
                new("Dialog.razor", "Dialog.razor", [])
            ]),
            new("app", "app",
            [
                new("Program.cs", "Program.cs", [])
            ])
        ]),
        new("docs", "docs",
        [
            new("readme.md", "readme.md", [])
        ]),
        new("package.json", "package.json", [])
    ];

    public static readonly IReadOnlyList<string> ExpandedKeys = ["src"];

    public static string GetIcon(TreeViewFileItem item) =>
        item.Children.Count > 0 ? "folder" : "file";
}
