using LowlandTech.Components.Popover;

namespace LowlandTech.Components.Tests.SPEC1035.Popover.US01.View;

[Trait(Spec.SPEC, "1035")]
[Trait(Spec.SC, "06")]
[UserStory("01", "Render PopoverProvider styles")]
public class SC06_PopoverProviderStyles : ViewScenario
{
    [Fact(DisplayName = "UAC01: PopoverProvider renders node-menu with scrollable styles")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_NodeMenuHasScrollableStyles()
    {
        var cut = Ctx.Render<PopoverProvider>();

        var style = cut.Find("style");
        var css = style.TextContent;

        css.ShouldContain(".node-menu {");
        css.ShouldContain("max-height: 15rem");
        css.ShouldContain("overflow-y: auto");
    }
}
