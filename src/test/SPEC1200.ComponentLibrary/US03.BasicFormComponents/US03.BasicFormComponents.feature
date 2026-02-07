@US03
Feature: US03 - Basic Form Components
    As a Blazor developer
    I want basic form components in Blazor
    So that I can collect user input consistently

    Background:
        Given I have a Blazor Razor Class Library project at "src/components"
        And Tailwind CSS v4 is configured
        And component classes are defined in wwwroot/css/input.css

    @SC01
    Scenario: Implement Checkbox component
        Given I have the shadcn/ui Checkbox component
        When I port it to a Blazor component
        #UAC01
        Then it should render an <input type="checkbox"> element by default
        #UAC02
        And it should apply the base "checkbox" class
        #UAC03
        And it should support a Checked parameter with two-way binding
        #UAC04
        And it should support an indeterminate state
        #UAC05
        And it should support a Disabled state

    @SC02
    Scenario: Implement Input component
        Given I have the shadcn/ui Input component
        When I port it to a Blazor component
        #UAC06
        Then it should render an <input> element with class "input"
        #UAC07
        And it should support a Type parameter (text, email, password, number, search)
        #UAC08
        And it should support a Placeholder parameter
        #UAC09
        And it should forward unmatched attributes to the underlying element

    @SC03
    Scenario: Implement Textarea component
        Given I have a shadcn/ui Textarea component
        When I port it to a Blazor component
        #UAC10
        Then it should render a <textarea> element with class "input"
        #UAC11
        And it should support Rows and Placeholder parameters
        #UAC12
        And it should support Disabled and ReadOnly states

    @SC04
    Scenario: Implement Label component
        Given I have a shadcn/ui Label component
        When I port it to a Blazor component
        #UAC13
        Then it should render a <label> element with proper classes
        #UAC14
        And it should merge the Class parameter with generated classes
        #UAC15
        And it should forward unmatched attributes to the underlying element
        #UAC16
        And it should render child content
