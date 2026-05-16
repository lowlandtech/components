# Sidebar Component Specification

## Component Details

| Property | Value |
|----------|-------|
| **SPEC** | 1043 |
| **Namespace** | `LowlandTech.Components.Sidebar` |
| **Root Element** | `<aside>` |
| **data-slot** | `sidebar` |
| **Base Class** | `TinyComponentBase` (provides Class, ChildContent, AdditionalAttributes) |
| **shadcn Reference** | https://ui.shadcn.com/docs/components/radix/sidebar |

## Parameters (Sidebar root)

| Parameter | Type | Default | Source |
|-----------|------|---------|--------|
| `Side` | `string` | `"left"` | `Sidebar.shared.cs` |
| `Variant` | `string` | `"sidebar"` | `Sidebar.shared.cs` |
| `Collapsible` | `string` | `"offcanvas"` | `Sidebar.shared.cs` |
| `SidebarState` | `SidebarState?` | `null` | `CascadingParameter` (from `SidebarProvider`) |
| `Class` | `string?` | `null` | `TinyComponentBase` |
| `ChildContent` | `RenderFragment?` | `null` | `TinyComponentBase` |
| `AdditionalAttributes` | `Dictionary<string, object>?` | `null` | `TinyComponentBase` |

## State Management

### SidebarState

`SidebarState` is a plain C# class (not a component) that holds all sidebar state. It is created and owned by `SidebarProvider` and cascaded to all descendant components via `CascadingValue`.

| Property / Method | Type | Default | Description |
|-------------------|------|---------|-------------|
| `Open` | `bool` | `true` | Whether the desktop sidebar is open |
| `OpenMobile` | `bool` | `false` | Whether the mobile sidebar overlay is open |
| `IsMobile` | `bool` | `false` | Whether the current viewport is mobile |
| `Side` | `string` | `"left"` | Which side the sidebar appears on |
| `Variant` | `string` | `"sidebar"` | Visual variant of the sidebar |
| `Collapsible` | `string` | `"offcanvas"` | Collapse behaviour (`offcanvas`, `icon`, `none`) |
| `State` | `string` (computed) | — | Returns `"expanded"` when `Open` is `true`, `"collapsed"` when `false` |
| `Toggle()` | `void` | — | Toggles `OpenMobile` when `IsMobile`, otherwise toggles `Open`; fires `OnChange` |
| `SetOpen(bool)` | `void` | — | Sets `Open` to the given value; fires `OnChange` |
| `SetOpenMobile(bool)` | `void` | — | Sets `OpenMobile` to the given value; fires `OnChange` |
| `SetIsMobile(bool)` | `void` | — | Sets `IsMobile` to the given value; fires `OnChange` |
| `OnChange` | `event Action?` | — | Fires whenever state changes (via `Toggle`, `SetOpen`, `SetOpenMobile`, `SetIsMobile`) |

### SidebarProvider

`SidebarProvider` is the top-level wrapper that owns the `SidebarState` instance and cascades it to all children.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `DefaultOpen` | `bool` | `true` | Initial value for `SidebarState.Open` |
| `Open` | `bool?` | `null` | Controlled mode: two-way binding for `Open` |
| `OpenChanged` | `EventCallback<bool>` | — | Callback for two-way binding |
| `Width` | `string` | `"16rem"` | CSS variable `--sidebar-width` |
| `MobileWidth` | `string` | `"18rem"` | CSS variable `--sidebar-width-mobile` |

Rendered markup:
```html
<div data-slot="sidebar-provider"
     style="--sidebar-width: 16rem; --sidebar-width-mobile: 18rem;"
     class="@Class">
    <CascadingValue Value="_state">
        @ChildContent
    </CascadingValue>
</div>
```

The provider subscribes to `SidebarState.OnChange` and calls `StateHasChanged()` to re-render when state changes. It implements `IDisposable` to unsubscribe.

## Sub-Components

