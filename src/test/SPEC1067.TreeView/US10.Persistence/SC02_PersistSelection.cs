namespace LowlandTech.Components.Tests.SPEC1067.TreeView.US10.Persistence;

[Trait(Spec.SPEC, "1067")]
[Trait(Spec.US, "10")]
[Trait(Spec.SC, "02")]
[UserStory("10", "TreeView PersistKey selection persistence via localStorage")]
public class SC02_PersistSelection : JsViewScenario
{
    private record Node(string Id, string Name, IReadOnlyList<Node> Children);

    private static readonly Node[] Roots =
    [
        new("a", "Alpha", [new("a1", "Alpha-1", [])]),
        new("b", "Beta",  [])
    ];

    private IRenderedComponent<TreeView<Node>> RenderTree(
        string? persistKey = null,
        bool noPersist = false,
        TreeViewSelectionMode selectionMode = TreeViewSelectionMode.Single,
        IReadOnlyList<string>? selectedKeys = null) =>
        Ctx.Render<TreeView<Node>>(p =>
        {
            p.Add(c => c.Items, Roots)
             .Add(c => c.GetChildren, n => n.Children)
             .Add(c => c.GetKey, n => n.Id)
             .Add(c => c.HasChildren, n => n.Children.Count > 0)
             .Add(c => c.SelectionMode, selectionMode)
             .Add(c => c.PersistKey, persistKey)
             .Add(c => c.NoPersist, noPersist);
            if (selectedKeys != null)
                p.Add(c => c.SelectedKeys, selectedKeys);
        });

    [Fact(DisplayName = "UAC01: selecting a node calls localStorage.setItem with 'treeview:{key}:selected'")]
    [Trait(Spec.UAC, "01")]
    [Given("a tree rendered with PersistKey='test-tree' and Single selection mode")]
    [When("node 'a' is clicked")]
    [Then("localStorage.setItem is called with key 'treeview:test-tree:selected' containing '\"a\"'")]
    public void UAC01_SelectionWritesToLocalStorage()
    {
        var cut = RenderTree(persistKey: "test-tree");

        cut.Find("[data-tree-node='a']").Click();

        var setItemCalls = Ctx.JSInterop.Invocations
            .Where(i => i.Identifier == "localStorage.setItem")
            .ToList();

        setItemCalls.ShouldNotBeEmpty();
        var call = setItemCalls.First(i => i.Arguments[0]?.ToString()?.Contains(":selected") == true);
        call.Arguments[0].ShouldBe("treeview:test-tree:selected");
        var json = call.Arguments[1]?.ToString();
        json.ShouldNotBeNullOrEmpty();
        json.ShouldContain("\"a\"");
    }

    [Fact(DisplayName = "UAC02: deselecting a node removes it from the persisted selection")]
    [Trait(Spec.UAC, "02")]
    [Given("a tree with PersistKey='test-tree' in Toggle mode")]
    [When("node 'a' is clicked twice")]
    [Then("the last localStorage.setItem call for ':selected' contains an empty array")]
    public void UAC02_DeselectionUpdatesLocalStorage()
    {
        var cut = RenderTree(persistKey: "test-tree", selectionMode: TreeViewSelectionMode.Toggle);

        cut.Find("[data-tree-node='a']").Click();
        cut.Find("[data-tree-node='a']").Click();

        var selectedCalls = Ctx.JSInterop.Invocations
            .Where(i => i.Identifier == "localStorage.setItem"
                     && i.Arguments[0]?.ToString()?.EndsWith(":selected") == true)
            .ToList();

        selectedCalls.ShouldNotBeEmpty();
        var lastSelectedCall = selectedCalls.Last();

        var keys = JsonSerializer.Deserialize<List<string>>(lastSelectedCall.Arguments[1]?.ToString()!);
        keys.ShouldBeEmpty();
    }

    [Fact(DisplayName = "UAC03: no selected-key localStorage calls when NoPersist is set")]
    [Trait(Spec.UAC, "03")]
    [Given("a tree rendered with NoPersist=true in Single selection mode")]
    [When("node 'a' is clicked")]
    [Then("no localStorage.setItem call is made for a ':selected' key")]
    public void UAC03_NoPersistSkipsSelectedStorage()
    {
        var cut = RenderTree(noPersist: true);

        cut.Find("[data-tree-node='a']").Click();

        var selectedCalls = Ctx.JSInterop.Invocations
            .Where(i => i.Identifier == "localStorage.setItem"
                     && i.Arguments[0]?.ToString()?.EndsWith(":selected") == true)
            .ToList();

        selectedCalls.ShouldBeEmpty();
    }

    [Fact(DisplayName = "UAC04: controlled SelectedKeys parameter is not overwritten by PersistKey on first render")]
    [Trait(Spec.UAC, "04")]
    [Given("a tree with both PersistKey and a controlled SelectedKeys=['b'] parameter")]
    [When("the component renders for the first time")]
    [Then("node 'b' has the 'lt-tree-node-selected' class and node 'a' does not")]
    public void UAC04_ControlledSelectedKeysNotOverridden()
    {
        var cut = RenderTree(persistKey: "test-tree", selectedKeys: ["b"]);

        cut.Find("[data-tree-node='b']").ClassList.ShouldContain("lt-tree-node-selected");
        cut.Find("[data-tree-node='a']").ClassList.ShouldNotContain("lt-tree-node-selected");
    }

    [Fact(DisplayName = "UAC05: selected state is restored from localStorage on first render")]
    [Trait(Spec.UAC, "05")]
    [Given("localStorage contains '[\"a\"]' under 'treeview:test-tree:selected'")]
    [When("the tree renders for the first time with PersistKey='test-tree' and no controlled SelectedKeys")]
    [Then("node 'a' has the 'lt-tree-node-selected' class after the async restore")]
    public async Task UAC05_SelectionRehydratesFromLocalStorage()
    {
        Ctx.JSInterop.SetupModule("localStorage.getItem")
            .SetupVoid();

        Ctx.JSInterop.Setup<string?>("localStorage.getItem", "treeview:test-tree:selected")
            .SetResult("[\"a\"]");
        Ctx.JSInterop.Setup<string?>("localStorage.getItem", "treeview:test-tree:expanded")
            .SetResult(null);

        var cut = RenderTree(persistKey: "test-tree");

        // Trigger the async OnAfterRenderAsync by waiting
        await cut.InvokeAsync(() => Task.CompletedTask);
        cut.Render();

        cut.Find("[data-tree-node='a']").ClassList.ShouldContain("lt-tree-node-selected");
        cut.Find("[data-tree-node='b']").ClassList.ShouldNotContain("lt-tree-node-selected");
    }
}
