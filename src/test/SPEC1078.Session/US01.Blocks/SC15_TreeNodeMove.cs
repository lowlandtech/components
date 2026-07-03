namespace LowlandTech.Components.Tests.SPEC1078.Session.US01.Blocks;

[Trait(Spec.SPEC, "1078")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "15")]
[UserStory("01", "Tree node move affordance delegates to host via SessionBlock")]
public sealed class SC15_TreeNodeMove : ViewScenario
{
    [Fact(DisplayName = "UAC01: SessionContextTreeNodeMenu Move Up fires MoveUpRequested")]
    [Trait(Spec.UAC, "01")]
    [Given("a SessionContextTreeNodeMenu with a node and a move-up callback")]
    [When("the Move up menu item is clicked")]
    [Then("MoveUpRequested fires with the node")]
    public void UAC01_MenuMoveUpFiresCallback()
    {
        SessionNodeInput? fired = null;
        var node = new SessionNodeInput("node-1", "workspace", "Foundry");

        var cut = Ctx.Render<SessionContextTreeNodeMenu>(p => p
            .Add(x => x.Node, node)
            .Add(x => x.CanMoveUp, true)
            .Add(x => x.MoveUpRequested, n => { fired = n; return Task.CompletedTask; }));

        var buttons = cut.FindAll(".node-menu-item").ToList();
        var moveUpButton = buttons.Single(b => b.TextContent.Trim().Equals("Move up", StringComparison.OrdinalIgnoreCase));
        moveUpButton.Click();

        fired.ShouldNotBeNull();
        fired.ShouldBe(node);
    }

    [Fact(DisplayName = "UAC02: SessionContextTreeNodeMenu Move Down is disabled when CanMoveDown is false")]
    [Trait(Spec.UAC, "02")]
    [Given("a SessionContextTreeNodeMenu with CanMoveDown false")]
    [When("the menu is rendered")]
    [Then("the Move down button is disabled")]
    public void UAC02_MenuMoveDownDisabledAtBoundary()
    {
        var node = new SessionNodeInput("node-1", "workspace", "Foundry");

        var cut = Ctx.Render<SessionContextTreeNodeMenu>(p => p
            .Add(x => x.Node, node)
            .Add(x => x.CanMoveDown, false));

        var buttons = cut.FindAll(".node-menu-item").ToList();
        var moveDownButton = buttons.Single(b => b.TextContent.Trim().Equals("Move down", StringComparison.OrdinalIgnoreCase));
        moveDownButton.HasAttribute("disabled").ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC03: SessionBlock forwards context node move-up to OnNodeMoveUpRequested")]
    [Trait(Spec.UAC, "03")]
    [Given("a SessionBlock with an active session and a context tree")]
    [When("the Move up option is chosen from a node's context menu")]
    [Then("OnNodeMoveUpRequested fires with the node")]
    public void UAC03_SessionBlockForwardsMoveUp()
    {
        SessionNodeInput? movedNode = null;
        var sessions = new[] { new SessionListEntry("s1", "Workspaces", 0, "session") };
        var stateService = new SessionStateService();
        stateService.SelectSession(new SessionNodeInput("s1", "session", "Workspaces"), sessions);

        var headerNode = new SessionNodeInput("ws-header", "workspaces.header", "Workspaces");
        var contextNodes = new[]
        {
            new SessionContextNode(headerNode, new[]
            {
                new SessionContextNode(new SessionNodeInput("foundry", "workspace", "Foundry"), IsLoaded: true),
                new SessionContextNode(new SessionNodeInput("acme", "workspace", "Acme"), IsLoaded: true)
            }, IsLoaded: true)
        };

        var cut = Ctx.Render<Components.SessionBlock>(p => p
            .Add(x => x.HasSidebar, true)
            .Add(x => x.StateService, stateService)
            .Add(x => x.Sessions, sessions)
            .Add(x => x.ContextNodes, contextNodes)
            .Add(x => x.OnNodeMoveUpRequested, n => { movedNode = n; return Task.CompletedTask; }));

        cut.Find("[data-testid='session-context-node-menu-trigger'][data-node-id='acme']").Click();

        var popoverService = Ctx.Services.GetRequiredService<IPopoverService>();
        var popover = popoverService.ActivePopovers.Single(reference => reference.State.Open);

        var fragmentCut = Ctx.Render(builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddContent(1, popover.State.Fragment);
            builder.CloseElement();
        });

        var buttons = fragmentCut.FindAll(".node-menu-item").ToList();
        var moveUpButton = buttons.Single(b => b.TextContent.Trim().Equals("Move up", StringComparison.OrdinalIgnoreCase));
        moveUpButton.Click();

        movedNode.ShouldNotBeNull();
        movedNode.Id.ShouldBe("acme");
    }

