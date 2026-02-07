using Bunit;
using LowlandTech.Components.Components.AspectRatio;
using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US07.LayoutComponents;

/// <summary>
/// SC04: AspectRatio component implementation
/// 
/// Feature: US07.LayoutComponents.feature
/// Scenario: SC04 - Implement AspectRatio component
/// 
/// Given I have the shadcn/ui AspectRatio component
/// When I port it to Blazor
/// Then it should render a wrapper that preserves a fixed aspect ratio
/// And it should support a Ratio parameter
/// And it should merge the Class parameter with generated classes
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "07")]
[Trait(Spec.SC, "04")]
public class SC04_AspectRatioRenders
{
    [Fact(DisplayName = "AspectRatio renders wrapper that preserves fixed aspect ratio")]
    [Trait(Spec.UAC, "14")]
    public void AspectRatio_RendersWrapperWithAspectRatio()
    {
        // Given
        using var ctx = new BunitContext();

        // When - Render AspectRatio with default ratio (16/9)
        var cut = ctx.Render<AspectRatio>(parameters => parameters
            .Add(p => p.Ratio, 16.0 / 9.0)
            .Add(p => p.ChildContent, builder =>
            {
                builder.OpenElement(0, "img");
                builder.AddAttribute(1, "src", "test.jpg");
                builder.AddAttribute(2, "alt", "Test image");
                builder.CloseElement();
            }));

        // Then - Should render a wrapper div
        var wrapper = cut.Find("div");
        Assert.NotNull(wrapper);
        
        // Should have aspect-ratio class
        var wrapperClass = wrapper.GetAttribute("class");
        Assert.Contains("aspect-ratio", wrapperClass);
        
        // Should contain the child content
        var img = cut.Find("img");
        Assert.NotNull(img);
        Assert.Equal("test.jpg", img.GetAttribute("src"));
    }

    [Fact(DisplayName = "AspectRatio supports Ratio parameter")]
    [Trait(Spec.UAC, "15")]
    public void AspectRatio_SupportsRatioParameter()
    {
        // Given
        using var ctx = new BunitContext();

        // When - Render AspectRatio with custom ratio (4/3)
        var cut = ctx.Render<AspectRatio>(parameters => parameters
            .Add(p => p.Ratio, 4.0 / 3.0)
            .Add(p => p.ChildContent, builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddContent(1, "Content");
                builder.CloseElement();
            }));

        // Then - Should render wrapper with aspect ratio styling
        var wrapper = cut.Find(".aspect-ratio");
        Assert.NotNull(wrapper);
        
        // Should have inline style with aspect ratio
        var style = wrapper.GetAttribute("style");
        Assert.NotNull(style);
        Assert.Contains("aspect-ratio", style);
    }

    [Fact(DisplayName = "AspectRatio merges Class parameter with generated classes")]
    [Trait(Spec.UAC, "16")]
    public void AspectRatio_MergesClassParameter()
    {
        // Given
        using var ctx = new BunitContext();

        // When - Render AspectRatio with custom class
        var cut = ctx.Render<AspectRatio>(parameters => parameters
            .Add(p => p.Ratio, 16.0 / 9.0)
            .Add(p => p.Class, "custom-class")
            .Add(p => p.ChildContent, builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddContent(1, "Content");
                builder.CloseElement();
            }));

        // Then - Should have both aspect-ratio and custom-class
        var wrapper = cut.Find("div");
        var classes = wrapper.GetAttribute("class");
        Assert.Contains("aspect-ratio", classes);
        Assert.Contains("custom-class", classes);
    }
}
