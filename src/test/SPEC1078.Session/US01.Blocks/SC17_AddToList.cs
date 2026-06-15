namespace LowlandTech.Components.Tests.SPEC1078.Session.US01.Blocks;

[Trait(Spec.SPEC, "1078")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "17")]
[UserStory("01", "Add non-list context nodes to lists")]
public sealed class SC17_AddToList : ViewScenario
{
    private const string UserContextId = "10000000-0000-0000-0000-000000000001";
    private const string ListsHeaderId = "10000000-0000-0000-0000-000000000002";
    private const string List1Id = "10000000-0000-0000-0000-000000000003";
    private const string List2Id = "10000000-0000-0000-0000-000000000004";
    private const string WorkspacesHeaderId = "10000000-0000-0000-0000-000000000005";
    private const string Flow1Id = "10000000-0000-0000-0000-000000000006";

    [Fact(DisplayName = "UAC01: Non-list node shows Add to list menu item")]
    [Trait(Spec.UAC, "01")]
    [Given("a SessionBlock with lists and a flow node")]
    [When("the user opens the node menu")]
    [Then("the Add to list item is visible")]
    public async Task UAC01_NonListNodeShowsAddToList()
    {
        var (cut, _) = RenderWithLists();

        var menu = await OpenNodeMenuAsync(cut, Flow1Id);

        var addToList = menu.FindAll("[data-testid='session-context-node-add-to-list']").FirstOrDefault();
        addToList.ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: List node does not show Add to list menu item")]
    [Trait(Spec.UAC, "02")]
    [Given("a SessionBlock with lists")]
    [When("the user opens a list node's menu")]
    [Then("the Add to list item is not visible")]
    public async Task UAC02_ListNodeHidesAddToList()
    {
        var (cut, _) = RenderWithLists();

        var menu = await OpenNodeMenuAsync(cut, List1Id);

        menu.FindAll("[data-testid='session-context-node-add-to-list']").ShouldBeEmpty();
    }

    [Fact(DisplayName = "UAC03: Add to list dialog shows the user's lists")]
    [Trait(Spec.UAC, "03")]
    [Given("a SessionBlock with lists and a flow node")]
    [When("the user clicks Add to list")]
    [Then("a dialog opens showing all lists")]
    public async Task UAC03_DialogShowsLists()
    {
        var (cut, _) = RenderWithLists();

        await OpenAddToListDialogAsync(cut, Flow1Id);

        cut.WaitForAssertion(() =>
        {
            cut.FindAll("[data-testid='add-to-list-candidate']").Count.ShouldBe(2);
        });
    }

    [Fact(DisplayName = "UAC04: Toggling a list on creates a list.contains edge")]
    [Trait(Spec.UAC, "04")]
    [Given("a SessionBlock with lists and a flow node")]
    [When("the user checks a list and saves")]
    [Then("the graph contains a list.contains edge from that list to the node")]
    public async Task UAC04_ToggleOnCreatesEdge()
    {
        var (cut, graphService) = RenderWithLists();

        await OpenAddToListDialogAsync(cut, Flow1Id);

        cut.WaitForAssertion(() => cut.FindAll("[data-testid='add-to-list-candidate']").Count.ShouldBeGreaterThan(0));
        var candidate = cut.FindAll("[data-testid='add-to-list-candidate']").First();
        cut.WaitForAssertion(() => candidate.QuerySelector("[data-slot='checkbox']").ShouldNotBeNull());
        var checkbox = candidate.QuerySelector("[data-slot='checkbox']");
        checkbox!.Click();

        var save = cut.Find("[data-testid='add-to-list-save']");
        await save.ClickAsync(new MouseEventArgs());

        var listId = candidate.GetAttribute("data-list-id");
        listId.ShouldNotBeNull();
        var flowId = Guid.Parse(Flow1Id);
        var listGuid = Guid.Parse(listId!);

        graphService.Edges.Any(e =>
            e.SourceNodeId == listGuid &&
            e.TargetNodeId == flowId).ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC05: Toggling a list off deletes the list.contains edge")]
    [Trait(Spec.UAC, "05")]
    [Given("a flow node already in a list")]
    [When("the user unchecks that list and saves")]
    [Then("the list.contains edge is removed")]
    public async Task UAC05_ToggleOffDeletesEdge()
    {
        var (cut, graphService) = RenderWithLists();
        var flowId = Guid.Parse(Flow1Id);
        var listId = Guid.Parse(List1Id);

        graphService.SeedEdge("list1-contains-flow1", "list.contains", listId, flowId);

        await OpenAddToListDialogAsync(cut, Flow1Id);

        cut.WaitForAssertion(() =>
        {
            cut.FindAll("[data-testid='add-to-list-candidate']")
                .FirstOrDefault(c => c.GetAttribute("data-list-id") == List1Id)
                .ShouldNotBeNull();
        });
        var candidate = cut.FindAll("[data-testid='add-to-list-candidate']")
            .First(c => c.GetAttribute("data-list-id") == List1Id);
        cut.WaitForAssertion(() => candidate.QuerySelector("[data-slot='checkbox']").ShouldNotBeNull());
        var checkbox = candidate.QuerySelector("[data-slot='checkbox']");
        checkbox!.Click();

        var save = cut.Find("[data-testid='add-to-list-save']");
        await save.ClickAsync(new MouseEventArgs());

        graphService.Edges.Any(e =>
            e.SourceNodeId == listId &&
            e.TargetNodeId == flowId).ShouldBeFalse();
    }

    private async Task<IRenderedComponent<Microsoft.AspNetCore.Components.IComponent>> OpenNodeMenuAsync(IRenderedComponent<Components.SessionBlock> cut, string nodeId)
    {
        var trigger = cut.Find($"[data-testid='session-context-node-menu-trigger'][data-node-id='{nodeId}']");
        await trigger.ClickAsync(new MouseEventArgs());

        var popoverService = Ctx.Services.GetRequiredService<IPopoverService>();
        var popover = popoverService.ActivePopovers.Single(reference => reference.State.Open);

        return Ctx.Render(builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddContent(1, popover.State.Fragment);
            builder.CloseElement();
        });
    }

    private async Task OpenAddToListDialogAsync(IRenderedComponent<Components.SessionBlock> cut, string nodeId)
    {
        var menu = await OpenNodeMenuAsync(cut, nodeId);

        var addToList = menu.Find("[data-testid='session-context-node-add-to-list']");
        await addToList.ClickAsync(new MouseEventArgs());
    }

    private (IRenderedComponent<Components.SessionBlock> Cut, FakeGraphService GraphService) RenderWithLists()
    {
        var userContextId = Guid.Parse(UserContextId);
        var listsHeaderId = Guid.Parse(ListsHeaderId);
        var list1Id = Guid.Parse(List1Id);
        var list2Id = Guid.Parse(List2Id);
        var workspacesHeaderId = Guid.Parse(WorkspacesHeaderId);
        var flow1Id = Guid.Parse(Flow1Id);

        var graphService = new FakeGraphService();
        graphService.SeedNodeType("flow");
        graphService.SeedEdgeType("has.flows", "user-context", "workspaces.header");
        graphService.SeedEdgeType("has.flow", "workspaces.header", "flow");

        graphService.SeedNode(userContextId, "user-context", "My Context");
        graphService.SeedNode(listsHeaderId, "lists", "Lists");
        graphService.SeedNode(list1Id, "list", "List 1");
        graphService.SeedNode(list2Id, "list", "List 2");
        graphService.SeedNode(workspacesHeaderId, "workspaces.header", "Flows");
        graphService.SeedNode(flow1Id, "flow", "Onboarding");

        graphService.SeedEdge("uc-lists", "has.lists", userContextId, listsHeaderId);
        graphService.SeedEdge("lists-list1", "has.list", listsHeaderId, list1Id);
        graphService.SeedEdge("lists-list2", "has.list", listsHeaderId, list2Id);
        graphService.SeedEdge("uc-flows", "has.flows", userContextId, workspacesHeaderId);
        graphService.SeedEdge("flows-flow1", "has.flow", workspacesHeaderId, flow1Id);

        var stateService = new SessionStateService(null, graphService);
        var sessions = new[] { new SessionListEntry("s1", "Workspaces", 0, "session") };
        stateService.SelectSession(new SessionNodeInput("s1", "session", "Workspaces"), sessions);
        stateService.LoadContextNodesAsync(userContextId, "My Context", Xunit.TestContext.Current.CancellationToken).GetAwaiter().GetResult();
        stateService.LoadChildrenAsync(new SessionNodeInput(ListsHeaderId, "lists", "Lists"), Xunit.TestContext.Current.CancellationToken).GetAwaiter().GetResult();
        stateService.LoadChildrenAsync(new SessionNodeInput(WorkspacesHeaderId, "workspaces.header", "Flows"), Xunit.TestContext.Current.CancellationToken).GetAwaiter().GetResult();

        var cut = Ctx.Render<Components.SessionBlock>(p => p
            .Add(x => x.HasSidebar, true)
            .Add(x => x.StateService, stateService)
            .Add(x => x.Sessions, sessions)
            .Add(x => x.ContextNodes, stateService.ContextNodes));

        foreach (var headerId in new[] { ListsHeaderId, WorkspacesHeaderId })
        {
            var chevron = cut.FindAll($"[data-node-id='{headerId}'] span.cursor-pointer").FirstOrDefault();
            chevron?.ClickAsync(new MouseEventArgs()).GetAwaiter().GetResult();
        }

        return (cut, graphService);
    }

}
