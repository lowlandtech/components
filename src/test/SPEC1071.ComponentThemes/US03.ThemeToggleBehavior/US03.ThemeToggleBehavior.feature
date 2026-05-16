@US:03
Feature: SPEC1071.US03 - Theme Toggle Behavior
  As a user of themed component previews
  I want ThemeToggle to cycle a local provider when inside ThemeProvider and toggle the document theme when outside
  So that local theme experimentation does not interfere with the global app theme.

  Background:
    Given the component library LowlandTech.Components.ThemeToggle
    And the browser has JavaScript interop available

  # STATE
  # interface ThemeToggleState {
  #   hasProviderContext: boolean
  #   providerId: string | null
  #   ariaLabel: string
  #   cssClass: string
  #   iconSize: number
  #   jsFunction: string
  #   renderedIconMode: 'cycle' | 'sunMoon'
  #   clickCount: number
  # }
  #
  # FORM
  # interface ThemeToggleForm {
  #   ariaLabel: string
  #   cssClass: string
  #   iconSize: number
  #   jsFunction: string
  # }
  #
  # REPRESENTATION
  # interface ThemeToggleRepresentation {
  #   element: 'button'
  #   type: 'button'
  #   ariaLabel: string
  #   title: string
  #   iconMode: 'cycle' | 'sunMoon'
  # }
  #
  # ACTIONS
  # type ThemeToggleAction =
  #   | { type: 'RenderToggle'; hasProviderContext: boolean }
  #   | { type: 'ClickToggle'; providerId: string | null }
  #   | { type: 'InvokeGlobalToggle'; jsFunction: string }
  #   | { type: 'AdvanceProviderTheme'; providerId: string }
  #
  # REDUCER TRANSITIONS
  # RenderToggle -> renderedIconMode: cycle when provider exists, sunMoon otherwise
  # ClickToggle -> clickCount + 1
  # InvokeGlobalToggle -> calls configured JS function
  # AdvanceProviderTheme -> ComponentThemeService.Next(providerId)
  #
  # EFFECTS
  # on ClickToggle with provider -> ComponentThemeService.Next(providerId)
  # on ClickToggle without provider -> IJSRuntime.InvokeVoidAsync(jsFunction)
  #
  # EVENTS
  # type ThemeToggleEvent = 'ThemeToggleRendered' | 'ProviderThemeCycleRequested' | 'GlobalThemeToggleRequested'

  @state:ThemeToggleState
  @SC:01
  Scenario: SC01 - Toggle renders as an accessible button
    @precondition:ToggleParametersDefault
    Given ThemeToggleForm.ariaLabel is "Toggle theme"
    @precondition:NoProviderContext
    And ThemeToggleState.hasProviderContext is false

    @action:RenderToggle
    When ThemeToggle renders
    @param:hasProviderContext
    And hasProviderContext is false

    @postcondition:AccessibleButtonRendered
    Then the root element should be a "button" @UAC:01
    And the button type should be "button" @UAC:02
    And the button aria-label should be "Toggle theme" @UAC:03
    And the button title should be "Toggle theme" @UAC:04
    @postevent:ThemeToggleRendered
    And ThemeToggleRendered should include icon mode "sunMoon" @UAC:05

  @state:ThemeToggleState
  @SC:02
  Scenario: SC02 - Toggle renders custom label, class, and icon size
    @precondition:CustomParametersProvided
    Given ThemeToggleForm.ariaLabel is "Cycle panel theme"
    @precondition:CustomClassProvided
    And ThemeToggleForm.cssClass is "toolbar-theme-button"
    @precondition:CustomIconSizeProvided
    And ThemeToggleForm.iconSize is 16

    @action:RenderToggle
    When ThemeToggle renders with custom parameters
    @param:hasProviderContext
    And hasProviderContext is true

    @postcondition:CustomParametersRendered
    Then the button should have aria-label="Cycle panel theme" @UAC:06
    And the button should have title="Cycle panel theme" @UAC:07
    And the button should have class "toolbar-theme-button" @UAC:08
    And the rendered icon should use size 16 @UAC:09

  @state:ThemeToggleState
  @SC:03
  Scenario: SC03 - Toggle inside a provider renders the cycle icon
    @precondition:ProviderContextAvailable
    Given ThemeToggleState.hasProviderContext is true
    @precondition:ProviderIdKnown
    And ThemeToggleState.providerId is "tp-a"

    @action:RenderToggle
    When ThemeToggle renders inside ThemeProvider
    @param:hasProviderContext
    And hasProviderContext is true

    @postcondition:CycleModeRendered
    Then ThemeToggle should render a refresh-cycle icon @UAC:10
    And ThemeToggle should not render theme-icon-sun @UAC:11
    And ThemeToggle should not render theme-icon-moon @UAC:12
    @postevent:ThemeToggleRendered
    And ThemeToggleRendered should include providerId "tp-a" @UAC:13

  @state:ThemeToggleState
  @SC:04
  Scenario: SC04 - Toggle outside a provider renders global sun and moon icons
    @precondition:NoProviderContext
    Given ThemeToggleState.hasProviderContext is false

    @action:RenderToggle
    When ThemeToggle renders outside ThemeProvider
    @param:hasProviderContext
    And hasProviderContext is false

    @postcondition:GlobalIconModeRendered
    Then ThemeToggle should render an element with class "theme-icon-sun" @UAC:14
    And ThemeToggle should render an element with class "theme-icon-moon" @UAC:15
    And ThemeToggle should not render the refresh-cycle icon as the primary mode @UAC:16
    @postevent:ThemeToggleRendered
    And ThemeToggleRendered should include icon mode "sunMoon" @UAC:17

  @state:ThemeToggleState
  @SC:05
  Scenario: SC05 - Clicking inside a provider cycles only that provider
    @precondition:ProviderContextAvailable
    Given ThemeToggleState.providerId is "tp-a"
    @precondition:ProviderThemeKnown
    And ComponentThemeService has provider "tp-a" set to "Ocean"

    @action:ClickToggle
    When the user clicks ThemeToggle
    @param:providerId
    And providerId is "tp-a"

    @postcondition:ProviderCycleRequested
    Then ComponentThemeService.Next should be called for "tp-a" @UAC:18
    And no JavaScript theme function should be invoked @UAC:19
    And provider "tp-a" should advance to the next registered theme @UAC:20
    @postevent:ProviderThemeCycleRequested
    And ProviderThemeCycleRequested should include providerId "tp-a" @UAC:21

  @state:ThemeToggleState
  @SC:06
  Scenario: SC06 - Clicking outside a provider invokes the configured JavaScript function
    @precondition:NoProviderContext
    Given ThemeToggleState.hasProviderContext is false
    @precondition:JsFunctionConfigured
    And ThemeToggleForm.jsFunction is "window.toggleTheme"

    @action:ClickToggle
    When the user clicks ThemeToggle
    @param:providerId
    And providerId is null

    @postcondition:GlobalToggleInvoked
    Then IJSRuntime.InvokeVoidAsync should be called with "window.toggleTheme" @UAC:22
    And ComponentThemeService.Next should not be called @UAC:23
    @postevent:GlobalThemeToggleRequested
    And GlobalThemeToggleRequested should include jsFunction "window.toggleTheme" @UAC:24

  @state:ThemeToggleState
  @SC:07
  Scenario: SC07 - Clicking outside a provider honors a custom JavaScript function
    @precondition:NoProviderContext
    Given ThemeToggleState.hasProviderContext is false
    @precondition:CustomJsFunctionConfigured
    And ThemeToggleForm.jsFunction is "appTheme.toggle"

    @action:InvokeGlobalToggle
    When the user clicks ThemeToggle with a custom JavaScript function
    @param:jsFunction
    And jsFunction is "appTheme.toggle"

    @postcondition:CustomGlobalToggleInvoked
    Then IJSRuntime.InvokeVoidAsync should be called with "appTheme.toggle" @UAC:25
    And no provider theme should be advanced @UAC:26
    @postevent:GlobalThemeToggleRequested
    And GlobalThemeToggleRequested should include jsFunction "appTheme.toggle" @UAC:27

  @state:ThemeToggleState
  @SC:08
  Scenario: SC08 - Repeated provider clicks continue cycling through registered themes
    @precondition:ProviderContextAvailable
    Given ThemeToggleState.providerId is "tp-cycle"
    @precondition:ThemesOrdered
    And ComponentThemeService registered themes are Light, Dark, Ocean, Rose, and Forest

    @action:ClickToggle
    When the user clicks ThemeToggle five times
    @param:providerId
    And providerId is "tp-cycle"

    @postcondition:CycleWrapsPredictably
    Then ComponentThemeService.Next should be called five times for "tp-cycle" @UAC:28
    And provider "tp-cycle" should return to its starting theme after five clicks @UAC:29
    And ThemeChanged should be raised once per click @UAC:30
