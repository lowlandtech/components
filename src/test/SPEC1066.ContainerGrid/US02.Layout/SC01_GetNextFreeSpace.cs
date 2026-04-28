using LowlandTech.Components.ContainerGrid;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1066.ContainerGrid.US02.Layout;

[Trait(Spec.SPEC, "1066")]
[Trait(Spec.US, "02")]
[Trait(Spec.SC, "01")]
[UserStory("02", "ContainerGrid layout helpers")]
public class SC01_GetNextFreeSpace : JsViewScenario
{
    [Fact(DisplayName = "UAC01: GetNextFreeSpace returns (1,1) on an empty grid")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_EmptyGridFirstCell()
    {
        var cut = Ctx.Render<LowlandTech.Components.ContainerGrid.ContainerGrid>(
            p => p.Add(x => x.Columns, 3).Add(x => x.Rows, 3));

        var result = cut.Instance.GetNextFreeSpace();
        result.ShouldNotBeNull();
        result.Value.Row.ShouldBe(1);
        result.Value.Column.ShouldBe(1);
    }

    [Fact(DisplayName = "UAC02: GetNextFreeSpace skips occupied cells")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_SkipsOccupiedCell()
    {
        // Fill (1,1) with a 1×1 section
        var cut = Ctx.Render<LowlandTech.Components.ContainerGrid.ContainerGrid>(
            p => p.Add(x => x.Columns, 2)
                  .Add(x => x.Rows, 2)
                  .AddChildContent<ContainerGridSection>(
                      s => s.Add(x => x.Id, "s1")
                            .Add(x => x.Column, 1)
                            .Add(x => x.Row, 1)
                            .Add(x => x.ColSpan, 1)
                            .Add(x => x.RowSpan, 1)));

        var result = cut.Instance.GetNextFreeSpace();
        result.ShouldNotBeNull();
        // First free cell should be (1,2)
        result.Value.Column.ShouldBe(2);
    }

    [Fact(DisplayName = "UAC03: GetNextFreeSpace returns null when grid is full")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_FullGridReturnsNull()
    {
        // 1×1 grid fully occupied
        var cut = Ctx.Render<LowlandTech.Components.ContainerGrid.ContainerGrid>(
            p => p.Add(x => x.Columns, 1)
                  .Add(x => x.Rows, 1)
                  .AddChildContent<ContainerGridSection>(
                      s => s.Add(x => x.Id, "s1")
                            .Add(x => x.Column, 1)
                            .Add(x => x.Row, 1)
                            .Add(x => x.ColSpan, 1)
                            .Add(x => x.RowSpan, 1)));

        var result = cut.Instance.GetNextFreeSpace();
        result.ShouldBeNull();
    }
}

