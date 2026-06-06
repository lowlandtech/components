using LowlandTech.Components;
namespace LowlandTech.Components.Tests.SPEC1081.RepositoryApp.US01.View;

[Trait(Spec.SPEC, "1081")]
[Trait(Spec.SC, "12")]
[UserStory("01", "Footer paginator drives page navigation")]
public class SC12_Pagination : Infrastructure.RepositoryScenario
{
    [Fact(DisplayName = "UAC01: Footer renders the page count label")]
    [Trait(Spec.UAC, "01")]
    [Given("a RepositoryApp is rendered on a listing view")]
    [When("the paginator footer is inspected")]
    [Then("a page count label should be visible")]
    public void UAC01_FooterRendersPageCount()
    {
        var cut = RenderRepositoryApp();

        var count = cut.Find("[data-testid='repo-footer-count']");
        count.ShouldNotBeNull();
        count.TextContent.ShouldContain("page 1 of 1");
    }

    [Fact(DisplayName = "UAC02: Page 1 disables the previous button via styling")]
    [Trait(Spec.UAC, "02")]
    [Given("the paginator is on page 1")]
    [When("the previous button is inspected")]
    [Then("it should be visually disabled via styling")]
    public void UAC02_PageOneDisablesPrev()
    {
        var cut = RenderRepositoryApp();
        StateService.SetPage(1);
        cut.Render();

        var prev = cut.Find("[data-testid='repo-footer-prev']");
        prev.ClassList.ShouldContain("pointer-events-none");
        prev.ClassList.ShouldContain("opacity-50");
    }

    [Fact(DisplayName = "UAC03: Clicking next advances the page number")]
    [Trait(Spec.UAC, "03")]
    [Given("the paginator is on page 1 with more items available")]
    [When("the next button is clicked")]
    [Then("the page number should advance to 2")]
    public void UAC03_ClickingNextAdvancesPage()
    {
        var cut = RenderRepositoryApp();
        StateService.SetPageSize(5);
        cut.Render();

        cut.Find("[data-testid='repo-footer-next']").Click();
        cut.Render();

        StateService.State.Ui.PageNumber.ShouldBe(2);
    }

    [Fact(DisplayName = "UAC04: Last page disables the next button via styling")]
    [Trait(Spec.UAC, "04")]
    [Given("the paginator is on the last page")]
    [When("the next button is inspected")]
    [Then("it should be visually disabled via styling")]
    public void UAC04_LastPageDisablesNext()
    {
        var cut = RenderRepositoryApp();
        StateService.SetPageSize(1);
        var pager = StateService.GetPager();
        StateService.SetPage(pager.PageCount);
        cut.Render();

        var next = cut.Find("[data-testid='repo-footer-next']");
        next.ClassList.ShouldContain("pointer-events-none");
        next.ClassList.ShouldContain("opacity-50");
    }

    [Fact(DisplayName = "UAC05: Clicking the first page link returns to page 1")]
    [Trait(Spec.UAC, "05")]
    [Given("the paginator is on a page greater than 1")]
    [When("the first page link is clicked")]
    [Then("the page number should return to 1")]
    public void UAC05_ClickingFirstPageLinkReturnsToPage1()
    {
        var cut = RenderRepositoryApp();
        StateService.SetPageSize(5);
        StateService.SetPage(2);
        cut.Render();

        cut.Find("[data-testid='repo-footer-page-1']").Click();
        cut.Render();

        StateService.State.Ui.PageNumber.ShouldBe(1);
    }

    [Fact(DisplayName = "UAC06: Clicking the last page link jumps to the last page")]
    [Trait(Spec.UAC, "06")]
    [Given("the paginator is on page 1 with more items available")]
    [When("the last page link is clicked")]
    [Then("the page number should jump to the last page")]
    public void UAC06_ClickingLastPageLinkJumpsToLastPage()
    {
        var cut = RenderRepositoryApp();
        StateService.SetPageSize(5);
        cut.Render();

        var pager = StateService.GetPager();
        cut.Find($"[data-testid='repo-footer-page-{pager.PageCount}']").Click();
        cut.Render();

        StateService.State.Ui.PageNumber.ShouldBe(pager.PageCount);
    }

