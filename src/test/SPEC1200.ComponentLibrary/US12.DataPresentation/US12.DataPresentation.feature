Feature: US12 - Data Presentation Components
    As a developer building enterprise applications with complex data requirements
    I want a powerful and flexible data grid component
    So that I can display, filter, sort, group, edit, and export tabular data efficiently

    Background:
        Given I have a collection of data items to display
        And the DataGrid component is available

    # Core Rendering and Display
    Scenario: SC01 - DataGrid renders basic table structure
        Given I have a DataGrid specification with basic columns
        When I render the DataGrid with a collection of items
        Then the grid should render a table element
        And it should display column headers
        And it should render one row per data item
        And each cell should display the correct property value

    Scenario: SC02 - DataGrid supports column templates
        Given I have a DataGrid specification
        When I define a column with a custom Template
        Then the column should render using the custom template
        And template context should provide access to the row item
        And I should be able to use any Blazor markup in the template

    Scenario: SC03 - DataGrid displays empty state
        Given I have a DataGrid specification
        When I render the DataGrid with an empty collection
        Then the grid should display the empty template
        And the table body should indicate no data available

    Scenario: SC04 - DataGrid supports header customization
        Given I have a DataGrid specification with HeaderTemplate
        When I render the DataGrid
        Then the custom header should appear above the grid
        And it should span the full grid width

    # Sorting
    Scenario: SC05 - DataGrid supports single column sorting
        Given I have a DataGrid specification with AllowSorting enabled
        When I click on a sortable column header
        Then the data should sort by that column in ascending order
        And the header should display a sort indicator
        When I click the same header again
        Then the data should sort in descending order
        And the sort indicator should reflect the direction

    Scenario: SC06 - DataGrid supports multi-column sorting
        Given I have a DataGrid specification with AllowMultiColumnSorting enabled
        When I click on multiple column headers while holding modifier key
        Then the data should sort by multiple columns in order
        And each sorted header should display sort indicators with priority numbers

    Scenario: SC07 - DataGrid supports programmatic sorting
        Given I have a DataGrid specification
        When I programmatically set sort descriptors
        Then the grid should apply the sorting
        And the headers should reflect the current sort state

    # Filtering
    Scenario: SC08 - DataGrid supports simple column filtering
        Given I have a DataGrid specification with AllowFiltering enabled
        And FilterMode is set to Simple
        When I render the DataGrid
        Then a filter row should appear below the headers
        And each filterable column should display a filter input
        When I enter a filter value
        Then the data should filter to matching rows only

    Scenario: SC09 - DataGrid supports advanced filter menu
        Given I have a DataGrid specification with FilterMode Advanced
        When I click a column's filter icon
        Then a filter menu should open
        And it should show filter operator options (equals, contains, starts with, etc.)
        And I should be able to add multiple filter conditions with AND/OR logic

    Scenario: SC10 - DataGrid supports filter templates
        Given I have a DataGrid column with FilterTemplate
        When the filter row renders
        Then the column should use the custom filter template
        And I can implement any custom filtering UI

    Scenario: SC11 - DataGrid supports programmatic filtering
        Given I have a DataGrid specification
        When I programmatically add filter descriptors
        Then the grid should apply the filters
        And the filter UI should reflect the current filters

    # Paging
    Scenario: SC12 - DataGrid supports client-side paging
        Given I have a DataGrid specification with AllowPaging enabled
        And PageSize is set to 10
        And I have 50 data items
        When I render the DataGrid
        Then only 10 rows should be visible
        And a pager should appear showing "1 of 5 pages"
        When I click the next page button
        Then rows 11-20 should be visible

    Scenario: SC13 - DataGrid pager supports top and bottom positions
        Given I have a DataGrid with PagerPosition set to TopAndBottom
        When I render the DataGrid
        Then a pager should appear above the grid
        And a pager should appear below the grid
        And both pagers should synchronize

    Scenario: SC14 - DataGrid supports page size selection
        Given I have a DataGrid with PageSizeOptions
        When I select a different page size from the dropdown
        Then the grid should display that many rows per page
        And the pager should recalculate total pages

    # Selection
    Scenario: SC15 - DataGrid supports single row selection
        Given I have a DataGrid with SelectionMode Single
        When I click on a row
        Then the row should be highlighted as selected
        And the RowSelect event should fire with the selected item
        When I click another row
        Then the previous row should be deselected
        And the new row should be selected

    Scenario: SC16 - DataGrid supports multiple row selection
        Given I have a DataGrid with SelectionMode Multiple
        When I click on multiple rows
        Then all clicked rows should be selected
        And the RowSelect event should fire for each selection
        When I click a selected row
        Then it should be deselected

    Scenario: SC17 - DataGrid supports checkbox selection column
        Given I have a DataGrid with ShowSelectColumn enabled
        When I render the DataGrid
        Then a checkbox column should appear as the first column
        And the header should show a "select all" checkbox
        When I check the header checkbox
        Then all rows should be selected

    Scenario: SC18 - DataGrid supports programmatic selection
        Given I have a DataGrid bound to a selection property
        When I programmatically set selected rows
        Then those rows should be highlighted
        And the UI should reflect the selection state

    # Grouping
    Scenario: SC19 - DataGrid supports column grouping
        Given I have a DataGrid with AllowGrouping enabled
        When I drag a column header to the group panel
        Then the data should group by that column
        And group rows should display with expand/collapse icons
        And grouped column values should appear in group headers

    Scenario: SC20 - DataGrid supports multi-level grouping
        Given I have a DataGrid with AllowGrouping enabled
        When I drag multiple columns to the group panel
        Then the data should group hierarchically
        And I should be able to expand/collapse each group level

    Scenario: SC21 - DataGrid supports group aggregates
        Given I have a DataGrid with grouped data
        And columns have aggregate functions defined (Sum, Average, Count, etc.)
        When I render the DataGrid
        Then group footer rows should display aggregate values
        And the grid footer should display grand totals

    Scenario: SC22 - DataGrid group templates
        Given I have a DataGrid with GroupHeaderTemplate
        When data is grouped
        Then group headers should use the custom template
        And I can display custom aggregate information

    # Editing
    Scenario: SC23 - DataGrid supports inline row editing (Single mode)
        Given I have a DataGrid with EditMode Single
        When I call EditRow for a specific item
        Then that row should enter edit mode
        And EditTemplate components should render in cells
        And other rows should remain in view mode

    Scenario: SC24 - DataGrid supports cell editing
        Given I have a DataGrid with EditMode Cell
        When I click on a cell
        Then only that cell should become editable
        And other cells should remain read-only

    Scenario: SC25 - DataGrid supports batch editing (Multiple mode)
        Given I have a DataGrid with EditMode Multiple
        When I call EditRow for multiple items
        Then all specified rows should enter edit mode simultaneously
        And I can edit multiple rows before saving

    Scenario: SC26 - DataGrid edit validation
        Given I have a DataGrid with edit templates containing validators
        When I edit a row with invalid data
        Then validation messages should display
        And the IsValid property should return false
        And I should not be able to save until validation passes

    Scenario: SC27 - DataGrid supports edit templates
        Given I have columns with EditTemplate defined
        When a row enters edit mode
        Then cells should render using the EditTemplate
        And I can use any input component (TextBox, NumericBox, DropDown, etc.)

    Scenario: SC28 - DataGrid insert new rows
        Given I have a DataGrid with editing enabled
        When I call InsertRow
        Then a new empty row should appear at the top
        And it should be in edit mode
        And I can fill in values and save

    Scenario: SC29 - DataGrid delete rows
        Given I have a DataGrid with row selection
        When I call DeleteRow for a selected item
        Then the RowRemove event should fire
        And I can handle the deletion in my code

    # Master-Detail / Expandable Rows
    Scenario: SC30 - DataGrid supports expandable row details
        Given I have a DataGrid with Template defined
        When I click the expand icon on a row
        Then a detail area should expand below the row
        And the Template should render with the row item as context
        And I can display any additional information or nested grids

    Scenario: SC31 - DataGrid expand column customization
        Given I have a DataGrid with Template and ShowExpandColumn
        When I render the DataGrid
        Then an expand/collapse icon column should appear
        And I can control which rows are expanded programmatically

    # Column Features
    Scenario: SC32 - DataGrid supports frozen columns
        Given I have columns with Frozen property set
        When I render the DataGrid with horizontal scrolling
        Then frozen columns should remain visible while scrolling
        And they should stay pinned to the left side

    Scenario: SC33 - DataGrid supports column resizing
        Given I have a DataGrid with AllowColumnResize enabled
        When I drag a column header border
        Then the column width should adjust
        And the ColumnResized event should fire

    Scenario: SC34 - DataGrid supports column reordering
        Given I have a DataGrid with AllowColumnReorder enabled
        When I drag a column header to a new position
        Then the column should move to the new position
        And the ColumnReordered event should fire

    Scenario: SC35 - DataGrid supports column picking
        Given I have a DataGrid with AllowColumnPicking enabled
        When I render the DataGrid
        Then a column picker dropdown should appear
        And I can show/hide columns by selecting them
        And hidden columns should not appear in the grid

    Scenario: SC36 - DataGrid column visibility
        Given I have columns with Visible property
        When I set a column's Visible to false
        Then that column should not render
        And I can toggle visibility programmatically

    Scenario: SC37 - DataGrid supports column formatting
        Given I have a column with FormatString specified
        When I render the DataGrid
        Then cell values should format according to the format string
        And I can use standard or custom .NET format strings

    Scenario: SC38 - DataGrid column text alignment
        Given I have columns with TextAlign property
        When I render the DataGrid
        Then cell content should align as specified (Left, Center, Right)

    # Server-Side Operations
    Scenario: SC39 - DataGrid supports server-side data loading
        Given I have a DataGrid with LoadData event handler
        When the grid initializes
        Then the LoadData event should fire
        And LoadDataArgs should include Skip, Top, OrderBy, and Filter
        When I perform sort, filter, or page operations
        Then LoadData should fire with updated parameters
        And I can fetch data from an API

    Scenario: SC40 - DataGrid loading indicator
        Given I have a DataGrid with IsLoading bound to a property
        When IsLoading is true
        Then a loading overlay should appear
        And the grid should indicate data is being fetched

    Scenario: SC41 - DataGrid server-side total count
        Given I have a DataGrid with LoadData
        When I set the Count property to the server total
        Then the pager should show correct page numbers
        And paging should work with server-side data

    # Export
    Scenario: SC42 - DataGrid export to Excel
        Given I have a DataGrid with data
        When I call the ExportToExcel method
        Then an Excel file should download
        And it should contain all visible columns and rows
        And formatting should be preserved where possible

    Scenario: SC43 - DataGrid export to CSV
        Given I have a DataGrid with data
        When I call the ExportToCSV method
        Then a CSV file should download
        And it should contain all visible data

    Scenario: SC44 - DataGrid export with custom filename
        Given I have a DataGrid
        When I call export methods with a custom filename
        Then the downloaded file should use that name

    # Virtualization
    Scenario: SC45 - DataGrid supports row virtualization
        Given I have a DataGrid with AllowVirtualization enabled
        And I have 10,000 data items
        When I render the DataGrid
        Then only visible rows should be in the DOM
        When I scroll down
        Then new rows should render dynamically
        And performance should remain smooth

    Scenario: SC46 - DataGrid virtualization overscan
        Given I have a DataGrid with virtualization
        When I set VirtualizationOverscanCount
        Then that many extra rows should render outside viewport
        And scrolling should feel smoother

    # Styling and Appearance
    Scenario: SC47 - DataGrid supports density modes
        Given I have a DataGrid with Density property
        When I set Density to Compact
        Then rows should have reduced padding
        And more data should fit in the same space

    Scenario: SC48 - DataGrid alternating row colors
        Given I have a DataGrid with AllowAlternatingRows enabled
        When I render the DataGrid
        Then odd and even rows should have different background colors
        And it should improve readability

    Scenario: SC49 - DataGrid grid lines
        Given I have a DataGrid with GridLines property
        When I set GridLines to Horizontal, Vertical, or Both
        Then appropriate border lines should appear between cells
        And the grid should have clear visual separation

    Scenario: SC50 - DataGrid responsive design
        Given I have a DataGrid with responsive settings
        When I view on different screen sizes
        Then the grid should adapt appropriately
        And I can use ShowOnMobile/ShowOnDesktop per column

    # Context Menu
    Scenario: SC51 - DataGrid context menu on rows
        Given I have a DataGrid with ContextMenu event
        When I right-click on a row
        Then the ContextMenu event should fire
        And I can display a custom context menu
        And the event args should include the row item and position

    # Keyboard Navigation
    Scenario: SC52 - DataGrid keyboard navigation
        Given I have a DataGrid with keyboard focus
        When I press arrow keys
        Then focus should move between cells
        When I press Tab
        Then focus should move to the next editable cell
        When I press Enter on a row
        Then the RowDoubleClick or RowClick event should fire

    # Row Details
    Scenario: SC53 - DataGrid row click events
        Given I have a DataGrid with RowClick event handler
        When I click on a row
        Then the RowClick event should fire with the row item

    Scenario: SC54 - DataGrid row double-click events
        Given I have a DataGrid with RowDoubleClick event handler
        When I double-click on a row
        Then the RowDoubleClick event should fire
        And I can navigate to detail view or trigger an action

    Scenario: SC55 - DataGrid row rendering customization
        Given I have a DataGrid with RowRender event
        When rows are rendered
        Then the RowRender event should fire for each row
        And I can add custom CSS classes or attributes to rows
        And I can implement conditional row styling

    # Footer
    Scenario: SC56 - DataGrid column footers
        Given I have columns with FooterTemplate
        When I render the DataGrid
        Then a footer row should appear at the bottom
        And each column's FooterTemplate should render
        And I can display aggregate values or summaries

    Scenario: SC57 - DataGrid group footers
        Given I have a grouped DataGrid with group footers
        When I render groups
        Then each group should have a footer row
        And I can display group-level aggregates

    # Column Groups (Composite Headers)
    Scenario: SC58 - DataGrid supports composite column headers
        Given I have columns with Parent property set
        When I render the DataGrid
        Then columns should group under parent headers
        And I can create multi-level header hierarchies

    # Data Refresh
    Scenario: SC59 - DataGrid manual refresh
        Given I have a DataGrid with data
        When I call the Reload method
        Then the grid should re-fetch or re-render data
        And LoadData should fire again if configured

    Scenario: SC60 - DataGrid real-time updates
        Given I have a DataGrid with an observable collection
        When the underlying collection changes
        Then the grid should automatically update
        And new/removed rows should reflect in the UI
