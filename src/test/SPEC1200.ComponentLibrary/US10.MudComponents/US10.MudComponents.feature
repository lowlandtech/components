@US10
Feature: US10 - MudBlazor-Inspired Components
    As a Blazor developer
    I want MudBlazor-style utility components in Tailwind/chadcn style
    So that I can reuse layout and interaction patterns consistently

    Background:
        Given I have a Blazor Razor Class Library project at "src/components"
        And Tailwind CSS v4 is configured
        And component classes are defined in wwwroot/css/input.css

    @SC01
    Scenario: Implement Stack component
        Given I have the MudBlazor Stack component
        When I port it to a Blazor component
        #UAC01
        Then it should render a <div> element with class "stack"
        #UAC02
        And it should support direction via "stack-row" or "stack-col"
        #UAC03
        And it should support gap spacing via a "stack-gap-*" class
        #UAC04
        And it should merge the Class parameter with generated classes
        #UAC05
        And it should forward unmatched attributes to the underlying element

    @SC02
    Scenario: Implement Spacer component
        Given I have the MudBlazor Spacer component
        When I port it to a Blazor component
        #UAC06
        Then it should render a <div> element with class "spacer"
        #UAC07
        And it should merge the Class parameter with generated classes
        #UAC08
        And it should forward unmatched attributes to the underlying element

    @SC03
    Scenario: Implement Timeline components
        Given I have the MudBlazor Timeline components
        When I port them to Blazor
        #UAC09
        Then I should have Timeline and TimelineItem components
        #UAC10
        And Timeline should render a wrapper with class "timeline"
        #UAC11
        And TimelineItem should render a wrapper with class "timeline-item"
        #UAC12
        And Timeline should support orientation classes "timeline-vertical" or "timeline-horizontal"
        #UAC13
        And TimelineItem should support alignment classes "timeline-start", "timeline-center", "timeline-end"

    @SC04
    Scenario: Implement SwipeArea component
        Given I have the MudBlazor SwipeArea component
        When I port it to a Blazor component
        #UAC14
        Then it should render a wrapper element with class "swipe-area"
        #UAC15
        And it should fire OnSwipeMove with direction data on pointer move
        #UAC16
        And it should fire OnSwipeEnd when movement exceeds Sensitivity
        #UAC17
        And it should honor PreventDefault by invoking the required JS interop

    @SC05
    Scenario: Implement Skeleton component (MudBlazor style)
        Given I have the MudBlazor Skeleton component
        When I port it to a Blazor component
        #UAC18
        Then it should render a <div> element with class "skeleton"
        #UAC19
        And it should support width and height parameters
        #UAC20
        And it should support variant classes "skeleton-text", "skeleton-rect", "skeleton-circle"
        #UAC21
        And it should support animation classes "skeleton-pulse", "skeleton-wave", or "skeleton-none"

    @SC06
    Scenario: Implement Stepper components
        Given I have the MudBlazor Stepper components
        When I port them to Blazor
        #UAC22
        Then I should have Stepper and Step components
        #UAC23
        And Stepper should render a wrapper with class "stepper"
        #UAC24
        And Step should render a wrapper with class "step"
        #UAC25
        And the active step should include class "step-active"
        #UAC26
        And completed and disabled steps should include "step-complete" and "step-disabled"
