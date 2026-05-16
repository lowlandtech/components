@SPEC:1067
@US:05
Feature: SPEC1067.US05 - TreeView Filtering and Search
  As an application author
  I want TreeView to filter nodes while preserving matching ancestors and useful expansion state
  So that it can match Radzen tree filtering and MudBlazor FilterFunc behavior

  Background:
    Given the component library LowlandTech.Components.TreeView
    And tree nodes can be searched by text, custom predicate, or asynchronously evaluated filter

  # STATE
  # interface TreeViewFilterState {
  #   query: string
  #   filterMode: 'none' | 'text' | 'predicate' | 'asyncPredicate'
  #   visibleKeys: string[]
  #   matchedKeys: string[]
  #   expandedKeys: string[]
  #   previousExpandedKeys: string[]
  #   loadingFilter: boolean
  # }

  # FORM
  # interface TreeViewFilterForm {
  #   query: string
  #   matchCase: boolean
  #   expandMatches: boolean
  #   includeAncestors: boolean
  # }

  # REPRESENTATION
  # interface TreeFilterResultRepresentation {
  #   key: string
  #   matched: boolean
  #   ancestorOfMatch: boolean
  #   visible: boolean
  #   expandedByFilter: boolean
  # }

  # ACTIONS
  # type TreeViewFilterAction =
  #   | { type: 'SetFilterQuery'; query: string }
  #   | { type: 'ApplyTextFilter'; query: string; matchCase: boolean }
  #   | { type: 'ApplyPredicateFilter' }
  #   | { type: 'ApplyAsyncPredicateFilter' }
  #   | { type: 'ClearFilter' }

  # REDUCER TRANSITIONS
  # SetFilterQuery -> query updated
  # ApplyTextFilter -> matchedKeys and visibleKeys recalculated
  # ApplyPredicateFilter -> matchedKeys from custom predicate
  # ApplyAsyncPredicateFilter -> loadingFilter true until predicate resolves
  # ClearFilter -> query cleared, matchedKeys cleared, previous expansion restored

  # EFFECTS
  # on ApplyAsyncPredicateFilter -> evaluate predicate for each loaded node
  # on ClearFilter -> restore expansion state that existed before filtering

  # EVENTS
  # type TreeViewFilterEvent = 'FilterChanged' | 'FilterApplied' | 'FilterCleared'

  @state:TreeViewFilterState
  @SC:01
  Scenario: SC01 - Text filter shows matching descendants and their ancestors
    @precondition:TreeContainsMatches
    Given the tree contains "Categories" with child "Social"
    And the query is "social"
    @action:ApplyTextFilter
    @param:query="social"
    @param:matchCase=false
    When TreeView applies the text filter
    @postcondition:MatchesAndAncestorsVisible
    Then matchedKeys should contain "Social" @UAC:01
    And visibleKeys should contain ancestor "Categories" @UAC:02
    And visibleKeys should not contain unrelated leaf "Trash" @UAC:03
    And ancestor "Categories" should expand while the filter is active @UAC:04
    @postevent:FilterApplied
    And FilterApplied should include query "social" @UAC:05

  @state:TreeViewFilterState
  @SC:02
  Scenario: SC02 - Predicate filter can match arbitrary node data
    @precondition:PredicateConfigured
    Given a predicate matches nodes with unread count greater than 100
    @action:ApplyPredicateFilter
    When TreeView applies the predicate filter
    @postcondition:PredicateMatchesVisible
    Then matchedKeys should contain every node whose unread count is greater than 100 @UAC:06
    And nonmatching ancestors of matched nodes should remain visible @UAC:07
    And matched nodes should expose a matched state for styling @UAC:08
    @postevent:FilterApplied
    And FilterApplied should include mode "predicate" @UAC:09

  @state:TreeViewFilterState
  @SC:03
  Scenario: SC03 - Async predicate exposes loading state and applies results after completion
    @precondition:AsyncPredicateConfigured
    Given the filter predicate returns a task
    @action:ApplyAsyncPredicateFilter
    When TreeView starts evaluating the async filter
    @postcondition:FilterLoading
    Then loadingFilter should be true @UAC:10
    And the tree should keep the previous visible nodes until results resolve @UAC:11
    @postcondition:AsyncFilterApplied
    Then loadingFilter should be false after all predicates complete @UAC:12
    And visibleKeys should reflect the async predicate results @UAC:13
    @postevent:FilterApplied
    And FilterApplied should include mode "asyncPredicate" @UAC:14

  @state:TreeViewFilterState
  @SC:04
  Scenario: SC04 - Clearing a filter restores prior expansion state
    @precondition:FilterActive
    Given the active query is "social"
    And previousExpandedKeys contains "Mail"
    @action:ClearFilter
    When the user clears the filter
    @postcondition:FilterCleared
    Then query should be empty @UAC:15
    And matchedKeys should be empty @UAC:16
    And visibleKeys should contain all unfiltered loaded nodes @UAC:17
    And expandedKeys should restore "Mail" without retaining filter-only expansions @UAC:18
    @postevent:FilterCleared
    And FilterCleared should include restored expansion count @UAC:19

  @state:TreeViewFilterState
  @SC:05
  Scenario: SC05 - No matches render an empty filter state without destroying data
    @precondition:NoMatchingNodes
    Given the query is "does-not-exist"
    @action:ApplyTextFilter
    @param:query="does-not-exist"
    @param:matchCase=false
    When TreeView applies the text filter
    @postcondition:NoMatchesVisible
    Then visibleKeys should be empty @UAC:20
    And an empty filter slot should render if provided @UAC:21
    And clearing the filter should restore the original tree data @UAC:22

