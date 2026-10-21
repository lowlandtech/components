# Sidebar (SPEC1043)

**Status:** 🔍 Reviewed with feedback

## Summary

All 23 sidebar subcomponents are ported with full rendering. Gaps are behavioral: **keyboard shortcut** to toggle the sidebar (Cmd/Ctrl+B), **cookie-based state persistence** across page loads, and **mobile responsive** detection that switches to sheet mode on small screens.

## Visual Comparison

### Current Implementation (https://localhost:7245/visual/sidebar)
![Current Implementation](../../../.playwright-mcp/sidebar_screenshot.png)

### shadcn Reference
![shadcn Reference](../../src/components/app/visual-artifacts/chadcn/Sidebar.png)

### Discrepancies Table

| # | Feature | shadcn Reference | Current Implementation | Status |
|---|---------|------------------|------------------------|--------|
| 1 | **SidebarRail** | Visible as collapsible edge trigger (4px wide hoverable area) | Not shown in any example | ❌ Missing demo |
| 2 | **SidebarTrigger** | Hamburger/toggle button to open sidebar | Not shown in any example | ❌ Missing demo |
| 3 | **SidebarInput** | Search input field in sidebar header | Not demonstrated | ❌ Missing demo |
| 4 | **Collapsible state** | Animated width transition between expanded/collapsed | Static widths only (w-64, w-16) | ❌ Missing behavior |
| 5 | **SidebarInset** | Main content offset when sidebar is open | Not demonstrated with Provider | ⚠️ Partial |
| 6 | **Mobile Sheet** | Overlay sheet with backdrop on small screens | `SidebarMobileSheet` exists but not demoed | ⚠️ Not demonstrated |
| 7 | **Submenu items** | Expandable nested menu with chevron indicators | Components exist but not demonstrated | ❌ Missing demo |
| 8 | **Menu badges** | Counter badges on menu items (e.g., "12" unread) | Shown in "With User Profile" example | ✅ Present |
| 9 | **User profile footer** | Avatar + name + email in footer | Shown in "With User Profile" example | ✅ Present |
| 10 | **Group labels** | Section headers (e.g., "Overview", "Management") | Shown in "Grouped Navigation" example | ✅ Present |

## What Is Ported

- All structural subcomponents: SidebarProvider, Sidebar, SidebarTrigger, SidebarRail, SidebarInset, SidebarContent, SidebarHeader, SidebarFooter, SidebarSeparator
- All group/menu subcomponents: SidebarGroup, SidebarGroupLabel, SidebarGroupAction, SidebarGroupContent, SidebarMenu, SidebarMenuItem, SidebarMenuButton, SidebarMenuAction, SidebarMenuBadge, SidebarMenuSkeleton
- All submenu subcomponents: SidebarMenuSub, SidebarMenuSubButton, SidebarMenuSubItem
- SidebarInput
- `data-state="expanded|collapsed"` on Sidebar
- `collapsible`, `variant`, `side` parameters on Sidebar

## What Is No Longer a Gap

- **Ctrl+B / Cmd+B keyboard shortcut** — implemented in `SidebarProvider.razor.cs` via `@onkeydown="HandleKeyDown"`
- **Mobile sheet mode** — `SidebarMobileSheet` component provides an overlay sheet when `SidebarState.IsMobile` is true, including hamburger trigger, backdrop, and `role="dialog"` panel

## Remaining Gaps vs shadcn Reference

### 1. Missing cookie-based state persistence

chadcn stores the sidebar open/closed state in a cookie (`sidebar_state`) so it persists across page reloads.

```gherkin
Feature: Sidebar State Persistence
  As a user
  I want my sidebar preference to persist between page reloads
  So that I don't have to re-collapse it every time I navigate

  Scenario: Sidebar collapsed state is saved when toggled
    Given a SidebarProvider with DefaultOpen="true"
    When the user collapses the sidebar
    Then the sidebar state is written to a cookie named "sidebar_state"
    And the cookie value is "false"

  Scenario: Sidebar restores state from cookie on page load
    Given a cookie "sidebar_state" with value "false"
    When a SidebarProvider is initialized
    Then the Sidebar renders in collapsed state

  Scenario: Sidebar defaults to open when no cookie exists
    Given no "sidebar_state" cookie exists
    And SidebarProvider has no DefaultOpen specified
    When the component initializes
    Then the Sidebar renders in expanded state
```

