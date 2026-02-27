using Bunit;
using CommandSut = LowlandTech.Components.Command.Command;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1016.Command.US01.View;

[Trait(Spec.SPEC, "1016")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Command component")]
public class SC01_RenderCommand : ViewScenario
{
    [Fact(DisplayName = "UAC01: Command renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<CommandSut>();
        cut.Find("[data-slot='command']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Command renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<CommandSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: Command renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<CommandSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='command']").ClassList.ShouldContain("test-class");
    }
}
