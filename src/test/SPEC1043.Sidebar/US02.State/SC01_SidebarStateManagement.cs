using LowlandTech.Components;
namespace LowlandTech.Components.Tests.SPEC1043.Sidebar.US02.State;

[Trait(Spec.SPEC, "1043")]
[Trait(Spec.SC, "05")]
[UserStory("02", "Sidebar state management")]
public class SC01_SidebarStateManagement
{
    [Fact(DisplayName = "UAC01: Initial state is expanded")]
    [Trait(Spec.UAC, "01")]
    [Then("state is expanded")]
    public void UAC01_InitialStateIsExpanded()
    {
        var state = new SidebarState();
        state.State.ShouldBe("expanded");
        state.Open.ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC02: Toggle switches to collapsed")]
    [Trait(Spec.UAC, "02")]
    [Then("state is collapsed after toggle")]
    public void UAC02_ToggleSwitchesToCollapsed()
    {
        var state = new SidebarState();
        state.Toggle();
        state.State.ShouldBe("collapsed");
        state.Open.ShouldBeFalse();
    }

    [Fact(DisplayName = "UAC03: Toggle on mobile switches OpenMobile")]
    [Trait(Spec.UAC, "03")]
    [Then("OpenMobile is toggled")]
    public void UAC03_ToggleOnMobileSwitchesOpenMobile()
    {
        var state = new SidebarState { IsMobile = true };
        state.OpenMobile.ShouldBeFalse();
        state.Toggle();
        state.OpenMobile.ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC04: SetOpen changes Open")]
    [Trait(Spec.UAC, "04")]
    [Then("Open reflects the set value")]
    public void UAC04_SetOpenChangesOpen()
    {
        var state = new SidebarState();
        state.SetOpen(false);
        state.Open.ShouldBeFalse();
    }

    [Fact(DisplayName = "UAC05: State property returns expanded or collapsed")]
    [Trait(Spec.UAC, "05")]
    [Then("state string matches open boolean")]
    public void UAC05_StatePropertyReturnsCorrectString()
    {
        var state = new SidebarState { Open = true };
        state.State.ShouldBe("expanded");
        state.Open = false;
        state.State.ShouldBe("collapsed");
    }

    [Fact(DisplayName = "UAC06: OnChange fires on Toggle")]
    [Trait(Spec.UAC, "06")]
    [Then("change event is raised")]
    public void UAC06_OnChangeFiresOnToggle()
    {
        var state = new SidebarState();
        var fired = false;
        state.OnChange += () => fired = true;
        state.Toggle();
        fired.ShouldBeTrue();
    }
}
