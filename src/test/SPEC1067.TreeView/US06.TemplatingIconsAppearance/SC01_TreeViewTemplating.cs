using Bunit;

namespace LowlandTech.Components.Tests.SPEC1067.TreeView.US06.TemplatingIconsAppearance;

[Trait(Spec.SPEC, "1067")]
[Trait(Spec.US, "06")]
[Trait(Spec.SC, "01")]
[UserStory("06", "TreeView Templating Icons and Appearance")]
public class SC01_TreeViewTemplating : ViewScenario
{
    private record Node(string Id, string Name, IReadOnlyList<Node> Children);

    private static readonly Node[] Roots =
    [
        new("a", "Alpha", [new("a1", "Alpha-1", [])]),
        new("b", "Beta", [])
    ];

    [Fact(DisplayName = "UAC01: ItemTemplate renders custom content")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_ItemTemplateRenders()
    {
        var cut = Ctx.Render<TreeView<Node>>(p => p
            .Add(c => c.Items, Roots)
            .Add(c => c.GetChildren, n => n.Children)
            .Add(c => c.GetKey, n => n.Id)
            .Add(c => c.HasChildren, n => n.Children.Count > 0)
            .Add(c => c.ItemTemplate, (TreeNodeContext<Node> ctx) => builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "class", "custom-item");
                builder.AddAttribute(2, "data-custom-key", ctx.Key);
                builder.AddContent(3, ctx.Item.Name);
                builder.CloseElement();
            }));

        cut.FindAll(".custom-item").Count.ShouldBe(2);
        cut.Find("[data-custom-key='a']").TextContent.ShouldBe("Alpha");
    }

    [Fact(DisplayName = "UAC02: EmptyContent renders when Items is empty")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_EmptyContentRenders()
    {
        var cut = Ctx.Render<TreeView<Node>>(p => p
            .Add(c => c.Items, Array.Empty<Node>())
            .Add(c => c.GetChildren, n => n.Children)
            .Add(c => c.GetKey, n => n.Id)
            .Add(c => c.EmptyContent, b =>
            {
                b.OpenElement(0, "div");
                b.AddAttribute(1, "class", "empty-message");
                b.AddContent(2, "No items");
                b.CloseElement();
            }));

        cut.FindAll(".empty-message").Count.ShouldBe(1);
    }

    [Fact(DisplayName = "UAC03: Dense parameter emits dense CSS class")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_DenseClass()
    {
        var cut = Ctx.Render<TreeView<Node>>(p => p
            .Add(c => c.Items, Roots)
            .Add(c => c.GetChildren, n => n.Children)
            .Add(c => c.GetKey, n => n.Id)
            .Add(c => c.Dense, true));

        var root = cut.Find("[data-slot='tree-view']");
        root.ClassList.ShouldContain("lt-tree-view-dense");
    }

    [Fact(DisplayName = "UAC04: Hover parameter emits hoverable CSS class")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_HoverClass()
    {
        var cut = Ctx.Render<TreeView<Node>>(p => p
            .Add(c => c.Items, Roots)
            .Add(c => c.GetChildren, n => n.Children)
            .Add(c => c.GetKey, n => n.Id)
            .Add(c => c.Hover, true));

        var root = cut.Find("[data-slot='tree-view']");
        root.ClassList.ShouldContain("lt-tree-view-hover");
    }

    [Fact(DisplayName = "UAC05: Class parameter is merged with root class")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_ClassMerged()
    {
        var cut = Ctx.Render<TreeView<Node>>(p => p
            .Add(c => c.Items, Roots)
            .Add(c => c.GetChildren, n => n.Children)
            .Add(c => c.GetKey, n => n.Id)
            .Add(c => c.Class, "my-custom-class"));

        var root = cut.Find("[data-slot='tree-view']");
        root.ClassList.ShouldContain("lt-tree-view");
        root.ClassList.ShouldContain("my-custom-class");
    }

    [Fact(DisplayName = "UAC06: GetText fallback uses ToString when not provided")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_GetTextFallback()
    {
        var cut = Ctx.Render<TreeView<Node>>(p => p
            .Add(c => c.Items, Roots)
            .Add(c => c.GetChildren, n => n.Children)
            .Add(c => c.GetKey, n => n.Id));

        // Default template uses ToString() when GetText is null
        var label = cut.Find("[data-tree-node='a'] .lt-tree-node-label");
        label.TextContent.ShouldContain("Alpha");
    }
}
