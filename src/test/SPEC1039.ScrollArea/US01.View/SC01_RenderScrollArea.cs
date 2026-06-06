using Bunit;
using ScrollAreaSut = LowlandTech.Components.ScrollArea;

namespace LowlandTech.Components.Tests.SPEC1039.ScrollArea.US01.View;

[Trait(Spec.SPEC, "1039")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render ScrollArea component")]
public class SC01_RenderScrollArea : ViewScenario
{
    [Fact(DisplayName = "UAC01: ScrollArea renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<ScrollAreaSut>();
        cut.Find("[data-slot='scroll-area']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: ScrollArea renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<ScrollAreaSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: ScrollArea renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<ScrollAreaSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='scroll-area']").ClassList.ShouldContain("test-class");
    }

    [Fact(DisplayName = "UAC04: ScrollArea has cn-scroll-area base class")]
    [Trait(Spec.UAC, "04")]
    [Then("the root element has cn-scroll-area class")]
    public void UAC04_HasBaseClass()
    {
        var cut = Ctx.Render<ScrollAreaSut>();
        cut.Find("[data-slot='scroll-area']").ClassList.ShouldContain("cn-scroll-area");
    }

    [Fact(DisplayName = "UAC05: ScrollArea renders viewport element")]
    [Trait(Spec.UAC, "05")]
    [Then("a scroll-area-viewport child element is present")]
    public void UAC05_RendersViewport()
    {
        var cut = Ctx.Render<ScrollAreaSut>();
        cut.Find("[data-slot='scroll-area-viewport']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC06: ScrollArea renders scrollbar and thumb elements")]
    [Trait(Spec.UAC, "06")]
    [Then("scroll-area-scrollbar and scroll-area-thumb are present")]
    public void UAC06_RendersScrollbarAndThumb()
    {
        var cut = Ctx.Render<ScrollAreaSut>();
        cut.Find("[data-slot='scroll-area-scrollbar']").ShouldNotBeNull();
        cut.Find("[data-slot='scroll-area-thumb']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC07: ScrollArea viewport contains ChildContent")]
    [Trait(Spec.UAC, "07")]
    [Then("child content is rendered inside the viewport")]
    public void UAC07_ChildContentInViewport()
    {
        var cut = Ctx.Render<ScrollAreaSut>(p => p.AddChildContent("<p>content</p>"));
        cut.Find("[data-slot='scroll-area-viewport']").InnerHtml.ShouldContain("content");
    }
}
