@SPEC:1067
@US:01
Feature: SPEC1067.US01 - TreeView Structural Rendering
  As an application author
  I want TreeView to render hierarchical data with stable depth and child metadata
  So that callers can build file explorers, navigation trees, and object trees on top of a reliable contract

  Background:
    Given the component library LowlandTech.Components.TreeView
    And TreeView can render typed items from a top-level item collection

  # STATE
  # interface TreeViewStructureState {
  #   roots: TreeNodeRepresentation[]
  #   expandedKeys: string[]
  #   visibleKeys: string[]
  #   depthByKey: Record<string, number>
  #   hasChildrenByKey: Record<string, boolean>
  #   renderMode: 'data' | 'static'
  # }

  # FORM
  # interface TreeViewStructureForm {
  #   rootKeys: string[]
  #   expandedKeys: string[]
  #   itemTemplateName: string | null
  # }

  # REPRESENTATION
  # interface TreeNodeRepresentation {
  #   key: string
  #   parentKey: string | null
  #   text: string
  #   children: TreeNodeRepresentation[]
  #   hasChildren: boolean
  #   expanded: boolean
  #   disabled: boolean
  # }

  # ACTIONS
  # type TreeViewStructureAction =
  #   | { type: 'RenderTree'; roots: TreeNodeRepresentation[] }
  #   | { type: 'RenderStaticTree' }
  #   | { type: 'SetExpandedKeys'; expandedKeys: string[] }
  #   | { type: 'RenderTemplate'; itemTemplateName: string }

  # REDUCER TRANSITIONS
  # RenderTree -> roots assigned, visibleKeys recalculated from expandedKeys
  # RenderStaticTree -> renderMode set to 'static'
  # SetExpandedKeys -> expandedKeys replaced, depthByKey recalculated for visible nodes
  # RenderTemplate -> item template receives item, depth, expanded, and hasChildren context

  # EFFECTS
  # on RenderTree -> no network side effect
  # on RenderStaticTree -> no network side effect
  # on SetExpandedKeys -> ExpandedChanged events emitted only when user initiated
  # on RenderTemplate -> component renders caller-provided content

  # EVENTS
  # type TreeViewStructureEvent = 'TreeRendered' | 'TemplateRendered' | 'ExpansionStateApplied'

  @state:TreeViewStructureState
  @SC:01
  Scenario: SC01 - Collapsed roots render without descendants
    @precondition:RootItemsAvailable
    Given TreeViewStructureState.roots contains "a" and "b"
    @precondition:NoExpandedKeys
    And TreeViewStructureState.expandedKeys is empty
    @action:RenderTree
    @param:roots=["a","b"]
    When TreeView renders the root collection
    @postcondition:RootContainerRendered
    Then the tree root should render data-slot "tree-view" @UAC:01
    And visibleKeys should be "a", "b" @UAC:02
    And descendants of collapsed roots should not render @UAC:03
    @postevent:TreeRendered
    And TreeRendered should include renderMode "data" @UAC:04

  @state:TreeViewStructureState
  @SC:02
  Scenario: SC02 - Expanded branches expose direct children with depth context
    @precondition:NestedItemsAvailable
    Given node "a" has children "a.1" and "a.2"
    @precondition:RootExpanded
    And TreeViewStructureState.expandedKeys contains "a"
    @action:SetExpandedKeys
    @param:expandedKeys=["a"]
    When TreeView applies the expansion state
    @postcondition:ChildrenVisible
    Then visibleKeys should be "a", "a.1", "a.2", "b" @UAC:05
    And depthByKey for "a" should be 0 @UAC:06
    And depthByKey for "a.1" should be 1 @UAC:07
    @postevent:ExpansionStateApplied
    And ExpansionStateApplied should include expanded key "a" @UAC:08

  @state:TreeViewStructureState
  @SC:03
  Scenario: SC03 - Deep descendants render only when every ancestor is expanded
    @precondition:DeepItemsAvailable
    Given node "a.1.x" is a child of "a.1"
    @precondition:AncestorExpanded
    And TreeViewStructureState.expandedKeys contains "a" and "a.1"
    @action:SetExpandedKeys
    @param:expandedKeys=["a","a.1"]
    When TreeView applies the nested expansion state
    @postcondition:DeepChildVisible
    Then visibleKeys should include "a.1.x" @UAC:09
    And depthByKey for "a.1.x" should be 2 @UAC:10
    And collapsing "a" should hide "a.1.x" even when "a.1" remains expanded in state @UAC:11

  @state:TreeViewStructureState
  @SC:04
  Scenario: SC04 - Item template receives structural context
    @precondition:ItemTemplateConfigured
    Given TreeView has an item template named "ExplorerNode"
    @precondition:NodeHasChildren
    And node "a" has children
    @action:RenderTemplate
    @param:itemTemplateName="ExplorerNode"
    When TreeView renders node "a"
    @postcondition:TemplateContextProvided
    Then the template context should expose the original item @UAC:12
    And the template context should expose depth 0 @UAC:13
    And the template context should expose hasChildren true @UAC:14
    And the template context should expose the current expanded state @UAC:15
    @postevent:TemplateRendered
    And TemplateRendered should include key "a" @UAC:16

