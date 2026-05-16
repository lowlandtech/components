@US:09
Feature: SPEC1062.US09 - Remote Data and Virtualization
  As a grid user with large datasets
  I want LoadData, IQueryable, OData, virtualization, performance, realtime updates, and pager APIs
  So that DataGrid can match Radzen DataGrid large-data workflows.

  Background:
    Given the component library LowlandTech.Components.DataGrid
    And the Radzen parity reference includes DataGridLoadData, DataGridIQueryable, DataGridOData, DataGridVirtualization, DataGridVirtualizationLoadData, DataGridPerformance, DataGridRealtimeData, DataGridPagerApi, DataGridPagerPosition, and DataGridPagerHorizontalAlign

  # STATE
  # interface DataGridRemoteVirtualState {
  #   rows: DataGridRowRepresentation[]
  #   totalCount: number
  #   loading: boolean
  #   skip: number
  #   top: number
  #   pageIndex: number
  #   pageSize: number
  #   viewportStartIndex: number
  #   viewportItemCount: number
  #   request: DataGridLoadDataRequest | null
  #   realtimeVersion: number
  # }
  #
  # FORM
  # interface DataGridRemoteVirtualForm {
  #   pageIndex: number
  #   pageSize: number
  #   viewportStartIndex: number
  #   viewportItemCount: number
  #   filter: string | null
  #   orderBy: string | null
  # }
  #
  # REPRESENTATION
  # interface DataGridLoadDataRequest {
  #   skip: number
  #   top: number
  #   filter: string | null
  #   orderBy: string | null
  #   count: boolean
  # }
  #
  # ACTIONS
  # type DataGridRemoteVirtualAction =
  #   | { type: 'RequestPage'; pageIndex: number; pageSize: number }
  #   | { type: 'ReceivePage'; rows: DataGridRowRepresentation[]; totalCount: number }
  #   | { type: 'RequestVirtualWindow'; viewportStartIndex: number; viewportItemCount: number }
  #   | { type: 'BuildODataQuery'; request: DataGridLoadDataRequest }
  #   | { type: 'ApplyRealtimeUpdate'; row: DataGridRowRepresentation }
  #   | { type: 'SetPagerOptions'; pageSize: number; position: string; horizontalAlign: string }
  #
  # REDUCER TRANSITIONS
  # RequestPage -> loading: true, request with skip/top
  # ReceivePage -> rows, totalCount, loading: false
  # RequestVirtualWindow -> loading: true, request for visible window plus overscan
  # BuildODataQuery -> serializable query string emitted
  # ApplyRealtimeUpdate -> row upserted, realtimeVersion + 1
  # SetPagerOptions -> pageSize and pager presentation updated
  #
  # EFFECTS
  # on RequestPage -> LoadData(request)
  # on RequestVirtualWindow -> LoadData(request)
  # on BuildODataQuery -> HTTP request with $skip, $top, $filter, $orderby, $count
  #
  # EVENTS
  # type DataGridRemoteVirtualEvent = 'LoadDataRequested' | 'PageReceived' | 'VirtualWindowRequested' | 'ODataQueryBuilt' | 'RealtimeRowUpdated' | 'PagerChanged'

  @state:DataGridRemoteVirtualState
  @SC:01
  Scenario: SC01 - LoadData request includes paging, sorting, and filtering
    @precondition:LoadDataConfigured
    Given pageIndex is 2 and pageSize is 20
    @precondition:SortAndFilterActive
    And filter is "CompanyName contains 'alf'" and orderBy is "CompanyName asc"

    @action:RequestPage
    When the grid requests page 2
    @param:pageIndex
    And pageIndex is 2
    @param:pageSize
    And pageSize is 20

    @postcondition:LoadDataRequestBuilt
    Then request.skip should be 40 @UAC:01
    And request.top should be 20 @UAC:02
    And request.filter should encode the active filter @UAC:03
    And request.orderBy should encode the active sort @UAC:04
    @postevent:LoadDataRequested
    And LoadDataRequested should include count=true @UAC:05

  @state:DataGridRemoteVirtualState
  @SC:02
  Scenario: SC02 - Receiving remote page updates rows and total count
    @precondition:LoadingPage
    Given loading is true
    @precondition:RemoteResponseAvailable
    And the remote response contains 20 rows and totalCount 143

    @action:ReceivePage
    When the grid receives the remote page
    @param:rows
    And rows contains 20 items
    @param:totalCount
    And totalCount is 143

    @postcondition:RemotePageRendered
    Then loading should be false @UAC:06
    And rows should contain the 20 received items @UAC:07
    And totalCount should be 143 @UAC:08
    And pager should report 8 pages when pageSize is 20 @UAC:09
    @postevent:PageReceived
    And PageReceived should include row count and total count @UAC:10

  @state:DataGridRemoteVirtualState
  @SC:03
  Scenario: SC03 - IQueryable binding composes query operations before enumeration
    @precondition:IQueryableBindingEnabled
    Given Items is an IQueryable source
    @precondition:OperationsActive
    And filters, sorts, and paging are active

    @action:RequestPage
    When the grid evaluates the current page
    @param:pageIndex
    And pageIndex is 1
    @param:pageSize
    And pageSize is 10

    @postcondition:QueryableComposed
    Then filtering should be applied before sorting @UAC:11
    And sorting should be applied before Skip and Take @UAC:12
    And the query should be enumerated only for the current page and count @UAC:13

  @state:DataGridRemoteVirtualState
  @SC:04
  Scenario: SC04 - OData request maps descriptors to query string parameters
    @precondition:ODataModeEnabled
    Given request has skip 40 and top 20
    @precondition:DescriptorsActive
    And request has filter "CompanyName contains 'alf'" and orderBy "CompanyName asc"

    @action:BuildODataQuery
    When the grid builds an OData query string
    @param:request
    And request includes skip, top, filter, orderBy, and count

    @postcondition:ODataQueryBuilt
    Then the query string should include "$skip=40" @UAC:14
    And the query string should include "$top=20" @UAC:15
    And the query string should include "$filter=" @UAC:16
    And the query string should include "$orderby=CompanyName asc" @UAC:17
    And the query string should include "$count=true" @UAC:18
    @postevent:ODataQueryBuilt
    And ODataQueryBuilt should include the encoded URI @UAC:19

  @state:DataGridRemoteVirtualState
  @SC:05
  Scenario: SC05 - Virtualization requests only visible window plus overscan
    @precondition:VirtualizationEnabled
    Given totalCount is 100000
    @precondition:ViewportKnown
    And viewportStartIndex is 500 and viewportItemCount is 30

    @action:RequestVirtualWindow
    When the user scrolls to index 500
    @param:viewportStartIndex
    And viewportStartIndex is 500
    @param:viewportItemCount
    And viewportItemCount is 30

    @postcondition:VirtualWindowRequestBuilt
    Then request.skip should be less than or equal to 500 @UAC:20
    And request.top should cover the visible 30 items plus overscan @UAC:21
    And the DOM should render only the virtualized rows for the current window @UAC:22
    @postevent:VirtualWindowRequested
    And VirtualWindowRequested should include viewport indexes @UAC:23

  @state:DataGridRemoteVirtualState
  @SC:06
  Scenario: SC06 - Virtualization with LoadData preserves scroll position after data returns
    @precondition:VirtualLoadDataEnabled
    Given the user is scrolled to item index 2500
    @precondition:LoadingVirtualWindow
    And loading is true for the virtual window

    @action:ReceivePage
    When LoadData returns rows for the virtual window
    @param:rows
    And rows cover item index 2500
    @param:totalCount
    And totalCount is 100000

    @postcondition:ScrollPositionPreserved
    Then the virtual spacer should preserve total scroll height @UAC:24
    And the rendered window should still align around item index 2500 @UAC:25
    And loading should be false @UAC:26

  @state:DataGridRemoteVirtualState
  @SC:07
  Scenario: SC07 - Realtime row update patches visible row without resetting grid state
    @precondition:RealtimeEnabled
    Given rows contain visible row "order-1"
    @precondition:GridStateActive
    And pageIndex, filters, sorts, and selected rows are active

    @action:ApplyRealtimeUpdate
    When realtime data updates row "order-1"
    @param:row
    And row has key "order-1" and new status "Shipped"

    @postcondition:VisibleRowPatched
    Then row "order-1" should render status "Shipped" @UAC:27
    And active filters and sorts should remain unchanged @UAC:28
    And selected rows should remain unchanged when their keys still exist @UAC:29
    @postevent:RealtimeRowUpdated
    And RealtimeRowUpdated should increment realtimeVersion @UAC:30

  @state:DataGridRemoteVirtualState
  @SC:08
  Scenario: SC08 - Pager API supports page size, position, and alignment
    @precondition:PagerEnabled
    Given totalCount is 143
    @precondition:PagerOptionsAvailable
    And pager position is "bottom" and horizontal align is "end"

    @action:SetPagerOptions
    When the user changes pageSize to 50 and position to "topAndBottom"
    @param:pageSize
    And pageSize is 50
    @param:position
    And position is "topAndBottom"

    @postcondition:PagerOptionsApplied
    Then pageSize should be 50 @UAC:31
    And total pages should recalculate to 3 @UAC:32
    And pager controls should render at both top and bottom positions @UAC:33
    And pager controls should use the configured horizontal alignment @UAC:34
    @postevent:PagerChanged
    And PagerChanged should include pageSize, position, and horizontalAlign @UAC:35
