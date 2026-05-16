@US:10
Feature: SPEC1062.US10 - State, Export, and Accessibility
  As a grid user and application developer
  I want saved settings, export-ready state, keyboard support, and accessible semantics
  So that DataGrid can match Radzen DataGrid operational polish while staying testable.

  Background:
    Given the component library LowlandTech.Components.DataGrid
    And the Radzen parity reference includes DataGridSaveSettings, DataGridSaveSettingsLoadData, DataGridCustomHeaderWithColumnPicker, DataGridDensity, DataGridGridLines, DataGridPagerApi, and Radzen DataGrid accessibility behavior

  # STATE
  # interface DataGridStateExportAccessibilityState {
  #   settings: DataGridSettingsRepresentation
  #   exportRequest: DataGridExportRequest | null
  #   focusedCell: { rowKey: string; columnKey: string } | null
  #   liveRegionMessage: string | null
  #   density: 'compact' | 'default' | 'comfortable'
  #   gridLines: 'none' | 'horizontal' | 'vertical' | 'both'
  # }
  #
  # FORM
  # interface DataGridSettingsForm {
  #   columns: DataGridColumnRepresentation[]
  #   filters: DataGridFilterDescriptor[]
  #   sorts: DataGridSortDescriptor[]
  #   groups: DataGridGroupDescriptor[]
  #   pageSize: number
  # }
  #
  # REPRESENTATION
  # interface DataGridSettingsRepresentation {
  #   columnOrder: string[]
  #   columnWidths: Record<string, string>
  #   visibleColumnKeys: string[]
  #   filters: DataGridFilterDescriptor[]
  #   sorts: DataGridSortDescriptor[]
  #   groups: DataGridGroupDescriptor[]
  #   pageSize: number
  #   density: string
  # }
  #
  # interface DataGridExportRequest {
  #   format: 'csv' | 'xlsx'
  #   visibleColumnsOnly: boolean
  #   includeFilters: boolean
  #   includeSorts: boolean
  #   rows: 'currentPage' | 'allFiltered'
  # }
  #
  # ACTIONS
  # type DataGridStateExportAccessibilityAction =
  #   | { type: 'SaveSettings'; settings: DataGridSettingsRepresentation }
  #   | { type: 'LoadSettings'; settings: DataGridSettingsRepresentation }
  #   | { type: 'ResetSettings' }
  #   | { type: 'CreateExportRequest'; request: DataGridExportRequest }
  #   | { type: 'MoveFocus'; direction: 'up' | 'down' | 'left' | 'right' }
  #   | { type: 'AnnounceGridChange'; message: string }
  #
  # REDUCER TRANSITIONS
  # SaveSettings -> settings captured from current grid state
  # LoadSettings -> current grid state restored from settings
  # ResetSettings -> settings returned to defaults
  # CreateExportRequest -> exportRequest populated
  # MoveFocus -> focusedCell updated within visible cells
  # AnnounceGridChange -> liveRegionMessage updated
  #
  # EFFECTS
  # on SaveSettings -> SettingsChanged callback or storage adapter
  # on CreateExportRequest -> Export callback receives request and descriptors
  #
  # EVENTS
  # type DataGridStateExportAccessibilityEvent = 'SettingsSaved' | 'SettingsLoaded' | 'SettingsReset' | 'ExportRequested' | 'GridFocusMoved' | 'GridChangeAnnounced'

  @state:DataGridStateExportAccessibilityState
  @SC:01
  Scenario: SC01 - Save settings captures user-customized grid state
    @precondition:GridCustomized
    Given columns have been reordered and resized
    @precondition:OperationsActive
    And filters, sorts, groups, pageSize, density, and visible columns are active

    @action:SaveSettings
    When the grid saves settings
    @param:settings
    And settings are captured from current grid state

    @postcondition:SettingsCaptured
    Then settings should contain column order @UAC:01
    And settings should contain column widths @UAC:02
    And settings should contain visible column keys @UAC:03
    And settings should contain filters, sorts, groups, pageSize, and density @UAC:04
    @postevent:SettingsSaved
    And SettingsSaved should emit a serializable settings payload @UAC:05

  @state:DataGridStateExportAccessibilityState
  @SC:02
  Scenario: SC02 - Load settings restores grid state before first data request
    @precondition:SavedSettingsAvailable
    Given saved settings contain filters, sorts, hidden columns, and pageSize 50
    @precondition:LoadDataModeEnabled
    And the grid loads data remotely

    @action:LoadSettings
    When the grid initializes with saved settings
    @param:settings
    And settings are applied before first LoadData request

    @postcondition:SettingsRestored
    Then hidden columns from settings should not render @UAC:06
    And filters and sorts from settings should be active @UAC:07
    And pageSize should be 50 @UAC:08
    And the first LoadData request should include restored filters and sorts @UAC:09
    @postevent:SettingsLoaded
    And SettingsLoaded should occur before LoadDataRequested @UAC:10

  @state:DataGridStateExportAccessibilityState
  @SC:03
  Scenario: SC03 - Reset settings returns grid to declarative defaults
    @precondition:SettingsLoaded
    Given settings currently hide the Notes column and sort by Country
    @precondition:DefaultsKnown
    And declarative grid defaults show Notes and have no sort

    @action:ResetSettings
    When the user resets grid settings

    @postcondition:DefaultsRestored
    Then Notes column should become visible @UAC:11
    And active sorts should be cleared @UAC:12
    And active filters should be cleared @UAC:13
    And pageSize should return to the declarative default @UAC:14
    @postevent:SettingsReset
    And SettingsReset should include the restored default settings @UAC:15

  @state:DataGridStateExportAccessibilityState
  @SC:04
  Scenario: SC04 - Export request includes visible columns and current descriptors
    @precondition:ExportEnabled
    Given visible columns are Customer, Country, and Freight
    @precondition:OperationsActive
    And filters and sorts are active

    @action:CreateExportRequest
    When the user exports all filtered rows as CSV
    @param:request
    And request has format "csv", visibleColumnsOnly true, includeFilters true, includeSorts true, and rows "allFiltered"

    @postcondition:ExportRequestBuilt
    Then exportRequest should include only visible columns @UAC:16
    And exportRequest should include active filter descriptors @UAC:17
    And exportRequest should include active sort descriptors @UAC:18
    And exportRequest should request all filtered rows rather than only the current page @UAC:19
    @postevent:ExportRequested
    And ExportRequested should include format "csv" @UAC:20

  @state:DataGridStateExportAccessibilityState
  @SC:05
  Scenario: SC05 - Keyboard navigation moves focus between cells
    @precondition:KeyboardNavigationEnabled
    Given focusedCell is row "employee-1" and column "FirstName"
    @precondition:AdjacentCellExists
    And the next visible column is "LastName"

    @action:MoveFocus
    When the user presses ArrowRight
    @param:direction
    And direction is "right"

    @postcondition:FocusMoved
    Then focusedCell should be row "employee-1" and column "LastName" @UAC:21
    And the LastName cell should have tabindex="0" @UAC:22
    And the previous FirstName cell should have tabindex="-1" @UAC:23
    @postevent:GridFocusMoved
    And GridFocusMoved should include previous and next cell coordinates @UAC:24

  @state:DataGridStateExportAccessibilityState
  @SC:06
  Scenario: SC06 - Enter and Space activate the focused cell behavior
    @precondition:FocusedCellInteractive
    Given focusedCell is row "employee-2" and column "Selected"
    @precondition:SelectionColumnFocused
    And the focused cell contains a row selection checkbox

    @action:MoveFocus
    When the user presses Space
    @param:direction
    And direction is "activate"

    @postcondition:FocusedCellActivated
    Then row "employee-2" should toggle selection @UAC:25
    And focus should remain on the selection cell @UAC:26
    And the selection checkbox accessible state should update @UAC:27

  @state:DataGridStateExportAccessibilityState
  @SC:07
  Scenario: SC07 - Grid announces sorting, filtering, paging, and selection changes
    @precondition:LiveRegionConfigured
    Given the grid has an aria-live region
    @precondition:GridChangeOccurs
    And the user applies a filter that reduces results to 12 rows

    @action:AnnounceGridChange
    When the filter is applied
    @param:message
    And message is "12 rows match the current filters"

    @postcondition:LiveRegionUpdated
    Then liveRegionMessage should be "12 rows match the current filters" @UAC:28
    And the live region should use polite announcement behavior @UAC:29
    And visual row rendering should not duplicate the announcement text @UAC:30
    @postevent:GridChangeAnnounced
    And GridChangeAnnounced should include the message @UAC:31

  @state:DataGridStateExportAccessibilityState
  @SC:08
  Scenario: SC08 - Accessible semantics describe grid, rows, columns, and sort state
    @precondition:GridRendered
    Given the grid has columns Customer, Country, and Freight
    @precondition:SortActive
    And Customer is sorted ascending

    @action:AnnounceGridChange
    When the grid renders accessible attributes
    @param:message
    And message is "Customer sorted ascending"

    @postcondition:AccessibleSemanticsRendered
    Then the table or grid root should expose an accessible grid/table role @UAC:32
    And header cells should expose column header semantics @UAC:33
    And Customer header should expose ascending sort state @UAC:34
    And rows should expose row semantics and selectable state when selection is enabled @UAC:35
