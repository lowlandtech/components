Feature: US13 - Responsive Layout Grid Components
    As a developer building responsive dashboard and layout applications
    I want a drag-and-drop grid layout system with resizable sections
    So that I can create flexible, customizable layouts for widgets, dashboards, and responsive designs

    Background:
        Given I have the Grid component available
        And the GridSection component is available

    # Core Rendering and Structure
    Scenario: SC01 - Grid renders with basic configuration
        Given I have a Grid specification with Row and Column properties
        When I render the Grid with rows set to 4 and columns set to 4
        Then the grid should render a 4x4 grid container
        And it should generate appropriate CSS grid styles
        And it should be ready to accept GridSection children

    Scenario: SC02 - Grid accepts child GridSection components
        Given I have a Grid with multiple GridSection children
        When I render the Grid
        Then each GridSection should register with the parent Grid
        And sections should position according to their Row and Column properties
        And the Grid should track all registered sections

    Scenario: SC03 - Grid supports custom dimensions
        Given I have a Grid specification
        When I set Row to 6 and Column to 12
        Then the grid should render a 6x12 layout
        And sections should be able to position anywhere within those bounds

    Scenario: SC04 - Grid supports height configuration
        Given I have a Grid with Height property set
        When I set Height to "600px" or "80vh"
        Then the grid container should have that height
        And sections should fill vertically within that space

    # GridSection - Basic Positioning
    Scenario: SC05 - GridSection positions at specific row and column
        Given I have a GridSection with Row=2 and Column=3
        When I render the GridSection inside a Grid
        Then the section should start at row 2, column 3
        And it should apply correct grid-area CSS

    Scenario: SC06 - GridSection supports column span
        Given I have a GridSection with ColSpan=2
        When I render the GridSection
        Then the section should span 2 columns horizontally
        And it should occupy the correct grid cells

    Scenario: SC07 - GridSection supports row span
        Given I have a GridSection with RowSpan=3
        When I render the GridSection
        Then the section should span 3 rows vertically
        And it should occupy the correct grid cells

    Scenario: SC08 - GridSection supports both row and column spans
        Given I have a GridSection with Row=1, Column=1, RowSpan=2, ColSpan=3
        When I render the GridSection
        Then it should start at position (1,1)
        And it should span 2 rows and 3 columns
        And it should occupy 6 total grid cells

    # Drag and Drop - Section Movement
    Scenario: SC09 - GridSection can be dragged to new position
        Given I have a GridSection with Movable=true
        When I drag the section from (1,1) to (2,3)
        Then the section should move to the new position
        And the Row property should update to 2
        And the Column property should update to 3
        And the OnManualPositionChanged event should fire

    Scenario: SC10 - GridSection drag handles overlap detection
        Given I have multiple GridSections in a Grid
        And one section is Movable
        When I drag a section over another section's space
        Then the Grid should detect the overlap
        And if ResolveCollisions is true, it should push other sections
        And sections should not overlap

    Scenario: SC11 - GridSection respects movement boundaries
        Given I have a GridSection with Movable=true
        And the Grid has 4 rows and 4 columns
        When I try to drag the section outside grid boundaries
        Then the section should snap to grid boundaries
        And it should not exceed Row or Column limits

    Scenario: SC12 - GridSection movement can be locked on X-axis
        Given I have a GridSection with Movable=true and LockX=true
        When I drag the section
        Then it should only move vertically (Y-axis)
        And horizontal position should remain constant

    Scenario: SC13 - GridSection movement can be locked on Y-axis
        Given I have a GridSection with Movable=true and LockY=true
        When I drag the section
        Then it should only move horizontally (X-axis)
        And vertical position should remain constant

    # Resize Functionality
    Scenario: SC14 - GridSection can be resized from corner handle
        Given I have a GridSection with Resizable=true
        When I render the GridSection
        Then resize handles should appear (SE, E, S corners)
        When I drag the SE (southeast) handle
        Then the section should resize both horizontally and vertically
        And ColSpan and RowSpan should update

    Scenario: SC15 - GridSection can be resized horizontally only
        Given I have a GridSection with Resizable=true
        When I drag the E (east) resize handle
        Then the section should resize horizontally only
        And ColSpan should increase/decrease
        And RowSpan should remain constant

    Scenario: SC16 - GridSection can be resized vertically only
        Given I have a GridSection with Resizable=true
        When I drag the S (south) resize handle
        Then the section should resize vertically only
        And RowSpan should increase/decrease
        And ColSpan should remain constant

    Scenario: SC17 - GridSection respects minimum and maximum column span
        Given I have a GridSection with MinColSpan=2 and MaxColSpan=4
        When I resize the section horizontally
        Then ColSpan should not go below 2
        And ColSpan should not exceed 4
        And resize should stop at these boundaries

    Scenario: SC18 - GridSection respects minimum and maximum row span
        Given I have a GridSection with MinRowSpan=1 and MaxRowSpan=3
        When I resize the section vertically
        Then RowSpan should not go below 1
        And RowSpan should not exceed 3

    Scenario: SC19 - GridSection respects minimum and maximum position
        Given I have a GridSection with MinRow=2, MaxRow=4, MinCol=1, MaxCol=3
        When I move or resize the section
        Then Row should stay between 2 and 4
        And Column should stay between 1 and 3

    # Collision Resolution and Compaction
    Scenario: SC20 - Grid resolves collisions when ResolveCollisions enabled
        Given I have a Grid with ResolveCollisions=true
        And multiple GridSections are positioned
        When I move a section that would overlap another
        Then the Grid should push overlapping sections out of the way
        And all sections should remain in valid positions

    Scenario: SC21 - Grid allows overlaps when ResolveCollisions disabled
        Given I have a Grid with ResolveCollisions=false
        When I move a section that overlaps another
        Then sections can overlap
        And no automatic repositioning occurs

    Scenario: SC22 - Grid compacts sections when CompactOnDrop enabled
        Given I have a Grid with CompactOnDrop=true
        And sections have gaps above them
        When I drop a moved section
        Then all sections should compact upward
        And gaps should be eliminated

    Scenario: SC23 - Grid supports float mode for automatic compaction
        Given I have a Grid with FloatMode=true
        When space frees up above a section
        Then sections should float upward to fill gaps
        And the layout should remain compact

    Scenario: SC24 - Grid can be manually compacted
        Given I have a Grid with scattered sections
        When I call the Compact() method
        Then all sections should move as far up as possible
        And gaps should be minimized

    # Layout Persistence
    Scenario: SC25 - Grid can save layout to JSON
        Given I have a Grid with multiple positioned sections
        When I call SaveLayoutAsync()
        Then it should return a JSON string
        And the JSON should contain grid dimensions (rows, columns)
        And it should contain all section positions and spans
        And each section should be identified by its Id

    Scenario: SC26 - Grid can restore layout from JSON
        Given I have saved layout JSON
        When I call RestoreLayoutAsync(json)
        Then the Grid should restore to saved dimensions
        And all sections should restore to their saved positions
        And all spans should match the saved values

    Scenario: SC27 - Grid layout persists section identifiers
        Given I have GridSections with specific Id values
        When I save and restore the layout
        Then sections should restore based on their Id
        And the correct sections should be in the correct positions

    # Animation and Visual Feedback
    Scenario: SC28 - GridSection animates during move and resize
        Given I have a GridSection with Animate=true
        When I drag or resize the section
        Then the position/size changes should animate smoothly
        And the animation duration should match AnimationMs property

    Scenario: SC29 - GridSection animation can be disabled
        Given I have a GridSection with Animate=false
        When I drag or resize the section
        Then changes should be instant without animation

    Scenario: SC30 - GridSection shows visual feedback during drag
        Given I have a GridSection with Movable=true
        When I start dragging the section
        Then visual feedback should indicate it's being dragged
        And a placeholder or overlay should show the grid

    # GridSection Content
    Scenario: SC31 - GridSection renders child content
        Given I have a GridSection with ChildContent
        When I render the GridSection
        Then the child content should render inside the section
        And I can place any Blazor components as children

    Scenario: SC32 - GridSection supports GroupBox wrapping
        Given I have a GridSection with WrapInGroupBox=true and Title="My Widget"
        When I render the GridSection
        Then the content should be wrapped in a MudExGroupBox
        And the GroupBox should display "My Widget" as the title
        And it should provide a bordered container with title

    Scenario: SC33 - GridSection without GroupBox renders content directly
        Given I have a GridSection with WrapInGroupBox=false
        When I render the GridSection
        Then child content should render directly without wrapper
        And no GroupBox border or title should appear

    # Events and Callbacks
    Scenario: SC34 - GridSection fires position change events
        Given I have a GridSection with RowChanged and ColumnChanged callbacks
        When the section's position changes via drag
        Then RowChanged should fire with the new row value
        And ColumnChanged should fire with the new column value

    Scenario: SC35 - GridSection fires span change events
        Given I have a GridSection with RowSpanChanged and ColSpanChanged callbacks
        When the section's size changes via resize
        Then RowSpanChanged should fire with the new row span
        And ColSpanChanged should fire with the new column span

    Scenario: SC36 - GridSection fires manual position changed event
        Given I have a GridSection with OnManualPositionChanged callback
        When the user manually moves or resizes the section
        Then OnManualPositionChanged should fire
        And the event args should include Row, Column, RowSpan, ColSpan
        And the section Id should be included

    Scenario: SC37 - GridSection supports click events
        Given I have a GridSection with OnClick callback
        When I click on the section
        Then the OnClick event should fire
        And event args should provide mouse event details

    Scenario: SC38 - GridSection supports double-click events
        Given I have a GridSection with OnDblClick callback
        When I double-click on the section
        Then the OnDblClick event should fire
        And I can implement custom double-click behavior

    Scenario: SC39 - GridSection supports context menu events
        Given I have a GridSection with OnContextMenu callback
        When I right-click on the section
        Then the OnContextMenu event should fire
        And I can display a custom context menu

    # Grid-Level Events
    Scenario: SC40 - Grid supports click events
        Given I have a Grid with OnClick callback
        When I click on the grid (not on a section)
        Then the Grid OnClick event should fire
        And I can handle grid-level interactions

    Scenario: SC41 - Grid supports context menu events
        Given I have a Grid with OnContextMenu callback
        When I right-click on the grid
        Then the OnContextMenu event should fire

    Scenario: SC42 - Grid commits layout changes from JavaScript
        Given I have sections being dragged/resized
        When the JavaScript interaction completes
        Then the Grid's CommitLayout method should be called
        And all section positions should update
        And StateHasChanged should be called to re-render

    # Automatic Placement
    Scenario: SC43 - Grid can find next free space
        Given I have a Grid with some sections placed
        When I call GetNextFreeSpace()
        Then it should return the next available (Row, Column) position
        And the position should not be occupied by any section

    Scenario: SC44 - Grid returns null when no free space available
        Given I have a Grid completely filled with sections
        When I call GetNextFreeSpace()
        Then it should return null
        And it indicates no available positions

    # Programmatic Section Management
    Scenario: SC45 - Grid tracks all child sections
        Given I have a Grid with multiple sections
        When I call GetAllChildSections()
        Then it should return an array of all registered sections
        And the count should match the number of child sections

    Scenario: SC46 - Grid registers sections on initialization
        Given I have a GridSection
        When the section initializes inside a Grid
        Then it should automatically register with the Grid
        And the Grid should track it in its internal collection

    Scenario: SC47 - Grid unregisters sections on disposal
        Given I have a GridSection that is registered
        When the section is disposed or removed
        Then it should unregister from the Grid
        And the Grid should remove it from its collection

    # Responsive and Styling
    Scenario: SC48 - Grid applies custom CSS classes
        Given I have a Grid with Class property set
        When I render the Grid
        Then the custom CSS class should be applied
        And I can style the grid with custom CSS

    Scenario: SC49 - Grid applies custom inline styles
        Given I have a Grid with Style property set
        When I render the Grid
        Then the custom inline styles should be applied
        And they should merge with component styles

    Scenario: SC50 - GridSection applies custom CSS classes
        Given I have a GridSection with Class property
        When I render the GridSection
        Then the custom CSS class should be applied to the section
        And I can style individual sections differently

    Scenario: SC51 - GridSection applies custom inline styles
        Given I have a GridSection with Style property
        When I render the GridSection
        Then the custom inline styles should be applied
        And animation styles should merge correctly

    Scenario: SC52 - Grid generates unique section IDs
        Given I have GridSections without explicit Id values
        When sections initialize
        Then the Grid should generate unique IDs for each section
        And IDs should follow the pattern "grid-id-section-N"

    # Advanced Scenarios
    Scenario: SC53 - Multiple sections can be moved simultaneously
        Given I have multiple GridSections with Movable=true
        When I move one section
        And it pushes other sections (if ResolveCollisions=true)
        Then the Grid should update all affected section positions
        And the CommitLayout should handle batch updates

    Scenario: SC54 - Grid prevents invalid placements
        Given I have a GridSection trying to position at invalid location
        When the section would exceed grid boundaries
        Then the Grid CanPlace validation should prevent it
        And the section should remain in a valid position

    Scenario: SC55 - Grid handles dynamic section addition
        Given I have an existing Grid with sections
        When I dynamically add a new GridSection component
        Then the new section should register with the Grid
        And it should position according to its properties
        And existing sections should adjust if needed

    Scenario: SC56 - Grid handles dynamic section removal
        Given I have a Grid with multiple sections
        When I dynamically remove a GridSection component
        Then the section should unregister from the Grid
        And if CompactOnDrop or FloatMode is enabled, other sections should adjust
        And the gap should be filled or remain as desired

    # Edge Cases and Validation
    Scenario: SC57 - Grid handles overlapping manual positions gracefully
        Given I have two GridSections with overlapping positions
        And ResolveCollisions is false
        When I render the Grid
        Then both sections should render
        And they may visually overlap (stacking based on DOM order)

    Scenario: SC58 - GridSection handles boundary resize attempts
        Given I have a GridSection at the bottom-right of the grid
        When I try to resize beyond grid boundaries
        Then resize should stop at the grid edge
        And the section should not exceed grid dimensions

    Scenario: SC59 - Grid suspend/resume during layout changes
        Given the Grid is processing layout changes
        When JavaScript commits multiple changes rapidly
        Then the Grid should suspend layout updates temporarily
        And batch changes should be applied together
        And StateHasChanged should be called once at the end

    Scenario: SC60 - Grid supports user-defined section identifiers
        Given I create GridSections with custom Id values
        When I save and restore layouts
        Then custom Ids should be preserved
        And I can reliably identify specific sections across sessions
