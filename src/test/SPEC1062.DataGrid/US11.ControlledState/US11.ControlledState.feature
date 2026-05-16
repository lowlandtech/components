@SPEC:1062
@US:11
Feature: SPEC1062.US11 - Controlled State and Stable Row Identity
  As an application author using DataGrid in a stateful Blazor page
  I want paging, sorting, filtering, expansion, and selection state to be externally observable and keyed by stable row identity
  So that the grid can participate in Flux-style state, remote data loading, saved settings, and item refreshes without losing user context

  Background:
    Given the LowlandTech DataGrid is rendered with typed row items
    And the grid may be used with local items or remote LoadData-style data flows
    And state changes must be traceable through callbacks, descriptors, and stable row keys

  # STATE
  # interface DataGridControlledState {
  #   pageIndex: number
  #   pageSize: number
  #   sortDescriptors: DataGridSortDescriptor[]
  #   filterDescriptors: DataGridFilterDescriptor[]
  #   selectedRowKeys: string[]
  #   expandedRowKeys: string[]
  #   totalItems: number
  #   rows: DataGridRowRepresentation[]
  # }

  # FORM
  # interface DataGridControlledForm {
  #   pendingPageIndex: number
  #   pendingSort: DataGridSortDescriptor[]
  #   pendingFilters: DataGridFilterDescriptor[]
  #   keyField: string | null
  # }

  # REPRESENTATION
  # interface DataGridRowRepresentation {
  #   key: string
  #   item: unknown
  # }
  # interface DataGridStateChangeRepresentation {
  #   pageIndex: number
  #   pageSize: number
  #   sortDescriptors: DataGridSortDescriptor[]
  #   filterDescriptors: DataGridFilterDescriptor[]
  #   selectedRowKeys: string[]
  #   expandedRowKeys: string[]
  # }

  # ACTIONS
  # type DataGridControlledAction =
  #   | { type: 'ChangePage'; pageIndex: number }
  #   | { type: 'ChangeSort'; descriptors: DataGridSortDescriptor[] }
  #   | { type: 'ChangeFilter'; descriptors: DataGridFilterDescriptor[] }
  #   | { type: 'RefreshRows'; rows: DataGridRowRepresentation[] }
  #   | { type: 'ToggleSelection'; rowKey: string }
  #   | { type: 'ToggleExpansion'; rowKey: string }
  #   | { type: 'ClampPage'; totalItems: number }

  # REDUCER TRANSITIONS
  # ChangePage -> pageIndex updated and state change emitted
  # ChangeSort -> sortDescriptors replaced, pageIndex reset to 0, state change emitted
  # ChangeFilter -> filterDescriptors replaced, pageIndex reset to 0, state change emitted
  # RefreshRows -> rows replaced while selectedRowKeys and expandedRowKeys are reconciled by key
  # ToggleSelection -> selectedRowKeys toggled by stable key
  # ToggleExpansion -> expandedRowKeys toggled by stable key
  # ClampPage -> pageIndex clamped to the highest available page and state change emitted when changed

  # EFFECTS
  # on ChangePage -> PageIndexChanged callback receives the new page index
  # on ChangeSort -> SortChanged callback receives descriptors before remote data is requested
  # on ChangeFilter -> FilterChanged callback receives descriptors before remote data is requested
  # on any state-changing action -> StateChanged callback receives a serializable state representation

  # EVENTS
  # type DataGridControlledEvent = 'PageIndexChanged' | 'SortChanged' | 'FilterChanged' | 'StateChanged' | 'RowsReconciled' | 'PageClamped'

  @state:DataGridControlledState
  @SC:01
  Scenario: SC01 - Sorting resets a controlled page and notifies the owner
    @precondition:PageTwoActive
    Given DataGridControlledState.pageIndex is 2
    @precondition:SortingEnabled
    And the user can sort the "Name" column
    @action:ChangeSort
    @param:descriptors=[{ property: "Name", direction: "Ascending", order: 0 }]
    When the user sorts by "Name"
    @postcondition:PageReset
    Then DataGridControlledState.pageIndex should be 0 @UAC:01
    And PageIndexChanged should receive 0 @UAC:02
    @postcondition:SortDescriptorsUpdated
    And DataGridControlledState.sortDescriptors should contain "Name" ascending @UAC:03
    @postevent:SortChanged
    And SortChanged should emit the updated descriptor list @UAC:04

  @state:DataGridControlledState
  @SC:02
  Scenario: SC02 - Filtering resets a controlled page and notifies the owner
    @precondition:PageThreeActive
    Given DataGridControlledState.pageIndex is 3
    @precondition:FilteringEnabled
    And the "Role" column can be filtered
    @action:ChangeFilter
    @param:descriptors=[{ property: "Role", operator: "Contains", value: "Admin" }]
    When the user applies a filter for "Admin"
    @postcondition:PageReset
    Then DataGridControlledState.pageIndex should be 0 @UAC:05
    And PageIndexChanged should receive 0 @UAC:06
    @postcondition:FilterDescriptorsUpdated
    And DataGridControlledState.filterDescriptors should contain the Role filter @UAC:07
    @postevent:FilterChanged
    And FilterChanged should emit the updated descriptor list @UAC:08

  @state:DataGridControlledState
  @SC:03
  Scenario: SC03 - Page clamping emits a callback when filtered rows shrink
    @precondition:PageBeyondFilteredResult
    Given DataGridControlledState.pageIndex is 4
    And DataGridControlledState.pageSize is 10
    And the filtered result contains 12 items
    @action:ClampPage
    @param:totalItems=12
    When the grid reconciles the current page against the filtered count
    @postcondition:PageClamped
    Then DataGridControlledState.pageIndex should be 1 @UAC:09
    And PageIndexChanged should receive 1 @UAC:10
    @postevent:PageClamped
    And PageClamped should include oldPageIndex 4 and newPageIndex 1 @UAC:11

  @state:DataGridControlledState
  @SC:04
  Scenario: SC04 - Stable row keys preserve selection after item instances refresh
    @precondition:StableKeySelectorConfigured
    Given the grid resolves row keys from the "Id" property
    @precondition:SelectionActive
    And DataGridControlledState.selectedRowKeys contains "employee-2"
    @action:RefreshRows
    @param:rows=[{ key: "employee-1" }, { key: "employee-2" }, { key: "employee-3" }]
    When the row collection is replaced with new object instances for the same keys
    @postcondition:SelectionReconciled
    Then row "employee-2" should remain selected @UAC:12
    And no selection should depend on object reference equality @UAC:13
    @postevent:RowsReconciled
    And RowsReconciled should include retainedSelectedRowKeys ["employee-2"] @UAC:14

  @state:DataGridControlledState
  @SC:05
  Scenario: SC05 - Stable row keys preserve detail expansion after sorting
    @precondition:DetailTemplateConfigured
    Given DataGridControlledState.expandedRowKeys contains "employee-3"
    @precondition:SortingEnabled
    And sorting will move "employee-3" to a different visual index
    @action:ChangeSort
    @param:descriptors=[{ property: "Name", direction: "Descending", order: 0 }]
    When the user sorts by "Name" descending
    @postcondition:ExpansionReconciled
    Then row "employee-3" should remain expanded @UAC:15
    And the expanded detail should follow the row key, not the row index @UAC:16
    @postevent:StateChanged
    And StateChanged should include expandedRowKeys ["employee-3"] @UAC:17

  @state:DataGridControlledState
  @SC:06
  Scenario: SC06 - Full grid state is serializable for saved settings and remote loading
    @precondition:StateCallbacksEnabled
    Given sorting, filtering, paging, selection, and expansion are active
    @action:ChangePage
    @param:pageIndex=1
    When the user navigates to page 1
    @postcondition:SerializableStateBuilt
    Then StateChanged should include pageIndex 1 @UAC:18
    And StateChanged should include sort descriptors @UAC:19
    And StateChanged should include filter descriptors @UAC:20
    And StateChanged should include selected row keys @UAC:21
    And StateChanged should include expanded row keys @UAC:22
    @postevent:StateChanged
    And the state payload should not contain component instances or RenderFragment values @UAC:23
