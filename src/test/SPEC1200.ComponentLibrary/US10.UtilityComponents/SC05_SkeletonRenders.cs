using Bunit;
using LowlandTech.Components.Components.Skeleton;
using Shouldly;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US10.UtilityComponents;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "10")]
[Trait(Spec.SC, "05")]
public class SC05_SkeletonRenders : Scenario
{
    private BunitContext _ctx = null!;

    protected override Task GivenAsync()
    {
        _ctx = new BunitContext();
        return Task.CompletedTask;
    }

    protected override Task WhenAsync()
    {
        return Task.CompletedTask;
    }

    public override async Task DisposeAsync()
    {
        _ctx?.Dispose();
        await base.DisposeAsync();
    }

    [Fact(DisplayName = "UAC18: Then it should render a div element with class skeleton")]
    [Trait(Spec.UAC, "18")]
    public async Task UAC18_ShouldRenderSkeletonDiv()
    {
        await ArrangeAndActAsync();
        var component = _ctx.Render<Skeleton>();

        var element = component.Find(".skeleton");
        element.ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC19: And it should support width and height parameters")]
    [Trait(Spec.UAC, "19")]
    public async Task UAC19_ShouldSupportWidthAndHeight()
    {
        await ArrangeAndActAsync();
        var component = _ctx.Render<Skeleton>(parameters => parameters
            .Add(p => p.Width, "200px")
            .Add(p => p.Height, "100px")
        );

        var element = component.Find(".skeleton");
        var style = element.GetAttribute("style");
        style.ShouldContain("width: 200px");
        style.ShouldContain("height: 100px");
    }

    [Fact(DisplayName = "UAC20: And it should support variant classes")]
    [Trait(Spec.UAC, "20")]
    public async Task UAC20_ShouldSupportVariantClasses()
    {
        await ArrangeAndActAsync();
        
        using var ctx1 = new BunitContext();
        var text = ctx1.Render<Skeleton>(parameters => parameters.Add(p => p.Variant, "text"));
        text.Find(".skeleton").ClassList.ShouldContain("skeleton-text");

        using var ctx2 = new BunitContext();
        var rect = ctx2.Render<Skeleton>(parameters => parameters.Add(p => p.Variant, "rect"));
        rect.Find(".skeleton").ClassList.ShouldContain("skeleton-rect");

        using var ctx3 = new BunitContext();
        var circle = ctx3.Render<Skeleton>(parameters => parameters.Add(p => p.Variant, "circle"));
        circle.Find(".skeleton").ClassList.ShouldContain("skeleton-circle");
    }

    [Fact(DisplayName = "UAC21: And it should support animation classes")]
    [Trait(Spec.UAC, "21")]
    public async Task UAC21_ShouldSupportAnimationClasses()
    {
        await ArrangeAndActAsync();
        
        using var ctx1 = new BunitContext();
        var pulse = ctx1.Render<Skeleton>(parameters => parameters.Add(p => p.Animation, "pulse"));
        pulse.Find(".skeleton").ClassList.ShouldContain("skeleton-pulse");

        using var ctx2 = new BunitContext();
        var wave = ctx2.Render<Skeleton>(parameters => parameters.Add(p => p.Animation, "wave"));
        wave.Find(".skeleton").ClassList.ShouldContain("skeleton-wave");

        using var ctx3 = new BunitContext();
        var none = ctx3.Render<Skeleton>(parameters => parameters.Add(p => p.Animation, "none"));
        none.Find(".skeleton").ClassList.ShouldNotContain("skeleton-none");
    }
}
