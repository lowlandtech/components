namespace LowlandTech.Components.Tests.SPEC1078.Session.US01.Blocks;

[Trait(Spec.SPEC, "1078")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "12")]
[UserStory("01", "Local context node deletion from context menu")]
public sealed class SC12_LocalContextNodeDelete : ViewScenario
{
    [Fact(DisplayName = "UAC01: Delete from context menu opens the confirmation dialog")]
    [Trait(Spec.UAC, "01")]
    [Given("a SessionBlock with context nodes")]
    [When("the user opens the context menu and chooses Delete")]
    [Then("the delete confirmation dialog is shown")]
    public void UAC01_DeleteFromContextMenuOpensConfirmationDialog()
    {
        var cut = Ctx.Render<Components.SessionBlock>();
        cut.Find("[data-node-id='workitems-session']").Click();

        cut.Find("[data-testid='session-context-node-menu-trigger'][data-node-id='setup-ci']").Click();

        var popoverService = Ctx.Services.GetRequiredService<IPopoverService>();
        var popover = popoverService.ActivePopovers.Single(reference => reference.State.Open);

        var fragmentCut = Ctx.Render(builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddContent(1, popover.State.Fragment);
            builder.CloseElement();
        });

        fragmentCut.Find(".node-menu-item.text-destructive").Click();

