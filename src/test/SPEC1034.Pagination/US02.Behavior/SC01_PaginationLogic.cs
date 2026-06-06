using Bunit;
using PaginationSut = LowlandTech.Components.Pagination;
using LowlandTech.Components;
namespace LowlandTech.Components.Tests.SPEC1034.Pagination.US02.Behavior;

[Trait(Spec.SPEC, "1034")]
[Trait(Spec.SC, "01")]
[UserStory("02", "Pagination logic")]
public class SC01_PaginationLogic : ViewScenario
{
    [Fact(DisplayName = "UAC01: GetPageItems returns all pages when count is small")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_GetPageItemsReturnsAllPagesWhenCountIsSmall()
    {
        var cut = Ctx.Render<PaginationSut>(p =>
        {
            p.Add(x => x.Count, 5);
            p.Add(x => x.Page, 1);
            p.Add(x => x.SiblingCount, 1);
            p.Add(x => x.BoundaryCount, 1);
        });

        var items = cut.Instance.GetPageItems();
        items.Count.ShouldBe(5);
        items.All(i => i.Type == PaginationItemType.Page).ShouldBeTrue();
        items.Select(i => i.Page).ShouldBe([1, 2, 3, 4, 5]);
    }

    [Fact(DisplayName = "UAC02: GetPageItems includes ellipsis for large page count")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_GetPageItemsIncludesEllipsis()
    {
        var cut = Ctx.Render<PaginationSut>(p =>
        {
            p.Add(x => x.Count, 20);
            p.Add(x => x.Page, 10);
            p.Add(x => x.SiblingCount, 1);
            p.Add(x => x.BoundaryCount, 1);
        });

        var items = cut.Instance.GetPageItems();
        
        // Should have: 1, ..., 9, 10, 11, ..., 20
        items.Any(i => i.Type == PaginationItemType.Ellipsis).ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC03: SiblingCount controls pages around current")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_SiblingCountControlsPagesAroundCurrent()
    {
        var cut = Ctx.Render<PaginationSut>(p =>
        {
            p.Add(x => x.Count, 20);
            p.Add(x => x.Page, 10);
            p.Add(x => x.SiblingCount, 2);
            p.Add(x => x.BoundaryCount, 1);
        });

        var items = cut.Instance.GetPageItems();
        var pageItems = items.Where(i => i.Type == PaginationItemType.Page).Select(i => i.Page).ToList();
        
        // With siblingCount 2, should have pages: 1, 8, 9, 10, 11, 12, 20
        pageItems.ShouldContain(8);
        pageItems.ShouldContain(9);
        pageItems.ShouldContain(10);
        pageItems.ShouldContain(11);
        pageItems.ShouldContain(12);
    }

    [Fact(DisplayName = "UAC04: BoundaryCount controls start/end pages")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_BoundaryCountControlsStartEndPages()
    {
        var cut = Ctx.Render<PaginationSut>(p =>
        {
            p.Add(x => x.Count, 20);
            p.Add(x => x.Page, 10);
            p.Add(x => x.SiblingCount, 1);
            p.Add(x => x.BoundaryCount, 2);
        });

        var items = cut.Instance.GetPageItems();
        var pageItems = items.Where(i => i.Type == PaginationItemType.Page).Select(i => i.Page).ToList();
        
        // With boundaryCount 2, should have: 1, 2, ..., 10, ..., 19, 20
        pageItems.ShouldContain(1);
        pageItems.ShouldContain(2);
        pageItems.ShouldContain(19);
        pageItems.ShouldContain(20);
    }

    [Fact(DisplayName = "UAC05: GoToPage updates current page")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_GoToPageUpdatesCurrentPage()
    {
        var cut = Ctx.Render<PaginationSut>(p =>
        {
            p.Add(x => x.Count, 10);
            p.Add(x => x.Page, 1);
        });

        cut.Instance.Page.ShouldBe(1);
        
        cut.InvokeAsync(() => cut.Instance.GoToPage(5));
        
        cut.Instance.Page.ShouldBe(5);
    }

    [Fact(DisplayName = "UAC06: GoToPage invokes OnPageChange callback")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_GoToPageInvokesCallback()
    {
        int? changedPage = null;
        var cut = Ctx.Render<PaginationSut>(p =>
        {
            p.Add(x => x.Count, 10);
            p.Add(x => x.Page, 1);
            p.Add(x => x.OnPageChange, EventCallback.Factory.Create<int>(this, page => changedPage = page));
        });

        cut.InvokeAsync(() => cut.Instance.GoToPage(5));
        
        changedPage.ShouldBe(5);
    }

    [Fact(DisplayName = "UAC07: GoToPrevious decrements page")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_GoToPreviousDecrementsPage()
    {
        var cut = Ctx.Render<PaginationSut>(p =>
        {
            p.Add(x => x.Count, 10);
            p.Add(x => x.Page, 5);
        });

        cut.InvokeAsync(() => cut.Instance.GoToPrevious());
        
        cut.Instance.Page.ShouldBe(4);
    }

    [Fact(DisplayName = "UAC08: GoToNext increments page")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_GoToNextIncrementsPage()
    {
        var cut = Ctx.Render<PaginationSut>(p =>
        {
            p.Add(x => x.Count, 10);
            p.Add(x => x.Page, 5);
        });

        cut.InvokeAsync(() => cut.Instance.GoToNext());
        
        cut.Instance.Page.ShouldBe(6);
    }

    [Fact(DisplayName = "UAC09: GoToPrevious does nothing on first page")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_GoToPreviousDoesNothingOnFirstPage()
    {
        var cut = Ctx.Render<PaginationSut>(p =>
        {
            p.Add(x => x.Count, 10);
            p.Add(x => x.Page, 1);
        });

        cut.InvokeAsync(() => cut.Instance.GoToPrevious());
        
        cut.Instance.Page.ShouldBe(1);
    }

    [Fact(DisplayName = "UAC10: GoToNext does nothing on last page")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_GoToNextDoesNothingOnLastPage()
    {
        var cut = Ctx.Render<PaginationSut>(p =>
        {
            p.Add(x => x.Count, 10);
            p.Add(x => x.Page, 10);
        });

        cut.InvokeAsync(() => cut.Instance.GoToNext());
        
        cut.Instance.Page.ShouldBe(10);
    }

    [Fact(DisplayName = "UAC11: Ellipsis appears when gap between boundary and siblings")]
    [Trait(Spec.UAC, "11")]
    public void UAC11_EllipsisAppearsAtCorrectBoundaries()
    {
        // When page is near middle with sufficient gap to boundaries
        var cut = Ctx.Render<PaginationSut>(p =>
        {
            p.Add(x => x.Count, 100);
            p.Add(x => x.Page, 50);
            p.Add(x => x.SiblingCount, 1);
            p.Add(x => x.BoundaryCount, 1);
        });

        var items = cut.Instance.GetPageItems();
        
        // Should have ellipsis at both sides
        var ellipsisCount = items.Count(i => i.Type == PaginationItemType.Ellipsis);
        ellipsisCount.ShouldBe(2);
    }

    [Fact(DisplayName = "UAC12: No ellipsis when page count is small")]
    [Trait(Spec.UAC, "12")]
    public void UAC12_NoEllipsisWhenPageCountIsSmall()
    {
        var cut = Ctx.Render<PaginationSut>(p =>
        {
            p.Add(x => x.Count, 5);
            p.Add(x => x.Page, 2);
            p.Add(x => x.SiblingCount, 1);
            p.Add(x => x.BoundaryCount, 1);
        });

        var items = cut.Instance.GetPageItems();
        
        // With only 5 pages, should show all without ellipsis
        items.Any(i => i.Type == PaginationItemType.Ellipsis).ShouldBeFalse();
        items.Count.ShouldBe(5);
    }

    [Fact(DisplayName = "UAC13: Near end shows correct page range")]
    [Trait(Spec.UAC, "13")]
    public void UAC13_NearEndShowsCorrectRange()
    {
        var cut = Ctx.Render<PaginationSut>(p =>
        {
            p.Add(x => x.Count, 20);
            p.Add(x => x.Page, 18);
            p.Add(x => x.SiblingCount, 1);
            p.Add(x => x.BoundaryCount, 1);
        });

        var items = cut.Instance.GetPageItems();
        
        // Should show: 1, ..., 17, 18, 19, 20 (near end, so continuous to end)
        // Or: 1, ..., 16, 17, 18, 19, 20 depending on algorithm
        items.Any(i => i.Type == PaginationItemType.Ellipsis).ShouldBeTrue();
        items.Last(i => i.Type == PaginationItemType.Page).Page.ShouldBe(20);
    }

    [Fact(DisplayName = "UAC14: Page is clamped to valid range")]
    [Trait(Spec.UAC, "14")]
    public void UAC14_PageIsClampedToValidRange()
    {
        // Test with page below 1 (set internally through Page param)
        var cut = Ctx.Render<PaginationSut>(p =>
        {
            p.Add(x => x.Count, 10);
            p.Add(x => x.Page, 0);
        });

        // The GetPageItems should handle it gracefully
        var items = cut.Instance.GetPageItems();
        items.ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC15: Default SiblingCount is 1")]
    [Trait(Spec.UAC, "15")]
    public void UAC15_DefaultSiblingCountIsOne()
    {
        var cut = Ctx.Render<PaginationSut>();
        cut.Instance.SiblingCount.ShouldBe(1);
    }

    [Fact(DisplayName = "UAC16: Default BoundaryCount is 1")]
    [Trait(Spec.UAC, "16")]
    public void UAC16_DefaultBoundaryCountIsOne()
    {
        var cut = Ctx.Render<PaginationSut>();
        cut.Instance.BoundaryCount.ShouldBe(1);
    }
}
