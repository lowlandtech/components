@US08
Feature: US08 - Integrations and Utility Components
    As a Blazor developer
    I want integration and utility components in Blazor
    So that I can add advanced behaviors with consistent APIs

    Background:
        Given I have a Blazor Razor Class Library project at "src/components"
        And Tailwind CSS v4 is configured
        And component classes are defined in wwwroot/css/input.css

    @SC01
    Scenario: Implement Chart component
        Given I have the shadcn/ui Chart component
        When I port it to Blazor
        #UAC01
        Then it should render chart containers with appropriate classes
        #UAC02
        And it should support data and series configuration parameters
        #UAC03
        And it should support tooltips and legends
        #UAC04
        And it should expose a way to customize colors and labels

    @SC02
    Scenario: Implement Command component
        Given I have the shadcn/ui Command component
        When I port it to Blazor
        #UAC05
        Then I should have Command, CommandInput, CommandList, and CommandItem components
        #UAC06
        And it should support filtering items based on input
        #UAC07
        And it should support keyboard navigation within the list
        #UAC08
        And it should support empty and loading states

    @SC03
    Scenario: Implement Progress component
        Given I have the shadcn/ui Progress component
        When I port it to a Blazor component
        #UAC09
        Then it should render a progress track and indicator
        #UAC10
        And it should support a Value parameter from 0 to 100
        #UAC11
        And it should support setting aria-valuenow and aria-valuemax

    @SC04
    Scenario: Implement Spinner component
        Given I have the shadcn/ui Spinner component
        When I port it to a Blazor component
        #UAC12
        Then it should render a loading spinner element
        #UAC13
        And it should support size variants
        #UAC14
        And it should merge the Class parameter with generated classes

    @SC05
    Scenario: Implement Empty component
        Given I have the shadcn/ui Empty component
        When I port it to a Blazor component
        #UAC15
        Then it should render a placeholder state container
        #UAC16
        And it should support title and description slots
        #UAC17
        And it should support an optional action slot

    @SC06
    Scenario: Implement Item component
        Given I have the shadcn/ui Item component
        When I port it to a Blazor component
        #UAC18
        Then it should render a list item container with consistent spacing
        #UAC19
        And it should support leading and trailing content slots
        #UAC20
        And it should merge the Class parameter with generated classes

    @SC07
    Scenario: Implement Kbd component
        Given I have the shadcn/ui Kbd component
        When I port it to a Blazor component
        #UAC21
        Then it should render a <kbd> element with class "kbd"
        #UAC22
        And it should support inline usage within text
        #UAC23
        And it should merge the Class parameter with generated classes

    @SC08
    Scenario: Implement Direction utility
        Given I have the shadcn/ui Direction utility component
        When I port it to Blazor
        #UAC24
        Then it should support setting text direction (ltr or rtl)
        #UAC25
        And it should propagate direction to child components
        #UAC26
        And it should support a default direction when not specified
