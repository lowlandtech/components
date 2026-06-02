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

    [Fact(DisplayName = "UAC04: Hover=true emits lt-tree-view-hover CSS class")]
    [Trait(Spec.UAC, "04")]
    [Given("a TreeView with Hover explicitly set to true")]
    [When("the component renders")]
    [Then("the root element has the 'lt-tree-view-hover' class")]
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

    [Fact(DisplayName = "UAC07: Hover defaults to true — hover state active without explicit parameter")]
    [Trait(Spec.UAC, "07")]
    [Given("a TreeView with no Hover parameter supplied")]
    [When("the component renders")]
    [Then("the root element has the 'lt-tree-view-hover' class by default")]
    public void UAC07_HoverDefaultsToTrue()
    {
        var cut = Ctx.Render<TreeView<Node>>(p => p
            .Add(c => c.Items, Roots)
            .Add(c => c.GetChildren, n => n.Children)
            .Add(c => c.GetKey, n => n.Id));

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

    [Fact(DisplayName = "UAC08: single click expands a node when ItemTemplate is used")]
    [Trait(Spec.UAC, "08")]
    [Given("a tree with a custom ItemTemplate and node 'a' collapsed")]
    [When("the outer node row for 'a' is single-clicked")]
    [Then("node 'a1' becomes visible (node 'a' is expanded)")]
    public void UAC08_ItemTemplateSingleClickExpands()
    {
        var cut = Ctx.Render<TreeView<Node>>(p => p
            .Add(c => c.Items, Roots)
            .Add(c => c.GetChildren, n => n.Children)
            .Add(c => c.GetKey, n => n.Id)
            .Add(c => c.HasChildren, n => n.Children.Count > 0)
            .Add(c => c.ItemTemplate, (TreeNodeContext<Node> ctx) => builder =>
            {
                builder.OpenElement(0, "span");
                builder.AddAttribute(1, "class", "custom-row");
                builder.AddContent(2, ctx.Item.Name);
                builder.CloseElement();
            }));

        // 'a1' should not be visible before expanding
        cut.FindAll("[data-tree-node='a1']").Count.ShouldBe(0);

        cut.Find("[data-tree-node='a']").Click();

        cut.FindAll("[data-tree-node='a1']").Count.ShouldBe(1);
    }

    [Fact(DisplayName = "UAC09: double click does not double-toggle when ItemTemplate is used")]
    [Trait(Spec.UAC, "09")]
    [Given("a tree with a custom ItemTemplate and node 'a' collapsed")]
    [When("the outer node row for 'a' is double-clicked")]
    [Then("node 'a' is expanded (single toggle only — not expanded then immediately collapsed)")]
    public void UAC09_ItemTemplateDoubleClickDoesNotDoubleToggle()
    {
        var cut = Ctx.Render<TreeView<Node>>(p => p
            .Add(c => c.Items, Roots)
            .Add(c => c.GetChildren, n => n.Children)
            .Add(c => c.GetKey, n => n.Id)
            .Add(c => c.HasChildren, n => n.Children.Count > 0)
            .Add(c => c.ItemTemplate, (TreeNodeContext<Node> ctx) => builder =>
            {
                builder.OpenElement(0, "span");
                builder.AddContent(1, ctx.Item.Name);
                builder.CloseElement();
            }));

        // Simulate double-click: browser fires onclick then ondblclick
        cut.Find("[data-tree-node='a']").Click();
        cut.Find("[data-tree-node='a']").DoubleClick();

        // After click+dblclick, should be expanded (click opened it, dblclick is suppressed)
        cut.FindAll("[data-tree-node='a1']").Count.ShouldBe(1);
    }

    [Fact(DisplayName = "UAC10: default template expands on double-click (no single-click expansion)")]
    [Trait(Spec.UAC, "10")]
    [Given("a tree using the default template with node 'a' collapsed")]
    [When("the outer row for 'a' is single-clicked (not on the toggle button)")]
    [Then("node 'a' remains collapsed — expansion requires the toggle button or double-click")]
    public void UAC10_DefaultTemplateSingleClickDoesNotExpand()
    {
        var cut = Ctx.Render<TreeView<Node>>(p => p
            .Add(c => c.Items, Roots)
            .Add(c => c.GetChildren, n => n.Children)
            .Add(c => c.GetKey, n => n.Id)
            .Add(c => c.HasChildren, n => n.Children.Count > 0));

        // Click the node row div itself (not the toggle button)
        cut.Find("[data-tree-node='a']").Click();

        // 'a1' should still not be visible — single-click row does not expand default template
        cut.FindAll("[data-tree-node='a1']").Count.ShouldBe(0);
    }
}
