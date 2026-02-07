@US01
Feature: US01 - Dashboard Demo Port
    As a Blazor developer
    I want the Vite dashboard demos ported to Blazor
    So that the theme showcases core layout and data presentation components

    Background:
        Given I have a Blazor theme project at "src/theme"
        And the Vite sample exists at "samples/vite"
        And component classes are defined in "src/lib/wwwroot/css/input.css"

    @SC01
    Scenario: Port demo1 light sidebar dashboard
        Given the Vite demo1 light sidebar page exists
        When I build the Blazor demo1 light sidebar page
        #UAC01
        Then it should render the channel stats cards section
        #UAC02
        And it should render the entry callout card section
        #UAC03
        And it should render the highlights card section
        #UAC04
        And it should render the earnings chart or a placeholder
        #UAC05
        And it should render the teams table section
        #UAC06
        And it should use component classes for cards and buttons

    @SC02
    Scenario: Port demo1 dark sidebar dashboard
        Given the Vite demo1 dark sidebar page exists
        When I build the Blazor demo1 dark sidebar page
        #UAC07
        Then it should reuse the demo1 light sidebar content
        #UAC08
        And it should use the dark sidebar layout shell
        #UAC09
        And it should keep the toolbar with date range control

    @SC03
    Scenario: Port demo2 dashboard
        Given the Vite demo2 dashboard page exists
        When I build the Blazor demo2 dashboard page
        #UAC10
        Then it should render options and my balance sections
        #UAC11
        And it should render entry callout and report settings sections
        #UAC12
        And it should render integrations and block list sections
        #UAC13
        And it should render teams and manage data sections

    @SC04
    Scenario: Port demo3 dashboard
        Given the Vite demo3 dashboard page exists
        When I build the Blazor demo3 dashboard page
        #UAC14
        Then it should render create team and highlights sections
        #UAC15
        And it should render teams and block list sections
        #UAC16
        And it should render integrations and manage data sections

    @SC05
    Scenario: Port demo4 dashboard
        Given the Vite demo4 dashboard page exists
        When I build the Blazor demo4 dashboard page
        #UAC17
        Then it should render create team and highlights sections
        #UAC18
        And it should render teams and block list sections

    @SC06
    Scenario: Port demo5 dashboard
        Given the Vite demo5 dashboard page exists
        When I build the Blazor demo5 dashboard page
        #UAC19
        Then it should render team info and options sections
        #UAC20
        And it should render teams and FAQ sections
