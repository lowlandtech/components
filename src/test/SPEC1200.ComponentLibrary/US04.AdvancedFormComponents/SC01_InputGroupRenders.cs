using Bunit;
using LowlandTech.Components.Components.InputGroup;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US04.AdvancedFormComponents;

/// <summary>
/// Tests for InputGroup component rendering and functionality.
/// SC01 - Implement InputGroup component
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "04")]
[Trait(Spec.SC, "01")]
public class SC01_InputGroupRenders : Scenario
{
    protected override Task GivenAsync()
    {
        return Task.CompletedTask;
    }

    protected override Task WhenAsync()
    {
        return Task.CompletedTask;
    }

    [Fact(DisplayName = "Then it should render a wrapper with class \"input-group\"")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_ShouldRenderWrapperWithClass()
    {
        await ArrangeAndActAsync();
        using var ctx = new BunitContext();
        var component = ctx.Render<InputGroup>(parameters => parameters
            .Add(p => p.ChildContent, "<input />")
        );
        var wrapper = component.Find(".input-group");
        wrapper.ShouldNotBeNull();
    }

    [Fact(DisplayName = "And it should support prefix and suffix content slots")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_ShouldSupportPrefixAndSuffix()
    {
        await ArrangeAndActAsync();
        using var ctx = new BunitContext();
        var component = ctx.Render<InputGroup>(parameters => parameters
            .Add(p => p.Prefix, "<span>$</span>")
            .Add(p => p.Suffix, "<span>.00</span>")
            .Add(p => p.ChildContent, "<input />")
        );
        component.Markup.ShouldContain("$");
        component.Markup.ShouldContain(".00");
    }

    [Fact(DisplayName = "And it should merge the Class parameter with generated classes")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_ShouldMergeCustomClass()
    {
        await ArrangeAndActAsync();
        using var ctx = new BunitContext();
        var component = ctx.Render<InputGroup>(parameters => parameters
            .Add(p => p.Class, "custom-class another-class")
            .Add(p => p.ChildContent, "<input />")
        );
        var wrapper = component.Find(".input-group");
        wrapper.ClassList.ShouldContain("input-group");
        wrapper.ClassList.ShouldContain("custom-class");
        wrapper.ClassList.ShouldContain("another-class");
    }
}
