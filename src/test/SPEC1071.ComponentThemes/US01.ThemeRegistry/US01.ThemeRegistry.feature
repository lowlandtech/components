@US:01
Feature: SPEC1071.US01 - Theme Registry and Token Contract
  As a component library consumer
  I want named component themes to be registered as token bags
  So that ThemeProvider can apply consistent design variables to any component subtree.

  Background:
    Given the component library LowlandTech.Components.Theme
    And the service ComponentThemeService is available through dependency injection

  # STATE
  # interface ComponentThemeRegistryState {
  #   registeredThemes: ComponentThemeRepresentation[]
  #   providerThemes: Record<string, string>
  #   defaultThemeName: string | null
  #   activeProviderId: string | null
  #   error: string | null
  # }
  #
  # FORM
  # interface ComponentThemeRegistrationForm {
  #   providerId: string
  #   initialThemeName: string | null
  #   requestedThemeName: string | null
  # }
  #
  # REPRESENTATION
  # interface ComponentThemeRepresentation {
  #   name: string
  #   tokens: Record<string, string>
  # }
  #
  # ACTIONS
  # type ComponentThemeRegistryAction =
  #   | { type: 'LoadRegisteredThemes'; themes: ComponentThemeRepresentation[] }
  #   | { type: 'RegisterProvider'; providerId: string; initialThemeName: string | null }
  #   | { type: 'GetThemeForProvider'; providerId: string }
  #   | { type: 'SetProviderTheme'; providerId: string; themeName: string }
  #   | { type: 'AdvanceProviderTheme'; providerId: string }
  #   | { type: 'UnregisterProvider'; providerId: string }
  #
  # REDUCER TRANSITIONS
  # LoadRegisteredThemes -> registeredThemes: payload, defaultThemeName: first theme name or null
  # RegisterProvider -> providerThemes[providerId]: initialThemeName or defaultThemeName or ''
  # GetThemeForProvider -> activeProviderId: providerId
  # SetProviderTheme -> providerThemes[providerId]: themeName
  # AdvanceProviderTheme -> providerThemes[providerId]: next registered theme name with wraparound
  # UnregisterProvider -> remove providerThemes[providerId]
  #
  # EFFECTS
  # on SetProviderTheme -> notify ThemeChanged(providerId)
  # on AdvanceProviderTheme -> notify ThemeChanged(providerId)
  #
  # EVENTS
  # type ComponentThemeRegistryEvent = 'ThemesLoaded' | 'ProviderRegistered' | 'ProviderThemeChanged' | 'ProviderUnregistered'

  @state:ComponentThemeRegistryState
  @SC:01
  Scenario: SC01 - Registered themes expose stable names and token dictionaries
    @precondition:RegisteredThemesProvided
    Given ComponentThemeRegistryState.registeredThemes contains Light, Dark, Ocean, Rose, and Forest
    @precondition:ThemesAreStateless
    And each registered theme has no provider-specific state

    @action:LoadRegisteredThemes
    When the component host loads registered IComponentTheme implementations
    @param:themes
    And themes are supplied in dependency injection registration order

    @postcondition:AllThemesAddressable
    Then each theme should expose a non-empty unique Name @UAC:01
    And each theme should expose a non-empty Tokens dictionary @UAC:02
    And every theme token key should begin with "--" @UAC:03
    And every token value should be written verbatim without service-side conversion @UAC:04
    @postevent:ThemesLoaded
    And ThemesLoaded should identify the registration order used for cycling @UAC:05

  @state:ComponentThemeRegistryState
  @SC:02
  Scenario: SC02 - Token contract includes the common shadcn-style semantic variables
    @precondition:ThemeTokenSetExists
    Given a registered component theme has been loaded

    @action:LoadRegisteredThemes
    When the theme token contract is inspected
    @param:themes
    And the inspected theme is one of the registered demo themes

    @postcondition:SemanticTokensPresent
    Then the theme should define "--background" @UAC:06
    And the theme should define "--foreground" @UAC:07
    And the theme should define "--card" and "--card-foreground" @UAC:08
    And the theme should define "--popover" and "--popover-foreground" @UAC:09
    And the theme should define "--primary" and "--primary-foreground" @UAC:10
    And the theme should define "--secondary" and "--secondary-foreground" @UAC:11
    And the theme should define "--muted" and "--muted-foreground" @UAC:12
    And the theme should define "--accent" and "--accent-foreground" @UAC:13
    And the theme should define "--destructive" and "--destructive-foreground" @UAC:14
    And the theme should define "--border", "--input", and "--ring" @UAC:15

  @state:ComponentThemeRegistryState
  @SC:03
  Scenario: SC03 - Provider registration chooses the explicit initial theme when supplied
    @precondition:ProviderNotRegistered
    Given ComponentThemeRegistryState.providerThemes does not contain provider "tp-a"
    @precondition:InitialThemeExists
    And ComponentThemeRegistryState.registeredThemes contains a theme named "Ocean"

    @action:RegisterProvider
    When a provider registers with id "tp-a"
    @param:providerId
    And providerId is "tp-a"
    @param:initialThemeName
    And initialThemeName is "Ocean"

    @postcondition:InitialThemeStored
    Then ComponentThemeRegistryState.providerThemes["tp-a"] should be "Ocean" @UAC:16
    And GetNameForProvider("tp-a") should return "Ocean" @UAC:17
    And GetForProvider("tp-a") should return the Ocean theme object @UAC:18
    @postevent:ProviderRegistered
    And ProviderRegistered should include providerId "tp-a" @UAC:19

  @state:ComponentThemeRegistryState
  @SC:04
  Scenario: SC04 - Provider registration falls back to the first registered theme
    @precondition:ProviderNotRegistered
    Given ComponentThemeRegistryState.providerThemes does not contain provider "tp-default"
    @precondition:RegisteredThemesOrdered
    And ComponentThemeRegistryState.registeredThemes first item is "Light"

    @action:RegisterProvider
    When a provider registers with id "tp-default"
    @param:providerId
    And providerId is "tp-default"
    @param:initialThemeName
    And initialThemeName is null

    @postcondition:DefaultThemeStored
    Then ComponentThemeRegistryState.providerThemes["tp-default"] should be "Light" @UAC:20
    And GetNameForProvider("tp-default") should return "Light" @UAC:21
    And GetForProvider("tp-default") should return the Light theme object @UAC:22

  @state:ComponentThemeRegistryState
  @SC:05
  Scenario: SC05 - Register is idempotent for an existing provider
    @precondition:ProviderAlreadyRegistered
    Given ComponentThemeRegistryState.providerThemes["tp-a"] is "Ocean"
    @precondition:AlternativeThemeExists
    And ComponentThemeRegistryState.registeredThemes contains a theme named "Rose"

    @action:RegisterProvider
    When the same provider registers again with id "tp-a"
    @param:providerId
    And providerId is "tp-a"
    @param:initialThemeName
    And initialThemeName is "Rose"

    @postcondition:ExistingThemePreserved
    Then ComponentThemeRegistryState.providerThemes["tp-a"] should remain "Ocean" @UAC:23
    And GetNameForProvider("tp-a") should still return "Ocean" @UAC:24
    And no ThemeChanged event should be raised by Register @UAC:25

  @state:ComponentThemeRegistryState
  @SC:06
  Scenario: SC06 - Set updates the provider theme and raises a targeted change event
    @precondition:ProviderRegistered
    Given ComponentThemeRegistryState.providerThemes["tp-a"] is "Ocean"
    @precondition:TargetThemeExists
    And ComponentThemeRegistryState.registeredThemes contains a theme named "Rose"

    @action:SetProviderTheme
    When the theme service sets provider "tp-a" to "Rose"
    @param:providerId
    And providerId is "tp-a"
    @param:themeName
    And themeName is "Rose"

    @postcondition:ThemeNameUpdated
    Then ComponentThemeRegistryState.providerThemes["tp-a"] should be "Rose" @UAC:26
    And GetForProvider("tp-a") should return the Rose theme object @UAC:27
    @postevent:ProviderThemeChanged
    And ThemeChanged should be raised exactly once for provider "tp-a" @UAC:28

  @state:ComponentThemeRegistryState
  @SC:07
  Scenario: SC07 - Next advances in registration order and wraps after the last theme
    @precondition:ThemesOrdered
    Given ComponentThemeRegistryState.registeredThemes are Light, Dark, Ocean, Rose, and Forest
    @precondition:ProviderAtLastTheme
    And ComponentThemeRegistryState.providerThemes["tp-cycle"] is "Forest"

    @action:AdvanceProviderTheme
    When the theme service advances provider "tp-cycle"
    @param:providerId
    And providerId is "tp-cycle"

    @postcondition:WrappedToFirstTheme
    Then ComponentThemeRegistryState.providerThemes["tp-cycle"] should be "Light" @UAC:29
    And GetForProvider("tp-cycle") should return the Light theme object @UAC:30
    @postevent:ProviderThemeChanged
    And ThemeChanged should be raised for provider "tp-cycle" @UAC:31

  @state:ComponentThemeRegistryState
  @SC:08
  Scenario: SC08 - Unregister removes provider state without mutating the theme registry
    @precondition:ProviderRegistered
    Given ComponentThemeRegistryState.providerThemes["tp-dispose"] is "Dark"
    @precondition:ThemesLoaded
    And ComponentThemeRegistryState.registeredThemes contains five themes

    @action:UnregisterProvider
    When provider "tp-dispose" is unregistered
    @param:providerId
    And providerId is "tp-dispose"

    @postcondition:ProviderStateRemoved
    Then ComponentThemeRegistryState.providerThemes should not contain "tp-dispose" @UAC:32
    And ComponentThemeRegistryState.registeredThemes should still contain five themes @UAC:33
    @postevent:ProviderUnregistered
    And ProviderUnregistered should include providerId "tp-dispose" @UAC:34
