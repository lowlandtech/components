@US:08
Feature: SPEC1062.US08 - Hierarchy and Templates
  As a grid author
  I want master-detail, self-reference hierarchy, dynamic columns, conditional templates, density, and empty states
  So that DataGrid can match Radzen DataGrid composition scenarios.

  Background:
    Given the component library LowlandTech.Components.DataGrid
    And the Radzen parity reference includes DataGridMasterDetail, DataGridMasterDetailHierarchy, DataGridMasterDetailHierarchyOnDemand, DataGridSelfReferenceHierarchy, DataGridDynamicData, DataGridDataTable, DataGridConditionalTemplate, DataGridConditionalColumns, DataGridDensity, DataGridGridLines, and DataGridEmpty

  # STATE
  # interface DataGridHierarchyTemplateState {
  #   rows: DataGridRowRepresentation[]
  #   expandedRowKeys: string[]
  #   childRows: Record<string, DataGridRowRepresentation[]>
  #   dynamicColumns: DataGridColumnRepresentation[]
  #   density: 'compact' | 'default' | 'comfortable'
  #   gridLines: 'none' | 'horizontal' | 'vertical' | 'both'
  #   emptyText: string
  # }
  #
  # FORM
  # interface DataGridHierarchyTemplateForm {
  #   rowKey: string
  #   parentKey: string | null
  #   templateName: string
  #   density: string
  #   gridLines: string
  # }
  #
  # REPRESENTATION
  # interface DataGridDetailRepresentation {
  #   rowKey: string
  #   expanded: boolean
  #   childRows: DataGridRowRepresentation[]
  #   loading: boolean
  # }
  #
  # ACTIONS
  # type DataGridHierarchyTemplateAction =
  #   | { type: 'ToggleDetailRow'; rowKey: string }
  #   | { type: 'LoadChildRows'; rowKey: string }
  #   | { type: 'BuildSelfReferenceTree'; parentKey: string | null }
  #   | { type: 'GenerateDynamicColumns'; schema: Record<string, string> }
  #   | { type: 'RenderConditionalTemplate'; rowKey: string; templateName: string }
  #   | { type: 'SetDensity'; density: 'compact' | 'default' | 'comfortable' }
  #   | { type: 'SetGridLines'; gridLines: 'none' | 'horizontal' | 'vertical' | 'both' }
  #
  # REDUCER TRANSITIONS
  # ToggleDetailRow -> expandedRowKeys toggled
  # LoadChildRows -> childRows[rowKey] populated
  # BuildSelfReferenceTree -> hierarchical row representation created
  # GenerateDynamicColumns -> dynamicColumns from schema
  # RenderConditionalTemplate -> template context emitted, state unchanged
  # SetDensity -> density updated
  # SetGridLines -> gridLines updated
  #
  # EFFECTS
  # on LoadChildRows -> LoadChildData callback
  #
  # EVENTS
  # type DataGridHierarchyTemplateEvent = 'DetailRowExpanded' | 'ChildRowsLoaded' | 'SelfReferenceTreeBuilt' | 'DynamicColumnsGenerated' | 'TemplateRendered' | 'PresentationChanged'

  @state:DataGridHierarchyTemplateState
  @SC:01
  Scenario: SC01 - Master-detail expands a row detail template
    @precondition:DetailTemplateConfigured
    Given row "order-10248" has a detail template
    @precondition:RowCollapsed
    And expandedRowKeys does not contain "order-10248"

    @action:ToggleDetailRow
    When the user expands row "order-10248"
    @param:rowKey
    And rowKey is "order-10248"

    @postcondition:DetailTemplateVisible
    Then expandedRowKeys should contain "order-10248" @UAC:01
    And the detail row should render immediately after the master row @UAC:02
    And the detail template should receive the master row item @UAC:03
    @postevent:DetailRowExpanded
    And DetailRowExpanded should include rowKey "order-10248" @UAC:04

  @state:DataGridHierarchyTemplateState
  @SC:02
  Scenario: SC02 - Master-detail hierarchy loads nested rows on demand
    @precondition:OnDemandHierarchyEnabled
    Given row "customer-alfki" has not loaded child orders
    @precondition:LoadChildDataConfigured
    And LoadChildData callback is configured

    @action:LoadChildRows
    When the user expands row "customer-alfki"
    @param:rowKey
    And rowKey is "customer-alfki"

    @postcondition:ChildRowsLoaded
    Then the grid should show a loading state for the detail region @UAC:05
    And LoadChildData should be called with row "customer-alfki" @UAC:06
    And childRows["customer-alfki"] should be populated when loading completes @UAC:07
    @postevent:ChildRowsLoaded
    And ChildRowsLoaded should include parent row key and child count @UAC:08

  @state:DataGridHierarchyTemplateState
  @SC:03
  Scenario: SC03 - Self-reference hierarchy builds expandable parent-child rows
    @precondition:SelfReferenceDataLoaded
    Given rows contain EmployeeId and ReportsTo fields
    @precondition:RootRowsExist
    And employees with ReportsTo null are root rows

    @action:BuildSelfReferenceTree
    When the grid builds a self-reference hierarchy
    @param:parentKey
    And parentKey is null

    @postcondition:TreeRowsBuilt
    Then root rows should render at level 0 @UAC:09
    And child rows should render under their parent when expanded @UAC:10
    And each hierarchical row should expose aria-level @UAC:11
    And collapse state should be tracked per row key @UAC:12
    @postevent:SelfReferenceTreeBuilt
    And SelfReferenceTreeBuilt should include root count @UAC:13

  @state:DataGridHierarchyTemplateState
  @SC:04
  Scenario: SC04 - Dynamic data generates columns from runtime schema
    @precondition:DynamicDataLoaded
    Given rows are dictionaries with keys ID, Name, Country, and Total
    @precondition:NoStaticColumnsProvided
    And no explicit column definitions are supplied

    @action:GenerateDynamicColumns
    When the grid receives the runtime schema
    @param:schema
    And schema contains ID:number, Name:string, Country:string, and Total:decimal

    @postcondition:ColumnsGenerated
    Then dynamicColumns should contain ID, Name, Country, and Total @UAC:14
    And generated columns should infer filter and sort types from schema @UAC:15
    And generated cells should resolve dictionary values by key @UAC:16
    @postevent:DynamicColumnsGenerated
    And DynamicColumnsGenerated should include schema column count @UAC:17

  @state:DataGridHierarchyTemplateState
  @SC:05
  Scenario: SC05 - DataTable binding reads columns and values from tabular metadata
    @precondition:DataTableBindingEnabled
    Given the grid is bound to a DataTable-like source
    @precondition:DataTableHasColumns
    And the source columns are ProductId, ProductName, and UnitPrice

    @action:GenerateDynamicColumns
    When the grid inspects DataTable metadata
    @param:schema
    And schema is derived from DataColumn metadata

    @postcondition:DataTableColumnsGenerated
    Then generated columns should use DataColumn names as keys @UAC:18
    And generated rows should read values from DataRow by column name @UAC:19
    And null DataRow values should render as empty cells @UAC:20

  @state:DataGridHierarchyTemplateState
  @SC:06
  Scenario: SC06 - Conditional template chooses content based on row values
    @precondition:ConditionalTemplateConfigured
    Given column "Status" has a conditional template
    @precondition:RowsHaveDifferentStatuses
    And one row has Status "Late" and another row has Status "On Track"

    @action:RenderConditionalTemplate
    When Status cells render
    @param:templateName
    And templateName is "ConditionalTemplate"

    @postcondition:ConditionalContentRendered
    Then the Late row should render the late status presentation @UAC:21
    And the On Track row should render the normal status presentation @UAC:22
    And both cells should preserve the same column semantics @UAC:23
    @postevent:TemplateRendered
    And TemplateRendered should include columnKey "Status" @UAC:24

  @state:DataGridHierarchyTemplateState
  @SC:07
  Scenario: SC07 - Density and grid lines update presentation without changing data
    @precondition:PresentationOptionsAvailable
    Given DataGridHierarchyTemplateState.density is "default"
    @precondition:RowsLoaded
    And rows contain 25 items

    @action:SetDensity
    When the user changes density to "compact" and grid lines to "both"
    @param:density
    And density is "compact"
    @param:gridLines
    And gridLines is "both"

    @postcondition:PresentationUpdated
    Then the grid should render data-density="compact" @UAC:25
    And the grid should render data-grid-lines="both" @UAC:26
    And rows should still contain 25 items @UAC:27
    @postevent:PresentationChanged
    And PresentationChanged should include density and gridLines @UAC:28

  @state:DataGridHierarchyTemplateState
  @SC:08
  Scenario: SC08 - Empty state renders custom text and template
    @precondition:NoRowsAfterFiltering
    Given rows are empty after filters are applied
    @precondition:EmptyTemplateConfigured
    And emptyText is "No orders match your filters"

    @action:RenderConditionalTemplate
    When the grid renders its empty state
    @param:templateName
    And templateName is "EmptyTemplate"

    @postcondition:EmptyStateRendered
    Then the empty state should render instead of the body rows @UAC:29
    And the empty state should contain "No orders match your filters" @UAC:30
    And the empty template should receive current filter descriptors @UAC:31
