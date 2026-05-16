# SPEC1062 DataGrid Implementation Findings

Audit date: 2026-05-16

This README records the current implementation status of `LowlandTech.Components.DataGrid<TItem>` against the SPEC1062 user stories and the Radzen DataGrid parity target. The audit covered:

- `src/components/lib/DataGrid/DataGrid.razor`
- `src/components/lib/DataGrid/DataGridColumn.razor`
- `src/components/lib/DataGrid/DataGridModels.cs`
- `src/components/lib/wwwroot/css/input.css`
- `src/components/test/SPEC1062.DataGrid/**`

## Current implementation summary

The component has moved past the minimal grid and now supports a useful local-data grid surface:

- Column registration through `DataGridColumn<TItem>` and generated columns.
- Visible columns, custom cell templates, custom header templates, format strings, alignment, and CSS classes.
- Local sorting, including Ctrl-click multi-sort.
- Local simple filtering through a text filter row.
- Local paging with Previous and Next controls.
- Empty and loading states, including `EmptyTemplate`, `EmptyText`, and `LoadingTemplate`.
- Single and multiple row selection, including select-all for the visible page.
- Detail row expansion through `DetailTemplate<TItem>`.
- Grid-level `FooterTemplate`.
- `Density` and `GridLines` presentation attributes and CSS styling.
- Basic keyboard activation on cells with Enter and Space.

The implementation is still primarily a local, reflection-based table component. It does not yet expose the richer descriptor model, callback surface, remote data flow, editing model, grouping model, or state persistence expected for full Radzen parity.

## Implemented and tested

### US01.View

Status: implemented.

Evidence:

- `SC01_RenderDataGrid.cs` verifies root/table slots, generated columns and rows, and the empty state.
- `DataGridColumn<TItem>` resolves direct and nested property paths using reflection.
- `FormatString`, `Visible`, `Template`, `HeaderTemplate`, `Class`, `HeaderClass`, and `TextAlign` are present on columns.

Notes:

- Nested property display is supported by `DataGridColumn<TItem>.GetValue`.
- Formatting is culture-aware through `IFormattable`.

### US02.Behavior

Status: implemented for local basic behavior.

Evidence:

- `SC01_DataGridInteractions.cs` verifies simple sorting, simple filtering, and local paging.
- `DataGrid.razor` maintains internal filter text and sort descriptors.

Notes:

- Filtering is string-based and uses the column formatted value.
- Paging is local only and uses the current filtered/sorted item set.

### US04.SortingGrouping

Status: sorting partially implemented, grouping not implemented.

Evidence:

- `SC01_MultiSort.cs` verifies Ctrl-click multi-sort and normal click replacement behavior.
- `DataGridSortDescriptor` exists.

Implemented:

- Single-column local sorting.
- Multi-column local sorting.
- Sort cycling through ascending, descending, and none.

Missing:

- Group descriptors.
- Group headers and footers.
- Aggregate calculations.
- Group expand/collapse state.
- Public sort callback or controlled sort descriptors.
- Accessible `aria-sort` state on sortable headers.

### US05.ColumnManagement

Status: templates partially implemented, column management not implemented.

Evidence:

- `SC01_HeaderTemplate.cs` verifies custom header content.
- `SC02_FooterTemplate.cs` verifies grid-level footer rendering in `tfoot`.

Implemented:

- Header templates.
- Cell templates.
- Grid-level footer template.
- Visibility.
- Text alignment.
- CSS class hooks.

Missing:

- Column resizing.
- Column reordering.
- Column picker.
- Frozen columns.
- Composite/header groups.
- Per-column footer templates and aggregate footer context.

Important mismatch:

- `SC01_HeaderTemplate.UAC02_HeaderTemplatePreservesSortButton` is named as though a sortable custom header preserves the sort button, but the current assertion only confirms the other sortable column still has a sort button. The implementation replaces the sort button with `HeaderTemplate` content for that column. This should either become documented behavior or be changed so custom headers can still opt into sorting.

### US06.SelectionInteraction

Status: row selection partially implemented.

Evidence:

- `SC01_SelectAll.cs` verifies select-all selects visible rows, deselects visible rows, and reflects all-visible selection state.
- `SelectedItemChanged` and `SelectedItemsChanged` callbacks exist.

Implemented:

- Single row selection.
- Multiple row selection.
- Select all for the visible page.
- Callback snapshots for selected items.

