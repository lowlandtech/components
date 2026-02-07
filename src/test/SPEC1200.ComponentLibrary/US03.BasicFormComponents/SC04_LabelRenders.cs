using Bunit;
using LowlandTech.Components.Components.Label;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US03.BasicFormComponents;

/// <summary>
/// Tests for Label component rendering and functionality.
/// SC04 - Implement Label component
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "03")]
[Trait(Spec.SC, "04")]
public class SC04_LabelRenders : Scenario
{
    protected override Task GivenAsync()
    {
        return Task.CompletedTask;
    }

    protected override Task WhenAsync()
    {
        return Task.CompletedTask;
    }

    [Fact(DisplayName = "Then it should render a <label> element with proper classes")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_ShouldRenderLabelElement()
    {
        await ArrangeAndActAsync();
        using var ctx = new BunitContext();
        var component = ctx.Render<Label>(parameters => parameters
            .Add(p => p.ChildContent, "Email Address")
        );
        var labelElement = component.Find("label");
        labelElement.ShouldNotBeNull();
        labelElement.ClassList.ShouldContain("cn-label");
        labelElement.ClassList.ShouldContain("flex");
        labelElement.ClassList.ShouldContain("items-center");
        labelElement.ClassList.ShouldContain("select-none");
    }

    [Fact(DisplayName = "And it should merge a Class parameter with generated classes")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_ShouldMergeCustomClass()
    {
        await ArrangeAndActAsync();
        using var ctx = new BunitContext();
        var component = ctx.Render<Label>(parameters => parameters
            .Add(p => p.Class, "custom-class another-class")
            .Add(p => p.ChildContent, "Field Label")
        );
        var labelElement = component.Find("label");
        labelElement.ClassList.ShouldContain("cn-label");
        labelElement.ClassList.ShouldContain("custom-class");
        labelElement.ClassList.ShouldContain("another-class");
    }
}
