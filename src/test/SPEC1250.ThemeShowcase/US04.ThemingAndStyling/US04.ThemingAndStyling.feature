@US04
Feature: US04 - Theming and Styling Compliance
    As a Blazor developer
    I want the theme port to follow styling rules
    So that the showcase matches the project's dark theme and component system

    Background:
        Given I have a Blazor theme project at "src/theme"
        And component classes are defined in "src/lib/wwwroot/css/input.css"
        And CSS variables are defined in "src/lib/wwwroot/css/variables.css"

    @SC01
    Scenario: Enforce semantic colors
        Given the theme pages are implemented
        When I inspect visual styles
        #UAC01
        Then no hardcoded color values should be used
        #UAC02
        And semantic color utilities should be used consistently

    @SC02
    Scenario: Enforce dark-only theme
        Given the theme pages are implemented
        When I load the theme pages
        #UAC03
        Then the color-scheme should be dark
        #UAC04
        And there should be no light theme overrides

    @SC03
    Scenario: Enforce spacing and typography scale
        Given the theme pages are implemented
        When I review layout spacing and headings
        #UAC05
        Then spacing should use the approved scale (gap-2/4/6, space-y-6)
        #UAC06
        And typography should use the approved text scale classes
