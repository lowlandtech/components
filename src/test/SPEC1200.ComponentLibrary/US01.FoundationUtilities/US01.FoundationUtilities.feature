@US01
Feature: US01 - Foundation Utilities
    As a Blazor developer
    I want core utility classes and services ported from shadcn/ui
    So that I can build components with consistent styling and behavior

    Background:
        Given I have a Blazor Razor Class Library project at "src/components"
        And Tailwind CSS v4 is configured
        And the project references bUnit for testing

    @SC01
    Scenario: Implement CssClassUtils.Merge (cn function port)
        Given I have the React cn() function from shadcn/ui
        When I port it to C# as CssClassUtils.Merge
        #UAC01
        Then it should accept multiple string inputs
        #UAC02
        And it should accept null values and ignore them
        #UAC03
        And it should merge class names into a single string
        #UAC04
        And it should handle Tailwind CSS class conflicts (later classes override earlier)
        #UAC05
        And later "p-4" should override earlier "p-2"
        #UAC06
        And it should handle conditional classes
        #UAC07
        And it should remove duplicate classes

    @SC02
    Scenario: Test CssClassUtils.Merge with various inputs
        When I call CssClassUtils.Merge("base", "extra")
        #UAC08
        Then it should return "base extra"
        When I call CssClassUtils.Merge("p-2", "p-4")
        #UAC09
        Then it should return "p-4" (later override)
        When I call CssClassUtils.Merge("base", null, "extra")
        #UAC10
        Then it should return "base extra" (null ignored)
        When I call CssClassUtils.Merge("text-red-500", "text-blue-500")
        #UAC11
        Then it should return "text-blue-500" (conflicting color utilities)

    @SC03
    Scenario: Implement VariantBuilder (CVA port)
        Given I have the class-variance-authority pattern from React
        When I port it to C# as VariantBuilder
        #UAC12
        Then it should accept base classes as a string
        #UAC13
        And it should accept a dictionary of variant definitions
        #UAC14
        And it should accept default variant values
        #UAC15
        And it should have a Build method that accepts selected variants
        #UAC16
        And Build should combine base classes with selected variant classes
        #UAC17
        And Build should use default variants when not specified
        #UAC18
        And Build should accept additional classes parameter

    @SC04
    Scenario: Test VariantBuilder with Button variants
        Given a VariantBuilder configured with:
            | Base Classes | inline-flex items-center justify-center |
            | Variant: variant.default | bg-primary text-white |
            | Variant: variant.outline | border border-input |
            | Variant: size.default | h-10 px-4 |
            | Variant: size.sm | h-9 px-3 |
            | Default: variant | default |
            | Default: size | default |
        When I call Build with variant="outline" and size="sm"
        #UAC19
        Then it should return classes including "inline-flex items-center justify-center border border-input h-9 px-3"
        When I call Build with no variants specified
        #UAC20
        Then it should return default variant classes
        When I call Build with additional classes "custom-class"
        #UAC21
        Then it should append "custom-class" to the result

    @SC05
    Scenario: Implement Portal component for overlay rendering
        Given I need to render overlays outside the DOM hierarchy
        When I create a Portal Blazor component
        #UAC22
        Then it should accept ChildContent RenderFragment parameter
        #UAC23
        And it should inject IJSRuntime
        #UAC24
        And it should render content to document.body via JSInterop
        #UAC25
        And it should create a unique portal container div
        #UAC26
        And it should cleanup the container on dispose
        #UAC27
        And it should handle server-side rendering gracefully

    @SC06
    Scenario: Test Portal component with bUnit
        Given I have a Portal component
        When I render it with test content in bUnit
        #UAC28
        Then the component should render without errors
        #UAC29
        And it should invoke JS "portalUtils.create" on first render
        #UAC30
        And it should pass the portal ID and content element reference
        #UAC31
        And the portal container should be created in the body (verify via JS mock)

    @SC07
    Scenario: Implement ThemeService for dark-only mode
        Given dark theme is the only supported theme
        When I create a ThemeService class
        #UAC32
        Then it should expose the current theme as "dark" only
        #UAC33
        And it should have an OnThemeChanged event
        #UAC34
        And applying the theme should trigger OnThemeChanged
        #UAC35
        And it should persist the dark theme to localStorage via JSInterop
        #UAC36
        And it should load the persisted theme on init, honoring only "dark"
        #UAC37
        And it should reject or ignore any non-dark theme value

    @SC08
    Scenario: Test ThemeService with bUnit
        Given I have a ThemeService registered in DI
        When I apply the dark theme
        #UAC38
        Then OnThemeChanged event should fire
        #UAC39
        And the Theme property should return "dark"
        #UAC40
        And it should invoke JS "themeUtils.setTheme" with "dark"
        When a component subscribes to OnThemeChanged
        #UAC41
        Then the component should re-render when theme changes

    @SC09
    Scenario: Create ComponentBase extensions
        Given components need common functionality
        When I create extension methods for ComponentBase
        #UAC42
        Then I should have MergeAttributes helper for combining attribute dictionaries
        #UAC43
        And I should have SetDataAttribute helper for data-* attributes
        #UAC44
        And I should have BuildClassString helper for class attribute construction

    @SC10
    Scenario: Implement focus trap utility
        Given overlay components need focus trapping
        When I create focus trap JSInterop utilities
        #UAC45
        Then it should trap focus within a container element
        #UAC46
        And it should move focus to first focusable element on activate
        #UAC47
        And Tab should cycle through focusable elements
        #UAC48
        And Shift+Tab should cycle backwards
        #UAC49
        And Escape should release focus trap
        #UAC50
        And it should restore focus to trigger element on release

    @SC11
    Scenario: Implement positioning utilities
        Given components like Popover need positioning
        When I create positioning JSInterop utilities
        #UAC51
        Then it should calculate optimal position for floating element
        #UAC52
        And it should support placement options (top, bottom, left, right)
        #UAC53
        And it should handle auto-positioning to avoid viewport edges
        #UAC54
        And it should return position coordinates (x, y)
        #UAC55
        And it should handle scroll events and update position
        #UAC56
        And it should cleanup event listeners on dispose
