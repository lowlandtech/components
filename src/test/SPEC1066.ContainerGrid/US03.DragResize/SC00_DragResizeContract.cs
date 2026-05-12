using System.Text.Json;
using Bunit;
using LowlandTech.Components.ContainerGrid;

namespace LowlandTech.Components.Tests.SPEC1066.ContainerGrid.US03.DragResize;

/// <summary>
/// SC00 — Drift guard for the data-* contract that containerGrid.js (the
/// ported MudExGrid drag/resize binding) reads on every pointer event.
/// Renaming any of the attribute names asserted below silently breaks
/// drag/resize without surfacing a runtime error.
/// </summary>
[Trait(Spec.SPEC, "1066")]
[Trait(Spec.US, "03")]
[Trait(Spec.SC, "00")]
[UserStory("03", "ContainerGrid drag + resize data-* contract")]
public class SC00_DragResizeContract : JsViewScenario
{
    private IRenderedComponent<LowlandTech.Components.ContainerGrid.ContainerGrid> RenderGridWithSection(
        bool resizable = true)
    {
        return Ctx.Render<LowlandTech.Components.ContainerGrid.ContainerGrid>(p => p
            .Add(c => c.Columns, 12)
            .Add(c => c.Rows, 8)
            .Add(c => c.ResolveCollisions, true)
            .AddChildContent<ContainerGridSection>(s => s
                .Add(x => x.Id, "s1")
                .Add(x => x.Column, 3)
                .Add(x => x.Row, 2)
                .Add(x => x.ColSpan, 4)
                .Add(x => x.RowSpan, 2)
                .Add(x => x.Resizable, resizable)));
    }

    // ── Grid root data contract ─────────────────────────────────────────────

    [Fact(DisplayName = "UAC01: grid root carries data-rows matching Rows parameter")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_DataRows()
    {
        var cut = RenderGridWithSection();
        cut.Find("[data-slot='container-grid']").GetAttribute("data-rows").ShouldBe("8");
    }

    [Fact(DisplayName = "UAC02: grid root carries data-cols matching Columns parameter")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_DataCols()
    {
        var cut = RenderGridWithSection();
        cut.Find("[data-slot='container-grid']").GetAttribute("data-cols").ShouldBe("12");
    }

    [Fact(DisplayName = "UAC03: grid data-options is valid JSON")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_DataOptionsIsValidJson()
    {
        var cut = RenderGridWithSection();
        var json = cut.Find("[data-slot='container-grid']").GetAttribute("data-options");
        Should.NotThrow(() => JsonDocument.Parse(json!));
    }

    [Fact(DisplayName = "UAC04: grid data-options.resolveCollisions reflects the parameter")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_ResolveCollisionsPropagates()
    {
        var cut = RenderGridWithSection();
        var json = cut.Find("[data-slot='container-grid']").GetAttribute("data-options");
        var doc = JsonDocument.Parse(json!);
        doc.RootElement.GetProperty("resolveCollisions").GetBoolean().ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC05: grid data-options uses camelCase keys (floatMode, compactOnDrop, resolveCollisions)")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_CamelCaseKeys()
    {
        var cut = RenderGridWithSection();
        var json = cut.Find("[data-slot='container-grid']").GetAttribute("data-options");
        var doc = JsonDocument.Parse(json!);
        // The JS reads opts.floatMode / opts.compactOnDrop / opts.resolveCollisions.
        // PascalCase keys (FloatMode, ...) would silently miss every option.
        doc.RootElement.TryGetProperty("floatMode", out _).ShouldBeTrue();
        doc.RootElement.TryGetProperty("compactOnDrop", out _).ShouldBeTrue();
        doc.RootElement.TryGetProperty("resolveCollisions", out _).ShouldBeTrue();
    }

    // ── Section root data contract ──────────────────────────────────────────

    [Fact(DisplayName = "UAC06: section root carries data-id matching Id parameter")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_DataId()
    {
        var cut = RenderGridWithSection();
        cut.Find("[data-slot='container-grid-section']").GetAttribute("data-id").ShouldBe("s1");
    }

    [Fact(DisplayName = "UAC07: section root carries data-row matching Row parameter")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_DataRow()
    {
        var cut = RenderGridWithSection();
        cut.Find("[data-slot='container-grid-section']").GetAttribute("data-row").ShouldBe("2");
    }

    [Fact(DisplayName = "UAC08: section root carries data-col matching Column parameter")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_DataCol()
    {
        var cut = RenderGridWithSection();
        cut.Find("[data-slot='container-grid-section']").GetAttribute("data-col").ShouldBe("3");
    }

    [Fact(DisplayName = "UAC09: section root carries data-row-span matching RowSpan parameter")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_DataRowSpan()
    {
        var cut = RenderGridWithSection();
        cut.Find("[data-slot='container-grid-section']").GetAttribute("data-row-span").ShouldBe("2");
    }

    [Fact(DisplayName = "UAC10: section root carries data-col-span matching ColSpan parameter")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_DataColSpan()
    {
        var cut = RenderGridWithSection();
        cut.Find("[data-slot='container-grid-section']").GetAttribute("data-col-span").ShouldBe("4");
    }

    // ── Resize handles ──────────────────────────────────────────────────────

    [Fact(DisplayName = "UAC11: Resizable=true renders three handles (E, S, SE)")]
    [Trait(Spec.UAC, "11")]
    public void UAC11_ResizableRendersHandles()
    {
        var cut = RenderGridWithSection(resizable: true);
        cut.FindAll(".lt-container-grid-handle.lt-handle-e").Count.ShouldBe(1);
        cut.FindAll(".lt-container-grid-handle.lt-handle-s").Count.ShouldBe(1);
        cut.FindAll(".lt-container-grid-handle.lt-handle-se").Count.ShouldBe(1);
    }

    [Fact(DisplayName = "UAC12: Resizable=false renders zero handles")]
    [Trait(Spec.UAC, "12")]
    public void UAC12_NonResizableHasNoHandles()
    {
        var cut = RenderGridWithSection(resizable: false);
        cut.FindAll(".lt-container-grid-handle").Count.ShouldBe(0);
    }
}