    [Fact(DisplayName = "UAC07: Clicking previous decreases the page number")]
    [Trait(Spec.UAC, "07")]
    [Given("the paginator is on page 2")]
    [When("the previous button is clicked")]
    [Then("the page number should decrease to 1")]
    public void UAC07_ClickingPreviousDecreasesPage()
    {
        var cut = RenderRepositoryApp();
        StateService.SetPageSize(5);
        StateService.SetPage(2);
        cut.Render();

        cut.Find("[data-testid='repo-footer-prev']").Click();
        cut.Render();

        StateService.State.Ui.PageNumber.ShouldBe(1);
    }

    [Fact(DisplayName = "UAC08: Active page link is marked with data-active")]
    [Trait(Spec.UAC, "08")]
    [Given("the paginator is on page 2")]
    [When("the page link is inspected")]
    [Then("page 2 should be marked active")]
    public void UAC08_ActivePageLinkIsMarked()
    {
        var cut = RenderRepositoryApp();
        StateService.SetPageSize(5);
        StateService.SetPage(2);
        cut.Render();

        var pageTwo = cut.Find("[data-testid='repo-footer-page-2']");
        pageTwo.GetAttribute("data-active").ShouldBe("true");
    }

    [Fact(DisplayName = "UAC09: Empty file set renders '0 items' in the count label")]
    [Trait(Spec.UAC, "09")]
    [Given("the repository has no files")]
    [When("the paginator footer is rendered")]
    [Then("the count label should read '0 items'")]
    public void UAC09_EmptyFileSetRendersZeroCount()
    {
        var emptyState = RepositoryUseCaseData.Primary() with
        {
            Files = [],
            TreeFiles = []
        };
        var cut = RenderRepositoryAppWithState(emptyState);

        var count = cut.Find("[data-testid='repo-footer-count']");
        count.TextContent.ShouldContain("0 items");
    }

    [Fact(DisplayName = "UAC10: Paginator renders inside the app footer slot")]
    [Trait(Spec.UAC, "10")]
    [Given("a RepositoryApp is rendered on a listing view")]
    [When("the footer is inspected")]
    [Then("the paginator should render inside the footer")]
    public void UAC10_PaginatorRendersInFooter()
    {
        var cut = RenderRepositoryApp();

        var footer = cut.Find("[data-slot='repository-app-footer']");
        footer.ClassList.ShouldNotContain("border-t");
        footer.QuerySelector("[data-slot='repository-app-paginator']").ShouldNotBeNull();
        footer.QuerySelector("[data-slot='repository-page-size-selector'] [data-slot='select-trigger']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC11: Page size and page selector render at opposite paginator edges")]
    [Trait(Spec.UAC, "11")]
    [Given("a paginator is rendered")]
    [When("the paginator layout is inspected")]
    [Then("page-size controls and page navigation should occupy separate zones")]
    public void UAC11_PageSizeAndPageSelectorRenderAtOppositeEdges()
    {
        var cut = RenderRepositoryApp();

        var paginator = cut.Find("[data-slot='repository-app-paginator']");
        paginator.ClassList.ShouldContain("justify-between");
        paginator.QuerySelector("[data-slot='repository-page-size-selector']")!.QuerySelector("[data-testid='repo-page-size-select']").ShouldNotBeNull();
        paginator.QuerySelector("[data-slot='repository-page-selector']")!.QuerySelector("[data-testid='repo-footer-page-1']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC12: Total item count label does not wrap")]
    [Trait(Spec.UAC, "12")]
    [Given("a paginator is rendered")]
    [When("the total item count label is inspected")]
    [Then("it should stay on a single line")]
    public void UAC12_TotalItemCountDoesNotWrap()
    {
        var cut = RenderRepositoryApp();

        var count = cut.Find("[data-testid='repo-footer-count']");
        count.ClassList.ShouldContain("whitespace-nowrap");
    }
}
