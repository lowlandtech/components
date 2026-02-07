using Bunit;
using LowlandTech.Components.Components.Field;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US04.AdvancedFormComponents;

/// <summary>
/// Tests for Field component rendering and functionality.
/// SC10 - Implement Field component
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "04")]
[Trait(Spec.SC, "10")]
public class SC10_FieldRenders : Scenario
{
    protected override Task GivenAsync()
    {
        return Task.CompletedTask;
    }

    protected override Task WhenAsync()
    {
        return Task.CompletedTask;
    }

    [Fact(DisplayName = "Then it should provide a wrapper for label, control, and description")]
    [Trait(Spec.UAC, "31")]
    public async Task UAC31_ShouldProvideFieldWrapper()
    {
        await ArrangeAndActAsync();
        using var ctx = new BunitContext();
        var component = ctx.Render<Field>(parameters => parameters
            .Add(p => p.Label, "Email Address")
            .Add(p => p.ChildContent, "<input type=\"email\" />")
        );
        var label = component.Find(".field-label");
        label.ShouldNotBeNull();
        label.TextContent.ShouldBe("Email Address");
    }

    [Fact(DisplayName = "And it should support validation and error message slots")]
    [Trait(Spec.UAC, "32")]
    public async Task UAC32_ShouldSupportValidationMessages()
    {
        await ArrangeAndActAsync();
        using var ctx = new BunitContext();
        var component = ctx.Render<Field>(parameters => parameters
            .Add(p => p.Description, "Enter your email")
            .Add(p => p.ErrorMessage, "Invalid email format")
            .Add(p => p.ChildContent, "<input type=\"email\" />")
        );
        var description = component.Find(".field-description");
        description.TextContent.ShouldBe("Enter your email");
        var error = component.Find(".field-error");
        error.TextContent.ShouldBe("Invalid email format");
    }

    [Fact(DisplayName = "And it should merge the Class parameter with generated classes")]
    [Trait(Spec.UAC, "33")]
    public async Task UAC33_ShouldMergeCustomClass()
    {
        await ArrangeAndActAsync();
        using var ctx = new BunitContext();
        var component = ctx.Render<Field>(parameters => parameters
            .Add(p => p.Label, "Field Label")
            .Add(p => p.Class, "custom-class another-class")
            .Add(p => p.ChildContent, "<input />")
        );
        var field = component.Find(".field");
        field.ClassList.ShouldContain("field");
        field.ClassList.ShouldContain("custom-class");
        field.ClassList.ShouldContain("another-class");
    }
}
