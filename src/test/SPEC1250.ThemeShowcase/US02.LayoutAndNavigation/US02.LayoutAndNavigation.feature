@US02
Feature: US02 - Theme Layout and Navigation
    As a Blazor developer
    I want a consistent layout and navigation for the theme showcase
    So that demo pages feel cohesive and are easy to explore

    Background:
        Given I have a Blazor theme project at "src/theme"
        And the Vite sample exists at "samples/vite"
        And component classes are defined in "src/lib/wwwroot/css/input.css"

    @SC01
    Scenario: Implement demo1 layout shell - Main structure
        Given the Vite demo1 layout exists at "samples/vite/src/layouts/demo1"
        When I build the Blazor demo1 layout shell
        #UAC01
        Then it should render a wrapper div with demo1 layout identifier
        #UAC02
        And it should contain a sidebar component as the first child
        #UAC03
        And it should contain a wrapper div for header and main content
        #UAC04
        And the layout should use flex or grid to position sidebar and content area
        #UAC05
        And the layout should be responsive with sidebar collapse on mobile

    @SC01.1
    Scenario: Implement demo1 sidebar component
        Given the Vite demo1 Sidebar component exists
        When I render the demo1 sidebar
        #UAC06
        Then it should render a sidebar container with fixed or sticky positioning
        #UAC07
        And it should display a SidebarHeader component at the top
        #UAC08
        And it should render a SidebarMenu component for navigation
        #UAC09
        And sidebar width should be defined using sidebar component classes
        #UAC10
        And sidebar should use dark background semantic colors
        #UAC11
        And sidebar should support collapsed state using peer-checked pattern

    @SC01.2
    Scenario: Implement demo1 sidebar header
        Given the Vite demo1 SidebarHeader component exists
        When I render the sidebar header
        #UAC12
        Then it should display the application logo or brand icon
        #UAC13
        And it should show the application name or title
        #UAC14
        And it should include a collapse/expand toggle button
        #UAC15
        And the header should use appropriate padding and spacing

    @SC01.3
    Scenario: Implement demo1 sidebar menu
        Given the Vite demo1 SidebarMenu component exists
        When I render the sidebar menu
        #UAC16
        Then it should render a vertical navigation list
        #UAC17
        And each menu item should have an icon and label
        #UAC18
        And menu items should highlight the active/current route
        #UAC19
        And menu items should support nested sub-menu items
        #UAC20
        And menu should use nav and link component classes
        #UAC21
        And menu items should be keyboard navigable

    @SC01.4
    Scenario: Implement demo1 header/toolbar component
        Given the Vite demo1 Header/Toolbar component exists
        When I render the demo1 header
        #UAC22
        Then it should render a header bar at the top of content area
        #UAC23
        And it should display a Breadcrumb component for navigation context
        #UAC24
        And it should include page title or heading
        #UAC25
        And it should render toolbar actions (search, notifications, profile)
        #UAC26
        And header should use toolbar component classes
        #UAC27
        And header should be sticky or fixed at top on scroll

    @SC01.5
    Scenario: Implement demo1 breadcrumb component
        Given the Vite demo1 Breadcrumb component exists
        When I render the breadcrumb navigation
        #UAC28
        Then it should display breadcrumb trail from root to current page
        #UAC29
        And each breadcrumb item should be clickable except the last
        #UAC30
        And breadcrumb items should be separated by chevron or slash
        #UAC31
        And breadcrumb should use breadcrumb component classes
        #UAC32
        And breadcrumb should be aria-labeled for accessibility

    @SC01.6
    Scenario: Implement demo1 content wrapper
        Given the Vite demo1 Content component exists
        When I render the main content area
        #UAC33
        Then it should render a main element with content role
        #UAC34
        And it should provide appropriate padding and max-width
        #UAC35
        And it should render the page-specific content via RenderBody or @Body
        #UAC36
        And content area should be scrollable when overflow occurs
        #UAC37
        And content should use container component classes

    @SC01.7
    Scenario: Implement demo1 footer component
        Given the Vite demo1 Footer component exists
        When I render the demo1 footer
        #UAC38
        Then it should render at the bottom of the content area
        #UAC39
        And it should display copyright or legal information
        #UAC40
        And it should include footer links (Privacy, Terms, Contact)
        #UAC41
        And footer should use footer component classes

    @SC01.8
    Scenario: Implement demo1 mega menu component
        Given the Vite demo1 MegaMenu component exists
        When I render the mega menu
        #UAC42
        Then it should render as a dropdown or overlay panel
        #UAC43
        And it should display organized sections of navigation links
        #UAC44
        And it should support icons for menu categories
        #UAC45
        And mega menu should close on outside click or escape key
        #UAC46
        And mega menu should use menu and dropdown component classes

    @SC01.9
    Scenario: Implement demo1 mega menu mobile
        Given the Vite demo1 MegaMenuMobile component exists
        When I render the mobile mega menu
        #UAC47
        Then it should render as a full-screen or slide-out panel on mobile
        #UAC48
        And it should include a close button
        #UAC49
        And it should display the same navigation structure as desktop
        #UAC50
        And mobile menu should animate in/out on open/close
        #UAC51
        And mobile menu should trap focus when open

    @SC02
    Scenario: Configure theme routes - Main routing
        Given the demo dashboards are implemented
        When I register the Blazor routes
        #UAC52
        Then the root route "/" should map to Demo1LightSidebar page
        #UAC53
        And route "/dark-sidebar" should map to Demo1DarkSidebar page
        #UAC54
        And route "/demo2" or "/dashboards/demo2" should map to Demo2 page
        #UAC55
        And route "/demo3" or "/dashboards/demo3" should map to Demo3 page
        #UAC56
        And route "/demo4" or "/dashboards/demo4" should map to Demo4 page
        #UAC57
        And route "/demo5" or "/dashboards/demo5" should map to Demo5 page

    @SC02.1
    Scenario: Configure theme routes - Layout association
        Given routes are defined for all demo pages
        When the application navigates to a demo route
        #UAC58
        Then demo1 light and dark sidebar should use Demo1Layout
        #UAC59
        And demo2-5 should use appropriate layout variants (or Demo1Layout as base)
        #UAC60
        And each layout should wrap its page component correctly
        #UAC61
        And layout should persist across navigation within same layout group

    @SC02.2
    Scenario: Configure theme routes - Error handling
        Given the routing is configured
        When a user navigates to an unknown route
        #UAC62
        Then the application should show a 404 Not Found page
        #UAC63
        And the not found page should use the demo1 layout shell
        #UAC64
        And the not found page should display helpful navigation links
        #UAC65
        And the not found page should use error page component classes

    @SC02.3
    Scenario: Layout navigation state management
        Given a user is navigating between demo pages
        When the route changes
        #UAC66
        Then the sidebar menu should update active item highlighting
        #UAC67
        And the breadcrumb should update to reflect current page
        #UAC68
        And the page title should update in the header
        #UAC69
        And the sidebar collapse state should persist across navigation
        #UAC70
        And scroll position should reset to top on route change
