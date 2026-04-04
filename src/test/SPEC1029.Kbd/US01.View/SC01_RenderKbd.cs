using Bunit;
using KbdSut = LowlandTech.Components.Kbd.Kbd;
using KbdGroupSut = LowlandTech.Components.Kbd.KbdGroup;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1029.Kbd.US01.View;

[Trait(Spec.SPEC, "1029")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render Kbd component")]
public class SC01_RenderKbd : ViewScenario
{
    [Fact(DisplayName = "UAC01: Kbd renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<KbdSut>();
        cut.Find("[data-slot='kbd']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: Kbd renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<KbdSut>(p => p.AddChildContent("Ctrl"));
        cut.Markup.ShouldContain("Ctrl");
    }

    [Fact(DisplayName = "UAC03: Kbd renders Class parameter")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_RendersClass()
    {
        var cut = Ctx.Render<KbdSut>(p => p.Add(x => x.Class, "test-class"));
        cut.Find("[data-slot='kbd']").ClassList.ShouldContain("test-class");
    }

    [Fact(DisplayName = "UAC04: Kbd renders kbd element")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_RendersKbdElement()
    {
        var cut = Ctx.Render<KbdSut>();
        cut.Find("kbd").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC05: Kbd renders keyboard shortcut text")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_RendersKeyboardShortcut()
    {
        var cut = Ctx.Render<KbdSut>(p => p.AddChildContent("⌘K"));
        cut.Markup.ShouldContain("⌘K");
    }

    [Fact(DisplayName = "UAC06: Kbd renders multiple keys")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_RendersMultipleKeys()
    {
        var cut = Ctx.Render<KbdSut>(p => p.AddChildContent("Ctrl + Shift + K"));
        cut.Markup.ShouldContain("Ctrl + Shift + K");
    }

    [Fact(DisplayName = "UAC07: Kbd renders data-size attribute")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_RendersDataSize()
    {
        var cut = Ctx.Render<KbdSut>(p => p.Add(x => x.Size, "md"));
        var kbd = cut.Find("[data-slot='kbd']");
        kbd.GetAttribute("data-size").ShouldBe("md");
    }

    [Fact(DisplayName = "UAC08: Kbd default size is md")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_DefaultSizeIsMd()
    {
        var cut = Ctx.Render<KbdSut>();
        var kbd = cut.Find("[data-slot='kbd']");
        kbd.GetAttribute("data-size").ShouldBe("md");
    }

    [Fact(DisplayName = "UAC09: KbdGroup renders data-slot attribute")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_KbdGroupRendersDataSlot()
    {
        var cut = Ctx.Render<KbdGroupSut>(p => p.AddChildContent("Group"));
        cut.Find("[data-slot='kbd-group']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC10: KbdGroup renders separator utility classes")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_KbdGroupRendersSeparatorUtilityClasses()
    {
        var cut = Ctx.Render<KbdGroupSut>(p =>
        {
            p.AddChildContent("<kbd>Ctrl</kbd><kbd>K</kbd>");
        });

        var group = cut.Find("[data-slot='kbd-group']");
        var classAttr = group.GetAttribute("class") ?? string.Empty;
        classAttr.ShouldContain("before:content-['+']");
    }
}
