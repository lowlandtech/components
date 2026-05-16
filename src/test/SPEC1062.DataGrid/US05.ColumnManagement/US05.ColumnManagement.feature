@US:05
Feature: SPEC1062.US05 - Column Management
  As a grid user
  I want to resize, reorder, pick, freeze, template, and compose columns
  So that DataGrid can match Radzen DataGrid column management features.

  Background:
    Given the component library LowlandTech.Components.DataGrid
    And the Radzen parity reference includes DataGridColumnResizing, DataGridColumnReoder, DataGridColumnPicking, DataGridFrozenColumns, DataGridColumnTemplate, DataGridCompositeColumns, DataGridCustomHeader, DataGridConditionalColumns, and DataGridFooterTotals

  # STATE
  # interface DataGridColumnState {
  #   columns: DataGridColumnRepresentation[]
  #   pickedColumnKeys: string[]
  #   order: string[]
  #   resizingColumnKey: string | null
  #   frozenStartKeys: string[]
  #   frozenEndKeys: string[]
  # }
  #
  # FORM
  # interface DataGridColumnForm {
  #   columnKey: string
  #   width: string | null
  #   visible: boolean
  #   pickable: boolean
  #   frozen: 'none' | 'start' | 'end'
  #   templateName: string | null
  # }
  #
  # REPRESENTATION
  # interface DataGridColumnRepresentation {
  #   key: string
  #   title: string
  #   propertyPath: string | null
  #   width: string | null
  #   visible: boolean
  #   pickable: boolean
  #   order: number
  #   frozen: 'none' | 'start' | 'end'
  #   children: DataGridColumnRepresentation[]
  # }
  #
  # ACTIONS
  # type DataGridColumnAction =
  #   | { type: 'ResizeColumn'; columnKey: string; width: string }
  #   | { type: 'ReorderColumn'; columnKey: string; targetIndex: number }
  #   | { type: 'PickColumns'; visibleColumnKeys: string[] }
  #   | { type: 'FreezeColumn'; columnKey: string; side: 'start' | 'end' }
  #   | { type: 'RenderColumnTemplate'; columnKey: string; templateName: string }
  #   | { type: 'RenderCompositeHeader'; parentColumnKey: string }
  #
  # REDUCER TRANSITIONS
  # ResizeColumn -> columns[columnKey].width updated
  # ReorderColumn -> order updated and column order indices normalized
  # PickColumns -> visible flags updated for pickable columns only
  # FreezeColumn -> frozen side updated and sticky offsets recalculated
  # RenderColumnTemplate -> view context emitted, state unchanged
  # RenderCompositeHeader -> header row spans recalculated
  #
  # EFFECTS
  # on PickColumns -> PickedColumnsChanged callback receives visible columns
  #
  # EVENTS
  # type DataGridColumnEvent = 'ColumnResized' | 'ColumnReordered' | 'PickedColumnsChanged' | 'ColumnFrozen' | 'ColumnTemplateRendered'

  @state:DataGridColumnState
  @SC:01
  Scenario: SC01 - Column resize updates explicit width and table layout
    @precondition:ColumnResizingEnabled
    Given column "CompanyName" has width "200px"
    @precondition:ResizeHandleRendered
    And the CompanyName header exposes a resize handle

    @action:ResizeColumn
    When the user drags the CompanyName resize handle to width "320px"
    @param:columnKey
    And columnKey is "CompanyName"
    @param:width
    And width is "320px"

    @postcondition:ColumnWidthUpdated
    Then column "CompanyName" should store width "320px" @UAC:01
    And the header cell should render width "320px" @UAC:02
    And body cells for CompanyName should render width "320px" @UAC:03
    @postevent:ColumnResized
    And ColumnResized should include old and new width @UAC:04

  @state:DataGridColumnState
  @SC:02
  Scenario: SC02 - Column reorder changes visual order without losing descriptors
    @precondition:ColumnReorderEnabled
    Given columns are ordered ID, CompanyName, City, Country
    @precondition:ReorderTargetAvailable
    And column "Country" can be dragged

    @action:ReorderColumn
    When the user drops Country before CompanyName
    @param:columnKey
    And columnKey is "Country"
    @param:targetIndex
    And targetIndex is 1

    @postcondition:ColumnOrderUpdated
    Then columns should be ordered ID, Country, CompanyName, City @UAC:05
    And all column property paths should be preserved @UAC:06
    And filter and sort descriptors should remap by column key rather than visual index @UAC:07
    @postevent:ColumnReordered
    And ColumnReordered should include source and target index @UAC:08

  @state:DataGridColumnState
  @SC:03
  Scenario: SC03 - Column picker hides and shows pickable columns
    @precondition:ColumnPickingEnabled
    Given columns include ID, Photo, FirstName, LastName, and Notes
    @precondition:PickableColumnsConfigured
    And Photo has pickable false

    @action:PickColumns
    When the user picks only ID, FirstName, and LastName
    @param:visibleColumnKeys
    And visibleColumnKeys are ID, FirstName, and LastName

    @postcondition:PickedColumnsApplied
    Then ID, FirstName, and LastName should be visible @UAC:09
    And Notes should be hidden @UAC:10
    And Photo should remain visible because it is not pickable @UAC:11
    @postevent:PickedColumnsChanged
    And PickedColumnsChanged should include visible pickable columns @UAC:12

  @state:DataGridColumnState
  @SC:04
  Scenario: SC04 - Column picker can filter available columns by title
    @precondition:ColumnPickerFilteringEnabled
    Given the column picker contains FirstName, LastName, BirthDate, HireDate, and HomePhone
    @precondition:PickerOpen
    And the picker search input is focused

    @action:PickColumns
    When the user searches picker columns by "date"
    @param:visibleColumnKeys
    And visibleColumnKeys remain unchanged

    @postcondition:PickerListFiltered
    Then the picker list should show BirthDate and HireDate @UAC:13
    And the picker list should hide FirstName, LastName, and HomePhone @UAC:14
    And the underlying grid visible columns should not change until a checkbox changes @UAC:15

  @state:DataGridColumnState
  @SC:05
  Scenario: SC05 - Frozen columns remain sticky during horizontal scroll
    @precondition:FrozenColumnsConfigured
    Given column "ID" is frozen at start
    @precondition:WideGridRendered
    And the grid has horizontal overflow

    @action:FreezeColumn
    When the grid computes frozen column offsets
    @param:columnKey
    And columnKey is "ID"
    @param:side
    And side is "start"

    @postcondition:FrozenColumnSticky
    Then the ID header cell should use sticky positioning at the start side @UAC:16
    And the ID body cells should use the same sticky offset @UAC:17
    And horizontally scrolling non-frozen columns should not move the ID column @UAC:18
    @postevent:ColumnFrozen
    And ColumnFrozen should include side "start" @UAC:19

  @state:DataGridColumnState
  @SC:06
  Scenario: SC06 - Cell template receives row item and column context
    @precondition:ColumnTemplateProvided
    Given column "Photo" has a cell Template
    @precondition:RowsLoaded
    And the first row has Photo URL "/images/a.png"

    @action:RenderColumnTemplate
    When the Photo cell renders for the first row
    @param:columnKey
    And columnKey is "Photo"
    @param:templateName
    And templateName is "Template"

    @postcondition:CellTemplateContextProvided
    Then the template context should include the row item @UAC:20
    And the template context should include the column descriptor @UAC:21
    And the rendered cell should contain the image URL from the row item @UAC:22
    @postevent:ColumnTemplateRendered
    And ColumnTemplateRendered should include columnKey "Photo" @UAC:23

  @state:DataGridColumnState
  @SC:07
  Scenario: SC07 - Composite columns render grouped headers with row and column spans
    @precondition:CompositeColumnsConfigured
    Given parent column "Employee" contains child columns FirstName and LastName
    @precondition:SiblingColumnExists
    And sibling column "Title" has no children

    @action:RenderCompositeHeader
    When the header rows render
    @param:parentColumnKey
    And parentColumnKey is "Employee"

    @postcondition:CompositeHeaderRendered
    Then the Employee header should span two child columns @UAC:24
    And FirstName and LastName should render in the second header row @UAC:25
    And Title should span the full header depth where appropriate @UAC:26

  @state:DataGridColumnState
  @SC:08
  Scenario: SC08 - Footer totals render aggregate cells aligned to columns
    @precondition:FooterTotalsEnabled
    Given column "Freight" has aggregate sum
    @precondition:ColumnOrderKnown
    And columns are ordered Customer, Country, Freight

    @action:RenderColumnTemplate
    When the footer row renders
    @param:columnKey
    And columnKey is "Freight"
    @param:templateName
    And templateName is "FooterTemplate"

    @postcondition:FooterTotalRendered
    Then the Freight footer cell should contain the sum of Freight values @UAC:27
    And footer cells should align under their body columns @UAC:28
    And hidden columns should not reserve footer cells @UAC:29
    @postevent:ColumnTemplateRendered
    And ColumnTemplateRendered should include templateName "FooterTemplate" @UAC:30
