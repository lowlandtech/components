namespace LowlandTech.Components.Tests.SPEC1035.Popover.US01.View;

[Trait(Spec.SPEC, "1035")]
[Trait(Spec.SC, "06")]
[UserStory("01", "Render PopoverProvider styles")]
public class SC06_PopoverProviderStyles : ViewScenario
{
    [Fact(DisplayName = "UAC01: PopoverProvider renders node-menu with scrollable styles")]
    [Trait(Spec.UAC, "01")]
    [Given("a rendered PopoverProvider")]
    [When("the provider portal styles are inspected")]
    [Then("node menus remain height constrained and vertically scrollable")]
    public void UAC01_NodeMenuHasScrollableStyles()
    {
        var cut = Ctx.Render<PopoverProvider>();

        var style = cut.Find("style");
        var css = style.TextContent;

        css.ShouldContain(".node-menu {");
        css.ShouldContain("max-height: 15rem");
        css.ShouldContain("overflow-y: auto");
    }

    [Fact(DisplayName = "UAC02: Placement runs after the open popover is rendered")]
    [Trait(Spec.UAC, "02")]
    [Given("a rendered PopoverProvider and a registered anchor-based popover")]
    [When("the popover service opens the popover")]
    [Then("the provider renders its content and invokes anchor positioning")]
    public void UAC02_PositionRunsAfterOpenPopoverIsRendered()
    {
        var cut = Ctx.Render<PopoverProvider>();
        var service = Ctx.Services.GetRequiredService<IPopoverService>();
        var id = Guid.NewGuid();

        service.Register(id);
        service.Update(id, new PopoverState
        {
            Open = true,
            Fragment = builder => builder.AddContent(0, "Anchored popup"),
            Placement = PopoverPlacement.BottomStart
        });

        cut.WaitForAssertion(() =>
        {
            cut.Find($"#popover-content-{id}").TextContent.ShouldContain("Anchored popup");
            Ctx.JSInterop.Invocations.ShouldContain(invocation =>
                invocation.Identifier == "ltPopover.position"
                && invocation.Arguments.Count == 2
                && Equals(invocation.Arguments[0], id.ToString())
                && Equals(invocation.Arguments[1], "bottomstart"));
        });
    }
}
