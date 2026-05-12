using Bunit;
using LowlandTech.Components.Pagination;
using PaginationComponent = LowlandTech.Components.Pagination.Pagination;

namespace LowlandTech.Components.Tests.SPEC1034.Pagination.US01.View;

[Trait(Spec.SPEC, "1034")]
[Trait(Spec.SC, "02")]
[UserStory("01", "Render Pagination subcomponents")]
public class SC02_RenderPaginationSubcomponents : ViewScenario
{
    [Fact(DisplayName = "UAC01: PaginationContent renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_PaginationContentRendersDataSlot()
    {
        var cut = Ctx.Render<PaginationContent>();
        cut.Find("[data-slot='pagination-content']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: PaginationItem renders data-slot attribute")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_PaginationItemRendersDataSlot()
    {
        var cut = Ctx.Render<PaginationItem>();
        cut.Find("[data-slot='pagination-item']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC03: PaginationLink renders with IsActive")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_PaginationLinkRendersWithIsActive()
    {
        var cut = Ctx.Render<PaginationLink>(p => p.Add(x => x.IsActive, true));
        cut.Find("[data-active='true']").ShouldNotBeNull();
        cut.Find("[aria-current='page']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC04: PaginationPrevious renders with text")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_PaginationPreviousRendersWithText()
    {
        var cut = Ctx.Render<PaginationPrevious>(p => p.Add(x => x.Text, "Prev"));
        cut.Markup.ShouldContain("Prev");
    }

    [Fact(DisplayName = "UAC05: PaginationNext renders with text")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_PaginationNextRendersWithText()
    {
        var cut = Ctx.Render<PaginationNext>(p => p.Add(x => x.Text, "Next"));
        cut.Markup.ShouldContain("Next");
    }

    [Fact(DisplayName = "UAC06: PaginationEllipsis renders data-slot attribute")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_PaginationEllipsisRendersDataSlot()
    {
        var cut = Ctx.Render<PaginationEllipsis>();
        cut.Find("[data-slot='pagination-ellipsis']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC07: PaginationPrevious is disabled on first page")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_PaginationPreviousDisabledOnFirstPage()
    {
        var cut = Ctx.Render<PaginationComponent>(p => p
            .Add(x => x.Page, 1)
            .Add(x => x.Count, 5)
            .AddChildContent<PaginationPrevious>());

        var previous = cut.Find("[data-slot='pagination-previous']");
        previous.ClassList.ShouldContain("pointer-events-none");
    }

    [Fact(DisplayName = "UAC08: PaginationNext is disabled on last page")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_PaginationNextDisabledOnLastPage()
    {
        var cut = Ctx.Render<PaginationComponent>(p => p
            .Add(x => x.Page, 5)
            .Add(x => x.Count, 5)
            .AddChildContent<PaginationNext>());

        var next = cut.Find("[data-slot='pagination-next']");
        next.ClassList.ShouldContain("pointer-events-none");
    }

    [Fact(DisplayName = "UAC09: PaginationContent renders as ul")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_PaginationContentRendersAsUl()
    {
        var cut = Ctx.Render<PaginationContent>();
        cut.Find("ul[data-slot='pagination-content']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC10: PaginationItem renders as li")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_PaginationItemRendersAsLi()
    {
        var cut = Ctx.Render<PaginationItem>();
        cut.Find("li[data-slot='pagination-item']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC11: PaginationEllipsis includes screen-reader label")]
    [Trait(Spec.UAC, "11")]
    public void UAC11_PaginationEllipsisHasScreenReaderLabel()
    {
        var cut = Ctx.Render<PaginationEllipsis>();
        cut.Find(".sr-only").TextContent.Trim().ShouldBe("More pages");
    }

    [Fact(DisplayName = "UAC12: PaginationEllipsis icon is aria-hidden")]
    [Trait(Spec.UAC, "12")]
    public void UAC12_PaginationEllipsisIconIsAriaHidden()
    {
        var cut = Ctx.Render<PaginationEllipsis>();
        cut.Find("[data-slot='pagination-ellipsis'] svg").GetAttribute("aria-hidden").ShouldBe("true");
    }
}