| SC# | Component | Element | data-slot | Default CSS Class | Extra Parameters | Inherits TinyComponentBase |
|-----|-----------|---------|-----------|-------------------|------------------|--------------------------|
| 01 | `Sidebar` | `<aside>` | `sidebar` | `app-sidebar` | `Side`, `Variant`, `Collapsible`, `SidebarState` (cascading) | Yes |
| 02 | `SidebarHeader` | `<div>` | `sidebar-header` | `sidebar-header` | — | Yes |
| 03 | `SidebarContent` | `<div>` | `sidebar-content` | `app-sidebar-nav` | — | Yes |
| 04 | `SidebarFooter` | `<div>` | `sidebar-footer` | `sidebar-footer` | — | Yes |
| 05 | `SidebarProvider` | `<div>` | `sidebar-provider` | — | `DefaultOpen`, `Open`, `OpenChanged`, `Width`, `MobileWidth` | Yes |
| 06 | `SidebarInset` | `<main>` | `sidebar-inset` | `sidebar-inset` | — | Yes |
| 07 | `SidebarRail` | `<button>` | `sidebar-rail` | `sidebar-rail` | `SidebarState` (cascading), `aria-label="Toggle Sidebar"` | Yes |
| 08 | `SidebarTrigger` | `<button>` | `sidebar-trigger` | `sidebar-trigger` | `SidebarState` (cascading) | Yes |
| 09 | `SidebarSeparator` | `<hr>` | `sidebar-separator` | `sidebar-separator` | — | No (standalone, owns `Class` + `AdditionalAttributes`) |
| 10 | `SidebarInput` | `<input>` | `sidebar-input` | `sidebar-input` | — | No (standalone, owns `Class` + `AdditionalAttributes`) |
| 11 | `SidebarGroup` | `<div>` | `sidebar-group` | `sidebar-group` | — | Yes |
| 12 | `SidebarGroupLabel` | `<div>` | `sidebar-group-label` | `sidebar-group-label` | — | Yes |
| 13 | `SidebarGroupContent` | `<div>` | `sidebar-group-content` | `sidebar-group-content` | — | Yes |
| 14 | `SidebarGroupAction` | `<button>` | `sidebar-group-action` | `sidebar-group-action` | — | Yes |
| 15 | `SidebarMenu` | `<ul>` | `sidebar-menu` | `sidebar-menu` | — | Yes |
| 16 | `SidebarMenuItem` | `<li>` | `sidebar-menu-item` | `sidebar-menu-item` | — | Yes |
| 17 | `SidebarMenuButton` | `<button>` | `sidebar-menu-button` | `sidebar-menu-button` | `IsActive` (bool) -> `data-active` | Yes |
| 18 | `SidebarMenuAction` | `<button>` | `sidebar-menu-action` | `sidebar-menu-action` | — | Yes |
| 19 | `SidebarMenuBadge` | `<div>` | `sidebar-menu-badge` | `sidebar-menu-badge` | — | Yes |
| 20 | `SidebarMenuSkeleton` | `<div>` | `sidebar-menu-skeleton` | `sidebar-menu-skeleton` | Renders skeleton children (icon + text) | Yes |
| 21 | `SidebarMenuSub` | `<ul>` | `sidebar-menu-sub` | `sidebar-menu-sub` | — | Yes |
| 22 | `SidebarMenuSubItem` | `<li>` | `sidebar-menu-sub-item` | `sidebar-menu-sub-item` | — | Yes |
| 23 | `SidebarMenuSubButton` | `<a>` | `sidebar-menu-sub-button` | `sidebar-menu-sub-button` | `IsActive` (bool) -> `data-active` | Yes |

## Nesting Hierarchy

```
SidebarProvider                  <- <div data-slot="sidebar-provider" style="--sidebar-width; --sidebar-width-mobile">
|                                     CascadingValue: SidebarState
|
+-- Sidebar                      <- <aside data-slot="sidebar" data-state data-side data-variant data-collapsible class="app-sidebar">
|   +-- SidebarHeader            <- <div data-slot="sidebar-header" class="sidebar-header">
|   +-- SidebarContent           <- <div data-slot="sidebar-content" class="app-sidebar-nav">
|   |   +-- SidebarGroup         <- <div data-slot="sidebar-group" class="sidebar-group">
|   |   |   +-- SidebarGroupLabel    <- <div data-slot="sidebar-group-label" class="sidebar-group-label">
|   |   |   +-- SidebarGroupAction   <- <button data-slot="sidebar-group-action" class="sidebar-group-action">
|   |   |   +-- SidebarGroupContent  <- <div data-slot="sidebar-group-content" class="sidebar-group-content">
|   |   |       +-- SidebarMenu      <- <ul data-slot="sidebar-menu" class="sidebar-menu">
|   |   |           +-- SidebarMenuItem      <- <li data-slot="sidebar-menu-item" class="sidebar-menu-item">
|   |   |           |   +-- SidebarMenuButton    <- <button data-slot="sidebar-menu-button" data-active class="sidebar-menu-button">
|   |   |           |   +-- SidebarMenuAction    <- <button data-slot="sidebar-menu-action" class="sidebar-menu-action">
|   |   |           |   +-- SidebarMenuBadge     <- <div data-slot="sidebar-menu-badge" class="sidebar-menu-badge">
|   |   |           |   +-- SidebarMenuSkeleton  <- <div data-slot="sidebar-menu-skeleton" class="sidebar-menu-skeleton">
|   |   |           |   +-- SidebarMenuSub       <- <ul data-slot="sidebar-menu-sub" class="sidebar-menu-sub">
|   |   |           |       +-- SidebarMenuSubItem       <- <li data-slot="sidebar-menu-sub-item" class="sidebar-menu-sub-item">
|   |   |           |           +-- SidebarMenuSubButton  <- <a data-slot="sidebar-menu-sub-button" data-active class="sidebar-menu-sub-button">
|   |   +-- SidebarSeparator    <- <hr data-slot="sidebar-separator" class="sidebar-separator">
|   |   +-- SidebarInput        <- <input data-slot="sidebar-input" class="sidebar-input">
|   +-- SidebarFooter            <- <div data-slot="sidebar-footer" class="sidebar-footer">
|   +-- SidebarRail              <- <button data-slot="sidebar-rail" aria-label="Toggle Sidebar" class="sidebar-rail">
|
+-- SidebarTrigger               <- <button data-slot="sidebar-trigger" class="sidebar-trigger">  (outside sidebar, inside provider)
+-- SidebarInset                  <- <main data-slot="sidebar-inset" class="sidebar-inset">  (main content area, sibling of sidebar)
```

