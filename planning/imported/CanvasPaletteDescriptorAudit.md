# Canvas Palette Descriptor Audit

## Purpose

This audit turns SPEC2100.US50 into a practical backlog for widening the Canvas page Components palette.

The Canvas palette is descriptor-driven: a component appears only when its root type carries `[Properties]`, and the right-side Parameters panel only renders `[Parameter]` values that also carry `[PropertyGrid]`. That is the right gate. The current limitation is coverage: many useful `LowlandTech.Components` primitives are implemented and documented, but are not yet descriptor-ready.

## Current Descriptor Surface

As of this pass, production descriptor discovery finds these component roots:

| Group | Components | Priority note |
|-------|------------|---------------|
| Display | Badge | Keep. Simple and useful. |
| Forms | Button, Input, Select | Keep. These are the first page-building controls. |
| Graph | GraphCanvas, GraphEdge, GraphEditor, GraphNode, GraphPort | Keep for graph-specific canvases, but do not let this category crowd the general page palette. |
| Layout | Card | Keep. This is the core container primitive. |
| Misc | ThemeProvider | Consider hiding by default until themed canvas composition is first-class. |
| Navigation | Tabs | Keep, but treat nested tab item authoring as a follow-up. |

## Prioritization Rules

Prioritize components that can be dropped onto a canvas and configured with static or simple scalar parameters.

Defer components whose useful behavior depends on any of these until the descriptor model grows matching concepts:

- `RenderFragment` slots
- `EventCallback` actions
- data sources or generic item types
- child collections
- overlays, portals, or focus traps
- service/runtime objects

## Backlog Matrix

| Component | Palette status | Priority | Descriptor work |
|-----------|----------------|----------|-----------------|
| Accordion | Missing | P1 Decorate next | Add group `Navigation`; expose `Variant`, `Size`, `Disabled`, default item/title text only if a static item model exists. |
| Alert | Missing | P1 Decorate next | Add group `Feedback`; expose `Variant`, `Title`, `Description`, optional icon. |
| AlertDialog | Missing | P3 Defer | Needs action buttons, open state, focus management, and modal semantics. |
| AspectRatio | Missing | P1 Decorate next | Add group `Layout`; expose `Ratio`, `Width`, `Height`, optional content label. |
| Avatar | Missing | P1 Decorate next | Add group `Display`; expose `ImageUrl`, `Fallback`, `Size`, `Shape`. |
| Badge | Ready | P0 Keep green | Already decorated; maintain option coverage for `Variant` and `Size`. |
| Breadcrumb | Missing | P2 Needs child/collection model | Useful, but real authoring needs item collections and links. |
| Button | Ready | P0 Keep green | Already decorated; icon options provider is a good pattern to reuse. |
| ButtonGroup | Missing | P2 Needs child/collection model | Needs grouped buttons or a child slot model. |
| Calendar | Missing | P2 Needs editor support | Needs date/range editor semantics before it is pleasant in the property grid. |
| Card | Ready | P0 Keep green | Already decorated; next gap is child-slot authoring for header/content/footer. |
| Carousel | Missing | P3 Defer | Needs item collections, navigation controls, and possibly autoplay behavior. |
| Chart | Missing | P3 Defer | Needs data source descriptors, series definitions, and chart-specific binding. |
| Checkbox | Missing | P1 Decorate next | Add group `Forms`; expose `Checked`, `Disabled`, `Label`, `Description`. |
| CodeEditor | Missing | P2 Needs editor support | Needs textarea/code editor parameter handling and language options. |
| Collapsible | Missing | P1 Decorate next | Add group `Disclosure`; expose `IsOpen`, trigger text, disabled state. |
| Combobox | Missing | P3 Defer | Needs option providers, filtering, and selected value behavior. |
| Command | Missing | P3 Defer | Needs command item collections, search data, and action descriptors. |
| ContextMenu | Missing | P3 Defer | Needs trigger/overlay composition and menu item action descriptors. |
| DataGrid | Missing | P3 Defer | Needs data sources, columns, templated cells, paging/sort/filter descriptors. |
| Dialog | Missing | P3 Defer | Needs open state, focus management, slots, actions, and overlay behavior. |
| Direction | Missing | P2 Optional | Provider-style component; expose `Dir` only if canvas supports wrapper/provider drops. |
| Drawer | Missing | P3 Defer | Same family as Dialog/Sheet; requires overlay and action semantics. |
| DropdownMenu | Missing | P3 Defer | Needs menu item collections and action descriptors. |
| Empty | Missing | P1 Decorate next | Add group `Display`; expose title, description, icon/media variant. |
| Field | Missing | P1 Decorate next | Add group `Forms`; expose label, description, error text, orientation. |
| HoverCard | Missing | P3 Defer | Needs trigger/content slots and overlay positioning. |
| Icon | Missing | P2 Needs editor support | Needs icon picker/select editor backed by the icon catalog. |
| Input | Ready | P0 Keep green | Already decorated; good example of excluding `ValueChanged`. |
| InputGroup | Missing | P2 Needs child/slot model | Needs affix/addon slots before it is more useful than Input. |
| InputOTP | Missing | P2 Needs editor support | Needs length/pattern editor and value handling. |
| Item | Missing | P1 Decorate next | Add group `Display`; expose title, description, variant, disabled. |
| Kbd | Missing | P1 Decorate next | Add group `Display`; expose key text and optional size. |
| Label | Missing | P1 Decorate next | Add group `Forms`; expose text, required, disabled, for/id if useful. |
| Menubar | Missing | P3 Defer | Needs item collections, keyboard behavior, and action descriptors. |
| NativeSelect | Missing | P2 Needs option model | Good form primitive, but needs options provider/static option model. |
| NavigationMenu | Missing | P3 Defer | Needs nested item collections and routing/action descriptors. |
| Pagination | Missing | P2 Needs action model | Static rendering is possible, but useful pagination needs callbacks/state. |
| Popover | Missing | P3 Defer | Needs trigger/content slots and overlay state. |
| Progress | Missing | P1 Decorate next | Add group `Feedback`; expose `Value`, `Max`, `Label`, `Variant`. |
| RadioGroup | Missing | P2 Needs option model | Needs static option collection support or an options provider convention. |
| Resizable | Missing | P2 Needs layout model | Useful for layout, but needs child panel composition before canvas authoring is natural. |
| ScrollArea | Missing | P1 Decorate next | Add group `Layout`; expose height/max height/orientation. |
| Select | Ready | P0 Keep green | Already decorated; use as the first test case for option-provider behavior. |
| SelectInput | Missing | P2 Needs option model | Similar to NativeSelect; wait for reusable static option descriptor support. |
| Separator | Missing | P1 Decorate next | Add group `Layout`; expose orientation, decorative, spacing. |
| Sheet | Missing | P3 Defer | Same family as Dialog/Drawer; needs overlay and focus semantics. |
| Sidebar | Missing | P3 Defer | Large application shell primitive; not a simple canvas drop target. |
| Skeleton | Missing | P1 Decorate next | Add group `Feedback`; expose shape, width, height, count. |
| Slider | Missing | P2 Needs editor support | Needs numeric range handling and possibly multi-thumb semantics. |
| Sonner | Missing | P3 Defer | Toast host/service behavior is not a static canvas component. |
| Spinner | Missing | P1 Decorate next | Add group `Feedback`; expose size, label, variant. |
| Switch | Missing | P1 Decorate next | Add group `Forms`; expose checked, disabled, label, description. |
| Table | Missing | P2 Needs child/collection model | Static table may be possible, but useful authoring needs rows/columns. |
| Tabs | Ready | P0 Keep green | Already decorated; follow-up needed for tab item collection authoring. |
| Textarea | Missing | P1 Decorate next | Add group `Forms`; expose placeholder, rows, disabled, read-only, invalid. |
| Toggle | Missing | P1 Decorate next | Add group `Forms`; expose pressed, variant, size, disabled. |
| ToggleGroup | Missing | P2 Needs option model | Needs item collection support. |
| Tooltip | Missing | P3 Defer | Needs trigger/content slots and overlay behavior. |

