using Bunit;
using InputOTPComponent = LowlandTech.Components.InputOTP;

namespace LowlandTech.Components.Tests.SPEC1027.InputOTP.US02.Behavior;

[Trait(Spec.SPEC, "1027")]
[Trait(Spec.US, "02")]
[UserStory("02", "InputOTP component behavior")]
public class SC01_InputOTPInteractions : ViewScenario
{
    [Fact(DisplayName = "UAC01: InputOTP triggers ValueChanged on input")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_InputOTPTriggersValueChanged()
    {
        var valueChanged = false;
        string? changedValue = null;

        var cut = Ctx.Render<InputOTPComponent>(p => p
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string>(this, v =>
            {
                valueChanged = true;
                changedValue = v;
            }))
        );

        var input = cut.Find("input");
        await input.InputAsync(new ChangeEventArgs { Value = "5" });

        valueChanged.ShouldBeTrue();
        changedValue.ShouldBe("5");
    }

    [Fact(DisplayName = "UAC02: InputOTP accepts only single digit")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_InputOTPAcceptsOnlySingleDigit()
    {
        var cut = Ctx.Render<InputOTPComponent>();

        var input = cut.Find("input");
        await input.InputAsync(new ChangeEventArgs { Value = "5" });

        input.GetAttribute("value").ShouldBe("5");
    }

    [Fact(DisplayName = "UAC03: InputOTP disabled inputs have disabled attribute")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_InputOTPDisabledInputsHaveDisabledAttribute()
    {
        var cut = Ctx.Render<InputOTPComponent>(p => p.Add(x => x.Disabled, true));
        var inputs = cut.FindAll("input");
        foreach (var input in inputs)
        {
            input.HasAttribute("disabled").ShouldBeTrue();
        }
    }

    [Fact(DisplayName = "UAC04: InputOTP builds complete value from all inputs")]
    [Trait(Spec.UAC, "04")]
    public async Task UAC04_InputOTPBuildsCompleteValue()
    {
        var values = new List<string>();

        var cut = Ctx.Render<InputOTPComponent>(p => p
            .Add(x => x.Length, 4)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string>(this, v => values.Add(v)))
        );

        // Input values one at a time, re-querying after each
        await cut.FindAll("input")[0].InputAsync(new ChangeEventArgs { Value = "1" });
        await cut.FindAll("input")[1].InputAsync(new ChangeEventArgs { Value = "2" });
        await cut.FindAll("input")[2].InputAsync(new ChangeEventArgs { Value = "3" });
        await cut.FindAll("input")[3].InputAsync(new ChangeEventArgs { Value = "4" });

        // The last value should be the complete code
        values.Last().ShouldBe("1234");
    }

    [Fact(DisplayName = "UAC05: InputOTP invokes Completed when the last digit is entered")]
    [Trait(Spec.UAC, "05")]
    public async Task UAC05_InputOTPInvokesCompletedOnLastDigit()
    {
        string? completedValue = null;

        var cut = Ctx.Render<InputOTPComponent>(p => p
            .Add(x => x.Length, 4)
            .Add(x => x.Completed, EventCallback.Factory.Create<string>(this, v => completedValue = v))
        );

        await cut.FindAll("input")[0].InputAsync(new ChangeEventArgs { Value = "1" });
        completedValue.ShouldBeNull();

        await cut.FindAll("input")[1].InputAsync(new ChangeEventArgs { Value = "2" });
        completedValue.ShouldBeNull();

        await cut.FindAll("input")[2].InputAsync(new ChangeEventArgs { Value = "3" });
        completedValue.ShouldBeNull();

        await cut.FindAll("input")[3].InputAsync(new ChangeEventArgs { Value = "4" });
        completedValue.ShouldBe("1234");
    }

    [Fact(DisplayName = "UAC06: InputOTP masks entered digits")]
    [Trait(Spec.UAC, "06")]
    public async Task UAC06_InputOTPMasksEnteredDigits()
    {
        var cut = Ctx.Render<InputOTPComponent>(p => p
            .Add(x => x.Mask, true)
            .Add(x => x.MaskChar, '*')
        );

        var input = cut.Find("input");
        await input.InputAsync(new ChangeEventArgs { Value = "5" });

        input.GetAttribute("value").ShouldBe("*");
    }
}
