namespace LowlandTech.Components.Tests.SPEC1067.TreeView.US10.Persistence;

[Trait(Spec.SPEC, "1067")]
[Trait(Spec.US, "10")]
[Trait(Spec.SC, "01")]
[UserStory("10", "TreeView PersistKey expansion persistence via localStorage")]
public class SC01_PersistKey : JsViewScenario
{
    private record Node(string Id, string Name, IReadOnlyList<Node> Children);

    private static readonly Node[] Roots =
    [
        new("a", "Alpha", [new("a1", "Alpha-1", [])]),
        new("b", "Beta",  [new("b1", "Beta-1",  [])])
    ];

    private IRenderedComponent<TreeView<Node>> RenderTree(
        string? persistKey = null,
        bool noPersist = false,
        IReadOnlyList<string>? expandedKeys = null,
        EventCallback<IReadOnlyList<string>>? expandedKeysChanged = null) =>
        Ctx.Render<TreeView<Node>>(p =>
        {
            p.Add(c => c.Items, Roots)
             .Add(c => c.GetChildren, n => n.Children)
             .Add(c => c.GetKey, n => n.Id)
             .Add(c => c.HasChildren, n => n.Children.Count > 0)
             .Add(c => c.PersistKey, persistKey)
             .Add(c => c.NoPersist, noPersist);
            if (expandedKeys != null)
                p.Add(c => c.ExpandedKeys, expandedKeys);
            if (expandedKeysChanged.HasValue)
                p.Add(c => c.ExpandedKeysChanged, expandedKeysChanged.Value);
        });

    [Fact(DisplayName = "UAC01: expanding a node calls localStorage.setItem with the persist key")]
    [Trait(Spec.UAC, "01")]
    [Given("a tree rendered with PersistKey set to 'test-tree'")]
    [When("a node with children is expanded")]
    [Then("localStorage.setItem is called with key 'treeview:test-tree:expanded' containing the expanded node id")]
    public void UAC01_ExpandWritesToLocalStorage()
    {
        var cut = RenderTree(persistKey: "test-tree");

        cut.Find("[data-tree-node='a'] .lt-tree-node-toggle").Click();

        var setItemCalls = Ctx.JSInterop.Invocations
            .Where(i => i.Identifier == "localStorage.setItem")
            .ToList();

        setItemCalls.ShouldNotBeEmpty();
        var call = setItemCalls.First();
        call.Arguments[0].ShouldBe("treeview:test-tree:expanded");
        var json = call.Arguments[1]?.ToString();
        json.ShouldNotBeNullOrEmpty();
        json.ShouldContain("\"a\"");
    }

    [Fact(DisplayName = "UAC02: collapsing a node removes it from the persisted set")]
    [Trait(Spec.UAC, "02")]
    [Given("a tree with PersistKey set and node 'a' already expanded via ExpandedKeys")]
    [When("node 'a' is collapsed by clicking its toggle")]
    [Then("the last localStorage.setItem call does not contain 'a' in its payload")]
    public void UAC02_CollapseUpdatesLocalStorage()
    {
        var cut = RenderTree(persistKey: "test-tree", expandedKeys: ["a"]);

        cut.Find("[data-tree-node='a'] .lt-tree-node-toggle").Click();

        var setItemCalls = Ctx.JSInterop.Invocations
            .Where(i => i.Identifier == "localStorage.setItem")
            .ToList();

        setItemCalls.ShouldNotBeEmpty();
        var lastPayload = setItemCalls.Last().Arguments[1]?.ToString();
        lastPayload.ShouldNotBeNullOrEmpty();
        // "a" should no longer be in the persisted list
        var keys = JsonSerializer.Deserialize<List<string>>(lastPayload!);
        keys!.ShouldNotContain("a");
    }

    [Fact(DisplayName = "UAC03: no localStorage calls when NoPersist is set")]
    [Trait(Spec.UAC, "03")]
    [Given("a tree rendered with NoPersist=true")]
    [When("a node is expanded")]
    [Then("no localStorage.setItem calls are made")]
    public void UAC03_NoPersistSkipsLocalStorage()
    {
        var cut = RenderTree(noPersist: true);

        cut.Find("[data-tree-node='a'] .lt-tree-node-toggle").Click();

        var setItemCalls = Ctx.JSInterop.Invocations
            .Where(i => i.Identifier == "localStorage.setItem")
            .ToList();

        setItemCalls.ShouldBeEmpty();
    }

