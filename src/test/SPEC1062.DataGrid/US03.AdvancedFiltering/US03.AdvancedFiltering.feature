@US:03
Feature: SPEC1062.US03 - Advanced Filtering
  As a grid user
  I want simple, advanced, typed, template, and API-driven filters
  So that DataGrid can match Radzen DataGrid filtering workflows for local and remote data.

  Background:
    Given the component library LowlandTech.Components.DataGrid
    And the Radzen parity reference includes DataGridAdvancedFilter, DataGridSimpleFilterWithMenu, DataGridCheckBoxListFilter, DataGridColumnFilterTemplate, DataGridColumnFilterValueTemplate, DataGridColumnEnumFilter, DataGridColumnSubPropertiesFilter, and DataGridFilterApi

  # STATE
  # interface DataGridFilteringState {
  #   rows: DataGridRowRepresentation[]
  #   columns: DataGridColumnRepresentation[]
  #   filters: DataGridFilterDescriptor[]
  #   filterMode: 'simple' | 'simpleWithMenu' | 'advanced'
  #   logicalOperator: 'and' | 'or'
  #   filteredRows: DataGridRowRepresentation[]
  #   pageIndex: number
  #   loadDataRequest: DataGridLoadDataRequest | null
  # }
  #
  # FORM
  # interface DataGridFilterForm {
  #   columnKey: string
  #   operator: string
  #   value: unknown
  #   secondOperator: string | null
  #   secondValue: unknown
  #   logicalOperator: 'and' | 'or'
  # }
  #
  # REPRESENTATION
  # interface DataGridFilterDescriptor {
  #   columnKey: string
  #   propertyPath: string
  #   operator: string
  #   value: unknown
  #   secondOperator?: string
  #   secondValue?: unknown
  #   logicalOperator?: 'and' | 'or'
  # }
  #
  # interface DataGridLoadDataRequest {
  #   skip: number
  #   top: number
  #   orderBy: string | null
  #   filter: string | null
  #   descriptors: DataGridFilterDescriptor[]
  # }
  #
  # ACTIONS
  # type DataGridFilteringAction =
  #   | { type: 'SetFilterMode'; filterMode: 'simple' | 'simpleWithMenu' | 'advanced' }
  #   | { type: 'ApplyColumnFilter'; descriptor: DataGridFilterDescriptor }
  #   | { type: 'ApplyFilterTemplateValue'; columnKey: string; value: unknown }
  #   | { type: 'ClearColumnFilter'; columnKey: string }
  #   | { type: 'ClearAllFilters' }
  #   | { type: 'BuildLoadDataFilter'; descriptors: DataGridFilterDescriptor[] }
  #
  # REDUCER TRANSITIONS
  # SetFilterMode -> filterMode: payload
  # ApplyColumnFilter -> upsert descriptor, pageIndex: 0, filteredRows recomputed
  # ApplyFilterTemplateValue -> upsert descriptor from template value, pageIndex: 0
  # ClearColumnFilter -> remove descriptor for columnKey, pageIndex: 0
  # ClearAllFilters -> filters: [], filteredRows: rows, pageIndex: 0
  # BuildLoadDataFilter -> loadDataRequest.filter/descriptors updated without mutating rows
  #
  # EFFECTS
  # on BuildLoadDataFilter -> LoadData callback receives filter expression and descriptors
  #
  # EVENTS
  # type DataGridFilteringEvent = 'FilterApplied' | 'FilterCleared' | 'FilterExpressionBuilt' | 'FilteredRowsChanged'

  @state:DataGridFilteringState
  @SC:01
  Scenario: SC01 - Simple filter row filters text columns with contains semantics
    @precondition:SimpleFilterEnabled
    Given DataGridFilteringState.filterMode is "simple"
    @precondition:TextColumnFilterable
    And column "Customer" is filterable with property path "CompanyName"

    @action:ApplyColumnFilter
    When the user types "alf" into the Customer filter input
    @param:descriptor
    And descriptor has columnKey "Customer", operator "contains", and value "alf"

    @postcondition:RowsFiltered
    Then filteredRows should contain rows whose CompanyName contains "alf" case-insensitively @UAC:01
    And filteredRows should exclude rows whose CompanyName does not contain "alf" @UAC:02
    And pageIndex should reset to 0 @UAC:03
    @postevent:FilterApplied
    And FilterApplied should include columnKey "Customer" @UAC:04

  @state:DataGridFilteringState
  @SC:02
  Scenario: SC02 - Advanced filter supports two predicates joined by logical operator
    @precondition:AdvancedFilterEnabled
    Given DataGridFilteringState.filterMode is "advanced"
    @precondition:NumericColumnFilterable
    And column "Freight" is filterable as a number

    @action:ApplyColumnFilter
    When the user applies Freight greater than 10 and less than 50
    @param:descriptor
    And descriptor has operator "greaterThan", value 10, secondOperator "lessThan", secondValue 50, and logicalOperator "and"

    @postcondition:RangeFilterApplied
    Then filteredRows should contain only rows where Freight is greater than 10 and less than 50 @UAC:05
    And the column should display both active filter predicates @UAC:06
    And the filter menu should preserve the selected logical operator "and" @UAC:07
    @postevent:FilteredRowsChanged
    And FilteredRowsChanged should include the new filtered row count @UAC:08

  @state:DataGridFilteringState
  @SC:03
  Scenario: SC03 - Checkbox list filter applies multiple values with OR semantics
    @precondition:CheckboxListFilterEnabled
    Given column "ShipCountry" has a checkbox list filter
    @precondition:DistinctValuesLoaded
    And distinct values include "France", "Germany", and "Brazil"

    @action:ApplyFilterTemplateValue
    When the user checks "France" and "Germany"
    @param:columnKey
    And columnKey is "ShipCountry"
    @param:value
    And value is ["France", "Germany"]

    @postcondition:MultiValueFilterApplied
    Then filteredRows should contain rows where ShipCountry is "France" or "Germany" @UAC:09
    And filteredRows should exclude rows where ShipCountry is "Brazil" @UAC:10
    And the filter chip text should summarize the two selected values @UAC:11
    @postevent:FilterApplied
    And FilterApplied should include operator "in" @UAC:12

  @state:DataGridFilteringState
  @SC:04
  Scenario: SC04 - Enum filter stores typed enum values instead of display text
    @precondition:EnumColumnFilterable
    Given column "WorkStatus" is bound to an enum property
    @precondition:EnumFilterValuesAvailable
    And the filter menu displays enum labels "Office", "Remote", and "Hybrid"

    @action:ApplyColumnFilter
    When the user selects "Remote"
    @param:descriptor
    And descriptor has columnKey "WorkStatus", operator "equals", and value WorkStatus.Remote

    @postcondition:TypedEnumFilterApplied
    Then filteredRows should compare against the enum value WorkStatus.Remote @UAC:13
    And the displayed filter value should be "Remote" @UAC:14
    And the descriptor value should not be the string "Remote" @UAC:15

  @state:DataGridFilteringState
  @SC:05
  Scenario: SC05 - Sub-property filter resolves nested property paths
    @precondition:NestedPropertyColumnFilterable
    Given column "Customer City" uses property path "Customer.Address.City"
    @precondition:NestedRowsLoaded
    And rows contain nested Customer and Address objects

    @action:ApplyColumnFilter
    When the user filters Customer City by "London"
    @param:descriptor
    And descriptor has propertyPath "Customer.Address.City" and value "London"

    @postcondition:NestedPropertyFiltered
    Then filteredRows should contain rows whose nested city is "London" @UAC:16
    And rows with null Customer or null Address should not throw @UAC:17
    And rows with null Customer or null Address should be treated as non-matches @UAC:18

  @state:DataGridFilteringState
  @SC:06
  Scenario: SC06 - Custom filter template replaces default filter UI for a column
    @precondition:FilterTemplateProvided
    Given column "Status" has a FilterTemplate
    @precondition:DefaultFilterUiAvailable
    And the grid would normally render a text filter input for Status

    @action:ApplyFilterTemplateValue
    When the custom template emits value "Active"
    @param:columnKey
    And columnKey is "Status"
    @param:value
    And value is "Active"

    @postcondition:TemplateOwnsFilterUi
    Then the default Status filter input should not render @UAC:19
    And the custom filter template should render in the filter cell or menu @UAC:20
    And filteredRows should reflect the template-emitted value @UAC:21
    @postevent:FilterApplied
    And FilterApplied should identify source "FilterTemplate" @UAC:22

  @state:DataGridFilteringState
  @SC:07
  Scenario: SC07 - Filter value template customizes only the value editor
    @precondition:FilterValueTemplateProvided
    Given column "HireDate" has a FilterValueTemplate
    @precondition:AdvancedFilterChromeAvailable
    And the default operator selector and clear button are enabled

    @action:ApplyFilterTemplateValue
    When the date picker template emits date "2026-05-16"
    @param:columnKey
    And columnKey is "HireDate"
    @param:value
    And value is date "2026-05-16"

    @postcondition:TemplateValueIntegrated
    Then the default operator selector should remain visible @UAC:23
    And the custom date picker should replace only the value editor @UAC:24
    And the filter descriptor should store the emitted date value @UAC:25

  @state:DataGridFilteringState
  @SC:08
  Scenario: SC08 - Filter API builds remote load request without client filtering
    @precondition:LoadDataModeEnabled
    Given the grid is bound through LoadData
    @precondition:RemoteRowsNotLoadedCompletely
    And DataGridFilteringState.rows contains only the current page

    @action:BuildLoadDataFilter
    When the user applies Customer contains "alf"
    @param:descriptors
    And descriptors include a contains filter for CompanyName

    @postcondition:RemoteFilterRequestBuilt
    Then LoadData should receive descriptors containing the CompanyName filter @UAC:26
    And LoadData should receive a serializable filter expression @UAC:27
    And client-side rows should not be filtered before LoadData returns @UAC:28
    @postevent:FilterExpressionBuilt
    And FilterExpressionBuilt should include skip, top, filter, and orderBy fields @UAC:29
