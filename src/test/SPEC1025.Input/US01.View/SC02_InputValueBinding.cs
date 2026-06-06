using Bunit;
using InputSut = LowlandTech.Components.Input;

namespace LowlandTech.Components.Tests.SPEC1025.Input.US01.View;

[Trait(Spec.SPEC, "1025")]
[Trait(Spec.SC, "02")]
[UserStory("01", "Input value binding")]
public class SC02_InputValueBinding : ViewScenario
{
    [Fact(DisplayName = "UAC01: Input renders with initial Value")]
    [Trait(Spec.UAC, "01")]
    [Then("the input displays the initial value")]
    public void UAC01_RendersWithInitialValue()
    {
        var cut = Ctx.Render<InputSut>(p => p.Add(x => x.Value, "initial-value"));
        cut.Find("[data-slot='input']").GetAttribute("value").ShouldBe("initial-value");
    }

    [Fact(DisplayName = "UAC02: Input updates Value on input")]
    [Trait(Spec.UAC, "02")]
    [Then("the ValueChanged callback is invoked with the new value")]
    public void UAC02_UpdatesValueOnInput()
    {
        var value = "initial";
        var cut = Ctx.Render<InputSut>(p =>
        {
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueChanged, (string? v) => value = v);
        });

        var input = cut.Find("[data-slot='input']");
        input.Input("updated-value");

        value.ShouldBe("updated-value");
    }

    [Fact(DisplayName = "UAC03: Input invokes ValueChanged with empty string")]
    [Trait(Spec.UAC, "03")]
    [Then("the ValueChanged callback is invoked with empty string")]
    public void UAC03_InvokesValueChangedWithEmptyString()
    {
        string? capturedValue = "initial";
        var cut = Ctx.Render<InputSut>(p =>
        {
            p.Add(x => x.Value, "initial");
            p.Add(x => x.ValueChanged, (string? v) => capturedValue = v);
        });

        var input = cut.Find("[data-slot='input']");
        input.Input("");

        capturedValue.ShouldBe("");
    }

    [Fact(DisplayName = "UAC04: Input handles null Value")]
    [Trait(Spec.UAC, "04")]
    [Then("the input renders without error when Value is null")]
    public void UAC04_HandlesNullValue()
    {
        var cut = Ctx.Render<InputSut>(p => p.Add(x => x.Value, (string?)null));
        cut.Find("[data-slot='input']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC05: Input supports two-way binding")]
    [Trait(Spec.UAC, "05")]
    [Then("the value updates correctly with bind-Value pattern")]
    public void UAC05_SupportsTwoWayBinding()
    {
        var value = "test";
        var cut = Ctx.Render<InputSut>(p =>
        {
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueChanged, (string? v) => value = v!);
        });

        var input = cut.Find("[data-slot='input']");
        input.Input("new-value");

        value.ShouldBe("new-value");
    }
}
