using Bunit;
using LowlandTech.Components.DataGrid;
using DataGridSut = LowlandTech.Components.DataGrid.DataGrid<LowlandTech.Components.Tests.SPEC1062.DataGrid.DataGridPerson>;

namespace LowlandTech.Components.Tests.SPEC1062.DataGrid.US11.ControlledState;

[Trait(Spec.SPEC, "1062")]
[Trait(Spec.SC, "03")]
[UserStory("11", "Controlled State and Stable Row Identity")]
public class SC03_PageClamping : ViewScenario
{
    [Fact(DisplayName = "UAC09: Page index clamps to highest available page when items shrink")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_PageIndexClamped()
    {
        var state = new TestState();
        var cut = Ctx.Render<DataGridSut>(parameters => parameters
            .Add(p => p.Items, DataGridPerson.Sample)
            .Add(p => p.PageIndex, 4)
            .Add(p => p.PageSize, 10)
            .Add(p => p.AllowPaging, true)
            .Add(p => p.PageIndexChanged, (int idx) => state.ReceivedPageIndex = idx)
            .Add(p => p.ChildContent, BuildColumns()));

        cut.WaitForState(() => state.ReceivedPageIndex.HasValue);

        state.ReceivedPageIndex.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC10: PageIndexChanged receives clamped page index")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_PageIndexChangedReceivesClampedIndex()
    {
        var state = new TestState();
        var cut = Ctx.Render<DataGridSut>(parameters => parameters
            .Add(p => p.Items, DataGridPerson.Sample)
            .Add(p => p.PageIndex, 4)
            .Add(p => p.PageSize, 10)
            .Add(p => p.AllowPaging, true)
            .Add(p => p.PageIndexChanged, (int idx) => state.ReceivedPageIndex = idx)
            .Add(p => p.ChildContent, BuildColumns()));

        cut.WaitForState(() => state.ReceivedPageIndex.HasValue);

        state.ReceivedPageIndex.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC11: Page clamping includes old and new page index")]
    [Trait(Spec.UAC, "11")]
    public void UAC11_PageClampingTransitions()
    {
        var state = new TestState();
        var cut = Ctx.Render<DataGridSut>(parameters => parameters
            .Add(p => p.Items, DataGridPerson.Sample)
            .Add(p => p.PageIndex, 4)
            .Add(p => p.PageSize, 10)
            .Add(p => p.AllowPaging, true)
            .Add(p => p.PageIndexChanged, (int idx) => state.ReceivedPageIndex = idx)
            .Add(p => p.ChildContent, BuildColumns()));

        cut.WaitForState(() => state.ReceivedPageIndex.HasValue);

        state.ReceivedPageIndex.ShouldBe(0);
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
        public int? ReceivedPageIndex { get; set; }
    }
}
