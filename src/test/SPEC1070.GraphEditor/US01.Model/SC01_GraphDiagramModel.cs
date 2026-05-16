namespace LowlandTech.Components.Tests.SPEC1070.GraphEditor.US01.Model;

/// <summary>
/// SC01 — GraphDiagram structural contract.
/// Tests the observable model layer in isolation — no Blazor, no bUnit.
/// </summary>
[Trait(Spec.SPEC, "1070")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "01")]
[UserStory("01", "GraphDiagram model — structural contract")]
public class SC01_GraphDiagramModel
{
    // ── GraphModel ────────────────────────────────────────────────────────────

    [Fact(DisplayName = "UAC01: GraphNodeModel gets a unique Id on construction")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_NodeGetsUniqueId()
    {
        var a = new GraphNodeModel();
        var b = new GraphNodeModel();
        a.Id.ShouldNotBeNullOrEmpty();
        a.Id.ShouldNotBe(b.Id);
    }

    [Fact(DisplayName = "UAC02: GraphNodeModel.SetPosition updates X and Y")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_SetPositionUpdatesCoords()
    {
        var node = new GraphNodeModel(10, 20);
        node.SetPosition(50, 75);
        node.X.ShouldBe(50);
        node.Y.ShouldBe(75);
    }

    // ── Port position sync ────────────────────────────────────────────────────

    [Fact(DisplayName = "UAC03: Right-aligned port sits at node right edge, middle height")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_RightPortPosition()
    {
        var node = new GraphNodeModel(100, 200) { Width = 120, Height = 44 };
        var port = node.AddPort(GraphPortAlignment.Right);
        port.X.ShouldBe(220);   // 100 + 120
        port.Y.ShouldBe(222);   // 200 + 44/2
    }

    [Fact(DisplayName = "UAC04: Left-aligned port sits at node left edge, middle height")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_LeftPortPosition()
    {
        var node = new GraphNodeModel(100, 200) { Width = 120, Height = 44 };
        var port = node.AddPort(GraphPortAlignment.Left);
        port.X.ShouldBe(100);
        port.Y.ShouldBe(222);
    }

    [Fact(DisplayName = "UAC05: Top/Bottom ports centre horizontally")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_TopBottomPortCentred()
    {
        var node = new GraphNodeModel(100, 200) { Width = 120, Height = 44 };
        var top    = node.AddPort(GraphPortAlignment.Top);
        var bottom = node.AddPort(GraphPortAlignment.Bottom);
        top.X.ShouldBe(160);     // 100 + 60
        top.Y.ShouldBe(200);
        bottom.X.ShouldBe(160);
        bottom.Y.ShouldBe(244);  // 200 + 44
    }

    [Fact(DisplayName = "UAC06: Port position follows node after SetPosition")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_PortFollowsNode()
    {
        var node = new GraphNodeModel(0, 0) { Width = 120, Height = 44 };
        var port = node.AddPort(GraphPortAlignment.Right);
        node.SetPosition(50, 100);
        port.X.ShouldBe(170);   // 50 + 120
        port.Y.ShouldBe(122);   // 100 + 22
    }

    // ── GraphDiagram ──────────────────────────────────────────────────────────

    [Fact(DisplayName = "UAC07: Added node appears in Diagram.Nodes")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_AddNodeAppearsInNodes()
    {
        var d    = new GraphDiagram();
        var node = d.AddNode(new GraphNodeModel());
        d.Nodes.ShouldContain(node);
    }

    [Fact(DisplayName = "UAC08: Added edge appears in Diagram.Edges with correct ports")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_AddEdgeAppearsInEdges()
    {
        var d    = new GraphDiagram();
        var a    = d.AddNode(new GraphNodeModel(0,  0));
        var b    = d.AddNode(new GraphNodeModel(200, 0));
        var src  = a.AddPort(GraphPortAlignment.Right);
        var tgt  = b.AddPort(GraphPortAlignment.Left);
        var edge = d.AddEdge(src, tgt);

        d.Edges.ShouldContain(edge);
        edge.Source.ShouldBe(src);
        edge.Target.ShouldBe(tgt);
    }

    [Fact(DisplayName = "UAC09: Port.Edges contains the edge after AddEdge")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_PortEdgesPopulatedAfterAdd()
    {
        var d   = new GraphDiagram();
        var a   = d.AddNode(new GraphNodeModel());
        var b   = d.AddNode(new GraphNodeModel());
        var src = a.AddPort(GraphPortAlignment.Right);
        var tgt = b.AddPort(GraphPortAlignment.Left);
        var e   = d.AddEdge(src, tgt);

        src.Edges.ShouldContain(e);
        tgt.Edges.ShouldContain(e);
    }

    [Fact(DisplayName = "UAC10: RemoveEdge removes it from Diagram.Edges and from both ports")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_RemoveEdgeCleansUp()
    {
        var d   = new GraphDiagram();
        var a   = d.AddNode(new GraphNodeModel());
        var b   = d.AddNode(new GraphNodeModel());
        var src = a.AddPort(GraphPortAlignment.Right);
        var tgt = b.AddPort(GraphPortAlignment.Left);
        var e   = d.AddEdge(src, tgt);
        d.RemoveEdge(e);

        d.Edges.ShouldBeEmpty();
        src.Edges.ShouldBeEmpty();
        tgt.Edges.ShouldBeEmpty();
    }

    [Fact(DisplayName = "UAC11: RemoveNode removes the node and all its edges")]
    [Trait(Spec.UAC, "11")]
    public void UAC11_RemoveNodeCleansUpEdges()
    {
        var d   = new GraphDiagram();
        var a   = d.AddNode(new GraphNodeModel());
        var b   = d.AddNode(new GraphNodeModel());
        d.AddEdge(a.AddPort(GraphPortAlignment.Right), b.AddPort(GraphPortAlignment.Left));

        d.RemoveNode(a);

        d.Nodes.ShouldNotContain(a);
        d.Edges.ShouldBeEmpty();
    }

    // ── Changed events ────────────────────────────────────────────────────────

    [Fact(DisplayName = "UAC12: Diagram.Changed fires when a node is added")]
    [Trait(Spec.UAC, "12")]
    public void UAC12_ChangedFiresOnAddNode()
    {
        var d = new GraphDiagram();
        var fired = false;
        d.Changed += () => fired = true;
        d.AddNode(new GraphNodeModel());
        fired.ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC13: Diagram.Changed fires when an edge is added")]
    [Trait(Spec.UAC, "13")]
    public void UAC13_ChangedFiresOnAddEdge()
    {
        var d   = new GraphDiagram();
        var a   = d.AddNode(new GraphNodeModel());
        var b   = d.AddNode(new GraphNodeModel());
        var fired = false;
        d.Changed += () => fired = true;
        d.AddEdge(a.AddPort(GraphPortAlignment.Right), b.AddPort(GraphPortAlignment.Left));
        fired.ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC14: Diagram.Changed fires when a node is moved")]
    [Trait(Spec.UAC, "14")]
    public void UAC14_ChangedFiresOnSetPosition()
    {
        var d    = new GraphDiagram();
        var node = d.AddNode(new GraphNodeModel(0, 0));
        var count = 0;
        d.Changed += () => count++;
        node.SetPosition(99, 99);
        count.ShouldBeGreaterThan(0);
    }

    [Fact(DisplayName = "UAC15: Diagram.Changed fires when a node is removed")]
    [Trait(Spec.UAC, "15")]
    public void UAC15_ChangedFiresOnRemoveNode()
    {
        var d    = new GraphDiagram();
        var node = d.AddNode(new GraphNodeModel());
        var fired = false;
        d.Changed += () => fired = true;
        d.RemoveNode(node);
        fired.ShouldBeTrue();
    }

    // ── Locked nodes ──────────────────────────────────────────────────────────

    [Fact(DisplayName = "UAC16: Locked flag does not affect position — diagram has no SetPosition guard")]
    [Trait(Spec.UAC, "16")]
    public void UAC16_LockedFlagIsExposedOnModel()
    {
        // Locked is a hint consumed by the component; the model itself does not
        // block SetPosition — that keeps the model layer trivially testable.
        var node = new GraphNodeModel() { Locked = true };
        node.Locked.ShouldBeTrue();
        node.SetPosition(5, 5); // must not throw
        node.X.ShouldBe(5);
    }

    // ── One shared port, multiple edges (branching topology) ─────────────────

    [Fact(DisplayName = "UAC17: A single port can be the source of multiple edges")]
    [Trait(Spec.UAC, "17")]
    public void UAC17_OnePortMultipleEdges()
    {
        var d      = new GraphDiagram();
        var src    = d.AddNode(new GraphNodeModel());
        var tgt1   = d.AddNode(new GraphNodeModel());
        var tgt2   = d.AddNode(new GraphNodeModel());
        var srcPort = src.AddPort(GraphPortAlignment.Right);

        var e1 = d.AddEdge(srcPort, tgt1.AddPort(GraphPortAlignment.Left));
        var e2 = d.AddEdge(srcPort, tgt2.AddPort(GraphPortAlignment.Left));

        srcPort.Edges.Count.ShouldBe(2);
        srcPort.Edges.ShouldContain(e1);
        srcPort.Edges.ShouldContain(e2);
    }
}