## Test File Structure

```
SPEC1043.Sidebar/
+-- Sidebar.feature                              <- Gherkin spec (this companion)
+-- GUIDE.md                                     <- This file
+-- US01.View/
|   +-- SC01_RenderSidebar.cs                    <- Sidebar root tests (UAC01-08)
|   +-- SC02_RenderSidebarHeader.cs              <- SidebarHeader tests (UAC01-03)
|   +-- SC03_RenderSidebarContent.cs             <- SidebarContent tests (UAC01-03)
|   +-- SC04_RenderSidebarFooter.cs              <- SidebarFooter tests (UAC01-03)
|   +-- SC05_RenderSidebarProvider.cs            <- SidebarProvider tests (UAC01-04)
|   +-- SC06_RenderSidebarInset.cs               <- SidebarInset tests (UAC01-04)
|   +-- SC07_RenderSidebarRail.cs                <- SidebarRail tests (UAC01-04)
|   +-- SC08_RenderSidebarTrigger.cs             <- SidebarTrigger tests (UAC01-04)
|   +-- SC09_RenderSidebarSeparator.cs           <- SidebarSeparator tests (UAC01-03)
|   +-- SC10_RenderSidebarInput.cs               <- SidebarInput tests (UAC01-03)
|   +-- SC11_RenderSidebarGroup.cs               <- SidebarGroup tests (UAC01-03)
|   +-- SC12_RenderSidebarGroupLabel.cs          <- SidebarGroupLabel tests (UAC01-03)
|   +-- SC13_RenderSidebarGroupContent.cs        <- SidebarGroupContent tests (UAC01-03)
|   +-- SC14_RenderSidebarGroupAction.cs         <- SidebarGroupAction tests (UAC01-04)
|   +-- SC15_RenderSidebarMenu.cs                <- SidebarMenu tests (UAC01-04)
|   +-- SC16_RenderSidebarMenuItem.cs            <- SidebarMenuItem tests (UAC01-04)
|   +-- SC17_RenderSidebarMenuButton.cs          <- SidebarMenuButton tests (UAC01-06)
|   +-- SC18_RenderSidebarMenuAction.cs          <- SidebarMenuAction tests (UAC01-04)
|   +-- SC19_RenderSidebarMenuBadge.cs           <- SidebarMenuBadge tests (UAC01-03)
|   +-- SC20_RenderSidebarMenuSkeleton.cs        <- SidebarMenuSkeleton tests (UAC01-03)
|   +-- SC21_RenderSidebarMenuSub.cs             <- SidebarMenuSub tests (UAC01-04)
|   +-- SC22_RenderSidebarMenuSubItem.cs         <- SidebarMenuSubItem tests (UAC01-04)
|   +-- SC23_RenderSidebarMenuSubButton.cs       <- SidebarMenuSubButton tests (UAC01-06)
+-- US02.State/
    +-- SC05_SidebarState.cs                     <- SidebarState unit tests (UAC01-06)
```

## Test Patterns

