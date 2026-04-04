using Bunit;
using LowlandTech.Components.Sidebar;
using LowlandTech.Components.Tests.Infrastructure;
using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Tests.SPEC1043.Sidebar.US01.View;

[Trait(Spec.SPEC, "1043")]
[Trait(Spec.SC, "05")]
[UserStory("01", "Render Sidebar component")]
public class SC05_RenderSidebarProvider : ViewScenario
{
    [Fact(DisplayName = "UAC01: SidebarProvider renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<SidebarProvider>();
        cut.Find("[data-slot='sidebar-provider']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: SidebarProvider renders CSS variables for width")]
    [Trait(Spec.UAC, "02")]
    [Then("the rendered markup contains sidebar width CSS variables")]
    public void UAC02_RendersCssVariables()
    {
        var cut = Ctx.Render<SidebarProvider>(p => p
            .Add(x => x.Width, "20rem")
            .Add(x => x.MobileWidth, "22rem"));
        var style = cut.Find("[data-slot='sidebar-provider']").GetAttribute("style");
        style.ShouldContain("--sidebar-width: 20rem");
        style.ShouldContain("--sidebar-width-mobile: 22rem");
    }

    [Fact(DisplayName = "UAC03: SidebarProvider renders ChildContent")]
    [Trait(Spec.UAC, "03")]
    [Then("the rendered markup contains child content")]
    public void UAC03_RendersChildContent()
    {
        var cut = Ctx.Render<SidebarProvider>(p => p.AddChildContent("Inner content"));
        cut.Markup.ShouldContain("Inner content");
    }

    [Fact(DisplayName = "UAC04: SidebarProvider provides cascading SidebarState")]
    [Trait(Spec.UAC, "04")]
    [Then("child components receive cascaded SidebarState")]
    public void UAC04_ProvidesCascadingSidebarState()
    {
        SidebarState? receivedState = null;
        
        var cut = Ctx.Render<SidebarProvider>(p => p
            .AddChildContent<SidebarTestConsumer>(child => 
                child.Add(x => x.OnStateReceived, (state) => receivedState = state)));

        receivedState.ShouldNotBeNull();
    }

    private class SidebarTestConsumer : ComponentBase
    {
        [CascadingParameter] public SidebarState? SidebarState { get; set; }
        [Parameter] public Action<SidebarState?>? OnStateReceived { get; set; }

        protected override void OnInitialized()
        {
            OnStateReceived?.Invoke(SidebarState);
        }
    }
}
