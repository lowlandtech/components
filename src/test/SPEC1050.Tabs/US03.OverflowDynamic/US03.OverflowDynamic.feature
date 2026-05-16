@SPEC:1050
@US:03
Feature: SPEC1050.US03 - Overflow and Dynamic IDE Tabs
  As an application author building dense editor-like surfaces
  I want Tabs to support overflow controls, closeable tabs, dynamic item collections, pinning, drag/drop reordering, IDE styling, and renaming
  So that tab headers remain usable inside constrained parent layouts

  Background:
    Given the component library LowlandTech.Components.Tabs
    And Tabs can be composed statically or rendered from a dynamic item collection

  # STATE
  # interface TabsDynamicState {
  #   items: TabsItemRepresentation[]
  #   activeValue: string | null
  #   focusedValue: string | null
  #   draggedValue: string | null
  #   overflow: 'unknown' | 'fits' | 'overflowing'
  #   renameValue: string | null
  # }

  # FORM
  # interface TabsDynamicForm {
  #   newTitle: string
  #   scrollDirection: 'back' | 'next' | null
  #   sourceValue: string | null
  #   targetValue: string | null
  # }

  # REPRESENTATION
  # interface TabsItemRepresentation {
  #   value: string
  #   title: string
  #   disabled: boolean
  #   closeable: boolean
  #   pinned: boolean
  #   pinnable: boolean
  #   renameable: boolean
  #   draggable: boolean
  # }

  # ACTIONS
  # type TabsDynamicAction =
  #   | { type: 'RenderOverflowStrip'; availableWidth: number; headerWidth: number }
  #   | { type: 'CloseTab'; value: string }
  #   | { type: 'TogglePinned'; value: string }
  #   | { type: 'BeginDrag'; value: string }
  #   | { type: 'DropTab'; sourceValue: string; targetValue: string }
  #   | { type: 'BeginRename'; value: string }
  #   | { type: 'CommitRename'; value: string; title: string }

  # REDUCER TRANSITIONS
  # RenderOverflowStrip -> overflow updated and header remains clipped to parent
  # CloseTab -> item removed, activeValue moved to nearest available tab when needed
  # TogglePinned -> item pinned state toggled and pinned items render first
  # BeginDrag -> draggedValue set
  # DropTab -> items reordered and draggedValue cleared
  # BeginRename -> renameValue set
  # CommitRename -> item title updated and renameValue cleared

  # EFFECTS
  # on CloseTab -> ItemsChanged and TabClosed callbacks receive updated state
  # on TogglePinned -> ItemsChanged and TabPinnedChanged callbacks receive updated state
  # on DropTab -> ItemsChanged and TabReordered callbacks receive old/new indexes
  # on CommitRename -> ItemsChanged and TabRenamed callbacks receive old/new title

  # EVENTS
  # type TabsDynamicEvent = 'TabsOverflowRendered' | 'TabClosed' | 'TabPinnedChanged' | 'TabReordered' | 'TabRenameStarted' | 'TabRenamed'

  @state:TabsDynamicState
  @SC:01
  Scenario: SC01 - Overflowing horizontal tabs show scroll buttons and stay inside parent
    @precondition:HorizontalTabs
    Given tabs are rendered in horizontal orientation
    @precondition:HeaderOverflowing
    And the total tab header width is wider than the parent header
    @action:RenderOverflowStrip
    @param:availableWidth=480
    @param:headerWidth=960
    When Tabs renders the tab list
    @postcondition:OverflowContained
    Then the tab list should render inside a viewport that clips horizontal overflow @UAC:01
    And back and next scroll buttons should be hidden before overflow is detected @UAC:02
    And back and next scroll buttons should be visible after overflow is detected @UAC:03
    And tabs should not visually flow outside the parent header @UAC:04
    @postevent:TabsOverflowRendered
    And TabsOverflowRendered should include overflow "overflowing" @UAC:24

  @state:TabsDynamicState
  @SC:02
  Scenario: SC02 - Closeable dynamic tabs remove the requested tab
    @precondition:CloseableTabExists
    Given TabsDynamicState.items contains a closeable tab "orders"
    @action:CloseTab
    @param:value="orders"
    When the user clicks the close action for "orders"
    @postcondition:TabRemoved
    Then TabsDynamicState.items should not contain "orders" @UAC:05
    And ItemsChanged should receive the remaining items @UAC:06
    @postevent:TabClosed
    And TabClosed should include value "orders" @UAC:07

  @state:TabsDynamicState
  @SC:03
  Scenario: SC03 - Closing the active tab activates a nearby enabled tab
    @precondition:ActiveTabCloseable
    Given TabsDynamicState.activeValue is "orders"
    And "orders" is closeable
    @action:CloseTab
    @param:value="orders"
    When the user closes "orders"
    @postcondition:ActiveTabReassigned
    Then TabsDynamicState.activeValue should move to the nearest enabled tab @UAC:08
    And ValueChanged should receive the new active value @UAC:09

  @state:TabsDynamicState
  @SC:04
  Scenario: SC04 - Pinnable tabs toggle pinned state and render before unpinned tabs
    @precondition:PinnableTabExists
    Given TabsDynamicState.items contains pinnable tab "readme"
    @action:TogglePinned
    @param:value="readme"
    When the user clicks the pin action for "readme"
    @postcondition:TabPinned
    Then tab "readme" should have pinned true @UAC:10
    And pinned tabs should render before unpinned tabs while preserving relative order @UAC:11
    @postevent:TabPinnedChanged
    And TabPinnedChanged should include value "readme" @UAC:12

  @state:TabsDynamicState
  @SC:05
  Scenario: SC05 - Dragging a tab onto another tab reorders the dynamic collection
    @precondition:DragEnabled
    Given TabsDynamicState.items contains "one", "two", and "three"
    @action:BeginDrag
    @param:value="three"
    When the user starts dragging "three"
    @postcondition:DragStarted
    Then TabsDynamicState.draggedValue should be "three" @UAC:13
    @action:DropTab
    @param:sourceValue="three"
    @param:targetValue="one"
    When the user drops "three" on "one"
    @postcondition:TabsReordered
    Then TabsDynamicState.items should be ordered "three", "one", "two" @UAC:14
    And TabsDynamicState.draggedValue should be null @UAC:15
    @postevent:TabReordered
    And TabReordered should include old and new indexes @UAC:16

  @state:TabsDynamicState
  @SC:06
  Scenario: SC06 - Renameable tabs commit a new header title
    @precondition:RenameableTabExists
    Given TabsDynamicState.items contains renameable tab "draft"
    @action:BeginRename
    @param:value="draft"
    When the user starts renaming "draft"
    @postcondition:RenameInputActive
    Then TabsDynamicState.renameValue should be "draft" @UAC:17
    @action:CommitRename
    @param:value="draft"
    @param:title="Final copy"
    When the user commits title "Final copy"
    @postcondition:TabTitleUpdated
    Then tab "draft" should have title "Final copy" @UAC:18
    And ItemsChanged should receive the renamed item @UAC:19
    @postevent:TabRenamed
    And TabRenamed should include old and new title @UAC:20

  @state:TabsDynamicState
  @SC:07
  Scenario: SC07 - IDE variant gives the active tab an editor-style surface
    @precondition:IdeVariantEnabled
    Given Tabs renders dynamic items with variant "ide"
    @precondition:ActiveTabAvailable
    And TabsDynamicState.activeValue is "program"
    @action:RenderOverflowStrip
    @param:availableWidth=640
    @param:headerWidth=640
    When Tabs renders the IDE tab header
    @postcondition:IdeActiveTabVisible
    Then active tab "program" should connect visually to the content surface @UAC:21
    And inactive tabs should remain visually recessed @UAC:22
    And close and pin actions should not change the active tab when clicked @UAC:23
    And hovering a tab header should keep the default cursor @UAC:25
