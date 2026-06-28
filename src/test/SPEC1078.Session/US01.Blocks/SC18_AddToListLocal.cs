namespace LowlandTech.Components.Tests.SPEC1078.Session.US01.Blocks;

[Trait(Spec.SPEC, "1078")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "18")]
[UserStory("01", "Add context nodes to locally maintained lists")]
public sealed class SC18_AddToListLocal : ViewScenario
{
    private const string UserContextId = "10000000-0000-0000-0000-000000000001";
    private const string ListsHeaderId = "10000000-0000-0000-0000-000000000002";
    private const string SomeListId = "10000000-0000-0000-0000-000000000003";
    private const string WorkspacesHeaderId = "10000000-0000-0000-0000-000000000004";
    private const string WorkspaceId = "10000000-0000-0000-0000-000000000005";

    [Fact(DisplayName = "UAC01: Saving add-to-list moves the node under the list locally")]
    [Trait(Spec.UAC, "01")]
    [Given("a SessionBlock without a graph service and a workspace next to a list")]
    [When("the user adds the workspace to the list and saves")]
    [Then("the workspace is recorded as a member of the list")]
    public async Task UAC01_AddToListLocallyAddsNodeUnderList()
    {
        var cut = RenderLocal();

        await OpenAddToListDialogAsync(cut, WorkspaceId);

        cut.WaitForAssertion(() =>
        {
            cut.FindAll("[data-testid='add-to-list-candidate']").Count.ShouldBe(1);
        });

        var candidate = cut.Find("[data-testid='add-to-list-candidate']");
        var checkbox = candidate.QuerySelector("[data-slot='checkbox']");
        checkbox.ShouldNotBeNull();
        checkbox!.Click();

        var save = cut.Find("[data-testid='add-to-list-save']");
        await save.ClickAsync(new MouseEventArgs());

        // Re-open the dialog and verify the list is now checked, proving the workspace
        // was actually recorded as a member of the list in local state.
        await OpenAddToListDialogAsync(cut, WorkspaceId);

        cut.WaitForAssertion(() =>
        {
            var candidate = cut.FindAll("[data-testid='add-to-list-candidate']")
                .FirstOrDefault(c => c.GetAttribute("data-list-id") == SomeListId);
            candidate.ShouldNotBeNull();

            var checkbox = candidate.QuerySelector("[data-slot='checkbox']");
            checkbox.ShouldNotBeNull();
            checkbox!.GetAttribute("data-state")?.ShouldBe("checked");
        });
    }

    private IRenderedComponent<Components.SessionBlock> RenderLocal()
    {
        var sessions = new[] { new SessionListEntry("s1", "Workspaces", 0, "session") };
        var stateService = new SessionStateService(null as ISessionSurfaceDiscovery, null as IGraphService);
        stateService.SelectSession(new SessionNodeInput("s1", "session", "Workspaces"), sessions);

        var contextNodes = new[]
        {
            new SessionContextNode(
                new SessionNodeInput(UserContextId, "user-context", "My Context"),
                new[]
                {
                    new SessionContextNode(
                        new SessionNodeInput(ListsHeaderId, "lists", "Lists"),
                        new[]
                        {
                            new SessionContextNode(
                                new SessionNodeInput(SomeListId, "list", "some list"),
                                null,
                                true)
                        },
                        true),
                    new SessionContextNode(
                        new SessionNodeInput(WorkspacesHeaderId, "workspaces.header", "Workspaces"),
                        new[]
                        {
                            new SessionContextNode(
                                new SessionNodeInput(WorkspaceId, "workspace", "Workspace"),
                                null,
                                true)
                        },
                        true)
                },
                true)
        };

        var cut = Ctx.Render<Components.SessionBlock>(p => p
            .Add(x => x.HasSidebar, true)
            .Add(x => x.StateService, stateService)
            .Add(x => x.Sessions, sessions)
            .Add(x => x.ContextNodes, contextNodes));

        foreach (var headerId in new[] { ListsHeaderId, WorkspacesHeaderId })
        {
            var chevron = cut.FindAll($"[data-node-id='{headerId}'] span.cursor-pointer").FirstOrDefault();
            chevron?.ClickAsync(new MouseEventArgs()).GetAwaiter().GetResult();
        }

        return cut;
    }

    private async Task<IRenderedComponent<Microsoft.AspNetCore.Components.IComponent>> OpenNodeMenuAsync(
        IRenderedComponent<Components.SessionBlock> cut, string nodeId)
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
}
