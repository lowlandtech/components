@SPEC:1073
@US:01
Feature: SPEC1073.US01 - Session Keyed App Layout Scenarios
  As an application shell author
  I want one MainLayout template to resolve enum-keyed layout scenarios from the app session
  So that a single template can deliver multiple topbar, sidebar, aside, footer, and main-content behaviors

  Background:
    Given the component library LowlandTech.Components.AppLayout
    And AppLayout renders named slots through the shared Grid component

  # STATE
  # interface AppLayoutScenarioState {
  #   sessionKey: string
  #   scenario: 'workspace' | 'documentation' | 'focus' | 'studio' | 'kiosk'
  #   hasTopBar: boolean
  #   hasSidebar: boolean
  #   hasAside: boolean
  #   hasFooter: boolean
  #   mainSpan: ResponsiveSpanRepresentation
  #   sidebarSpan: ResponsiveSpanRepresentation | null
  #   asideSpan: ResponsiveSpanRepresentation | null
  # }

  # FORM
  # interface AppLayoutScenarioForm {
  #   scenario: AppLayoutScenarioState['scenario']
  #   sessionKey: string
  #   topbarOpen: boolean
  #   sidebarOpen: boolean
  # }

  # REPRESENTATION
  # interface ResponsiveSpanRepresentation {
  #   xs: number
  #   sm: number | null
  #   md: number | null
  #   lg: number | null
  #   xl: number | null
  # }

  # ACTIONS
  # type AppLayoutScenarioAction =
  #   | { type: 'RenderScenario'; scenario: AppLayoutScenarioState['scenario'] }
  #   | { type: 'ApplySessionScenario'; sessionKey: string; scenario: AppLayoutScenarioState['scenario'] }
  #   | { type: 'ChangeSessionScenario'; scenario: AppLayoutScenarioState['scenario'] }
  #   | { type: 'RenderSlots' }

  # REDUCER TRANSITIONS
  # RenderScenario -> slot visibility and GridItem spans resolved from enum key
  # ApplySessionScenario -> session scenario wins over parameter scenario
  # ChangeSessionScenario -> scenario replaced and rendered layout updates reactively
  # RenderSlots -> topbar/sidebar/main/aside/footer slot fragments rendered according to scenario

  # EFFECTS
  # on ChangeSessionScenario -> AppLayoutSession raises ScenarioChanged and AppLayout re-renders
  # on RenderScenario -> no template replacement, only scenario metadata and Grid spans change

  # EVENTS
  # type AppLayoutScenarioEvent = 'ScenarioRendered' | 'SessionScenarioChanged' | 'SlotsRendered'

  @state:AppLayoutScenarioState
  @SC:01
  Scenario: SC01 - Workspace scenario renders a docked topbar sidebar and main content
    @precondition:WorkspaceScenario
    Given AppLayoutScenarioState.scenario is "workspace"
    @action:RenderScenario
    @param:scenario="workspace"
    When AppLayout renders the scenario
    @postcondition:WorkspaceSlotsVisible
    Then the root should expose data-layout-scenario "workspace" @UAC:01
    And the topbar slot should render @UAC:02
    And the sidebar slot should render @UAC:03
    And the main slot should render @UAC:04
    And the sidebar should use a narrower desktop Grid span than the main content @UAC:05
    @postevent:ScenarioRendered
    And ScenarioRendered should include scenario "workspace" @UAC:06

  @state:AppLayoutScenarioState
  @SC:02
  Scenario: SC02 - Documentation scenario adds a right aside without changing the template
    @precondition:DocumentationScenario
    Given AppLayoutScenarioState.scenario is "documentation"
    @action:RenderScenario
    @param:scenario="documentation"
    When AppLayout renders the scenario
    @postcondition:DocumentationSlotsVisible
    Then the root should expose data-layout-scenario "documentation" @UAC:07
    And the topbar, sidebar, main, and aside slots should render @UAC:08
    And the aside should use a right-side Grid span @UAC:09
    And the same Main slot content should render as in other scenarios @UAC:10

  @state:AppLayoutScenarioState
  @SC:03
  Scenario: SC03 - Focus and kiosk scenarios hide navigation chrome
    @precondition:FocusScenario
    Given AppLayoutScenarioState.scenario is "focus"
    @action:RenderScenario
    @param:scenario="focus"
    When AppLayout renders the focus scenario
    @postcondition:FocusChromeReduced
    Then the topbar slot should render @UAC:11
    And the sidebar and aside slots should not render @UAC:12
    And the main slot should span all desktop columns @UAC:13
    @precondition:KioskScenario
    Given AppLayoutScenarioState.scenario is "kiosk"
    @action:RenderScenario
    @param:scenario="kiosk"
    When AppLayout renders the kiosk scenario
    @postcondition:KioskChromeHidden
    Then only the main slot should render @UAC:14
    And the root should expose data-layout-scenario "kiosk" @UAC:15

  @state:AppLayoutScenarioState
  @SC:04
  Scenario: SC04 - App session scenario overrides the parameter scenario
    @precondition:SessionScenarioProvided
    Given AppLayout has parameter scenario "workspace"
    And the app session scenario is "documentation"
    @action:ApplySessionScenario
    @param:sessionKey="demo-session"
    @param:scenario="documentation"
    When AppLayout resolves the effective scenario
    @postcondition:SessionWins
    Then the root should expose data-session-key "demo-session" @UAC:16
    And the root should expose data-layout-scenario "documentation" @UAC:17
    And the aside slot should render from the documentation scenario @UAC:18

  @state:AppLayoutScenarioState
  @SC:05
  Scenario: SC05 - Session scenario changes update the Grid reactively
    @precondition:SessionScenarioActive
    Given the app session scenario is "workspace"
    @action:ChangeSessionScenario
    @param:scenario="studio"
    When the app session changes to "studio"
    @postcondition:ScenarioReRendered
    Then the root should expose data-layout-scenario "studio" @UAC:19
    And the footer slot should render because studio enables footer chrome @UAC:20
    And the main content should remain mounted with the same template content @UAC:21
    @postevent:SessionScenarioChanged
    And SessionScenarioChanged should include scenario "studio" @UAC:22

