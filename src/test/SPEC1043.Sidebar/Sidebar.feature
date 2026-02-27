Feature: Sidebar
  SPEC1043 | Composable sidebar navigation system with collapsible state, sub-menus, and responsive mobile support

  Background:
    Given the component library LowlandTech.Components.Sidebar

  # ── SidebarState (state management) ────────────────────────────

  @US02 @state @SC05
  Scenario: US02-SC05-UAC01 SidebarState initial Open is true
    Given a new SidebarState instance
    Then Open should be true

  @US02 @state @SC05
  Scenario: US02-SC05-UAC02 SidebarState initial State is expanded
    Given a new SidebarState instance
    Then State should be "expanded"

  @US02 @state @SC05
  Scenario: US02-SC05-UAC03 SidebarState Toggle switches Open to false
    Given a new SidebarState instance with IsMobile false
    When Toggle is called
    Then Open should be false

  @US02 @state @SC05
  Scenario: US02-SC05-UAC04 SidebarState Toggle on mobile switches OpenMobile
    Given a new SidebarState instance with IsMobile true
    When Toggle is called
    Then OpenMobile should be true

  @US02 @state @SC05
  Scenario: US02-SC05-UAC05 SidebarState SetOpen changes Open
    Given a new SidebarState instance
    When SetOpen is called with false
    Then Open should be false
    And State should be "collapsed"

  @US02 @state @SC05
  Scenario: US02-SC05-UAC06 SidebarState OnChange fires on Toggle
    Given a new SidebarState instance with an OnChange subscriber
    When Toggle is called
    Then the OnChange event should have fired

  # ── SidebarProvider ────────────────────────────────────────────

  @US01 @view @SC05
  Scenario: US01-SC05-UAC01 SidebarProvider renders data-slot attribute
    When a SidebarProvider component is rendered
    Then it should have data-slot="sidebar-provider"

  @US01 @view @SC05
  Scenario: US01-SC05-UAC02 SidebarProvider renders CSS variables for sidebar width
    When a SidebarProvider component is rendered with Width="16rem" and MobileWidth="18rem"
    Then the root element style should contain "--sidebar-width: 16rem"
    And the root element style should contain "--sidebar-width-mobile: 18rem"

  @US01 @view @SC05
  Scenario: US01-SC05-UAC03 SidebarProvider renders ChildContent
    When a SidebarProvider component is rendered with child content "Provider Content"
    Then the markup should contain "Provider Content"

  @US01 @view @SC05
  Scenario: US01-SC05-UAC04 SidebarProvider cascades SidebarState to children
    When a SidebarProvider is rendered containing a Sidebar child
    Then the Sidebar child should receive a cascaded SidebarState

  # ── Sidebar (root) ────────────────────────────────────────────

  @US01 @view @SC01
  Scenario: US01-SC01-UAC01 Sidebar renders data-slot attribute
    When a Sidebar component is rendered
    Then it should have data-slot="sidebar"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC02 Sidebar renders ChildContent
    When a Sidebar component is rendered with child content "Sidebar Content"
    Then the markup should contain "Sidebar Content"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC03 Sidebar renders as aside element
    When a Sidebar component is rendered
    Then the root element should be a "aside"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC04 Sidebar renders default app-sidebar CSS class
    When a Sidebar component is rendered
    Then the root element should have class "app-sidebar"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC05 Sidebar renders data-side attribute with default left
    When a Sidebar component is rendered
    Then the root element should have data-side="left"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC06 Sidebar renders data-variant attribute with default sidebar
    When a Sidebar component is rendered
    Then the root element should have data-variant="sidebar"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC07 Sidebar renders data-collapsible attribute with default offcanvas
    When a Sidebar component is rendered
    Then the root element should have data-collapsible="offcanvas"

  @US01 @view @SC01
  Scenario: US01-SC01-UAC08 Sidebar renders data-state from SidebarState
    When a Sidebar component is rendered with a cascaded SidebarState where Open is true
    Then the root element should have data-state="expanded"

  # ── SidebarHeader ─────────────────────────────────────────────

  @US01 @view @SC02
  Scenario: US01-SC02-UAC01 SidebarHeader renders data-slot attribute
    When a SidebarHeader component is rendered
    Then it should have data-slot="sidebar-header"

  @US01 @view @SC02
  Scenario: US01-SC02-UAC02 SidebarHeader renders ChildContent
    When a SidebarHeader component is rendered with child content "Header"
    Then the markup should contain "Header"

  @US01 @view @SC02
  Scenario: US01-SC02-UAC03 SidebarHeader renders default CSS class
    When a SidebarHeader component is rendered
    Then the root element should have class "sidebar-header"

  # ── SidebarContent ────────────────────────────────────────────

  @US01 @view @SC03
  Scenario: US01-SC03-UAC01 SidebarContent renders data-slot attribute
    When a SidebarContent component is rendered
    Then it should have data-slot="sidebar-content"

  @US01 @view @SC03
  Scenario: US01-SC03-UAC02 SidebarContent renders ChildContent
    When a SidebarContent component is rendered with child content "Nav Items"
    Then the markup should contain "Nav Items"

  @US01 @view @SC03
  Scenario: US01-SC03-UAC03 SidebarContent renders default CSS class
    When a SidebarContent component is rendered
    Then the root element should have class "app-sidebar-nav"

  # ── SidebarFooter ─────────────────────────────────────────────

  @US01 @view @SC04
  Scenario: US01-SC04-UAC01 SidebarFooter renders data-slot attribute
    When a SidebarFooter component is rendered
    Then it should have data-slot="sidebar-footer"

  @US01 @view @SC04
  Scenario: US01-SC04-UAC02 SidebarFooter renders ChildContent
    When a SidebarFooter component is rendered with child content "Footer"
    Then the markup should contain "Footer"

  @US01 @view @SC04
  Scenario: US01-SC04-UAC03 SidebarFooter renders default CSS class
    When a SidebarFooter component is rendered
    Then the root element should have class "sidebar-footer"

  # ── SidebarInset ──────────────────────────────────────────────

  @US01 @view @SC06
  Scenario: US01-SC06-UAC01 SidebarInset renders data-slot attribute
    When a SidebarInset component is rendered
    Then it should have data-slot="sidebar-inset"

  @US01 @view @SC06
  Scenario: US01-SC06-UAC02 SidebarInset renders ChildContent
    When a SidebarInset component is rendered with child content "Main Area"
    Then the markup should contain "Main Area"

  @US01 @view @SC06
  Scenario: US01-SC06-UAC03 SidebarInset renders default CSS class
    When a SidebarInset component is rendered
    Then the root element should have class "sidebar-inset"

  @US01 @view @SC06
  Scenario: US01-SC06-UAC04 SidebarInset renders as main element
    When a SidebarInset component is rendered
    Then the root element should be a "main"

  # ── SidebarRail ───────────────────────────────────────────────

  @US01 @view @SC07
  Scenario: US01-SC07-UAC01 SidebarRail renders data-slot attribute
    When a SidebarRail component is rendered
    Then it should have data-slot="sidebar-rail"

  @US01 @view @SC07
  Scenario: US01-SC07-UAC02 SidebarRail renders default CSS class
    When a SidebarRail component is rendered
    Then the root element should have class "sidebar-rail"

  @US01 @view @SC07
  Scenario: US01-SC07-UAC03 SidebarRail renders as button element
    When a SidebarRail component is rendered
    Then the root element should be a "button"

  @US01 @view @SC07
  Scenario: US01-SC07-UAC04 SidebarRail renders aria-label
    When a SidebarRail component is rendered
    Then the root element should have aria-label="Toggle Sidebar"

  # ── SidebarTrigger ────────────────────────────────────────────

  @US01 @view @SC08
  Scenario: US01-SC08-UAC01 SidebarTrigger renders data-slot attribute
    When a SidebarTrigger component is rendered
    Then it should have data-slot="sidebar-trigger"

  @US01 @view @SC08
  Scenario: US01-SC08-UAC02 SidebarTrigger renders ChildContent
    When a SidebarTrigger component is rendered with child content "Toggle"
    Then the markup should contain "Toggle"

  @US01 @view @SC08
  Scenario: US01-SC08-UAC03 SidebarTrigger renders default CSS class
    When a SidebarTrigger component is rendered
    Then the root element should have class "sidebar-trigger"

  @US01 @view @SC08
  Scenario: US01-SC08-UAC04 SidebarTrigger renders as button element
    When a SidebarTrigger component is rendered
    Then the root element should be a "button"

  # ── SidebarSeparator ──────────────────────────────────────────

  @US01 @view @SC09
  Scenario: US01-SC09-UAC01 SidebarSeparator renders data-slot attribute
    When a SidebarSeparator component is rendered
    Then it should have data-slot="sidebar-separator"

  @US01 @view @SC09
  Scenario: US01-SC09-UAC02 SidebarSeparator renders default CSS class
    When a SidebarSeparator component is rendered
    Then the root element should have class "sidebar-separator"

  @US01 @view @SC09
  Scenario: US01-SC09-UAC03 SidebarSeparator renders as hr element
    When a SidebarSeparator component is rendered
    Then the root element should be a "hr"

  # ── SidebarInput ──────────────────────────────────────────────

  @US01 @view @SC10
  Scenario: US01-SC10-UAC01 SidebarInput renders data-slot attribute
    When a SidebarInput component is rendered
    Then it should have data-slot="sidebar-input"

  @US01 @view @SC10
  Scenario: US01-SC10-UAC02 SidebarInput renders default CSS class
    When a SidebarInput component is rendered
    Then the root element should have class "sidebar-input"

  @US01 @view @SC10
  Scenario: US01-SC10-UAC03 SidebarInput renders as input element
    When a SidebarInput component is rendered
    Then the root element should be a "input"

  # ── SidebarGroup ──────────────────────────────────────────────

  @US01 @view @SC11
  Scenario: US01-SC11-UAC01 SidebarGroup renders data-slot attribute
    When a SidebarGroup component is rendered
    Then it should have data-slot="sidebar-group"

  @US01 @view @SC11
  Scenario: US01-SC11-UAC02 SidebarGroup renders ChildContent
    When a SidebarGroup component is rendered with child content "Group"
    Then the markup should contain "Group"

  @US01 @view @SC11
  Scenario: US01-SC11-UAC03 SidebarGroup renders default CSS class
    When a SidebarGroup component is rendered
    Then the root element should have class "sidebar-group"

  # ── SidebarGroupLabel ─────────────────────────────────────────

  @US01 @view @SC12
  Scenario: US01-SC12-UAC01 SidebarGroupLabel renders data-slot attribute
    When a SidebarGroupLabel component is rendered
    Then it should have data-slot="sidebar-group-label"

  @US01 @view @SC12
  Scenario: US01-SC12-UAC02 SidebarGroupLabel renders ChildContent
    When a SidebarGroupLabel component is rendered with child content "Label"
    Then the markup should contain "Label"

  @US01 @view @SC12
  Scenario: US01-SC12-UAC03 SidebarGroupLabel renders default CSS class
    When a SidebarGroupLabel component is rendered
    Then the root element should have class "sidebar-group-label"

  # ── SidebarGroupContent ───────────────────────────────────────

  @US01 @view @SC13
  Scenario: US01-SC13-UAC01 SidebarGroupContent renders data-slot attribute
    When a SidebarGroupContent component is rendered
    Then it should have data-slot="sidebar-group-content"

  @US01 @view @SC13
  Scenario: US01-SC13-UAC02 SidebarGroupContent renders ChildContent
    When a SidebarGroupContent component is rendered with child content "Content"
    Then the markup should contain "Content"

  @US01 @view @SC13
  Scenario: US01-SC13-UAC03 SidebarGroupContent renders default CSS class
    When a SidebarGroupContent component is rendered
    Then the root element should have class "sidebar-group-content"

  # ── SidebarGroupAction ────────────────────────────────────────

  @US01 @view @SC14
  Scenario: US01-SC14-UAC01 SidebarGroupAction renders data-slot attribute
    When a SidebarGroupAction component is rendered
    Then it should have data-slot="sidebar-group-action"

  @US01 @view @SC14
  Scenario: US01-SC14-UAC02 SidebarGroupAction renders ChildContent
    When a SidebarGroupAction component is rendered with child content "Action"
    Then the markup should contain "Action"

  @US01 @view @SC14
  Scenario: US01-SC14-UAC03 SidebarGroupAction renders default CSS class
    When a SidebarGroupAction component is rendered
    Then the root element should have class "sidebar-group-action"

  @US01 @view @SC14
  Scenario: US01-SC14-UAC04 SidebarGroupAction renders as button element
    When a SidebarGroupAction component is rendered
    Then the root element should be a "button"

  # ── SidebarMenu ───────────────────────────────────────────────

  @US01 @view @SC15
  Scenario: US01-SC15-UAC01 SidebarMenu renders data-slot attribute
    When a SidebarMenu component is rendered
    Then it should have data-slot="sidebar-menu"

  @US01 @view @SC15
  Scenario: US01-SC15-UAC02 SidebarMenu renders ChildContent
    When a SidebarMenu component is rendered with child content "Menu"
    Then the markup should contain "Menu"

  @US01 @view @SC15
  Scenario: US01-SC15-UAC03 SidebarMenu renders default CSS class
    When a SidebarMenu component is rendered
    Then the root element should have class "sidebar-menu"

  @US01 @view @SC15
  Scenario: US01-SC15-UAC04 SidebarMenu renders as ul element
    When a SidebarMenu component is rendered
    Then the root element should be a "ul"

  # ── SidebarMenuItem ───────────────────────────────────────────

  @US01 @view @SC16
  Scenario: US01-SC16-UAC01 SidebarMenuItem renders data-slot attribute
    When a SidebarMenuItem component is rendered
    Then it should have data-slot="sidebar-menu-item"

  @US01 @view @SC16
  Scenario: US01-SC16-UAC02 SidebarMenuItem renders ChildContent
    When a SidebarMenuItem component is rendered with child content "Item"
    Then the markup should contain "Item"

  @US01 @view @SC16
  Scenario: US01-SC16-UAC03 SidebarMenuItem renders default CSS class
    When a SidebarMenuItem component is rendered
    Then the root element should have class "sidebar-menu-item"

  @US01 @view @SC16
  Scenario: US01-SC16-UAC04 SidebarMenuItem renders as li element
    When a SidebarMenuItem component is rendered
    Then the root element should be a "li"

  # ── SidebarMenuButton ─────────────────────────────────────────

  @US01 @view @SC17
  Scenario: US01-SC17-UAC01 SidebarMenuButton renders data-slot attribute
    When a SidebarMenuButton component is rendered
    Then it should have data-slot="sidebar-menu-button"

  @US01 @view @SC17
  Scenario: US01-SC17-UAC02 SidebarMenuButton renders ChildContent
    When a SidebarMenuButton component is rendered with child content "Button"
    Then the markup should contain "Button"

  @US01 @view @SC17
  Scenario: US01-SC17-UAC03 SidebarMenuButton renders default CSS class
    When a SidebarMenuButton component is rendered
    Then the root element should have class "sidebar-menu-button"

  @US01 @view @SC17
  Scenario: US01-SC17-UAC04 SidebarMenuButton renders as button element
    When a SidebarMenuButton component is rendered
    Then the root element should be a "button"

  @US01 @view @SC17
  Scenario: US01-SC17-UAC05 SidebarMenuButton renders data-active when IsActive is true
    When a SidebarMenuButton component is rendered with IsActive=true
    Then the root element should have data-active="true"

  @US01 @view @SC17
  Scenario: US01-SC17-UAC06 SidebarMenuButton omits data-active when IsActive is false
    When a SidebarMenuButton component is rendered with IsActive=false
    Then the root element should not have data-active

  # ── SidebarMenuAction ─────────────────────────────────────────

  @US01 @view @SC18
  Scenario: US01-SC18-UAC01 SidebarMenuAction renders data-slot attribute
    When a SidebarMenuAction component is rendered
    Then it should have data-slot="sidebar-menu-action"

  @US01 @view @SC18
  Scenario: US01-SC18-UAC02 SidebarMenuAction renders ChildContent
    When a SidebarMenuAction component is rendered with child content "Action"
    Then the markup should contain "Action"

  @US01 @view @SC18
  Scenario: US01-SC18-UAC03 SidebarMenuAction renders default CSS class
    When a SidebarMenuAction component is rendered
    Then the root element should have class "sidebar-menu-action"

  @US01 @view @SC18
  Scenario: US01-SC18-UAC04 SidebarMenuAction renders as button element
    When a SidebarMenuAction component is rendered
    Then the root element should be a "button"

  # ── SidebarMenuBadge ──────────────────────────────────────────

  @US01 @view @SC19
  Scenario: US01-SC19-UAC01 SidebarMenuBadge renders data-slot attribute
    When a SidebarMenuBadge component is rendered
    Then it should have data-slot="sidebar-menu-badge"

  @US01 @view @SC19
  Scenario: US01-SC19-UAC02 SidebarMenuBadge renders ChildContent
    When a SidebarMenuBadge component is rendered with child content "99"
    Then the markup should contain "99"

  @US01 @view @SC19
  Scenario: US01-SC19-UAC03 SidebarMenuBadge renders default CSS class
    When a SidebarMenuBadge component is rendered
    Then the root element should have class "sidebar-menu-badge"

  # ── SidebarMenuSkeleton ───────────────────────────────────────

  @US01 @view @SC20
  Scenario: US01-SC20-UAC01 SidebarMenuSkeleton renders data-slot attribute
    When a SidebarMenuSkeleton component is rendered
    Then it should have data-slot="sidebar-menu-skeleton"

  @US01 @view @SC20
  Scenario: US01-SC20-UAC02 SidebarMenuSkeleton renders default CSS class
    When a SidebarMenuSkeleton component is rendered
    Then the root element should have class "sidebar-menu-skeleton"

  @US01 @view @SC20
  Scenario: US01-SC20-UAC03 SidebarMenuSkeleton renders skeleton children
    When a SidebarMenuSkeleton component is rendered
    Then the markup should contain an element with class "sidebar-menu-skeleton-icon"
    And the markup should contain an element with class "sidebar-menu-skeleton-text"

  # ── SidebarMenuSub ────────────────────────────────────────────

  @US01 @view @SC21
  Scenario: US01-SC21-UAC01 SidebarMenuSub renders data-slot attribute
    When a SidebarMenuSub component is rendered
    Then it should have data-slot="sidebar-menu-sub"

  @US01 @view @SC21
  Scenario: US01-SC21-UAC02 SidebarMenuSub renders ChildContent
    When a SidebarMenuSub component is rendered with child content "SubMenu"
    Then the markup should contain "SubMenu"

  @US01 @view @SC21
  Scenario: US01-SC21-UAC03 SidebarMenuSub renders default CSS class
    When a SidebarMenuSub component is rendered
    Then the root element should have class "sidebar-menu-sub"

  @US01 @view @SC21
  Scenario: US01-SC21-UAC04 SidebarMenuSub renders as ul element
    When a SidebarMenuSub component is rendered
    Then the root element should be a "ul"

  # ── SidebarMenuSubItem ────────────────────────────────────────

  @US01 @view @SC22
  Scenario: US01-SC22-UAC01 SidebarMenuSubItem renders data-slot attribute
    When a SidebarMenuSubItem component is rendered
    Then it should have data-slot="sidebar-menu-sub-item"

  @US01 @view @SC22
  Scenario: US01-SC22-UAC02 SidebarMenuSubItem renders ChildContent
    When a SidebarMenuSubItem component is rendered with child content "SubItem"
    Then the markup should contain "SubItem"

  @US01 @view @SC22
  Scenario: US01-SC22-UAC03 SidebarMenuSubItem renders default CSS class
    When a SidebarMenuSubItem component is rendered
    Then the root element should have class "sidebar-menu-sub-item"

  @US01 @view @SC22
  Scenario: US01-SC22-UAC04 SidebarMenuSubItem renders as li element
    When a SidebarMenuSubItem component is rendered
    Then the root element should be a "li"

  # ── SidebarMenuSubButton ──────────────────────────────────────

  @US01 @view @SC23
  Scenario: US01-SC23-UAC01 SidebarMenuSubButton renders data-slot attribute
    When a SidebarMenuSubButton component is rendered
    Then it should have data-slot="sidebar-menu-sub-button"

  @US01 @view @SC23
  Scenario: US01-SC23-UAC02 SidebarMenuSubButton renders ChildContent
    When a SidebarMenuSubButton component is rendered with child content "Sub Link"
    Then the markup should contain "Sub Link"

  @US01 @view @SC23
  Scenario: US01-SC23-UAC03 SidebarMenuSubButton renders default CSS class
    When a SidebarMenuSubButton component is rendered
    Then the root element should have class "sidebar-menu-sub-button"

  @US01 @view @SC23
  Scenario: US01-SC23-UAC04 SidebarMenuSubButton renders as a element
    When a SidebarMenuSubButton component is rendered
    Then the root element should be a "a"

  @US01 @view @SC23
  Scenario: US01-SC23-UAC05 SidebarMenuSubButton renders data-active when IsActive is true
    When a SidebarMenuSubButton component is rendered with IsActive=true
    Then the root element should have data-active="true"

  @US01 @view @SC23
  Scenario: US01-SC23-UAC06 SidebarMenuSubButton omits data-active when IsActive is false
    When a SidebarMenuSubButton component is rendered with IsActive=false
    Then the root element should not have data-active
