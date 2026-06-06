using Bunit;
using CodeEditorSut = LowlandTech.Components.CodeEditor;

namespace LowlandTech.Components.Tests.SPEC1011.CodeEditor.US01.View;

[Trait(Spec.SPEC, "1011")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Render CodeEditor component")]
public class SC01_RenderCodeEditor : ViewScenario
{
    [Fact(DisplayName = "UAC01: CodeEditor renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<CodeEditorSut>();
        cut.Find("[data-slot='code-editor']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: CodeEditor renders input with data-slot")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_RendersInputWithDataSlot()
    {
        var cut = Ctx.Render<CodeEditorSut>();
        cut.Find("[data-slot='code-editor-input']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC03: CodeEditor renders textarea element")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_RendersTextareaElement()
    {
        var cut = Ctx.Render<CodeEditorSut>();
        cut.Find("textarea").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC04: CodeEditor renders with default rows")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_RendersWithDefaultRows()
    {
        var cut = Ctx.Render<CodeEditorSut>();
        var textarea = cut.Find("textarea");
        textarea.GetAttribute("rows").ShouldBe("4");
    }

    [Fact(DisplayName = "UAC05: CodeEditor renders with custom rows")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_RendersWithCustomRows()
    {
        var cut = Ctx.Render<CodeEditorSut>(p => p.Add(x => x.Rows, 10));
        var textarea = cut.Find("textarea");
        textarea.GetAttribute("rows").ShouldBe("10");
    }

    [Fact(DisplayName = "UAC06: CodeEditor renders with placeholder")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_RendersWithPlaceholder()
    {
        var cut = Ctx.Render<CodeEditorSut>(p => p.Add(x => x.Placeholder, "Enter code..."));
        var textarea = cut.Find("textarea");
        textarea.GetAttribute("placeholder").ShouldBe("Enter code...");
    }

    [Fact(DisplayName = "UAC07: CodeEditor renders disabled state")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_RendersDisabledState()
    {
        var cut = Ctx.Render<CodeEditorSut>(p => p.Add(x => x.Disabled, true));
        var textarea = cut.Find("textarea");
        textarea.HasAttribute("disabled").ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC08: CodeEditor renders readonly state")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_RendersReadonlyState()
    {
        var cut = Ctx.Render<CodeEditorSut>(p => p.Add(x => x.ReadOnly, true));
        var textarea = cut.Find("textarea");
        textarea.HasAttribute("readonly").ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC09: CodeEditor renders with Class parameter")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_RendersWithClass()
    {
        var cut = Ctx.Render<CodeEditorSut>(p => p.Add(x => x.Class, "my-editor"));
        cut.Find("[data-slot='code-editor']").ClassList.ShouldContain("my-editor");
    }

    [Fact(DisplayName = "UAC10: CodeEditor has spellcheck disabled")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_HasSpellcheckDisabled()
    {
        var cut = Ctx.Render<CodeEditorSut>();
        var textarea = cut.Find("textarea");
        textarea.GetAttribute("spellcheck").ShouldBe("false");
    }
}
