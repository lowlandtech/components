namespace LowlandTech.Components.Tests.SPEC1062.DataGrid.US12.PagerComposition;

[Trait(Spec.SPEC, "1062")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "01")]
[UserStory("12", "DataGrid pager and controls are composed from LowlandTech components")]
public class SC01_PagerComposition : ViewScenario
{
    [Fact(DisplayName = "UAC01: Page-size selector renders a LowlandTech Select")]
    [Trait(Spec.UAC, "01")]
    [Given("a paging DataGrid with AlwaysShowPager")]
    [When("the pager is rendered")]
    [Then("the page-size control is a LowlandTech Select and no native select is present")]
    public void UAC01_PageSizeRendersSelect()
    {
        var cut = RenderGrid(allowPaging: true, alwaysShowPager: true);

        cut.Find("[data-slot='data-grid-pager'] [data-slot='data-grid-page-size'] [data-slot='select']").ShouldNotBeNull();
        cut.Find("[data-slot='data-grid-pager'] [data-slot='select-trigger']").ShouldNotBeNull();
        cut.FindAll("[data-slot='data-grid-pager'] select").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC02: Pager actions render LowlandTech Button components")]
    [Trait(Spec.UAC, "02")]
    [Given("a paging DataGrid with multiple pages")]
    [When("the pager is rendered")]
    [Then("previous and next actions are LowlandTech Button components")]
    public void UAC02_PagerActionsRenderButtons()
    {
        var cut = RenderGrid(allowPaging: true, pageSize: 2);

        var previous = cut.Find("[data-slot='data-grid-page-previous']");
        var next = cut.Find("[data-slot='data-grid-page-next']");

        previous.GetAttribute("data-slot").ShouldBe("data-grid-page-previous");
        next.GetAttribute("data-slot").ShouldBe("data-grid-page-next");
        previous.TagName.ShouldBe("BUTTON");
        next.TagName.ShouldBe("BUTTON");
        previous.GetAttribute("data-variant").ShouldNotBeNull();
        next.GetAttribute("data-variant").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC03: Filter editor renders a LowlandTech Input")]
    [Trait(Spec.UAC, "03")]
    [Given("a filtering DataGrid")]
    [When("the filter row is rendered")]
    [Then("the filter editor is a LowlandTech Input and no native input is present")]
    public void UAC03_FilterRendersInput()
    {
        var cut = RenderGrid(allowFiltering: true);

        var filter = cut.Find("[data-slot='data-grid-filter-input']");
        filter.GetAttribute("data-slot").ShouldBe("data-grid-filter-input");
        filter.TagName.ShouldBe("INPUT");
        filter.GetAttribute("data-variant").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC04: Empty state renders the LowlandTech Empty component")]
    [Trait(Spec.UAC, "04")]
    [Given("a DataGrid with no items")]
    [When("the empty state is rendered")]
    [Then("the empty placeholder uses the LowlandTech Empty component")]
    public void UAC04_EmptyRendersEmptyComponent()
    {
        var cut = RenderGrid(items: []);

        cut.Find("[data-slot='data-grid-empty'] [data-slot='empty']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC05: Loading state renders the LowlandTech Spinner component")]
    [Trait(Spec.UAC, "05")]
    [Given("a DataGrid in loading state")]
    [When("the loading state is rendered")]
    [Then("the loading placeholder uses the LowlandTech Spinner component")]
    public void UAC05_LoadingRendersSpinner()
    {
        var cut = RenderGrid(isLoading: true);

        cut.Find("[data-slot='data-grid-loading'] [data-slot='spinner']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC06: Sort button renders a LowlandTech Button")]
    [Trait(Spec.UAC, "06")]
    [Given("a sortable DataGrid")]
    [When("the header row is rendered")]
    [Then("the sort action is a LowlandTech Button")]
    public void UAC06_SortRendersButton()
    {
        var cut = RenderGrid();

        var sort = cut.Find("[data-slot='data-grid-sort-button']");
        sort.GetAttribute("data-slot").ShouldBe("data-grid-sort-button");
        sort.TagName.ShouldBe("BUTTON");
        sort.GetAttribute("data-variant").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC07: Changing page size resets page index and invokes both callbacks")]
    [Trait(Spec.UAC, "07")]
    [Given("a paging DataGrid on the second page")]
    [When("the user selects a different page size")]
    [Then("PageIndex resets to zero and both PageSizeChanged and PageIndexChanged fire exactly once")]
    public void UAC07_PageSizeChangeResetsPageIndex()
    {
        var state = new PageSizeState();
        var cut = Ctx.Render<PagerTestWrapper>(parameters => parameters
            .Add(p => p.Items, DataGridPerson.Sample)
            .Add(p => p.AllowPaging, true)
            .Add(p => p.PageSize, 2)
            .Add(p => p.PageIndex, 1)
            .Add(p => p.PageSizeChanged, (int size) => state.ReceivedPageSize = size)
            .Add(p => p.PageIndexChanged, (int idx) => state.ReceivedPageIndex = idx));

        cut.Find("[data-slot='data-grid-pager'] [data-slot='select-trigger']").Click();
        cut.WaitForState(() => cut.FindAll("[data-slot='select-content']").Count > 0);

        var option = cut.FindAll("[data-slot='select-item']")
            .First(item => item.GetAttribute("data-value") == "10");
        option.Click();

        cut.WaitForState(() => state.ReceivedPageSize.HasValue && state.ReceivedPageIndex.HasValue);

        state.ReceivedPageSize.ShouldBe(10);
        state.ReceivedPageIndex.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC08: Controlled page size and page index rehydrate after rerender")]
    [Trait(Spec.UAC, "08")]
    [Given("a paging DataGrid with controlled page size and index")]
    [When("the parent re-renders with new controlled values")]
    [Then("the grid reflects the new page size and index")]
    public void UAC08_ControlledValuesRehydrate()
    {
        var cut = Ctx.Render<PagerTestWrapper>(parameters => parameters
            .Add(p => p.Items, DataGridPerson.Sample)
            .Add(p => p.AllowPaging, true)
            .Add(p => p.PageSize, 2)
            .Add(p => p.PageIndex, 0));

        cut.FindAll("[data-slot='data-grid-row']").Count.ShouldBe(2);

        cut.Render(parameters => parameters
            .Add(p => p.PageSize, 10)
            .Add(p => p.PageIndex, 0));

        cut.FindAll("[data-slot='data-grid-row']").Count.ShouldBe(3);
    }

    private IRenderedComponent<DataGrid<DataGridPerson>> RenderGrid(
        bool allowPaging = false,
        bool alwaysShowPager = false,
        bool allowFiltering = false,
        bool isLoading = false,
        int pageSize = 10,
        IReadOnlyList<DataGridPerson>? items = null)
    {
        return Ctx.Render<DataGrid<DataGridPerson>>(parameters => parameters
            .Add(component => component.Items, items ?? DataGridPerson.Sample)
            .Add(component => component.AllowPaging, allowPaging)
            .Add(component => component.AlwaysShowPager, alwaysShowPager)
            .Add(component => component.AllowFiltering, allowFiltering)
            .Add(component => component.IsLoading, isLoading)
            .Add(component => component.PageSize, pageSize)
            .Add(component => component.ChildContent, BuildColumns()));
    }

    private static RenderFragment BuildColumns()
    {
        return builder =>
        {
            builder.OpenComponent<DataGridColumn<DataGridPerson>>(0);
            builder.AddAttribute(1, nameof(DataGridColumn<DataGridPerson>.Property), nameof(DataGridPerson.Name));
            builder.AddAttribute(2, nameof(DataGridColumn<DataGridPerson>.Title), "Name");
            builder.CloseComponent();

            builder.OpenComponent<DataGridColumn<DataGridPerson>>(3);
            builder.AddAttribute(4, nameof(DataGridColumn<DataGridPerson>.Property), nameof(DataGridPerson.Role));
            builder.AddAttribute(5, nameof(DataGridColumn<DataGridPerson>.Title), "Role");
            builder.CloseComponent();
        };
    }

    private sealed class PageSizeState
    {
        public int? ReceivedPageSize { get; set; }
        public int? ReceivedPageIndex { get; set; }
    }

    private sealed class PagerTestWrapper : ComponentBase
    {
        [Parameter] public IReadOnlyList<DataGridPerson> Items { get; set; } = DataGridPerson.Sample;
        [Parameter] public bool AllowPaging { get; set; }
        [Parameter] public bool AlwaysShowPager { get; set; }
        [Parameter] public bool AllowFiltering { get; set; }
        [Parameter] public int PageSize { get; set; } = 10;
        [Parameter] public int PageIndex { get; set; }
        [Parameter] public EventCallback<int> PageSizeChanged { get; set; }
        [Parameter] public EventCallback<int> PageIndexChanged { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            builder.OpenComponent<PopoverProvider>(1);
            builder.CloseComponent();
            builder.OpenComponent<DataGrid<DataGridPerson>>(2);
            builder.AddAttribute(3, nameof(DataGrid<DataGridPerson>.Items), Items);
            builder.AddAttribute(4, nameof(DataGrid<DataGridPerson>.AllowPaging), AllowPaging);
            builder.AddAttribute(5, nameof(DataGrid<DataGridPerson>.AlwaysShowPager), AlwaysShowPager);
            builder.AddAttribute(6, nameof(DataGrid<DataGridPerson>.AllowFiltering), AllowFiltering);
            builder.AddAttribute(7, nameof(DataGrid<DataGridPerson>.PageSize), PageSize);
            builder.AddAttribute(8, nameof(DataGrid<DataGridPerson>.PageIndex), PageIndex);
            builder.AddAttribute(9, nameof(DataGrid<DataGridPerson>.PageSizeChanged), PageSizeChanged);
            builder.AddAttribute(10, nameof(DataGrid<DataGridPerson>.PageIndexChanged), PageIndexChanged);
            builder.AddAttribute(11, nameof(DataGrid<DataGridPerson>.ChildContent), BuildColumns());
            builder.CloseComponent();
            builder.CloseElement();
        }

        private static RenderFragment BuildColumns()
        {
            return builder =>
            {
                builder.OpenComponent<DataGridColumn<DataGridPerson>>(0);
                builder.AddAttribute(1, nameof(DataGridColumn<DataGridPerson>.Property), nameof(DataGridPerson.Name));
                builder.AddAttribute(2, nameof(DataGridColumn<DataGridPerson>.Title), "Name");
                builder.CloseComponent();

                builder.OpenComponent<DataGridColumn<DataGridPerson>>(3);
                builder.AddAttribute(4, nameof(DataGridColumn<DataGridPerson>.Property), nameof(DataGridPerson.Role));
                builder.AddAttribute(5, nameof(DataGridColumn<DataGridPerson>.Title), "Role");
                builder.CloseComponent();
            };
        }
    }
}
