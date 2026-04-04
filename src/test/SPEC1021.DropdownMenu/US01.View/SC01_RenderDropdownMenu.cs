using Bunit;
using DropdownMenuComponent = LowlandTech.Components.DropdownMenu.DropdownMenu;
using LowlandTech.Components.DropdownMenu;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1021.DropdownMenu.US01.View;

[Trait(Spec.SPEC, "1021")]
[Trait(Spec.US, "01")]
[UserStory("01", "DropdownMenu component rendering")]
public class SC01_RenderDropdownMenu : ViewScenario
{
    [Fact(DisplayName = "UAC01: DropdownMenu renders with data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_DropdownMenuRendersWithDataSlot()
    {
        var cut = Ctx.Render<DropdownMenuComponent>();
        cut.Find("[data-slot='dropdown-menu']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: DropdownMenu renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_DropdownMenuRendersChildContent()
    {
        var cut = Ctx.Render<DropdownMenuComponent>(p => p.AddChildContent("<span class='test'>Content</span>"));
        cut.Find(".test").TextContent.ShouldBe("Content");
    }

    [Fact(DisplayName = "UAC03: DropdownMenuTrigger renders with data-slot attribute")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_DropdownMenuTriggerRendersWithDataSlot()
    {
        var cut = Ctx.Render<DropdownMenuTrigger>();
        var trigger = cut.Find("[data-slot='dropdown-menu-trigger']");
        trigger.ShouldNotBeNull();
        trigger.TagName.ShouldBe("BUTTON");
    }

    [Fact(DisplayName = "UAC04: DropdownMenuTrigger renders ChildContent")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_DropdownMenuTriggerRendersChildContent()
    {
        var cut = Ctx.Render<DropdownMenuTrigger>(p => p.AddChildContent("Open Menu"));
        cut.Find("[data-slot='dropdown-menu-trigger']").TextContent.ShouldBe("Open Menu");
    }

    [Fact(DisplayName = "UAC05: DropdownMenuContent renders with data-slot attribute")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_DropdownMenuContentRendersWithDataSlot()
    {
        var cut = Ctx.Render<DropdownMenuContent>();
        cut.Find("[data-slot='dropdown-menu-content']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC06: DropdownMenuContent renders ChildContent")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_DropdownMenuContentRendersChildContent()
    {
        var cut = Ctx.Render<DropdownMenuContent>(p => p.AddChildContent("<span class='content'>Items</span>"));
        cut.Find(".content").TextContent.ShouldBe("Items");
    }

    [Fact(DisplayName = "UAC07: DropdownMenuItem renders with data-slot attribute")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_DropdownMenuItemRendersWithDataSlot()
    {
        var cut = Ctx.Render<DropdownMenuItem>();
        cut.Find("[data-slot='dropdown-menu-item']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC08: DropdownMenuItem renders ChildContent")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_DropdownMenuItemRendersChildContent()
    {
        var cut = Ctx.Render<DropdownMenuItem>(p => p.AddChildContent("Item Text"));
        cut.Find("[data-slot='dropdown-menu-item']").TextContent.ShouldBe("Item Text");
    }

    [Fact(DisplayName = "UAC09: DropdownMenuSeparator renders with data-slot attribute")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_DropdownMenuSeparatorRendersWithDataSlot()
    {
        var cut = Ctx.Render<DropdownMenuSeparator>();
        cut.Find("[data-slot='dropdown-menu-separator']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC10: DropdownMenuGroup renders with data-slot attribute")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_DropdownMenuGroupRendersWithDataSlot()
    {
        var cut = Ctx.Render<DropdownMenuGroup>();
        cut.Find("[data-slot='dropdown-menu-group']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC11: DropdownMenuLabel renders with data-slot attribute")]
    [Trait(Spec.UAC, "11")]
    public void UAC11_DropdownMenuLabelRendersWithDataSlot()
    {
        var cut = Ctx.Render<DropdownMenuLabel>();
        cut.Find("[data-slot='dropdown-menu-label']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC12: DropdownMenuLabel renders ChildContent")]
    [Trait(Spec.UAC, "12")]
    public void UAC12_DropdownMenuLabelRendersChildContent()
    {
        var cut = Ctx.Render<DropdownMenuLabel>(p => p.AddChildContent("Label Text"));
        cut.Find("[data-slot='dropdown-menu-label']").TextContent.ShouldBe("Label Text");
    }

    [Fact(DisplayName = "UAC13: Full DropdownMenu SUT renders correctly")]
    [Trait(Spec.UAC, "13")]
    public void UAC13_FullDropdownMenuSutRenders()
    {
        var cut = Ctx.Render<DropdownMenuSut>();
        cut.Find("[data-slot='dropdown-menu']").ShouldNotBeNull();
        cut.Find("[data-slot='dropdown-menu-trigger']").ShouldNotBeNull();
        cut.Find("[data-slot='dropdown-menu-content']").ShouldNotBeNull();
        cut.FindAll("[data-slot='dropdown-menu-item']").Count.ShouldBe(3);
        cut.Find("[data-slot='dropdown-menu-separator']").ShouldNotBeNull();
        cut.Find("[data-slot='dropdown-menu-group']").ShouldNotBeNull();
        cut.Find("[data-slot='dropdown-menu-label']").ShouldNotBeNull();
    }
}
