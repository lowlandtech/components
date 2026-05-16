@US:02
Feature: SPEC1071.US02 - Theme Provider Boundary
  As a component author
  I want ThemeProvider to create a scoped CSS variable boundary
  So that child components can render with a selected theme without changing the whole document.

  Background:
    Given the component library LowlandTech.Components.ThemeProvider
    And ComponentThemeService has registered component themes

  # STATE
  # interface ThemeProviderBoundaryState {
  #   providerId: string
  #   requestedThemeName: string | null
  #   activeThemeName: string | null
  #   inlineStyle: string
  #   childContentRendered: boolean
  #   contextProviderId: string | null
  #   disposed: boolean
  # }
  #
  # FORM
  # interface ThemeProviderBoundaryForm {
  #   id: string
  #   theme: string | null
  #   className: string | null
  #   additionalAttributes: Record<string, string>
  # }
  #
  # REPRESENTATION
  # interface ThemeProviderBoundaryRepresentation {
  #   dataThemeProvider: string
  #   className: string | null
  #   inlineStyle: string
  #   cascadedContext: { providerId: string }
  # }
  #
  # ACTIONS
  # type ThemeProviderBoundaryAction =
  #   | { type: 'InitializeProvider'; id: string; theme: string | null }
  #   | { type: 'RenderProvider'; id: string; className: string | null; additionalAttributes: Record<string, string> }
  #   | { type: 'ApplyThemeTokens'; id: string; themeName: string }
  #   | { type: 'ReceiveThemeChanged'; id: string }
  #   | { type: 'DisposeProvider'; id: string }
  #
  # REDUCER TRANSITIONS
  # InitializeProvider -> providerId, requestedThemeName, activeThemeName from service
  # RenderProvider -> childContentRendered: true, contextProviderId: providerId
  # ApplyThemeTokens -> inlineStyle: semicolon-joined token declarations
  # ReceiveThemeChanged -> activeThemeName and inlineStyle refreshed when id matches
  # DisposeProvider -> disposed: true
  #
  # EFFECTS
  # on InitializeProvider -> ThemeService.Register(id, theme), subscribe ThemeChanged
  # on DisposeProvider -> unsubscribe ThemeChanged, ThemeService.Unregister(id)
  #
  # EVENTS
  # type ThemeProviderBoundaryEvent = 'ThemeProviderInitialized' | 'ThemeProviderRendered' | 'ThemeProviderStyleUpdated' | 'ThemeProviderDisposed'

  @state:ThemeProviderBoundaryState
  @SC:01
  Scenario: SC01 - Provider renders a root boundary with its identity
    @precondition:ProviderParametersSet
    Given ThemeProviderBoundaryForm.id is "settings-panel"
    @precondition:ThemeServiceAvailable
    And ComponentThemeService contains a Light theme

    @action:RenderProvider
    When ThemeProvider renders
    @param:id
    And id is "settings-panel"
    @param:additionalAttributes
    And additionalAttributes are empty

    @postcondition:BoundaryIdentityRendered
    Then the root element should be a "div" @UAC:01
    And the root element should have data-theme-provider="settings-panel" @UAC:02
    And the root element should preserve child content inside the boundary @UAC:03
    @postevent:ThemeProviderRendered
    And ThemeProviderRendered should include providerId "settings-panel" @UAC:04

  @state:ThemeProviderBoundaryState
  @SC:02
  Scenario: SC02 - Provider writes selected theme tokens as inline CSS variables
    @precondition:ProviderInitialized
    Given ThemeProviderBoundaryState.providerId is "preview"
    @precondition:SelectedThemeExists
    And the Ocean theme defines "--background" as "#f0f9ff"

    @action:ApplyThemeTokens
    When ThemeProvider applies tokens for theme "Ocean"
    @param:id
    And id is "preview"
    @param:themeName
    And themeName is "Ocean"

    @postcondition:InlineStyleContainsTokens
    Then the root element style should contain "--background:#f0f9ff" @UAC:05
    And the root element style should contain "--foreground:" @UAC:06
    And the root element style should contain "--primary:" @UAC:07
    And token declarations should be separated by semicolons @UAC:08
    @postevent:ThemeProviderStyleUpdated
    And ThemeProviderStyleUpdated should include providerId "preview" @UAC:09

  @state:ThemeProviderBoundaryState
  @SC:03
  Scenario: SC03 - Provider cascades context to child controls
    @precondition:ProviderInitialized
    Given ThemeProviderBoundaryState.providerId is "toolbar"
    @precondition:ChildContentContainsThemeToggle
    And child content contains a ThemeToggle component

    @action:RenderProvider
    When ThemeProvider renders child content
    @param:id
    And id is "toolbar"
    @param:className
    And className is "theme-panel"

    @postcondition:ContextAvailable
    Then child components should receive ThemeProviderContext @UAC:10
    And ThemeProviderContext.ProviderId should be "toolbar" @UAC:11
    And ThemeProviderContext.ThemeService should be the same scoped service instance @UAC:12
    And the context should not be fixed so updates can flow to descendants @UAC:13

  @state:ThemeProviderBoundaryState
  @SC:04
  Scenario: SC04 - Provider passes through class and additional attributes
    @precondition:ProviderParametersSet
    Given ThemeProviderBoundaryForm.className is "rounded-xl border p-4"
    @precondition:AdditionalAttributesProvided
    And ThemeProviderBoundaryForm.additionalAttributes contains data-testid="theme-root"

    @action:RenderProvider
    When ThemeProvider renders with additional attributes
    @param:className
    And className is "rounded-xl border p-4"
    @param:additionalAttributes
    And additionalAttributes include data-testid="theme-root"

    @postcondition:AttributesRendered
    Then the root element should have class "rounded-xl" @UAC:14
    And the root element should have class "border" @UAC:15
    And the root element should have data-testid="theme-root" @UAC:16
    And data-theme-provider should remain present alongside additional attributes @UAC:17

  @state:ThemeProviderBoundaryState
  @SC:05
  Scenario: SC05 - Provider updates only when the matching provider receives ThemeChanged
    @precondition:TwoProvidersRegistered
    Given ThemeProviderBoundaryState.providerId is "tp-a"
    @precondition:SiblingProviderExists
    And another provider "tp-b" is registered

    @action:ReceiveThemeChanged
    When ComponentThemeService raises ThemeChanged for "tp-a"
    @param:id
    And id is "tp-a"

    @postcondition:MatchingProviderUpdated
    Then provider "tp-a" should refresh its inline style @UAC:18
    And provider "tp-a" should request a component rerender @UAC:19
    And provider "tp-b" should not refresh because of this event @UAC:20
    @postevent:ThemeProviderStyleUpdated
    And ThemeProviderStyleUpdated should be scoped to provider "tp-a" @UAC:21

  @state:ThemeProviderBoundaryState
  @SC:06
  Scenario: SC06 - Provider ignores ThemeChanged for a different provider
    @precondition:ProviderInitialized
    Given ThemeProviderBoundaryState.providerId is "tp-a"
    @precondition:CurrentInlineStyleKnown
    And ThemeProviderBoundaryState.inlineStyle contains Ocean tokens

    @action:ReceiveThemeChanged
    When ComponentThemeService raises ThemeChanged for "tp-b"
    @param:id
    And id is "tp-b"

    @postcondition:NonMatchingProviderUnchanged
    Then provider "tp-a" should keep its existing inline style @UAC:22
    And provider "tp-a" should not request a rerender for the non-matching event @UAC:23
    And no child context should be replaced @UAC:24

  @state:ThemeProviderBoundaryState
  @SC:07
  Scenario: SC07 - Provider unregisters and unsubscribes on disposal
    @precondition:ProviderInitialized
    Given ThemeProviderBoundaryState.providerId is "temporary-preview"
    @precondition:ProviderSubscribed
    And ThemeProvider is subscribed to ThemeChanged

    @action:DisposeProvider
    When ThemeProvider is disposed
    @param:id
    And id is "temporary-preview"

    @postcondition:ProviderDisposed
    Then ComponentThemeService should not contain provider "temporary-preview" @UAC:25
    And ThemeProvider should unsubscribe from ThemeChanged @UAC:26
    And later ThemeChanged events should not call the disposed provider @UAC:27
    @postevent:ThemeProviderDisposed
    And ThemeProviderDisposed should include providerId "temporary-preview" @UAC:28

  @state:ThemeProviderBoundaryState
  @SC:08
  Scenario: SC08 - Provider with no matching theme renders without token style
    @precondition:ProviderRegisteredWithUnknownTheme
    Given ThemeProviderBoundaryState.providerId is "unknown-preview"
    @precondition:UnknownThemeNameStored
    And ComponentThemeService has provider "unknown-preview" set to "Solarized"

    @action:ApplyThemeTokens
    When ThemeProvider applies tokens for provider "unknown-preview"
    @param:id
    And id is "unknown-preview"
    @param:themeName
    And themeName is "Solarized"

    @postcondition:NoStyleForUnknownTheme
    Then the provider inline style should be empty @UAC:29
    And child content should still render @UAC:30
    And the provider should keep data-theme-provider="unknown-preview" @UAC:31
