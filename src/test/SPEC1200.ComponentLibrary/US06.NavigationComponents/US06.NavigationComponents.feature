@US06
Feature: US06 - Navigation Components
    As a Blazor developer
    I want navigation components in Blazor
    So that users can switch views and content sections

    Background:
        Given I have a Blazor Razor Class Library project at "src/components"
        And Tailwind CSS v4 is configured
        And component classes are defined in wwwroot/css/input.css

    @SC01
    Scenario: Implement Tabs component
        Given I have the shadcn/ui Tabs component
        When I port it to Blazor
        #UAC01
        Then I should have Tabs, TabsList, TabsTrigger, and TabsContent components
        #UAC02
        And it should support a default active tab
        #UAC03
        And it should support a controlled active tab via Value and ValueChanged
        #UAC04
        And it should render only the active tab content
        #UAC05
        And it should use appropriate ARIA roles (tablist, tab, tabpanel)

    @SC02
    Scenario: Implement Accordion component
        Given I have the shadcn/ui Accordion component
        When I port it to Blazor
        #UAC06
        Then I should have Accordion, AccordionItem, AccordionTrigger, and AccordionContent components
        #UAC07
        And it should support single and multiple expansion modes
        #UAC08
        And it should support default open items
        #UAC09
        And it should use appropriate ARIA attributes for disclosure

    @SC03
    Scenario: Implement Breadcrumb component
        Given I have the shadcn/ui Breadcrumb component
        When I port it to Blazor
        #UAC10
        Then I should have Breadcrumb, BreadcrumbList, BreadcrumbItem, BreadcrumbLink, and BreadcrumbSeparator components
        #UAC11
        And it should render semantic navigation markup
        #UAC12
        And it should support custom separators
        #UAC13
        And it should merge the Class parameter with generated classes

    @SC04
    Scenario: Implement Pagination component
        Given I have the shadcn/ui Pagination component
        When I port it to Blazor
        #UAC14
        Then I should have Pagination, PaginationContent, PaginationItem, PaginationLink, PaginationPrevious, and PaginationNext components
        #UAC15
        And it should support active and disabled states for links
        #UAC16
        And it should support a custom page size and current page
        #UAC17
        And it should render semantic navigation markup

    @SC05
    Scenario: Implement NavigationMenu component
        Given I have the shadcn/ui NavigationMenu component
        When I port it to Blazor
        #UAC18
        Then I should have NavigationMenu, NavigationMenuList, NavigationMenuItem, NavigationMenuTrigger, and NavigationMenuContent components
        #UAC19
        And it should support keyboard navigation between items
        #UAC20
        And it should support open and close animations
        #UAC21
        And it should manage focus when menus open
        #UAC22
        And it should support nested content groups

    @SC06
    Scenario: Implement Menubar component
        Given I have the shadcn/ui Menubar component
        When I port it to Blazor
        #UAC23
        Then I should have Menubar, MenubarMenu, MenubarTrigger, and MenubarContent components
        #UAC24
        And it should support keyboard navigation between menus
        #UAC25
        And it should close on Escape and outside click
        #UAC26
        And it should support disabled menu items

    @SC07
    Scenario: Implement Sidebar component
        Given I have the shadcn/ui Sidebar component
        When I port it to Blazor
        #UAC27
        Then it should render a sidebar container with class \"sidebar\"
        #UAC28
        And it should support a collapsible layout using the peer-checked pattern
        #UAC29
        And it should support sidebar link styles via \"nav-link\" classes
        #UAC30
        And it should support hiding sidebar text when collapsed
        #UAC31
        And it should merge the Class parameter with generated classes

    @SC08
    Scenario: Implement Collapsible component
        Given I have the shadcn/ui Collapsible component
        When I port it to Blazor
        #UAC32
        Then I should have Collapsible, CollapsibleTrigger, and CollapsibleContent components
        #UAC33
        And it should support controlled open state via parameters
        #UAC34
        And it should support a default open state
        #UAC35
        And it should support keyboard activation
