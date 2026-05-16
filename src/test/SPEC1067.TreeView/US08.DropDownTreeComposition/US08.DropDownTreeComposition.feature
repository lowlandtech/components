@SPEC:1067
@US:08
Feature: SPEC1067.US08 - TreeView DropDown Composition
  As an application author
  I want TreeView to compose cleanly inside a popover or dropdown selector
  So that Radzen DropDownTree style single and multiple hierarchical pickers can be built without a separate tree implementation

  Background:
    Given the component library LowlandTech.Components.TreeView
    And TreeView may be hosted inside a popup, combobox, or dropdown shell

  # STATE
  # interface TreeViewDropDownState {
  #   open: boolean
  #   selectedKey: string | null
  #   selectedKeys: string[]
  #   displayText: string
  #   query: string
  #   popupWidth: string | null
  #   focusOwner: 'trigger' | 'tree' | 'filter'
  # }

  # FORM
  # interface TreeViewDropDownForm {
  #   triggerText: string
  #   selectedKeys: string[]
  #   query: string
  #   closeOnSelect: boolean
  # }

  # REPRESENTATION
  # interface TreeDropDownSelectionRepresentation {
  #   key: string
  #   text: string
  #   path: string[]
  #   selected: boolean
  # }

  # ACTIONS
  # type TreeViewDropDownAction =
  #   | { type: 'OpenDropDownTree' }
  #   | { type: 'CloseDropDownTree' }
  #   | { type: 'SelectDropDownNode'; key: string }
  #   | { type: 'ToggleDropDownNode'; key: string }
  #   | { type: 'FilterDropDownTree'; query: string }
  #   | { type: 'ClearDropDownSelection' }

  # REDUCER TRANSITIONS
  # OpenDropDownTree -> open true, focusOwner set to tree or filter
  # CloseDropDownTree -> open false, focusOwner set to trigger
  # SelectDropDownNode -> selectedKey set, displayText updated, optional close
  # ToggleDropDownNode -> selectedKeys toggled, displayText summarized
  # FilterDropDownTree -> query updated and visible tree nodes recalculated
  # ClearDropDownSelection -> selected state cleared and display text reset

  # EFFECTS
  # on SelectDropDownNode -> ValueChanged callback fires
  # on ToggleDropDownNode -> ValuesChanged callback fires
  # on FilterDropDownTree -> TreeView filtering logic runs inside popup

  # EVENTS
  # type TreeViewDropDownEvent = 'DropDownOpened' | 'DropDownClosed' | 'DropDownSelectionChanged' | 'DropDownFilterChanged'

  @state:TreeViewDropDownState
  @SC:01
  Scenario: SC01 - Single selection updates trigger text and closes the popup
    @precondition:DropDownClosed
    Given TreeViewDropDownState.open is false
    @action:OpenDropDownTree
    When the user opens the dropdown tree
    @postcondition:PopupOpened
    Then open should be true @UAC:01
    And focusOwner should be "tree" or "filter" according to configuration @UAC:02
    @postevent:DropDownOpened
    And DropDownOpened should be emitted @UAC:03
    @action:SelectDropDownNode
    @param:key="Products"
    When the user selects node "Products"
    @postcondition:SingleSelectionCommitted
    Then selectedKey should be "Products" @UAC:04
    And displayText should be the selected node text or path @UAC:05
    And open should be false when closeOnSelect is true @UAC:06
    @postevent:DropDownSelectionChanged
    And DropDownSelectionChanged should include key "Products" @UAC:07

  @state:TreeViewDropDownState
  @SC:02
  Scenario: SC02 - Multiple selection summarizes selected nodes
    @precondition:MultipleDropDownSelection
    Given selectedKeys contains "Products"
    @action:ToggleDropDownNode
    @param:key="Services"
    When the user toggles node "Services"
    @postcondition:MultipleSelectionUpdated
    Then selectedKeys should contain "Products" and "Services" @UAC:08
    And displayText should summarize the selected nodes @UAC:09
    And the popup should remain open while multiple selection is active @UAC:10
    @postevent:DropDownSelectionChanged
    And DropDownSelectionChanged should include selected count 2 @UAC:11

  @state:TreeViewDropDownState
  @SC:03
  Scenario: SC03 - Dropdown tree filtering narrows popup results
    @precondition:FilterableDropDownTree
    Given the dropdown tree is open
    @action:FilterDropDownTree
    @param:query="prod"
    When the user types "prod" in the dropdown filter
    @postcondition:PopupTreeFiltered
    Then query should be "prod" @UAC:12
    And the popup tree should show matching nodes and ancestors @UAC:13
    And keyboard focus should remain in the filter when configured @UAC:14
    @postevent:DropDownFilterChanged
    And DropDownFilterChanged should include query "prod" @UAC:15

  @state:TreeViewDropDownState
  @SC:04
  Scenario: SC04 - Escape closes popup and restores trigger focus
    @precondition:DropDownOpen
    Given TreeViewDropDownState.open is true
    @action:CloseDropDownTree
    When the user presses Escape
    @postcondition:PopupClosed
    Then open should be false @UAC:16
    And focusOwner should be "trigger" @UAC:17
    And selection state should remain unchanged @UAC:18
    @postevent:DropDownClosed
    And DropDownClosed should include reason "escape" @UAC:19