    [Fact(DisplayName = "UAC04: PersistKey storage key is scoped with 'treeview:' prefix")]
    [Trait(Spec.UAC, "04")]
    [Given("a tree with PersistKey set to 'my-nav-tree'")]
    [When("a node is expanded")]
    [Then("the localStorage key used is 'treeview:my-nav-tree:expanded'")]
    public void UAC04_StorageKeyIsPrefixed()
    {
        var cut = RenderTree(persistKey: "my-nav-tree");

        cut.Find("[data-tree-node='a'] .lt-tree-node-toggle").Click();

        var storageKey = Ctx.JSInterop.Invocations
            .Where(i => i.Identifier == "localStorage.setItem")
            .Select(i => i.Arguments[0]?.ToString())
            .FirstOrDefault();

        storageKey.ShouldBe("treeview:my-nav-tree:expanded");
    }

    [Fact(DisplayName = "UAC05: controlled ExpandedKeys parameter is not overwritten by PersistKey on first render")]
    [Trait(Spec.UAC, "05")]
    [Given("a tree with both PersistKey and a controlled ExpandedKeys=['b'] parameter")]
    [When("the component renders for the first time")]
    [Then("only node 'b1' is visible and node 'a1' remains hidden")]
    public void UAC05_ControlledExpandedKeysNotOverridden()
    {
        // ExpandedKeys is externally controlled — persistence should not stomp it
        var cut = RenderTree(persistKey: "test-tree", expandedKeys: ["b"]);

        // "b1" is visible because ExpandedKeys=["b"] was honoured
        cut.FindAll("[data-tree-node='b1']").Count.ShouldBe(1);
        // "a1" is NOT visible — the localStorage mock returns null (no stored state)
        cut.FindAll("[data-tree-node='a1']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC06: controlled ExpandedKeys rehydrate when the same list instance changes")]
    [Trait(Spec.UAC, "06")]
    [Given("a tree bound to a reusable ExpandedKeys list containing 'a'")]
    [When("the host rehydrates the same list instance with only 'b' and rerenders")]
    [Then("node 'b1' is visible and node 'a1' is hidden")]
    public void UAC06_ControlledExpandedKeysRehydrateWhenSameListChanges()
    {
        var expandedKeys = new List<string> { "a" };
        var cut = RenderTree(persistKey: "test-tree", expandedKeys: expandedKeys);

        expandedKeys.Clear();
        expandedKeys.Add("b");
        cut.Render(p => p.Add(c => c.ExpandedKeys, expandedKeys));

        cut.FindAll("[data-tree-node='b1']").Count.ShouldBe(1);
        cut.FindAll("[data-tree-node='a1']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC07: controlled empty ExpandedKeys restore from localStorage on reload")]
    [Trait(Spec.UAC, "07")]
    [Given("localStorage contains '[\"b\"]' and the host passes an empty controlled ExpandedKeys list")]
    [When("the tree renders for the first time with PersistKey='test-tree'")]
    [Then("node 'b1' is visible and ExpandedKeysChanged is fired with 'b'")]
    public async Task UAC07_ControlledEmptyExpandedKeysRestoreFromLocalStorage()
    {
        Ctx.JSInterop.Setup<string?>("localStorage.getItem", "treeview:test-tree:expanded")
            .SetResult("[\"b\"]");

        var captured = new List<IReadOnlyList<string>>();
        var cb = EventCallback.Factory.Create<IReadOnlyList<string>>(this, keys => captured.Add(keys));
        var cut = RenderTree(persistKey: "test-tree", expandedKeys: [], expandedKeysChanged: cb);

        await cut.InvokeAsync(() => Task.CompletedTask);
        cut.Render();

        cut.FindAll("[data-tree-node='b1']").Count.ShouldBe(1);
        captured.Count.ShouldBe(1);
        captured[0].ShouldContain("b");
    }

    [Fact(DisplayName = "UAC08: persisted ExpandedKeys override controlled defaults on reload")]
    [Trait(Spec.UAC, "08")]
    [Given("localStorage contains '[\"a\",\"b\"]' and the host passes controlled ExpandedKeys=['a']")]
    [When("the tree renders for the first time with PersistKey='test-tree'")]
    [Then("both 'a1' and 'b1' are visible and ExpandedKeysChanged includes 'b'")]
    public async Task UAC08_PersistedExpandedKeysOverrideControlledDefaults()
    {
        Ctx.JSInterop.Setup<string?>("localStorage.getItem", "treeview:test-tree:expanded")
            .SetResult("[\"a\",\"b\"]");

        var captured = new List<IReadOnlyList<string>>();
        var cb = EventCallback.Factory.Create<IReadOnlyList<string>>(this, keys => captured.Add(keys));
        var cut = RenderTree(persistKey: "test-tree", expandedKeys: ["a"], expandedKeysChanged: cb);

        await cut.InvokeAsync(() => Task.CompletedTask);
        cut.Render();

        cut.FindAll("[data-tree-node='a1']").Count.ShouldBe(1);
        cut.FindAll("[data-tree-node='b1']").Count.ShouldBe(1);
        captured.Count.ShouldBe(1);
        captured[0].ShouldContain("b");
    }
}
