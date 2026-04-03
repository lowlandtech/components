using Bunit;
using KbdSut = LowlandTech.Components.Kbd.Kbd;
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
}
