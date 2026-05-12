using Bunit;
using LowlandTech.Components.Field;

namespace LowlandTech.Components.Tests.SPEC1023.Field.US01.View;

[Trait(Spec.SPEC, "1023")]
[Trait(Spec.SC, "02")]
[UserStory("01", "Render Field subcomponents")]
public class SC02_RenderFieldSubcomponents : ViewScenario
{
    [Fact(DisplayName = "UAC01: FieldGroup renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_FieldGroupRendersDataSlot()
    {
        var cut = Ctx.Render<FieldGroup>();
        cut.Find("[data-slot='field-group']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: FieldLabel renders data-slot attribute")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_FieldLabelRendersDataSlot()
    {
        var cut = Ctx.Render<FieldLabel>(p => p.AddChildContent("Label"));
        cut.Find("[data-slot='field-label']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC03: FieldContent renders data-slot attribute")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_FieldContentRendersDataSlot()
    {
        var cut = Ctx.Render<FieldContent>();
        cut.Find("[data-slot='field-content']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC04: FieldDescription renders data-slot attribute")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_FieldDescriptionRendersDataSlot()
    {
        var cut = Ctx.Render<FieldDescription>();
        cut.Find("[data-slot='field-description']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC05: FieldError renders when ErrorMessage provided")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_FieldErrorRendersWhenErrorMessageProvided()
    {
        var cut = Ctx.Render<FieldError>(p => p.Add(x => x.ErrorMessage, "Error!"));
        cut.Find("[data-slot='field-error']").ShouldNotBeNull();
        cut.Markup.ShouldContain("Error!");
    }

    [Fact(DisplayName = "UAC06: FieldSet renders data-slot attribute")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_FieldSetRendersDataSlot()
    {
        var cut = Ctx.Render<FieldSet>();
        cut.Find("[data-slot='field-set']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC07: FieldLegend renders data-slot attribute")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_FieldLegendRendersDataSlot()
    {
        var cut = Ctx.Render<FieldLegend>();
        cut.Find("[data-slot='field-legend']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC08: FieldLegend renders with Variant parameter")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_FieldLegendRendersWithVariant()
    {
        var cut = Ctx.Render<FieldLegend>(p => p.Add(x => x.Variant, "label"));
        cut.Find("[data-variant='label']").ShouldNotBeNull();
    }
}