    [Fact(DisplayName = "UAC04: SessionBlock forwards context node move-down to OnNodeMoveDownRequested")]
    [Trait(Spec.UAC, "04")]
    [Given("a SessionBlock with an active session and a context tree")]
    [When("the Move down option is chosen from a node's context menu")]
    [Then("OnNodeMoveDownRequested fires with the node")]
    public void UAC04_SessionBlockForwardsMoveDown()
    {
        SessionNodeInput? movedNode = null;
        var sessions = new[] { new SessionListEntry("s1", "Workspaces", 0, "session") };
        var stateService = new SessionStateService();
        stateService.SelectSession(new SessionNodeInput("s1", "session", "Workspaces"), sessions);

        var headerNode = new SessionNodeInput("ws-header", "workspaces.header", "Workspaces");
        var contextNodes = new[]
        {
            new SessionContextNode(headerNode, new[]
            {
                new SessionContextNode(new SessionNodeInput("foundry", "workspace", "Foundry"), IsLoaded: true),
                new SessionContextNode(new SessionNodeInput("acme", "workspace", "Acme"), IsLoaded: true)
            }, IsLoaded: true)
        };

        var cut = Ctx.Render<Components.SessionBlock>(p => p
            .Add(x => x.HasSidebar, true)
            .Add(x => x.StateService, stateService)
            .Add(x => x.Sessions, sessions)
            .Add(x => x.ContextNodes, contextNodes)
            .Add(x => x.OnNodeMoveDownRequested, n => { movedNode = n; return Task.CompletedTask; }));

        cut.Find("[data-testid='session-context-node-menu-trigger'][data-node-id='foundry']").Click();

        var popoverService = Ctx.Services.GetRequiredService<IPopoverService>();
        var popover = popoverService.ActivePopovers.Single(reference => reference.State.Open);

        var fragmentCut = Ctx.Render(builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddContent(1, popover.State.Fragment);
            builder.CloseElement();
        });

        var buttons = fragmentCut.FindAll(".node-menu-item").ToList();
        var moveDownButton = buttons.Single(b => b.TextContent.Trim().Equals("Move down", StringComparison.OrdinalIgnoreCase));
        moveDownButton.Click();

