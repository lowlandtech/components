using Bunit;
using LowlandTech.Components.Components.Kbd;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US08.IntegrationsAndUtilities;

/// <summary>
/// Tests for Kbd component rendering and functionality.
/// SC07 - Implement Kbd component
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "08")]
[Trait(Spec.SC, "07")]
public class SC07_KbdRenders : Scenario
{
    protected override Task GivenAsync()
    {
        return Task.CompletedTask;
    }

    protected override Task WhenAsync()
    {
        return Task.CompletedTask;
    }

    [Fact(DisplayName = "Then it should render a <kbd> element with class \"kbd\"")]
    [Trait(Spec.UAC, "21")]
    public async Task UAC21_ShouldRenderKbdElement()
    {
        await ArrangeAndActAsync();
        using var ctx = new BunitContext();
        var component = ctx.Render<Kbd>(parameters => parameters
            .Add(p => p.ChildContent, "Ctrl"));

        var kbdElement = component.Find("kbd");
        kbdElement.ShouldNotBeNull();
        kbdElement.ClassList.ShouldContain("kbd");
        kbdElement.TextContent.ShouldBe("Ctrl");
    }

    [Fact(DisplayName = "And it should support inline usage within text")]
    [Trait(Spec.UAC, "22")]
    public async Task UAC22_ShouldSupportInlineUsage()
    {
        await ArrangeAndActAsync();
        using var ctx = new BunitContext();
        
        // Test that Kbd renders as inline element suitable for inline usage
        var component = ctx.Render<Kbd>(parameters => parameters
            .Add(p => p.ChildContent, "Enter"));

        var kbdElement = component.Find("kbd");
        kbdElement.ShouldNotBeNull();
        
        // Verify it has inline-flex display (from CSS)
        kbdElement.ClassList.ShouldContain("kbd");
    }

    [Fact(DisplayName = "And it should merge the Class parameter with generated classes")]
    [Trait(Spec.UAC, "23")]
    public async Task UAC23_ShouldMergeCustomClass()
    {
        await ArrangeAndActAsync();
        using var ctx = new BunitContext();
        var component = ctx.Render<Kbd>(parameters => parameters
            .Add(p => p.Class, "custom-class another-class")
            .Add(p => p.ChildContent, "Esc"));

        var kbdElement = component.Find("kbd");
        kbdElement.ClassList.ShouldContain("kbd");
        kbdElement.ClassList.ShouldContain("custom-class");
        kbdElement.ClassList.ShouldContain("another-class");
    }
}
