using Bunit;
using TextareaSut = LowlandTech.Components.Textarea.Textarea;

namespace LowlandTech.Components.Tests.SPEC1051.Textarea.US01.View;

[Trait(Spec.SPEC, "1051")]
[Trait(Spec.SC, "02")]
[UserStory("01", "Textarea value binding")]
public class SC02_TextareaValueBinding : ViewScenario
{
    [Fact(DisplayName = "UAC01: Textarea renders with initial Value")]
    [Trait(Spec.UAC, "01")]
    [Then("the textarea displays the initial value")]
    public void UAC01_RendersWithInitialValue()
    {
        var cut = Ctx.Render<TextareaSut>(p => p.Add(x => x.Value, "initial-value"));
        cut.Find("[data-slot='textarea']").TextContent.ShouldBe("initial-value");
    }

    [Fact(DisplayName = "UAC02: Textarea updates Value on input")]
    [Trait(Spec.UAC, "02")]
    [Then("the ValueChanged callback is invoked with the new value")]
    public void UAC02_UpdatesValueOnInput()
    {
        var value = "initial";
        var cut = Ctx.Render<TextareaSut>(p =>
        {
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueChanged, (string? v) => value = v);
        });

        var textarea = cut.Find("[data-slot='textarea']");
        textarea.Input("updated-value");

        value.ShouldBe("updated-value");
    }

    [Fact(DisplayName = "UAC03: Textarea invokes ValueChanged with empty string")]
    [Trait(Spec.UAC, "03")]
    [Then("the ValueChanged callback is invoked with empty string")]
    public void UAC03_InvokesValueChangedWithEmptyString()
    {
        string? capturedValue = "initial";
        var cut = Ctx.Render<TextareaSut>(p =>
        {
            p.Add(x => x.Value, "initial");
            p.Add(x => x.ValueChanged, (string? v) => capturedValue = v);
        });

        var textarea = cut.Find("[data-slot='textarea']");
        textarea.Input("");

        capturedValue.ShouldBe("");
    }

    [Fact(DisplayName = "UAC04: Textarea handles null Value")]
    [Trait(Spec.UAC, "04")]
    [Then("the textarea renders without error when Value is null")]
    public void UAC04_HandlesNullValue()
    {
        var cut = Ctx.Render<TextareaSut>(p => p.Add(x => x.Value, (string?)null));
        cut.Find("[data-slot='textarea']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC05: Textarea supports two-way binding")]
    [Trait(Spec.UAC, "05")]
    [Then("the value updates correctly with bind-Value pattern")]
    public void UAC05_SupportsTwoWayBinding()
    {
        var value = "test";
        var cut = Ctx.Render<TextareaSut>(p =>
        {
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueChanged, (string? v) => value = v!);
        });

        var textarea = cut.Find("[data-slot='textarea']");
        textarea.Input("new-value");

        value.ShouldBe("new-value");
    }

    [Fact(DisplayName = "UAC06: Textarea handles multiline text")]
    [Trait(Spec.UAC, "06")]
    [Then("the textarea renders multiline content correctly")]
    public void UAC06_HandlesMultilineText()
    {
        var multilineText = "Line 1\nLine 2\nLine 3";
        var cut = Ctx.Render<TextareaSut>(p => p.Add(x => x.Value, multilineText));
        cut.Find("[data-slot='textarea']").TextContent.ShouldBe(multilineText);
    }
}
