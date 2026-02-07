@US04
Feature: US04 - Theming and Styling Compliance
    As a Blazor developer
    I want the theme port to follow styling rules
    So that the showcase matches the project's dark theme and component system

    Background:
        Given I have a Blazor theme project at "src/theme"
        And component classes are defined in "src/lib/wwwroot/css/input.css"
        And CSS variables are defined in "src/lib/wwwroot/css/variables.css"

    @SC01
    Scenario: Enforce semantic colors - Background colors
        Given the theme pages are implemented
        When I inspect background color usage
        #UAC01
        Then page backgrounds should use bg-background semantic color
        #UAC02
        And card backgrounds should use bg-card semantic color
        #UAC03
        And primary actions should use bg-primary semantic color
        #UAC04
        And secondary actions should use bg-secondary semantic color
        #UAC05
        And no element should use hardcoded color values like #ffffff or rgb()

    @SC01.1
    Scenario: Enforce semantic colors - Text colors
        Given the theme pages are implemented
        When I inspect text color usage
        #UAC06
        Then primary text should use text-foreground semantic color
        #UAC07
        And secondary text should use text-muted-foreground semantic color
        #UAC08
        And headings should use text-foreground or text-primary
        #UAC09
        And links should use text-primary with hover states
        #UAC10
        And no text should use hardcoded color values

    @SC01.2
    Scenario: Enforce semantic colors - Border colors
        Given the theme pages are implemented
        When I inspect border color usage
        #UAC11
        Then default borders should use border or border-border semantic color
        #UAC12
        And input borders should use border-input semantic color
        #UAC13
        And focus rings should use ring-ring semantic color
        #UAC14
        And dividers should use border-border semantic color
        #UAC15
        And no borders should use hardcoded color values

    @SC01.3
    Scenario: Enforce semantic colors - State colors
        Given the theme pages use interactive elements
        When I inspect state-specific colors
        #UAC16
        Then destructive actions should use destructive semantic color
        #UAC17
        And success states should use success semantic color if defined
        #UAC18
        And warning states should use warning semantic color if defined
        #UAC19
        And error states should use error or destructive semantic color
        #UAC20
        And muted elements should use muted semantic color

    @SC01.4
    Scenario: Enforce semantic colors - Accent and highlights
        Given the theme pages highlight important elements
        When I inspect accent color usage
        #UAC21
        Then accented elements should use accent semantic color
        #UAC22
        And hover states should use hover variant of semantic colors
        #UAC23
        And active states should use appropriate semantic color variants
        #UAC24
        And disabled states should use muted semantic colors
        #UAC25
        And all semantic colors should reference CSS variables

    @SC02
    Scenario: Enforce dark-only theme - Color scheme
        Given the theme pages are implemented
        When I load the theme pages in a browser
        #UAC26
        Then the root element should set color-scheme: dark
        #UAC27
        And the background should be dark themed
        #UAC28
        And text should be light colored for dark background contrast
        #UAC29
        And there should be no light theme CSS classes
        #UAC30
        And there should be no light theme toggle or switcher

    @SC02.1
    Scenario: Enforce dark-only theme - No light theme code
        Given the codebase enforces dark-only theming
        When I review theme implementation files
        #UAC31
        Then there should be no light mode CSS variable definitions
        #UAC32
        And there should be no theme switcher UI components
        #UAC33
        And there should be no light/dark conditional styling logic
        #UAC34
        And ThemeService should not support light or system modes
        #UAC35
        And all colors should be optimized for dark backgrounds

    @SC02.2
    Scenario: Enforce dark-only theme - Visual consistency
        Given all pages use dark theme
        When I navigate between different demo pages
        #UAC36
        Then all pages should maintain consistent dark background
        #UAC37
        And all cards should use dark-appropriate backgrounds
        #UAC38
        And all text should maintain proper contrast for accessibility
        #UAC39
        And all borders should be subtle on dark backgrounds
        #UAC40
        And all shadows should work well with dark theme

    @SC03
    Scenario: Enforce spacing scale - Gap and margin utilities
        Given the theme pages are implemented
        When I review layout spacing
        #UAC41
        Then grid gaps should use approved scale (gap-2, gap-4, gap-5, gap-6, gap-7.5)
        #UAC42
        And vertical spacing should use space-y-2, space-y-4, space-y-6, etc.
        #UAC43
        And horizontal spacing should use space-x-2, space-x-4, space-x-6, etc.
        #UAC44
        And margins should use approved scale (m-2, m-4, m-6, etc.)
        #UAC45
        And custom spacing values should be avoided unless documented

    @SC03.1
    Scenario: Enforce spacing scale - Padding utilities
        Given components use padding for internal spacing
        When I review padding usage
        #UAC46
        Then card padding should use approved scale (p-4, p-5, p-6)
        #UAC47
        And button padding should use consistent px and py values
        #UAC48
        And input padding should match button padding for alignment
        #UAC49
        And section padding should use larger scale values (p-6, p-8)
        #UAC50
        And custom padding values should be avoided unless documented

    @SC03.2
    Scenario: Enforce spacing scale - Container and wrapper spacing
        Given layouts use containers and wrappers
        When I review container spacing
        #UAC51
        Then page containers should use consistent horizontal padding
        #UAC52
        And nested containers should reduce padding appropriately
        #UAC53
        And responsive padding should adjust at breakpoints (px-4 md:px-6 lg:px-8)
        #UAC54
        And max-width containers should use approved values
        #UAC55
        And spacing should create clear visual hierarchy

    @SC04
    Scenario: Enforce typography scale - Heading sizes
        Given the theme pages display headings
        When I review heading usage
        #UAC56
        Then h1 headings should use text-3xl or text-4xl classes
        #UAC57
        And h2 headings should use text-2xl or text-3xl classes
        #UAC58
        And h3 headings should use text-xl or text-2xl classes
        #UAC59
        And h4 headings should use text-lg or text-xl classes
        #UAC60
        And heading sizes should be consistent across similar contexts

    @SC04.1
    Scenario: Enforce typography scale - Body text sizes
        Given the theme pages display body text
        When I review text size usage
        #UAC61
        Then default body text should use text-base or text-sm
        #UAC62
        And small text should use text-xs or text-sm
        #UAC63
        And large text should use text-lg
        #UAC64
        And text sizes should be responsive when needed (text-sm md:text-base)
        #UAC65
        And custom text sizes should be avoided unless documented

    @SC04.2
    Scenario: Enforce typography scale - Font weights
        Given text uses different font weights for emphasis
        When I review font weight usage
        #UAC66
        Then headings should use font-semibold or font-bold
        #UAC67
        And body text should use font-normal or font-medium
        #UAC68
        And emphasized text should use font-medium or font-semibold
        #UAC69
        And de-emphasized text should use font-light or font-normal
        #UAC70
        And font weights should be consistent within component types

    @SC04.3
    Scenario: Enforce typography scale - Line heights and letter spacing
        Given text needs proper readability
        When I review line height and letter spacing
        #UAC71
        Then headings should use tight or normal line height
        #UAC72
        And body text should use relaxed or normal line height
        #UAC73
        And dense layouts should use snug line height
        #UAC74
        And letter spacing should be used sparingly for headings
        #UAC75
        And line height should support multi-line readability

    @SC05
    Scenario: Enforce component class usage - No raw Tailwind
        Given input.css defines component classes
        When I review component implementations
        #UAC76
        Then buttons should use btn class, not raw Tailwind button styles
        #UAC77
        And cards should use card class, not raw Tailwind card styles
        #UAC78
        And inputs should use input class, not raw Tailwind input styles
        #UAC79
        And badges should use badge class, not raw Tailwind badge styles
        #UAC80
        And all components should prefer component classes over utility classes

    @SC05.1
    Scenario: Enforce component class usage - Component variants
        Given component classes support variants
        When I use component variants
        #UAC81
        Then variant classes should be combined with base class (btn btn-primary)
        #UAC82
        And size classes should be combined with base class (btn btn-sm)
        #UAC83
        And state classes should be combined when needed (btn btn-primary disabled)
        #UAC84
        And custom variant combinations should not override base styles
        #UAC85
        And variant classes should be documented in input.css

    @SC05.2
    Scenario: Enforce component class usage - Utility class limits
        Given raw Tailwind utilities should be minimal
        When I review utility class usage
        #UAC86
        Then layout utilities (flex, grid, gap) are acceptable
        #UAC87
        And spacing utilities (m, p, space) are acceptable for layout
        #UAC88
        And responsive utilities (hidden, block) are acceptable
        #UAC89
        And styling utilities (bg, text, border colors) should use semantic colors only
        #UAC90
        And complex utility combinations should be extracted to component classes

    @SC06
    Scenario: Enforce border radius and shadows - Border radius
        Given components use rounded corners
        When I review border radius usage
        #UAC91
        Then cards should use rounded-lg or rounded-xl
        #UAC92
        And buttons should use rounded-md or rounded-lg
        #UAC93
        And inputs should use rounded-md
        #UAC94
        And badges should use rounded-full or rounded-md
        #UAC95
        And border radius should be consistent within component types

    @SC06.1
    Scenario: Enforce border radius and shadows - Shadow usage
        Given components may use shadows for depth
        When I review shadow usage
        #UAC96
        Then elevated cards should use shadow-sm or shadow-md
        #UAC97
        And modals/dropdowns should use shadow-lg or shadow-xl
        #UAC98
        And shadows should be subtle on dark backgrounds
        #UAC99
        And shadow colors should work with dark theme
        #UAC100
        And excessive shadow usage should be avoided

    @SC07
    Scenario: Enforce responsive design - Breakpoint usage
        Given the theme should work on all screen sizes
        When I review responsive classes
        #UAC101
        Then mobile-first approach should be used (base class, then md:, lg:)
        #UAC102
        And grids should use responsive columns (grid-cols-1 md:grid-cols-2 lg:grid-cols-3)
        #UAC103
        And typography should scale responsively when needed
        #UAC104
        And spacing should adjust at breakpoints when needed
        #UAC105
        And layouts should stack vertically on mobile, expand on desktop

    @SC07.1
    Scenario: Enforce responsive design - Mobile optimization
        Given mobile users access the theme
        When I view the theme on mobile devices
        #UAC106
        Then sidebar should collapse to hamburger menu on mobile
        #UAC107
        And tables should scroll horizontally on mobile
        #UAC108
        And touch targets should be appropriately sized (min 44x44px)
        #UAC109
        And horizontal overflow should be prevented
        #UAC110
        And mobile navigation should be thumb-friendly

    @SC08
    Scenario: Enforce accessibility - Color contrast
        Given accessibility is required
        When I audit color contrast
        #UAC111
        Then all text should meet WCAG AA contrast ratios (4.5:1 for normal text)
        #UAC112
        And large text should meet WCAG AA contrast ratios (3:1)
        #UAC113
        And interactive elements should have sufficient contrast
        #UAC114
        And focus indicators should have 3:1 contrast with background
        #UAC115
        And semantic colors should be tested for contrast compliance

    @SC08.1
    Scenario: Enforce accessibility - Focus management
        Given keyboard users navigate the theme
        When I tab through interactive elements
        #UAC116
        Then all interactive elements should be keyboard accessible
        #UAC117
        And focus order should be logical and predictable
        #UAC118
        And focus indicators should be clearly visible
        #UAC119
        And skip links should be provided for main content
        #UAC120
        And focus should not be trapped unintentionally
