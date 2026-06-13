namespace LowlandTech.Components.Tests.SPEC1078.Session.US01.Blocks;

[Trait(Spec.SPEC, "1078")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "15")]
[UserStory("01", "Edge-type-driven node creation in Create mode")]
public sealed class SC15_EdgeTypeDrivenCreate : ViewScenario
{
    [Fact(DisplayName = "UAC01: Submitting create on a workspaces header resolves the WorkspaceCreator tool")]
    [Trait(Spec.UAC, "01")]
    [Given("a SessionBlock with a workspaces.header node selected in create mode")]
    [When("the user submits a title")]
    [Then("WorkspaceCreatorTool.ExecuteAsync is invoked with the title")]
    public async Task UAC01_CreateOnWorkspacesHeaderInvokesWorkspaceCreator()
    {
        var (cut, stateService, parentNode) = RenderWorkspacesHeaderInCreateMode();
        var tool = (WorkspaceCreatorTool)Ctx.Services.GetServices<IGraphToolService>()
            .Single(s => s is WorkspaceCreatorTool);

        object? executedEntity = null;
        string? capturedTitle = null;
        tool.OnExecuted = (entity, title) =>
        {
            executedEntity = entity;
            capturedTitle = title;
        };

        cut.Find("[data-testid='session-block-command-bar-toggle']").Click();

        var commandBar = cut.FindComponent<SessionCommandBar>();
        await commandBar.InvokeAsync(() => commandBar.Instance.OnSubmit.InvokeAsync(
            new SessionCommandSubmitted(parentNode, "create", "Accounting")));

        capturedTitle.ShouldBe("Accounting");
        executedEntity.ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Edge-driven create adds a workspace child under the workspaces header")]
    [Trait(Spec.UAC, "02")]
    [Given("a SessionBlock with a workspaces.header node selected in create mode")]
    [When("the user submits a title")]
    [Then("a new workspace node appears under the header")]
    public async Task UAC02_CreateOnWorkspacesHeaderAddsWorkspaceChild()
    {
        var (cut, stateService, parentNode) = RenderWorkspacesHeaderInCreateMode();

        cut.Find("[data-testid='session-block-command-bar-toggle']").Click();

        var commandBar = cut.FindComponent<SessionCommandBar>();
        await commandBar.InvokeAsync(() => commandBar.Instance.OnSubmit.InvokeAsync(
            new SessionCommandSubmitted(parentNode, "create", "Accounting")));

        cut.Markup.ShouldContain("title=\"workspace\"");
        cut.Markup.ShouldContain(">Accounting</span>");
    }

    [Fact(DisplayName = "UAC03: Edge-driven create resets agent type to ask after normal submit")]
    [Trait(Spec.UAC, "03")]
    [Given("a SessionBlock with a workspaces.header node selected in create mode")]
    [When("the user submits a title")]
    [Then("agent type resets to ask and input is cleared")]
    public async Task UAC03_EdgeDrivenCreateResetsAgentType()
    {
        var (cut, stateService, parentNode) = RenderWorkspacesHeaderInCreateMode();

        cut.Find("[data-testid='session-block-command-bar-toggle']").Click();

        var commandBar = cut.FindComponent<SessionCommandBar>();
        await commandBar.InvokeAsync(() => commandBar.Instance.OnSubmit.InvokeAsync(
            new SessionCommandSubmitted(parentNode, "create", "Accounting")));

        stateService.State.Ui.ActiveAgentTypeKey.ShouldBe("ask");
        stateService.State.Ui.Input.ShouldBeEmpty();
    }

    private (IRenderedComponent<Components.SessionBlock> Cut, SessionStateService StateService, SessionNodeInput ParentNode) RenderWorkspacesHeaderInCreateMode()
    {
        var stateService = new SessionStateService();
        var sessions = new[] { new SessionListEntry("s1", "Orders", 0, "session") };
        stateService.SelectSession(new SessionNodeInput("s1", "session", "Orders"), sessions);

        var parentNode = new SessionNodeInput("ws-header", "workspaces.header", "Workspaces");
        var contextNodes = new[]
        {
            new SessionContextNode(parentNode, IsLoaded: true)
        };

        var cut = Ctx.Render<Components.SessionBlock>(p => p
            .Add(x => x.HasSidebar, true)
            .Add(x => x.StateService, stateService)
            .Add(x => x.Sessions, sessions)
            .Add(x => x.ContextNodes, contextNodes));

        stateService.ProjectContext(parentNode);
        stateService.SetAgentType("create");

        return (cut, stateService, parentNode);
    }
}