        cut.Find("[data-slot='dialog-content']").ShouldNotBeNull();
        cut.Find("[data-slot='dialog-content']").TextContent.ShouldContain("Delete node");
    }

    [Fact(DisplayName = "UAC02: Confirming the dialog removes the node")]
    [Trait(Spec.UAC, "02")]
    [Given("the delete confirmation dialog is open")]
    [When("the user clicks Delete")]
    [Then("the node is removed from the context explorer")]
    public void UAC02_ConfirmingDialogRemovesNode()
    {
        var cut = Ctx.Render<Components.SessionBlock>();
        cut.Find("[data-node-id='workitems-session']").Click();
        OpenDeleteDialog(cut);

        cut.Find("[data-slot='dialog-content']")
            .QuerySelectorAll("button")
            .Single(b => string.Equals(b.TextContent.Trim(), "Delete", StringComparison.Ordinal))
            .Click();

        cut.FindAll("[data-node-id='setup-ci']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC03: Cancelling the dialog keeps the node")]
    [Trait(Spec.UAC, "03")]
    [Given("the delete confirmation dialog is open")]
    [When("the user clicks Cancel")]
    [Then("the node remains in the context explorer")]
    public void UAC03_CancellingDialogKeepsNode()
    {
        var cut = Ctx.Render<Components.SessionBlock>();
        cut.Find("[data-node-id='workitems-session']").Click();
        OpenDeleteDialog(cut);

        cut.Find("[data-slot='dialog-content']")
            .QuerySelectorAll("button")
            .Single(b => string.Equals(b.TextContent.Trim(), "Cancel", StringComparison.Ordinal))
            .Click();

        cut.FindAll("[data-node-id='setup-ci']").Count.ShouldBeGreaterThan(0);
    }

    [Fact(DisplayName = "UAC04: Wired host handler receives the node when dialog is confirmed")]
    [Trait(Spec.UAC, "04")]
    [Given("a SessionBlock with OnNodeDeleteRequested wired")]
    [When("the user confirms the delete dialog")]
    [Then("the host callback is invoked with the node")]
    public void UAC04_HostHandlerReceivesNodeOnConfirm()
    {
        SessionNodeInput? deleted = null;
        var cut = Ctx.Render<Components.SessionBlock>(p => p
            .Add(x => x.OnNodeDeleteRequested, node => { deleted = node; return Task.CompletedTask; }));

        cut.Find("[data-node-id='workitems-session']").Click();
        OpenDeleteDialog(cut);

        cut.Find("[data-slot='dialog-content']")
            .QuerySelectorAll("button")
            .Single(b => string.Equals(b.TextContent.Trim(), "Delete", StringComparison.Ordinal))
            .Click();

        deleted.ShouldNotBeNull();
        deleted.Id.ShouldBe("setup-ci");
    }

    [Fact(DisplayName = "UAC05: Skip preference deletes immediately without showing the dialog")]
    [Trait(Spec.UAC, "05")]
    [Given("the user has opted out of delete confirmations")]
    [When("the user chooses Delete from the context menu")]
    [Then("the node is removed without showing the dialog")]
    public void UAC05_SkipPreferenceDeletesImmediately()
    {
        Ctx.JSInterop.Setup<string?>("localStorage.getItem", "session-context-delete-skip-confirmation").SetResult("true");

        var cut = Ctx.Render<Components.SessionBlock>();

        cut.Find("[data-node-id='workitems-session']").Click();
        cut.Find("[data-testid='session-context-node-menu-trigger'][data-node-id='setup-ci']").Click();

        var popoverService = Ctx.Services.GetRequiredService<IPopoverService>();
        var popover = popoverService.ActivePopovers.Single(reference => reference.State.Open);

        var fragmentCut = Ctx.Render(builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddContent(1, popover.State.Fragment);
            builder.CloseElement();
        });

        fragmentCut.Find(".node-menu-item.text-destructive").Click();

        cut.FindAll("[data-slot='dialog-content']").Count.ShouldBe(0);
        cut.FindAll("[data-node-id='setup-ci']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC06: Checking 'Don't ask again' persists the skip preference")]
    [Trait(Spec.UAC, "06")]
    [Given("the delete confirmation dialog is open")]
    [When("the user checks 'Don't ask again' and confirms")]
    [Then("the skip preference is persisted to localStorage")]
    public void UAC06_DontAskAgainPersistsSkipPreference()
    {
        var cut = Ctx.Render<Components.SessionBlock>();
        cut.Find("[data-node-id='workitems-session']").Click();
        OpenDeleteDialog(cut);

        var dialog = cut.Find("[data-slot='dialog-content']");
        dialog.QuerySelector("input[type='checkbox']")?.Change(true);

        dialog.QuerySelectorAll("button")
            .Single(b => string.Equals(b.TextContent.Trim(), "Delete", StringComparison.Ordinal))
            .Click();

        var persisted = Ctx.JSInterop.Invocations
            .Where(i => i.Identifier == "localStorage.setItem")
            .Select(i => i.Arguments.ElementAtOrDefault(1)?.ToString())
            .LastOrDefault();

        persisted.ShouldNotBeNull();
        persisted.ShouldBe("true");
    }

    [Fact(DisplayName = "UAC07: Delete confirmation dialog structure is preserved against drift")]
    [Trait(Spec.UAC, "07")]
    [Given("the delete confirmation dialog is open")]
    [When("the dialog markup is inspected")]
    [Then("the expected title, description, checkbox, and buttons are present")]
    public void UAC07_DeleteConfirmationDialogStructureIsPreserved()
    {
        var cut = Ctx.Render<Components.SessionBlock>();
        cut.Find("[data-node-id='workitems-session']").Click();
        OpenDeleteDialog(cut);

        var dialog = cut.Find("[data-slot='dialog-content']");
        dialog.ShouldNotBeNull();

        var title = dialog.QuerySelector("[data-slot='dialog-title']");
        title.ShouldNotBeNull();
        title.TextContent.Trim().ShouldBe("Delete node");

        var description = dialog.QuerySelector("[data-slot='dialog-description']");
        description.ShouldNotBeNull();
        description.TextContent.ShouldContain("Are you sure you want to delete this node?");

        var checkbox = dialog.QuerySelector("input[type='checkbox']");
        checkbox.ShouldNotBeNull();

        var checkboxLabel = dialog.QuerySelector("label[for='delete-node-skip-confirmation']");
        checkboxLabel.ShouldNotBeNull();
        checkboxLabel.TextContent.Trim().ShouldBe("Don't ask again");

        var buttons = dialog.QuerySelectorAll("button");
        buttons.Any(b => string.Equals(b.TextContent.Trim(), "Cancel", StringComparison.Ordinal)).ShouldBeTrue();
        buttons.Any(b => string.Equals(b.TextContent.Trim(), "Delete", StringComparison.Ordinal)).ShouldBeTrue();

        var deleteButton = buttons.Single(b => string.Equals(b.TextContent.Trim(), "Delete", StringComparison.Ordinal));
        deleteButton.ClassList.ShouldContain("btn-destructive");
    }

    private void OpenDeleteDialog(IRenderedComponent<Components.SessionBlock> cut, string nodeId = "setup-ci")
    {
        cut.Find($"[data-testid='session-context-node-menu-trigger'][data-node-id='{nodeId}']").Click();

        var popoverService = Ctx.Services.GetRequiredService<IPopoverService>();
        var popover = popoverService.ActivePopovers.Single(reference => reference.State.Open);

        var fragmentCut = Ctx.Render(builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddContent(1, popover.State.Fragment);
            builder.CloseElement();
        });

        fragmentCut.Find(".node-menu-item.text-destructive").Click();
    }
}