### Basic render test (data-slot + CSS class):
```csharp
using Bunit;
using LowlandTech.Components.Sidebar;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1043")]
[Trait(Spec.SC, "02")]
[UserStory("01", "Render Sidebar components")]
public class SC02_RenderSidebarHeader : ViewScenario
{
    [Fact(DisplayName = "UAC01: SidebarHeader renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    [Then("the rendered markup contains data-slot")]
    public void UAC01_RendersDataSlot()
    {
        var cut = Ctx.Render<SidebarHeader>();
        cut.Find("[data-slot='sidebar-header']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: SidebarHeader renders ChildContent")]
    [Trait(Spec.UAC, "02")]
    [Then("the markup contains the child content")]
    public void UAC02_RendersChildContent()
    {
        var cut = Ctx.Render<SidebarHeader>(p => p.AddChildContent("Header"));
        cut.Markup.ShouldContain("Header");
    }

    [Fact(DisplayName = "UAC03: SidebarHeader renders default CSS class")]
    [Trait(Spec.UAC, "03")]
    [Then("the root element has class sidebar-header")]
    public void UAC03_RendersDefaultCss()
    {
        var cut = Ctx.Render<SidebarHeader>();
        cut.Find("[data-slot='sidebar-header']").ClassList.ShouldContain("sidebar-header");
    }
}
```

### State test (pure unit test, no ViewScenario needed):
```csharp
using LowlandTech.Components.Sidebar;
using LowlandTech.Components.Tests.Infrastructure;

[Trait(Spec.SPEC, "1043")]
[Trait(Spec.SC, "05")]
[UserStory("02", "Sidebar state management")]
public class SC05_SidebarState
{
    [Fact(DisplayName = "UAC01: SidebarState initial Open is true")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_InitialOpenIsTrue()
    {
        var state = new SidebarState();
        state.Open.ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC03: SidebarState Toggle switches Open to false")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_ToggleSwitchesOpen()
    {
        var state = new SidebarState();
        state.Toggle();
        state.Open.ShouldBeFalse();
    }

    [Fact(DisplayName = "UAC06: SidebarState OnChange fires on Toggle")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_OnChangeFiresOnToggle()
    {
        var state = new SidebarState();
        bool fired = false;
        state.OnChange += () => fired = true;
        state.Toggle();
        fired.ShouldBeTrue();
    }
}
```

### CascadingParameter test (Sidebar root inside SidebarProvider):
```csharp
[Fact(DisplayName = "UAC08: Sidebar renders data-state from SidebarState")]
[Trait(Spec.UAC, "08")]
public void UAC08_RendersDataStateFromSidebarState()
{
    var cut = Ctx.Render<SidebarProvider>(p => p
        .Add(sp => sp.DefaultOpen, true)
        .AddChildContent<Sidebar>());
    cut.Find("[data-slot='sidebar']")
       .GetAttribute("data-state")
       .ShouldBe("expanded");
}
```

### IsActive / data-active test pattern:
```csharp
[Fact(DisplayName = "UAC05: SidebarMenuButton renders data-active when IsActive is true")]
[Trait(Spec.UAC, "05")]
public void UAC05_RendersDataActiveWhenTrue()
{
    var cut = Ctx.Render<SidebarMenuButton>(p => p.Add(x => x.IsActive, true));
    cut.Find("[data-slot='sidebar-menu-button']")
       .GetAttribute("data-active")
       .ShouldBe("true");
}

[Fact(DisplayName = "UAC06: SidebarMenuButton omits data-active when IsActive is false")]
[Trait(Spec.UAC, "06")]
public void UAC06_OmitsDataActiveWhenFalse()
{
    var cut = Ctx.Render<SidebarMenuButton>(p => p.Add(x => x.IsActive, false));
    cut.Find("[data-slot='sidebar-menu-button']")
       .GetAttribute("data-active")
       .ShouldBeNull();
}
```

## Implementation Notes

- **Sidebar root** is auto-generated (`Sidebar.razor` + `Sidebar.razor.cs`) and extended via `Sidebar.shared.cs` (partial class). Custom parameters (`Side`, `Variant`, `Collapsible`, `SidebarState` cascading) live in the shared file.
- **SidebarProvider** owns the `SidebarState` instance, cascades it via `<CascadingValue>`. It subscribes to `SidebarState.OnChange` to trigger re-renders and implements `IDisposable` to unsubscribe.
- **SidebarRail** and **SidebarTrigger** both consume `SidebarState` via `[CascadingParameter]` and call `SidebarState.Toggle()` on click.
- **SidebarSeparator** (`<hr>`) and **SidebarInput** (`<input>`) do **not** inherit `TinyComponentBase`. They are standalone components that define their own `Class` and `AdditionalAttributes` parameters. They also do not accept `ChildContent` (void elements).
- **SidebarMenuSkeleton** does not accept `ChildContent`. It renders its own internal skeleton structure (icon placeholder + randomized-width text placeholder).
- **SidebarMenuButton** and **SidebarMenuSubButton** accept an `IsActive` boolean parameter that conditionally renders `data-active="true"` (present when active, absent when inactive).
- CSS class merging follows the pattern: `class="default-class @Class"` where `@Class` is the user-supplied override.
- The `data-state` attribute on the Sidebar root is driven by `SidebarState.State` (computed: `Open ? "expanded" : "collapsed"`).
