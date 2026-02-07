using Bunit;
using LowlandTech.Components.Components.Table;
using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US07.LayoutComponents;

/// <summary>
/// Tests for Table component functionality.
/// SC01 - Implement Table components
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "07")]
[Trait(Spec.SC, "01")]
public class SC01_TableRenders : Scenario
{
    private BunitContext _ctx = null!;
    private IRenderedComponent<Table>? _component;

    protected override Task GivenAsync()
    {
        _ctx = new BunitContext();
        return Task.CompletedTask;
    }

    protected override Task WhenAsync()
    {
        _component = _ctx.Render<Table>();
        return Task.CompletedTask;
    }

    public override async Task DisposeAsync()
    {
        _ctx?.Dispose();
        await base.DisposeAsync();
    }

    [Fact(DisplayName = "Then it should render a wrapper with class \"table-wrapper\"")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_ShouldRenderWrapper()
    {
        await ArrangeAndActAsync();
        _component.ShouldNotBeNull();
        
        // Debug: Output the actual markup
        System.Console.WriteLine("Table Markup: " + _component!.Markup);
        
        var wrapper = _component!.Find(".table-wrapper");
        wrapper.ShouldNotBeNull();
    }

    [Fact(DisplayName = "And it should render a <table> element with class \"table\"")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_ShouldRenderTableElement()
    {
        await ArrangeAndActAsync();
        
        var table = _component!.Find("table");
        table.ShouldNotBeNull();
        table.ClassList.ShouldContain("table");
    }

    [Fact(DisplayName = "And I should have TableHeader, TableBody, TableRow, TableHead, and TableCell components")]
    [Trait(Spec.UAC, "03")]
    public async Task UAC03_ShouldHaveSubcomponents()
    {
        // Simply verify that the components can be instantiated
        using var ctx = new BunitContext();
        
        // Test TableHeader
        var header = ctx.Render<TableHeader>();
        header.ShouldNotBeNull();
        header.Find("thead").ShouldNotBeNull();
        
        // Test TableBody
        var body = ctx.Render<TableBody>();
        body.ShouldNotBeNull();
        body.Find("tbody").ShouldNotBeNull();
        
        // Test TableRow
        var row = ctx.Render<TableRow>();
        row.ShouldNotBeNull();
        row.Find("tr").ShouldNotBeNull();
        
        // Test TableHead
        var head = ctx.Render<TableHead>();
        head.ShouldNotBeNull();
        head.Find("th").ShouldNotBeNull();
        
        // Test TableCell
        var cell = ctx.Render<TableCell>();
        cell.ShouldNotBeNull();
        cell.Find("td").ShouldNotBeNull();
    }

    [Fact(DisplayName = "And table subcomponents should render correct semantic elements (thead, tbody, tr, th, td)")]
    [Trait(Spec.UAC, "04")]
    public async Task UAC04_ShouldRenderSemanticElements()
    {
        using var ctx = new BunitContext();
        
        // Verify TableHeader renders thead
        var header = ctx.Render<TableHeader>();
        header.Find("thead").ShouldNotBeNull();
        
        // Verify TableBody renders tbody
        var body = ctx.Render<TableBody>();
        body.Find("tbody").ShouldNotBeNull();
        
        // Verify TableRow renders tr
        var row = ctx.Render<TableRow>();
        row.Find("tr").ShouldNotBeNull();
        
        // Verify TableHead renders th
        var head = ctx.Render<TableHead>();
        head.Find("th").ShouldNotBeNull();
        
        // Verify TableCell renders td
        var cell = ctx.Render<TableCell>();
        cell.Find("td").ShouldNotBeNull();
    }

    [Fact(DisplayName = "And it should merge the Class parameter with generated classes")]
    [Trait(Spec.UAC, "05")]
    public async Task UAC05_ShouldMergeClasses()
    {
        using var ctx = new BunitContext();
        var component = ctx.Render<Table>(parameters => parameters
            .Add(p => p.Class, "custom-table-class")
        );

        var table = component.Find("table");
        table.ClassList.ShouldContain("table");
        table.ClassList.ShouldContain("custom-table-class");
    }
}
