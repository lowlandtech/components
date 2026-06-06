using LowlandTech.Components.Core;

namespace LowlandTech.Components.Tests.SPEC1072.DataForm.US02.FormFieldComponent;

[Trait(Spec.SPEC, "1072")]
[Trait(Spec.US, "02")]
[Trait(Spec.SC, "04")]
[UserStory("02", "FormField Event Triggers")]
public class SC04_FormFieldEvents : ViewScenario
{
    [Fact(DisplayName = "UAC01: OnChanging fires during input")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_OnChangingFires()
    {
        FormFieldChangeArgs? capturedArgs = null;

        var cut = Ctx.Render<LowlandTech.Components.FormField>(p => p
            .Add(x => x.FieldName, "name")
            .Add(x => x.FieldType, FormFieldType.Text)
            .Add(x => x.Label, "Name")
            .Add(x => x.OnChanging, EventCallback.Factory.Create<FormFieldChangeArgs>(this, args => capturedArgs = args)));

        cut.Find("[data-field-input='name']").Input("J");

        capturedArgs.ShouldNotBeNull();
        capturedArgs!.FieldName.ShouldBe("name");
        capturedArgs.NewValue.ShouldBe("J");
    }

    [Fact(DisplayName = "UAC02: OnChanged fires after value commit")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_OnChangedFires()
    {
        FormFieldChangeArgs? capturedArgs = null;

        var cut = Ctx.Render<LowlandTech.Components.FormField>(p => p
            .Add(x => x.FieldName, "name")
            .Add(x => x.FieldType, FormFieldType.Text)
            .Add(x => x.Label, "Name")
            .Add(x => x.OnChanged, EventCallback.Factory.Create<FormFieldChangeArgs>(this, args => capturedArgs = args)));

        cut.Find("[data-field-input='name']").Input("John");
        cut.Find("[data-field-input='name']").Change("John");

        capturedArgs.ShouldNotBeNull();
        capturedArgs!.FieldName.ShouldBe("name");
    }

    [Fact(DisplayName = "UAC03: OnChange fires on value change")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_OnChangeFires()
    {
        FormFieldChangeArgs? capturedArgs = null;

        var cut = Ctx.Render<LowlandTech.Components.FormField>(p => p
            .Add(x => x.FieldName, "email")
            .Add(x => x.FieldType, FormFieldType.Text)
            .Add(x => x.Label, "Email")
            .Add(x => x.OnChange, EventCallback.Factory.Create<FormFieldChangeArgs>(this, args => capturedArgs = args)));

        cut.Find("[data-field-input='email']").Input("test");
        cut.Find("[data-field-input='email']").Change("test");

        capturedArgs.ShouldNotBeNull();
        capturedArgs!.FieldName.ShouldBe("email");
    }

    [Fact(DisplayName = "UAC04: OnBlur fires on focus leave")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_OnBlurFires()
    {
        var blurFired = false;

        var cut = Ctx.Render<LowlandTech.Components.FormField>(p => p
            .Add(x => x.FieldName, "name")
            .Add(x => x.FieldType, FormFieldType.Text)
            .Add(x => x.Label, "Name")
            .Add(x => x.OnBlur, EventCallback.Factory.Create<FocusEventArgs>(this, _ => blurFired = true)));

        cut.Find("[data-field-input='name']").Blur();

        blurFired.ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC05: OnFocus fires on focus enter")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_OnFocusFires()
    {
        var focusFired = false;

        var cut = Ctx.Render<LowlandTech.Components.FormField>(p => p
            .Add(x => x.FieldName, "name")
            .Add(x => x.FieldType, FormFieldType.Text)
            .Add(x => x.Label, "Name")
            .Add(x => x.OnFocus, EventCallback.Factory.Create<FocusEventArgs>(this, _ => focusFired = true)));

        cut.Find("[data-field-input='name']").Focus();

        focusFired.ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC06: ValueChanged fires with converted value")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_ValueChangedFiresWithConvertedValue()
    {
        object? capturedValue = null;

        var cut = Ctx.Render<LowlandTech.Components.FormField>(p => p
            .Add(x => x.FieldName, "age")
            .Add(x => x.FieldType, FormFieldType.Number)
            .Add(x => x.Label, "Age")
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<object?>(this, v => capturedValue = v)));

        cut.Find("[data-field-input='age']").Input("25");
        cut.Find("[data-field-input='age']").Change("25");

        capturedValue.ShouldBe(25);
    }
}
