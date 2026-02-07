@US04
Feature: US04 - Advanced Form Components
    As a Blazor developer
    I want advanced form controls in Blazor
    So that I can collect rich input consistently

    Background:
        Given I have a Blazor Razor Class Library project at "src/components"
        And Tailwind CSS v4 is configured
        And component classes are defined in wwwroot/css/input.css

    @SC01
    Scenario: Implement InputGroup component
        Given I have the shadcn/ui InputGroup component
        When I port it to a Blazor component
        #UAC01
        Then it should render a wrapper with class "input-group"
        #UAC02
        And it should support prefix and suffix content slots
        #UAC03
        And it should merge the Class parameter with generated classes

    @SC02
    Scenario: Implement InputOtp component
        Given I have the shadcn/ui InputOtp component
        When I port it to a Blazor component
        #UAC04
        Then it should render a grouped set of input slots
        #UAC05
        And it should support a Length parameter for the number of slots
        #UAC06
        And it should expose a Value and ValueChanged for binding

    @SC03
    Scenario: Implement Select component
        Given I have the shadcn/ui Select component
        When I port it to Blazor
        #UAC07
        Then I should have Select, SelectTrigger, SelectContent, and SelectItem components
        #UAC08
        And select content should render via Portal
        #UAC09
        And it should support keyboard navigation and selection
        #UAC10
        And it should support disabled items

    @SC04
    Scenario: Implement Combobox component
        Given I have the shadcn/ui Combobox component
        When I port it to Blazor
        #UAC11
        Then it should combine an input with a selectable list
        #UAC12
        And it should filter items based on user input
        #UAC13
        And it should support an empty state when no matches are found
        #UAC14
        And it should expose a Value and ValueChanged for binding

    @SC05
    Scenario: Implement RadioGroup component
        Given I have the shadcn/ui RadioGroup component
        When I port it to Blazor
        #UAC15
        Then I should have RadioGroup and RadioGroupItem components
        #UAC16
        And it should support a default selected value
        #UAC17
        And it should support a controlled value via Value and ValueChanged
        #UAC18
        And it should support a Disabled state for items

    @SC06
    Scenario: Implement Switch component
        Given I have the shadcn/ui Switch component
        When I port it to a Blazor component
        #UAC19
        Then it should render a toggle control with class "switch"
        #UAC20
        And it should support Checked and CheckedChanged for binding
        #UAC21
        And it should support a Disabled state

    @SC07
    Scenario: Implement Slider component
        Given I have the shadcn/ui Slider component
        When I port it to Blazor
        #UAC22
        Then it should render a track and thumb
        #UAC23
        And it should support Min, Max, and Step parameters
        #UAC24
        And it should support Value and ValueChanged for binding

    @SC08
    Scenario: Implement Calendar component
        Given I have the shadcn/ui Calendar component
        When I port it to Blazor
        #UAC25
        Then it should render a monthly calendar grid
        #UAC26
        And it should support single date selection
        #UAC27
        And it should support disabled dates

    @SC09
    Scenario: Implement NativeSelect component
        Given I have the shadcn/ui NativeSelect component
        When I port it to a Blazor component
        #UAC28
        Then it should render a <select> element with class "input"
        #UAC29
        And it should forward unmatched attributes to the underlying element
        #UAC30
        And it should support a Disabled state

    @SC10
    Scenario: Implement Field component
        Given I have the shadcn/ui Field component
        When I port it to a Blazor component
        #UAC31
        Then it should provide a wrapper for label, control, and description
        #UAC32
        And it should support validation and error message slots
        #UAC33
        And it should merge the Class parameter with generated classes

    @SC11
    Scenario: Implement Toggle component
        Given I have the shadcn/ui Toggle component
        When I port it to a Blazor component
        #UAC34
        Then it should render a button-like toggle control
        #UAC35
        And it should support a pressed state via Value and ValueChanged
        #UAC36
        And it should support a Disabled state

    @SC12
    Scenario: Implement ToggleGroup component
        Given I have the shadcn/ui ToggleGroup component
        When I port it to Blazor
        #UAC37
        Then I should have ToggleGroup and ToggleGroupItem components
        #UAC38
        And it should support single and multiple selection modes
        #UAC39
        And it should support a default selected value
        #UAC40
        And it should support a controlled value via Value and ValueChanged
