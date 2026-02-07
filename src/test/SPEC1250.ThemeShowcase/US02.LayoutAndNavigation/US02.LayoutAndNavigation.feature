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
    Scenario: Implement theme layout shell
        Given the Vite demo layout uses a sidebar and top toolbar
        When I build the Blazor theme layout
        #UAC01
        Then it should render a sidebar navigation area
        #UAC02
        And it should render a top toolbar area
        #UAC03
        And it should place page content in a main container
        #UAC04
        And it should use the peer-checked sidebar pattern if the sidebar is collapsible

    @SC02
    Scenario: Configure theme routes
        Given the demo dashboards are implemented
        When I register the Blazor routes
        #UAC05
        Then the root route should map to the default dashboard demo
        #UAC06
        And each demo should have its own route under the theme area
        #UAC07
        And unknown routes should show a not-found page
