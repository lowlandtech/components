Feature: US11 - Application Shell Components
    As a developer building enterprise applications
    I want a comprehensive sidebar navigation system
    So that I can provide consistent navigation across 100+ plugins

    Scenario: SC01 - Sidebar renders with default configuration
        Given I have a Sidebar component specification
        When I render the Sidebar with default settings
        Then the Sidebar should be visible
        And it should have the expanded state
        And it should be positioned on the left side
        And it should use the sidebar variant

    Scenario: SC02 - Sidebar supports collapsed state
        Given I have a Sidebar component specification
        When I render the Sidebar in collapsed state
        Then the Sidebar should apply collapsed state styles
        And it should reduce its width appropriately

    Scenario: SC03 - SidebarProvider manages state
        Given I have a SidebarProvider specification
        When I wrap components with SidebarProvider
        Then child components should access sidebar state via context
        And the provider should manage open/closed state
        And the provider should track mobile vs desktop state

    Scenario: SC04 - SidebarTrigger toggles sidebar
        Given I have a SidebarTrigger specification
        When I click the SidebarTrigger button
        Then the Sidebar should toggle between expanded and collapsed
        And the state change should be reflected in the context

    Scenario: SC05 - Sidebar supports mobile responsive behavior
        Given I have a Sidebar component specification
        When I render the Sidebar on mobile viewport
        Then it should render as a Sheet overlay
        And it should be hidden by default
        And it should open when triggered

    Scenario: SC06 - Sidebar supports variants
        Given I have a Sidebar component specification
        When I render the Sidebar with variant "floating"
        Then it should apply floating variant styles
        And it should have rounded corners and shadow

    Scenario: SC07 - Sidebar supports collapsible modes
        Given I have a Sidebar component specification
        When I set collapsible mode to "icon"
        Then the Sidebar should collapse to icon width
        And text labels should be hidden in collapsed state

    Scenario: SC08 - SidebarMenu renders navigation structure
        Given I have a SidebarMenu specification
        When I render SidebarMenu with SidebarMenuItem children
        Then the menu should render as a list
        And menu items should be interactive

    Scenario: SC09 - SidebarMenuButton supports active state
        Given I have a SidebarMenuButton specification
        When I set isActive to true
        Then the button should display active state styles
        And it should be visually distinct from inactive items

    Scenario: SC10 - SidebarGroup organizes menu sections
        Given I have a SidebarGroup specification
        When I render a SidebarGroup with label and content
        Then the group should render with proper spacing
        And the label should be visible in expanded state
        And the label should hide in collapsed icon mode

    Scenario: SC11 - SidebarContent provides scrollable area
        Given I have a SidebarContent specification
        When I render content that overflows
        Then the content area should be scrollable
        And scrollbars should be hidden by default

    Scenario: SC12 - Sidebar supports left and right positioning
        Given I have a Sidebar component specification
        When I set side to "right"
        Then the Sidebar should position on the right
        And all positioning styles should flip appropriately

    Scenario: SC13 - SidebarInset provides main content area
        Given I have a SidebarInset specification
        When I render SidebarInset as main content
        Then it should fill remaining space
        And it should respect sidebar variant margins

    Scenario: SC14 - SidebarRail provides resize handle
        Given I have a SidebarRail specification
        When I hover over the SidebarRail
        Then it should show visual feedback
        And clicking should toggle sidebar state

    Scenario: SC15 - SidebarMenuButton supports tooltip
        Given I have a SidebarMenuButton specification with tooltip
        When the Sidebar is in collapsed state
        Then tooltips should display on hover
        And tooltips should be hidden in expanded state
