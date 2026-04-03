using Bunit;
using LowlandTech.Components.Table;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1049.Table.US01.View;

[Trait(Spec.SPEC, "1049")]
[Trait(Spec.SC, "02")]
[UserStory("01", "Render Table subcomponents")]
public class SC02_RenderTableSubcomponents : ViewScenario
{
    [Fact(DisplayName = "UAC01: TableHeader renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_HeaderRendersDataSlot()
    {
        var cut = Ctx.Render<TableHeader>();
        cut.Find("[data-slot='table-header']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: TableHeader renders thead element")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_HeaderRendersThead()
    {
        var cut = Ctx.Render<TableHeader>();
        cut.Find("thead").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC03: TableBody renders data-slot attribute")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_BodyRendersDataSlot()
    {
        var cut = Ctx.Render<TableBody>();
        cut.Find("[data-slot='table-body']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC04: TableBody renders tbody element")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_BodyRendersTbody()
    {
        var cut = Ctx.Render<TableBody>();
        cut.Find("tbody").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC05: TableFooter renders data-slot attribute")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_FooterRendersDataSlot()
    {
        var cut = Ctx.Render<TableFooter>();
        cut.Find("[data-slot='table-footer']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC06: TableFooter renders tfoot element")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_FooterRendersTfoot()
    {
        var cut = Ctx.Render<TableFooter>();
        cut.Find("tfoot").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC07: TableRow renders data-slot attribute")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_RowRendersDataSlot()
    {
        var cut = Ctx.Render<TableRow>();
        cut.Find("[data-slot='table-row']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC08: TableRow renders tr element")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_RowRendersTr()
    {
        var cut = Ctx.Render<TableRow>();
        cut.Find("tr").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC09: TableHead renders data-slot attribute")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_HeadRendersDataSlot()
    {
        var cut = Ctx.Render<TableHead>(p => p.AddChildContent("Header"));
        cut.Find("[data-slot='table-head']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC10: TableHead renders th element")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_HeadRendersTh()
    {
        var cut = Ctx.Render<TableHead>(p => p.AddChildContent("Header"));
        cut.Find("th").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC11: TableCell renders data-slot attribute")]
    [Trait(Spec.UAC, "11")]
    public void UAC11_CellRendersDataSlot()
    {
        var cut = Ctx.Render<TableCell>(p => p.AddChildContent("Cell"));
        cut.Find("[data-slot='table-cell']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC12: TableCell renders td element")]
    [Trait(Spec.UAC, "12")]
    public void UAC12_CellRendersTd()
    {
        var cut = Ctx.Render<TableCell>(p => p.AddChildContent("Cell"));
        cut.Find("td").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC13: TableCaption renders data-slot attribute")]
    [Trait(Spec.UAC, "13")]
    public void UAC13_CaptionRendersDataSlot()
    {
        var cut = Ctx.Render<TableCaption>(p => p.AddChildContent("Caption"));
        cut.Find("[data-slot='table-caption']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC14: TableCaption renders caption element")]
    [Trait(Spec.UAC, "14")]
    public void UAC14_CaptionRendersCaption()
    {
        var cut = Ctx.Render<TableCaption>(p => p.AddChildContent("Caption"));
        cut.Find("caption").ShouldNotBeNull();
    }
}
