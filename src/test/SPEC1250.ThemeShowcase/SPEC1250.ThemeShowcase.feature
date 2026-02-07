@SPEC1250
Feature: SPEC1250 - Theme Showcase Port
    As a Blazor developer
    I want to port the Vite theme showcase to Blazor
    So that I can demonstrate the component library in realistic, production-quality layouts

    Background:
        Given I have a component library at "src/lib"
        And I have a Blazor theme project at "src/theme"
        And the Vite sample exists at "samples/vite"
        And component classes are defined in "src/lib/wwwroot/css/input.css"
        And CSS variables are defined in "src/lib/wwwroot/css/variables.css"
        And the project follows dark-only theming per .cursorrules

    @EPIC
    Scenario: Port Vite theme showcase to Blazor
        Given the Vite theme showcase demonstrates a complete admin dashboard
        When I port the showcase to Blazor
        Then the Blazor theme should match the Vite visual design
        And the theme should use the component library classes
        And the theme should follow dark-only theming rules
        And the theme should be fully responsive
        And the theme should be accessible (WCAG AA)

    # This epic is broken down into the following User Stories:
    # - US01: Dashboard Demo Port (5 demo pages)
    # - US02: Layout and Navigation (sidebar, toolbar, routing)
    # - US03: Component Showcase Coverage (cards, buttons, tables, forms, etc.)
    # - US04: Theming and Styling Compliance (semantic colors, dark theme, spacing, typography)

    @OVERVIEW
    Scenario: SPEC1250 User Story Overview
        Given SPEC1250 defines the theme showcase port
        Then US01 should port 5 dashboard demo pages (demo1-demo5)
        And US02 should implement layout shell and navigation routing
        And US03 should ensure comprehensive component coverage across demos
        And US04 should enforce theming and styling compliance

    @SCOPE
    Scenario: SPEC1250 Scope Definition
        Given the Vite sample has multiple demo layouts and pages
        When defining the scope for SPEC1250
        Then the scope should include demo1 through demo5 dashboards
        And the scope should include demo1 layout (sidebar + toolbar)
        And the scope should NOT include auth pages (handled separately)
        And the scope should NOT include account pages (handled separately)
        And the scope should NOT include network pages (handled separately)
        And the scope should NOT include public profile pages (handled separately)
        And the scope should NOT include store pages (handled separately)
        And the scope should focus on core dashboard showcase only

    @ACCEPTANCE
    Scenario: SPEC1250 Overall Acceptance Criteria
        Given SPEC1250 implementation is complete
        When I review the Blazor theme showcase
        #UAC01
        Then all 5 demo pages (demo1-demo5) should be fully implemented
        #UAC02
        And demo1 light sidebar should be the default home page
        #UAC03
        And demo1 dark sidebar should be accessible via route
        #UAC04
        And all demos should use consistent layout shell
        #UAC05
        And navigation should work between all demo pages
        #UAC06
        And sidebar menu should highlight active demo
        #UAC07
        And all component classes from input.css should be used (no raw Tailwind)
        #UAC08
        And all colors should use semantic CSS variables
        #UAC09
        And theme should be dark-only with no light theme code
        #UAC10
        And all pages should be responsive (mobile, tablet, desktop)
        #UAC11
        And all interactive elements should be keyboard accessible
        #UAC12
        And color contrast should meet WCAG AA standards
        #UAC13
        And the Blazor theme should visually match the Vite sample
        #UAC14
        And build should complete without errors
        #UAC15
        And all theme pages should have corresponding bUnit tests

    @TECHNICAL
    Scenario: SPEC1250 Technical Architecture
        Given the theme showcase is a Blazor application
        When implementing the theme
        Then it should use Blazor components (.razor files)
        And it should use the component library from src/lib
        And it should reference lib CSS via link tag in index.html or App.razor
        And it should use Blazor routing with RouteAttribute
        And it should use LayoutComponentBase for layout shells
        And it should use NavigationManager for route changes
        And it should use RenderFragment for page content
        And it should use cascading parameters for shared state if needed
        And it should follow Blazor best practices and conventions

    @DEPENDENCIES
    Scenario: SPEC1250 Dependencies
        Given SPEC1250 requires certain prerequisites
        Then the component library (src/lib) should be built
        And input.css should define all component classes
        And variables.css should define all semantic colors
        And the Vite sample should be available for reference
        And keenicons should be available for icon usage
        And bUnit should be available for component testing

    @DELIVERABLES
    Scenario: SPEC1250 Deliverables
        Given SPEC1250 is complete
        Then the following should be delivered:
        # Pages
        And Demo1LightSidebar.razor page component
        And Demo1DarkSidebar.razor page component
        And Demo2.razor page component
        And Demo3.razor page component
        And Demo4.razor page component
        And Demo5.razor page component
        # Layouts
        And Demo1Layout.razor layout component
        And Sidebar.razor navigation component
        And SidebarHeader.razor component
        And SidebarMenu.razor component
        And Header.razor or Toolbar.razor component
        And Breadcrumb.razor component
        And Footer.razor component
        # Section Components (reusable across demos)
        And ChannelStats.razor component
        And EntryCallout.razor component
        And Highlights.razor component
        And EarningsChart.razor component (or placeholder)
        And TeamMeeting.razor component
        And Teams.razor table component
        And Options.razor component (demo2, demo5 variants)
        And MyBalance.razor component
        And Integrations.razor component (demo2, demo3 variants)
        And ManageData.razor component
        And CreateTeam.razor component
        And TeamInfo.razor component
        And Faq.razor component
        # Tests
        And US01 bUnit tests for all demo pages
        And US02 bUnit tests for layout and navigation
        And US03 bUnit tests for component coverage
        And US04 bUnit tests for theming compliance
        # Documentation
        And Updated SPEC1250 feature files with all UACs
        And Code comments explaining component structure
        And README or guide for theme structure

    @RISKS
    Scenario: SPEC1250 Risk Mitigation
        Given complex UI ports have inherent risks
        Then the following risks should be mitigated:
        # Risk: Visual parity is hard to achieve
        And implement one demo at a time with visual comparison
        # Risk: Component library may be incomplete
        And identify missing components early and implement or document
        # Risk: Chart components may be complex
        And use placeholders for charts initially, implement later
        # Risk: Responsive design is complex
        And test at each breakpoint (mobile, tablet, desktop)
        # Risk: Accessibility may be overlooked
        And audit with keyboard navigation and screen reader
        # Risk: Semantic colors may not match Vite exactly
        And document color mapping from Vite to Blazor variables
        # Risk: Performance issues with large demos
        And use virtualization or lazy loading if needed

    @PHASES
    Scenario: SPEC1250 Implementation Phases
        Given SPEC1250 is a large effort
        When planning implementation
        # Phase 1: Foundation
        Then Phase 1 should implement US02 layout shell and routes
        # Phase 2: First Demo
        And Phase 2 should implement US01.SC01 demo1 light sidebar fully
        # Phase 3: Remaining Demos
        And Phase 3 should implement US01.SC02-SC06 remaining demos
        # Phase 4: Polish
        And Phase 4 should address US03 component coverage gaps
        And Phase 4 should address US04 theming compliance issues
        # Phase 5: Testing
        And Phase 5 should implement comprehensive bUnit tests
        And Phase 5 should perform visual regression testing

    @VALIDATION
    Scenario: SPEC1250 Validation Checklist
        Given SPEC1250 is ready for review
        When validating the implementation
        Then visual comparison with Vite should show high fidelity
        And all routes should navigate correctly
        And sidebar menu should work (expand/collapse, active state)
        And all cards should use card component class
        And all buttons should use btn component classes
        And all inputs should use input component class
        And all tables should use table component classes
        And all badges should use badge component class
        And all colors should be semantic (no hardcoded values)
        And dark theme should be consistent across all pages
        And spacing should follow approved scale
        And typography should follow approved scale
        And responsive breakpoints should work correctly
        And keyboard navigation should work for all interactive elements
        And screen reader should announce page structure correctly
        And build should have zero errors and zero warnings
        And all bUnit tests should pass
