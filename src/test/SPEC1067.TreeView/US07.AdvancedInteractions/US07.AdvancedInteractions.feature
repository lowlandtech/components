@SPEC:1067
@US:07
Feature: SPEC1067.US07 - TreeView Advanced Interactions
  As an application author
  I want TreeView to expose drag and drop, context menu, file system, and refresh interaction hooks
  So that it can cover Radzen Tree drag/drop, context menu, file binding, and refresh data binding demos

  Background:
    Given the component library LowlandTech.Components.TreeView
    And TreeView nodes have stable keys, values, and optional payload data

  # STATE
  # interface TreeViewAdvancedState {
  #   draggedKey: string | null
  #   dropTargetKey: string | null
  #   dropPosition: 'before' | 'inside' | 'after' | null
  #   contextMenuKey: string | null
  #   refreshedKeys: string[]
  #   fileSystemRoot: string | null
  #   nodes: TreeNodeRepresentation[]
  # }

  # FORM
  # interface TreeViewAdvancedForm {
  #   sourceKey: string
  #   targetKey: string
  #   dropPosition: 'before' | 'inside' | 'after'
  #   pointerX: number
  #   pointerY: number
  #   fileSystemPath: string
  # }

  # REPRESENTATION
  # interface TreeNodeRepresentation {
  #   key: string
  #   text: string
  #   value: unknown
  #   parentKey: string | null
  #   children: TreeNodeRepresentation[]
  #   canDrag: boolean
  #   canDrop: boolean
  #   canOpenContextMenu: boolean
  # }

  # ACTIONS
  # type TreeViewAdvancedAction =
  #   | { type: 'BeginDragNode'; sourceKey: string }
  #   | { type: 'DragOverNode'; targetKey: string; dropPosition: TreeViewAdvancedState['dropPosition'] }
  #   | { type: 'DropNode'; sourceKey: string; targetKey: string; dropPosition: TreeViewAdvancedState['dropPosition'] }
  #   | { type: 'OpenNodeContextMenu'; key: string; pointerX: number; pointerY: number }
  #   | { type: 'RefreshTree'; key: string | null }
  #   | { type: 'BindFileSystemRoot'; path: string }

  # REDUCER TRANSITIONS
  # BeginDragNode -> draggedKey set
  # DragOverNode -> drop target and position set when target accepts the source
  # DropNode -> node collection reordered or reparented, drag state cleared
  # OpenNodeContextMenu -> contextMenuKey and pointer coordinates set
  # RefreshTree -> refreshedKeys updated and matching data cache invalidated
  # BindFileSystemRoot -> roots populated from directory entries

  # EFFECTS
  # on DropNode -> invoke NodeDropped callback with source, target, and position
  # on OpenNodeContextMenu -> invoke ItemContextMenu callback and let caller show menu
  # on RefreshTree -> invoke reload delegate for whole tree or branch
  # on BindFileSystemRoot -> enumerate directory and file children through caller service

  # EVENTS
  # type TreeViewAdvancedEvent = 'NodeDragStarted' | 'NodeDropTargetChanged' | 'NodeDropped' | 'NodeContextMenuRequested' | 'TreeRefreshed' | 'FileSystemBound'

  @state:TreeViewAdvancedState
  @SC:01
  Scenario: SC01 - Dragging a node onto another node emits a drop event
    @precondition:DragDropEnabled
    Given node "drafts" can be dragged
    And node "archive" can accept drops
    @action:BeginDragNode
    @param:sourceKey="drafts"
    When the user starts dragging "drafts"
    @postcondition:DragStarted
    Then draggedKey should be "drafts" @UAC:01
    And the dragged node should expose dragging state @UAC:02
    @postevent:NodeDragStarted
    And NodeDragStarted should include sourceKey "drafts" @UAC:03
    @action:DropNode
    @param:sourceKey="drafts"
    @param:targetKey="archive"
    @param:dropPosition="inside"
    When the user drops "drafts" inside "archive"
    @postcondition:NodeReparented
    Then "drafts" should become a child of "archive" when the drop is accepted @UAC:04
    And draggedKey should be null @UAC:05
    @postevent:NodeDropped
    And NodeDropped should include sourceKey "drafts", targetKey "archive", and dropPosition "inside" @UAC:06

  @state:TreeViewAdvancedState
  @SC:02
  Scenario: SC02 - Invalid drop targets are rejected without changing the tree
    @precondition:InvalidDropTarget
    Given node "archive" cannot be dropped onto its own descendant "archive.2026"
    @action:DragOverNode
    @param:targetKey="archive.2026"
    @param:dropPosition="inside"
    When the user drags "archive" over "archive.2026"
    @postcondition:DropRejected
    Then dropTargetKey should remain null @UAC:07
    And the tree order should remain unchanged @UAC:08
    And the target should expose a rejected drop state for styling @UAC:09

  @state:TreeViewAdvancedState
  @SC:03
  Scenario: SC03 - Right-click requests a context menu for a node
    @precondition:ContextMenuEnabled
    Given node "orders" can open a context menu
    @action:OpenNodeContextMenu
    @param:key="orders"
    @param:pointerX=320
    @param:pointerY=180
    When the user opens the context menu for "orders"
    @postcondition:ContextMenuRequested
    Then contextMenuKey should be "orders" @UAC:10
    And the context menu event should include pointer coordinates @UAC:11
    And default browser context menu should be preventable by the caller @UAC:12
    @postevent:NodeContextMenuRequested
    And NodeContextMenuRequested should include key "orders" @UAC:13

  @state:TreeViewAdvancedState
  @SC:04
  Scenario: SC04 - Refreshing the tree reloads current data without losing controlled state
    @precondition:TreeLoaded
    Given expandedKeys contains "src"
    And selectedKey is "src/TreeView.razor"
    @action:RefreshTree
    @param:key=null
    When the application refreshes the whole tree
    @postcondition:TreeReloaded
    Then refreshedKeys should include the root refresh marker @UAC:14
    And expandedKeys should be restored for keys still present after reload @UAC:15
    And selectedKey should be retained when the selected node still exists @UAC:16
    @postevent:TreeRefreshed
    And TreeRefreshed should include scope "root" @UAC:17

  @state:TreeViewAdvancedState
  @SC:05
  Scenario: SC05 - File system binding represents directories and files
    @precondition:FileSystemProviderConfigured
    Given the file system root is "samples"
    @action:BindFileSystemRoot
    @param:path="samples"
    When TreeView binds the file system root
    @postcondition:FileSystemNodesRendered
    Then directories should render as expandable nodes @UAC:18
    And files should render as leaf nodes @UAC:19
    And file and directory icons should be distinguishable @UAC:20
    And file system exceptions should render an error node instead of breaking the tree @UAC:21
    @postevent:FileSystemBound
    And FileSystemBound should include root path "samples" @UAC:22

