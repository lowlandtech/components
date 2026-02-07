@US07
Feature: US07 - Layout Components
    As a Blazor developer
    I want layout and data display components in Blazor
    So that I can present structured content consistently

    Background:
        Given I have a Blazor Razor Class Library project at "src/components"
        And Tailwind CSS v4 is configured
        And component classes are defined in wwwroot/css/input.css

    @SC01
    Scenario: Implement Table components
        Given I have the shadcn/ui Table component
        When I port it to Blazor
        #UAC01
        Then it should render a wrapper with class "table-wrapper"
        #UAC02
        And it should render a <table> element with class "table"
        #UAC03
        And I should have TableHeader, TableBody, TableRow, TableHead, and TableCell components
        #UAC04
        And table subcomponents should render correct semantic elements (thead, tbody, tr, th, td)
        #UAC05
        And it should merge the Class parameter with generated classes

    @SC02
    Scenario: Implement Resizable component
        Given I have the shadcn/ui Resizable component
        When I port it to Blazor
        #UAC06
        Then I should have ResizablePanelGroup, ResizablePanel, and ResizableHandle components
        #UAC07
        And it should support horizontal and vertical orientations
        #UAC08
        And it should support default panel sizes
        #UAC09
        And it should emit size changes when panels are resized

    @SC03
    Scenario: Implement ScrollArea component
        Given I have the shadcn/ui ScrollArea component
        When I port it to Blazor
        #UAC10
        Then it should render a scrollable viewport container
        #UAC11
        And it should render a custom scrollbar component
        #UAC12
        And it should support horizontal and vertical scrollbars
        #UAC13
        And it should merge the Class parameter with generated classes

    @SC04
    Scenario: Implement AspectRatio component
        Given I have the shadcn/ui AspectRatio component
        When I port it to Blazor
        #UAC14
        Then it should render a wrapper that preserves a fixed aspect ratio
        #UAC15
        And it should support a Ratio parameter
        #UAC16
        And it should merge the Class parameter with generated classes

    @SC05
    Scenario: Implement Carousel component
        Given I have the shadcn/ui Carousel component
        When I port it to Blazor
        #UAC17
        Then I should have Carousel, CarouselContent, CarouselItem, CarouselPrevious, and CarouselNext components
        #UAC18
        And it should support keyboard navigation between items
        #UAC19
        And it should support optional looping behavior
        #UAC20
        And it should merge the Class parameter with generated classes
