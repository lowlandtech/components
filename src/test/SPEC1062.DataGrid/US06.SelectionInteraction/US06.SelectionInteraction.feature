@US:06
Feature: SPEC1062.US06 - Selection and Interaction
  As a grid user
  I want row selection, cell selection, context menus, and drag interactions
  So that DataGrid can match Radzen DataGrid interaction workflows.

  Background:
    Given the component library LowlandTech.Components.DataGrid
    And the Radzen parity reference includes DataGridSingleSelection, DataGridMultipleSelection, DataGridCellSelect, DataGridCellContextMenu, DataGridRowReorder, DataGridRowDragBetween, and DataGridRowDragScheduler

  # STATE
  # interface DataGridInteractionState {
  #   rows: DataGridRowRepresentation[]
  #   selectedRowKeys: string[]
  #   selectedCell: { rowKey: string; columnKey: string } | null
  #   contextMenuTarget: { rowKey: string; columnKey: string } | null
  #   dragSourceRowKey: string | null
  #   selectionMode: 'none' | 'single' | 'multiple'
  # }
  #
  # FORM
  # interface DataGridInteractionForm {
  #   rowKey: string
  #   columnKey: string | null
  #   ctrlKey: boolean
  #   shiftKey: boolean
  #   targetIndex: number | null
  # }
  #
  # REPRESENTATION
  # interface DataGridSelectionRepresentation {
  #   selectedRowKeys: string[]
  #   selectedItems: DataGridRowRepresentation[]
  #   selectedCell: { rowKey: string; columnKey: string } | null
  # }
  #
  # ACTIONS
  # type DataGridInteractionAction =
  #   | { type: 'SelectRow'; rowKey: string; ctrlKey: boolean; shiftKey: boolean }
  #   | { type: 'SelectCell'; rowKey: string; columnKey: string }
  #   | { type: 'OpenCellContextMenu'; rowKey: string; columnKey: string }
  #   | { type: 'StartRowDrag'; rowKey: string }
  #   | { type: 'DropRow'; rowKey: string; targetIndex: number }
  #   | { type: 'DropRowExternal'; rowKey: string; targetId: string }
  #
  # REDUCER TRANSITIONS
  # SelectRow -> selectedRowKeys according to selectionMode, ctrlKey, shiftKey
  # SelectCell -> selectedCell updated
  # OpenCellContextMenu -> contextMenuTarget updated
  # StartRowDrag -> dragSourceRowKey updated
  # DropRow -> rows reordered, dragSourceRowKey: null
  # DropRowExternal -> external drop event emitted, dragSourceRowKey: null
  #
  # EFFECTS
  # on SelectRow -> SelectedItemChanged or SelectedItemsChanged
  # on SelectCell -> CellSelect callback
  # on OpenCellContextMenu -> CellContextMenu callback
  # on DropRowExternal -> RowDrop callback
  #
  # EVENTS
  # type DataGridInteractionEvent = 'RowSelectionChanged' | 'CellSelected' | 'CellContextMenuOpened' | 'RowReordered' | 'RowDropped'

  @state:DataGridInteractionState
  @SC:01
  Scenario: SC01 - Single row selection replaces the previous selection
    @precondition:SingleSelectionEnabled
    Given DataGridInteractionState.selectionMode is "single"
    @precondition:ExistingSelection
    And selectedRowKeys contains "employee-1"

    @action:SelectRow
    When the user clicks row "employee-2"
    @param:rowKey
    And rowKey is "employee-2"
    @param:ctrlKey
    And ctrlKey is false

    @postcondition:SingleSelectionUpdated
    Then selectedRowKeys should contain only "employee-2" @UAC:01
    And row "employee-1" should render data-selected false @UAC:02
    And row "employee-2" should render data-selected true @UAC:03
    @postevent:RowSelectionChanged
    And SelectedItemChanged should receive row "employee-2" @UAC:04

  @state:DataGridInteractionState
  @SC:02
  Scenario: SC02 - Multiple row selection toggles rows independently
    @precondition:MultipleSelectionEnabled
    Given DataGridInteractionState.selectionMode is "multiple"
    @precondition:NoSelection
    And selectedRowKeys is empty

    @action:SelectRow
    When the user selects rows "employee-1" and "employee-3"
    @param:rowKey
    And rowKey is "employee-3"
    @param:ctrlKey
    And ctrlKey is true

    @postcondition:MultipleSelectionUpdated
    Then selectedRowKeys should contain "employee-1" and "employee-3" @UAC:05
    And each selected row checkbox should be checked @UAC:06
    And unselected row checkboxes should remain unchecked @UAC:07
    @postevent:RowSelectionChanged
    And SelectedItemsChanged should receive both selected items @UAC:08

  @state:DataGridInteractionState
  @SC:03
  Scenario: SC03 - Header checkbox selects and clears all visible rows
    @precondition:MultipleSelectionEnabled
    Given DataGridInteractionState.selectionMode is "multiple"
    @precondition:VisibleRowsKnown
    And the current page has rows employee-1 through employee-10

    @action:SelectRow
    When the user clicks the header selection checkbox
    @param:rowKey
    And rowKey is "*visible"

    @postcondition:VisibleRowsSelected
    Then selectedRowKeys should contain every visible row key @UAC:09
    And the header checkbox should be checked @UAC:10
    And clicking the header checkbox again should clear visible row selections @UAC:11

  @state:DataGridInteractionState
  @SC:04
  Scenario: SC04 - Cell selection identifies row and column
    @precondition:CellSelectionEnabled
    Given selectedCell is null
    @precondition:CellExists
    And row "employee-2" has column "City"

    @action:SelectCell
    When the user clicks the City cell in row "employee-2"
    @param:rowKey
    And rowKey is "employee-2"
    @param:columnKey
    And columnKey is "City"

    @postcondition:CellSelected
    Then selectedCell should be row "employee-2" and column "City" @UAC:12
    And the selected cell should render data-selected="true" @UAC:13
    @postevent:CellSelected
    And CellSelect callback should receive row and column context @UAC:14

  @state:DataGridInteractionState
  @SC:05
  Scenario: SC05 - Cell context menu receives cell context without changing selection
    @precondition:ContextMenuEnabled
    Given selectedRowKeys contains "employee-1"
    @precondition:CellExists
    And row "employee-3" has column "Title"

    @action:OpenCellContextMenu
    When the user opens the context menu on employee-3 Title
    @param:rowKey
    And rowKey is "employee-3"
    @param:columnKey
    And columnKey is "Title"

    @postcondition:ContextMenuTargetStored
    Then contextMenuTarget should be row "employee-3" and column "Title" @UAC:15
    And selectedRowKeys should remain "employee-1" @UAC:16
    @postevent:CellContextMenuOpened
    And CellContextMenu callback should receive mouse coordinates and cell context @UAC:17

  @state:DataGridInteractionState
  @SC:06
  Scenario: SC06 - Row reorder moves a row within the same grid
    @precondition:RowReorderEnabled
    Given rows are ordered employee-1, employee-2, employee-3
    @precondition:DragStarted
    And dragSourceRowKey is "employee-3"

    @action:DropRow
    When the user drops employee-3 at index 0
    @param:rowKey
    And rowKey is "employee-3"
    @param:targetIndex
    And targetIndex is 0

    @postcondition:RowsReordered
    Then rows should be ordered employee-3, employee-1, employee-2 @UAC:18
    And dragSourceRowKey should be null @UAC:19
    @postevent:RowReordered
    And RowReordered should include source and target indexes @UAC:20

  @state:DataGridInteractionState
  @SC:07
  Scenario: SC07 - Row drag between grids emits external drop payload
    @precondition:ExternalRowDropEnabled
    Given source grid contains row "order-1"
    @precondition:TargetGridAvailable
    And target grid id is "selected-orders"

    @action:DropRowExternal
    When the user drops row "order-1" onto target grid "selected-orders"
    @param:rowKey
    And rowKey is "order-1"
    @param:targetId
    And targetId is "selected-orders"

    @postcondition:ExternalDropEmitted
    Then source grid should emit RowDrop with row "order-1" @UAC:21
    And RowDrop should include target id "selected-orders" @UAC:22
    And the source rows should not mutate unless the consumer accepts the drop @UAC:23
    @postevent:RowDropped
    And RowDropped should include source and target grid identifiers @UAC:24

  @state:DataGridInteractionState
  @SC:08
  Scenario: SC08 - Scheduler drop target receives row payload
    @precondition:SchedulerDropTargetConfigured
    Given a scheduler target accepts grid rows
    @precondition:GridRowDragging
    And dragSourceRowKey is "task-7"

    @action:DropRowExternal
    When the user drops row "task-7" onto scheduler slot "2026-05-16T09:00"
    @param:rowKey
    And rowKey is "task-7"
    @param:targetId
    And targetId is "scheduler:2026-05-16T09:00"

    @postcondition:SchedulerDropPayloadEmitted
    Then RowDrop should include row "task-7" @UAC:25
    And RowDrop should include scheduler slot metadata @UAC:26
    And DataGrid should clear the drag state after the drop @UAC:27
