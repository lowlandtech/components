using Bunit;
using InputOTPComponent = LowlandTech.Components.InputOTP;

namespace LowlandTech.Components.Tests.SPEC1027.InputOTP.US01.View;

[Trait(Spec.SPEC, "1027")]
[Trait(Spec.US, "01")]
[UserStory("01", "InputOTP component rendering")]
public class SC01_RenderInputOTP : ViewScenario
{
    [Fact(DisplayName = "UAC01: InputOTP renders with data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_InputOTPRendersWithDataSlot()
    {
        var cut = Ctx.Render<InputOTPComponent>();
        cut.Find("[data-slot='input-otp']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: InputOTP renders 6 input fields by default")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_InputOTPRendersSixInputsByDefault()
    {
        var cut = Ctx.Render<InputOTPComponent>();
        var inputs = cut.FindAll("input");
        inputs.Count.ShouldBe(6);
    }

    [Fact(DisplayName = "UAC03: InputOTP renders correct number of inputs based on Length")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_InputOTPRendersCorrectNumberOfInputs()
    {
        var cut = Ctx.Render<InputOTPComponent>(p => p.Add(x => x.Length, 4));
        var inputs = cut.FindAll("input");
        inputs.Count.ShouldBe(4);
    }

    [Fact(DisplayName = "UAC04: InputOTP inputs have numeric inputmode")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_InputOTPInputsHaveNumericInputmode()
    {
        var cut = Ctx.Render<InputOTPComponent>();
        var input = cut.Find("input");
        input.GetAttribute("inputmode").ShouldBe("numeric");
    }

    [Fact(DisplayName = "UAC05: InputOTP inputs have digit pattern")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_InputOTPInputsHaveDigitPattern()
    {
        var cut = Ctx.Render<InputOTPComponent>();
        var input = cut.Find("input");
        input.GetAttribute("pattern").ShouldBe("[0-9]*");
    }

    [Fact(DisplayName = "UAC06: InputOTP inputs have maxlength of 1")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_InputOTPInputsHaveMaxlengthOne()
    {
        var cut = Ctx.Render<InputOTPComponent>();
        var input = cut.Find("input");
        input.GetAttribute("maxlength").ShouldBe("1");
    }

    [Fact(DisplayName = "UAC07: InputOTP inputs are type text by default")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_InputOTPInputsAreTypeText()
    {
        var cut = Ctx.Render<InputOTPComponent>();
        var input = cut.Find("input");
        input.GetAttribute("type").ShouldBe("text");
    }

    [Fact(DisplayName = "UAC09: InputOTP inputs are type password when masked")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_InputOTPInputsAreTypePasswordWhenMasked()
    {
        var cut = Ctx.Render<InputOTPComponent>(p => p.Add(x => x.Mask, true));
        var input = cut.Find("input");
        input.GetAttribute("type").ShouldBe("password");
    }

    [Fact(DisplayName = "UAC08: InputOTP renders with custom Length")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_InputOTPRendersWithCustomLength()
    {
        var cut = Ctx.Render<InputOTPComponent>(p => p.Add(x => x.Length, 8));
        cut.FindAll("input").Count.ShouldBe(8);
    }
}
