@US02
Feature: US02 - Core Components
    As a Blazor developer
    I want foundational shadcn/ui components in Blazor
    So that I can build consistent layouts

    Background:
        Given I have a Blazor Razor Class Library project at "src/components"
        And Tailwind CSS v4 is configured
        And component classes are defined in wwwroot/css/input.css

    @SC01
    Scenario: Implement Button component
        Given I have the shadcn/ui Button component
        When I port it to a Blazor component
        #UAC01
        Then it should render a <button> element by default
        #UAC02
        And it should apply the base "btn" class
        #UAC03
        And it should support variant classes (btn-default, btn-secondary, btn-outline, btn-ghost, btn-link, btn-destructive)
        #UAC04
        And it should support size classes (btn-sm, btn-lg, btn-icon)
        #UAC05
        And it should merge the Class parameter with generated classes
        #UAC06
        And it should forward unmatched attributes to the underlying element

    @SC02
    Scenario: Implement Badge component
        Given I have the shadcn/ui Badge component
        When I port it to a Blazor component
        #UAC07
        Then it should render a <span> element by default
        #UAC08
        And it should apply the base "badge" class
        #UAC09
        And it should support variant classes (badge-default, badge-secondary, badge-destructive, badge-success, badge-outline)
        #UAC10
        And it should merge the Class parameter with generated classes

    @SC03
    Scenario: Implement Card components
        Given I have the shadcn/ui Card components
        When I port them to Blazor
        #UAC11
        Then I should have Card, CardHeader, CardTitle, CardDescription, CardContent, and CardFooter components
        #UAC12
        And Card should render a wrapper with class "card"
        #UAC13
        And Card subcomponents should render wrappers with their respective classes (card-header, card-title, card-description, card-content, card-footer)
        #UAC14
        And Card should merge the Class parameter with generated classes

    @SC04
    Scenario: Implement Separator component
        Given I have the shadcn/ui Separator component
        When I port it to a Blazor component
        #UAC15
        Then it should render a <div> element with class "separator"
        #UAC16
        And it should support orientation (horizontal | vertical) mapped to "separator-horizontal" or "separator-vertical"
        #UAC17
        And it should be aria-hidden by default

    @SC05
    Scenario: Implement Alert components
        Given I have the shadcn/ui Alert components
        When I port them to Blazor
        #UAC18
        Then I should have Alert, AlertTitle, and AlertDescription components
        #UAC19
        And Alert should render a wrapper with class "alert"
        #UAC20
        And AlertTitle and AlertDescription should render wrappers with classes "alert-title" and "alert-description"
        #UAC21
        And Alert should merge the Class parameter with generated classes

    @SC06
    Scenario: Implement Avatar components
        Given I have the shadcn/ui Avatar components
        When I port them to Blazor
        #UAC22
        Then I should have Avatar, AvatarImage, and AvatarFallback components
        #UAC23
        And Avatar should render a wrapper with class "avatar"
        #UAC24
        And AvatarImage should render an <img> element and AvatarFallback should render a fallback container
        #UAC25
        And Avatar should merge the Class parameter with generated classes

    @SC07
    Scenario: Implement Label component
        Given I have the shadcn/ui Label component
        When I port it to a Blazor component
        #UAC26
        Then it should render a <label> element with class "label"
        #UAC27
        And it should support a For parameter for associating inputs
        #UAC28
        And it should merge the Class parameter with generated classes

    @SC08
    Scenario: Implement Skeleton component
        Given I have the shadcn/ui Skeleton component
        When I port it to a Blazor component
        #UAC29
        Then it should render a <div> element with class "skeleton"
        #UAC30
        And it should support size control via class or style parameters
        #UAC31
        And it should merge the Class parameter with generated classes

    @SC09
    Scenario: Implement ButtonGroup components
        Given I have the shadcn/ui ButtonGroup components
        When I port them to Blazor
        #UAC32
        Then I should have ButtonGroup, ButtonGroupText, and ButtonGroupSeparator components
        #UAC33
        And ButtonGroup should render a wrapper with role="group" and base group class
        #UAC34
        And it should support horizontal and vertical orientations
