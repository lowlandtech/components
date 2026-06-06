using Bunit;
using LowlandTech.Components;
namespace LowlandTech.Components.Tests.SPEC1070.GraphEditor.US02.Component;

/// <summary>
/// SC01 — GraphEditor rendering contract.
/// Verifies that the SVG canvas renders the correct number of nodes and
/// edges, applies variant CSS classes, and reflects position in the
/// <c>transform</c> attribute. Drag interaction is covered in SC02.
/// </summary>
[Trait(Spec.SPEC, "1070")]
[Trait(Spec.US, "02")]
[Trait(Spec.SC, "01")]
[UserStory("02", "GraphEditor component rendering")]
public class SC01_GraphEditorRendering : JsViewScenario
{
    private static GraphDiagram TwoNodeDiagram()
    {
        var d = new GraphDiagram();
        var a = d.AddNode(new GraphNodeModel(40, 80) { Title = "A", Variant = "primary" });
        var b = d.AddNode(new GraphNodeModel(200, 80) { Title = "B" });
        d.AddEdge(a.AddPort(GraphPortAlignment.Right), b.AddPort(GraphPortAlignment.Left));
        return d;
    }

    private IRenderedComponent<LowlandTech.Components.GraphEditor> Render(
        GraphDiagram? diagram = null,
        int width = 400, int height = 200, bool showGrid = false) =>
        Ctx.Render<LowlandTech.Components.GraphEditor>(p => p
            .Add(c => c.Diagram, diagram ?? TwoNodeDiagram())
            .Add(c => c.Width, width)
            .Add(c => c.Height, height)
            .Add(c => c.ShowGrid, showGrid));

    // ── SVG root ──────────────────────────────────────────────────────────────

