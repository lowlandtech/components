namespace LowlandTech.Components.Tests.SPEC1078.Session.US01.Blocks;

[Trait(Spec.SPEC, "1078")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "14")]
[UserStory("01", "Context tree supports keyboard navigation")]
public sealed class SC14_TreeKeyboardNavigation : ViewScenario
{
    [Fact(DisplayName = "UAC01: ArrowDown invokes the JS focusNext navigator")]
    [Trait(Spec.UAC, "01")]
    [Given("a SessionContextExplorer with focus on a node")]
    [When("the user presses ArrowDown")]
    [Then("the tree calls ltSessionContextTree.focusNext")]
    public void UAC01_ArrowDownCallsFocusNext()
    {
        SetupJs();
        var cut = RenderExplorer();

        cut.Find("[data-node-id='h1']").KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });

        Ctx.JSInterop.Invocations
            .Any(invocation => invocation.Identifier == "ltSessionContextTree.focusNext")
            .ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC02: ArrowUp invokes the JS focusPrevious navigator")]
    [Trait(Spec.UAC, "02")]
    [Given("a SessionContextExplorer with focus on a node")]
    [When("the user presses ArrowUp")]
    [Then("the tree calls ltSessionContextTree.focusPrevious")]
    public void UAC02_ArrowUpCallsFocusPrevious()
    {
        SetupJs();
        var cut = RenderExplorer();

        cut.Find("[data-node-id='h1']").KeyDown(new KeyboardEventArgs { Key = "ArrowUp" });

        Ctx.JSInterop.Invocations
            .Any(invocation => invocation.Identifier == "ltSessionContextTree.focusPrevious")
            .ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC03: ArrowRight expands a collapsed header node")]
    [Trait(Spec.UAC, "03")]
    [Given("a SessionContextExplorer with a collapsed loaded header")]
    [When("the user presses ArrowRight")]
    [Then("the header expands and its children become visible")]
    public void UAC03_ArrowRightExpandsCollapsedHeader()
    {
        SetupJs();
        var cut = RenderExplorer();

        cut.FindAll("[data-node-id='c1']").Count.ShouldBe(0);

        cut.Find("[data-node-id='h1']").KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });

        cut.FindAll("[data-node-id='c1']").Count.ShouldBeGreaterThan(0);
        Ctx.JSInterop.Invocations
            .Where(invocation => invocation.Identifier == "localStorage.setItem")
            .Select(invocation => invocation.Arguments[1]?.ToString())
            .Any(json => json?.Contains("\"h1\"", StringComparison.Ordinal) == true)
            .ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC04: ArrowRight on an already expanded header does nothing")]
    [Trait(Spec.UAC, "04")]
    [Given("a SessionContextExplorer with an expanded loaded header")]
    [When("the user presses ArrowRight")]
    [Then("the header stays expanded and no OnExpand callback fires")]
    public void UAC04_ArrowRightOnExpandedHeaderIsNoOp()
    {
        SetupJs(expandedNodeId: "h1");
        var fired = false;
        var cut = Ctx.Render<SessionContextExplorer>(p => p
            .Add(x => x.RootNodes, BuildRoot())
            .Add(x => x.OnLoadChildren, _ => { fired = true; return Task.CompletedTask; }));

        cut.FindAll("[data-node-id='c1']").Count.ShouldBeGreaterThan(0);

        cut.Find("[data-node-id='h1']").KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });

        cut.FindAll("[data-node-id='c1']").Count.ShouldBeGreaterThan(0);
        fired.ShouldBeFalse();
    }

    [Fact(DisplayName = "UAC05: ArrowLeft collapses an expanded header node")]
    [Trait(Spec.UAC, "05")]
    [Given("a SessionContextExplorer with an expanded loaded header")]
    [When("the user presses ArrowLeft")]
    [Then("the header collapses and its children are hidden")]
    public void UAC05_ArrowLeftCollapsesExpandedHeader()
    {
        SetupJs(expandedNodeId: "h1");
        var cut = Ctx.Render<SessionContextExplorer>(p => p
            .Add(x => x.RootNodes, BuildRoot()));

        cut.FindAll("[data-node-id='c1']").Count.ShouldBeGreaterThan(0);

        cut.Find("[data-node-id='h1']").KeyDown(new KeyboardEventArgs { Key = "ArrowLeft" });

        cut.FindAll("[data-node-id='c1']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC06: Enter on a node projects it")]
    [Trait(Spec.UAC, "06")]
    [Given("a SessionContextExplorer with focus on a node")]
    [When("the user presses Enter")]
    [Then("the OnProjected callback fires for that node")]
    public void UAC06_EnterProjectsNode()
    {
        SetupJs();
        SessionNodeInput? projected = null;
        var cut = Ctx.Render<SessionContextExplorer>(p => p
            .Add(x => x.RootNodes, BuildRoot())
            .Add(x => x.OnProjected, e => { projected = e.Node; return Task.CompletedTask; }));

        cut.Find("[data-node-id='h1']").KeyDown(new KeyboardEventArgs { Key = "Enter" });

        projected.ShouldNotBeNull();
        projected.Id.ShouldBe("h1");
    }

    private void SetupJs(string? expandedNodeId = null)
    {
        var stored = expandedNodeId is null
            ? null
            : $"[\"{expandedNodeId}\"]";

        Ctx.JSInterop.Setup<string?>("localStorage.getItem", _ => true).SetResult(stored);
        Ctx.JSInterop.SetupVoid("localStorage.setItem", _ => true);
    }

    private IRenderedComponent<SessionContextExplorer> RenderExplorer()
    {
        return Ctx.Render<SessionContextExplorer>(p => p
            .Add(x => x.RootNodes, BuildRoot()));
    }

    private static IReadOnlyList<SessionContextNode> BuildRoot()
    {
        var header = new SessionContextNode(
            new SessionNodeInput("h1", "header", "Workspaces"),
            new[] { new SessionContextNode(new SessionNodeInput("c1", "workspace", "Accounting"), IsLoaded: false) },
            true);

        return new[]
        {
            new SessionContextNode(
                new SessionNodeInput("user", "user-context", "User"),
                new[] { header },
                true)
        };
    }

    [Fact(DisplayName = "UAC07: keyboard JS module exports directional focus helpers")]
    [Trait(Spec.UAC, "07")]
    [Given("the sessionContextTree.js source")]
    [When("inspecting the module surface")]
    [Then("it defines sibling, child, and parent focus helpers")]
    public void UAC07_JsModuleExportsFocusFunctions()
    {
        var script = ReadEmbeddedResource("sessionContextTree.js");

        script.ShouldContain("ltSessionContextTree.focusNext");
        script.ShouldContain("ltSessionContextTree.focusPrevious");
        script.ShouldContain("ltSessionContextTree.focusFirstChild");
        script.ShouldContain("ltSessionContextTree.focusParent");
    }

    [Fact(DisplayName = "UAC08: keyboard JS module scopes navigation to the active explorer")]
    [Trait(Spec.UAC, "08")]
    [Given("the sessionContextTree.js source")]
    [When("inspecting the focus query logic")]
    [Then("it only considers buttons inside the closest [data-slot=session-context-explorer]")]
    public void UAC08_JsModuleScopesToActiveExplorer()
    {
        var script = ReadEmbeddedResource("sessionContextTree.js");

        script.ShouldContain("[data-slot=\"session-context-explorer\"]");
        script.ShouldContain("[data-slot=\"session-context-node\"]");
    }

    [Fact(DisplayName = "UAC09: keyboard JS module suppresses arrow-key default scrolling")]
    [Trait(Spec.UAC, "09")]
    [Given("the sessionContextTree.js source")]
    [When("inspecting the capture listener")]
    [Then("it calls preventDefault for Arrow keys on session-context-node buttons")]
    public void UAC09_JsModulePreventsDefaultArrowScrolling()
    {
        var script = ReadEmbeddedResource("sessionContextTree.js");

        script.ShouldContain("preventDefault");
        script.ShouldContain("event.key.startsWith('Arrow')");
    }

    [Fact(DisplayName = "UAC10: all app hosts include the sessionContextTree.js script")]
    [Trait(Spec.UAC, "10")]
    [Given("the host page sources")]
    [When("inspecting script tags")]
    [Then("each host references _content/LowlandTech.Components/js/sessionContextTree.js")]
    public void UAC10_HostPagesReferenceKeyboardScript()
    {
        ReadEmbeddedResource("web.App.razor").ShouldContain("sessionContextTree.js");
        ReadEmbeddedResource("shell.App.razor").ShouldContain("sessionContextTree.js");
        ReadEmbeddedResource("app.App.razor").ShouldContain("sessionContextTree.js");
        ReadEmbeddedResource("components.app.index.html").ShouldContain("sessionContextTree.js");
        ReadEmbeddedResource("desktop.index.html").ShouldContain("sessionContextTree.js");
    }

    [Fact(DisplayName = "UAC11: focus styling covers the whole node row")]
    [Trait(Spec.UAC, "11")]
    [Given("a SessionContextTreeNode renders")]
    [When("inspecting the row markup")]
    [Then("the row has a focus-within background class like the active/projected row")]
    public void UAC11_FocusStylingCoversWholeRow()
    {
        var cut = Ctx.Render<SessionContextTreeNode>(p => p
            .Add(x => x.Item, new SessionContextNode(new SessionNodeInput("n1", "workspace", "Orders"), IsLoaded: false)));

        var row = cut.Find("div[data-projected]");
        row.ClassList.ShouldContain("focus-within:bg-primary/5");
        row.ClassList.ShouldContain("data-[projected=true]:bg-primary/10");
    }

    [Fact(DisplayName = "UAC12: focused node button suppresses the default outline")]
    [Trait(Spec.UAC, "12")]
    [Given("a SessionContextTreeNode renders")]
    [When("inspecting the node button markup")]
    [Then("the button removes the default focus outline")]
    public void UAC12_FocusedNodeButtonRemovesDefaultOutline()
    {
        var cut = Ctx.Render<SessionContextTreeNode>(p => p
            .Add(x => x.Item, new SessionContextNode(new SessionNodeInput("n1", "workspace", "Orders"), IsLoaded: false)));

        var button = cut.Find("[data-slot='session-context-node']");
        button.ClassList.ShouldContain("focus:outline-none");
        button.ClassList.ShouldContain("focus-visible:outline-none");
    }

    [Fact(DisplayName = "UAC13: focused node row is styled distinctly from the active row")]
    [Trait(Spec.UAC, "13")]
    [Given("a SessionContextTreeNode renders")]
    [When("inspecting the row markup")]
    [Then("the row has data-focused background and ring classes")]
    public void UAC13_FocusedRowHasVisibleStyling()
    {
        var cut = Ctx.Render<SessionContextTreeNode>(p => p
            .Add(x => x.Item, new SessionContextNode(new SessionNodeInput("n1", "workspace", "Orders"), IsLoaded: false)));

        var row = cut.Find("[data-slot='session-context-node-row']");
        row.ClassList.ShouldContain("data-[focused=true]:bg-primary/15");
        row.ClassList.ShouldContain("data-[focused=true]:ring-1");
        row.ClassList.ShouldContain("data-[focused=true]:ring-primary/30");
        row.GetAttribute("data-focused").ShouldBe("false");
    }

    [Fact(DisplayName = "UAC14: focused node button shows a ring when its row is focused")]
    [Trait(Spec.UAC, "14")]
    [Given("a SessionContextTreeNode renders")]
    [When("inspecting the node button markup")]
    [Then("the button uses group-data-focused to show a ring")]
    public void UAC14_FocusedNodeButtonShowsRing()
    {
        var cut = Ctx.Render<SessionContextTreeNode>(p => p
            .Add(x => x.Item, new SessionContextNode(new SessionNodeInput("n1", "workspace", "Orders"), IsLoaded: false)));

        var button = cut.Find("[data-slot='session-context-node']");
        button.ClassList.ShouldContain("group-data-[focused=true]:ring-2");
        button.ClassList.ShouldContain("group-data-[focused=true]:ring-primary/70");
    }

    [Fact(DisplayName = "UAC15: node focus state controls the visible focused-row marker")]
    [Trait(Spec.UAC, "15")]
    [Given("a rendered context-tree node")]
    [When("its node button receives and loses browser focus")]
    [Then("the row's data-focused marker follows the browser focus state")]
    public void UAC15_NodeFocusStateTracksBrowserFocus()
    {
        var cut = Ctx.Render<SessionContextTreeNode>(p => p
            .Add(x => x.Item, new SessionContextNode(new SessionNodeInput("n1", "workspace", "Orders"), IsLoaded: false)));
        var button = cut.Find("[data-slot='session-context-node']");

        button.Focus();
        cut.Find("[data-slot='session-context-node-row']")
            .GetAttribute("data-focused")
            .ShouldBe("true");

        button.Blur();
        cut.Find("[data-slot='session-context-node-row']")
            .GetAttribute("data-focused")
            .ShouldBe("false");
    }

    [Fact(DisplayName = "UAC16: ArrowRight moves focus to the first child of an expanded node")]
    [Trait(Spec.UAC, "16")]
    [Given("an expanded context-tree node with a visible child")]
    [When("the user presses ArrowRight")]
    [Then("the tree asks the browser to focus the first child")]
    public void UAC16_ArrowRightOnExpandedNodeFocusesFirstChild()
    {
        SetupJs(expandedNodeId: "h1");
        var cut = RenderExplorer();

        cut.Find("[data-node-id='h1']").KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });

        Ctx.JSInterop.Invocations
            .Any(invocation => invocation.Identifier == "ltSessionContextTree.focusFirstChild")
            .ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC17: ArrowLeft moves focus to the parent of a collapsed child")]
    [Trait(Spec.UAC, "17")]
    [Given("a collapsed non-root context-tree node")]
    [When("the user presses ArrowLeft")]
    [Then("the tree asks the browser to focus its parent")]
    public void UAC17_ArrowLeftOnCollapsedNodeFocusesParent()
    {
        SetupJs(expandedNodeId: "h1");
        var cut = RenderExplorer();

        cut.Find("[data-node-id='c1']").KeyDown(new KeyboardEventArgs { Key = "ArrowLeft" });

        Ctx.JSInterop.Invocations
            .Any(invocation => invocation.Identifier == "ltSessionContextTree.focusParent")
            .ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC18: keyboard navigation does not activate the focused node")]
    [Trait(Spec.UAC, "18")]
    [Given("a context tree with a projected node")]
    [When("the user navigates from another node with ArrowDown")]
    [Then("focus navigation runs without projecting the navigated node")]
    public void UAC18_ArrowNavigationDoesNotProjectFocusedNode()
    {
        SetupJs();
        SessionNodeInput? projected = null;
        var cut = Ctx.Render<SessionContextExplorer>(p => p
            .Add(x => x.RootNodes, BuildRoot())
            .Add(x => x.OnProjected, e => { projected = e.Node; return Task.CompletedTask; }));

        cut.Find("[data-node-id='h1']").KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });

        projected.ShouldBeNull();
    }

    private static string ReadEmbeddedResource(string name)
    {
        var assembly = typeof(SC14_TreeKeyboardNavigation).Assembly;
        using var stream = assembly.GetManifestResourceStream(name);
        stream.ShouldNotBeNull($"Embedded resource '{name}' must exist.");
        using var reader = new StreamReader(stream!);
        return reader.ReadToEnd();
    }
}
