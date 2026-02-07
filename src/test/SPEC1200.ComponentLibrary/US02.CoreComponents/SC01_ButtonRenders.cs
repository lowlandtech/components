using Bunit;
using LowlandTech.Components.Components.Button;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US02.CoreComponents;

/// <summary>
/// Tests for Button component rendering and base functionality.
/// SC01 - Implement Button component
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "02")]
[Trait(Spec.SC, "01")]
public class SC01_ButtonRenders : Scenario
{
    private BunitContext _ctx = null!;
    private IRenderedComponent<Button>? _component;

    protected override Task GivenAsync()
    {
        _ctx = new BunitContext();
        return Task.CompletedTask;
    }

    protected override Task WhenAsync()
    {
        _component = _ctx.Render<Button>(parameters => parameters
            .Add(p => p.ChildContent, "Click me")
        );
        return Task.CompletedTask;
    }

    public override async Task DisposeAsync()
    {
        _ctx?.Dispose();
        await base.DisposeAsync();
    }

    [Fact(DisplayName = "Then it should render a <button> element by default")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_ShouldRenderButtonElement()
    {
        await ArrangeAndActAsync();
        _component.ShouldNotBeNull();
        
        var button = _component!.Find("button");
        button.ShouldNotBeNull();
    }

    [Fact(DisplayName = "And it should apply the base \"btn\" class")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_ShouldApplyBaseClass()
    {
        await ArrangeAndActAsync();
        
        var button = _component!.Find("button");
        button.ClassList.ShouldContain("btn");
    }

    [Fact(DisplayName = "And it should support variant classes (btn-default, btn-secondary, btn-outline, btn-ghost, btn-link, btn-destructive)")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_ShouldSupportVariantClasses()
    {
        // Test default variant
        using var ctx1 = new BunitContext();
        var defaultBtn = ctx1.Render<Button>(parameters => parameters
            .Add(p => p.Variant, ButtonVariant.Default)
        );
        defaultBtn.Find("button").ClassList.ShouldContain("btn-default");

        // Test secondary variant
        using var ctx2 = new BunitContext();
        var secondaryBtn = ctx2.Render<Button>(parameters => parameters
            .Add(p => p.Variant, ButtonVariant.Secondary)
        );
        secondaryBtn.Find("button").ClassList.ShouldContain("btn-secondary");

        // Test outline variant
        using var ctx3 = new BunitContext();
        var outlineBtn = ctx3.Render<Button>(parameters => parameters
            .Add(p => p.Variant, ButtonVariant.Outline)
        );
        outlineBtn.Find("button").ClassList.ShouldContain("btn-outline");

        // Test ghost variant
        using var ctx4 = new BunitContext();
        var ghostBtn = ctx4.Render<Button>(parameters => parameters
            .Add(p => p.Variant, ButtonVariant.Ghost)
        );
        ghostBtn.Find("button").ClassList.ShouldContain("btn-ghost");

        // Test link variant
        using var ctx5 = new BunitContext();
        var linkBtn = ctx5.Render<Button>(parameters => parameters
            .Add(p => p.Variant, ButtonVariant.Link)
        );
        linkBtn.Find("button").ClassList.ShouldContain("btn-link");

        // Test destructive variant
        using var ctx6 = new BunitContext();
        var destructiveBtn = ctx6.Render<Button>(parameters => parameters
            .Add(p => p.Variant, ButtonVariant.Destructive)
        );
        destructiveBtn.Find("button").ClassList.ShouldContain("btn-destructive");
    }

    [Fact(DisplayName = "And it should support size classes (btn-sm, btn-lg, btn-icon)")]
    [Trait(Spec.UAC, "04")]
    public async Task UAC04_ShouldSupportSizeClasses()
    {
        // Test small size
        using var ctx1 = new BunitContext();
        var smallBtn = ctx1.Render<Button>(parameters => parameters
            .Add(p => p.Size, ButtonSize.Small)
        );
        smallBtn.Find("button").ClassList.ShouldContain("btn-sm");

        // Test large size
        using var ctx2 = new BunitContext();
        var largeBtn = ctx2.Render<Button>(parameters => parameters
            .Add(p => p.Size, ButtonSize.Large)
        );
        largeBtn.Find("button").ClassList.ShouldContain("btn-lg");

        // Test icon size
        using var ctx3 = new BunitContext();
        var iconBtn = ctx3.Render<Button>(parameters => parameters
            .Add(p => p.Size, ButtonSize.Icon)
        );
        iconBtn.Find("button").ClassList.ShouldContain("btn-icon");
    }

    [Fact(DisplayName = "And it should merge the Class parameter with generated classes")]
    [Trait(Spec.UAC, "05")]
    public async Task UAC05_ShouldMergeCustomClass()
    {
        using var ctx = new BunitContext();
        var component = ctx.Render<Button>(parameters => parameters
            .Add(p => p.Class, "custom-class another-class")
        );

        var button = component.Find("button");
        button.ClassList.ShouldContain("btn");
        button.ClassList.ShouldContain("btn-default");
        button.ClassList.ShouldContain("custom-class");
        button.ClassList.ShouldContain("another-class");
    }

    [Fact(DisplayName = "And it should forward unmatched attributes to the underlying element")]
    [Trait(Spec.UAC, "06")]
    public async Task UAC06_ShouldForwardUnmatchedAttributes()
    {
        using var ctx = new BunitContext();
        var component = ctx.Render<Button>(parameters => parameters
            .AddUnmatched("data-testid", "my-button")
            .AddUnmatched("aria-label", "Submit form")
        );

        var button = component.Find("button");
        button.GetAttribute("data-testid").ShouldBe("my-button");
        button.GetAttribute("aria-label").ShouldBe("Submit form");
    }
}
