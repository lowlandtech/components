@SPEC:1067
@US:03
Feature: SPEC1067.US03 - TreeView Selection and Checkboxes
  As an application author
  I want TreeView to support single selection, toggle selection, multi selection, and tri-state checkboxes
  So that it can match MudBlazor selection modes and Radzen checked-value behavior

  Background:
    Given the component library LowlandTech.Components.TreeView
    And tree nodes have stable keys and optional disabled or readonly flags

  # STATE
  # interface TreeViewSelectionState {
  #   selectedKey: string | null
  #   selectedKeys: string[]
  #   checkedKeys: string[]
  #   indeterminateKeys: string[]
  #   disabledKeys: string[]
  #   readonlyKeys: string[]
  #   selectionMode: 'single' | 'toggle' | 'multi' | 'none'
  #   checkMode: 'none' | 'checkbox' | 'triState'
  # }

  # FORM
  # interface TreeViewSelectionForm {
  #   targetKey: string
  #   selectedKeys: string[]
  #   checkedKeys: string[]
  #   allowCheckChildren: boolean
  #   allowCheckParents: boolean
  # }

  # REPRESENTATION
  # interface TreeSelectionRepresentation {
  #   key: string
  #   selected: boolean
  #   checked: boolean
  #   indeterminate: boolean
  #   disabled: boolean
  #   readonly: boolean
  # }

  # ACTIONS
  # type TreeViewSelectionAction =
  #   | { type: 'SelectNode'; key: string }
  #   | { type: 'ToggleNodeSelection'; key: string }
  #   | { type: 'SetSelectedKeys'; keys: string[] }
  #   | { type: 'ToggleNodeChecked'; key: string }
  #   | { type: 'SetCheckedKeys'; keys: string[] }
  #   | { type: 'SetSelectionMode'; mode: TreeViewSelectionState['selectionMode'] }

  # REDUCER TRANSITIONS
  # SelectNode -> selectedKey set and selectedKeys replaced in single mode
  # ToggleNodeSelection -> key added or removed in toggle and multi modes
  # SetSelectedKeys -> selectedKeys replaced from controlled binding
  # ToggleNodeChecked -> checkedKeys recalculated, parent and child propagation applied
  # SetCheckedKeys -> checkedKeys replaced from controlled binding, indeterminate recalculated
  # SetSelectionMode -> incompatible selection state normalized

  # EFFECTS
  # on SelectNode -> SelectedValueChanged and Change callbacks fire
  # on ToggleNodeSelection -> SelectedValuesChanged callbacks fire
  # on ToggleNodeChecked -> CheckedValuesChanged callbacks fire
  # on SetCheckedKeys -> no duplicate callbacks for controlled echo values

  # EVENTS
  # type TreeViewSelectionEvent = 'NodeSelected' | 'SelectedKeysChanged' | 'CheckedKeysChanged' | 'SelectionRejected'

  @state:TreeViewSelectionState
  @SC:01
  Scenario: SC01 - Single selection replaces the selected node
    @precondition:SingleSelectionMode
    Given TreeViewSelectionState.selectionMode is "single"
    @precondition:NoSelectedNode
    And TreeViewSelectionState.selectedKey is null
    @action:SelectNode
    @param:key="orders"
    When the user selects node "orders"
    @postcondition:SingleNodeSelected
    Then selectedKey should be "orders" @UAC:01
    And selectedKeys should contain only "orders" @UAC:02
    And the selected node should expose aria-selected true @UAC:03
    @postevent:NodeSelected
    And NodeSelected should include key "orders" @UAC:04

  @state:TreeViewSelectionState
  @SC:02
  Scenario: SC02 - Toggle selection can clear the active node
    @precondition:ToggleSelectionMode
    Given TreeViewSelectionState.selectionMode is "toggle"
    And TreeViewSelectionState.selectedKey is "orders"
    @action:ToggleNodeSelection
    @param:key="orders"
    When the user selects node "orders" again
    @postcondition:SelectionCleared
    Then selectedKey should be null @UAC:05
    And selectedKeys should be empty @UAC:06
    @postevent:SelectedKeysChanged
    And SelectedKeysChanged should include an empty key set @UAC:07

  @state:TreeViewSelectionState
  @SC:03
  Scenario: SC03 - Multi selection preserves existing selections
    @precondition:MultiSelectionMode
    Given TreeViewSelectionState.selectionMode is "multi"
    And selectedKeys contains "orders"
    @action:ToggleNodeSelection
    @param:key="customers"
    When the user toggles node "customers"
    @postcondition:MultipleNodesSelected
    Then selectedKeys should contain "orders" and "customers" @UAC:08
    And both nodes should expose selected styling @UAC:09
    @postevent:SelectedKeysChanged
    And SelectedKeysChanged should include both selected keys @UAC:10

  @state:TreeViewSelectionState
  @SC:04
  Scenario: SC04 - Checkbox checking cascades to children
    @precondition:CheckboxMode
    Given TreeViewSelectionState.checkMode is "checkbox"
    And node "Categories" has children "Social" and "Updates"
    @action:ToggleNodeChecked
    @param:key="Categories"
    When the user checks "Categories"
    @postcondition:ChildrenChecked
    Then checkedKeys should contain "Categories", "Social", and "Updates" @UAC:11
    And CheckedValuesChanged should receive all checked descendants @UAC:12
    @postevent:CheckedKeysChanged
    And CheckedKeysChanged should include key "Categories" @UAC:13

  @state:TreeViewSelectionState
  @SC:05
  Scenario: SC05 - Tri-state parent becomes indeterminate when only some children are checked
    @precondition:TriStateMode
    Given TreeViewSelectionState.checkMode is "triState"
    And node "Categories" has children "Social" and "Updates"
    @action:ToggleNodeChecked
    @param:key="Social"
    When the user checks child "Social"
    @postcondition:ParentIndeterminate
    Then checkedKeys should contain "Social" @UAC:14
    And indeterminateKeys should contain "Categories" @UAC:15
    And the parent checkbox should expose aria-checked "mixed" @UAC:16
    @postevent:CheckedKeysChanged
    And CheckedKeysChanged should include indeterminate key "Categories" @UAC:17

  @state:TreeViewSelectionState
  @SC:06
  Scenario: SC06 - Disabled and readonly nodes reject selection changes
    @precondition:NodeNotInteractive
    Given disabledKeys contains "locked"
    And readonlyKeys contains "archive"
    @action:SelectNode
    @param:key="locked"
    When the user selects node "locked"
    @postcondition:DisabledSelectionRejected
    Then selectedKeys should remain unchanged @UAC:18
    And the disabled node should expose aria-disabled true @UAC:19
    @postevent:SelectionRejected
    And SelectionRejected should include reason "disabled" @UAC:20

