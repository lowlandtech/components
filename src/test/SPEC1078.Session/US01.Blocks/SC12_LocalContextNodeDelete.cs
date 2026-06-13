namespace LowlandTech.Components.Tests.SPEC1078.Session.US01.Blocks;

[Trait(Spec.SPEC, "1078")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "12")]
[UserStory("01", "Local context node deletion from context menu")]
public sealed class SC12_LocalContextNodeDelete : ViewScenario
{
    [Fact(DisplayName = "UAC01: Delete from context menu removes the node from the explorer")]
    [Trait(Spec.UAC, "01")]
    [Given("a SessionBlock with context nodes")]
    [When("the user opens the context menu and chooses Delete")]
    [Then("the node is removed from the context explorer")]
    public void UAC01_DeleteFromContextMenuRemovesNode()
    {
        var cut = Ctx.Render<Components.SessionBlock>();

        cut.Find("[data-node-id='release-session']").Click();
        cut.Find("[data-node-id='invoice-approval']").ShouldNotBeNull();

        cut.Find("[data-testid='session-context-node-menu-trigger'][data-node-id='invoice-approval']").Click();

        var popoverService = Ctx.Services.GetRequiredService<IPopoverService>();
        var popover = popoverService.ActivePopovers.Single(reference => reference.State.Open);

        var fragmentCut = Ctx.Render(builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddContent(1, popover.State.Fragment);
            builder.CloseElement();
        });

        fragmentCut.Find(".node-menu-item.text-destructive").Click();

        cut.FindAll("[data-node-id='invoice-approval']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC02: Wired host handler takes precedence over local delete fallback")]
    [Trait(Spec.UAC, "02")]
    [Given("a SessionBlock with OnNodeDeleteRequested wired")]
    [When("the user chooses Delete from the context menu")]
    [Then("the host callback is invoked and the node is not removed locally")]
    public void UAC02_HostHandlerTakesPrecedence()
    {
        SessionNodeInput? deleted = null;
        var cut = Ctx.Render<Components.SessionBlock>(p => p
            .Add(x => x.OnNodeDeleteRequested, node => { deleted = node; return Task.CompletedTask; }));

        cut.Find("[data-node-id='release-session']").Click();
        cut.Find("[data-testid='session-context-node-menu-trigger'][data-node-id='invoice-approval']").Click();

        var popoverService = Ctx.Services.GetRequiredService<IPopoverService>();
        var popover = popoverService.ActivePopovers.Single(reference => reference.State.Open);

        var fragmentCut = Ctx.Render(builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddContent(1, popover.State.Fragment);
            builder.CloseElement();
        });

        fragmentCut.Find(".node-menu-item.text-destructive").Click();

        deleted.ShouldNotBeNull();
        deleted.Id.ShouldBe("invoice-approval");
        cut.FindAll("[data-node-id='invoice-approval']").Count.ShouldBeGreaterThan(0);
    }

}
