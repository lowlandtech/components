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
        pageOne.GetAttribute("data-variant").ShouldBe("default");
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
        prev.HasAttribute("disabled").ShouldBeTrue();
    }
}
