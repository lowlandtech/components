using Bunit;
using AvatarSut = LowlandTech.Components.Avatar;

namespace LowlandTech.Components.Tests.SPEC1004.Avatar.US01.View;

[Trait(Spec.SPEC, "1004")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Avatar component")]
public class SC01_RenderAvatar : ViewScenario
{
    [Fact(DisplayName = "UAC01: Avatar renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<AvatarSut>();
        cut.Find("[data-slot='avatar']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Avatar renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<AvatarSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: Avatar renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<AvatarSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='avatar']").ClassList.ShouldContain("test-class");
    }
}