    [Fact(DisplayName = "UAC01: SVG root carries the cn-graph-canvas class")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_SvgHasCanvasClass()
    {
        var cut = Render();
        cut.Find("svg").ClassList.ShouldContain("cn-graph-canvas");
    }

    [Fact(DisplayName = "UAC02: SVG width and height match parameters")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_SvgDimensions()
    {
        var cut = Render(width: 640, height: 320);
        var svg = cut.Find("svg");
        svg.GetAttribute("width").ShouldBe("640");
        svg.GetAttribute("height").ShouldBe("320");
    }

    // ── Grid ──────────────────────────────────────────────────────────────────

    [Fact(DisplayName = "UAC03: ShowGrid=true renders a fill rect")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_ShowGridRendersRect()
    {
        var cut = Render(showGrid: true);
        // The grid background <rect> starts with "fill:url(#dot-" style / attribute.
        cut.FindAll("rect").ShouldNotBeEmpty();
    }

    [Fact(DisplayName = "UAC04: ShowGrid=false renders no background rect")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_HideGridRendersNoBackgroundRect()
    {
        var cut  = Render(showGrid: false);
        // No rects at all at top level (node rects are inside <g> elements).
        var topLevelRects = cut.FindAll("svg > rect");
        topLevelRects.ShouldBeEmpty();
    }

    // ── Nodes ─────────────────────────────────────────────────────────────────

    [Fact(DisplayName = "UAC05: One <g data-node> per node in the diagram")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_OneNodeGroupPerNode()
    {
        var cut = Render();
        cut.FindAll("[data-node]").Count.ShouldBe(2);
    }

    [Fact(DisplayName = "UAC06: Node group transform matches node position")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_NodeGroupTransformMatchesPosition()
    {
        var d    = new GraphDiagram();
        d.AddNode(new GraphNodeModel(40, 80));
        var cut  = Render(d);
        var g    = cut.Find("[data-node]");
        g.GetAttribute("transform").ShouldBe("translate(40, 80)");
    }

    [Fact(DisplayName = "UAC07: Primary variant node rect has cn-graph-node-rect-primary")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_PrimaryVariantClass()
    {
        var d    = new GraphDiagram();
        d.AddNode(new GraphNodeModel(0, 0) { Title = "X", Variant = "primary" });
        var cut  = Render(d);
        var rect = cut.Find("[data-node] rect");
        rect.ClassList.ShouldContain("cn-graph-node-rect-primary");
    }

    [Fact(DisplayName = "UAC08: Default variant node rect has only cn-graph-node-rect")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_DefaultVariantClassOnly()
    {
        var d   = new GraphDiagram();
        d.AddNode(new GraphNodeModel(0, 0) { Title = "X" });
        var cut = Render(d);
        var rect = cut.Find("[data-node] rect");
        rect.ClassList.ShouldContain("cn-graph-node-rect");
        rect.ClassList.ShouldNotContain("cn-graph-node-rect-default");
    }

    // ── Ports ─────────────────────────────────────────────────────────────────

    [Fact(DisplayName = "UAC09: Output port circle has cn-graph-port-output class")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_OutputPortClass()
    {
        var d    = new GraphDiagram();
        var node = d.AddNode(new GraphNodeModel(0, 0));
        node.AddPort(GraphPortAlignment.Right);
        var cut  = Render(d);
        cut.Find(".cn-graph-port-output").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC10: Input port circle has cn-graph-port-input class")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_InputPortClass()
    {
        var d    = new GraphDiagram();
        var node = d.AddNode(new GraphNodeModel(0, 0));
        node.AddPort(GraphPortAlignment.Left);
        var cut  = Render(d);
        cut.Find(".cn-graph-port-input").ShouldNotBeNull();
    }

    // ── Edges ─────────────────────────────────────────────────────────────────

    [Fact(DisplayName = "UAC11: One <g data-edge> per edge in the diagram")]
    [Trait(Spec.UAC, "11")]
    public void UAC11_OneEdgeGroupPerEdge()
    {
        var cut = Render();
        cut.FindAll("[data-edge]").Count.ShouldBe(1);
    }

    [Fact(DisplayName = "UAC12: Edge path carries cn-graph-edge-path class")]
    [Trait(Spec.UAC, "12")]
    public void UAC12_EdgePathClass()
    {
        var cut  = Render();
        var path = cut.Find("[data-edge] path");
        path.ClassList.ShouldContain("cn-graph-edge-path");
    }

    [Fact(DisplayName = "UAC13: Directed edge path has marker-end attribute")]
    [Trait(Spec.UAC, "13")]
    public void UAC13_DirectedEdgeHasMarkerEnd()
    {
        var cut  = Render();
        var path = cut.Find("[data-edge] path");
        path.GetAttribute("marker-end").ShouldNotBeNullOrEmpty();
    }

    [Fact(DisplayName = "UAC14: Dashed variant edge has cn-graph-edge-path-dashed class")]
    [Trait(Spec.UAC, "14")]
    public void UAC14_DashedVariantEdge()
    {
        var d    = new GraphDiagram();
        var a    = d.AddNode(new GraphNodeModel(0, 0));
        var b    = d.AddNode(new GraphNodeModel(200, 0));
        var edge = d.AddEdge(a.AddPort(GraphPortAlignment.Right), b.AddPort(GraphPortAlignment.Left));
        edge.Variant = "dashed";
        var cut  = Render(d);
        cut.Find("[data-edge] path").ClassList.ShouldContain("cn-graph-edge-path-dashed");
    }

    [Fact(DisplayName = "UAC15: Edge label element appears when Label is set")]
    [Trait(Spec.UAC, "15")]
    public void UAC15_EdgeLabelRenderedWhenSet()
    {
        var d    = new GraphDiagram();
        var a    = d.AddNode(new GraphNodeModel(0, 0));
        var b    = d.AddNode(new GraphNodeModel(200, 0));
        var edge = d.AddEdge(a.AddPort(GraphPortAlignment.Right), b.AddPort(GraphPortAlignment.Left));
        edge.Label = "my-label";
        var cut  = Render(d);
        cut.Find(".cn-graph-edge-label").TextContent.ShouldContain("my-label");
    }

    // ── Reactivity ───────────────────────────────────────────────────────────

    [Fact(DisplayName = "UAC16: Moving a node updates the transform attribute")]
    [Trait(Spec.UAC, "16")]
    public void UAC16_MoveNodeUpdatesTransform()
    {
        var d    = new GraphDiagram();
        var node = d.AddNode(new GraphNodeModel(10, 20));
        var cut  = Render(d);

        node.SetPosition(50, 99);

        var g = cut.Find("[data-node]");
        g.GetAttribute("transform").ShouldBe("translate(50, 99)");
    }

    [Fact(DisplayName = "UAC17: Adding a node to the diagram re-renders an extra node group")]
    [Trait(Spec.UAC, "17")]
    public void UAC17_AddNodeCausesRerender()
    {
        var d   = new GraphDiagram();
        d.AddNode(new GraphNodeModel(0, 0));
        var cut = Render(d);
        cut.FindAll("[data-node]").Count.ShouldBe(1);

        d.AddNode(new GraphNodeModel(200, 0));

        cut.FindAll("[data-node]").Count.ShouldBe(2);
    }

    [Fact(DisplayName = "UAC18: Drag-capture overlay absent before drag starts")]
    [Trait(Spec.UAC, "18")]
    public void UAC18_OverlayAbsentWhenIdle()
    {
        var cut = Render();
        // No fixed overlay should exist before any pointerdown.
        cut.FindAll("div.fixed").ShouldBeEmpty();
    }

    [Fact(DisplayName = "UAC19: Drag-capture overlay appears after pointerdown on a node")]
    [Trait(Spec.UAC, "19")]
    public void UAC19_OverlayAppearsOnPointerDown()
    {
        var d    = new GraphDiagram();
        d.AddNode(new GraphNodeModel(40, 80) { Title = "N" });
        var cut  = Render(d);

        cut.Find("[data-node]").TriggerEvent("onpointerdown",
            new PointerEventArgs { ClientX = 100, ClientY = 102 });

        cut.FindAll("div.fixed").Count.ShouldBe(1);
    }

    [Fact(DisplayName = "UAC20: Drag-capture overlay disappears on pointerup")]
    [Trait(Spec.UAC, "20")]
    public void UAC20_OverlayGoneAfterPointerUp()
    {
        var d    = new GraphDiagram();
        d.AddNode(new GraphNodeModel(40, 80) { Title = "N" });
        var cut  = Render(d);

        cut.Find("[data-node]").TriggerEvent("onpointerdown",
            new PointerEventArgs { ClientX = 100, ClientY = 102 });
        cut.Find("div.fixed").TriggerEvent("onpointerup",
            new PointerEventArgs { ClientX = 100, ClientY = 102 });

        cut.FindAll("div.fixed").ShouldBeEmpty();
    }

    [Fact(DisplayName = "UAC21: Node position updates when overlay receives pointermove")]
    [Trait(Spec.UAC, "21")]
    public void UAC21_NodeMovesOnOverlayPointerMove()
    {
        var d    = new GraphDiagram();
        var node = d.AddNode(new GraphNodeModel(40, 80) { Title = "N" });
        var cut  = Render(d);

        // Start drag at client (100, 100), then move to (150, 130).
        cut.Find("[data-node]").TriggerEvent("onpointerdown",
            new PointerEventArgs { ClientX = 100, ClientY = 100 });

        cut.Find("div.fixed").TriggerEvent("onpointermove",
            new PointerEventArgs { ClientX = 150, ClientY = 130 });

        // Node moved by delta (50, 30) from its initial position (40, 80).
        node.X.ShouldBe(90);
        node.Y.ShouldBe(110);
        cut.Find("[data-node]").GetAttribute("transform").ShouldBe("translate(90, 110)");
    }
}
