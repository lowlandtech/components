using Bunit;
using SkeletonSut = LowlandTech.Components.Skeleton.Skeleton;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1044.Skeleton.US01.View;

[Trait(Spec.SPEC, "1044")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Skeleton component")]
public class SC01_RenderSkeleton : ViewScenario
{
    [Fact(DisplayName = "UAC01: Skeleton renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<SkeletonSut>();
        cut.Find("[data-slot='skeleton']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Skeleton renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<SkeletonSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: Skeleton renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<SkeletonSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='skeleton']").ClassList.ShouldContain("test-class");
    }
}
