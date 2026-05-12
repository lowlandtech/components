using Bunit;
using CodeEditorSut = LowlandTech.Components.CodeEditor.CodeEditor;

namespace LowlandTech.Components.Tests.SPEC1011.CodeEditor.US01.View;

[Trait(Spec.SPEC, "1011")]
[Trait(Spec.SC, "02")]
[UserStory("01", "CodeEditor value binding")]
public class SC02_CodeEditorValue : ViewScenario
{
    [Fact(DisplayName = "UAC01: CodeEditor renders with initial value")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_RendersWithInitialValue()
    {
        var cut = Ctx.Render<CodeEditorSut>(p => p.Add(x => x.Value, "const x = 1;"));
        var textarea = cut.Find("textarea");
        textarea.GetAttribute("value").ShouldBe("const x = 1;");
    }

    [Fact(DisplayName = "UAC02: CodeEditor updates value on input")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_UpdatesValueOnInput()
    {
        var value = "initial";
        var cut = Ctx.Render<CodeEditorSut>(p =>
        {
            p.Add(x => x.Value, value);
            p.Add(x => x.ValueChanged, (string v) => value = v);
        });

        var textarea = cut.Find("textarea");
        textarea.Input("updated");

        value.ShouldBe("updated");
    }

    [Fact(DisplayName = "UAC03: CodeEditor highlights interpolated strings")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_HighlightsInterpolatedStrings()
    {
        var cut = Ctx.Render<CodeEditorSut>(p => p.Add(x => x.Value, "Hello ${name}"));
        var pre = cut.Find("pre");
        pre.InnerHtml.ShouldContain("<span");
    }

    [Fact(DisplayName = "UAC04: CodeEditor escapes HTML in value")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_EscapesHtmlInValue()
    {
        var cut = Ctx.Render<CodeEditorSut>(p => p.Add(x => x.Value, "<script>alert('xss')</script>"));
        var pre = cut.Find("pre");
        pre.InnerHtml.ShouldContain("&lt;script&gt;");
    }
}
