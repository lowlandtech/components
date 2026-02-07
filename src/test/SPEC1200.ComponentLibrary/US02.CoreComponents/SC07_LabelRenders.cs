using Bunit;
using LowlandTech.Components.Components.Label;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US02.CoreComponents;

/// <summary>
/// Tests for Label component rendering and functionality.
/// SC07 - Implement Label component
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "02")]
[Trait(Spec.SC, "07")]
public class SC07_LabelRenders : Scenario
{
    private BunitContext _ctx = null!;
    private IRenderedComponent<Label>? _labelComponent;

    protected override Task GivenAsync()
    {
        _ctx = new BunitContext();
        return Task.CompletedTask;
    }

    protected override Task WhenAsync()
    {
        _labelComponent = _ctx.Render<Label>(parameters => parameters
            .AddChildContent("Test Label")
        );
        return Task.CompletedTask;
    }

    public override async Task DisposeAsync()
    {
        _ctx?.Dispose();
        await base.DisposeAsync();
    }

    [Fact(DisplayName = "Then it should render a <label> element with class \"label\"")]
    [Trait(Spec.UAC, "26")]
    public async Task UAC26_ShouldRenderLabelElement()
    {
        using var ctx = new BunitContext();
        
        // Create Label component
        var component = ctx.Render<Label>(parameters => parameters
            .AddChildContent("Email Address"));
        
        // Should render <label> element
        var label = component.Find("label");
        label.ShouldNotBeNull();
        
        // Should have "label" class
        label.ClassList.ShouldContain("label");
        
        // Should contain the text content
        label.TextContent.ShouldBe("Email Address");
    }

    [Fact(DisplayName = "And it should support a For parameter for associating inputs")]
    [Trait(Spec.UAC, "27")]
    public async Task UAC27_ShouldSupportForParameter()
    {
        using var ctx = new BunitContext();
        
        // Create Label with For parameter
        var component = ctx.Render<Label>(parameters => parameters
            .Add(p => p.For, "email-input")
            .AddChildContent("Email"));
        
        // Should have for attribute matching the For parameter
        var label = component.Find("label");
        label.GetAttribute("for").ShouldBe("email-input");
    }

    [Fact(DisplayName = "And it should merge the Class parameter with generated classes")]
    [Trait(Spec.UAC, "28")]
    public async Task UAC28_ShouldMergeCustomClass()
    {
        using var ctx = new BunitContext();
        
        // Create Label with custom class
        var component = ctx.Render<Label>(parameters => parameters
            .Add(p => p.Class, "custom-label")
            .AddChildContent("Username"));
        
        var label = component.Find("label");
        
        // Should have both the base "label" class and custom class
        label.ClassList.ShouldContain("label");
        label.ClassList.ShouldContain("custom-label");
    }
}
