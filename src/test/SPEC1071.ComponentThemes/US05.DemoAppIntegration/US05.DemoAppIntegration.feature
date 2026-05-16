@US:05
Feature: SPEC1071.US05 - Demo App Integration
  As a component library maintainer
  I want the demo app to register themes and expose ThemeProvider in the component catalog
  So that designers and developers can verify theme boundaries interactively.

  Background:
    Given the component demo app LowlandTech.Components.App
    And StoryRegistry discovers StoryComponent implementations

  # STATE
  # interface ThemeDemoIntegrationState {
  #   registeredServices: string[]
  #   registeredThemeNames: string[]
  #   storyGroups: Record<string, string[]>
  #   selectedStoryName: string | null
  #   componentTableEntries: string[]
  # }
  #
  # FORM
  # interface ThemeDemoIntegrationForm {
  #   serviceType: string
  #   implementationType: string
  #   storyName: string
  #   storyGroup: string
  # }
  #
  # REPRESENTATION
  # interface ThemeDemoStoryRepresentation {
  #   displayName: string
  #   group: string
  #   states: string[]
  #   codeTabs: string[]
  # }
  #
  # ACTIONS
  # type ThemeDemoIntegrationAction =
  #   | { type: 'RegisterThemeServices'; serviceType: string; implementationTypes: string[] }
  #   | { type: 'DiscoverThemeProviderStory'; storyName: string }
  #   | { type: 'RenderComponentTable'; storyName: string }
  #   | { type: 'SelectThemeProviderStory'; storyName: string }
  #
  # REDUCER TRANSITIONS
  # RegisterThemeServices -> registeredThemeNames and registeredServices populated
  # DiscoverThemeProviderStory -> storyGroups['Misc'] includes Theme Provider
  # RenderComponentTable -> componentTableEntries includes Theme Provider with icon and abbreviation
  # SelectThemeProviderStory -> selectedStoryName: Theme Provider
  #
  # EFFECTS
  # on SelectThemeProviderStory -> ShowcodeState.Select(story)
  #
  # EVENTS
  # type ThemeDemoIntegrationEvent = 'ThemeServicesRegistered' | 'ThemeProviderStoryDiscovered' | 'ThemeProviderStorySelected'

  @state:ThemeDemoIntegrationState
  @SC:01
  Scenario: SC01 - Demo app registers default component themes
    @precondition:HostBuilderCreated
    Given the WebAssembly host builder is configured
    @precondition:ThemeImplementationsAvailable
    And Light, Dark, Ocean, Rose, and Forest theme implementations exist

    @action:RegisterThemeServices
    When the demo app registers component theme services
    @param:serviceType
    And serviceType is "IComponentTheme"
    @param:implementationTypes
    And implementationTypes include Light, Dark, Ocean, Rose, and Forest

    @postcondition:ThemesRegistered
    Then IComponentTheme should be registered for LightComponentTheme @UAC:01
    And IComponentTheme should be registered for DarkComponentTheme @UAC:02
    And IComponentTheme should be registered for OceanComponentTheme @UAC:03
    And IComponentTheme should be registered for RoseComponentTheme @UAC:04
    And IComponentTheme should be registered for ForestComponentTheme @UAC:05
    @postevent:ThemeServicesRegistered
    And ThemeServicesRegistered should preserve registration order @UAC:06

  @state:ThemeDemoIntegrationState
  @SC:02
  Scenario: SC02 - Demo app registers ComponentThemeService as scoped state
    @precondition:HostBuilderCreated
    Given the WebAssembly host builder is configured
    @precondition:ThemesAreSingletonTokenBags
    And IComponentTheme implementations are registered as singleton services

    @action:RegisterThemeServices
    When the demo app registers ComponentThemeService
    @param:serviceType
    And serviceType is "ComponentThemeService"

    @postcondition:ScopedStateRegistered
    Then ComponentThemeService should be registered as scoped @UAC:07
    And per-provider theme state should not be stored on singleton theme objects @UAC:08
    And each user session should receive an independent ComponentThemeService instance @UAC:09

  @state:ThemeDemoIntegrationState
  @SC:03
  Scenario: SC03 - ThemeProvider story is discoverable in the Misc group
    @precondition:StoryAssemblyLoaded
    Given StoryRegistry scans the component app assembly
    @precondition:StoryHasDisplayName
    And ThemeProviderStory has DisplayName "Misc/Theme Provider"

    @action:DiscoverThemeProviderStory
    When stories are registered
    @param:storyName
    And storyName is "Theme Provider"

    @postcondition:StoryRegistered
    Then the Misc story group should contain "Theme Provider" @UAC:10
    And the story should inherit StoryComponent @UAC:11
    And the story should render through StoryCard @UAC:12
    @postevent:ThemeProviderStoryDiscovered
    And ThemeProviderStoryDiscovered should include group "Misc" @UAC:13

  @state:ThemeDemoIntegrationState
  @SC:04
  Scenario: SC04 - ThemeProvider story exposes boundary and nested states
    @precondition:StoryDiscovered
    Given ThemeProviderStory is registered
    @precondition:StoryStatesDefined
    And ThemeProviderStory defines its States collection

    @action:DiscoverThemeProviderStory
    When the story metadata is inspected
    @param:storyName
    And storyName is "Theme Provider"

    @postcondition:StatesAvailable
    Then the story should expose a "Boundary" state @UAC:14
    And the story should expose a "Nested" state @UAC:15
    And the Boundary state should include code tab "ThemeProviderStory.razor" @UAC:16
    And the Nested state should include code tab "ThemeProviderStory.razor" @UAC:17

  @state:ThemeDemoIntegrationState
  @SC:05
  Scenario: SC05 - Boundary story demonstrates sibling provider independence
    @precondition:BoundaryStateSelected
    Given the ThemeProvider story Boundary state is active
    @precondition:SiblingProvidersRendered
    And the preview contains providers "tp-a" and "tp-b"

    @action:SelectThemeProviderStory
    When the user views the Boundary state
    @param:storyName
    And storyName is "Theme Provider"

    @postcondition:BoundaryPreviewShowsIndependence
    Then provider "tp-a" should start with theme "Ocean" @UAC:18
    And provider "tp-b" should start with theme "Rose" @UAC:19
    And each provider should contain its own ThemeToggle @UAC:20
    And each provider should render sample Button components using scoped tokens @UAC:21
    @postevent:ThemeProviderStorySelected
    And ThemeProviderStorySelected should include state "Boundary" @UAC:22

  @state:ThemeDemoIntegrationState
  @SC:06
  Scenario: SC06 - Nested story demonstrates inner provider override
    @precondition:NestedStateSelected
    Given the ThemeProvider story Nested state is active
    @precondition:NestedProvidersRendered
    And the preview contains providers "tp-outer" and "tp-inner"

    @action:SelectThemeProviderStory
    When the user views the Nested state
    @param:storyName
    And storyName is "Theme Provider"

    @postcondition:NestedPreviewShowsOverride
    Then provider "tp-outer" should start with theme "Forest" @UAC:23
    And provider "tp-inner" should start with theme "Dark" @UAC:24
    And the inner provider should render inside the outer provider @UAC:25
    And each provider should contain a ThemeToggle scoped to its own provider ID @UAC:26
    @postevent:ThemeProviderStorySelected
    And ThemeProviderStorySelected should include state "Nested" @UAC:27

  @state:ThemeDemoIntegrationState
  @SC:07
  Scenario: SC07 - Component table displays Theme Provider with catalog identity
    @precondition:ComponentTableLoaded
    Given ComponentTablePage has grouped stories
    @precondition:ThemeProviderStoryRegistered
    And the Misc group contains "Theme Provider"

    @action:RenderComponentTable
    When the component table renders the Misc group
    @param:storyName
    And storyName is "Theme Provider"

    @postcondition:CatalogEntryRendered
    Then the component table should show "Theme Provider" @UAC:28
    And the component abbreviation should be "TP" @UAC:29
    And the component icon should use the palette-style Theme Provider icon paths @UAC:30
    And selecting the table entry should call ShowcodeState.Select for the Theme Provider story @UAC:31

  @state:ThemeDemoIntegrationState
  @SC:08
  Scenario: SC08 - Theme Provider story copy communicates scoped behavior
    @precondition:StoryDiscovered
    Given ThemeProviderStory is registered
    @precondition:DescriptionAvailable
    And ThemeProviderStory has descriptive text

    @action:SelectThemeProviderStory
    When the story header renders
    @param:storyName
    And storyName is "Theme Provider"

    @postcondition:ScopedBehaviorDocumented
    Then the description should explain that ThemeProvider establishes a scoped theme boundary @UAC:32
    And the description should explain that multiple providers can be active simultaneously @UAC:33
    And the description should explain that ThemeToggle inside a provider cycles registered themes @UAC:34
    And the description should explain that sibling boundaries are independent @UAC:35