### 2. Automatic mobile breakpoint detection requires JS interop

`SidebarMobileSheet` renders based on `SidebarState.IsMobile`, but nothing sets `IsMobile` automatically. shadcn uses a `useIsMobile()` hook watching `window.matchMedia`. In Blazor this requires JS interop and is not yet wired up.

```gherkin
Feature: Sidebar Automatic Mobile Detection
  As a developer
  I want IsMobile to be set automatically based on viewport width
  So that the sidebar switches to sheet mode without manual configuration

  Scenario: IsMobile is set when viewport is below mobile breakpoint
    Given a SidebarProvider is initialized
    And the viewport width is below 768px
    Then SidebarState.IsMobile is true
    And SidebarMobileSheet renders the sheet UI

  Scenario: IsMobile is false on desktop viewport
    Given the viewport width is at or above 768px
    Then SidebarState.IsMobile is false
    And Sidebar renders as an inline panel

  Scenario: IsMobile updates on window resize
    Given the viewport is resized from desktop to mobile width
    Then SidebarState.IsMobile updates to true
    And the sidebar switches to sheet mode
```

### 3. Missing visual demonstrations for key features

While components exist in the library, the visual test page does not demonstrate:

```gherkin
Feature: SidebarRail Demonstration
  As a developer
  I want to see the SidebarRail component in action
  So that I understand how to implement the collapsible sidebar pattern

  Scenario: SidebarRail toggles sidebar state on hover/click
    Given a collapsible sidebar with SidebarRail
    When the user hovers over the rail edge
    Then the rail becomes visible as a 4px wide interactive area
    When the user clicks the rail
    Then the sidebar toggles between expanded and collapsed states
    And the width animates smoothly between w-64 and w-16

Feature: SidebarTrigger Demonstration
  As a developer
  I want to see the SidebarTrigger component
  So that I can implement a hamburger menu toggle

  Scenario: SidebarTrigger button toggles sidebar
    Given a sidebar with a SidebarTrigger
    When the user clicks the trigger button
    Then the sidebar toggles open/closed state
    And the button icon changes to reflect the new state

Feature: SidebarInput Demonstration
  As a developer
  I want to see the SidebarInput component
  So that I can add search functionality to the sidebar

  Scenario: SidebarInput renders in header
    Given a sidebar with a search input in the header
    When the user types in the input
    Then the value is bound to the component
    And the search can filter menu items

Feature: Collapsible Submenu Demonstration
  As a developer
  I want to see nested submenu items
  So that I can implement hierarchical navigation

  Scenario: Submenu expands and collapses
    Given a menu item with nested submenu items
    When the user clicks the parent menu item
    Then the submenu expands showing child items
    And a chevron icon rotates to indicate open state
    When the user clicks the parent again
    Then the submenu collapses hiding child items

Feature: SidebarInset Demonstration
  As a developer
  I want to see the SidebarInset component
  So that I understand how to offset main content

  Scenario: SidebarInset adjusts content area
    Given a SidebarProvider with collapsible sidebar
    And a SidebarInset wrapping the main content
    When the sidebar is expanded
    Then the inset adds left margin to accommodate sidebar width
    When the sidebar collapses
    Then the inset margin reduces accordingly
```

### 4. Animation and transitions

The shadcn reference includes smooth CSS transitions when the sidebar collapses/expands. Current implementation uses static Tailwind classes without transition animations.

```gherkin
Feature: Sidebar Collapse Animation
  As a user
  I want smooth animations when the sidebar collapses or expands
  So that the UI feels polished and responsive

  Scenario: Sidebar width animates on toggle
    Given a collapsible sidebar
    When the sidebar state changes from expanded to collapsed
    Then the width animates over 200ms with ease-out timing
    And menu item text fades out during collapse
    And icons remain visible and centered

  Scenario: Menu items transition gracefully
    Given a collapsing sidebar
    When the width reduces below the text threshold
    Then the text labels fade out with opacity transition
    And only icons remain visible in collapsed state
```

---

## Navigation

[[components/_index|Back to Index]]