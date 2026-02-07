@US05
Feature: US05 - Overlay Components
    As a Blazor developer
    I want overlay components in Blazor
    So that I can present layered UI safely and accessibly

    Background:
        Given I have a Blazor Razor Class Library project at "src/components"
        And Tailwind CSS v4 is configured
        And component classes are defined in wwwroot/css/input.css

    @SC01
    Scenario: Implement Dialog component
        Given I have the shadcn/ui Dialog component
        When I port it to Blazor
        #UAC01
        Then I should have Dialog, DialogTrigger, DialogContent, DialogHeader, DialogTitle, DialogDescription, and DialogFooter components
        #UAC02
        And dialog content should render via Portal
        #UAC03
        And it should render an overlay/backdrop behind the dialog content
        #UAC04
        And pressing Escape should close the dialog when it is open
        #UAC05
        And focus should be trapped within the dialog when open
        #UAC06
        And focus should return to the trigger when the dialog closes

    @SC02
    Scenario: Implement DropdownMenu component
        Given I have the shadcn/ui DropdownMenu component
        When I port it to Blazor
        #UAC07
        Then I should have DropdownMenu, DropdownMenuTrigger, DropdownMenuContent, and DropdownMenuItem components
        #UAC08
        And dropdown content should render via Portal
        #UAC09
        And it should support keyboard navigation within the menu
        #UAC10
        And it should close on item selection and outside click
        #UAC11
        And it should support disabled menu items

    @SC03
    Scenario: Implement Tooltip component
        Given I have the shadcn/ui Tooltip component
        When I port it to Blazor
        #UAC12
        Then I should have TooltipProvider, Tooltip, TooltipTrigger, and TooltipContent components
        #UAC13
        And it should support configurable open and close delays
        #UAC14
        And tooltip content should render via Portal
        #UAC15
        And it should position relative to the trigger element

    @SC04
    Scenario: Implement AlertDialog component
        Given I have the shadcn/ui AlertDialog component
        When I port it to Blazor
        #UAC16
        Then I should have AlertDialog, AlertDialogTrigger, AlertDialogContent, AlertDialogTitle, AlertDialogDescription, and AlertDialogFooter components
        #UAC17
        And alert dialog content should render via Portal
        #UAC18
        And it should require explicit confirmation actions
        #UAC19
        And pressing Escape should close the alert dialog when allowed
        #UAC20
        And focus should be trapped within the alert dialog when open

    @SC05
    Scenario: Implement Popover component
        Given I have the shadcn/ui Popover component
        When I port it to Blazor
        #UAC21
        Then I should have Popover, PopoverTrigger, and PopoverContent components
        #UAC22
        And popover content should render via Portal
        #UAC23
        And it should position relative to the trigger element
        #UAC24
        And it should close on outside click
        #UAC25
        And it should support controlled open state via parameters

    @SC06
    Scenario: Implement Sheet component
        Given I have the shadcn/ui Sheet component
        When I port it to Blazor
        #UAC26
        Then I should have Sheet, SheetTrigger, and SheetContent components
        #UAC27
        And sheet content should render via Portal
        #UAC28
        And it should support side placement (top, right, bottom, left)
        #UAC29
        And it should close on outside click and Escape
        #UAC30
        And focus should be trapped within the sheet when open

    @SC07
    Scenario: Implement Drawer component
        Given I have the shadcn/ui Drawer component
        When I port it to Blazor
        #UAC31
        Then I should have Drawer, DrawerTrigger, and DrawerContent components
        #UAC32
        And drawer content should render via Portal
        #UAC33
        And it should support bottom or side placement
        #UAC34
        And it should close on outside click and Escape
        #UAC35
        And focus should be trapped within the drawer when open

    @SC08
    Scenario: Implement HoverCard component
        Given I have the shadcn/ui HoverCard component
        When I port it to Blazor
        #UAC36
        Then I should have HoverCard, HoverCardTrigger, and HoverCardContent components
        #UAC37
        And hover card content should render via Portal
        #UAC38
        And it should open on hover and close on mouse leave
        #UAC39
        And it should position relative to the trigger element

    @SC09
    Scenario: Implement ContextMenu component
        Given I have the shadcn/ui ContextMenu component
        When I port it to Blazor
        #UAC40
        Then I should have ContextMenu, ContextMenuTrigger, ContextMenuContent, and ContextMenuItem components
        #UAC41
        And context menu content should render via Portal
        #UAC42
        And it should open on right-click or context menu key
        #UAC43
        And it should close on outside click and Escape
        #UAC44
        And it should support disabled menu items

    @SC10
    Scenario: Implement Sonner (toast) component
        Given I have the shadcn/ui Sonner component
        When I port it to Blazor
        #UAC45
        Then I should have a Toaster root component and toast API helpers
        #UAC46
        And toasts should render via Portal
        #UAC47
        And it should support variants (default, success, error)
        #UAC48
        And it should support auto-dismiss with configurable duration
