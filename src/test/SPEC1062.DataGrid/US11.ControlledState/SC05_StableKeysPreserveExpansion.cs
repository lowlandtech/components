using Bunit;
using LowlandTech.Components.DataGrid;
using DataGridSut = LowlandTech.Components.DataGrid.DataGrid<LowlandTech.Components.Tests.SPEC1062.DataGrid.DataGridPerson>;

namespace LowlandTech.Components.Tests.SPEC1062.DataGrid.US11.ControlledState;

[Trait(Spec.SPEC, "1062")]
[Trait(Spec.SC, "05")]
[UserStory("11", "Controlled State and Stable Row Identity")]
public class SC05_StableKeysPreserveExpansion : ViewScenario
{
    [Fact(DisplayName = "UAC15: Expanded row remains expanded after sorting")]
    [Trait(Spec.UAC, "15")]
    public void UAC15_ExpansionPreservedAfterSort()
    {
        var cut = RenderGrid();

        cut.FindAll("[data-slot='data-grid-expand-button']")[1].Click();
        cut.FindAll("[data-slot='data-grid-sort-button']")[0].Click();

        var expandedRows = cut.FindAll("[data-expanded='true']");
        expandedRows.Count.ShouldBe(1);
        expandedRows[0].TextContent.ShouldContain("Ada");
    }

    [Fact(DisplayName = "UAC16: Expansion follows row key, not row index")]
    [Trait(Spec.UAC, "16")]
    public void UAC16_ExpansionFollowsRowKey()
    {
        var state = new TestState();
        var cut = Ctx.Render<DataGridSut>(parameters => parameters
            .Add(p => p.Items, DataGridPerson.Sample)
            .Add(p => p.ItemKey, (DataGridPerson p) => p.Id.ToString())
            .Add(p => p.DetailTemplate, (RenderFragment<DataGridPerson>)(person => builder => builder.AddContent(0, $"Detail: {person.Name}")))
            .Add(p => p.StateChanged, (DataGridStateRepresentation s) => state.LastState = s)
            .Add(p => p.ChildContent, BuildColumns()));

        cut.FindAll("[data-slot='data-grid-expand-button']")[2].Click();
        cut.FindAll("[data-slot='data-grid-sort-button']")[0].Click();

        var detailCells = cut.FindAll("[data-slot='data-grid-detail-cell']");
        detailCells.Count.ShouldBe(1);
        detailCells[0].TextContent.ShouldContain("Detail: Bea");
    }

    [Fact(DisplayName = "UAC17: StateChanged includes expanded row keys")]
    [Trait(Spec.UAC, "17")]
    public void UAC17_StateChangedIncludesExpandedKeys()
    {
        var state = new TestState();
        var cut = Ctx.Render<DataGridSut>(parameters => parameters
            .Add(p => p.Items, DataGridPerson.Sample)
            .Add(p => p.ItemKey, (DataGridPerson p) => p.Id.ToString())
            .Add(p => p.DetailTemplate, (RenderFragment<DataGridPerson>)(person => builder => builder.AddContent(0, $"Detail: {person.Name}")))
            .Add(p => p.StateChanged, (DataGridStateRepresentation s) => state.LastState = s)
            .Add(p => p.ChildContent, BuildColumns()));

        cut.FindAll("[data-slot='data-grid-expand-button']")[1].Click();

        state.LastState.ShouldNotBeNull();
        state.LastState.ExpandedRowKeys.ShouldContain("2");
    }

    private IRenderedComponent<DataGridSut> RenderGrid()
    {
        return Ctx.Render<DataGridSut>(parameters => parameters
            .Add(p => p.Items, DataGridPerson.Sample)
            .Add(p => p.ItemKey, (DataGridPerson p) => p.Id.ToString())
            .Add(p => p.DetailTemplate, (RenderFragment<DataGridPerson>)(person => builder => builder.AddContent(0, $"Detail: {person.Name}")))
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
        };
    }

    private sealed class TestState
    {
        public DataGridStateRepresentation? LastState { get; set; }
    }
}
