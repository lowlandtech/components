using Bunit;
using AlertSut = LowlandTech.Components.Alert.Alert;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1001.Alert.US01.View;

[Trait(Spec.SPEC, "1001")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Alert component")]
public class SC01_RenderAlert : ViewScenario
{
    [Fact(DisplayName = "UAC01: Alert renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<AlertSut>();
        cut.Find("[data-slot='alert']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Alert renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<AlertSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: Alert renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<AlertSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='alert']").ClassList.ShouldContain("test-class");
    }
}
