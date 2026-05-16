@SPEC:1067
@US:02
Feature: SPEC1067.US02 - TreeView Data Binding and Lazy Loading
  As an application author
  I want TreeView to bind to hierarchical, level-based, self-referencing, and lazy data
  So that it can cover Radzen Tree data binding and MudBlazor server data scenarios

  Background:
    Given the component library LowlandTech.Components.TreeView
    And callers may provide data as nested children, level descriptors, parent identifiers, or async loaders

  # STATE
  # interface TreeViewDataBindingState {
  #   roots: TreeNodeRepresentation[]
  #   levels: TreeLevelDescriptorRepresentation[]
  #   flatItems: TreeFlatNodeRepresentation[]
  #   loadedKeys: string[]
  #   loadingKeys: string[]
  #   expandedKeys: string[]
  #   errorByKey: Record<string, string | null>
  # }

  # FORM
  # interface TreeViewDataBindingForm {
  #   textProperty: string
  #   childrenProperty: string
  #   parentKeyProperty: string
  #   hasChildrenProperty: string
  #   loadKey: string | null
  # }

  # REPRESENTATION
  # interface TreeLevelDescriptorRepresentation {
  #   level: number
  #   textProperty: string
  #   childrenProperty: string | null
  #   hasChildrenProperty: string | null
  #   iconProperty: string | null
  #   templateName: string | null
  # }
  # interface TreeFlatNodeRepresentation {
  #   key: string
  #   parentKey: string | null
  #   text: string
  #   hasChildren: boolean
  # }
  # interface TreeNodeRepresentation {
  #   key: string
  #   text: string
  #   children: TreeNodeRepresentation[]
  #   hasChildren: boolean
  #   loaded: boolean
  # }

  # ACTIONS
  # type TreeViewDataBindingAction =
  #   | { type: 'BindHierarchicalData'; roots: TreeNodeRepresentation[] }
  #   | { type: 'BindLevelDescriptors'; levels: TreeLevelDescriptorRepresentation[] }
  #   | { type: 'BindSelfReferencingData'; items: TreeFlatNodeRepresentation[] }
  #   | { type: 'ExpandLazyNode'; key: string }
  #   | { type: 'LoadChildrenSuccess'; key: string; children: TreeNodeRepresentation[] }
  #   | { type: 'LoadChildrenFailure'; key: string; error: string }
  #   | { type: 'RefreshNode'; key: string | null }

  # REDUCER TRANSITIONS
  # BindHierarchicalData -> roots assigned from nested children
  # BindLevelDescriptors -> level metadata assigned and render functions resolved
  # BindSelfReferencingData -> flat rows grouped by parent key into roots
  # ExpandLazyNode -> key added to loadingKeys when children are not loaded
  # LoadChildrenSuccess -> children assigned, key added to loadedKeys, loading cleared
  # LoadChildrenFailure -> errorByKey set, loading cleared
  # RefreshNode -> loaded state cleared for key or whole tree

  # EFFECTS
  # on ExpandLazyNode -> invoke ServerData(key) or LoadData(key)
  # on RefreshNode -> invoke ServerData(null) for roots or ServerData(key) for branch
  # on LoadChildrenFailure -> surface inline load error content

  # EVENTS
  # type TreeViewDataBindingEvent = 'DataBound' | 'LazyLoadRequested' | 'ChildrenLoaded' | 'ChildrenLoadFailed' | 'TreeRefreshed'

  @state:TreeViewDataBindingState
  @SC:01
  Scenario: SC01 - Hierarchical children bind directly
    @precondition:NestedDataAvailable
    Given roots contain nested children through a children accessor
    @action:BindHierarchicalData
    @param:roots=["Categories","Trash"]
    When TreeView binds hierarchical data
    @postcondition:NestedRootsBound
    Then root node "Categories" should render at depth 0 @UAC:01
    And child node "Social" should render at depth 1 when "Categories" is expanded @UAC:02
    And leaf node "Trash" should not render an expand affordance @UAC:03
    @postevent:DataBound
    And DataBound should include source "hierarchical" @UAC:04

  @state:TreeViewDataBindingState
  @SC:02
  Scenario: SC02 - Level descriptors map text and children per hierarchy level
    @precondition:LevelDescriptorsConfigured
    Given level 0 uses text property "Name" and children property "Orders"
    And level 1 uses text property "OrderId" and children property "Lines"
    @action:BindLevelDescriptors
    @param:levels=["Customer","Order","Line"]
    When TreeView renders with level descriptors
    @postcondition:LevelSpecificMappingApplied
    Then customer nodes should use the level 0 text selector @UAC:05
    And order nodes should use the level 1 text selector @UAC:06
    And each level should be able to supply its own icon and template metadata @UAC:07
    @postevent:DataBound
    And DataBound should include source "levels" @UAC:08

  @state:TreeViewDataBindingState
  @SC:03
  Scenario: SC03 - Self-referencing rows group by parent id
    @precondition:FlatRowsAvailable
    Given flat rows contain "1" with no parent
    And flat rows contain "1.1" with parent "1"
    @action:BindSelfReferencingData
    @param:parentKeyProperty="ParentId"
    When TreeView binds self-referencing data
    @postcondition:RowsGroupedIntoTree
    Then row "1" should render as a root @UAC:09
    And row "1.1" should render as a child of "1" @UAC:10
    And orphan rows should render according to the configured orphan policy @UAC:11
    @postevent:DataBound
    And DataBound should include source "self-referencing" @UAC:12

  @state:TreeViewDataBindingState
  @SC:04
  Scenario: SC04 - Lazy branch expansion loads children once
    @precondition:LazyNodeAvailable
    Given node "src" has hasChildren true and loaded false
    @action:ExpandLazyNode
    @param:key="src"
    When the user expands "src"
    @postcondition:LazyLoadStarted
    Then loadingKeys should contain "src" @UAC:13
    And a loading indicator should render inside the "src" branch @UAC:14
    @postevent:LazyLoadRequested
    And LazyLoadRequested should include key "src" @UAC:15
    @action:LoadChildrenSuccess
    @param:key="src"
    And the loader returns children "Components" and "Tests"
    @postcondition:LazyLoadCompleted
    Then loadedKeys should contain "src" @UAC:16
    And children "Components" and "Tests" should render under "src" @UAC:17
    And expanding "src" again should not reload children unless refresh is requested @UAC:18
    @postevent:ChildrenLoaded
    And ChildrenLoaded should include child count 2 @UAC:19

  @state:TreeViewDataBindingState
  @SC:05
  Scenario: SC05 - Refresh clears cached lazy data
    @precondition:NodeAlreadyLoaded
    Given node "src" exists in loadedKeys
    @action:RefreshNode
    @param:key="src"
    When TreeView refreshes node "src"
    @postcondition:CacheInvalidated
    Then loadedKeys should not contain "src" @UAC:20
    And the next expansion should request children again @UAC:21
    @postevent:TreeRefreshed
    And TreeRefreshed should include key "src" @UAC:22

