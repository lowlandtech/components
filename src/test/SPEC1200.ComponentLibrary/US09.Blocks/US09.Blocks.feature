@US09
Feature: US09 - Blocks
    As a Blazor developer
    I want shadcn/ui blocks ported to Blazor
    So that I can use production-ready page sections

    Background:
        Given I have a Blazor Razor Class Library project at "src/components"
        And Tailwind CSS v4 is configured
        And component classes are defined in wwwroot/css/input.css

    @SC01
    Scenario: Port block chatgpt
        Given I have the shadcn/ui block "chatgpt"
        When I port it to a Blazor component set
        #UAC01
        Then it should render the block layout using existing library components

    @SC02
    Scenario: Port block dashboard-01
        Given I have the shadcn/ui block "dashboard-01"
        When I port it to a Blazor component set
        #UAC02
        Then it should render the block layout using existing library components

    @SC03
    Scenario: Port block elevenlabs
        Given I have the shadcn/ui block "elevenlabs"
        When I port it to a Blazor component set
        #UAC03
        Then it should render the block layout using existing library components

    @SC04
    Scenario: Port block github
        Given I have the shadcn/ui block "github"
        When I port it to a Blazor component set
        #UAC04
        Then it should render the block layout using existing library components

    @SC05
    Scenario: Port block login-01
        Given I have the shadcn/ui block "login-01"
        When I port it to a Blazor component set
        #UAC05
        Then it should render the block layout using existing library components

    @SC06
    Scenario: Port block login-02
        Given I have the shadcn/ui block "login-02"
        When I port it to a Blazor component set
        #UAC06
        Then it should render the block layout using existing library components

    @SC07
    Scenario: Port block login-03
        Given I have the shadcn/ui block "login-03"
        When I port it to a Blazor component set
        #UAC07
        Then it should render the block layout using existing library components

    @SC08
    Scenario: Port block login-04
        Given I have the shadcn/ui block "login-04"
        When I port it to a Blazor component set
        #UAC08
        Then it should render the block layout using existing library components

    @SC09
    Scenario: Port block login-05
        Given I have the shadcn/ui block "login-05"
        When I port it to a Blazor component set
        #UAC09
        Then it should render the block layout using existing library components

    @SC10
    Scenario: Port block preview
        Given I have the shadcn/ui block "preview"
        When I port it to a Blazor component set
        #UAC10
        Then it should render the block layout using existing library components

    @SC11
    Scenario: Port block sidebar-01
        Given I have the shadcn/ui block "sidebar-01"
        When I port it to a Blazor component set
        #UAC11
        Then it should render the block layout using existing library components

    @SC12
    Scenario: Port block sidebar-02
        Given I have the shadcn/ui block "sidebar-02"
        When I port it to a Blazor component set
        #UAC12
        Then it should render the block layout using existing library components

    @SC13
    Scenario: Port block sidebar-03
        Given I have the shadcn/ui block "sidebar-03"
        When I port it to a Blazor component set
        #UAC13
        Then it should render the block layout using existing library components

    @SC14
    Scenario: Port block sidebar-04
        Given I have the shadcn/ui block "sidebar-04"
        When I port it to a Blazor component set
        #UAC14
        Then it should render the block layout using existing library components

    @SC15
    Scenario: Port block sidebar-05
        Given I have the shadcn/ui block "sidebar-05"
        When I port it to a Blazor component set
        #UAC15
        Then it should render the block layout using existing library components

    @SC16
    Scenario: Port block sidebar-06
        Given I have the shadcn/ui block "sidebar-06"
        When I port it to a Blazor component set
        #UAC16
        Then it should render the block layout using existing library components

    @SC17
    Scenario: Port block sidebar-07
        Given I have the shadcn/ui block "sidebar-07"
        When I port it to a Blazor component set
        #UAC17
        Then it should render the block layout using existing library components

    @SC18
    Scenario: Port block sidebar-08
        Given I have the shadcn/ui block "sidebar-08"
        When I port it to a Blazor component set
        #UAC18
        Then it should render the block layout using existing library components

    @SC19
    Scenario: Port block sidebar-09
        Given I have the shadcn/ui block "sidebar-09"
        When I port it to a Blazor component set
        #UAC19
        Then it should render the block layout using existing library components

    @SC20
    Scenario: Port block sidebar-10
        Given I have the shadcn/ui block "sidebar-10"
        When I port it to a Blazor component set
        #UAC20
        Then it should render the block layout using existing library components

    @SC21
    Scenario: Port block sidebar-11
        Given I have the shadcn/ui block "sidebar-11"
        When I port it to a Blazor component set
        #UAC21
        Then it should render the block layout using existing library components

    @SC22
    Scenario: Port block sidebar-12
        Given I have the shadcn/ui block "sidebar-12"
        When I port it to a Blazor component set
        #UAC22
        Then it should render the block layout using existing library components

    @SC23
    Scenario: Port block sidebar-13
        Given I have the shadcn/ui block "sidebar-13"
        When I port it to a Blazor component set
        #UAC23
        Then it should render the block layout using existing library components

    @SC24
    Scenario: Port block sidebar-14
        Given I have the shadcn/ui block "sidebar-14"
        When I port it to a Blazor component set
        #UAC24
        Then it should render the block layout using existing library components

    @SC25
    Scenario: Port block sidebar-15
        Given I have the shadcn/ui block "sidebar-15"
        When I port it to a Blazor component set
        #UAC25
        Then it should render the block layout using existing library components

    @SC26
    Scenario: Port block sidebar-16
        Given I have the shadcn/ui block "sidebar-16"
        When I port it to a Blazor component set
        #UAC26
        Then it should render the block layout using existing library components

    @SC27
    Scenario: Port block signup-01
        Given I have the shadcn/ui block "signup-01"
        When I port it to a Blazor component set
        #UAC27
        Then it should render the block layout using existing library components

    @SC28
    Scenario: Port block signup-02
        Given I have the shadcn/ui block "signup-02"
        When I port it to a Blazor component set
        #UAC28
        Then it should render the block layout using existing library components

    @SC29
    Scenario: Port block signup-03
        Given I have the shadcn/ui block "signup-03"
        When I port it to a Blazor component set
        #UAC29
        Then it should render the block layout using existing library components

    @SC30
    Scenario: Port block signup-04
        Given I have the shadcn/ui block "signup-04"
        When I port it to a Blazor component set
        #UAC30
        Then it should render the block layout using existing library components

    @SC31
    Scenario: Port block signup-05
        Given I have the shadcn/ui block "signup-05"
        When I port it to a Blazor component set
        #UAC31
        Then it should render the block layout using existing library components

    @SC32
    Scenario: Port block vercel
        Given I have the shadcn/ui block "vercel"
        When I port it to a Blazor component set
        #UAC32
        Then it should render the block layout using existing library components

