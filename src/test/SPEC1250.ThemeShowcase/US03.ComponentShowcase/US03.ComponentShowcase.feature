@US03
Feature: US03 - Component Showcase Coverage
    As a Blazor developer
    I want the theme pages to exercise core components
    So that the component library is demonstrated in realistic layouts

    Background:
        Given I have a Blazor theme project at "src/theme"
        And component classes are defined in "src/lib/wwwroot/css/input.css"

    @SC01
    Scenario: Showcase card components - Basic structure
        Given the dashboard demos are implemented
        When I review card usage across theme pages
        #UAC01
        Then cards should use the card base class from input.css
        #UAC02
        And cards should have consistent padding (p-4, p-5, or p-6)
        #UAC03
        And cards should use semantic background colors (bg-card)
        #UAC04
        And cards should have subtle borders using border semantic colors
        #UAC05
        And cards should support rounded corners (rounded-lg or rounded-xl)

    @SC01.1
    Scenario: Showcase card components - Card variants
        Given the dashboard demos use different card types
        When I examine card variations
        #UAC06
        Then stat cards should display numeric values prominently
        #UAC07
        And callout cards should emphasize important messages
        #UAC08
        And content cards should organize related information
        #UAC09
        And interactive cards should show hover states
        #UAC10
        And card headers should use card-header class when present

    @SC01.2
    Scenario: Showcase card components - Card composition
        Given cards contain child components
        When I review card content structure
        #UAC11
        Then cards should support card-title for headings
        #UAC12
        And cards should support card-description for subtitles
        #UAC13
        And cards should support card-content for main content area
        #UAC14
        And cards should support card-footer for actions
        #UAC15
        And nested cards should maintain proper spacing hierarchy

    @SC02
    Scenario: Showcase button components - Button variants
        Given the dashboard demos are implemented
        When I review button usage across theme pages
        #UAC16
        Then buttons should use the btn base class from input.css
        #UAC17
        And buttons should use explicit variant classes (btn-primary, btn-secondary, btn-mono)
        #UAC18
        And buttons should never use raw Tailwind utilities for styling
        #UAC19
        And destructive actions should use btn-destructive variant
        #UAC20
        And ghost buttons should use btn-ghost for minimal emphasis

    @SC02.1
    Scenario: Showcase button components - Button sizes
        Given buttons appear in various contexts
        When I examine button sizing
        #UAC21
        Then buttons should support btn-sm for compact actions
        #UAC22
        And buttons should support btn-md or default for standard actions
        #UAC23
        And buttons should support btn-lg for prominent actions
        #UAC24
        And icon-only buttons should use btn-icon class
        #UAC25
        And button sizes should be consistent within the same context

    @SC02.2
    Scenario: Showcase button components - Button states
        Given buttons are interactive elements
        When I interact with buttons
        #UAC26
        Then buttons should show hover state styling
        #UAC27
        And buttons should show active/pressed state styling
        #UAC28
        And buttons should support disabled state with reduced opacity
        #UAC29
        And buttons should show focus ring for keyboard navigation
        #UAC30
        And loading buttons should display spinner or loading indicator

    @SC03
    Scenario: Showcase table components - Basic structure
        Given the Teams table appears in demo1 and other demos
        When I review the teams table implementation
        #UAC31
        Then tables should use table component class
        #UAC32
        And table headers should use table-header class
        #UAC33
        And table rows should use table-row class
        #UAC34
        And table cells should use table-cell class with proper padding
        #UAC35
        And tables should have proper border styling using semantic colors

    @SC03.1
    Scenario: Showcase table components - Table features
        Given tables display structured data
        When I examine table functionality
        #UAC36
        Then table headers should indicate sortable columns
        #UAC37
        And table rows should support hover highlighting
        #UAC38
        And table rows should support selection state
        #UAC39
        And tables should display row action buttons or menus
        #UAC40
        And tables should handle empty state with appropriate message

    @SC03.2
    Scenario: Showcase table components - Responsive behavior
        Given tables may have many columns
        When I view tables on different screen sizes
        #UAC41
        Then tables should be horizontally scrollable on mobile
        #UAC42
        And tables should use table-responsive wrapper class
        #UAC43
        And critical columns should remain visible on small screens
        #UAC44
        And table cells should have responsive padding
        #UAC45
        And tables should maintain readability at all breakpoints

    @SC04
    Scenario: Showcase input components - Form inputs
        Given filtering and options sections exist in demos
        When I review input field usage
        #UAC46
        Then text inputs should use input base class from input.css
        #UAC47
        And inputs should use semantic border and background colors
        #UAC48
        And inputs should have consistent height and padding
        #UAC49
        And inputs should support placeholder text styling
        #UAC50
        And inputs should show focus ring using semantic colors

    @SC04.1
    Scenario: Showcase input components - Input states
        Given inputs are interactive form elements
        When I interact with input fields
        #UAC51
        Then inputs should show error state with error border color
        #UAC52
        And inputs should show success/valid state when applicable
        #UAC53
        And inputs should support disabled state with reduced opacity
        #UAC54
        And inputs should display validation messages below the field
        #UAC55
        And inputs should support leading and trailing icons

    @SC04.2
    Scenario: Showcase select and dropdown components
        Given options sections include select controls
        When I review select/dropdown usage
        #UAC56
        Then select controls should use select or dropdown component class
        #UAC57
        And selects should match input styling for consistency
        #UAC58
        And dropdown menus should use menu component classes
        #UAC59
        And dropdown options should show hover highlighting
        #UAC60
        And selected options should be visually indicated

    @SC04.3
    Scenario: Showcase toggle and switch components
        Given demos may include toggle controls
        When I review toggle/switch usage
        #UAC61
        Then toggles should use switch component class if available
        #UAC62
        And toggles should have clear on/off visual states
        #UAC63
        And toggles should use semantic colors for active state
        #UAC64
        And toggles should support disabled state
        #UAC65
        And toggles should be keyboard accessible

    @SC05
    Scenario: Showcase badge components - Basic badges
        Given highlights and stats sections use badges
        When I review badge usage
        #UAC66
        Then badges should use badge base class from input.css
        #UAC67
        And badges should support variant classes (badge-primary, badge-secondary)
        #UAC68
        And badges should support size variants (badge-sm, badge-md, badge-lg)
        #UAC69
        And badges should use semantic colors for status (success, warning, danger)
        #UAC70
        And badges should have appropriate padding and border radius

    @SC05.1
    Scenario: Showcase badge components - Badge contexts
        Given badges appear in various contexts
        When I examine badge usage patterns
        #UAC71
        Then status badges should indicate item state (active, pending, completed)
        #UAC72
        And count badges should display numeric values
        #UAC73
        And label badges should categorize or tag items
        #UAC74
        And notification badges should draw attention to updates
        #UAC75
        And badges should contrast well against their backgrounds

    @SC06
    Scenario: Showcase stat components - Stat cards
        Given channel stats and other metrics are displayed
        When I review stat card implementations
        #UAC76
        Then stat cards should prominently display the numeric value
        #UAC77
        And stat cards should show the stat label or name
        #UAC78
        And stat cards should include trend indicators (up/down arrows)
        #UAC79
        And stat cards should use semantic colors for positive/negative trends
        #UAC80
        And stat cards should display percentage or delta values

    @SC06.1
    Scenario: Showcase stat components - Stat layouts
        Given stats appear in grid layouts
        When I examine stat card arrangements
        #UAC81
        Then stat grids should use responsive column counts
        #UAC82
        And stat cards should have consistent heights in the same row
        #UAC83
        And stat cards should support icons for visual context
        #UAC84
        And stat cards should use card component as container
        #UAC85
        And stat spacing should match overall theme gap patterns

    @SC07
    Scenario: Showcase navigation components - Menu items
        Given sidebar and navigation menus exist
        When I review navigation component usage
        #UAC86
        Then nav menus should use nav or menu component classes
        #UAC87
        And menu items should use nav-item or menu-item class
        #UAC88
        And active menu items should have distinct styling
        #UAC89
        And menu items should support icons with consistent sizing
        #UAC90
        And menu items should show hover states

    @SC07.1
    Scenario: Showcase navigation components - Breadcrumbs
        Given breadcrumbs appear in the header
        When I review breadcrumb implementation
        #UAC91
        Then breadcrumbs should use breadcrumb component class
        #UAC92
        And breadcrumb items should use breadcrumb-item class
        #UAC93
        And breadcrumb separators should use consistent styling
        #UAC94
        And current breadcrumb item should be non-interactive
        #UAC95
        And breadcrumbs should support icons for items

    @SC08
    Scenario: Showcase icon components - Icon usage
        Given icons appear throughout the theme
        When I review icon implementation
        #UAC96
        Then icons should use the keenicons icon set
        #UAC97
        And icons should have consistent sizing classes (icon-sm, icon-md, icon-lg)
        #UAC98
        And icons should use semantic colors for fill or stroke
        #UAC99
        And icons should align properly with adjacent text
        #UAC100
        And icons should be accessible with appropriate aria-labels

    @SC09
    Scenario: Showcase avatar components - User avatars
        Given team member and user lists display avatars
        When I review avatar usage
        #UAC101
        Then avatars should use avatar component class if available
        #UAC102
        And avatars should support size variants (avatar-sm, avatar-md, avatar-lg)
        #UAC103
        And avatars should display images with proper aspect ratio
        #UAC104
        And avatars should support fallback initials when no image
        #UAC105
        And avatars should have circular border radius

    @SC09.1
    Scenario: Showcase avatar components - Avatar groups
        Given team sections show multiple members
        When I review avatar group implementations
        #UAC106
        Then avatar groups should overlap avatars slightly
        #UAC107
        And avatar groups should show "+N" indicator for overflow
        #UAC108
        And avatar groups should use avatar-group wrapper class
        #UAC109
        And avatar groups should maintain consistent z-index stacking
        #UAC110
        And avatar groups should support max visible count limit
