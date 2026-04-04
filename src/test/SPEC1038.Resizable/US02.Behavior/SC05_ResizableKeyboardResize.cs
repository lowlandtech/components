using Bunit;
using LowlandTech.Components.Resizable;
using LowlandTech.Components.Tests.Infrastructure;
using Microsoft.AspNetCore.Components.Web;

namespace LowlandTech.Components.Tests.SPEC1038.Resizable.US02.Behavior;

[Trait(Spec.SPEC, "1038")]
[Trait(Spec.SC, "05")]
[UserStory("02", "Resizable keyboard resizing")]
public class SC05_ResizableKeyboardResize : ViewScenario
{
    [Fact(DisplayName = "UAC01: ArrowRight increases the previous panel size in horizontal mode")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_ArrowRightResizesPanels()
    {
        var cut = Ctx.Render<Resizable>(p => p
            .AddChildContent<ResizablePanel>(panel => panel.Add(x => x.Id, "left"))
            .AddChildContent<ResizableHandle>()
            .AddChildContent<ResizablePanel>(panel => panel.Add(x => x.Id, "right")));

        var handle = cut.Find("[data-slot='resizable-handle']");
        handle.KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });

        var panels = cut.FindAll("[data-slot='resizable-panel']");
        panels[0].GetAttribute("style").ShouldContain("55%");
        panels[1].GetAttribute("style").ShouldContain("45%");
    }
}