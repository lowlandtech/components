@US:07
Feature: SPEC1062.US07 - Editing and Validation
  As a grid user
  I want inline editing, incell editing, validation, insertion, cancellation, and command columns
  So that DataGrid can match Radzen DataGrid CRUD workflows.

  Background:
    Given the component library LowlandTech.Components.DataGrid
    And the Radzen parity reference includes DataGridInLineEdit and DataGridInCellEdit

  # STATE
  # interface DataGridEditingState {
  #   rows: DataGridRowRepresentation[]
  #   editMode: 'none' | 'row' | 'cell'
  #   editingRowKey: string | null
  #   editingCell: { rowKey: string; columnKey: string } | null
  #   form: Record<string, unknown>
  #   validationErrors: Record<string, string[]>
  #   dirty: boolean
  # }
  #
  # FORM
  # interface DataGridEditForm {
  #   rowKey: string | null
  #   columnKey: string | null
  #   values: Record<string, unknown>
  #   isNew: boolean
  # }
  #
  # REPRESENTATION
  # interface DataGridEditCommandRepresentation {
  #   rowKey: string
  #   mode: 'view' | 'edit' | 'insert'
  #   commands: string[]
  # }
  #
  # ACTIONS
  # type DataGridEditingAction =
  #   | { type: 'BeginRowEdit'; rowKey: string }
  #   | { type: 'BeginCellEdit'; rowKey: string; columnKey: string }
  #   | { type: 'UpdateEditField'; field: string; value: unknown }
  #   | { type: 'SaveEdit'; rowKey: string | null }
  #   | { type: 'CancelEdit'; rowKey: string | null }
  #   | { type: 'InsertRow'; values: Record<string, unknown> }
  #   | { type: 'DeleteRow'; rowKey: string }
  #
  # REDUCER TRANSITIONS
  # BeginRowEdit -> editMode: row, editingRowKey, form initialized
  # BeginCellEdit -> editMode: cell, editingCell, form initialized
  # UpdateEditField -> form[field]: value, dirty: true
  # SaveEdit -> validate, update or insert row, clear edit state on success
  # CancelEdit -> clear edit state and discard form
  # InsertRow -> add optimistic new row in insert mode
  # DeleteRow -> remove row after command confirmation
  #
  # EFFECTS
  # on SaveEdit -> RowUpdate or RowCreate callback
  # on DeleteRow -> RowDelete callback
  #
  # EVENTS
  # type DataGridEditingEvent = 'RowEditStarted' | 'CellEditStarted' | 'EditFieldChanged' | 'RowSaved' | 'EditCanceled' | 'RowInserted' | 'RowDeleted' | 'ValidationFailed'

  @state:DataGridEditingState
  @SC:01
  Scenario: SC01 - Inline row edit swaps display cells for edit templates
    @precondition:RowEditingEnabled
    Given row "employee-1" is rendered in view mode
    @precondition:EditTemplatesConfigured
    And editable columns have EditTemplate content

    @action:BeginRowEdit
    When the user clicks Edit for row "employee-1"
    @param:rowKey
    And rowKey is "employee-1"

    @postcondition:RowEditModeActive
    Then editingRowKey should be "employee-1" @UAC:01
    And row "employee-1" should render edit templates @UAC:02
    And row "employee-1" should render Save and Cancel commands @UAC:03
    @postevent:RowEditStarted
    And RowEditStarted should include the original row item @UAC:04

  @state:DataGridEditingState
  @SC:02
  Scenario: SC02 - Inline row save validates and commits changes
    @precondition:RowEditModeActive
    Given editingRowKey is "employee-1"
    @precondition:ValidFormValues
    And form contains FirstName "Ada" and Title "Principal Engineer"

    @action:SaveEdit
    When the user clicks Save for row "employee-1"
    @param:rowKey
    And rowKey is "employee-1"

    @postcondition:RowUpdated
    Then row "employee-1" should contain Title "Principal Engineer" @UAC:05
    And editMode should be "none" @UAC:06
    And validationErrors should be empty @UAC:07
    @postevent:RowSaved
    And RowUpdate callback should receive the updated item @UAC:08

  @state:DataGridEditingState
  @SC:03
  Scenario: SC03 - Inline row cancel restores original values
    @precondition:RowEditModeActive
    Given editingRowKey is "employee-1"
    @precondition:DirtyForm
    And form contains unsaved Title "Temporary"

    @action:CancelEdit
    When the user clicks Cancel for row "employee-1"
    @param:rowKey
    And rowKey is "employee-1"

    @postcondition:OriginalValuesRestored
    Then row "employee-1" should not contain Title "Temporary" @UAC:09
    And editMode should be "none" @UAC:10
    And dirty should be false @UAC:11
    @postevent:EditCanceled
    And EditCanceled should include rowKey "employee-1" @UAC:12

  @state:DataGridEditingState
  @SC:04
  Scenario: SC04 - Incell edit commits one cell without replacing the whole row UI
    @precondition:CellEditingEnabled
    Given editMode is "none"
    @precondition:EditableCellExists
    And row "employee-2" has editable column "City"

    @action:BeginCellEdit
    When the user activates the City cell in row "employee-2"
    @param:rowKey
    And rowKey is "employee-2"
    @param:columnKey
    And columnKey is "City"

    @postcondition:CellEditModeActive
    Then editingCell should be row "employee-2" and column "City" @UAC:13
    And only the City cell should render its edit template @UAC:14
    And other cells in row "employee-2" should remain in display mode @UAC:15
    @postevent:CellEditStarted
    And CellEditStarted should include row and column context @UAC:16

  @state:DataGridEditingState
  @SC:05
  Scenario: SC05 - Validation failure keeps row in edit mode
    @precondition:RowEditModeActive
    Given editingRowKey is "employee-3"
    @precondition:InvalidFormValues
    And form contains empty required FirstName

    @action:SaveEdit
    When the user clicks Save
    @param:rowKey
    And rowKey is "employee-3"

    @postcondition:ValidationErrorDisplayed
    Then editingRowKey should remain "employee-3" @UAC:17
    And validationErrors should contain FirstName required @UAC:18
    And RowUpdate callback should not be invoked @UAC:19
    @postevent:ValidationFailed
    And ValidationFailed should include field "FirstName" @UAC:20

  @state:DataGridEditingState
  @SC:06
  Scenario: SC06 - Insert row creates a new editable item
    @precondition:InsertEnabled
    Given rows contain no row with key "new"
    @precondition:DefaultValuesAvailable
    And the insert command has default values

    @action:InsertRow
    When the user clicks Add row
    @param:values
    And values contain default fields for a new item

    @postcondition:InsertModeActive
    Then a new row should render in insert mode @UAC:21
    And the new row should render edit templates @UAC:22
    And the new row should render Insert and Cancel commands @UAC:23
    @postevent:RowInserted
    And RowInserted should not be committed until SaveEdit succeeds @UAC:24

  @state:DataGridEditingState
  @SC:07
  Scenario: SC07 - Delete command emits delete callback with row context
    @precondition:DeleteCommandEnabled
    Given row "employee-4" is rendered
    @precondition:DeleteConfirmed
    And destructive action confirmation has completed

    @action:DeleteRow
    When the user clicks Delete for row "employee-4"
    @param:rowKey
    And rowKey is "employee-4"

    @postcondition:RowDeleteRequested
    Then RowDelete callback should receive row "employee-4" @UAC:25
    And the row should be removed only if the consumer updates Items or confirms optimistic deletion @UAC:26
    @postevent:RowDeleted
    And RowDeleted should include rowKey "employee-4" @UAC:27

  @state:DataGridEditingState
  @SC:08
  Scenario: SC08 - Command column changes commands based on edit state
    @precondition:CommandColumnConfigured
    Given row "employee-1" is in view mode
    @precondition:RowEditModeInactive
    And editMode is "none"

    @action:BeginRowEdit
    When the user enters edit mode for row "employee-1"
    @param:rowKey
    And rowKey is "employee-1"

    @postcondition:CommandsSwapped
    Then the command column should hide Edit and Delete for row "employee-1" @UAC:28
    And the command column should show Save and Cancel for row "employee-1" @UAC:29
    And command buttons should preserve accessible names @UAC:30