Missing:

- Stable key-based selection.
- Shift/range selection.
- Cell selection.
- Cell context menu.
- Row reorder.
- Drag/drop between grids or external targets.

### US08.HierarchyTemplates

Status: detail rows and presentation options partially implemented.

Evidence:

- `SC01_DetailTemplate.cs` verifies detail expansion and collapse.
- `SC02_DensityGridLines.cs` verifies `data-density` and `data-grid-lines`.
- `SC03_EmptyText.cs` verifies `EmptyText` and `EmptyTemplate` precedence.

Implemented:

- One-level detail template.
- Expand/collapse button.
- Density values: `Default`, `Compact`, `Comfortable`.
- Grid line values: `None`, `Horizontal`, `Vertical`, `Both`.
- Empty template and text override.

Missing:

- Hierarchical child DataGrid composition helpers.
- On-demand hierarchy loading.
- Self-reference hierarchy helpers.
- Dynamic data and DataTable adapters.
- Conditional columns as first-class state.

## Spec-only areas

These stories currently describe desired parity behavior but do not have matching implementation in `src/components/lib/DataGrid`.

### US03.AdvancedFiltering

Status: mostly missing.

Current behavior is a simple text input per filterable visible column. The grid filters formatted strings with a case-insensitive contains check.

Missing:

- Advanced filter menus.
- Typed operators.
- Checkbox list filters.
- Enum filters.
- Filter templates and filter value templates.
- Public filter descriptors.
- LoadData-compatible serialized filter expressions.
- Distinct handling of display formatting vs raw filter values.

### US07.EditingValidation

Status: missing.

There are no edit mode parameters, edit templates, validation hooks, insert/delete commands, or row/cell edit callbacks.

### US09.RemoteVirtualization

Status: missing.

There is no `LoadData` callback, no request descriptor type, no total-count owned remote paging flow, no `IQueryable`/OData adapter, and no virtualization surface.

### US10.StateExportAccessibility

Status: partially implemented for basic keyboard behavior, otherwise mostly missing.

Implemented:

- Native table markup.
- Focusable cells.
- Enter/Space activation for selection or detail expansion.

Missing:

- Saved settings.
- Export requests.
- Pager positioning/alignment APIs.
- Roving focus model.
- Live announcements for changes.
- `aria-sort` and richer grid semantics.
- Public serializable state payload.

## Additional spec added

`US11.ControlledState/US11.ControlledState.feature` was added from this audit.

Why it was needed:

- The current grid mutates `PageIndex` internally when sorting and filtering, but does not notify `PageIndexChanged` for those resets.
- Sorting and filtering are internal-only, so remote data and Flux-style owners cannot observe descriptor changes.
- Detail expansion uses `GetHashCode()` as the row key, which is not stable enough for refreshed item instances and can collide.
- Selection is item-instance based, which is fine for local in-memory data but fragile for remote refreshes.
- Saved settings and remote loading both need a serializable state representation.

US11 now specifies controlled page, sort, filter, selection, expansion, row identity, page clamping, and serializable state behavior.

## Implementation risks and follow-up notes

- Row identity: `GetRowKey` currently returns `item.GetHashCode().ToString()` and returns a new GUID for null items. This makes expansion state unstable for refreshed rows and unsafe for null rows.
- Controlled paging: sort and filter actions reset `PageIndex` to 0 without invoking `PageIndexChanged`; parent components can drift out of sync.
- Filtering semantics: filters run against formatted display text instead of typed raw values, so results can change with culture or formatting.
- Header templates: a custom `HeaderTemplate` replaces built-in sorting UI for that column.
- Accessibility: sortable headers do not appear to expose `aria-sort`, and change announcements are not present.
- Performance: filtered/sorted items are recomputed in multiple places during render, which is acceptable for small local lists but likely expensive for larger data.
- Glyph encoding: the expanded detail button currently renders mojibake instead of a minus glyph. Prefer ASCII `-` or a correctly encoded minus symbol.

## Recommended next implementation order

1. US11 controlled state and row keys.
2. US03 filter descriptors and typed filtering.
3. US09 `LoadData` request/response model.
4. US10 accessibility descriptors and saved settings.
5. US05 column resize/reorder/picker.
6. US04 grouping and aggregates.
7. US07 editing and validation.
8. US06 advanced interaction features.

This order keeps the state model stable before adding remote data, persistence, grouping, and editing on top.
