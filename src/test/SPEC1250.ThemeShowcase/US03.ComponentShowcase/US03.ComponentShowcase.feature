@US03
Feature: US03 - Component Showcase Coverage
    As a Blazor developer
    I want the theme pages to exercise core components
    So that the component library is demonstrated in realistic layouts

    Background:
        Given I have a Blazor theme project at "src/theme"
        And component classes are defined in "src/lib/wwwroot/css/input.css"

    @SC01
    Scenario: Showcase cards and buttons
        Given the dashboard demos are implemented
        When I review the theme pages
        #UAC01
        Then cards should be used for major content sections
        #UAC02
        And buttons should use btn classes with explicit variants

    @SC02
    Scenario: Showcase tables or data grids
        Given the dashboard demos are implemented
        When I review the teams section
        #UAC03
        Then it should render a table or data grid presentation
        #UAC04
        And the header and rows should follow table component classes

    @SC03
    Scenario: Showcase inputs and selectors
        Given the dashboard demos are implemented
        When I review filtering or options sections
        #UAC05
        Then it should render inputs with the input class
        #UAC06
        And it should render select or toggle controls where applicable

    @SC04
    Scenario: Showcase badges and stats
        Given the dashboard demos are implemented
        When I review highlights or stats sections
        #UAC07
        Then it should render badges using badge classes
        #UAC08
        And it should render stat cards using stat classes where applicable
