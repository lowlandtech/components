using Bunit;
using LowlandTech.Components;
using DataGridSut = LowlandTech.Components.DataGrid<LowlandTech.Components.Tests.SPEC1062.DataGrid.DataGridPerson>;

namespace LowlandTech.Components.Tests.SPEC1062.DataGrid.US11.ControlledState;

[Trait(Spec.SPEC, "1062")]
[Trait(Spec.SC, "02")]
[UserStory("11", "Controlled State and Stable Row Identity")]
public class SC02_FilterResetsPage : ViewScenario
{
    [Fact(DisplayName = "UAC05: Filtering resets page index to 0")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_FilterResetsPageIndex()
    {
        var (cut, state) = RenderGrid(pageIndex: 3);

        cut.FindAll("[data-slot='data-grid-filter-input']")[1].Input("Engineer");

        state.ReceivedPageIndex.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC06: PageIndexChanged receives 0 after filter reset")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_PageIndexChangedReceivesZero()
    {
        var (cut, state) = RenderGrid(pageIndex: 3);

        cut.FindAll("[data-slot='data-grid-filter-input']")[1].Input("Engineer");

        state.ReceivedPageIndex.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC07: FilterDescriptors contain the Role filter")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_FilterDescriptorsContainRoleFilter()
    {
        var (cut, state) = RenderGrid(pageIndex: 3);

        cut.FindAll("[data-slot='data-grid-filter-input']")[1].Input("Engineer");

        state.ReceivedFilterDescriptors.Count.ShouldBe(1);
        state.ReceivedFilterDescriptors[0].ColumnKey.ShouldBe("Role");
        state.ReceivedFilterDescriptors[0].Operator.ShouldBe("Contains");
        state.ReceivedFilterDescriptors[0].Value.ShouldBe("Engineer");
    }

    [Fact(DisplayName = "UAC08: FilterChanged emits the updated descriptor list")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_FilterChangedEmitted()
    {
        var (cut, state) = RenderGrid(pageIndex: 3);

        cut.FindAll("[data-slot='data-grid-filter-input']")[1].Input("Engineer");

        state.FilterChangedCalled.ShouldBeTrue();
    }

    private (IRenderedComponent<DataGridSut> Cut, TestState State) RenderGrid(int pageIndex = 0)
    {
        var testState = new TestState();
        var cut = Ctx.Render<DataGridSut>(parameters => parameters
            .Add(p => p.Items, DataGridPerson.Sample)
            .Add(p => p.PageIndex, pageIndex)
            .Add(p => p.PageIndexChanged, (int idx) => testState.ReceivedPageIndex = idx)
            .Add(p => p.FilterChanged, (IReadOnlyList<DataGridFilterDescriptor> descriptors) =>
            {
                testState.FilterChangedCalled = true;
                testState.ReceivedFilterDescriptors = descriptors.ToList();
            })
            .Add(p => p.AllowPaging, true)
            .Add(p => p.AllowFiltering, true)
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
        public bool FilterChangedCalled { get; set; }
        public List<DataGridFilterDescriptor> ReceivedFilterDescriptors { get; set; } = [];
    }
}
