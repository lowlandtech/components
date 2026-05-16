@US:04
Feature: SPEC1062.US04 - Sorting and Grouping
  As a grid user
  I want single sort, multiple sort, sort API, and grouping with templates
  So that DataGrid can match Radzen DataGrid sort and group behavior.

  Background:
    Given the component library LowlandTech.Components.DataGrid
    And the Radzen parity reference includes DataGridSort, DataGridMultipleSort, DataGridSortApi, DataGridGroupingApi, DataGridGroupHeaderTemplate, and DataGridGroupFooterTemplate

  # STATE
  # interface DataGridSortGroupState {
  #   rows: DataGridRowRepresentation[]
  #   columns: DataGridColumnRepresentation[]
  #   sorts: DataGridSortDescriptor[]
  #   groups: DataGridGroupDescriptor[]
  #   groupedRows: DataGridGroupRepresentation[]
  #   allowMultiSort: boolean
  #   loadDataRequest: DataGridLoadDataRequest | null
  # }
  #
  # FORM
  # interface DataGridSortGroupForm {
  #   columnKey: string
  #   direction: 'ascending' | 'descending' | 'none'
  #   additive: boolean
  #   groupTemplateName: string | null
  # }
  #
  # REPRESENTATION
  # interface DataGridSortDescriptor { columnKey: string; propertyPath: string; direction: 'ascending' | 'descending'; order: number }
  # interface DataGridGroupDescriptor { columnKey: string; propertyPath: string; direction: 'ascending' | 'descending'; expanded: boolean }
  # interface DataGridGroupRepresentation { key: unknown; count: number; rows: DataGridRowRepresentation[]; aggregates: Record<string, unknown> }
  #
  # ACTIONS
  # type DataGridSortGroupAction =
  #   | { type: 'ToggleColumnSort'; columnKey: string; additive: boolean }
  #   | { type: 'ClearSorts' }
  #   | { type: 'BuildSortRequest'; sorts: DataGridSortDescriptor[] }
  #   | { type: 'GroupByColumn'; columnKey: string }
  #   | { type: 'ToggleGroupExpanded'; groupKey: unknown }
  #   | { type: 'RenderGroupTemplate'; groupKey: unknown; templateName: string }
  #
  # REDUCER TRANSITIONS
  # ToggleColumnSort -> cycles none/ascending/descending/none, updates sort order
  # ClearSorts -> sorts: []
  # BuildSortRequest -> loadDataRequest.orderBy updated
  # GroupByColumn -> groups upserted, groupedRows recomputed
  # ToggleGroupExpanded -> matching group.expanded toggled
  # RenderGroupTemplate -> groupedRows unchanged, view template context emitted
  #
  # EFFECTS
  # on BuildSortRequest -> LoadData receives orderBy and descriptors
  #
  # EVENTS
  # type DataGridSortGroupEvent = 'SortChanged' | 'SortRequestBuilt' | 'GroupChanged' | 'GroupExpandedChanged'

  @state:DataGridSortGroupState
  @SC:01
  Scenario: SC01 - Single column sort cycles through ascending, descending, and none
    @precondition:SingleSortEnabled
    Given DataGridSortGroupState.allowMultiSort is false
    @precondition:SortableColumn
    And column "LastName" is sortable

    @action:ToggleColumnSort
    When the user clicks the LastName header three times
    @param:columnKey
    And columnKey is "LastName"
    @param:additive
    And additive is false

    @postcondition:SortCycleComplete
    Then the first click should sort LastName ascending @UAC:01
    And the second click should sort LastName descending @UAC:02
    And the third click should clear LastName sorting @UAC:03
    And the sort indicator should match the active direction after each click @UAC:04
    @postevent:SortChanged
    And SortChanged should be raised for each direction change @UAC:05

  @state:DataGridSortGroupState
  @SC:02
  Scenario: SC02 - Multiple sort preserves sort priority order
    @precondition:MultiSortEnabled
    Given DataGridSortGroupState.allowMultiSort is true
    @precondition:SortableColumns
    And columns "Country" and "City" are sortable

    @action:ToggleColumnSort
    When the user sorts Country and then additively sorts City
    @param:columnKey
    And columnKey is "City"
    @param:additive
    And additive is true

    @postcondition:MultipleSortDescriptorsStored
    Then sorts should contain Country with order 0 @UAC:06
    And sorts should contain City with order 1 @UAC:07
    And rows should be ordered by Country before City @UAC:08
    And each sorted header should display its sort priority @UAC:09

  @state:DataGridSortGroupState
  @SC:03
  Scenario: SC03 - Sort API emits remote orderBy expression
    @precondition:LoadDataModeEnabled
    Given DataGridSortGroupState.loadDataRequest is empty
    @precondition:SortDescriptorsExist
    And sorts contain Country ascending and City descending

    @action:BuildSortRequest
    When the grid builds a LoadData request
    @param:sorts
    And sorts are Country ascending and City descending

    @postcondition:RemoteSortRequestBuilt
    Then loadDataRequest.orderBy should encode "Country asc, City desc" @UAC:10
    And loadDataRequest should include structured sort descriptors @UAC:11
    And client-side rows should not be reordered before LoadData returns @UAC:12
    @postevent:SortRequestBuilt
    And SortRequestBuilt should include the descriptor order @UAC:13

  @state:DataGridSortGroupState
  @SC:04
  Scenario: SC04 - Grouping creates group rows with counts
    @precondition:GroupingEnabled
    Given rows contain employees from multiple countries
    @precondition:GroupableColumn
    And column "Country" is groupable

    @action:GroupByColumn
    When the user groups by Country
    @param:columnKey
    And columnKey is "Country"

    @postcondition:GroupsCreated
    Then groupedRows should contain one group per distinct Country @UAC:14
    And each group should expose its key @UAC:15
    And each group should expose its row count @UAC:16
    And grouped rows should render under their matching group header @UAC:17
    @postevent:GroupChanged
    And GroupChanged should include columnKey "Country" @UAC:18

  @state:DataGridSortGroupState
  @SC:05
  Scenario: SC05 - Group expansion toggles row visibility
    @precondition:GroupedRowsRendered
    Given groupedRows contains group "France"
    @precondition:GroupExpanded
    And group "France" is expanded

    @action:ToggleGroupExpanded
    When the user collapses group "France"
    @param:groupKey
    And groupKey is "France"

    @postcondition:GroupCollapsed
    Then the France group header should remain visible @UAC:19
    And rows inside the France group should be hidden @UAC:20
    And the group toggle should have aria-expanded="false" @UAC:21
    @postevent:GroupExpandedChanged
    And GroupExpandedChanged should include groupKey "France" @UAC:22

  @state:DataGridSortGroupState
  @SC:06
  Scenario: SC06 - Group header template receives key, count, and aggregate context
    @precondition:GroupHeaderTemplateProvided
    Given the grid has a GroupHeaderTemplate for Country
    @precondition:GroupedRowsRendered
    And groupedRows contains group "Germany" with 8 rows

    @action:RenderGroupTemplate
    When the group header for Germany renders
    @param:groupKey
    And groupKey is "Germany"
    @param:templateName
    And templateName is "GroupHeaderTemplate"

    @postcondition:HeaderTemplateContextProvided
    Then the template context should include key "Germany" @UAC:23
    And the template context should include count 8 @UAC:24
    And the template context should include aggregate values for configured aggregate columns @UAC:25

  @state:DataGridSortGroupState
  @SC:07
  Scenario: SC07 - Group footer template renders aggregate totals
    @precondition:GroupFooterTemplateProvided
    Given the grid has a GroupFooterTemplate for Country
    @precondition:AggregateColumnConfigured
    And column "Freight" has aggregate "sum"

    @action:RenderGroupTemplate
    When the group footer renders for Country group "Brazil"
    @param:groupKey
    And groupKey is "Brazil"
    @param:templateName
    And templateName is "GroupFooterTemplate"

    @postcondition:FooterTemplateContextProvided
    Then the footer template should receive the Brazil group rows @UAC:26
    And the footer template should receive Freight sum @UAC:27
    And the footer should render after the group's data rows @UAC:28

  @state:DataGridSortGroupState
  @SC:08
  Scenario: SC08 - Clearing grouping restores flat row order
    @precondition:GroupingActive
    Given DataGridSortGroupState.groups contains Country
    @precondition:GroupedRowsRendered
    And groupedRows are currently rendered

    @action:GroupByColumn
    When the user removes grouping for Country
    @param:columnKey
    And columnKey is "Country"

    @postcondition:FlatRowsRestored
    Then groups should not contain Country @UAC:29
    And group header rows should no longer render @UAC:30
    And flat rows should render in current sort order @UAC:31
    @postevent:GroupChanged
    And GroupChanged should include an empty group descriptor list @UAC:32
