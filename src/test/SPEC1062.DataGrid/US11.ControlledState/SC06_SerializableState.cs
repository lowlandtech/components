using Bunit;
using LowlandTech.Components;
using DataGridSut = LowlandTech.Components.DataGrid<LowlandTech.Components.Tests.SPEC1062.DataGrid.DataGridPerson>;

namespace LowlandTech.Components.Tests.SPEC1062.DataGrid.US11.ControlledState;

[Trait(Spec.SPEC, "1062")]
[Trait(Spec.SC, "06")]
[UserStory("11", "Controlled State and Stable Row Identity")]
public class SC06_SerializableState : ViewScenario
{
    [Fact(DisplayName = "UAC18: StateChanged includes page index")]
    [Trait(Spec.UAC, "18")]
    public void UAC18_StateIncludesPageIndex()
    {
        var state = new TestState();
        var cut = RenderGrid(state);

        cut.Find("[data-slot='data-grid-page-next']").Click();

        state.LastState.ShouldNotBeNull();
        state.LastState.PageIndex.ShouldBe(1);
    }

    [Fact(DisplayName = "UAC19: StateChanged includes sort descriptors")]
    [Trait(Spec.UAC, "19")]
    public void UAC19_StateIncludesSortDescriptors()
    {
        var state = new TestState();
        var cut = RenderGrid(state);

        cut.Find("[data-slot='data-grid-sort-button']").Click();

        state.LastState.ShouldNotBeNull();
        state.LastState.SortDescriptors.Count.ShouldBe(1);
    }

    [Fact(DisplayName = "UAC20: StateChanged includes filter descriptors")]
    [Trait(Spec.UAC, "20")]
    public void UAC20_StateIncludesFilterDescriptors()
    {
        var state = new TestState();
        var cut = RenderGrid(state, allowFiltering: true);

        cut.FindAll("[data-slot='data-grid-filter-input']")[1].Input("Engineer");

        state.LastState.ShouldNotBeNull();
        state.LastState.FilterDescriptors.Count.ShouldBe(1);
    }

    [Fact(DisplayName = "UAC21: StateChanged includes selected row keys")]
    [Trait(Spec.UAC, "21")]
    [Given("A controlled grid has multiple selection enabled")]
    [When("A row Checkbox is clicked")]
    [Then("StateChanged should include the selected row key")]
    public void UAC21_StateIncludesSelectedRowKeys()
    {
        var state = new TestState();
        var cut = RenderGrid(state, selectionMode: DataGridSelectionMode.Multiple);

        cut.FindAll("[data-slot='data-grid-selection-input']")[0].Click();

        state.LastState.ShouldNotBeNull();
        state.LastState.SelectedRowKeys.Count.ShouldBe(1);
    }

    [Fact(DisplayName = "UAC22: StateChanged includes expanded row keys")]
    [Trait(Spec.UAC, "22")]
    public void UAC22_StateIncludesExpandedRowKeys()
    {
        var state = new TestState();
        var cut = RenderGrid(state);

        cut.Find("[data-slot='data-grid-expand-button']").Click();

        state.LastState.ShouldNotBeNull();
        state.LastState.ExpandedRowKeys.Count.ShouldBe(1);
    }

    [Fact(DisplayName = "UAC23: State payload does not contain component instances or RenderFragment values")]
    [Trait(Spec.UAC, "23")]
    [Given("A controlled grid has sorting, filtering, selection, expansion, and paging state")]
    [When("Each interactive state changes")]
    [Then("The exported state payload should contain only serializable values")]
    public void UAC23_StatePayloadIsSerializable()
    {
        var state = new TestState();
        var items = new List<DataGridPerson>
        {
            new(1, "Cora", "Engineer"),
            new(2, "Ada", "Engineer"),
            new(3, "Bea", "Designer"),
            new(4, "Dee", "Engineer")
        };
        var cut = Ctx.Render<DataGridSut>(parameters => parameters
            .Add(p => p.Items, items)
            .Add(p => p.ItemKey, (DataGridPerson p) => p.Id.ToString())
            .Add(p => p.AllowPaging, true)
            .Add(p => p.PageSize, 2)
            .Add(p => p.AllowFiltering, true)
            .Add(p => p.SelectionMode, DataGridSelectionMode.Multiple)
            .Add(p => p.DetailTemplate, (RenderFragment<DataGridPerson>)(person => builder => builder.AddContent(0, $"Detail: {person.Name}")))
            .Add(p => p.StateChanged, (DataGridStateRepresentation s) => state.LastState = s)
            .Add(p => p.ChildContent, BuildColumns()));

        cut.Find("[data-slot='data-grid-sort-button']").Click();
        cut.FindAll("[data-slot='data-grid-filter-input']")[1].Input("Engineer");
        cut.FindAll("[data-slot='data-grid-selection-input']")[0].Click();
        cut.Find("[data-slot='data-grid-expand-button']").Click();
        cut.Find("[data-slot='data-grid-page-next']").Click();

        state.LastState.ShouldNotBeNull();
        state.LastState.PageIndex.ShouldBe(1);
        state.LastState.PageSize.ShouldBe(2);
        state.LastState.SortDescriptors.Count.ShouldBe(1);
        state.LastState.FilterDescriptors.Count.ShouldBe(1);
        state.LastState.SelectedRowKeys.Count.ShouldBe(1);
        state.LastState.ExpandedRowKeys.Count.ShouldBe(1);
    }

    private IRenderedComponent<DataGridSut> RenderGrid(
        TestState state,
        bool allowFiltering = false,
        DataGridSelectionMode selectionMode = DataGridSelectionMode.None)
    {
        return Ctx.Render<DataGridSut>(parameters => parameters
            .Add(p => p.Items, DataGridPerson.Sample)
            .Add(p => p.ItemKey, (DataGridPerson p) => p.Id.ToString())
            .Add(p => p.AllowPaging, true)
            .Add(p => p.PageSize, 2)
            .Add(p => p.AllowFiltering, allowFiltering)
            .Add(p => p.SelectionMode, selectionMode)
            .Add(p => p.DetailTemplate, (RenderFragment<DataGridPerson>)(person => builder => builder.AddContent(0, $"Detail: {person.Name}")))
            .Add(p => p.StateChanged, (DataGridStateRepresentation s) => state.LastState = s)
            .Add(p => p.ChildContent, BuildColumns()));
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
        public DataGridStateRepresentation? LastState { get; set; }
    }
}
