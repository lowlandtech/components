using Bunit;
using LowlandTech.Components.Components.Spacer;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US10.UtilityComponents;

/// <summary>
/// Tests for Spacer component rendering and functionality.
/// SC02 - Implement Spacer component
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "10")]
[Trait(Spec.SC, "02")]
public class SC02_SpacerRenders : Scenario
{
    private BunitContext _ctx = null!;
    private IRenderedComponent<Spacer>? _spacerComponent;

    protected override Task GivenAsync()
    {
        _ctx = new BunitContext();
        return Task.CompletedTask;
    }

    protected override Task WhenAsync()
    {
        _spacerComponent = _ctx.Render<Spacer>();
        return Task.CompletedTask;
    }

    public override async Task DisposeAsync()
    {
        _ctx?.Dispose();
        await base.DisposeAsync();
    }

    [Fact(DisplayName = "Then it should render a <div> element with class \"spacer\"")]
    [Trait(Spec.UAC, "06")]
    public async Task UAC06_ShouldRenderDivWithSpacerClass()
    {
        using var ctx = new BunitContext();
        
        // Create Spacer component
        var component = ctx.Render<Spacer>();
        
        // Should render <div> element
        var div = component.Find("div");
        div.ShouldNotBeNull();
        
        // Should have "spacer" class
        div.ClassList.ShouldContain("spacer");
    }

    [Fact(DisplayName = "And it should merge the Class parameter with generated classes")]
    [Trait(Spec.UAC, "07")]
    public async Task UAC07_ShouldMergeCustomClass()
    {
        using var ctx = new BunitContext();
        
        // Create Spacer with custom class
        var component = ctx.Render<Spacer>(parameters => parameters
            .Add(p => p.Class, "my-custom-spacer"));
        
        var div = component.Find("div");
        
        // Should have both "spacer" and custom class
        div.ClassList.ShouldContain("spacer");
        div.ClassList.ShouldContain("my-custom-spacer");
    }

    [Fact(DisplayName = "And it should forward unmatched attributes to the underlying element")]
    [Trait(Spec.UAC, "08")]
    public async Task UAC08_ShouldForwardUnmatchedAttributes()
    {
        using var ctx = new BunitContext();
        
        // Create Spacer with custom attributes
        var component = ctx.Render<Spacer>(parameters => parameters
            .AddUnmatched("data-testid", "custom-spacer")
            .AddUnmatched("aria-hidden", "true"));
        
        var div = component.Find("div");
        
        // Should have forwarded attributes
        div.GetAttribute("data-testid").ShouldBe("custom-spacer");
        div.GetAttribute("aria-hidden").ShouldBe("true");
    }
}
