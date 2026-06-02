namespace LowlandTech.Components.Tests.SPEC1067.TreeView.US09.DeterministicKeys;

[Trait(Spec.SPEC, "1067")]
[Trait(Spec.US, "09")]
[Trait(Spec.SC, "01")]
[UserStory("09", "TreeView deterministic GuidV5 node keys")]
public class SC01_GuidV5 : ViewScenario
{
    private record Node(string Name, IReadOnlyList<Node> Children);

    private static Node Root(string name, params Node[] children) => new(name, children);

    // Render a tree without GetKey so the GuidV5 fallback is exercised.
    private IRenderedComponent<TreeView<Node>> RenderTree(Node[] roots, string? getText = null) =>
        Ctx.Render<TreeView<Node>>(p => p
            .Add(c => c.Items, roots)
            .Add(c => c.GetChildren, n => n.Children)
            .Add(c => c.GetText, n => n.Name));

    [Fact(DisplayName = "UAC01: same label at same path produces the same key across renders")]
    [Trait(Spec.UAC, "01")]
    [Given("a tree rendered twice with the same item instances and no explicit GetKey")]
    [When("the data-tree-node attribute is read from the root node in both renders")]
    [Then("both keys are identical")]
    public void UAC01_SamePathSameKey()
    {
        var roots = new[] { Root("src", Root("components")) };
        var cut1 = RenderTree(roots);
        var cut2 = RenderTree(roots);

        var key1 = cut1.Find("[data-tree-node]").GetAttribute("data-tree-node");
        var key2 = cut2.Find("[data-tree-node]").GetAttribute("data-tree-node");

        key1.ShouldNotBeNullOrEmpty();
        key1.ShouldBe(key2);
    }

    [Fact(DisplayName = "UAC02: different labels produce different keys")]
    [Trait(Spec.UAC, "02")]
    [Given("two trees with root nodes whose labels differ")]
    [When("the data-tree-node key of each root is read")]
    [Then("the two keys are not equal")]
    public void UAC02_DifferentLabelsDifferentKeys()
    {
        var rootsA = new[] { Root("alpha") };
        var rootsB = new[] { Root("beta") };

        var cutA = RenderTree(rootsA);
        var cutB = RenderTree(rootsB);

        var keyA = cutA.Find("[data-tree-node]").GetAttribute("data-tree-node");
        var keyB = cutB.Find("[data-tree-node]").GetAttribute("data-tree-node");

        keyA.ShouldNotBe(keyB);
    }

    [Fact(DisplayName = "UAC03: sibling nodes with the same label but different parent paths get distinct keys")]
    [Trait(Spec.UAC, "03")]
    [Given("two parent nodes each containing a child with the identical label 'lib'")]
    [When("both parents are expanded and all data-tree-node keys are collected")]
    [Then("all four keys are distinct")]
    public void UAC03_SameLabelDifferentParentDistinctKeys()
    {
        // Two different parents each having a child named "lib"
        var roots = new[] { Root("src", Root("lib")), Root("components", Root("lib")) };
        var cut = RenderTree(roots);

        // Expand both parents so both "lib" nodes are rendered
        cut.Find("[data-tree-node] .lt-tree-node-toggle").Click(); // expand "src"
        cut.FindAll(".lt-tree-node-toggle")[1].Click();            // expand "components"
        cut.Render();

        var allKeys = cut.FindAll("[data-tree-node]")
            .Select(e => e.GetAttribute("data-tree-node"))
            .ToList();

        // Should have 4 distinct keys: src, components, src/lib, components/lib
        allKeys.Distinct().Count().ShouldBe(4);
    }

    [Fact(DisplayName = "UAC04: key is a valid 32-char hex string (no dashes) for predictable localStorage keys")]
    [Trait(Spec.UAC, "04")]
    [Given("a tree with a single root node and no explicit GetKey")]
    [When("the data-tree-node attribute value is read")]
    [Then("the value is a 32-character lowercase hexadecimal string with no dashes")]
    public void UAC04_KeyIsHexWithoutDashes()
    {
        var cut = RenderTree(new[] { Root("root") });
        var key = cut.Find("[data-tree-node]").GetAttribute("data-tree-node");

        key.ShouldNotBeNullOrEmpty();
        key!.Length.ShouldBe(32);
        key.ShouldMatch("^[0-9a-f]{32}$");
    }

    [Fact(DisplayName = "UAC05: explicit GetKey parameter overrides GuidV5 fallback")]
    [Trait(Spec.UAC, "05")]
    [Given("a tree configured with an explicit GetKey delegate")]
    [When("the tree is rendered")]
    [Then("the node key matches the value returned by GetKey, not a GuidV5 hash")]
    public void UAC05_ExplicitGetKeyOverrides()
    {
        var roots = new[] { Root("anything") };
        var cut = Ctx.Render<TreeView<Node>>(p => p
            .Add(c => c.Items, roots)
            .Add(c => c.GetChildren, n => n.Children)
            .Add(c => c.GetKey, n => "explicit-" + n.Name)
            .Add(c => c.GetText, n => n.Name));

        cut.Find("[data-tree-node='explicit-anything']").ShouldNotBeNull();
    }
}
