using Bunit;
using PaginationSut = LowlandTech.Components.Pagination.Pagination;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1034.Pagination.US01.View;

[Trait(Spec.SPEC, "1034")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Pagination component")]
public class SC01_RenderPagination : ViewScenario
{
    [Fact(DisplayName = "UAC01: Pagination renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<PaginationSut>();
        cut.Find("[data-slot='pagination']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Pagination renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<PaginationSut>(p => p.AddChildContent("Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact(DisplayName = "UAC03: Pagination renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains the CSS class")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<PaginationSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='pagination']").ClassList.ShouldContain("test-class");
    }
}
