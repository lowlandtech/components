namespace LowlandTech.Components.Tests.SPEC1080.Work.US01.View;

[Trait(Spec.SPEC, "1080")]
[Trait(Spec.SC, "06")]
[UserStory("01", "Paginator controls page navigation")]
public class SC06_Pagination : Infrastructure.WorkScenario
{
    [Fact(DisplayName = "UAC01: Paginator renders on listing views")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_PaginatorRendersOnListingViews()
    {
        var cut = RenderWorkApp();
        StateService.SetView(WorkView.Enqueued);
        cut.Render();

        cut.Find("[data-slot='work-paginator']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Page 1 is active by default")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_PageOneActiveByDefault()
    {
        var cut = RenderWorkApp();
        StateService.SetView(WorkView.Enqueued);
        cut.Render();

        var pageOne = cut.Find("[data-testid='work-paginator-page-1']");
        pageOne.GetAttribute("data-active").ShouldBe("true");
    }

    [Fact(DisplayName = "UAC03: Clicking next page updates state")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_ClickingNextPageUpdatesState()
    {
        var cut = RenderWorkApp();
        StateService.SetView(WorkView.Enqueued);
        // Force small page size so we have 2+ pages
        StateService.SetPageSize(5);
        cut.Render();

        cut.Find("[data-testid='work-paginator-next']").Click();

        StateService.State.Ui.PageNumber.ShouldBe(2);
    }

    [Fact(DisplayName = "UAC04: Previous button is disabled on page 1")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_PreviousDisabledOnPageOne()
    {
        var cut = RenderWorkApp();
        StateService.SetView(WorkView.Enqueued);
        StateService.SetPage(1);
        cut.Render();

        var prev = cut.Find("[data-testid='work-paginator-prev']");
        prev.ClassList.ShouldContain("pointer-events-none");
        prev.ClassList.ShouldContain("opacity-50");
    }

    [Fact(DisplayName = "UAC05: Paginator renders in bottom footer")]
    [Trait(Spec.UAC, "05")]
    [Given("A listing view is rendered")]
    [When("The work app footer is inspected")]
    [Then("Paginator and page-size controls should render in the footer")]
    public void UAC05_PaginatorRendersInBottomFooter()
    {
        var cut = RenderWorkApp();
        StateService.SetView(WorkView.Enqueued);
        cut.Render();

        var footer = cut.Find("[data-slot='work-app-footer']");
        footer.ClassList.ShouldNotContain("border-t");
        footer.QuerySelector("[data-slot='work-paginator']").ShouldNotBeNull();
        footer.QuerySelector("[data-slot='work-page-size-selector'] [data-slot='select-trigger']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC06: Page size and page selector render at opposite footer edges")]
    [Trait(Spec.UAC, "06")]
    [Given("A listing view is rendered")]
    [When("The paginator layout is inspected")]
    [Then("Page-size controls and page navigation should occupy separate footer zones")]
    public void UAC06_PageSizeAndPageSelectorRenderAtOppositeFooterEdges()
    {
        var cut = RenderWorkApp();
        StateService.SetView(WorkView.Enqueued);
        cut.Render();

        var paginator = cut.Find("[data-slot='work-paginator']");
        paginator.ClassList.ShouldContain("justify-between");
        paginator.QuerySelector("[data-slot='work-page-size-selector']")!.QuerySelector("[data-testid='work-page-size-select']").ShouldNotBeNull();
        paginator.QuerySelector("[data-slot='work-page-selector']")!.QuerySelector("[data-testid='work-paginator-page-1']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC07: Page size is selected through Select")]
    [Trait(Spec.UAC, "07")]
    [Given("A listing view is rendered")]
    [When("A page-size option is selected")]
    [Then("The work app page size should update")]
    public void UAC07_PageSizeIsSelectedThroughSelect()
    {
        var selectedSize = 20;
        var cut = Ctx.Render(builder =>
        {
            builder.OpenComponent<PopoverProvider>(0);
            builder.CloseComponent();
            builder.OpenComponent<WorkPaginator>(1);
            builder.AddAttribute(2, nameof(WorkPaginator.Pager), new WorkPager(1, 20, 100, 5));
            builder.AddAttribute(3, nameof(WorkPaginator.PageSize), selectedSize);
            builder.AddAttribute(4, nameof(WorkPaginator.OnPageSizeChanged), EventCallback.Factory.Create<int>(this, size => selectedSize = size));
            builder.CloseComponent();
        });

        cut.Find("[data-testid='work-page-size-select']").Click();
        cut.Find("[data-testid='work-page-size-50']").Click();

        selectedSize.ShouldBe(50);
    }

    [Fact(DisplayName = "UAC08: Total item count does not wrap")]
    [Trait(Spec.UAC, "08")]
    [Given("A paginator has a total item count")]
    [When("The total item count label is rendered")]
    [Then("The label should remain on a single line")]
    public void UAC08_TotalItemCountDoesNotWrap()
    {
        var cut = Ctx.Render<WorkPaginator>(parameters => parameters
            .Add(x => x.Pager, new WorkPager(1, 20, 2, 1))
            .Add(x => x.PageSize, 20));

        var totalItems = cut.Find("[data-slot='work-total-items']");

        totalItems.TextContent.ShouldContain("Total items: 2");
        totalItems.ClassList.ShouldContain("whitespace-nowrap");
    }
}
