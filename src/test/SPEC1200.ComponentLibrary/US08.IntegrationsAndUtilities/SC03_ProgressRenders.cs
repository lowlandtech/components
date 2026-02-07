using Bunit;
using LowlandTech.Components;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US08.IntegrationsAndUtilities;

/// <summary>
/// Tests for Progress component rendering and functionality.
/// SC03 - Implement Progress component
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "08")]
[Trait(Spec.SC, "03")]
public class SC03_ProgressRenders : Scenario
{
    protected override Task GivenAsync()
    {
        return Task.CompletedTask;
    }

    protected override Task WhenAsync()
    {
        return Task.CompletedTask;
    }

    [Fact(DisplayName = "Then it should render a progress track and indicator")]
    [Trait(Spec.UAC, "09")]
    public async Task UAC09_ShouldRenderProgressTrackAndIndicator()
    {
        await ArrangeAndActAsync();
        using var ctx = new BunitContext();
        
        // Arrange & Act
        var cut = ctx.Render<Progress>(parameters => parameters
            .Add(p => p.Value, 50));

        // Assert - should render progress track with "progress" class
        var progressTrack = cut.Find(".progress");
        progressTrack.ShouldNotBeNull();
        progressTrack.GetAttribute("role").ShouldBe("progressbar");

        // Assert - should render progress indicator
        var indicator = cut.Find(".progress-indicator");
        indicator.ShouldNotBeNull();
    }

    [Fact(DisplayName = "And it should support a Value parameter from 0 to 100")]
    [Trait(Spec.UAC, "10")]
    public async Task UAC10_ShouldSupportValueParameter()
    {
        await ArrangeAndActAsync();
        using var ctx = new BunitContext();
        
        // Arrange & Act
        var cut = ctx.Render<Progress>(parameters => parameters
            .Add(p => p.Value, 75));

        // Assert - indicator width should match value percentage
        var indicator = cut.Find(".progress-indicator");
        var style = indicator.GetAttribute("style");
        style.ShouldContain("width: 75%");
    }

    [Fact(DisplayName = "And it should support setting aria-valuenow and aria-valuemax")]
    [Trait(Spec.UAC, "11")]
    public async Task UAC11_ShouldSupportAriaAttributes()
    {
        await ArrangeAndActAsync();
        using var ctx = new BunitContext();
        
        // Arrange & Act
        var cut = ctx.Render<Progress>(parameters => parameters
            .Add(p => p.Value, 33));

        // Assert - should have proper ARIA attributes
        var progressTrack = cut.Find(".progress");
        progressTrack.GetAttribute("aria-valuenow").ShouldBe("33");
        progressTrack.GetAttribute("aria-valuemin").ShouldBe("0");
        progressTrack.GetAttribute("aria-valuemax").ShouldBe("100");
    }
}
