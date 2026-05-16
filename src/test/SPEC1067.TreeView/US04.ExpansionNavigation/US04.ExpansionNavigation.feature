@SPEC:1067
@US:04
Feature: SPEC1067.US04 - TreeView Expansion and Keyboard Navigation
  As a keyboard and pointer user
  I want TreeView expansion, collapse, focus, and keyboard navigation to follow accessible tree patterns
  So that large trees are usable without a mouse and can match Radzen and MudBlazor interaction behavior

  Background:
    Given the component library LowlandTech.Components.TreeView
    And TreeView renders a hierarchical tree with focusable items

  # STATE
  # interface TreeViewNavigationState {
  #   expandedKeys: string[]
  #   focusedKey: string | null
  #   selectedKey: string | null
  #   visibleKeys: string[]
  #   singleExpand: boolean
  #   autoExpand: boolean
  #   expandTrigger: 'toggle' | 'click' | 'doubleClick'
  # }

  # FORM
  # interface TreeViewNavigationForm {
  #   targetKey: string
  #   key: 'ArrowUp' | 'ArrowDown' | 'ArrowLeft' | 'ArrowRight' | 'Home' | 'End' | 'Enter' | 'Space'
  #   expandAll: boolean
  # }

  # REPRESENTATION
  # interface TreeNavigationItemRepresentation {
  #   key: string
  #   depth: number
  #   expanded: boolean
  #   selected: boolean
  #   focused: boolean
  #   hasChildren: boolean
  # }

  # ACTIONS
  # type TreeViewNavigationAction =
  #   | { type: 'ToggleExpanded'; key: string }
  #   | { type: 'ExpandNode'; key: string }
  #   | { type: 'CollapseNode'; key: string }
  #   | { type: 'ExpandAll' }
  #   | { type: 'CollapseAll' }
  #   | { type: 'FocusNode'; key: string }
  #   | { type: 'HandleKeyboard'; key: TreeViewNavigationForm['key'] }
  #   | { type: 'SetSelectedKey'; key: string | null }

  # REDUCER TRANSITIONS
  # ToggleExpanded -> key added or removed from expandedKeys
  # ExpandNode -> key added, sibling keys removed when singleExpand is true
  # CollapseNode -> key removed and descendants hidden
  # ExpandAll -> all expandable visible and loaded keys added
  # CollapseAll -> expandedKeys cleared
  # FocusNode -> focusedKey set to visible key
  # HandleKeyboard -> focus, expansion, collapse, or selection updated by key semantics
  # SetSelectedKey -> selectedKey set, ancestors expanded when autoExpand is true

  # EFFECTS
  # on ExpandNode -> Expand callback fires
  # on CollapseNode -> Collapse callback fires
  # on ExpandAll -> programmatic API expands every loaded branch
  # on CollapseAll -> programmatic API collapses every branch

  # EVENTS
  # type TreeViewNavigationEvent = 'NodeExpanded' | 'NodeCollapsed' | 'FocusChanged' | 'KeyboardHandled' | 'AllExpanded' | 'AllCollapsed'

  @state:TreeViewNavigationState
  @SC:01
  Scenario: SC01 - Toggle button expands and collapses a branch
    @precondition:BranchCollapsed
    Given node "Categories" has children and is collapsed
    @action:ToggleExpanded
    @param:key="Categories"
    When the user activates the node toggle
    @postcondition:BranchExpanded
    Then expandedKeys should contain "Categories" @UAC:01
    And the node should expose aria-expanded true @UAC:02
    And children should render after the parent @UAC:03
    @postevent:NodeExpanded
    And NodeExpanded should include key "Categories" @UAC:04
    @action:ToggleExpanded
    @param:key="Categories"
    When the user activates the node toggle again
    @postcondition:BranchCollapsed
    Then expandedKeys should not contain "Categories" @UAC:05
    And descendants should be removed from the visible item list @UAC:06
    @postevent:NodeCollapsed
    And NodeCollapsed should include key "Categories" @UAC:07

  @state:TreeViewNavigationState
  @SC:02
  Scenario: SC02 - Single expand collapses sibling branches
    @precondition:SingleExpandEnabled
    Given TreeViewNavigationState.singleExpand is true
    And expandedKeys contains "Mail"
    @action:ExpandNode
    @param:key="Categories"
    When the user expands sibling node "Categories"
    @postcondition:SiblingsCollapsed
    Then expandedKeys should contain "Categories" @UAC:08
    And expandedKeys should not contain "Mail" @UAC:09
    And only one branch at the same depth should remain expanded @UAC:10

  @state:TreeViewNavigationState
  @SC:03
  Scenario: SC03 - Auto expand reveals selected descendants
    @precondition:AutoExpandEnabled
    Given TreeViewNavigationState.autoExpand is true
    And selected node "logo.png" is nested under "images" and "C:"
    @action:SetSelectedKey
    @param:key="logo.png"
    When TreeView receives the selected key
    @postcondition:AncestorsExpanded
    Then expandedKeys should contain "C:" @UAC:11
    And expandedKeys should contain "images" @UAC:12
    And visibleKeys should contain "logo.png" @UAC:13

  @state:TreeViewNavigationState
  @SC:04
  Scenario: SC04 - Keyboard arrows move focus and manage expansion
    @precondition:TreeFocused
    Given focusedKey is "Categories"
    And node "Categories" has children and is collapsed
    @action:HandleKeyboard
    @param:key="ArrowRight"
    When the user presses ArrowRight
    @postcondition:KeyboardExpanded
    Then expandedKeys should contain "Categories" @UAC:14
    And focusedKey should remain "Categories" @UAC:15
    @action:HandleKeyboard
    @param:key="ArrowDown"
    When the user presses ArrowDown
    @postcondition:FocusMovedToNextVisible
    Then focusedKey should be the first visible child of "Categories" @UAC:16
    @action:HandleKeyboard
    @param:key="ArrowLeft"
    When the user presses ArrowLeft on a child
    @postcondition:FocusMovedToParent
    Then focusedKey should move to the parent branch @UAC:17
    @postevent:KeyboardHandled
    And KeyboardHandled should include the last key "ArrowLeft" @UAC:18

  @state:TreeViewNavigationState
  @SC:05
  Scenario: SC05 - Home End Enter and Space follow tree accessibility semantics
    @precondition:MultipleVisibleNodes
    Given visibleKeys contains "A", "B", and "C"
    @action:HandleKeyboard
    @param:key="End"
    When the user presses End
    @postcondition:FocusLastVisible
    Then focusedKey should be "C" @UAC:19
    @action:HandleKeyboard
    @param:key="Home"
    When the user presses Home
    @postcondition:FocusFirstVisible
    Then focusedKey should be "A" @UAC:20
    @action:HandleKeyboard
    @param:key="Enter"
    When the user presses Enter
    @postcondition:KeyboardSelectsFocusedNode
    Then selectedKey should be "A" @UAC:21
    And Space should perform the same selection action as Enter @UAC:22

  @state:TreeViewNavigationState
  @SC:06
  Scenario: SC06 - Programmatic expand all and collapse all APIs update all branches
    @precondition:MultipleBranchesAvailable
    Given the tree contains expandable nodes "C:", "config", and "images"
    @action:ExpandAll
    When the application calls ExpandAll
    @postcondition:EveryBranchExpanded
    Then expandedKeys should contain every loaded expandable key @UAC:23
    @postevent:AllExpanded
    And AllExpanded should include expanded count 3 @UAC:24
    @action:CollapseAll
    When the application calls CollapseAll
    @postcondition:EveryBranchCollapsed
    Then expandedKeys should be empty @UAC:25
    @postevent:AllCollapsed
    And AllCollapsed should include collapsed count 3 @UAC:26

