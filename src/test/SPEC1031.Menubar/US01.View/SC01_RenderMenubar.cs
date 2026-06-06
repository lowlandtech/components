using Bunit;
using MenubarComponent = LowlandTech.Components.Menubar;
using LowlandTech.Components;
namespace LowlandTech.Components.Tests.SPEC1031.Menubar.US01.View;

[Trait(Spec.SPEC, "1031")]
[Trait(Spec.US, "01")]
[UserStory("01", "Menubar component rendering")]
public class SC01_RenderMenubar : ViewScenario
{
    [Fact(DisplayName = "UAC01: Menubar renders with data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_MenubarRendersWithDataSlot()
    {
        var cut = Ctx.Render<MenubarComponent>();
        cut.Find("[data-slot='menubar']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Menubar renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_MenubarRendersChildContent()
    {
        var cut = Ctx.Render<MenubarComponent>(p => p.AddChildContent("<span class='test'>Content</span>"));
        cut.Find(".test").TextContent.ShouldBe("Content");
    }

    [Fact(DisplayName = "UAC03: MenubarMenu renders with data-slot attribute")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_MenubarMenuRendersWithDataSlot()
    {
        var cut = Ctx.Render<MenubarMenu>();
        cut.Find("[data-slot='menubar-menu']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC04: MenubarTrigger renders with data-slot attribute")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_MenubarTriggerRendersWithDataSlot()
    {
        var cut = Ctx.Render<MenubarTrigger>();
        var trigger = cut.Find("[data-slot='menubar-trigger']");
        trigger.ShouldNotBeNull();
        trigger.TagName.ShouldBe("BUTTON");
    }

    [Fact(DisplayName = "UAC05: MenubarTrigger renders ChildContent")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_MenubarTriggerRendersChildContent()
    {
        var cut = Ctx.Render<MenubarTrigger>(p => p.AddChildContent("Menu"));
        cut.Find("[data-slot='menubar-trigger']").TextContent.ShouldBe("Menu");
    }

    [Fact(DisplayName = "UAC06: MenubarContent renders with data-slot attribute")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_MenubarContentRendersWithDataSlot()
    {
        var cut = Ctx.Render<MenubarContent>();
        cut.Find("[data-slot='menubar-content']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC07: MenubarContent does not render when IsOpen is false")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_MenubarContentDoesNotRenderWhenIsOpenFalse()
    {
        var cut = Ctx.Render<MenubarContent>(p => p.Add(x => x.IsOpen, false));
        cut.FindAll("[data-slot='menubar-content']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC08: MenubarItem renders with data-slot attribute")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_MenubarItemRendersWithDataSlot()
    {
        var cut = Ctx.Render<MenubarItem>();
        cut.Find("[data-slot='menubar-item']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC09: MenubarItem renders ChildContent")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_MenubarItemRendersChildContent()
    {
        var cut = Ctx.Render<MenubarItem>(p => p.AddChildContent("Item"));
        cut.Find("[data-slot='menubar-item']").TextContent.ShouldBe("Item");
    }

    [Fact(DisplayName = "UAC10: MenubarSeparator renders with data-slot attribute")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_MenubarSeparatorRendersWithDataSlot()
    {
        var cut = Ctx.Render<MenubarSeparator>();
        cut.Find("[data-slot='menubar-separator']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC11: MenubarGroup renders with data-slot attribute")]
    [Trait(Spec.UAC, "11")]
    public void UAC11_MenubarGroupRendersWithDataSlot()
    {
        var cut = Ctx.Render<MenubarGroup>();
        cut.Find("[data-slot='menubar-group']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC12: MenubarLabel renders with data-slot attribute")]
    [Trait(Spec.UAC, "12")]
    public void UAC12_MenubarLabelRendersWithDataSlot()
    {
        var cut = Ctx.Render<MenubarLabel>();
        cut.Find("[data-slot='menubar-label']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC13: MenubarLabel renders ChildContent")]
    [Trait(Spec.UAC, "13")]
    public void UAC13_MenubarLabelRendersChildContent()
    {
        var cut = Ctx.Render<MenubarLabel>(p => p.AddChildContent("Label"));
        cut.Find("[data-slot='menubar-label']").TextContent.ShouldBe("Label");
    }

    [Fact(DisplayName = "UAC14: Full Menubar SUT renders correctly")]
    [Trait(Spec.UAC, "14")]
    public void UAC14_FullMenubarSutRenders()
    {
        var cut = Ctx.Render<MenubarSut>();
        cut.Find("[data-slot='menubar']").ShouldNotBeNull();
        cut.FindAll("[data-slot='menubar-menu']").Count.ShouldBe(2);
        cut.FindAll("[data-slot='menubar-trigger']").Count.ShouldBe(2);
        cut.FindAll("[data-slot='menubar-content']").Count.ShouldBe(2);
        cut.FindAll("[data-slot='menubar-item']").Count.ShouldBe(6);
        cut.FindAll("[data-slot='menubar-separator']").Count.ShouldBe(1);
        cut.FindAll("[data-slot='menubar-group']").Count.ShouldBe(1);
        cut.FindAll("[data-slot='menubar-label']").Count.ShouldBe(1);
    }
}
