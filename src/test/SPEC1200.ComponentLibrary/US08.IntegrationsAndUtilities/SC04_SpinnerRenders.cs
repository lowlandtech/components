using Bunit;
using LowlandTech.Components;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US08.IntegrationsAndUtilities;

/// <summary>
/// Tests for Spinner component rendering and functionality.
/// SC04 - Implement Spinner component
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "08")]
[Trait(Spec.SC, "04")]
public class SC04_SpinnerRenders : Scenario
{
    protected override Task GivenAsync()
    {
        return Task.CompletedTask;
    }

    protected override Task WhenAsync()
    {
        return Task.CompletedTask;
    }

    [Fact(DisplayName = "Then it should render a loading spinner element")]
    [Trait(Spec.UAC, "12")]
    public async Task UAC12_ShouldRenderSpinnerElement()
    {
        await ArrangeAndActAsync();
        using var ctx = new BunitContext();
        
        // Arrange & Act
        var cut = ctx.Render<Spinner>();

        // Assert - should render spinner with "spinner" class
        var spinner = cut.Find(".spinner");
        spinner.ShouldNotBeNull();
        spinner.GetAttribute("role").ShouldBe("status");
        spinner.GetAttribute("aria-label").ShouldBe("Loading");

        // Assert - should contain SVG element
        var svg = cut.Find(".spinner-svg");
        svg.ShouldNotBeNull();
    }

    [Fact(DisplayName = "And it should support size variants")]
    [Trait(Spec.UAC, "13")]
    public async Task UAC13_ShouldSupportSizeVariants()
    {
        await ArrangeAndActAsync();
        using var ctx = new BunitContext();
        
        // Test small size
        var smallSpinner = ctx.Render<Spinner>(parameters => parameters
            .Add(p => p.Size, SpinnerSize.Small));
        smallSpinner.Find(".spinner-sm").ShouldNotBeNull();

        // Test medium size (default)
        var mediumSpinner = ctx.Render<Spinner>(parameters => parameters
            .Add(p => p.Size, SpinnerSize.Medium));
        mediumSpinner.Find(".spinner-md").ShouldNotBeNull();

        // Test large size
        var largeSpinner = ctx.Render<Spinner>(parameters => parameters
            .Add(p => p.Size, SpinnerSize.Large));
        largeSpinner.Find(".spinner-lg").ShouldNotBeNull();
    }

    [Fact(DisplayName = "And it should merge the Class parameter with generated classes")]
    [Trait(Spec.UAC, "14")]
    public async Task UAC14_ShouldMergeCustomClass()
    {
        await ArrangeAndActAsync();
        using var ctx = new BunitContext();
        
        // Arrange & Act
        var cut = ctx.Render<Spinner>(parameters => parameters
            .Add(p => p.Class, "custom-spinner another-class"));

        // Assert - should have both base and custom classes
        var spinner = cut.Find(".spinner");
        spinner.ClassList.ShouldContain("spinner");
        spinner.ClassList.ShouldContain("custom-spinner");
        spinner.ClassList.ShouldContain("another-class");
    }
}
