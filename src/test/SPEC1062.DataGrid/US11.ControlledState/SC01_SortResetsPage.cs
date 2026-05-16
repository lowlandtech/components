using Bunit;
using LowlandTech.Components.DataGrid;
using DataGridSut = LowlandTech.Components.DataGrid.DataGrid<LowlandTech.Components.Tests.SPEC1062.DataGrid.DataGridPerson>;

namespace LowlandTech.Components.Tests.SPEC1062.DataGrid.US11.ControlledState;

[Trait(Spec.SPEC, "1062")]
[Trait(Spec.SC, "01")]
[UserStory("11", "Controlled State and Stable Row Identity")]
public class SC01_SortResetsPage : ViewScenario
{
    [Fact(DisplayName = "UAC01: Sorting resets page index to 0")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_SortResetsPageIndex()
    {
        var (cut, state) = RenderGrid(pageIndex: 2);

        cut.Find("[data-slot='data-grid-sort-button']").Click();

        state.ReceivedPageIndex.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC02: PageIndexChanged receives 0 after sort reset")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_PageIndexChangedReceivesZero()
    {
        var (cut, state) = RenderGrid(pageIndex: 2);

        cut.Find("[data-slot='data-grid-sort-button']").Click();

        state.ReceivedPageIndex.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC03: SortDescriptors contain Name ascending after click")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_SortDescriptorsContainNameAscending()
    {
        var (cut, state) = RenderGrid(pageIndex: 2);

        cut.Find("[data-slot='data-grid-sort-button']").Click();

        state.ReceivedSortDescriptors.Count.ShouldBe(1);
        state.ReceivedSortDescriptors[0].ColumnKey.ShouldBe("Name");
        state.ReceivedSortDescriptors[0].Direction.ShouldBe(DataGridSortDirection.Ascending);
    }

    [Fact(DisplayName = "UAC04: SortChanged emits updated descriptor list")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_SortChangedEmitted()
    {
        var (cut, state) = RenderGrid(pageIndex: 2);

        cut.Find("[data-slot='data-grid-sort-button']").Click();

        state.SortChangedCalled.ShouldBeTrue();
    }

    private (IRenderedComponent<DataGridSut> Cut, TestState State) RenderGrid(int pageIndex = 0)
    {
        var testState = new TestState();
        var cut = Ctx.Render<DataGridSut>(parameters => parameters
            .Add(p => p.Items, DataGridPerson.Sample)
            .Add(p => p.PageIndex, pageIndex)
            .Add(p => p.PageIndexChanged, (int idx) => testState.ReceivedPageIndex = idx)
            .Add(p => p.SortChanged, (IReadOnlyList<DataGridSortDescriptor> descriptors) =>
            {
                testState.SortChangedCalled = true;
                testState.ReceivedSortDescriptors = descriptors.ToList();
            })
            .Add(p => p.AllowPaging, true)
            .Add(p => p.PageSize, 2)
            .Add(p => p.ChildContent, BuildColumns()));

        return (cut, testState);
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

    private sealed class TestState
    {
        public int? ReceivedPageIndex { get; set; }
        public bool SortChangedCalled { get; set; }
        public List<DataGridSortDescriptor> ReceivedSortDescriptors { get; set; } = [];
    }
}
