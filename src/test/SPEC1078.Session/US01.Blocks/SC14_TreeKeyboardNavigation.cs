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
}
