namespace LowlandTech.Components.Tests.SPEC1067.TreeView.US10.Persistence;

[Trait(Spec.SPEC, "1067")]
[Trait(Spec.US, "10")]
[Trait(Spec.SC, "03")]
[UserStory("10", "TreeView PersistKey checked-state persistence via localStorage")]
public class SC03_PersistChecking : JsViewScenario
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
        TreeViewCheckMode checkMode = TreeViewCheckMode.Checkbox,
        IReadOnlyList<string>? checkedKeys = null,
        EventCallback<IReadOnlyList<string>>? checkedKeysChanged = null) =>
        Ctx.Render<TreeView<Node>>(p =>
        {
            p.Add(c => c.Items, Roots)
             .Add(c => c.GetChildren, n => n.Children)
             .Add(c => c.GetKey, n => n.Id)
             .Add(c => c.HasChildren, n => n.Children.Count > 0)
             .Add(c => c.CheckMode, checkMode)
             .Add(c => c.PersistKey, persistKey)
             .Add(c => c.NoPersist, noPersist);
            if (checkedKeys != null)
                p.Add(c => c.CheckedKeys, checkedKeys);
            if (checkedKeysChanged.HasValue)
                p.Add(c => c.CheckedKeysChanged, checkedKeysChanged.Value);
        });

    [Fact(DisplayName = "UAC01: checking a node writes to localStorage with 'treeview:{key}:checked'")]
    [Trait(Spec.UAC, "01")]
    [Given("a tree rendered with PersistKey='test-tree' and Checkbox check mode")]
    [When("node 'a' is checked")]
    [Then("localStorage.setItem is called with key 'treeview:test-tree:checked' containing '\"a\"'")]
    public void UAC01_CheckingWritesToLocalStorage()
    {
        var cut = RenderTree(persistKey: "test-tree");

        cut.Find("[data-tree-node='a'] .lt-tree-node-checkbox").Change(true);

        var call = Ctx.JSInterop.Invocations
            .Where(i => i.Identifier == "localStorage.setItem")
            .First(i => i.Arguments[0]?.ToString() == "treeview:test-tree:checked");

        var json = call.Arguments[1]?.ToString();
        json.ShouldNotBeNullOrEmpty();
        json.ShouldContain("\"a\"");
    }

    [Fact(DisplayName = "UAC02: controlled empty CheckedKeys restore from localStorage on reload")]
    [Trait(Spec.UAC, "02")]
    [Given("localStorage contains '[\"a\"]' and the host passes an empty controlled CheckedKeys list")]
    [When("the tree renders for the first time with PersistKey='test-tree'")]
    [Then("node 'a' is checked and CheckedKeysChanged is fired with 'a'")]
    public async Task UAC02_ControlledEmptyCheckedKeysRestoreFromLocalStorage()
    {
        Ctx.JSInterop.Setup<string?>("localStorage.getItem", "treeview:test-tree:expanded")
            .SetResult(null);
        Ctx.JSInterop.Setup<string?>("localStorage.getItem", "treeview:test-tree:checked")
            .SetResult("[\"a\"]");

        var captured = new List<IReadOnlyList<string>>();
        var cb = EventCallback.Factory.Create<IReadOnlyList<string>>(this, keys => captured.Add(keys));
        var cut = RenderTree(persistKey: "test-tree", checkedKeys: [], checkedKeysChanged: cb);

        await cut.InvokeAsync(() => Task.CompletedTask);
        cut.Render();

        cut.Find("[data-tree-node='a'] .lt-tree-node-checkbox")
            .GetAttribute("checked")
            .ShouldNotBeNull();
        captured.Count.ShouldBe(1);
        captured[0].ShouldContain("a");
    }

    [Fact(DisplayName = "UAC03: no checked-key localStorage calls when NoPersist is set")]
    [Trait(Spec.UAC, "03")]
    [Given("a tree rendered with NoPersist=true in Checkbox mode")]
    [When("node 'a' is checked")]
    [Then("no localStorage.setItem call is made for a ':checked' key")]
    public void UAC03_NoPersistSkipsCheckedStorage()
    {
        var cut = RenderTree(noPersist: true);

        cut.Find("[data-tree-node='a'] .lt-tree-node-checkbox").Change(true);

        var checkedCalls = Ctx.JSInterop.Invocations
            .Where(i => i.Identifier == "localStorage.setItem"
                     && i.Arguments[0]?.ToString()?.EndsWith(":checked") == true)
            .ToList();

        checkedCalls.ShouldBeEmpty();
    }
}
