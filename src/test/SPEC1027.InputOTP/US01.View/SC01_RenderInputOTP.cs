using Bunit;
using InputOTPSut = LowlandTech.Components.InputOTP.InputOTP;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1027.InputOTP.US01.View;

[Trait(Spec.SPEC, "1027")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render InputOTP component")]
public class SC01_RenderInputOTP : ViewScenario
{
    [Fact(DisplayName = "UAC01: InputOTP renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<InputOTPSut>();
        cut.Find("[data-slot='input-otp']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: InputOTP renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<InputOTPSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: InputOTP renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<InputOTPSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='input-otp']").ClassList.ShouldContain("test-class");
    }
}
