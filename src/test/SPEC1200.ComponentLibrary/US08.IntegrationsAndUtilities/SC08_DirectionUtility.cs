using Bunit;
using LowlandTech.Components;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US08.IntegrationsAndUtilities;

/// <summary>
/// Tests for Direction utility component and functionality.
/// SC08 - Implement Direction utility
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "08")]
[Trait(Spec.SC, "08")]
public class SC08_DirectionUtility : Scenario
{
    protected override Task GivenAsync()
    {
        return Task.CompletedTask;
    }

    protected override Task WhenAsync()
    {
        return Task.CompletedTask;
    }

    [Fact(DisplayName = "Then it should support setting text direction (ltr or rtl)")]
    [Trait(Spec.UAC, "24")]
    public async Task UAC24_ShouldSupportTextDirection()
    {
        await ArrangeAndActAsync();
        using var ctx = new BunitContext();
        
        // Test LTR direction
        var ltrComponent = ctx.Render<Direction>(parameters => parameters
            .Add(p => p.Value, TextDirection.Ltr)
            .Add(p => p.ChildContent, "<span>LTR Content</span>"));

        var ltrDiv = ltrComponent.Find("div[dir]");
        ltrDiv.ShouldNotBeNull();
        ltrDiv.GetAttribute("dir").ShouldBe("ltr");

        // Test RTL direction
        var rtlComponent = ctx.Render<Direction>(parameters => parameters
            .Add(p => p.Value, TextDirection.Rtl)
            .Add(p => p.ChildContent, "<span>RTL Content</span>"));

        var rtlDiv = rtlComponent.Find("div[dir]");
        rtlDiv.ShouldNotBeNull();
        rtlDiv.GetAttribute("dir").ShouldBe("rtl");
    }

    [Fact(DisplayName = "And it should propagate direction to child components")]
    [Trait(Spec.UAC, "25")]
    public async Task UAC25_ShouldPropagateDirection()
    {
        await ArrangeAndActAsync();
        using var ctx = new BunitContext();
        
        // Create a consumer component that receives the cascading direction
        var component = ctx.Render<Direction>(parameters => parameters
            .Add(p => p.Value, TextDirection.Rtl)
            .Add(p => p.ChildContent, "<div class=\"child-content\">Child</div>"));

        // Verify the direction is set on the container
        var container = component.Find("div[dir='rtl']");
        container.ShouldNotBeNull();
        
        // Verify child content is rendered within the direction context
        var child = component.Find(".child-content");
        child.ShouldNotBeNull();
        child.TextContent.ShouldBe("Child");
    }

    [Fact(DisplayName = "And it should support a default direction when not specified")]
    [Trait(Spec.UAC, "26")]
    public async Task UAC26_ShouldSupportDefaultDirection()
    {
        await ArrangeAndActAsync();
        using var ctx = new BunitContext();
        
        // Don't specify Value parameter - should default to LTR
        var component = ctx.Render<Direction>(parameters => parameters
            .Add(p => p.ChildContent, "<span>Default Direction</span>"));

        var div = component.Find("div[dir]");
        div.ShouldNotBeNull();
        div.GetAttribute("dir").ShouldBe("ltr"); // Default should be LTR
    }
}