        movedNode.ShouldNotBeNull();
        movedNode.Id.ShouldBe("foundry");
    }

    [Fact(DisplayName = "UAC05: SessionContextTreeNodeMenu Move Up is disabled when CanMoveUp is false")]
    [Trait(Spec.UAC, "05")]
    [Given("a SessionContextTreeNodeMenu with CanMoveUp false")]
    [When("the menu is rendered")]
    [Then("the Move up button is disabled")]
    public void UAC05_MenuMoveUpDisabledAtBoundary()
    {
        var node = new SessionNodeInput("node-1", "workspace", "Foundry");

        var cut = Ctx.Render<SessionContextTreeNodeMenu>(p => p
            .Add(x => x.Node, node)
            .Add(x => x.CanMoveUp, false));

        var buttons = cut.FindAll(".node-menu-item").ToList();
        var moveUpButton = buttons.Single(b => b.TextContent.Trim().Equals("Move up", StringComparison.OrdinalIgnoreCase));
        moveUpButton.HasAttribute("disabled").ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC06: SessionContextTreeNode propagates parentNodeId on move callbacks")]
    [Trait(Spec.UAC, "06")]
    [Given("a parent tree node with two expanded children")]
    [When("the second child's Move up option is chosen")]
    [Then("the callback includes the parent node id in Properties")]
    public void UAC06_NestedNodeMoveIncludesParentId()
    {
        SessionNodeInput? movedNode = null;
        var parentNode = new SessionNodeInput("parent-1", "workspaces.header", "Workspaces");
        var child1 = new SessionNodeInput("child-1", "workspace", "Foundry");
        var child2 = new SessionNodeInput("child-2", "workspace", "Acme");
        var contextNode = new SessionContextNode(parentNode, new[]
        {
            new SessionContextNode(child1, IsLoaded: true),
            new SessionContextNode(child2, IsLoaded: true)
        }, IsLoaded: true);

        var cut = Ctx.Render<SessionContextTreeNode>(p => p
            .Add(x => x.Item, contextNode)
            .Add(x => x.ExpandedNodeIds, new HashSet<string>(StringComparer.Ordinal) { parentNode.Id })
            .Add(x => x.OnNodeMoveUp, n => { movedNode = n; return Task.CompletedTask; }));

        cut.Find("[data-testid='session-context-node-menu-trigger'][data-node-id='child-2']").Click();

        var popoverService = Ctx.Services.GetRequiredService<IPopoverService>();
        var popover = popoverService.ActivePopovers.Single(reference => reference.State.Open);

        var fragmentCut = Ctx.Render(builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddContent(1, popover.State.Fragment);
            builder.CloseElement();
        });

        var buttons = fragmentCut.FindAll(".node-menu-item").ToList();
        var moveUpButton = buttons.Single(b => b.TextContent.Trim().Equals("Move up", StringComparison.OrdinalIgnoreCase));
        moveUpButton.Click();

        movedNode.ShouldNotBeNull();
        movedNode.Id.ShouldBe("child-2");
        movedNode.Properties.ShouldNotBeNull();
        movedNode.Properties.ShouldContainKey("parentNodeId");
        movedNode.Properties["parentNodeId"].ShouldBe(parentNode.Id);
    }

    [Fact(DisplayName = "UAC07: first sibling has Move up disabled and Move down enabled")]
    [Trait(Spec.UAC, "07")]
    [Given("a parent tree node with three expanded children")]
    [When("the first child's menu is opened")]
    [Then("Move up is disabled and Move down is enabled")]
    public void UAC07_FirstSiblingMoveUpDisabled()
    {
        var root = BuildThreeChildNode();

        var (moveUp, moveDown, popover) = OpenMoveMenu(root, "child-1");

        moveUp.HasAttribute("disabled").ShouldBeTrue();
        moveDown.HasAttribute("disabled").ShouldBeFalse();

        ClosePopover(popover);
    }

    [Fact(DisplayName = "UAC08: last sibling has Move up enabled and Move down disabled")]
    [Trait(Spec.UAC, "08")]
    [Given("a parent tree node with three expanded children")]
    [When("the last child's menu is opened")]
    [Then("Move up is enabled and Move down is disabled")]
    public void UAC08_LastSiblingMoveDownDisabled()
    {
        var root = BuildThreeChildNode();

        var (moveUp, moveDown, popover) = OpenMoveMenu(root, "child-3");

        moveUp.HasAttribute("disabled").ShouldBeFalse();
        moveDown.HasAttribute("disabled").ShouldBeTrue();

        ClosePopover(popover);
    }

    [Fact(DisplayName = "UAC09: middle sibling has both Move up and Move down enabled")]
    [Trait(Spec.UAC, "09")]
    [Given("a parent tree node with three expanded children")]
    [When("the middle child's menu is opened")]
    [Then("both Move up and Move down are enabled")]
    public void UAC09_MiddleSiblingBothEnabled()
    {
        var root = BuildThreeChildNode();

        var (moveUp, moveDown, popover) = OpenMoveMenu(root, "child-2");

        moveUp.HasAttribute("disabled").ShouldBeFalse();
        moveDown.HasAttribute("disabled").ShouldBeFalse();

        ClosePopover(popover);
    }

    [Fact(DisplayName = "UAC10: only sibling has both Move up and Move down disabled")]
    [Trait(Spec.UAC, "10")]
    [Given("a parent tree node with one expanded child")]
    [When("the only child's menu is opened")]
    [Then("both Move up and Move down are disabled")]
    public void UAC10_OnlySiblingBothDisabled()
    {
        var parentNode = new SessionNodeInput("parent-1", "workspaces.header", "Workspaces");
        var child = new SessionNodeInput("only-child", "workspace", "Foundry");
        var root = new SessionContextNode(parentNode, new[]
        {
            new SessionContextNode(child, IsLoaded: true)
        }, IsLoaded: true);

        var (moveUp, moveDown, popover) = OpenMoveMenu(root, "only-child");

        moveUp.HasAttribute("disabled").ShouldBeTrue();
        moveDown.HasAttribute("disabled").ShouldBeTrue();

        ClosePopover(popover);
    }

    private static SessionContextNode BuildThreeChildNode()
    {
        var parentNode = new SessionNodeInput("parent-1", "workspaces.header", "Workspaces");
        var child1 = new SessionNodeInput("child-1", "workspace", "Foundry");
        var child2 = new SessionNodeInput("child-2", "workspace", "Acme");
        var child3 = new SessionNodeInput("child-3", "workspace", "Globex");
        return new SessionContextNode(parentNode, new[]
        {
            new SessionContextNode(child1, IsLoaded: true),
            new SessionContextNode(child2, IsLoaded: true),
            new SessionContextNode(child3, IsLoaded: true)
        }, IsLoaded: true);
    }

    private (AngleSharp.Dom.IElement MoveUp, AngleSharp.Dom.IElement MoveDown, PopoverReference Popover) OpenMoveMenu(SessionContextNode root, string nodeId)
    {
        var cut = Ctx.Render<SessionContextTreeNode>(p => p
            .Add(x => x.Item, root)
            .Add(x => x.ExpandedNodeIds, new HashSet<string>(StringComparer.Ordinal) { root.Node.Id }));

        cut.Find($"[data-testid='session-context-node-menu-trigger'][data-node-id='{nodeId}']").Click();

        var popoverService = Ctx.Services.GetRequiredService<IPopoverService>();
        var popover = popoverService.ActivePopovers.Single(reference => reference.State.Open);

        var fragmentCut = Ctx.Render(builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddContent(1, popover.State.Fragment);
            builder.CloseElement();
        });

        var buttons = fragmentCut.FindAll(".node-menu-item").ToList();
        var moveUp = buttons.Single(b => b.TextContent.Trim().Equals("Move up", StringComparison.OrdinalIgnoreCase));
        var moveDown = buttons.Single(b => b.TextContent.Trim().Equals("Move down", StringComparison.OrdinalIgnoreCase));
        return (moveUp, moveDown, popover);
    }

    private void ClosePopover(PopoverReference popover)
    {
        var popoverService = Ctx.Services.GetRequiredService<IPopoverService>();
        popoverService.Update(popover.Id, new PopoverState { Open = false });
    }
}
