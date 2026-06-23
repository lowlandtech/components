using System.Text.Json;
using Bunit;
using Bunit.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using ResizableSut = LowlandTech.Components.Resizable;

namespace LowlandTech.Components.Tests.SPEC1038.Resizable.US03.Persistence;

[Trait(Spec.SPEC, "1038")]
[Trait(Spec.US, "03")]
[Trait(Spec.SC, "01")]
[UserStory("03", "Resizable panel sizes persist to localStorage and rehydrate on render")]
public class SC01_ResizablePersistence : ViewScenario
{
    [Fact(DisplayName = "UAC01: Resizable with PersistKey writes panel sizes to localStorage on resize end")]
    [Trait(Spec.UAC, "01")]
    [Given("a Resizable group with PersistKey and two panels")]
    [When("the handle is dragged and released")]
    [Then("localStorage.setItem is called with the persisted sizes keyed by panel id")]
    public async Task UAC01_ResizeEndPersistsSizes()
    {
        var cut = Ctx.Render<ResizableSut>(p => p
            .Add(x => x.PersistKey, "test-key")
            .AddChildContent(builder =>
            {
                builder.OpenComponent<ResizablePanel>(0);
                builder.AddAttribute(1, nameof(ResizablePanel.Id), "first");
                builder.AddAttribute(2, nameof(ResizablePanel.DefaultSize), 30);
                builder.CloseComponent();

                builder.OpenComponent<ResizableHandle>(3);
                builder.CloseComponent();

                builder.OpenComponent<ResizablePanel>(4);
                builder.AddAttribute(5, nameof(ResizablePanel.Id), "second");
                builder.AddAttribute(6, nameof(ResizablePanel.DefaultSize), 70);
                builder.CloseComponent();
            }));

        var handle = cut.FindComponent<ResizableHandle>().Instance;

        await cut.InvokeAsync(handle.OnPointerDownJs);
        await cut.InvokeAsync(() => handle.OnPointerMoveJs(200));
        await cut.InvokeAsync(handle.OnPointerUpJs);

        var setItemCalls = Ctx.JSInterop.Invocations
            .Where(i => i.Identifier == "localStorage.setItem")
            .ToList();

        setItemCalls.ShouldNotBeEmpty();
        var call = setItemCalls.Last();
        call.Arguments[0].ShouldBe("resizable:test-key");

        var json = call.Arguments[1]?.ToString();
        json.ShouldNotBeNullOrEmpty();
        var sizes = JsonSerializer.Deserialize<Dictionary<string, int>>(json!);
        sizes.ShouldNotBeNull();
        sizes.ShouldContainKey("first");
        sizes.ShouldContainKey("second");
        sizes!["first"].ShouldBe(50);
        sizes["second"].ShouldBe(50);
    }

    [Fact(DisplayName = "UAC02: Resizable with PersistKey restores panel sizes from localStorage on first render")]
    [Trait(Spec.UAC, "02")]
    [Given("localStorage contains saved panel sizes")]
    [When("the Resizable group renders")]
    [Then("the panels use the persisted sizes instead of their defaults")]
    public void UAC02_RenderRestoresPersistedSizes()
    {
        Ctx.JSInterop.Setup<string?>("localStorage.getItem", "resizable:test-key")
            .SetResult("{\"first\":60,\"second\":40}");

        var cut = Ctx.Render<ResizableSut>(p => p
            .Add(x => x.PersistKey, "test-key")
            .AddChildContent(builder =>
            {
                builder.OpenComponent<ResizablePanel>(0);
                builder.AddAttribute(1, nameof(ResizablePanel.Id), "first");
                builder.AddAttribute(2, nameof(ResizablePanel.DefaultSize), 30);
                builder.CloseComponent();

                builder.OpenComponent<ResizableHandle>(3);
                builder.CloseComponent();

                builder.OpenComponent<ResizablePanel>(4);
                builder.AddAttribute(5, nameof(ResizablePanel.Id), "second");
                builder.AddAttribute(6, nameof(ResizablePanel.DefaultSize), 70);
                builder.CloseComponent();
            }));

        var firstStyle = cut.Find("[data-panel='first']").GetAttribute("style");
        firstStyle.ShouldNotBeNull();
        firstStyle.ShouldContain("flex-basis: calc(60%");
        var secondStyle = cut.Find("[data-panel='second']").GetAttribute("style");
        secondStyle.ShouldNotBeNull();
        secondStyle.ShouldContain("flex-basis: calc(40%");
    }

    [Fact(DisplayName = "UAC03: Panels registered after first render rehydrate from persisted sizes")]
    [Trait(Spec.UAC, "03")]
    [Given("a Resizable group rendered with only one panel and persisted sizes for two panels")]
    [When("a second panel is added after initial render")]
    [Then("the newly added panel uses its persisted size")]
    public void UAC03_LateRegisteredPanelRestoresPersistedSize()
    {
        Ctx.JSInterop.Setup<string?>("localStorage.getItem", "resizable:test-key")
            .SetResult("{\"first\":60,\"second\":40}");

        var cut = Ctx.Render<TestWrapper>(p => p
            .Add(x => x.PersistKey, "test-key")
            .Add(x => x.ShowSecondPanel, false));

        var firstStyle = cut.Find("[data-panel='first']").GetAttribute("style");
        firstStyle.ShouldNotBeNull();
        firstStyle.ShouldContain("flex-basis: calc(60%");
        cut.FindAll("[data-panel='second']").Count.ShouldBe(0);

        cut.Render(p => p.Add(x => x.ShowSecondPanel, true));

        var secondStyle = cut.Find("[data-panel='second']").GetAttribute("style");
        secondStyle.ShouldNotBeNull();
        secondStyle.ShouldContain("flex-basis: calc(40%");
    }

    private class TestWrapper : ComponentBase
    {
        [Parameter] public string PersistKey { get; set; } = string.Empty;
        [Parameter] public bool ShowSecondPanel { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent<ResizableSut>(0);
            builder.AddAttribute(1, nameof(ResizableSut.PersistKey), PersistKey);
            builder.AddAttribute(2, "ChildContent", (RenderFragment)(childBuilder =>
            {
                childBuilder.OpenComponent<ResizablePanel>(3);
                childBuilder.AddAttribute(4, nameof(ResizablePanel.Id), "first");
                childBuilder.AddAttribute(5, nameof(ResizablePanel.DefaultSize), 30);
                childBuilder.CloseComponent();

                if (ShowSecondPanel)
                {
                    childBuilder.OpenComponent<ResizableHandle>(6);
                    childBuilder.CloseComponent();

                    childBuilder.OpenComponent<ResizablePanel>(7);
                    childBuilder.AddAttribute(8, nameof(ResizablePanel.Id), "second");
                    childBuilder.AddAttribute(9, nameof(ResizablePanel.DefaultSize), 70);
                    childBuilder.CloseComponent();
                }
            }));
            builder.CloseComponent();
        }
    }
}
