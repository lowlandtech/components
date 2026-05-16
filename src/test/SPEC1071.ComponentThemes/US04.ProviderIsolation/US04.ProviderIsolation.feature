@US:04
Feature: SPEC1071.US04 - Provider Isolation
  As a page builder
  I want multiple ThemeProvider instances to maintain independent theme state
  So that sibling and nested component regions can demonstrate different themes at the same time.

  Background:
    Given the component library LowlandTech.Components.ThemeProvider
    And ComponentThemeService tracks active themes by provider ID

  # STATE
  # interface ThemeProviderIsolationState {
  #   providers: Record<string, { activeThemeName: string; inlineStyle: string }>
  #   providerTree: Record<string, string[]>
  #   lastChangedProviderId: string | null
  #   affectedProviderIds: string[]
  # }
  #
  # FORM
  # interface ThemeProviderIsolationForm {
  #   parentProviderId: string | null
  #   childProviderId: string
  #   themeName: string
  # }
  #
  # REPRESENTATION
  # interface ThemeBoundaryRepresentation {
  #   providerId: string
  #   themeName: string
  #   subtreeSelector: string
  #   inlineStyle: string
  # }
  #
  # ACTIONS
  # type ThemeProviderIsolationAction =
  #   | { type: 'RenderSiblingProviders'; providers: ThemeBoundaryRepresentation[] }
  #   | { type: 'RenderNestedProviders'; parentProviderId: string; childProviderId: string }
  #   | { type: 'CycleProviderTheme'; providerId: string }
  #   | { type: 'ApplyNestedOverride'; parentProviderId: string; childProviderId: string }
  #
  # REDUCER TRANSITIONS
  # RenderSiblingProviders -> providers keyed by id with separate active themes
  # RenderNestedProviders -> providerTree[parentProviderId] includes childProviderId
  # CycleProviderTheme -> update only providers[providerId]
  # ApplyNestedOverride -> child inlineStyle overrides inherited parent variables inside child subtree
  #
  # EFFECTS
  # on CycleProviderTheme -> ComponentThemeService.Next(providerId), ThemeChanged(providerId)
  #
  # EVENTS
  # type ThemeProviderIsolationEvent = 'SiblingProvidersRendered' | 'NestedProvidersRendered' | 'ProviderIsolationMaintained'

  @state:ThemeProviderIsolationState
  @SC:01
  Scenario: SC01 - Sibling providers render with independent initial themes
    @precondition:SiblingProvidersConfigured
    Given provider "tp-a" is configured with initial theme "Ocean"
    @precondition:SecondSiblingConfigured
    And provider "tp-b" is configured with initial theme "Rose"

    @action:RenderSiblingProviders
    When sibling ThemeProvider components render
    @param:providers
    And providers are "tp-a" and "tp-b"

    @postcondition:SiblingThemesIndependent
    Then provider "tp-a" should have active theme "Ocean" @UAC:01
    And provider "tp-b" should have active theme "Rose" @UAC:02
    And provider "tp-a" inline style should contain Ocean tokens @UAC:03
    And provider "tp-b" inline style should contain Rose tokens @UAC:04
    @postevent:SiblingProvidersRendered
    And SiblingProvidersRendered should include both provider IDs @UAC:05

  @state:ThemeProviderIsolationState
  @SC:02
  Scenario: SC02 - Cycling one sibling does not affect another sibling
    @precondition:SiblingsRendered
    Given provider "tp-a" has active theme "Ocean"
    @precondition:SiblingThemeKnown
    And provider "tp-b" has active theme "Rose"

    @action:CycleProviderTheme
    When the user cycles provider "tp-a"
    @param:providerId
    And providerId is "tp-a"

    @postcondition:OnlyTargetSiblingChanged
    Then provider "tp-a" should advance from "Ocean" to the next registered theme @UAC:06
    And provider "tp-b" should remain "Rose" @UAC:07
    And ThemeChanged should be raised for "tp-a" only @UAC:08
    @postevent:ProviderIsolationMaintained
    And ProviderIsolationMaintained should list "tp-a" as the only affected provider @UAC:09

  @state:ThemeProviderIsolationState
  @SC:03
  Scenario: SC03 - Nested provider overrides parent tokens inside its own subtree
    @precondition:NestedProvidersConfigured
    Given provider "tp-outer" is configured with initial theme "Forest"
    @precondition:InnerProviderConfigured
    And provider "tp-inner" is configured with initial theme "Dark"

    @action:RenderNestedProviders
    When nested ThemeProvider components render
    @param:parentProviderId
    And parentProviderId is "tp-outer"
    @param:childProviderId
    And childProviderId is "tp-inner"

    @postcondition:NestedOverrideApplied
    Then content directly inside "tp-outer" should resolve Forest tokens @UAC:10
    And content inside "tp-inner" should resolve Dark tokens @UAC:11
    And "tp-inner" should have its own data-theme-provider attribute @UAC:12
    @postevent:NestedProvidersRendered
    And NestedProvidersRendered should preserve parent and child provider IDs @UAC:13

  @state:ThemeProviderIsolationState
  @SC:04
  Scenario: SC04 - Cycling nested inner provider does not change outer provider
    @precondition:NestedProvidersRendered
    Given provider "tp-outer" has active theme "Forest"
    @precondition:InnerThemeKnown
    And provider "tp-inner" has active theme "Dark"

    @action:CycleProviderTheme
    When the user cycles provider "tp-inner"
    @param:providerId
    And providerId is "tp-inner"

    @postcondition:OnlyInnerChanged
    Then provider "tp-inner" should advance from "Dark" to the next registered theme @UAC:14
    And provider "tp-outer" should remain "Forest" @UAC:15
    And outer content outside "tp-inner" should continue resolving Forest tokens @UAC:16
    @postevent:ProviderIsolationMaintained
    And ProviderIsolationMaintained should list "tp-inner" as the only affected provider @UAC:17

  @state:ThemeProviderIsolationState
  @SC:05
  Scenario: SC05 - Cycling nested outer provider does not overwrite inner provider
    @precondition:NestedProvidersRendered
    Given provider "tp-outer" has active theme "Forest"
    @precondition:InnerThemeKnown
    And provider "tp-inner" has active theme "Dark"

    @action:CycleProviderTheme
    When the user cycles provider "tp-outer"
    @param:providerId
    And providerId is "tp-outer"

    @postcondition:OuterChangedInnerPreserved
    Then provider "tp-outer" should advance from "Forest" to the next registered theme @UAC:18
    And provider "tp-inner" should remain "Dark" @UAC:19
    And inner content should continue resolving Dark tokens @UAC:20
    @postevent:ProviderIsolationMaintained
    And ProviderIsolationMaintained should list "tp-outer" as the only affected provider @UAC:21

  @state:ThemeProviderIsolationState
  @SC:06
  Scenario: SC06 - Disposing one provider does not remove sibling provider state
    @precondition:SiblingsRendered
    Given provider "tp-a" has active theme "Ocean"
    @precondition:SiblingThemeKnown
    And provider "tp-b" has active theme "Rose"

    @action:CycleProviderTheme
    When provider "tp-a" is disposed and provider "tp-b" is then cycled
    @param:providerId
    And providerId is "tp-b"

    @postcondition:RemainingSiblingStillWorks
    Then ComponentThemeService should not contain provider "tp-a" @UAC:22
    And ComponentThemeService should still contain provider "tp-b" @UAC:23
    And provider "tp-b" should advance from "Rose" to the next registered theme @UAC:24
    And no disposed provider callback should run @UAC:25

  @state:ThemeProviderIsolationState
  @SC:07
  Scenario: SC07 - Duplicate provider IDs intentionally share service state
    @precondition:DuplicateProviderIdsRendered
    Given two ThemeProvider components are rendered with id "shared"
    @precondition:SharedStateKnown
    And ComponentThemeService has one provider state entry for "shared"

    @action:CycleProviderTheme
    When the user cycles one provider with id "shared"
    @param:providerId
    And providerId is "shared"

    @postcondition:DuplicateIdsShareBoundaryState
    Then ComponentThemeService should update the single "shared" provider entry @UAC:26
    And ThemeChanged should be raised for provider "shared" @UAC:27
    And both provider components with id "shared" should refresh when subscribed @UAC:28

  @state:ThemeProviderIsolationState
  @SC:08
  Scenario: SC08 - Independent providers can host ordinary components without extra parameters
    @precondition:ProvidersContainComponents
    Given provider "tp-a" contains Button and Badge components
    @precondition:SiblingContainsComponents
    And provider "tp-b" contains Button and Badge components

    @action:RenderSiblingProviders
    When both providers render themed child components
    @param:providers
    And providers are "tp-a" and "tp-b"

    @postcondition:ChildrenUseScopedVariables
    Then child Button components should render without theme-specific parameters @UAC:29
    And child Badge components should render without theme-specific parameters @UAC:30
    And each child should resolve CSS variables from its nearest ThemeProvider boundary @UAC:31
    @postevent:ProviderIsolationMaintained
    And ProviderIsolationMaintained should confirm no document-level theme mutation @UAC:32
