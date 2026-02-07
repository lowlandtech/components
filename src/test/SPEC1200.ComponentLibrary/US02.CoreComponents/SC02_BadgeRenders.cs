using Bunit;
using Shouldly;
using BadgeComponent = LowlandTech.Components.Badge.Badge;
using LowlandTech.Components.Badge;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US02.CoreComponents;

/// <summary>
/// Tests for Badge component rendering and variants.
/// SC02 - Implement Badge component
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "02")]
[Trait(Spec.SC, "02")]
public class SC02_BadgeRenders : Scenario
{
    private TestContext _ctx = null!;
    private IRenderedComponent<BadgeComponent>? _badge;

    protected override Task GivenAsync()
    {
        _ctx = new TestContext();
        return Task.CompletedTask;
    }

    protected override Task WhenAsync()
    {
        _badge = _ctx.Render<BadgeComponent>(parameters => parameters
            .Add(p => p.ChildContent, "Badge")
        );
        return Task.CompletedTask;
    }

    public override async Task DisposeAsync()
    {
        _ctx?.Dispose();
        await base.DisposeAsync();
    }

    [Fact(DisplayName = "Then it should render a <span> element by default")]
    [Trait(Spec.UAC, "07")]
    public async Task UAC07_ShouldRenderSpanElement()
    {
        await ArrangeAndActAsync();
        _badge.ShouldNotBeNull();
        
        var span = _badge!.Find("span");
        span.ShouldNotBeNull();
        span.TagName.ShouldBe("SPAN");
    }

    [Fact(DisplayName = "And it should apply base \"badge\" class")]
    [Trait(Spec.UAC, "08")]
    public async Task UAC08_ShouldApplyBaseClass()
    {
        _ctx = new TestContext();

        _badge = _ctx.Render<BadgeComponent>(parameters => parameters
            .Add(p => p.ChildContent, "Default")
        );
        _badge.ShouldNotBeNull();

        var span = _badge!.Find("span");
        span.ClassList.ShouldContain("badge-default");
        var classAttr = span.GetAttribute("class");
        classAttr.ShouldContain("badge");
    }

    [Fact(DisplayName = "And it should support variant classes (badge-default, badge-secondary, badge-destructive, badge-success, badge-outline)")]
    [Trait(Spec.UAC, "09")]
    public async Task UAC09_ShouldSupportVariantClasses()
    {
        _ctx = new TestContext();

        _badge = _ctx.Render<BadgeComponent>(parameters => parameters
            .Add(p => p.Variant, BadgeVariants.Default)
            .Add(p => p.ChildContent, "Default")
        );
        _badge!.Find("span").ClassList.ShouldContain("badge-default");

        _badge = _ctx.Render<BadgeComponent>(parameters => parameters
            .Add(p => p.Variant, BadgeVariants.Secondary)
            .Add(p => p.ChildContent, "Secondary")
        );
        _badge!.Find("span").ClassList.ShouldContain("badge-secondary");

        _badge = _ctx.Render<BadgeComponent>(parameters => parameters
            .Add(p => p.Variant, BadgeVariants.Destructive)
            .Add(p => p.ChildContent, "Destructive")
        );
        _badge!.Find("span").ClassList.ShouldContain("badge-destructive");

        _badge = _ctx.Render<BadgeComponent>(parameters => parameters
            .Add(p => p.Variant, BadgeVariants.Success)
            .Add(p => p.ChildContent, "Success")
        );
        _badge!.Find("span").ClassList.ShouldContain("badge-success");

        _badge = _ctx.Render<BadgeComponent>(parameters => parameters
            .Add(p => p.Variant, BadgeVariants.Outline)
            .Add(p => p.ChildContent, "Outline")
        );
        _badge!.Find("span").ClassList.ShouldContain("badge-outline");
    }

    [Fact(DisplayName = "And it should merge Class parameter with generated classes")]
    [Trait(Spec.UAC, "10")]
    public async Task UAC10_ShouldMergeClassParameter()
    {
        _ctx = new TestContext();

        _badge = _ctx.Render<BadgeComponent>(parameters => parameters
            .Add(p => p.Class, "custom-class")
            .Add(p => p.Variant, BadgeVariants.Default)
            .Add(p => p.ChildContent, "Custom")
        );
        _badge.ShouldNotBeNull();
        
        var span = _badge!.Find("span");
        span.ClassList.ShouldContain("badge-default");
        var classAttr = span.GetAttribute("class");
        classAttr.ShouldContain("badge");
        classAttr.ShouldContain("custom-class");
    }
}
