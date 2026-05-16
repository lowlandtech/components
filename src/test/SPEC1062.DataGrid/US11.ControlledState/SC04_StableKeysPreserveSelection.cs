using Bunit;
using LowlandTech.Components.DataGrid;
using DataGridSut = LowlandTech.Components.DataGrid.DataGrid<LowlandTech.Components.Tests.SPEC1062.DataGrid.DataGridPerson>;

namespace LowlandTech.Components.Tests.SPEC1062.DataGrid.US11.ControlledState;

[Trait(Spec.SPEC, "1062")]
[Trait(Spec.SC, "04")]
[UserStory("11", "Controlled State and Stable Row Identity")]
public class SC04_StableKeysPreserveSelection : ViewScenario
{
    [Fact(DisplayName = "UAC12: Selection preserved after item instances refresh")]
    [Trait(Spec.UAC, "12")]
    public void UAC12_SelectionPreservedAfterRefresh()
    {
        var state = new TestState();
        var cut = Ctx.Render<DataGridSut>(parameters => parameters
            .Add(p => p.Items, DataGridPerson.Sample)
            .Add(p => p.ItemKey, (DataGridPerson p) => p.Id.ToString())
            .Add(p => p.SelectionMode, DataGridSelectionMode.Multiple)
            .Add(p => p.SelectedItemsChanged, (ISet<DataGridPerson> items) => state.SelectedItems = items.ToList())
            .Add(p => p.ChildContent, BuildColumns()));

        cut.FindAll("[data-slot='data-grid-selection-input']")[1].Change(true);

        cut.Instance.Items = new List<DataGridPerson>
        {
            new(1, "Cora", "Engineer"),
            new(2, "Ada", "Engineer"),
            new(3, "Bea", "Designer")
        };
        cut.Render();

        var selectedRows = cut.FindAll("[data-selected='true']");
        selectedRows.Count.ShouldBe(1);
        selectedRows[0].TextContent.ShouldContain("Ada");
    }

    [Fact(DisplayName = "UAC13: Selection does not depend on object reference equality")]
    [Trait(Spec.UAC, "13")]
    public void UAC13_SelectionNotByReference()
    {
        var state = new TestState();
        var originalItems = new List<DataGridPerson>
        {
            new(1, "Cora", "Engineer"),
            new(2, "Ada", "Engineer"),
            new(3, "Bea", "Designer")
        };

        var cut = Ctx.Render<DataGridSut>(parameters => parameters
            .Add(p => p.Items, originalItems)
            .Add(p => p.ItemKey, (DataGridPerson p) => p.Id.ToString())
            .Add(p => p.SelectionMode, DataGridSelectionMode.Multiple)
            .Add(p => p.SelectedItemsChanged, (ISet<DataGridPerson> items) => state.SelectedItems = items.ToList())
            .Add(p => p.ChildContent, BuildColumns()));

        cut.FindAll("[data-slot='data-grid-selection-input']")[1].Change(true);

        var newInstances = new List<DataGridPerson>
        {
            new(1, "Cora", "Engineer"),
            new(2, "Ada", "Engineer"),
            new(3, "Bea", "Designer")
        };

        cut.Instance.Items = newInstances;
        cut.Render();

        state.SelectedItems.Count.ShouldBe(1);
        state.SelectedItems[0].Id.ShouldBe(2);
    }

    [Fact(DisplayName = "UAC14: RowsReconciled includes retained selected row keys")]
    [Trait(Spec.UAC, "14")]
    public void UAC14_RowsReconciledRetainsKeys()
    {
        var state = new TestState();
        var cut = Ctx.Render<DataGridSut>(parameters => parameters
            .Add(p => p.Items, DataGridPerson.Sample)
            .Add(p => p.ItemKey, (DataGridPerson p) => p.Id.ToString())
            .Add(p => p.SelectionMode, DataGridSelectionMode.Multiple)
            .Add(p => p.StateChanged, (DataGridStateRepresentation s) => state.LastState = s)
            .Add(p => p.ChildContent, BuildColumns()));

        cut.FindAll("[data-slot='data-grid-selection-input']")[1].Change(true);

        state.LastState.ShouldNotBeNull();
        state.LastState.SelectedRowKeys.ShouldContain("2");
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
        public List<DataGridPerson> SelectedItems { get; set; } = [];
        public DataGridStateRepresentation? LastState { get; set; }
    }
}