## Domain And System Components

These folders contain application-specific, infrastructure, or domain surfaces. They should not be swept into the general palette until a specific descriptor story says how their data/action model works.

| Area | Components | Recommendation |
|------|------------|----------------|
| Work | WorkDashboard, WorkList, WorkDetail, WorkFilterBar, WorkGraph, WorkCommandBar, WorkErrorBanner | P4. Add a Work-specific descriptor pack after data-source descriptors exist. |
| Session | SessionList, SessionCommandBar, SessionContextExplorer, SessionContextBlock, Session overlays, shipment/event components | P4. Treat as app-shell/session designer work, not core page palette work. |
| Graph | Graph components | Keep in a Graph group; optionally hide by default for non-graph page authoring. |
| ContainerGrid | ContainerGrid, ContainerGridSection | P2/P3. These are layout authoring primitives and need careful interaction with Canvas grid placement. |
| DataForm | DataForm | P3. Needs model/edit-context descriptors. |
| AppLayout, Sidebar, Explorer, Feed | App shell primitives | P3/P4. Useful for templates, but too large for the basic component palette. |
| ThemeProvider, ThemeToggle, CultureSelector | Providers/settings | P2. Only expose when provider drops and scoped settings are supported. |

## First Implementation Slice

The first slice should add descriptors for static, low-risk primitives:

1. `Separator`
2. `Textarea`
3. `Checkbox`
4. `Switch`
5. `Label`
6. `Alert`
7. `Progress`
8. `Skeleton`
9. `Spinner`
10. `Empty`
11. `Kbd`
12. `ScrollArea`
13. `AspectRatio`
14. `Avatar`
15. `Collapsible`

This gives the Canvas enough vocabulary to build a real page without solving data binding, event wiring, overlays, or slot composition first.

## Contract Follow-Ups

- Add a descriptor audit test that fails when a component listed as `P1 Decorate next` remains undecorated after the slice starts.
- Add a descriptor completeness test for label, group, editor, options, min/max, and safe default capture.
- Add a palette visibility preference for graph/provider/system groups so they do not overwhelm general page composition.
- Add a richer descriptor story for child slots and static item collections before decorating Breadcrumb, ButtonGroup, Table, ToggleGroup, and navigation/menu components.
- Add a data-source/action descriptor story before decorating Work, Session, DataGrid, Chart, DataForm, and command/menu surfaces.

---

## Navigation

[[components/_index|Back to Index]]